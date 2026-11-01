namespace EssaLab.Zid.Tests;

public class ZidTests
{
    [Fact]
    public void Next_ShouldGenerateStrictlyIncreasingIds()
    {
        // Act
        var id1 = Zid.Next();
        var id2 = Zid.Next();
        var id3 = Zid.Next();

        // Assert - Ensure that identifiers are chronologically ordered and strictly increasing
        Assert.True(id2.Value > id1.Value, "Second ID must be greater than first ID");
        Assert.True(id3.Value > id2.Value, "Third ID must be greater than second ID");
    }

    [Fact]
    public void Base62_RoundTrip_ShouldMatchOriginalZid()
    {
        // Arrange
        var originalId = Zid.Next();

        // Act
        string shortCode = originalId.ToShort();
        bool parseResult = Zid.TryParse(shortCode, out Zid parsedId);

        // Assert
        Assert.True(parseResult, "Base62 parsing should succeed");
        Assert.Equal(originalId, parsedId);
        Assert.Equal(originalId.Value, parsedId.Value);
    }

    [Fact]
    public void MetadataExtraction_ShouldExtractCorrectTimestampAndMachine()
    {
        // Arrange
        var beforeGeneration = DateTimeOffset.UtcNow.AddSeconds(-1);
        var zid = Zid.Next();
        var afterGeneration = DateTimeOffset.UtcNow.AddSeconds(1);

        // Act
        var timestamp = zid.Timestamp;
        var machineId = zid.MachineId;

        // Assert
        Assert.True(timestamp >= beforeGeneration, "Timestamp should be after or equal to test start");
        Assert.True(timestamp <= afterGeneration, "Timestamp should be before or equal to test end");
        Assert.True(machineId >= 0, "Machine ID should be non-negative");
    }

    [Fact]
    public void TryParse_WithInvalidString_ShouldReturnFalse()
    {
        // Arrange
        string invalidCode = "Invalid@#$Base62!";

        // Act
        bool result = Zid.TryParse(invalidCode, out Zid parsedId);

        // Assert
        Assert.False(result, "Parsing invalid string should fail gracefully");
        Assert.Equal(default, parsedId);
    }
    
    [Fact]
    public void Operators_EqualityAndInequality_ShouldWorkCorrectly()
    {
        // Arrange - Use a genuinely generated Zid to ensure integrity check passes
        var id1 = Zid.Next();
        var id2 = new Zid(id1.Value); // Same raw value
        var id3 = Zid.Next();       // Completely different value

        // Assert (== and !=)
        Assert.True(id1 == id2);
        Assert.False(id1 != id2);
        Assert.True(id1 != id3);
        Assert.False(id1 == id3);

        // Assert Equals method
        Assert.True(id1.Equals(id2));
        Assert.True(id1.Equals((object)id2));
        Assert.False(id1.Equals(null));
    }

    [Fact]
    public void Operators_Relational_ShouldCompareCorrectly()
    {
        // Arrange - Use sequentially generated Zids to ensure correct chronological order
        var smaller = Zid.Next();
        var larger = Zid.Next();

        // Assert (<, >, <=, >=)
        Assert.True(smaller < larger);
        Assert.True(larger > smaller);
        Assert.True(smaller <= larger);
        Assert.True(larger >= smaller);
        Assert.True(smaller <= smaller);
    }
    
    [Fact]
    public void DefaultZid_ShouldHaveZeroValue()
    {
        // Arrange & Act
        Zid defaultZid = default;

        // Assert
        Assert.Equal(0L, defaultZid.Value);
        // Verify that ToShort for zero returns its program output according to the masking algorithm
        Assert.False(string.IsNullOrEmpty(defaultZid.ToShort()));
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData("Invalid_Base62!@#")]
    [InlineData("TOOLONGSTRINGFORZID123456789")]
    public void TryParse_WithInvalidOrNullInputs_ShouldReturnFalse(string? invalidInput)
    {
        // Act
        bool success = Zid.TryParse(invalidInput, out var result);

        // Assert
        Assert.False(success);
        Assert.Equal(default, result);
    }

    [Fact]
    public void Constructor_WithInvalidIntegrity_ShouldThrowFormatException()
    {
        // Arrange & Act & Assert - Ensure that passing a random number not matching Zid criteria fails with a proper exception handler
        long invalidRawValue = 123456789L;
        
        Assert.Throws<FormatException>(() => new Zid(invalidRawValue));
    }
    
    [Fact]
    public void RapidGeneration_InSameMillisecond_ShouldNotThrowException()
    {
        // Act & Assert - Generate 500 IDs in an immediate loop to ensure SpinWait works smoothly without errors
        var list = new List<Zid>(500);
        for (int i = 0; i < 500; i++)
        {
            list.Add(Zid.Next());
        }

        // Ensure that all identifiers are unique and not identical
        var uniqueCount = list.Distinct().Count();
        Assert.Equal(500, uniqueCount);
    }
}