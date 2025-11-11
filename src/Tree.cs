using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Collections.Generic;

public class Tree
{
    public static List<string> GetTreeEntryNames(string treeHash)
    {
        List<string> names = new List<string>();

        var entries = ReadEntries(treeHash);
        names.AddRange(from entry in entries
                       select entry.Name);

        return names;
    }
    
    public static List<TreeEntry> ReadEntries(string treeHash)
    {
        // read object and decompress with zlib
        var obj = ReadTreeObject(treeHash);

        // Read header
        int i = 0;
        string header = "";

        while (i != obj.Length && (char)obj[i] != ' ')
        {
            header += (char)obj[i];
            i++;
        }
        i++; // skip space

        if (header != "tree")
        {
            throw new Exception($"Error: Invalid header '{header}' in tree '{treeHash}'");
        }

        // Read size
        string size = "";
        while (i != obj.Length && obj[i] != 0)
        {
            size += (char)obj[i];
            i++;
        }
        i++; // Skip null byte

        // Convert size value to integer
        var contentSize = Convert.ToInt32(size);

        // construct a list of tree entries
        List<TreeEntry> entries = new List<TreeEntry>();
        int contentStart = i;
        int contentEnd = contentStart + contentSize;
        if (contentEnd > obj.Length) contentEnd = obj.Length;

        while (i < contentEnd)
        {
            TreeEntry currentEntry = new TreeEntry();

            // Read mode
            while (i != obj.Length && obj[i] != (byte)' ')
            {
                currentEntry.Mode += (char)obj[i];
                i++;
            }
            i++; // Skip space

            // Read name
            while (i != obj.Length && obj[i] != 0)
            {
                currentEntry.Name += (char)obj[i];
                i++;
            }
            i++; // Skip null byte

            // Read 20 byte SHA1 hash
            byte[] sha1Bytes = new byte[20];
            Array.Copy(obj, i, sha1Bytes, 0, 20);
            currentEntry.SHA1 = sha1Bytes;
            i += 20;

            entries.Add(currentEntry);
        }

        return entries;
    }

    private static byte[] ReadTreeObject(string treeHash)
    {
        string treePath = GetFullPathToObject(treeHash);
        if (!File.Exists(treePath))
        {
            throw new FileNotFoundException($"Error while reading tree: no file exists at {treePath}");
        }

        // Decompress object
        using FileStream sr = new(treePath, FileMode.Open);
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