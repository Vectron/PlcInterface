using System.Collections.Generic;

namespace PlcInterface.OpcUa
{
    /// <summary>
    /// Type containing the server operation limits.
    /// </summary>
    internal sealed class OperationLimits
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationLimits"/> class.
        /// </summary>
        /// <param name="values">The values to create this <see cref="OperationLimits"/> from.</param>
        public OperationLimits(IDictionary<string, uint> values)
        {
            MaxMonitoredItemsPerCall = values.TryGetValue(nameof(MaxMonitoredItemsPerCall), out var value) ? value : uint.MaxValue;
            MaxNodesPerBrowse = values.TryGetValue(nameof(MaxNodesPerBrowse), out value) ? value : uint.MaxValue;
            MaxNodesPerHistoryReadData = values.TryGetValue(nameof(MaxNodesPerHistoryReadData), out value) ? value : uint.MaxValue;
            MaxNodesPerHistoryReadEvents = values.TryGetValue(nameof(MaxNodesPerHistoryReadEvents), out value) ? value : uint.MaxValue;
            MaxNodesPerHistoryUpdateData = values.TryGetValue(nameof(MaxNodesPerHistoryUpdateData), out value) ? value : uint.MaxValue;
            MaxNodesPerHistoryUpdateEvents = values.TryGetValue(nameof(MaxNodesPerHistoryUpdateEvents), out value) ? value : uint.MaxValue;
            MaxNodesPerMethodCall = values.TryGetValue(nameof(MaxNodesPerMethodCall), out value) ? value : uint.MaxValue;
            MaxNodesPerNodeManagement = values.TryGetValue(nameof(MaxNodesPerNodeManagement), out value) ? value : uint.MaxValue;
            MaxNodesPerRead = values.TryGetValue(nameof(MaxNodesPerRead), out value) ? value : uint.MaxValue;
            MaxNodesPerRegisterNodes = values.TryGetValue(nameof(MaxNodesPerRegisterNodes), out value) ? value : uint.MaxValue;
            MaxNodesPerTranslateBrowsePathsToNodeIds = values.TryGetValue(nameof(MaxNodesPerTranslateBrowsePathsToNodeIds), out value) ? value : uint.MaxValue;
            MaxNodesPerWrite = values.TryGetValue(nameof(MaxNodesPerWrite), out value) ? value : uint.MaxValue;
        }

        /// <summary>
        /// Gets the value for Max Monitored Items Per Call (default is <see cref="uint.MaxValue"/>).
        /// </summary>
        public uint MaxMonitoredItemsPerCall
        {
            get;
        }

        /// <summary>
        /// Gets the value for Max Nodes Per Browse (default is <see cref="uint.MaxValue"/>).
        /// </summary>
        public uint MaxNodesPerBrowse
        {
            get;
        }

        /// <summary>
        /// Gets the value for Max Nodes Per Browse (default is <see cref="uint.MaxValue"/>).
        /// </summary>
        public uint MaxNodesPerHistoryReadData
        {
            get;
        }

        /// <summary>
        /// Gets the value for Max Nodes Per History Read Events (default is <see cref="uint.MaxValue"/>).
        /// </summary>
        public uint MaxNodesPerHistoryReadEvents
        {
            get;
        }

        /// <summary>
        /// Gets the value for Max Nodes Per History Update Data (default is <see cref="uint.MaxValue"/>).
        /// </summary>
        public uint MaxNodesPerHistoryUpdateData
        {
            get;
        }

        /// <summary>
        /// Gets the value for Max Nodes Per History Update Events (default is <see cref="uint.MaxValue"/>).
        /// </summary>
        public uint MaxNodesPerHistoryUpdateEvents
        {
            get;
        }

        /// <summary>
        /// Gets the value for Max Nodes Per Method Call (default is <see cref="uint.MaxValue"/>).
        /// </summary>
        public uint MaxNodesPerMethodCall
        {
            get;
        }

        /// <summary>
        /// Gets the value for Max Nodes Per Node Management (default is <see cref="uint.MaxValue"/>).
        /// </summary>
        public uint MaxNodesPerNodeManagement
        {
            get;
        }

        /// <summary>
        /// Gets the value for Max Nodes Per Read (default is <see cref="uint.MaxValue"/>).
        /// </summary>
        public uint MaxNodesPerRead
        {
            get;
        }

        /// <summary>
        /// Gets the value for Max Nodes Per Register Nodes (default is <see cref="uint.MaxValue"/>).
        /// </summary>
        public uint MaxNodesPerRegisterNodes
        {
            get;
        }

        /// <summary>
        /// Gets the value for Max Nodes Per Translate Browse Paths To Node Ids (default is <see cref="uint.MaxValue"/>).
        /// </summary>
        public uint MaxNodesPerTranslateBrowsePathsToNodeIds
        {
            get;
        }

        /// <summary>
        /// Gets the value for Max Nodes Per Write (default is <see cref="uint.MaxValue"/>).
        /// </summary>
        public uint MaxNodesPerWrite
        {
            get;
        }
    }
}