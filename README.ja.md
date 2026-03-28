<div align="center">
  <img src="./Assets/UI/boost-icon.png" alt="TimberBoostControl boost icon" width="156" />
  <h1>TimberBoostControl</h1>
  <p><strong>bottom bar から <code>settings.json</code> を再読み込みし、blueprint boost を再生成できる Timberborn DLL MOD です。</strong></p>
</div>

<p align="center">
  <img alt="Timberborn 1.0+" src="https://img.shields.io/badge/Timberborn-1.0%2B-7d5632?style=flat-square" />
  <img alt="C# DLL mod" src="https://img.shields.io/badge/C%23-DLL%20mod-355f9d?style=flat-square" />
  <img alt="Docs with VitePress" src="https://img.shields.io/badge/Docs-VitePress-2b6cb0?style=flat-square" />
  <img alt="License MIT" src="https://img.shields.io/badge/License-MIT-246b54?style=flat-square" />
</p>

<p align="center">
  <a href="./README.md">English</a>
  |
  <a href="./README.ja.md">日本語</a>
</p>

<p align="center">
  <a href="https://sunwood-ai-labs.github.io/TimberBoostControl/">Documentation</a>
  |
  <a href="https://github.com/Sunwood-ai-labs/TimberBoostControl">GitHub</a>
</p>

## 🚀 概要

TimberBoostControl は、C# DLL MOD と生成済み `.blueprint.json` override を組み合わせた Timberborn 用の学習向け MOD です。固定プリセットを焼き込むのではなく、`settings.json` の数値を読み取り、その値を bottom bar のパネルで確認しながら、対応する blueprint override を再生成できます。

## ✨ できること

- Timberborn の bottom bar に `Boost` ランチャーを追加する
- MOD フォルダ内の `settings.json` を読み込み、正規化する
- 読み取り専用のゲーム内パネルに現在値と解決済みパスを表示する
- JSON の現在値からキャラクターと建物の blueprint override を再生成する
- `.generated-files.txt` で生成ファイルを追跡し、次回再生成時に安全に置き換える

主な設定グループ:

- `CarryMultiplier`
- `MoveSpeedPercent`
- `StorageMultiplier`
- `BuildCostPercent`
- `ScienceCostPercent`
- `FactoryWorkerMultiplier`
- `PowerInputPercent`

互換性のため JSON キー名は `FactoryWorkerMultiplier` のままですが、現在の UI では適用範囲に合わせて `Workplace workers` と表示しています。

## 🧭 実行フロー

1. MOD の起動時に作業ディレクトリを解決します。
2. `settings.json` が無ければ自動作成します。
3. スターターが現在の設定から blueprint override を再生成します。
4. ゲーム中は bottom bar のランチャーから現在値を確認できます。
5. `settings.json` を編集し、`Reload settings.json` を押してから Timberborn を再起動すると、再生成した内容を完全に反映できます。

## 🛠️ ビルド

必要なもの:

- Windows
- ローカルにインストールされた Timberborn `1.0.x`
- `C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe` にある `.NET Framework` の C# コンパイラ

DLL のビルド:

```powershell
powershell -ExecutionPolicy Bypass -File .\build.ps1
```

Timberborn が標準とは別の場所にある場合は、`-GameRoot` を指定します。

```powershell
powershell -ExecutionPolicy Bypass -File .\build.ps1 -GameRoot "C:\Path\To\Timberborn"
```

## 🎮 導入と使い方

1. `Code.dll` をビルドします。
2. このリポジトリを `Documents\Timberborn\Mods\TimberBoostControl` にコピーするか、そこへ向けたジャンクションを作成します。
3. Timberborn の Mod Manager で有効化します。
4. bottom bar の `Boost` ランチャーを開きます。
5. 値を変更したいときは `settings.json` を直接編集します。
6. パネルの `Reload settings.json` を押します。
7. 再生成された gameplay data を反映するため、ゲームを再起動します。

## 📚 ドキュメント

- [導入ガイド](https://sunwood-ai-labs.github.io/TimberBoostControl/ja/getting-started)
- [設定](https://sunwood-ai-labs.github.io/TimberBoostControl/ja/settings)
- [アーキテクチャ](https://sunwood-ai-labs.github.io/TimberBoostControl/ja/architecture)
- [トラブルシューティング](https://sunwood-ai-labs.github.io/TimberBoostControl/ja/troubleshooting)

## 📁 リポジトリ構成

- `Source/`: DLL MOD の C# ソース
- `Assets/`: ランチャーアイコンなどの runtime UI 資産
- `docs/`: VitePress ベースのドキュメントサイト
- `scripts/validate-repo.ps1`: リポジトリ整備用の構造 QA スクリプト
- `build.ps1`: `Code.dll` をビルドするローカルスクリプト
- `manifest.json`: Timberborn MOD マニフェスト
- `settings.json`: MOD が読み込む永続設定ファイル

`Buildings/`、`Characters/`、`Code.dll`、`.generated-files.txt` などの生成物は意図的に git 管理外です。

## 🧪 リポジトリ QA

Node 依存を一度入れたあと、公開面は次のコマンドで確認できます。

```powershell
npm install
npm run validate
```

## 📝 補足

- このリポジトリは `IModStarter`、`Configurator`、`ILoadableSingleton`、`UILayout`、そして JSON 駆動のコンテンツ生成を学ぶ題材として作られています。
- gameplay 値をメモリ上で直接書き換えるのではなく、MOD フォルダに override ファイルを書き出す方式です。
- 古いローカル設定ファイルが残っていても、legacy boolean キーを読み替えられます。
