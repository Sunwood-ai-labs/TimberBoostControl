# 開発メモ

## 実行時アーキテクチャ

コードは役割ごとに分かれています。

- `Source/TimberBoostControlStarter.cs`: MOD フォルダの初期化、`settings.json` の存在保証、起動時の override 再生成
- `Source/TimberBoostControlConfigurator.cs`: Bindito による settings store / generator / panel / bottom-bar module の配線
- `Source/TimberBoostControlPanel.cs`: 読み取り専用パネル、設定ファイルパス表示、再読み込み操作
- `Source/TimberBoostControlGenerator.cs`: `Blueprints.zip` を読み、建物やキャラクターの override を出力
- `Source/TimberBoostControlSettingsStore.cs`: 設定の読み書き、正規化、旧形式からの移行、不正 JSON 時の警告ログ出力

## ビジュアル資産

`Assets/UI/boost-icon.png` はゲーム内ランチャーで使うアイコンです。読み込みに失敗した場合は `Boost` テキストボタンにフォールバックします。

## ビルドスクリプトの補足

`build.ps1` は `csc.exe` を直接呼び出して DLL をコンパイルします。現在のスクリプトは、ランチャーのアイコン読み込みに必要な Unity の image module 参照も含めています。

## リポジトリ構成

- `Source/`: DLL MOD の C# ソース
- `Assets/`: ランチャーアイコンなどの runtime 資産
- `Buildings/` と `Characters/`: 生成される override 出力先
- `docs/`: VitePress ドキュメント
- `scripts/validate-repo.ps1`: ローカルと CI で使う構造 QA スクリプト

## 推奨検証

リポジトリを更新したら、次の順で確認するのがおすすめです。

1. Timberborn がある環境では `powershell -ExecutionPolicy Bypass -File .\build.ps1` を実行する
2. `npm install` 後に `npm run validate` を実行する
3. `settings.json` の再読み込みでファイル再生成と bottom bar ランチャーが維持されることを確認する
