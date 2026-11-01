using Microsoft.Extensions.DependencyInjection;

namespace EssaLab.Zid.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for setting up <see cref="Zid"/> in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ZidServiceCollectionExtensions
{
    /// <summary>
    /// Adds and initializes <see cref="Zid"/> services into the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configure">An optional action to configure <see cref="ZidOptions"/>.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddZid(this IServiceCollection services, Action<ZidOptions>? configure = null)
    {
        var options = new ZidOptions();
        configure?.Invoke(options);

        // Initialize once
        Zid.Initialize(options);
        
        return services;
    }
}