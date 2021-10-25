using System;

namespace PlcInterface.Opc.IntegrationTests;

internal static class Settings
{
    public static string OpcIp
        => "localhost";

    public static int OpcPort
        => 4840;

    public static Uri PLCUri
        => new UriBuilder("opc.tcp", OpcIp, OpcPort, RootNode).Uri;

    public static Uri PLCUriNoRoot
        => new UriBuilder("opc.tcp", OpcIp, OpcPort, string.Empty).Uri;

    public static string RootNode
        => "PLC1";
}