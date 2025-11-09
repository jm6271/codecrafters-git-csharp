using System.IO.Compression;

class Blob
{
    private string _blobPath;

    public Blob(string path)
    {
        _blobPath = GetFullPathToObject(path);
    }

    public string ReadBlobContents()
    {
        var blob = ReadBlob();

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
            throw new Exception($"Error: Invalid header {header} in blob {_blobPath}");

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
    
    public byte[] ReadBlob()
    {
        if (!File.Exists(_blobPath))
        {
            throw new FileNotFoundException($"Error while reading blob: no file exists at {_blobPath}");
        }

        // Decompress object
        using FileStream sr = new(_blobPath, FileMode.Open);
        sr.Seek(2, SeekOrigin.Begin);
        using MemoryStream obj = new();
        using (var zlibStream = new DeflateStream(sr, CompressionMode.Decompress))
        {
            zlibStream.CopyTo(obj);
        }
        return obj.ToArray();
    }

    private static string GetFullPathToObject(string blob)
    {
        return $".git/objects/{blob[..2]}/{blob[2..]}";
    }
}