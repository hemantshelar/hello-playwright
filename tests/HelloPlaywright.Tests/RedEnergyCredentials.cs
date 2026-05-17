namespace HelloPlaywright.Tests;

internal static class RedEnergyCredentials
{
    private static bool _envLoaded;

    public static (string Email, string Password) FromEnvironment()
    {
        EnsureEnvLoaded();

        var email = Environment.GetEnvironmentVariable("REDENERGY_EMAIL");
        var password = Environment.GetEnvironmentVariable("REDENERGY_PASSWORD");

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "Set REDENERGY_EMAIL and REDENERGY_PASSWORD in a .env file at the repo root " +
                "(copy from .env.example) or as environment variables.");
        }

        return (email, password);
    }

    private static void EnsureEnvLoaded()
    {
        if (_envLoaded)
            return;

        var envPath = Path.Combine(RepoPaths.FindRoot(), ".env");
        if (File.Exists(envPath))
            DotNetEnv.Env.Load(envPath);

        _envLoaded = true;
    }
}
