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
    /// If null, the <c>ZID_MACHINE_ID</c> environment variable is used; if that is not set either, a
    /// <b>random</b> value is picked at startup. Random values can collide between instances
    /// (e.g. Kubernetes replicas) and silently produce duplicate IDs, so always set this explicitly in production.
    /// </summary>
    public long? MachineId { get; set; }

    /// <summary>
    /// Gets or sets the XOR mask key used for obfuscating raw values during Base62 conversion.
    /// </summary>
    /// <remarks>
    /// This is light obfuscation only, not encryption: the default key is public, and the key must never change
    /// once short codes have been issued, otherwise they stop parsing.
    /// </remarks>
    public long MaskKey { get; set; } = 0x5D3759DF12345678;

    /// <summary>
    /// Validates the options and throws if any value would corrupt generated IDs.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <see cref="MachineId"/> is outside 0..<see cref="Zid.MaxMachineId"/>, or <see cref="Epoch"/> is in the future,
    /// before 1970, or so old that the 41-bit timestamp has already overflowed.
    /// </exception>
    public void Validate()
    {
        if (MachineId is { } machineId && (machineId < 0 || machineId > Zid.MaxMachineId))
            throw new ArgumentOutOfRangeException(nameof(MachineId), machineId,
                $"MachineId must be between 0 and {Zid.MaxMachineId}.");

        DateTimeOffset now = DateTimeOffset.UtcNow;
        long epochMs = Epoch.ToUnixTimeMilliseconds();

        if (epochMs < 0)
            throw new ArgumentOutOfRangeException(nameof(Epoch), Epoch, "Epoch must not be before 1970-01-01 (UTC).");
        if (Epoch > now)
            throw new ArgumentOutOfRangeException(nameof(Epoch), Epoch, "Epoch must not be in the future.");
        if (now.ToUnixTimeMilliseconds() - epochMs > Zid.MaxTimestamp)
            throw new ArgumentOutOfRangeException(nameof(Epoch), Epoch,
                "Epoch is too old: the 41-bit timestamp range has already been exhausted.");
    }
}