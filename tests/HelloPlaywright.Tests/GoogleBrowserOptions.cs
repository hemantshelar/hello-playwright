using Microsoft.Playwright;

namespace HelloPlaywright.Tests;

internal static class GoogleBrowserOptions
{
    private static readonly string[] IgnoreAutomationArgs = ["--enable-automation"];

    private static readonly string[] ReduceDetectionArgs =
    [
        "--disable-blink-features=AutomationControlled",
    ];

    public static BrowserTypeLaunchOptions ForTests() =>
        new()
        {
            Headless = true,
            Channel = ResolveChannel(),
            IgnoreDefaultArgs = IgnoreAutomationArgs,
            Args = ReduceDetectionArgs,
        };

    private static string ResolveChannel()
    {
        var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        if (Directory.Exists(Path.Combine(programFiles, "Google", "Chrome", "Application")))
            return "chrome";

        var programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        if (Directory.Exists(Path.Combine(programFilesX86, "Microsoft", "Edge", "Application"))
            || Directory.Exists(Path.Combine(programFiles, "Microsoft", "Edge", "Application")))
            return "msedge";

        throw new InvalidOperationException(
            "Google Chrome or Microsoft Edge must be installed for Gmail tests.");
    }
}
