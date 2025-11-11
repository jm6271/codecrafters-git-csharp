public static class LsTree
{
    public static int Run(string treeHash, bool nameOnly = false)
    {
        if (nameOnly)
        {
            // Output only the filenames
            var names = Tree.GetTreeEntryNames(treeHash);
            foreach (var name in names)
            {
                Console.WriteLine(name);
            }

            return 0;
        }

        // Output all data in tree
        var entries = Tree.ReadEntries(treeHash);
        foreach (var entry in entries)
        {
            Console.Write(entry.Mode);
            Console.Write(" ");
            // Determine object type for this entry (use the entry's hash)
            var entryType = ObjectType.GetObjectType(entry.SHA1Str);
            if (entryType == ObjectType.GitObjectType.Blob)
                Console.Write("blob ");
            else
                Console.Write("tree ");

            Console.Write(entry.SHA1Str);
            Console.Write("\t");
            Console.Write(entry.Name);

            Console.WriteLine();
        }

        return 0;
    }
}