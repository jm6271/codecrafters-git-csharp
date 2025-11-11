public class TreeEntry
{
    public string Mode { get; set; } = "";
    public string Name { get; set; } = "";
    public byte[] SHA1 { get; set; } = System.Array.Empty<byte>();

    public string SHA1Str
    {
        get
        {
            // Return a lowercase hex string without separators (git-style)
            return Convert.ToHexString(SHA1).ToLowerInvariant();
        }
        set
        {
            SHA1 = Convert.FromHexString(value);
        }
    }

}
