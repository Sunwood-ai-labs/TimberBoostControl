# TimberBoostControl

[English](./README.md)

TimberBoostControl は、Timberborn の `C# DLL MOD` と小さな `ゲーム内設定UI` を組み合わせた練習用プロジェクトです。固定のバランスMODを1つ配るのではなく、ゲーム中にいくつかの調整項目を切り替え、その内容に合わせた `.blueprint.json` を MOD フォルダへ自動生成します。

## ✨ 特徴

- ゲーム下部の `Boost` ボタンから設定パネルを開ける
- 設定内容を `settings.json` に保存
- MOD フォルダ内に実行用の blueprint override を生成
- 生成したファイルを `.generated-files.txt` で追跡
- セーブ後にゲーム再起動すると変更が反映

現在切り替えられる項目です。

- `10x carry capacity`
- `2x move speed`
- `10x storage capacity`
- `1/10 building cost`
- `0 science cost`
- `2x factory workers`
- `1/10 power input`

## 🗂️ 構成

- `Source/`: DLL MOD の C# ソース
- `build.ps1`: `Code.dll` をビルドするスクリプト
- `manifest.json`: Timberborn の MOD マニフェスト
- `settings.json`: UI の初期保存設定

`Buildings/`、`Characters/`、`Code.dll`、`.generated-files.txt` などの生成物は git では追跡しません。

## 🔧 ビルド方法

必要なものです。

- Windows
- ローカルにインストールされた Timberborn 1.0.x
- `C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe` が使える環境

DLL をビルドします。

```powershell
powershell -ExecutionPolicy Bypass -File .\build.ps1
```

Timberborn の場所が標準パスと違う場合は、`-GameRoot` を渡してください。

```powershell
powershell -ExecutionPolicy Bypass -File .\build.ps1 -GameRoot "C:\Path\To\Timberborn"
```

## 🚀 使い方

1. `Code.dll` をビルドします。
2. このフォルダを `Documents\Timberborn\Mods\TimberBoostControl` にコピーするか、ジャンクションでつなぎます。
   現在のローカル環境では、`C:\Users\Aslan\OneDrive\ドキュメント\Timberborn\Mods\TimberBoostControl` から `D:\Prj\TimberBoostControl` へのジャンクションで運用しています。
3. Timberborn を再起動して、Mod Manager でこの MOD を有効化します。
4. セーブデータに入り、下部バーの `Boost` ボタンを押して設定パネルを開き、項目を選んで `Save` を押します。
5. もう一度ゲームを再起動すると、生成された blueprint 変更が反映されます。

## 🧪 補足

- `IModStarter`、`Configurator`、`ILoadableSingleton`、`UILayout` の練習用サンプルとして作っています。
- 値をメモリ上で直接差し替えるのではなく、元 blueprint を読んで JSON override を生成する方式です。
- 設定を変えて `Save` し直せば、生成ファイルは安全に作り直されます。
