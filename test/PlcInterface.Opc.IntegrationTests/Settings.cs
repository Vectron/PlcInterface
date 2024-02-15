using System;
using System.Globalization;

namespace PlcInterface.Opc.IntegrationTests;

internal static class Settings
{
    public static string OpcIp
        => "192.168.17.211";

    public static int OpcPort
        => 4840;

    public static string RootVariable
        => $"PLC1.OpcNet{Environment.Version.Major.ToString(CultureInfo.InvariantCulture)}";
}
