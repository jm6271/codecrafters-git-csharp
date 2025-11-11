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
            if (value.Length != 40)
                throw new Exception($"Error: Invalid hash '{value}'");

            List<byte> sha = [];
            for (int i = 0; i < value.Length; i += 2)
            {
                string hex = value[i].ToString() + value[i + 1].ToString();

                byte b = Convert.ToByte(hex, 16);
                sha.Add(b);
            }

            SHA1 = [.. sha];
        }
    }

}
