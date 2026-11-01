using Microsoft.EntityFrameworkCore;

namespace EssaLab.Zid.EntityFramework;

/// <summary>
/// Provides extension methods for <see cref="ModelConfigurationBuilder"/> to configure Zid globally.
/// </summary>
public static class ZidModelConfigurationBuilderExtensions
{
    /// <summary>
    /// Automatically registers the <see cref="ZidValueConverter"/> and <see cref="ZidValueComparer"/> 
    /// for all <see cref="Zid"/> properties across the entire model prior to validation.
    /// </summary>
    /// <param name="configurationBuilder">The <see cref="ModelConfigurationBuilder"/> instance.</param>
    /// <returns>The same <see cref="ModelConfigurationBuilder"/> instance for method chaining.</returns>
    public static ModelConfigurationBuilder AddZid(this ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<Zid>()
            .HaveConversion<ZidValueConverter, ZidValueComparer>();

        configurationBuilder.Properties<Zid?>()
            .HaveConversion<ZidValueConverter, ZidValueComparer>();

        return configurationBuilder;
    }
}