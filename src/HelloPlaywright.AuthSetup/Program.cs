using HelloPlaywright.AuthSetup;
using Microsoft.Playwright;

var repoRoot = RepoPaths.FindRoot();
var authDir = Path.Combine(repoRoot, "auth");
var profileDir = Path.Combine(authDir, "chrome-profile");
var statePath = Path.Combine(authDir, "google-storage.json");

Console.WriteLine("Google auth setup — log in manually, then press Enter.");
Console.WriteLine($"Chrome profile: {profileDir}");
Console.WriteLine($"Storage state:  {statePath}");
Console.WriteLine();

Directory.CreateDirectory(profileDir);

using var playwright = await Playwright.CreateAsync();
await using var context = await playwright.Chromium.LaunchPersistentContextAsync(
    profileDir,
    GoogleBrowserOptions.PersistentLogin());

var page = context.Pages.Count > 0 ? context.Pages[0] : await context.NewPageAsync();
await page.GotoAsync("https://mail.google.com");

Console.WriteLine("1. Sign in using Chrome/Edge (not Playwright Chromium).");
Console.WriteLine("2. If Google still blocks sign-in, see README troubleshooting.");
Console.WriteLine("3. Wait until the Gmail inbox loads, then press Enter here...");
Console.ReadLine();

await context.StorageStateAsync(new() { Path = statePath });

Console.WriteLine();
Console.WriteLine($"Saved storage state to {statePath}");
Console.WriteLine("Run tests with: dotnet test");

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
            "Could not find repo root. Run from the hello-playwright directory.");
    }

    private static bool IsRepoRoot(string path) =>
        Directory.Exists(Path.Combine(path, "src", "HelloPlaywright.AuthSetup")) &&
        Directory.Exists(Path.Combine(path, "auth"));
}
