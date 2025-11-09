using System.IO.Compression;

class CatFile
{
    public static int Run(string path)
    {
        try
        {
            Blob blob = new(path);
            Console.WriteLine(blob.ReadBlobContents());
            return 0;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.Message);
            return 1;
        }
    }
}
