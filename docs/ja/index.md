---
layout: home

hero:
  name: TimberBoostControl
  text: Timberborn 用の JSON 駆動ブースト MOD
  tagline: bottom bar のパネルから状態を確認し、settings.json の再読み込みで blueprint override を再生成できる DLL MOD です。
  image:
    src: /boost-icon.png
    alt: TimberBoostControl boost icon
  actions:
    - theme: brand
      text: 導入ガイド
      link: /ja/getting-started
    - theme: alt
      text: 設定
      link: /ja/settings
    - theme: alt
      text: GitHub
      link: https://github.com/Sunwood-ai-labs/TimberBoostControl

features:
  - title: bottom bar から起動
    details: Timberborn の bottom bar にランチャーを追加します。アイコンが読めないときはテキスト表示に自動で切り替わります。
  - title: settings.json を再読み込み
    details: JSON を直接編集してからパネルで再読み込みすると、blueprint override をその場で再生成できます。
  - title: blueprint 生成を明示管理
    details: 生成ファイルは .generated-files.txt で追跡され、再生成時に安全に置き換えられます。
---

## このリポジトリで見られること

TimberBoostControl は、学習用として追いやすい規模を保ちながら、Timberborn DLL MOD の一連の流れをひと通り含んでいます。

- `IModStarter` による起動処理
- Bindito による依存解決
- bottom bar から開く UI
- 互換性を考慮した `settings.json` ロード
- mod フォルダ内へ書き出す blueprint override 生成

このリポジトリのワークフローと初期の MOD 土台は [timberborn-modding-skill](https://github.com/Sunwood-ai-labs/timberborn-modding-skill) の支援を受けて整備しています。

<div class="quick-facts">
  <div>
    <strong>対象ゲーム</strong>
    Timberborn 1.0.x
  </div>
  <div>
    <strong>実行モデル</strong>
    DLL MOD と生成済み blueprint override の組み合わせ
  </div>
  <div>
    <strong>基本フロー</strong>
    settings.json を編集して再読み込みし、ゲームを再起動
  </div>
</div>

## 最新リリース

- [v0.1.0 リリースノート](/ja/releases/v0.1.0) で、初回公開の範囲と出荷挙動を確認できます。
- [v0.1.0 導入記事](/ja/guide/articles/timberboostcontrol-v0-1-0) で、release 観点の導入と動作確認手順を追えます。

## ドキュメント案内

- [導入ガイド](/ja/getting-started) でビルド・配置・運用フローを確認できます。
- [設定](/ja/settings) で JSON キーと効果を確認できます。
- [アーキテクチャ](/ja/architecture) でコード構成と生成物の扱いを確認できます。
- [トラブルシューティング](/ja/troubleshooting) で JSON 破損やビルドエラー時の対処を確認できます。
