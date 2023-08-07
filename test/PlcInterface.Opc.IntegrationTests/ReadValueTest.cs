﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcInterface.IntegrationTests;
using PlcInterface.OpcUa;

namespace PlcInterface.Opc.IntegrationTests;

[TestClass]
[DoNotParallelize]
public sealed class ReadValueTest : IReadValueTestBase
{
    protected override string DataRoot => "Opc";

    protected override ServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection()
            .AddOpcPLC()
            .Configure<OpcPlcConnectionOptions>(o => o.Address = Settings.PLCUri);

        services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(NullLogger<>)));

        return services.BuildServiceProvider();
    }
}