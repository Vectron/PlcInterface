﻿using PlcInterface;
using PlcInterface.Ads;
using PlcInterface.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class IServiceCollectionExtension
    {
        /// <summary>
        /// Configure the <see cref="IServiceCollection"/> for this PLC.
        /// </summary>
        /// <param name="serviceDescriptors">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddAdsPLC(this IServiceCollection serviceDescriptors)
            => serviceDescriptors
                .AddSingletonFactory<ReadWrite, IReadWrite, IAdsReadWrite>()
                .AddSingletonFactory<Monitor, IMonitor, IAdsMonitor>()
                .AddSingletonFactory<SymbolHandler, ISymbolHandler, IAdsSymbolHandler>()
                .AddSingletonFactory<PlcConnection, IPlcConnection<TwinCAT.Ads.IAdsConnection>, IAdsPlcConnection>()
                .AddSingleton<IPlcConnection>(x => x.GetRequiredService<IAdsPlcConnection>())
                .AddTransient<IAdsTypeConverter, AdsTypeConverter>()
                .ConfigureOptions<DefaultConnectionSettingsConfigureOptions>()
                .ConfigureOptions<DefaultSymbolHandlerSettingsConfigureOptions>();
    }
}