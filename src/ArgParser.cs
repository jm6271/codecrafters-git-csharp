using System.Dynamic;
using CommandLine;

[Verb("cat-file", HelpText = "Display the contents of a blob")]
class CatFileOptions
{
    [Option('p', Required = true, HelpText = "Path to blob")]
    public string BlobName { get; set; } = "";
}

[Verb("init", HelpText = "Initialize a repository")]
class InitOptions
{

}

[Verb("hash-object", HelpText = "Compute the SHA-1 has of an object")]
class HashObjectOptions
{
    [Option('w', Required = false, HelpText = "Write the object")]
    public bool Write { get; set; } = false;

    [Value(0, MetaName = "filename", Required = true, HelpText = "Path to file to compute hash of")]
    public string Filename { get; set; } = "";
}

[Verb("ls-tree", HelpText = "Display the entries in a tree object")]
class LsTreeOptions
{
    [Option("name-only")]
    public bool NameOnly { get; set; } = false;

    [Value(0, MetaName = "tree", Required = true, HelpText = "SHA-1 of tree")]
    public string SHA1 { get; set; } = "";
}

[Verb("write-tree", HelpText = "Write current directory as a tree")]
class WriteTreeOptions
{

}

[Verb("commit-tree", HelpText = "Commit a tree")]
class CommitTreeOptions
{
    [Option('p', Required = true, HelpText = "Parent commit SHA-1")]
    public string ParentSha { get; set; } = "";

    [Option('m', Required = true, HelpText = "Commit message")]
    public string Message { get; set; } = "";

    [Value(0, MetaName = "tree", Required = true, HelpText = "SHA-1 of tree to commit")]
    public string TreeSha { get; set; } = "";
}

