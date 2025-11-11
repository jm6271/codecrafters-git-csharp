using System.IO.Compression;
using System.Net;
using System.Text;

class Commit
{
    public static string WriteCommit(string treeSha, string parentSha, string message, string author, string email)
    {
        string header = "commit ";

        string content = $"tree {treeSha}\n";
        content += $"parent {parentSha}\n";
        content += $"author {author} {email} {DateTimeOffset.Now.ToUnixTimeSeconds()} {TimeZoneInfo.Local.BaseUtcOffset}\n";
        content += $"committer {author} {email} {DateTimeOffset.Now.ToUnixTimeSeconds()} {TimeZoneInfo.Local.BaseUtcOffset}\n\n";
        content += message + "\n";

        header += content.Length.ToString() + '\0';

        string commitStr = header + content;

        var commit = Encoding.UTF8.GetBytes(commitStr);
        var hash = Blob.ComputeFileSha1(commit);
        var path = GetFullPathToObject(hash);

        // Write commit
        Directory.CreateDirectory(Directory.GetParent(path)?.FullName ?? "");
        using FileStream outputStream = new(path, FileMode.Create);
        using var zlibStream = new ZLibStream(outputStream, CompressionMode.Compress);
        zlibStream.Write(commit, 0, commit.Length);

        return hash;
    }

    private static string GetFullPathToObject(string blob)
    {
        return $".git/objects/{blob[..2]}/{blob[2..]}";
    }
}