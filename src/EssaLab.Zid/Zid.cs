using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace EssaLab.Zid;

/// <summary>
/// Represents a high-performance, distributed, time-sortable 64-bit unique identifier.
/// </summary>
public readonly struct Zid : IEquatable<Zid>, IComparable<Zid>, ISpanFormattable, ISpanParsable<Zid>
{
    private readonly long _value;

    private const int TimestampBits = 41;  
    private const int MachineIdBits = 10;  
    private const int SequenceBits = 8;    
    private const int ChecksumBits = 4;    

    private const long MaxMachineId = (1L << MachineIdBits) - 1;
    private const long MaxSequence = (1L << SequenceBits) - 1;
    private const int ChecksumMask = (1 << ChecksumBits) - 1;
    private const long MaxTimestamp = (1L << TimestampBits) - 1;

    private const int ShiftSequence = ChecksumBits;
    private const int ShiftMachine = SequenceBits + ChecksumBits;
    private const int ShiftTimestamp = MachineIdBits + SequenceBits + ChecksumBits;

    private static DateTimeOffset _epochOffset = new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
    private static long _epochMs = _epochOffset.ToUnixTimeMilliseconds();
    private static long _machineId;
    private static long _maskKey = 0x5D3759DF12345678;
    private static bool _isInitialized = false;

    private static long _state;

    static Zid()
    {
        Initialize(new ZidOptions());
    }

    /// <summary>
    /// Initializes the Zid generator globally with custom configuration options.
    /// </summary>
    /// <param name="options">Configuration options instance.</param>
    public static void Initialize(ZidOptions options)
    {
        if (_isInitialized && Environment.GetEnvironmentVariable("ZID_ALLOW_REINIT") != "true")
            return;

        _epochOffset = options.Epoch;
        _epochMs = _epochOffset.ToUnixTimeMilliseconds();
        _maskKey = options.MaskKey;
        _machineId = options.MachineId ?? ResolveMachineId();

        long initialTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - _epochMs;
        if (initialTime < 0) initialTime = 0;
        _state = (initialTime << SequenceBits);
        _isInitialized = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Zid"/> struct from a raw 64-bit integer value with data integrity verification.
    /// </summary>
    /// <param name="value">The raw 64-bit integer value.</param>
    /// <exception cref="FormatException">Thrown if integrity verification fails.</exception>
    public Zid(long value)
    {
        if (!Verify(value))
            throw new FormatException("Invalid Zid: Integrity check failed.");
        _value = value;
    }

    private Zid(long value, bool verified) => _value = value;

    /// <summary>
    /// Gets the raw 64-bit underlying integer value of this <see cref="Zid"/>.
    /// </summary>
    public long Value => _value;
    
    /// <summary>
    /// Gets the creation timestamp extracted from this identifier in UTC.
    /// </summary>
    public DateTimeOffset Timestamp
    {
        get
        {
            long timestampDelta = (_value >> ShiftTimestamp) & MaxTimestamp;
            return _epochOffset.AddMilliseconds(timestampDelta);
        }
    }
    
    /// <summary>
    /// Gets the machine/node identifier extracted from this identifier.
    /// </summary>
    public long MachineId => (_value >> ShiftMachine) & MaxMachineId;
    /// <summary>
    /// Gets the sequence number generated within the same millisecond.
    /// </summary>
    public long Sequence => (_value >> ShiftSequence) & MaxSequence;
    /// <summary>
    /// Gets the checksum nibble used for data integrity verification.
    /// </summary>
    public int Checksum => (int)(_value & ChecksumMask);

    /// <summary>
    /// Generates a new unique, time-sortable <see cref="Zid"/> instance using thread-safe mechanics.
    /// </summary>
    /// <returns>A unique <see cref="Zid"/> instance.</returns>
    /// <exception cref="OverflowException">Thrown when the timestamp epoch limit is reached.</exception>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static Zid Next()
    {
        SpinWait spin = new();
        while (true)
        {
            long currentState = Volatile.Read(ref _state);
            long lastTimestamp = currentState >> SequenceBits;
            long sequence = currentState & MaxSequence;

            long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - _epochMs;

            if (currentTimestamp < lastTimestamp) currentTimestamp = lastTimestamp;

            long nextSequence, nextTimestamp;

            if (currentTimestamp == lastTimestamp)
            {
                nextSequence = sequence + 1;
                if (nextSequence > MaxSequence)
                {
                    spin.SpinOnce();
                    continue;
                }
                nextTimestamp = currentTimestamp;
            }
            else
            {
                nextSequence = 0;
                nextTimestamp = currentTimestamp;
            }

            if (nextTimestamp > MaxTimestamp)
                throw new OverflowException("Zid Epoch Exhausted.");

            long nextState = (nextTimestamp << SequenceBits) | nextSequence;

            if (Interlocked.CompareExchange(ref _state, nextState, currentState) == currentState)
            {
                long rawId = (nextTimestamp << (MachineIdBits + SequenceBits))
                           | (_machineId << SequenceBits)
                           | nextSequence;

                int checksum = CalculateChecksum(rawId);
                long finalId = (rawId << ChecksumBits) | (long)checksum;

                return new Zid(finalId, true);
            }

            spin.SpinOnce();
        }
    }

    private static long ResolveMachineId()
    {
        string? envMachineId = Environment.GetEnvironmentVariable("ZID_MACHINE_ID");
        if (!string.IsNullOrEmpty(envMachineId) && long.TryParse(envMachineId, out long parsedId))
        {
            if (parsedId >= 0 && parsedId <= MaxMachineId) return parsedId;
        }

        object anchor = new();
        ulong hash = (ulong)Environment.TickCount64 ^ (ulong)RuntimeHelpers.GetHashCode(anchor);
        hash ^= (ulong)Environment.ProcessId * 2654435761uL;
        return (long)(hash & (ulong)MaxMachineId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CalculateChecksum(long rawId)
    {
        ulong v = (ulong)rawId;
        v ^= v >> 32; v ^= v >> 16; v ^= v >> 8; v ^= v >> 4;
        return (int)(v & (ulong)ChecksumMask);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool Verify(long value)
    {
        if (value <= 0) return false;
        long rawId = value >> ChecksumBits;
        int check = (int)(value & ChecksumMask);
        return CalculateChecksum(rawId) == check;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long ApplyMask(long value) => value ^ _maskKey;

    /// <summary>
    /// Converts this <see cref="Zid"/> into a short Base62 string representation.
    /// </summary>
    /// <returns>Base62 string representation.</returns>
    public string ToShort() => Base62Encode(_value);

    private static string Base62Encode(long value)
    {
        Span<char> buffer = stackalloc char[11];
        TryEncodeBase62(value, buffer, out int charsWritten);
        return new string(buffer.Slice(11 - charsWritten, charsWritten));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryEncodeBase62(long value, Span<char> destination, out int charsWritten)
    {
        const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        ulong v = (ulong)ApplyMask(value);

        if (destination.Length < 11) { charsWritten = 0; return false; }

        int i = 11;
        if (v == 0) { destination[--i] = '0'; }
        else
        {
            while (v > 0)
            {
                destination[--i] = chars[(int)(v % 62)];
                v /= 62;
            }
        }

        int length = 11 - i;
        destination.Slice(i, length).CopyTo(destination);
        charsWritten = length;
        return true;
    }

    /// <summary>
    /// Tries to parse a Base62 character span into a <see cref="Zid"/> instance.
    /// </summary>
    /// <param name="s">The character span containing the Base62 string.</param>
    /// <param name="result">When this method returns, contains the parsed <see cref="Zid"/> if successful.</param>
    /// <returns><c>true</c> if parsing succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryParse(ReadOnlySpan<char> s, out Zid result)
    {
        if (TryBase62Decode(s, out var value)) { result = new Zid(value); return true; }
        result = default; return false;
    }

    /// <summary>
    /// Tries to parse a Base62 string into a <see cref="Zid"/> instance.
    /// </summary>
    /// <param name="s">The input string to parse.</param>
    /// <param name="result">When this method returns, contains the parsed <see cref="Zid"/> if successful.</param>
    /// <returns><c>true</c> if parsing succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryParse(string? s, out Zid result) => TryParse(s.AsSpan(), out result);

    private static bool TryBase62Decode(ReadOnlySpan<char> s, out long result)
    {
        result = 0;
        if (s.IsEmpty) return false;

        ulong res = 0;
        foreach (char c in s)
        {
            int d = c switch
            {
                >= '0' and <= '9' => c - '0',
                >= 'a' and <= 'z' => c - 'a' + 10,
                >= 'A' and <= 'Z' => c - 'A' + 36,
                _ => -1
            };

            if (d == -1) return false;
            if (res > (ulong.MaxValue - (ulong)d) / 62) return false;
            res = res * 62 + (ulong)d;
        }

        result = ApplyMask((long)res);
        return Verify(result);
    }
    /// <inheritdoc />
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        if (format.Equals("S".AsSpan(), StringComparison.OrdinalIgnoreCase))
            return TryEncodeBase62(_value, destination, out charsWritten);
        return _value.TryFormat(destination, out charsWritten, format, provider);
    }
    /// <inheritdoc />
    public static Zid Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => TryParse(s, out var r) ? r : throw new FormatException();
    /// <inheritdoc />
    public static Zid Parse(string s, IFormatProvider? provider) => Parse(s.AsSpan(), provider);
    /// <inheritdoc />
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Zid result) => TryParse(s, out result);
    /// <inheritdoc />
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Zid result) => TryParse(s.AsSpan(), out result);
    /// <inheritdoc />
    public override string ToString() => _value.ToString();
    /// <inheritdoc />
    public string ToString(string? format, IFormatProvider? formatProvider) => format is "S" or "s" ? ToShort() : _value.ToString(format, formatProvider);
    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Zid z && _value == z._value;
    /// <inheritdoc />
    public bool Equals(Zid other) => _value == other._value;
    /// <inheritdoc />
    public override int GetHashCode() => _value.GetHashCode();
    /// <inheritdoc />
    public int CompareTo(Zid other) => _value.CompareTo(other._value);
    /// <summary>Compares two <see cref="Zid"/> instances for equality.</summary>
    public static bool operator ==(Zid l, Zid r) => l._value == r._value;
    /// <summary>Compares two <see cref="Zid"/> instances for inequality.</summary>
    public static bool operator !=(Zid l, Zid r) => l._value != r._value;
    /// <summary>Implicitly converts a <see cref="Zid"/> instance to its underlying <see cref="long"/> value.</summary>
    public static implicit operator long(Zid z) => z._value;
    /// <summary>Implicitly converts a <see cref="long"/> raw value into a <see cref="Zid"/> instance.</summary>
    public static implicit operator Zid(long v) => new(v);
}