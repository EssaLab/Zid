using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EssaLab.Zid.EntityFramework;

/// <summary>
/// EF Core Value Converter to store <see cref="Zid"/> as a 64-bit integer (<c>BIGINT</c>) in the database
/// and convert it back automatically when reading.
/// </summary>
public class ZidValueConverter : ValueConverter<Zid, long>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ZidValueConverter"/> class.
    /// </summary>
    public ZidValueConverter() : base(
        zid => zid.Value,
        value => new Zid(value))
    { }
}