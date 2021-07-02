using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;

namespace PlcInterface.OpcUa
{
    public class SymbolHandler : ISymbolHandler, IDisposable
    {
        private readonly Dictionary<string, ISymbolInfo> allSymbols = new Dictionary<string, ISymbolInfo>();
        private readonly IPlcConnection<Session> client;
        private readonly CompositeDisposable disposables = new CompositeDisposable();
        private readonly ILogger logger;
        private bool disposedValue = false;
        private SymbolInfo rootNode;
        private Session session;

        public SymbolHandler(IPlcConnection<Session> client, ILogger<SymbolHandler> logger)
        {
            this.client = client;
            this.logger = logger;
            disposables.Add(client.SessionStream.Subscribe(x =>
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

        public IReadOnlyCollection<ISymbolInfo> AllSymbols
            => allSymbols.Values;

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        public ISymbolInfo GetSymbolinfo(string ioName)
        {
            allSymbols.TryGetValue(ioName.ToLower(), out ISymbolInfo value);

            if (value == null)
            {
                throw new SymbolException($"{ioName} Does not excist in the PLC");
            }

            return value;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    disposables.Dispose();
                    allSymbols.Clear();
                    rootNode = null;
                    session = null;
                }

                disposedValue = true;
            }
        }

        private static void GetElements(Session session, Browser browser, uint level, ReferenceDescriptionCollection references)
        {
            var spaces = string.Empty;
            for (int i = 0; i <= level; i++)
            {
                spaces += "   ";
            }

            // Iterate through the references and print the variables
            foreach (ReferenceDescription reference in references)
            {
                // make sure the type definition is in the cache.
                session.NodeCache.Find(reference.ReferenceTypeId);
                switch (reference.NodeClass)
                {
                    case NodeClass.Object:
                        Console.WriteLine(spaces + "+ " + reference.DisplayName);
                        break;

                    default:
                        Console.WriteLine(spaces + "- " + reference.DisplayName);
                        break;
                }

                var subReferences = browser.Browse((NodeId)reference.NodeId);
                level += 1;
                GetElements(session, browser, level, subReferences);
                level -= 1;
            }
        }

        private SymbolInfo AddSymbol(ReferenceDescription description, string fullName)
        {
            var symbol = new SymbolInfo(description, fullName.Replace("\"", string.Empty));
            if (!allSymbols.ContainsKey(symbol.NameLower))
            {
                allSymbols.Add(symbol.NameLower, symbol);
            }

            return symbol;
        }

        private void BuildSymbolList(IDictionary<SymbolInfo, ReferenceDescriptionCollection> items, Browser browser)
        {
            foreach (var kv in items)
            {
                var parrent = kv.Key;
                var itemsToBrowseNext = new List<SymbolInfo>();

                foreach (var item in kv.Value)
                {
                    var arrayIndex = item.BrowseName.Name.IndexOf('[');
                    var itemName = arrayIndex == -1 ? $"{parrent.Name}.{item.BrowseName.Name}" : $"{parrent.Name}{item.BrowseName.Name.Substring(arrayIndex)}";
                    itemName = CleanName(itemName);
                    var symbol = AddSymbol(item, itemName);
                    parrent.ChildSymbols.Add(symbol.Name);
                    itemsToBrowseNext.Add(symbol);
                }

                if (itemsToBrowseNext.Count > 0)
                {
                    var browseResult = browser.Browse(itemsToBrowseNext);
                    BuildSymbolList(browseResult, browser);
                }
            }
        }

        private string CleanName(string uri)
        {
            var splitChar = '.';
            var rootName = rootNode.NameLower;
            if (string.IsNullOrWhiteSpace(rootName))
            {
                return uri.Trim(splitChar);
            }

            return uri.Replace(rootName, string.Empty).Trim(splitChar);
        }

        private void UpdateRootNode(Browser browser)
        {
            var lastNode = new ReferenceDescription() { NodeId = ObjectIds.ObjectsFolder, NodeClass = NodeClass.Object };
            if (!(client.Settings is OPCSettings settings))
            {
                throw new NullReferenceException("No vallid settings found");
            }

            var path = settings.Address.AbsolutePath.Trim('/');
            var rootNodeName = string.Empty;

            if (!string.IsNullOrWhiteSpace(path))
            {
                var pathParts = path.Split('/');

                foreach (var item in pathParts)
                {
                    var subNode = browser.Browse((NodeId)lastNode.NodeId).Where(x => x.BrowseName.Name.Trim('"').Equals(item)).FirstOrDefault();

                    if (subNode == null)
                    {
                        continue;
                    }

                    lastNode = subNode;
                }
            }

            rootNode = AddSymbol(lastNode, rootNodeName);
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
            UpdateRootNode(browser);

            // get the nodes in the root
            var result = browser.Browse(rootNode.Handle);

            allSymbols.Clear();
            var dictResult = new Dictionary<SymbolInfo, ReferenceDescriptionCollection>
            {
                { rootNode, result }
            };

            BuildSymbolList(dictResult, browser);
        }
    }
}