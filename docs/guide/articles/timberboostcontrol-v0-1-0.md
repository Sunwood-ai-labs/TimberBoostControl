# TimberBoostControl v0.1.0 walkthrough

![TimberBoostControl v0.1.0 release banner](/releases/release-header-v0.1.0.svg)

This walkthrough documents the v0.1.0 setup and runtime checks used by this repository.

## Prerequisites

- Windows with Timberborn `1.0.x`.
- C# compiler at `C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe`.
- A local copy of this repo and [build script requirements](/architecture) satisfied.

## 1) Build `Code.dll`

From repository root:

```powershell
powershell -ExecutionPolicy Bypass -File .\build.ps1
```

If the game is not in a standard Steam path, pass `-GameRoot`:

```powershell
powershell -ExecutionPolicy Bypass -File .\build.ps1 -GameRoot "C:\Path\To\Timberborn"
```

## 2) Install or link the mod

1. Copy the repo folder into `Documents\Timberborn\Mods\TimberBoostControl`.
2. In Timberborn Mod Manager, enable the mod.
3. Start the game and confirm startup logs include the startup generator message.

The startup generator path is in `TimberBoostControlStarter` and runs immediately after reading settings.

## 3) Open the in-game panel

1. Look for the bottom-bar launcher (icon `Assets/UI/boost-icon.png`, tooltip **Timber Boost Control**).
2. Open the panel by clicking it.
3. Confirm the read-only rows show values from `settings.json`.

`TimberBoostControlPanel` keeps the panel read-only by design and displays the resolved path to `settings.json`.

## 4) Tune settings and regenerate

1. Edit `settings.json` in the mod folder using numeric values.
2. Click **Reload settings.json** in the panel.
3. Confirm status updates and optionally the quick notification appears.
4. Check `.generated-files.txt` exists and contains generated `.blueprint.json` paths.

The generator reads tracked files, removes old tracked outputs, then rewrites only current outputs.

## 5) Apply generated data

To apply regenerated blueprint overrides in-game:

1. Save your edits, reopen the panel if needed.
2. Click **Reload settings.json**.
3. Restart Timberborn.

This step is required because regenerated gameplay data is reloaded on game restart.

## Where this maps in code

- Startup/bootstrap: `Source/TimberBoostControlStarter.cs`
- Settings handling: `Source/TimberBoostControlSettingsStore.cs`
- Runtime UI and reload action: `Source/TimberBoostControlPanel.cs`
- Generation pipeline: `Source/TimberBoostControlGenerator.cs`
- Bottom-bar wiring: `Source/TimberBoostControlBottomBarButton.cs`
- Dependency registration: `Source/TimberBoostControlConfigurator.cs`

## Known limitations and checks

- The mod normalizes values before generation; invalid JSON falls back to defaults.
- Legacy boolean keys are still accepted for compatibility.
- When no overrides are needed, generation can complete with no tracked files.
- The release is intentionally scoped to the core workflow above; there are no GitHub release assets or tag operations required for this docs step.

## Related docs

- [Release notes for v0.1.0](/releases/v0.1.0)
- [Getting Started](/getting-started)
- [Settings](/settings)
