# トラブルシューティング

## パネルの値が更新されない

- パネルに表示される `settings.json` パスが、実際に編集したファイルか確認します。
- 保存後に `Reload settings.json` を押します。
- 再生成された内容を完全に反映するため、Timberborn を再起動します。

## build.ps1 が Timberborn を見つけられない

- `build.ps1 -GameRoot "C:\Path\To\Timberborn"` を指定します。
- そのパスに `Timberborn_Data\Managed` があるか確認します。
- `C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe` が存在するか確認します。

## ランチャーアイコンが出ない

- MOD フォルダに `Assets/UI/boost-icon.png` が存在するか確認します。
- アイコンを読めない場合は `Boost` テキストボタンへフォールバックします。

## settings.json が壊れている

- JSON の解析に失敗すると、ローダーは組み込み既定値へ戻ります。
- その際はゲームログに警告が残るので、原因の切り分けに使えます。
- 余分なカンマ、コメント、閉じ忘れがないか確認します。
- 修正後にもう一度 `Reload settings.json` を押します。
