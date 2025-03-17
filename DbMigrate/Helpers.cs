namespace DbMigrate;

/// <summary>
/// Some static helper methods.
/// </summary>
public static class Helpers
{
    /// <summary>
    /// Calculates the number of bytes needed to represent <paramref name="bytes"/> bytes in Base64 encoding.
    /// </summary>
    /// <param name="bytes">The number of bytes to represent.</param>
    /// <returns>The number of Base64 bytes required.</returns>
    public static int Base64ByteCount(int bytes)
    {
        return 4 * ((bytes + 2) / 3);
    }

    /// <summary>
    /// Calculates the number of bytes needed to represent <paramref name="bits"/> bits.
    /// </summary>
    /// <param name="bits">The number of bits to represent.</param>
    /// <returns>The number of bytes required.</returns>
    public static int ByteCountFromBits(int bits)
    {
        return (bits + 7) / 8;
    }
}