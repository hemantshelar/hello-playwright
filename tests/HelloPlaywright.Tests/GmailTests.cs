using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace HelloPlaywright.Tests;

public class GmailTests
{
    private const int DefaultTimeoutMs = 60_000;

    [Fact]
    public async Task Gmail_LoadsInbox_WhenStorageStatePresent()
    {
        var statePath = RepoPaths.GoogleStorageStatePath();
        Assert.True(
            File.Exists(statePath),
            $"Storage state not found at {statePath}. " +
            "Run once: dotnet run --project src/HelloPlaywright.AuthSetup");

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(GoogleBrowserOptions.ForTests());
        await using var context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            StorageStatePath = statePath,
        });

        var page = await context.NewPageAsync();
        page.SetDefaultTimeout(DefaultTimeoutMs);
        page.SetDefaultNavigationTimeout(DefaultTimeoutMs);

        await page.GotoAsync("https://mail.google.com");

        await Assertions.Expect(page).ToHaveURLAsync(
            new Regex(@"mail\.google\.com"),
            new PageAssertionsToHaveURLOptions { Timeout = DefaultTimeoutMs });

        // Compose is unique in the Gmail UI (unlike "Inbox", which also matches message-row links).
        await Assertions.Expect(page.GetByRole(AriaRole.Button, new() { Name = "Compose" }))
            .ToBeVisibleAsync(new() { Timeout = DefaultTimeoutMs });
    }
}
