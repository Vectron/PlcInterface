using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Disposables;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Client;

namespace PlcInterface.OpcUa
{
    /// <summary>
    /// Implementation of <see cref="ISymbolHandler"/>.
    /// </summary>
    public class SymbolHandler : IOpcSymbolHandler, IDisposable
    {
        private readonly Dictionary<string, SymbolInfo> allSymbols = new(StringComparer.OrdinalIgnoreCase);
        private readonly IOpcPlcConnection connection;
        private readonly CompositeDisposable disposables = new();
        private readonly ILogger logger;
        private bool disposedValue;
        private Session? session;

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolHandler"/> class.
        /// </summary>
        /// <param name="connection">A <see cref="IPlcConnection{T}"/> implementation.</param>
        /// <param name="logger">A <see cref="ILogger"/> implementation.</param>
        public SymbolHandler(IOpcPlcConnection connection, ILogger<SymbolHandler> logger)
        {
            this.connection = connection;
            this.logger = logger;
            disposables.Add(connection.SessionStream.Subscribe(x =>
            {
                if (x.IsConnected)
                {
                    session = x.Value;
                    UpdateSymbols();
                }
                else
                {
                    session = null;
                }
            }));
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<ISymbolInfo> AllSymbols
            => allSymbols.Values;

        /// <inheritdoc/>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public ISymbolInfo GetSymbolinfo(string ioName)
        {
            if (!allSymbols.TryGetValue(ioName.ToLower(CultureInfo.InvariantCulture), out var value))
            {
                throw new SymbolException($"{ioName} Does not excist in the PLC");
            }

            return value;
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">Value indicating if we need to cleanup managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    disposables.Dispose();
                    allSymbols.Clear();
                    session = null;
                }

                disposedValue = true;
            }
        }

        private static string CleanName(string uri, SymbolInfo rootNode)
        {
            var splitChar = '.';
            var rootName = rootNode?.NameLower;
            if (string.IsNullOrWhiteSpace(rootName))
            {
                return uri.Trim(splitChar);
            }

            return uri.Replace(rootName, string.Empty).Trim(splitChar);
        }

        private static NodeInfo NodeInfoFromReadResult(DataValueCollection results, Session session)
        {
            var nodeInfo = new NodeInfo
            {
                Description = results[0].GetValue(LocalizedText.Null).Text,
                DataType = results[1].GetValue(NodeId.Null),
                ValueRank = results[2].GetValue(ValueRanks.Any),
            };

            if (!NodeId.IsNull(nodeInfo.DataType))
            {
                nodeInfo.BuiltInType = DataTypes.GetBuiltInType(nodeInfo.DataType, session.TypeTree);
                nodeInfo.DataTypeDisplayText = session.NodeCache.GetDisplayText(nodeInfo.DataType);

                if (nodeInfo.ValueRank >= 0)
                {
                    nodeInfo.DataTypeDisplayText += "[]";
                }

                if (nodeInfo.BuiltInType == BuiltInType.Enumeration)
                {
                    var nodesToRead2 = new ReadValueIdCollection()
                    {
                        new ReadValueId
                        {
                            NodeId = nodeInfo.DataType,
                            AttributeId = Attributes.DataTypeDefinition,
                        },
                    };

                    _ = session.Read(
                        null,
                        0,
                        TimestampsToReturn.Neither,
                        nodesToRead2,
                        out var results2,
                        out var diagnosticInfos2);

                    ClientBase.ValidateResponse(results2, nodesToRead2);
                    ClientBase.ValidateDiagnosticInfos(diagnosticInfos2, nodesToRead2);
                }
            }

            return nodeInfo;
        }

        private SymbolInfo AddSymbol(ReferenceDescription description, string fullName)
        {
            var name = fullName.Replace("\"", string.Empty);
            if (!allSymbols.TryGetValue(name.ToLower(CultureInfo.InvariantCulture), out var symbol))
            {
                var nodeInfo = ReadNodeInfo((NodeId)description.NodeId);
                symbol = new SymbolInfo(description, name, nodeInfo);
                allSymbols.Add(symbol.NameLower, symbol);
            }

            return symbol;
        }

        private void BuildSymbolList(IDictionary<SymbolInfo, ReferenceDescriptionCollection> items, Browser browser, SymbolInfo rootNode)
        {
            foreach (var kv in items)
            {
                var parrent = kv.Key;
                var itemsToBrowseNext = new List<SymbolInfo>();

                foreach (var item in kv.Value)
                {
                    var arrayIndex = item.BrowseName.Name.IndexOf('[');
                    var itemName = arrayIndex == -1 ? $"{parrent.Name}.{item.BrowseName.Name}" : $"{parrent.Name}{item.BrowseName.Name.Substring(arrayIndex)}";
                    itemName = CleanName(itemName, rootNode);
                    var symbol = AddSymbol(item, itemName);
                    parrent.ChildSymbols.Add(symbol.Name);
                    itemsToBrowseNext.Add(symbol);
                }

                if (itemsToBrowseNext.Count > 0)
                {
                    var browseResult = browser.Browse(itemsToBrowseNext);
                    BuildSymbolList(browseResult, browser, rootNode);
                }
            }
        }

        private SymbolInfo CreateRootNode(Browser browser)
        {
            var lastNode = new ReferenceDescription() { NodeId = ObjectIds.ObjectsFolder, NodeClass = NodeClass.Object };
            if (connection.Settings is not OPCSettings settings)
            {
                throw new InvalidOperationException("No vallid settings found");
            }

            var path = settings.Address?.AbsolutePath.Trim('/').Replace("%20", " ");
            var rootNodeName = string.Empty;

            if (!string.IsNullOrWhiteSpace(path))
            {
                var pathParts = path!.Split('/');

                foreach (var item in pathParts)
                {
                    var subNode = browser.Browse((NodeId)lastNode.NodeId).FirstOrDefault(x => x.BrowseName.Name.Trim('"').Equals(item, StringComparison.Ordinal));

                    if (subNode == null)
                    {
                        continue;
                    }

                    lastNode = subNode;
                }
            }

            return AddSymbol(lastNode, rootNodeName);
        }

        /// <summary>
        /// Gets the datatype of an OPC tag.
        /// </summary>
        /// <param name="nodeId"><see cref="NodeId"/> to get datatype of.</param>
        /// <returns>System Type.</returns>
        private NodeInfo ReadNodeInfo(NodeId nodeId)
        {
            if (session == null)
            {
                throw new InvalidOperationException("Session is null");
            }

            var nodesToRead = new ReadValueIdCollection()
            {
                new ReadValueId
                {
                    NodeId = nodeId,
                    AttributeId = Attributes.Description,
                },
                new ReadValueId
                {
                    NodeId = nodeId,
                    AttributeId = Attributes.DataType,
                },
                new ReadValueId
                {
                    NodeId = nodeId,
                    AttributeId = Attributes.ValueRank,
                },
            };

            _ = session.Read(
                null,
                0,
                TimestampsToReturn.Neither,
                nodesToRead,
                out var results,
                out var diagnosticInfos);

            ClientBase.ValidateResponse(results, nodesToRead);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);

            var nodeInfo = NodeInfoFromReadResult(results, session);

            return nodeInfo;
        }

        private void UpdateSymbols()
        {
            logger.LogInformation("Updating Symbols");

            // create a browser to browse the node tree
            var browser = new Browser(session)
            {
                BrowseDirection = BrowseDirection.Forward,
                ReferenceTypeId = ReferenceTypeIds.HierarchicalReferences,
                IncludeSubtypes = true,
                NodeClassMask = 0,
                ContinueUntilDone = false,
            };

            // update the root node to the path in settings
            var rootNode = CreateRootNode(browser);

            // get the nodes in the root
            var result = browser.Browse(rootNode.Handle);

            allSymbols.Clear();
            var dictResult = new Dictionary<SymbolInfo, ReferenceDescriptionCollection>
            {
                { rootNode, result },
            };

            BuildSymbolList(dictResult, browser, rootNode);
        }
    }
}