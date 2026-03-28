# TimberBoostControl v0.1.0 導入ガイド

![TimberBoostControl v0.1.0 リリースバナー](/releases/release-header-v0.1.0.svg)

このガイドは本リリースで確立される導線と、実行時の最小検証手順をまとめたものです。

## 前提

- Windows 環境
- Timberborn `1.0.x`
- `C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe` が利用可能
- リポジトリの [Architecture](/ja/architecture) で示す構成が揃っている状態

## 1) `Code.dll` のビルド

リポジトリルートから実行します。

```powershell
powershell -ExecutionPolicy Bypass -File .\build.ps1
```

Timberborn の場所が標準外なら `-GameRoot` を指定します。

```powershell
powershell -ExecutionPolicy Bypass -File .\build.ps1 -GameRoot "C:\Path\To\Timberborn"
```

## 2) MOD の配置

1. 本体フォルダを `Documents\Timberborn\Mods\TimberBoostControl` にコピーする。
2. Timberborn の Mod Manager で有効化する。
3. 起動時ログで起動ジェネレーター処理が呼ばれたことを確認する。

起動時処理は `TimberBoostControlStarter` が担当し、設定読込後すぐに生成処理を起動します。

## 3) in-game パネル確認

1. bottom-bar の起動ボタン（`Assets/UI/boost-icon.png` / `Boost`）を確認する。
2. クリックしてパネルを開く。
3. 読み取り専用行に現在の設定値と `settings.json` の解決パスが表示されることを確認する。

`TimberBoostControlPanel` は編集不可の読み取りビューとして実装され、再読込はボタン経由でのみ実行します。

## 4) 設定変更と再生成

1. `settings.json` を文字ベース編集する（数値キー）。
2. パネルの **Reload settings.json** を押す。
3. ステータスメッセージと通知を確認する。
4. `.generated-files.txt` が生成され、対象パスが更新されることを確認する。

ジェネレーターは追跡済みファイルを先に削除し、次に現在の上書き結果を再作成します。

## 5) ゲーム適用

1. 保存後、パネルで再読込を実施。
2. Timberborn を再起動する。

変更した `.blueprint.json` の反映は再起動後にゲーム側反映される想定です。

## コード対応場所

- 起動・初期化: `Source/TimberBoostControlStarter.cs`
- 設定読み込み/保存/互換: `Source/TimberBoostControlSettingsStore.cs`
- UI 表示と再読込: `Source/TimberBoostControlPanel.cs`
- 生成処理: `Source/TimberBoostControlGenerator.cs`
- bottom-bar 連携: `Source/TimberBoostControlBottomBarButton.cs`
- 依存登録: `Source/TimberBoostControlConfigurator.cs`

## 補足

- 無効な `settings.json` は安全側へフォールバックされ、既定値で生成されます。
- 旧式の boolean キーは互換読み込みされます。
- 変更がない場合は `.generated-files.txt` が更新されないケースがあります。
- ここでは GitHub の本体リリース作成（タグ・release body）は対象外です。

## 関連情報

- [v0.1.0 リリースノート](/ja/releases/v0.1.0)
- [はじめに](/ja/getting-started)
- [設定](/ja/settings)
