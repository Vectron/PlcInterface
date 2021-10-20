using System;
using System.Collections.Generic;

namespace Opc.Ua.Client
{
    /// <summary>
    /// Extension methods for <see cref="Session"/>.
    /// </summary>
    internal static class SessionExtensions
    {
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
    }
}