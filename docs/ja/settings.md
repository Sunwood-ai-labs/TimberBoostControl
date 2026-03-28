# 設定リファレンス

## JSON スキーマ

`settings.json` は数値ベースの設定を保持します。互換性のため旧来の boolean キーも読み込めますが、新しい保存形式としては以下の数値キーを使う想定です。

コード側の組み込み既定値は控えめですが、このリポジトリにコミットしている `settings.json` は `CarryMultiplier=100`、`MoveSpeedPercent=500`、`StorageMultiplier=100` の強めなサンプル値を使っています。

| キー | コード既定値 | 効果 | 補足 |
| --- | --- | --- | --- |
| `CarryMultiplier` | `10` | `GoodCarrierSpec.BaseLiftingCapacity` を倍率変更します。 | `1` でバニラ相当です。 |
| `MoveSpeedPercent` | `200` | 歩行速度と減速時速度を割合変更します。 | `100` でバニラ相当です。 |
| `StorageMultiplier` | `10` | `StockpileSpec.MaxCapacity` を倍率変更します。 | `StockpileSpec` を持つ blueprint に作用します。 |
| `BuildCostPercent` | `10` | `BuildingSpec.BuildingCost` を割合変更します。 | 端数は切り上げで処理します。 |
| `ScienceCostPercent` | `0` | `BuildingSpec.ScienceCost` を割合変更します。 | `0` で研究コストが無料になります。 |
| `FactoryWorkerMultiplier` | `2` | `WorkplaceSpec` の労働者数を拡張します。 | UI 表記は `Workplace workers` ですが、互換性のため JSON キー名はそのままです。 |
| `PowerInputPercent` | `10` | `MechanicalNodeSpec.PowerInput` を割合変更します。 | 正の値を保つように補正されます。 |

## 設定ファイルの例

![settings.json screenshot](/screenshots/timberboostcontrol-settings-json.png)

## 正規化ルール

生成前に各値は正規化されます。

- 人数や倍率系は `1` 未満になりません
- 割合系でゼロが妥当なものは `0` 未満になりません
- JSON が壊れている場合は組み込み既定値へ戻ります

## 旧設定との互換性

`TimberBoostControlSettingsStore` は、次の旧 boolean キーを見つけた場合に新しい数値モデルへ変換します。

- `CarryTenX`
- `MoveTwoX`
- `StorageTenX`
- `BuildCostTenth`
- `FreeScience`
- `DoubleFactoryWorkers`
- `PowerTenth`

そのため、過去のローカル設定ファイルが残っていても自動で読み替えられます。
