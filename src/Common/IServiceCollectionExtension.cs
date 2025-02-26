using Microsoft.Extensions.DependencyInjection;

namespace PlcInterface;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>.
/// </summary>
#if !CommonProject
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
internal static class IServiceCollectionExtension
{
    /// <summary>
    /// Add <typeparamref name="TService1"/> and <typeparamref name="TService2"/> as a <typeparamref name="TImplementation"/> to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TImplementation">The concrete implementation.</typeparam>
    /// <typeparam name="TService1">The first service.</typeparam>
    /// <typeparam name="TService2">The second service.</typeparam>
    /// <param name="serviceDescriptors">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddSingletonFactory<TImplementation, TService1, TService2>(this IServiceCollection serviceDescriptors)
        where TService1 : class
        where TService2 : class, TService1
        where TImplementation : class, TService1, TService2
        => serviceDescriptors
            .AddSingleton<TService2, TImplementation>()
            .AddSingleton(x => (TService1)x.GetRequiredService<TService2>());
}
