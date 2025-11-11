using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

static class Blob
{
    public static string ReadBlobContents(string blobHash)
    {
        var blob = ReadBlob(blobHash);

        int i = 0;
        string header = "";

        while (i != blob.Length && blob[i] != ' ')
        {
            header += (char)blob[i];
            i++;
        }
        i++; // skip space

        // check header
        if (header != "blob")
            throw new Exception($"Error: Invalid header {header} in blob {blobHash}");

        // Read size
        string size = "";
        while (i != blob.Length && blob[i] != '\0')
        {
            size += (char)blob[i];
            i++;
        }
        i++; // Skip null byte

        // Convert size value to integer
        var contentSize = Convert.ToInt32(size);

        // Read content
        string content = "";
        for (int j = 0; j < contentSize; j++)
        {
            content += (char)blob[j + i];
        }

        return content;
    }

    public static byte[] ReadBlob(string blobHash)
    {
        string blobPath = GetFullPathToObject(blobHash);
        if (!File.Exists(blobPath))
        {
            throw new FileNotFoundException($"Error while reading blob: no file exists at {blobPath}");
        }

        // Decompress object
        using FileStream sr = new(blobPath, FileMode.Open);
        sr.Seek(2, SeekOrigin.Begin);
        using MemoryStream obj = new();
        using (var zlibStream = new DeflateStream(sr, CompressionMode.Decompress))
        {
            zlibStream.CopyTo(obj);
        }
        return obj.ToArray();
    }

    public static string WriteBlob(string contents)
    {
        // Create blob string
        string blob = "blob ";
        blob += contents.Length.ToString();
        blob += '\0';
        blob += contents;

        var encodedBlob = Encoding.UTF8.GetBytes(blob);

        // Get hash
        string hash = ComputeFileSha1(encodedBlob);
        string blobPath = GetFullPathToObject(hash);

        // open a filestream
        Directory.CreateDirectory(Directory.GetParent(blobPath)?.FullName ?? "");
        using FileStream outputStream = new(blobPath, FileMode.Create);
        using var zlibStream = new ZLibStream(outputStream, CompressionMode.Compress);
        zlibStream.Write(encodedBlob, 0, encodedBlob.Length);

        return hash;
    }

    private static string GetFullPathToObject(string blob)
    {
        return $".git/objects/{blob[..2]}/{blob[2..]}";
    }

    public static string ComputeFileSha1(byte[] fileContents)
    {
        using var sha1 = SHA1.Create();
        using var stream = new MemoryStream(fileContents);
        byte[] hashBytes = sha1.ComputeHash(stream);
        return Convert.ToHexString(hashBytes).ToLower();
    }

    public static string ComputeFileSha1(string filePath)
    {
        string contents = File.ReadAllText(filePath);

        // Create blob string
        string blob = "blob ";
        blob += contents.Length.ToString();
        blob += '\0';
        blob += contents;

        var encodedBlob = Encoding.UTF8.GetBytes(blob);

        // Get hash
        string hash = ComputeFileSha1(encodedBlob);

        return hash;
    }
}
