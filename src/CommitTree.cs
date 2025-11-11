static class CommitTree
{
    public static int Run(string treeSha, string parentSha, string message)
    {
        Console.WriteLine(Commit.WriteCommit(treeSha, parentSha, message, "Jordan", "jordan@example.com"));
        return 0;
    }
}
