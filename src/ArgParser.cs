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
