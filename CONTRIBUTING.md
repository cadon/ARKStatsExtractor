# Contributing to ARK Smart Breeding

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- PowerShell 7+ (`pwsh`) — for the build script
- Windows — the application targets `net10.0-windows` (WinForms + WPF APIs)

---

## Building

All build steps go through `build.ps1` at the repo root. Run it from PowerShell:

```powershell
.\build.ps1
```

This will:
1. Regenerate `ARKBreedingStats/_manifest.json` from the current version and JSON data files
2. Build the entire solution
3. Run the test suite

### Build parameters

| Parameter | Default | Description |
|---|---|---|
| `-Configuration` | `Debug` | `Debug` or `Release` |
| `-Clean` | off | Forces a non-incremental rebuild |
| `-SkipTests` | off | Skips running tests after build |

**Examples:**

```powershell
# Normal development build + tests
.\build.ps1

# Clean rebuild
.\build.ps1 -Clean

# Build without running tests
.\build.ps1 -SkipTests

# Full release package (see below)
.\build.ps1 -Configuration Release
```

---

## Debug vs Release

### Debug (default)

- Standard compiler optimisations off, full debug symbols
- Builds in-place: `ARKBreedingStats\bin\Debug\net10.0-windows\`
- No packaging step — run the exe directly from the bin folder
- Used for day-to-day development and all CI builds on PRs

### Release

Everything Debug does, plus:

1. **Publish** — `dotnet publish` copies the trimmed, self-sufficient output to `.work\bin\`
2. **Zip** — creates `ARK.Smart.Breeding_<version>.zip` in `.work\publish\`
3. **Installer** — runs [Inno Setup](https://jrsoftware.org/isinfo.php) to produce `setup-ArkSmartBreeding-<version>.exe` in `.work\publish\`

Inno Setup is downloaded automatically the first time if it isn't already installed system-wide (pinned to a specific version, cached in `.work\innosetup\`).

The `.work\` directory is gitignored.

---

## Tests

Tests live in `ARKBreedingStats.Tests/` and use MSTest. They are run automatically at the end of every `build.ps1` invocation unless `-SkipTests` is specified.

To run tests directly:

```powershell
dotnet test ARKBreedingStats.Tests\ARKBreedingStats.Tests.csproj --configuration Debug
```

---

## Project structure

| Project | Framework | Description |
|---|---|---|
| `ARKBreedingStats` | `net10.0-windows` | Main WinForms application |
| `ASB-Updater` | `net10.0-windows` | WPF updater executable, copied to output at build |
| `ARKBreedingStats.Tests` | `net10.0-windows` | MSTest suite |
| `ArkSavegameToolkit/SavegameToolkit` | `netstandard2.0` | Savegame parsing library |
| `ArkSavegameToolkit/SavegameToolkitAdditions` | `netstandard2.0` | Savegame parsing extensions |

---

## Version

The application version is defined once in `ARKBreedingStats/ARKBreedingStats.csproj`:

```xml
<Version>0.72.1.0</Version>
```

This is the only place that needs updating for a version bump. The build script, manifest, zip filename, and installer all derive the version from this value automatically.

---

## CI

GitHub Actions runs on every push and pull request to `dev`. The workflow (`.github/workflows/ci.yml`) runs `build.ps1 -Configuration Release` and uploads the packaged artifacts.

A GitHub Release can be created by triggering the workflow manually with the **Publish a GitHub Release** option enabled.
