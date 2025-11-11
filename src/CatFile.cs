using System.IO.Compression;

class CatFile
{
    public static int Run(string path)
    {
        try
        {
            Console.Write(Blob.ReadBlobContents(path));
            return 0;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.Message);
            return 1;
        }
    }
}
