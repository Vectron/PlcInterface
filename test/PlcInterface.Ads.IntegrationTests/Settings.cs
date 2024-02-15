using System;
using System.Globalization;

namespace PlcInterface.Ads.IntegrationTests;

internal static class Settings
{
    public static string AmsNetId
        => "172.105.0.1.1.1";

    public static int Port
        => 851;

    public static string RootVariable
        => $"AdsNet{Environment.Version.Major.ToString(CultureInfo.InvariantCulture)}";
}
