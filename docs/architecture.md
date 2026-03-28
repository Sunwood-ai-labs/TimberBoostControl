# Architecture

## Runtime architecture

The codebase is split into a few focused pieces:

- `Source/TimberBoostControlStarter.cs`: initializes the mod folder, ensures `settings.json` exists, and regenerates blueprint overrides at startup.
- `Source/TimberBoostControlConfigurator.cs`: wires the settings store, generator, panel, and bottom-bar module through Bindito.
- `Source/TimberBoostControlPanel.cs`: renders the read-only panel, shows the resolved settings path, and exposes the reload action.
- `Source/TimberBoostControlGenerator.cs`: reads `Blueprints.zip`, writes character and building overrides, and tracks generated files.
- `Source/TimberBoostControlSettingsStore.cs`: loads, saves, normalizes, upgrades persisted settings, and now logs a warning when invalid JSON forces a fallback.

## Visual assets

`Assets/UI/boost-icon.png` is used by the in-game launcher. The button falls back to the text label `Boost` if the icon cannot be loaded.

## Build script notes

`build.ps1` compiles the DLL directly with `csc.exe` and references Timberborn-managed assemblies from the local game install. The current script also includes the Unity image modules needed for the button icon loading flow.

## Repository layout

- `Source/`: C# sources for the DLL mod
- `Assets/`: runtime UI assets such as the launcher icon
- `Buildings/` and `Characters/`: generated override output directories
- `docs/`: VitePress documentation site
- `scripts/validate-repo.ps1`: structural QA used locally and in CI

## Recommended verification

When you change the repo:

1. Run `powershell -ExecutionPolicy Bypass -File .\build.ps1` if you have Timberborn installed locally.
2. Run `npm install` once, then `npm run validate`.
3. Smoke-check that `settings.json` reload regenerates files and that the bottom-bar launcher still appears.
