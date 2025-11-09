using CommandLine;

class Program
{
    static int Main(string[] args)
    {
        return Parser.Default.ParseArguments<InitOptions, CatFileOptions>(args)
            .MapResult(
                (InitOptions opts) => Init.Run(),
                (CatFileOptions opts) => CatFile.Run(opts.BlobName),
                errs => 1
            );
    }
}
