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
