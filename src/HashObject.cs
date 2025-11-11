static class HashObject
{
    public static int Run(string filepath, bool write = false)
    {
        try
        {
            if (!write)
            {
                // Print hash
                Console.WriteLine(Blob.ComputeFileSha1(filepath));
            }
            else
            {
                // Write the object to the hash
                StreamReader sr = new(filepath);
                string content = sr.ReadToEnd();
                sr.Close();
                var hash = Blob.WriteBlob(content);
                Console.WriteLine(hash);
            }
            return 0;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.Message);
            return 1;
        }
    }
}
