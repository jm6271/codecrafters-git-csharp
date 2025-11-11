using CommandLine;

class Program
{
    static int Main(string[] args)
    {
        return Parser.Default.ParseArguments<InitOptions,
                                            CatFileOptions,
                                            HashObjectOptions,
                                            LsTreeOptions>(args)
            .MapResult(
                (InitOptions opts) => Init.Run(),
                (CatFileOptions opts) => CatFile.Run(opts.BlobName),
                (HashObjectOptions opts) => HashObject.Run(opts.Filename, opts.Write),
                (LsTreeOptions opts) => LsTree.Run(opts.SHA1, opts.NameOnly),
                errs => 1
            );
    }
}
