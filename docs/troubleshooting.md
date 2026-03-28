# Troubleshooting

## The panel shows old values

- Confirm you edited the active `settings.json` path shown inside the panel.
- Click `Reload settings.json` after saving the file.
- Restart Timberborn to apply regenerated gameplay data.

## The game does not find Timberborn during build

- Pass `-GameRoot "C:\Path\To\Timberborn"` to `build.ps1`.
- Confirm `Timberborn_Data\Managed` exists under that path.
- Confirm `C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe` exists.

## The launcher icon is missing

- Make sure `Assets/UI/boost-icon.png` is present in the mod folder.
- If the icon still cannot be loaded, the launcher falls back to the `Boost` text label.

## settings.json is invalid

- The loader falls back to built-in defaults when the JSON cannot be parsed.
- A warning is logged so the fallback is visible in the game log.
- Check the file for missing commas, comments, or invalid trailing characters.
- After fixing the file, use `Reload settings.json` again.
