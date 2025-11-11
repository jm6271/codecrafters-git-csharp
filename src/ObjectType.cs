using System.IO.Compression;

class ObjectType
{
    public enum GitObjectType
    {
        Blob,
        Tree,
    }

    public static GitObjectType GetObjectType(string hash)
    {
        // Read object header to determine its type
        var obj = ReadObject(hash);

        int i = 0;
        string header = "";

        while (i != obj.Length && obj[i] != ' ')
        {
            header += (char)obj[i];
            i++;
        }

        return header switch
        {
            "blob" => GitObjectType.Blob,
            "tree" => GitObjectType.Tree,
            _ => throw new Exception($"Error: Invalid header in object '{hash}'"),
        };
    }

    private static byte[] ReadObject(string hash)
    {
        string path = GetFullPathToObject(hash);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Error while reading object: no file exists at {path}");
        }

        // Decompress object
        using FileStream sr = new(path, FileMode.Open);
        using MemoryStream obj = new();
        using (var zlibStream = new ZLibStream(sr, CompressionMode.Decompress))
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