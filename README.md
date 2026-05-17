# hello-playwright

.NET Playwright tests: Gmail (saved session) and Red Energy (automated login via `.env`).

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- **Google Chrome** or **Microsoft Edge** (required — Google blocks Playwright’s bundled Chromium at sign-in)
- Optional: [Docker Desktop](https://www.docker.com/products/docker-desktop/) with **MCP Toolkit** → **Playwright** enabled for exploring Gmail UI in Cursor

## Setup

```powershell
cd c:\Users\User\code\mygithub\hello-playwright
dotnet build
```

## One-time: save Google session

```powershell
dotnet run --project src/HelloPlaywright.AuthSetup
```

1. A browser opens at Gmail.
2. Sign in manually (including 2FA if your account uses it).
3. Wait until the inbox loads.
4. Press **Enter** in the terminal.

This writes `auth/google-storage.json` (gitignored; contains session cookies).

## Run Gmail test

```powershell
dotnet test --filter "FullyQualifiedName~GmailTests"
```

The test loads the saved session and checks that Gmail opens signed in (Compose button visible).

If the session expires, run the auth setup again.

## Red Energy: electricity bill test

Automated login on each run. Credentials are read from a `.env` file at the repo root (never committed).

```powershell
copy .env.example .env
# Edit .env: set REDENERGY_EMAIL and REDENERGY_PASSWORD

dotnet test --filter "FullyQualifiedName~RedEnergyBill"
```

The test logs into [Red Energy My Account](https://www.redenergy.com.au/myaccount/) and **passes** when `span.price` shows **Nil** (bill paid).

Run all tests:

```powershell
dotnet test
```

## Troubleshooting: "This browser or app may not be secure"

Google rejects automated Chromium. This project works around that by:

- Opening **installed Chrome or Edge** (`channel: chrome` / `msedge`), not Playwright Chromium
- Using a **persistent profile** at `auth/chrome-profile/` during login
- Disabling common automation flags (`--enable-automation`, `AutomationControlled`)

If sign-in still fails:

1. Confirm **Google Chrome** is installed and close other Chrome windows before running auth setup.
2. Delete `auth/chrome-profile/` and run auth setup again.
3. Sign in at [https://mail.google.com](https://mail.google.com) in normal Chrome first, then run auth setup.
4. As a last resort, save state from Playwright CLI using your installed browser:
   ```powershell
   pwsh tests/HelloPlaywright.Tests/bin/Debug/net8.0/playwright.ps1 codegen https://mail.google.com --save-storage=auth/google-storage.json --channel=chrome
   ```

## Project layout

```
HelloPlaywright.sln
src/HelloPlaywright.AuthSetup/   # manual login → auth/google-storage.json
tests/HelloPlaywright.Tests/     # Gmail + Red Energy Playwright tests
auth/                            # chrome-profile + google-storage.json (not committed)
.env.example                     # template for Red Energy credentials
```

## Security

- Never commit `auth/google-storage.json` or `.env`.
- Gmail: no passwords in code — only browser storage state from manual login.
- Red Energy: use `REDENERGY_EMAIL` and `REDENERGY_PASSWORD` in local `.env` only (see `.env.example`).
