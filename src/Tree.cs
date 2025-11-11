using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Collections.Generic;
using System.Text;

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
    
    public static string WriteTreeObject(string directoryPath)
    {
        // Write the specified directory as a tree, and return the SHA-1 hash

        List<TreeEntry> entries = [];

        List<string> directoryEntries = [.. Directory.GetFileSystemEntries(directoryPath)];

        foreach (var entry in directoryEntries)
        {
            TreeEntry treeEntry = new TreeEntry();
            FileAttributes attr = File.GetAttributes(entry);
            if (attr.HasFlag(FileAttributes.Directory))
            {
                // Directory

                // Make sure this isn't the .git directory
                if (Path.GetFileName(entry) == ".git")
                    continue;

                treeEntry.Mode = "40000";
                treeEntry.Name = Path.GetFileName(entry);
                treeEntry.SHA1Str = WriteTreeObject(entry);
            }
            else
            {
                // File
                treeEntry.Mode = "100644";

                treeEntry.Name = Path.GetFileName(entry);

                StreamReader sr = new(entry);
                string content = sr.ReadToEnd();
                sr.Close();
                treeEntry.SHA1Str = Blob.WriteBlob(content);
            }
            entries.Add(treeEntry);
        }

        // Sort entries by name
        entries = [.. entries.OrderBy(e => e.Name)];

        // Write the entries to a tree object
        List<byte> treeContents = [];
        foreach (var entry in entries)
        {
            var mode = Encoding.UTF8.GetBytes(entry.Mode + " ");
            treeContents.AddRange(mode);

            var name = Encoding.UTF8.GetBytes(entry.Name + "\0");
            treeContents.AddRange(name);

            treeContents.AddRange(entry.SHA1);
        }

        // Compute length
        string treeLength = treeContents.Count.ToString();

        // Create tree object
        List<byte> treeObject = [];
        var header = Encoding.UTF8.GetBytes("tree " + treeLength + "\0");
        treeObject.AddRange(header);
        treeObject.AddRange(treeContents);

        var tree = treeObject.ToArray();

        // Compute SHA-1 hash
        var treeHash = Blob.ComputeFileSha1(tree);

        // Write to disk with ZLib
        string treePath = GetFullPathToObject(treeHash);
        Directory.CreateDirectory(Directory.GetParent(treePath)?.FullName ?? "");
        using FileStream outputStream = new(treePath, FileMode.Create);
        using var zlibStream = new ZLibStream(outputStream, CompressionMode.Compress);
        zlibStream.Write(tree, 0, tree.Length);

        return treeHash;
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
    
    private static string GetFullPathToObject(string hash)
    {
        return $".git/objects/{hash[..2]}/{hash[2..]}";
    }
}