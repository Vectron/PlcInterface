using System;
using System.Collections.Generic;
using PlcInterface.OpcUa;

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

            var operationLimits = browser.Browse(ObjectIds.Server_ServerCapabilities_OperationLimits);
            var limits = new Dictionary<string, uint>(StringComparer.Ordinal);

            foreach (var item in operationLimits)
            {
                var dataValue = session?.ReadValue((NodeId)item.NodeId);
                if (dataValue?.Value is uint value)
                {
                    limits.Add(item.BrowseName.Name, value);
                }
            }

            return new OperationLimits(limits);
        }
    }
}