using PlcInterface.OpcUa;
using System.Collections.Generic;

namespace Opc.Ua.Client
{
    internal static class BrowseExtension
    {
        public static IDictionary<SymbolInfo, ReferenceDescriptionCollection> Browse(this Browser browser, IList<SymbolInfo> nodes)
        {
            if (browser.Session == null)
            {
                throw new ServiceResultException(StatusCodes.BadServerNotConnected, "Cannot browse if not connected to a server.");
            }

            var nodesToBrowse = new BrowseDescriptionCollection();

            foreach (var item in nodes)
            {
                // construct request.
                nodesToBrowse.Add(new BrowseDescription()
                {
                    NodeId = item.Handle,
                    BrowseDirection = browser.BrowseDirection,
                    ReferenceTypeId = browser.ReferenceTypeId,
                    IncludeSubtypes = browser.IncludeSubtypes,
                    NodeClassMask = (uint)browser.NodeClassMask,
                    ResultMask = browser.ResultMask
                });
            }

            // make the call to the server.
            ResponseHeader responseHeader = browser.Session.Browse(
                null,
                browser.View,
                browser.MaxReferencesReturned,
                nodesToBrowse,
                out BrowseResultCollection results,
                out DiagnosticInfoCollection diagnosticInfos);

            // ensure that the server returned valid results.
            ClientBase.ValidateResponse(results, nodesToBrowse);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToBrowse);
            var browseresults = new Dictionary<SymbolInfo, ReferenceDescriptionCollection>();

            for (int i = 0; i < results.Count; i++)
            {
                var references = new ReferenceDescriptionCollection();
                var item = results[i];
                var node = nodes[i];

                // check if valid.
                if (StatusCode.IsBad(item.StatusCode))
                {
                    throw ServiceResultException.Create(item.StatusCode, 0, diagnosticInfos, responseHeader.StringTable);
                }

                references.AddRange(item.References);
                byte[] continuationPoint = item.ContinuationPoint;
                while (continuationPoint != null)
                {
                    ReferenceDescriptionCollection additionalReferences;

                    if (!browser.ContinueUntilDone)
                    {
                        BrowseNext(ref continuationPoint, true, browser);
                        return browseresults;
                    }

                    additionalReferences = BrowseNext(ref continuationPoint, false, browser);
                    if (additionalReferences != null && additionalReferences.Count > 0)
                    {
                        references.AddRange(additionalReferences);
                    }
                    else
                    {
                        Utils.Trace("Continuation point exists, but the browse results are null/empty.");
                        break;
                    }
                }

                browseresults.Add(node, references);
            }

            // return the results.
            return browseresults;
        }

        /// <summary>
        /// Fetches the next batch of references.
        /// </summary>
        /// <param name="continuationPoint">The continuation point.</param>
        /// <param name="cancel">if set to <c>true</c> the browse operation is cancelled.</param>
        /// <returns>The next batch of references</returns>
        private static ReferenceDescriptionCollection BrowseNext(ref byte[] continuationPoint, bool cancel, Browser browser)
        {
            var continuationPoints = new ByteStringCollection
            {
                continuationPoint
            };

            // make the call to the server.
            ResponseHeader responseHeader = browser.Session.BrowseNext(
                null,
                cancel,
                continuationPoints,
                out BrowseResultCollection results,
                out DiagnosticInfoCollection diagnosticInfos);

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
    }
}