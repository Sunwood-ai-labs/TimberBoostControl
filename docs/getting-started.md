# Getting Started

## What the mod does

TimberBoostControl adds a launcher to the Timberborn bottom bar, loads `settings.json`, and generates `.blueprint.json` overrides inside the mod folder. The in-game panel is a read-only status surface: it shows the current values, exposes the resolved `settings.json` path, and lets you reload the file to regenerate overrides.

## Requirements

- Windows
- Timberborn `1.0.x`
- `csc.exe` from `.NET Framework 4.x` at `C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe`
- A local Timberborn installation whose `Timberborn_Data\Managed` folder can be resolved by `build.ps1`

## Build the DLL

```powershell
powershell -ExecutionPolicy Bypass -File .\build.ps1
```

If Timberborn is installed in a non-standard location, pass the game root explicitly:

```powershell
powershell -ExecutionPolicy Bypass -File .\build.ps1 -GameRoot "C:\Path\To\Timberborn"
```

## Install the mod

1. Build `Code.dll`.
2. Copy this repository into `Documents\Timberborn\Mods\TimberBoostControl`, or create a junction that points there.
3. Start Timberborn and enable the mod in Mod Manager.

## Runtime flow

1. On startup, the mod initializes its working directory and makes sure `settings.json` exists.
2. The starter loads the settings and immediately regenerates blueprint overrides from them.
3. During gameplay, open the bottom-bar launcher to inspect the active values.
4. Edit `settings.json` outside the game or while the game is paused.
5. Click `Reload settings.json` in the panel.
6. Restart Timberborn so the regenerated gameplay data is fully applied.

## Generated files

The generator cleans up files listed in `.generated-files.txt`, writes the current set of overrides, and then refreshes the tracking file. Typical outputs are:

- `Buildings/**.blueprint.json`
- `Characters/**.blueprint.json`
- `.generated-files.txt`

This keeps the mod folder deterministic and makes it safe to regenerate repeatedly.
