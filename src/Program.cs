using CommandLine;

class Program
{
    static int Main(string[] args)
    {
        return Parser.Default.ParseArguments<InitOptions,
                                            CatFileOptions,
                                            HashObjectOptions>(args)
            .MapResult(
                (InitOptions opts) => Init.Run(),
                (CatFileOptions opts) => CatFile.Run(opts.BlobName),
                (HashObjectOptions opts) => HashObject.Run(opts.Filename, opts.Write),
                errs => 1
            );
    }
}
