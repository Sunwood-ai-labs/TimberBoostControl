# Settings

## JSON schema

`settings.json` stores numeric values. The code keeps the legacy boolean keys readable for compatibility, but new saves should use the numeric schema below.

The built-in code defaults are conservative. The committed `settings.json` in this repository currently uses a stronger showcase preset with `CarryMultiplier=100`, `MoveSpeedPercent=500`, and `StorageMultiplier=100`.

| Key | Code default | Effect | Notes |
| --- | --- | --- | --- |
| `CarryMultiplier` | `10` | Multiplies `GoodCarrierSpec.BaseLiftingCapacity`. | Use `1` for vanilla behavior. |
| `MoveSpeedPercent` | `200` | Scales walking and slowed walking speeds. | `100` keeps vanilla values. |
| `StorageMultiplier` | `10` | Multiplies `StockpileSpec.MaxCapacity`. | Affects storage blueprints that expose `StockpileSpec`. |
| `BuildCostPercent` | `10` | Scales `BuildingSpec.BuildingCost`. | Uses ceiling rounding so small costs do not disappear accidentally. |
| `ScienceCostPercent` | `0` | Scales `BuildingSpec.ScienceCost`. | `0` makes science free. |
| `FactoryWorkerMultiplier` | `2` | Expands `WorkplaceSpec` worker counts. | The UI label says `Workplace workers` because the generator now applies to general workplaces, not only factories. |
| `PowerInputPercent` | `10` | Scales `MechanicalNodeSpec.PowerInput`. | Values above `0` are clamped to stay positive. |

## Example file

![settings.json screenshot](/screenshots/timberboostcontrol-settings-json.png)

## Normalization rules

The settings object normalizes each field before generation:

- multipliers stay at `1` or higher where vanilla needs a positive count
- percentage-based values stay at `0` or higher where zero is valid
- invalid or unreadable JSON falls back to the built-in defaults

## Legacy compatibility

`TimberBoostControlSettingsStore` still maps the older boolean keys when they appear:

- `CarryTenX`
- `MoveTwoX`
- `StorageTenX`
- `BuildCostTenth`
- `FreeScience`
- `DoubleFactoryWorkers`
- `PowerTenth`

That means the mod can load older local files and translate them into the new numeric model automatically.
