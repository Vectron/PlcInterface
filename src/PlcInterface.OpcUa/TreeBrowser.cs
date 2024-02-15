using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Client;

namespace PlcInterface.OpcUa;

/// <summary>
/// Extension of <see cref="Browser"/> to recursive browse symbols.
/// </summary>
internal sealed partial class TreeBrowser : Browser
{
    private readonly ILogger logger;
    private readonly OperationLimits operationLimits;

    /// <summary>
    /// Initializes a new instance of the <see cref="TreeBrowser"/> class.
    /// </summary>
    /// <param name="session">The session to browse.</param>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    public TreeBrowser(ISession session, ILogger logger)
        : base(session)
    {
        BrowseDirection = BrowseDirection.Forward;
        ReferenceTypeId = ReferenceTypeIds.HierarchicalReferences;
        IncludeSubtypes = true;
        NodeClassMask = 0;
        ContinueUntilDone = true;
        operationLimits = Session.ReadOperationLimits();
        this.logger = logger;
    }

    /// <summary>
    /// Browses the specified symbols.
    /// </summary>
    /// <param name="nodes">A <see cref="IEnumerable{T}"/> of <see cref="NodeId"/> to browse.</param>
    /// <returns>A <see cref="IDictionary{TKey, TValue}"/> with the browse result for every symbol.</returns>
    public IEnumerable<ReferenceDescriptionCollection> Browse(IEnumerable<NodeId> nodes)
    {
        if (Session == null)
        {
            throw new ServiceResultException(StatusCodes.BadServerNotConnected, "Cannot browse if not connected to a server.");
        }

        var nodesToBrowse = new BrowseDescriptionCollection(nodes.Select(BrowseDescriptionFromNodeId));

        if (nodesToBrowse.Count <= 0)
        {
            return [];
        }

        var chunkSize = nodesToBrowse.Count + 1;
        while (true)
        {
            try
            {
                return nodesToBrowse
                    .Chunk(chunkSize)
                    .SelectMany(x => CallServer(x))
                    .ToImmutableArray();
            }
            catch (ServiceResultException ex)
            {
                if (ex.StatusCode is not StatusCodes.BadResponseTooLarge
                    and not StatusCodes.BadEncodingLimitsExceeded)
                {
                    return [];
                }

                var previous = chunkSize;
                chunkSize /= 2;
                LogBrowseFailed(ex.StatusCode, previous, chunkSize);
            }
        }

        IEnumerable<ReferenceDescriptionCollection> CallServer(BrowseDescriptionCollection nodesToBrowse)
        {
            // make the call to the server.
            var responseHeader = Session.Browse(
                requestHeader: null,
                View,
                MaxReferencesReturned,
                nodesToBrowse,
                out var results,
                out var diagnosticInfos);

            // ensure that the server returned valid results.
            ClientBase.ValidateResponse(results, nodesToBrowse);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToBrowse);
            return DecodeResult(responseHeader, results, diagnosticInfos);
        }
    }

    /// <summary>
    /// Browse the symbol tree starting from the root node.
    /// </summary>
    /// <param name="rootNodePath">The path to start the tree at.</param>
    /// <returns>All found symbols on the server.</returns>
    public IDictionary<string, IOpcSymbolInfo> BrowseTree(string rootNodePath)
    {
        var path = rootNodePath.Trim('/').Replace("%20", " ", StringComparison.OrdinalIgnoreCase);
        var rootNode = CreateRootNodeSymbol(path);
        var treeStart = new[] { rootNode };
        return treeStart
            .Concat(BrowseRecursive(treeStart, rootNode.Name))
            .DistinctBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
    }

    private static string CleanName(string uri, string rootName)
    {
        var splitChar = '.';
        if (string.IsNullOrWhiteSpace(rootName))
        {
            return uri.Trim(splitChar);
        }

        return uri.Replace(rootName, string.Empty, StringComparison.OrdinalIgnoreCase).Trim(splitChar);
    }

    private static SymbolInfo CreateSymbol(ReferenceDescription description, NodeInfo nodeInfo, IOpcSymbolInfo? parent, string rootName)
    {
        var nameBuilder = new StringBuilder();
        var parentName = parent == null ? [] : parent.Name.AsSpan();
        var arrayIndex = description.BrowseName.Name.IndexOf('[', StringComparison.OrdinalIgnoreCase);
        ReadOnlySpan<char> itemName;
        ReadOnlySpan<char> arrayIndexString;
        if (arrayIndex == -1)
        {
            itemName = description.BrowseName.Name.AsSpan();
            arrayIndexString = [];
        }
        else
        {
            itemName = description.BrowseName.Name.AsSpan(0, arrayIndex);
            arrayIndexString = description.BrowseName.Name.AsSpan(arrayIndex);
        }

        _ = nameBuilder.Append(parentName);
        if (arrayIndex == -1)
        {
            if (!parentName.IsEmpty)
            {
                _ = nameBuilder.Append('.');
            }

            _ = nameBuilder.Append(itemName);
        }

        var fullName = nameBuilder.Append(arrayIndexString).ToString();
        fullName = CleanName(fullName, rootName).Replace("\"", string.Empty, StringComparison.OrdinalIgnoreCase);
        parent?.ChildSymbols.Add(fullName);
        return new SymbolInfo(description, fullName, nodeInfo);
    }

    private BrowseDescription BrowseDescriptionFromNodeId(NodeId nodeId)
            => new()
            {
                NodeId = nodeId,
                BrowseDirection = BrowseDirection,
                ReferenceTypeId = ReferenceTypeId,
                IncludeSubtypes = IncludeSubtypes,
                NodeClassMask = (uint)NodeClassMask,
                ResultMask = ResultMask,
            };

    /// <summary>
    /// Fetches the next batch of references.
    /// </summary>
    /// <param name="continuationPoint">The continuation point.</param>
    /// <param name="cancel">if set to <see langword="true"/> the browse operation is cancelled.</param>
    /// <returns>The next batch of references.</returns>
    private ReferenceDescriptionCollection BrowseNext(ref byte[] continuationPoint, bool cancel)
    {
        var continuationPoints = new ByteStringCollection
            {
                continuationPoint,
            };

        // make the call to the server.
        var responseHeader = Session.BrowseNext(
            requestHeader: null,
            cancel,
            continuationPoints,
            out var results,
            out var diagnosticInfos);

        // ensure that the server returned valid results.
        ClientBase.ValidateResponse(results, continuationPoints);
        ClientBase.ValidateDiagnosticInfos(diagnosticInfos, continuationPoints);

        // check if valid.
        if (StatusCode.IsBad(results[0].StatusCode))
        {
            throw ServiceResultException.Create(results[0].StatusCode, 0, diagnosticInfos, responseHeader.StringTable);
        }

        // update continuation point.
        continuationPoint = results[0].ContinuationPoint;

        // return references.
        return results[0].References;
    }

    private IEnumerable<IOpcSymbolInfo> BrowseRecursive(IEnumerable<IOpcSymbolInfo> symbols, string rootName)
    {
        var browseResult = symbols
            .Select(x => x.Handle)
            .Chunk((int)operationLimits.MaxNodesPerBrowse)
            .SelectMany(Browse)
            .ToArray();

        var nodeInfos = browseResult
            .SelectMany(x => x)
            .Select(x => (NodeId)x.NodeId)
            .Chunk((int)operationLimits.MaxNodesPerRead)
            .SelectMany(x => Session.ReadNodeInfo(x, logger));

        var allSymbols = browseResult
           .Zip(symbols, (referenceDescriptions, parent) => (referenceDescriptions, parent))
           .SelectMany(x => x.referenceDescriptions.Select(referenceDescription => (referenceDescription, x.parent)))
           .Zip(nodeInfos, (rp, nodeInfo) => (rp.referenceDescription, rp.parent, nodeInfo))
           .Where(x => x.parent.BuiltInType != BuiltInType.String)
           .Select(x => CreateSymbol(x.referenceDescription, x.nodeInfo, x.parent, rootName))
           .ToList();

        if (allSymbols.Count <= 0)
        {
            return [];
        }

        return allSymbols.Concat(BrowseRecursive(allSymbols, rootName));
    }

    private SymbolInfo CreateRootNodeSymbol(string path)
    {
        var lastNode = new ReferenceDescription() { NodeId = ObjectIds.ObjectsFolder, NodeClass = NodeClass.Object, BrowseName = string.Empty };
        var rootNodeName = string.Empty;

        if (!string.IsNullOrWhiteSpace(path))
        {
            var pathParts = path!.Split('.');

            foreach (var item in pathParts)
            {
                var subNode = Browse((NodeId)lastNode.NodeId)
                    .FirstOrDefault(x => x.BrowseName.Name.Trim('"').Equals(item, StringComparison.OrdinalIgnoreCase));

                if (subNode == null)
                {
                    continue;
                }

                lastNode = subNode;
            }
        }

        var nodeInfo = Session.ReadNodeInfo((NodeId)lastNode.NodeId);
        return CreateSymbol(lastNode, nodeInfo, parent: null, string.Empty);
    }

    private IEnumerable<ReferenceDescriptionCollection> DecodeResult(ResponseHeader responseHeader, BrowseResultCollection results, DiagnosticInfoCollection diagnosticInfos)
    {
        var browseResults = new List<ReferenceDescriptionCollection>(results.Count);

        for (var i = 0; i < results.Count; i++)
        {
            var references = new ReferenceDescriptionCollection();
            var item = results[i];

            // check if valid.
            if (StatusCode.IsBad(item.StatusCode))
            {
                throw ServiceResultException.Create(item.StatusCode, 0, diagnosticInfos, responseHeader.StringTable);
            }

            references.AddRange(item.References);
            var continuationPoint = item.ContinuationPoint;
            while (continuationPoint != null)
            {
                ReferenceDescriptionCollection additionalReferences;

                if (!ContinueUntilDone)
                {
                    _ = BrowseNext(ref continuationPoint, cancel: true);
                    return browseResults;
                }

                additionalReferences = BrowseNext(ref continuationPoint, cancel: false);
                if (additionalReferences != null && additionalReferences.Count > 0)
                {
                    references.AddRange(additionalReferences);
                    continue;
                }

                Utils.Trace("Continuation point exists, but the browse results are null/empty.");
                break;
            }

            browseResults.Add(references);
        }

        // return the results.
        return browseResults.AsEnumerable();
    }
}
