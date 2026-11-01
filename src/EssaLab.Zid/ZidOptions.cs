namespace EssaLab.Zid;

/// <summary>
/// Configuration options for initializing the <see cref="Zid"/> generator instance.
/// </summary>
public sealed class ZidOptions
{
    /// <summary>
    /// Gets or sets the custom reference epoch for timestamp calculations.
    /// Default is January 1, 2024 (UTC).
    /// </summary>
    public DateTimeOffset Epoch { get; set; } = new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);

    /// <summary>
    /// Gets or sets the explicit machine/node identifier (0 - 1023).
    /// If null, a deterministic ID will be generated based on host machine details.
    /// </summary>
    public long? MachineId { get; set; }

    /// <summary>
    /// Gets or sets the XOR mask key used for obfuscating raw values during Base62 conversion.
    /// </summary>
    public long MaskKey { get; set; } = 0x5D3759DF12345678;
}