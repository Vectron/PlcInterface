using System;
using TwinCAT.Ads;

namespace PlcInterface.Ads.Extensions
{
    internal static class TcAdsClientExtension
    {
        public static TcAdsClient ValidateConnection(this TcAdsClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (client.RouterState != AmsRouterState.Start)
            {
                throw new InvalidOperationException("Router not connected");
            }

            if (!client.IsConnected)
            {
                throw new InvalidOperationException("PLC not connected");
            }

            client.TryReadState(out StateInfo lastPLCState);

            if (lastPLCState.AdsState != AdsState.Run)
            {
                throw new InvalidOperationException("PLC not running");
            }

            return client;
        }
    }
}