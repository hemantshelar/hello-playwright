namespace HelloPlaywright.Tests;

internal static class RepoPaths
{
    public static string FindRoot()
    {
        foreach (var start in new[] { AppContext.BaseDirectory, Directory.GetCurrentDirectory() })
        {
            var dir = new DirectoryInfo(start);
            while (dir is not null)
            {
                if (IsRepoRoot(dir.FullName))
                    return dir.FullName;
                dir = dir.Parent;
            }
        }

        throw new InvalidOperationException(
            "Could not find repo root. Run tests from the hello-playwright directory.");
    }

    private static bool IsRepoRoot(string path) =>
        Directory.Exists(Path.Combine(path, "src", "HelloPlaywright.AuthSetup")) &&
        Directory.Exists(Path.Combine(path, "auth"));

    public static string GoogleStorageStatePath() =>
        Path.Combine(FindRoot(), "auth", "google-storage.json");
}
