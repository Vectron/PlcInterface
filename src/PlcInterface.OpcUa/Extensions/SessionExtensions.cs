using System;
using System.Collections.Generic;
using System.Linq;
using Opc.Ua;
using Opc.Ua.Client;

#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace PlcInterface.OpcUa;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Extension methods for <see cref="Session"/>.
/// </summary>
internal static class SessionExtensions
{
    /// <summary>
    /// Gets the datatype of an OPC tag.
    /// </summary>
    /// /// <param name="session">The <see cref="Session"/> to read from.</param>
    /// <param name="nodeId"><see cref="NodeId"/> to get datatype of.</param>
    /// <returns>System Type.</returns>
    public static NodeInfo ReadNodeInfo(this Session session, NodeId nodeId)
    {
        if (session == null)
        {
            throw new InvalidOperationException("Session is null");
        }

        var nodesToRead = new ReadValueIdCollection(GetReadValueIds(nodeId));
        _ = session.Read(
            null,
            0,
            TimestampsToReturn.Neither,
            nodesToRead,
            out var results,
            out var diagnosticInfos);

        ClientBase.ValidateResponse(results, nodesToRead);
        ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);
        var nodeInfo = NodeInfoFromDataValue(results, session);
        return nodeInfo;
    }

    /// <summary>
    /// Gets the datatype of an OPC tag.
    /// </summary>
    /// <param name="session">The <see cref="Session"/> to read from.</param>
    /// <param name="nodeIds">The <see cref="NodeId"/> to get datatype of.</param>
    /// <returns>System Type.</returns>
    public static IEnumerable<NodeInfo> ReadNodeInfo(this Session session, IEnumerable<NodeId> nodeIds)
    {
        if (session == null)
        {
            throw new InvalidOperationException("Session is null");
        }

        var nodesToRead = new ReadValueIdCollection(nodeIds.SelectMany(GetReadValueIds));

        if (nodesToRead.Count <= 0)
        {
            return Enumerable.Empty<NodeInfo>();
        }

        try
        {
            _ = session.Read(
                null,
                0,
                TimestampsToReturn.Neither,
                nodesToRead,
                out var results,
                out var diagnosticInfos);
            ClientBase.ValidateResponse(results, nodesToRead);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);
            var nodeInfos = results.Chunk(3).Select(x => NodeInfoFromDataValue(x, session));
            return nodeInfos;
        }
        catch (ServiceResultException ex)
        {
            if (ex.Result.StatusCode == StatusCodes.BadEncodingLimitsExceeded)
            {
                var chunkSize = ((nodesToRead.Count / 3) >> 1) + 1;
                return nodeIds.Chunk(chunkSize).SelectMany(x => session.ReadNodeInfo(x));
            }

            throw;
        }
    }

    /// <summary>
    /// Reads the Operation Limits from the server.
    /// </summary>
    /// <param name="session">The <see cref="Session"/> to read from.</param>
    /// <returns>The read <see cref="OperationLimits"/>.</returns>
    public static OperationLimits ReadOperationLimits(this Session session)
    {
        // create a browser to browse the node tree
        var browser = new Browser(session)
        {
            BrowseDirection = BrowseDirection.Forward,
            ReferenceTypeId = ReferenceTypeIds.HierarchicalReferences,
            IncludeSubtypes = true,
            NodeClassMask = 0,
            ContinueUntilDone = false,
        };

        var operationLimitsValues = browser.Browse(ObjectIds.Server_ServerCapabilities_OperationLimits);
        var limits = new Dictionary<string, uint>(StringComparer.Ordinal);

        foreach (var item in operationLimitsValues)
        {
            var dataValue = session?.ReadValue((NodeId)item.NodeId);
            if (dataValue?.Value is uint uintValue)
            {
                limits.Add(item.BrowseName.Name, uintValue);
            }
        }

        var operationLimits = new OperationLimits
        {
            MaxMonitoredItemsPerCall = limits.TryGetValue(nameof(OperationLimits.MaxMonitoredItemsPerCall), out var value) ? value : uint.MaxValue,
            MaxNodesPerBrowse = limits.TryGetValue(nameof(OperationLimits.MaxNodesPerBrowse), out value) ? value : uint.MaxValue,
            MaxNodesPerHistoryReadData = limits.TryGetValue(nameof(OperationLimits.MaxNodesPerHistoryReadData), out value) ? value : uint.MaxValue,
            MaxNodesPerHistoryReadEvents = limits.TryGetValue(nameof(OperationLimits.MaxNodesPerHistoryReadEvents), out value) ? value : uint.MaxValue,
            MaxNodesPerHistoryUpdateData = limits.TryGetValue(nameof(OperationLimits.MaxNodesPerHistoryUpdateData), out value) ? value : uint.MaxValue,
            MaxNodesPerHistoryUpdateEvents = limits.TryGetValue(nameof(OperationLimits.MaxNodesPerHistoryUpdateEvents), out value) ? value : uint.MaxValue,
            MaxNodesPerMethodCall = limits.TryGetValue(nameof(OperationLimits.MaxNodesPerMethodCall), out value) ? value : uint.MaxValue,
            MaxNodesPerNodeManagement = limits.TryGetValue(nameof(OperationLimits.MaxNodesPerNodeManagement), out value) ? value : uint.MaxValue,
            MaxNodesPerRead = limits.TryGetValue(nameof(OperationLimits.MaxNodesPerRead), out value) ? value : uint.MaxValue,
            MaxNodesPerRegisterNodes = limits.TryGetValue(nameof(OperationLimits.MaxNodesPerRegisterNodes), out value) ? value : uint.MaxValue,
            MaxNodesPerTranslateBrowsePathsToNodeIds = limits.TryGetValue(nameof(OperationLimits.MaxNodesPerTranslateBrowsePathsToNodeIds), out value) ? value : uint.MaxValue,
            MaxNodesPerWrite = limits.TryGetValue(nameof(OperationLimits.MaxNodesPerWrite), out value) ? value : uint.MaxValue,
        };

        return operationLimits;
    }

    private static IEnumerable<ReadValueId> GetReadValueIds(NodeId nodeId)
    {
        yield return new ReadValueId
        {
            NodeId = nodeId,
            AttributeId = Attributes.Description,
        };

        yield return new ReadValueId
        {
            NodeId = nodeId,
            AttributeId = Attributes.DataType,
        };

        yield return new ReadValueId
        {
            NodeId = nodeId,
            AttributeId = Attributes.ValueRank,
        };
    }

    private static NodeInfo NodeInfoFromDataValue(IList<DataValue> dataValues, Session session)
    {
        if (dataValues.Count != 3)
        {
            throw new ArgumentException("Collection needs 3 items", nameof(dataValues));
        }

        var description = dataValues[0].GetValue(LocalizedText.Null).Text;
        var dataType = dataValues[1].GetValue(NodeId.Null);
        var valueRank = dataValues[2].GetValue(ValueRanks.Any);
        return new NodeInfo(session, dataType, description, valueRank);
    }
}