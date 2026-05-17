using Microsoft.Playwright;

namespace HelloPlaywright.Tests;

public class RedEnergyBillTests
{
    private const int DefaultTimeoutMs = 60_000;

    [Fact]
    public async Task ElectricityBill_IsPaid_WhenBalanceShowsNil()
    {
        var (email, password) = RedEnergyCredentials.FromEnvironment();

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(GoogleBrowserOptions.ForTests());
        await using var context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            // Desktop layout exposes .login-button; mobile header only shows "LOGIN".
            ViewportSize = new ViewportSize { Width = 1280, Height = 720 },
        });
        var page = await context.NewPageAsync();
        page.SetDefaultTimeout(DefaultTimeoutMs);
        page.SetDefaultNavigationTimeout(DefaultTimeoutMs);

        await page.GotoAsync("https://www.redenergy.com.au/myaccount/");

        // Site uses <motion class="login-button"> — not a button role (see DevTools).
        await page.Locator(".login-button-container .login-button").ClickAsync();

        // Popup: <a class="button small login-link" href=".../signin">Login to myaccount</a>
        var loginPopup = page.Locator(".login-button-container--open .login-reveal");
        await Assertions.Expect(loginPopup).ToBeVisibleAsync();
        await loginPopup.Locator("a.login-link[href*='signin']").ClickAsync();

        // OAuth redirect goes to login.redenergy.com.au — not a URL containing "signin".
        var username = page.Locator("#okta-signin-username");
        await Assertions.Expect(username).ToBeVisibleAsync(new() { Timeout = DefaultTimeoutMs });
        await username.FillAsync(email);

        await page.Locator("input#okta-signin-submit.button-primary[value='Next']").ClickAsync();

        // Okta assigns dynamic ids (e.g. input108); name/class are stable on the password step.
        var passwordField = page.Locator("input[name='password'].password-with-toggle");
        await Assertions.Expect(passwordField).ToBeVisibleAsync(new() { Timeout = DefaultTimeoutMs });
        await passwordField.FillAsync(password);

        await page.Locator("input.button.button-primary[type='submit'][value='login']").ClickAsync();

        var nilBalance = page.Locator("span.price").Filter(new() { HasText = "Nil" });
        await Assertions.Expect(nilBalance).ToBeVisibleAsync(new() { Timeout = DefaultTimeoutMs });
        await Assertions.Expect(nilBalance).ToHaveTextAsync("Nil");
    }
}
