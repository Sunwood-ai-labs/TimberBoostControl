# Release QA Inventory

## Release Context

- repository: `TimberBoostControl`
- release tag: `v0.1.0`
- compare range: `<none>; initial release mode from root commit c00afca5608cad6887e48bb42c54ff333f209815 to release commit d4bee65e6def62325a4e0b2b31d353e5270d365e`
- requested outputs: GitHub release body, docs-backed release notes, companion walkthrough article
- validation commands run: `npm run validate`, `powershell -ExecutionPolicy Bypass -File .\scripts\validate-repo.ps1`, `powershell -ExecutionPolicy Bypass -File D:\Prj\gh-release-notes-skill\scripts\verify-svg-assets.ps1 -RepoPath . -Path docs/public/brand/boost-emblem.svg,docs/public/releases/release-header-v0.1.0.svg`, `powershell -ExecutionPolicy Bypass -File .\build.ps1`
- release URLs: `https://github.com/Sunwood-ai-labs/TimberBoostControl/releases/tag/v0.1.0`, `https://sunwood-ai-labs.github.io/TimberBoostControl/releases/v0.1.0`, `https://sunwood-ai-labs.github.io/TimberBoostControl/guide/articles/timberboostcontrol-v0-1-0`, `https://sunwood-ai-labs.github.io/TimberBoostControl/ja/releases/v0.1.0`, `https://sunwood-ai-labs.github.io/TimberBoostControl/ja/guide/articles/timberboostcontrol-v0-1-0`

## Claim Matrix

| claim | code refs | validation refs | docs surfaces touched | scope |
| --- | --- | --- | --- | --- |
| Startup bootstraps the mod path, creates `settings.json` when needed, then regenerates overrides during startup. | `Source/TimberBoostControlStarter.cs`, `Source/ModContext.cs` | `powershell -ExecutionPolicy Bypass -File .\build.ps1`, source inspection | `none` | `release_note` |
| Settings loading normalizes numeric values and still accepts legacy boolean keys for compatibility. | `Source/TimberBoostControlSettingsStore.cs`, `Source/TimberBoostControlSettings.cs` | source inspection, `powershell -ExecutionPolicy Bypass -File .\build.ps1` | `none` | `release_note` |
| The runtime UI exposes a bottom-bar `Boost` launcher, icon fallback, and a read-only reload panel. | `Source/TimberBoostControlBottomBarButton.cs`, `Source/TimberBoostControlPanel.cs` | source inspection, `powershell -ExecutionPolicy Bypass -File .\build.ps1` | `none` | `release_note` |
| Generation removes previously tracked outputs, rewrites current blueprint overrides, and refreshes `.generated-files.txt`. | `Source/TimberBoostControlGenerator.cs` | source inspection, `powershell -ExecutionPolicy Bypass -File .\build.ps1` | `none` | `release_note` |
| Steady-state entry points now route readers to the live v0.1.0 release collateral in both locales. | `README.md`, `README.ja.md`, `docs/index.md`, `docs/ja/index.md`, `docs/.vitepress/config.mts` | `npm run validate`, `powershell -ExecutionPolicy Bypass -File .\scripts\validate-repo.ps1`, live URL checks | `README.md, README.ja.md, docs/index.md, docs/ja/index.md, docs/.vitepress/config.mts` | `steady_state` |

## Steady-State Docs Review

| surface | status | evidence |
| --- | --- | --- |
| README.md | pass | Added live English links for the v0.1.0 release notes and walkthrough article |
| README.ja.md | pass | Added live Japanese links for the v0.1.0 release notes and walkthrough article |
| docs/index.md | pass | Added a Latest release block instead of extra hero actions to surface v0.1.0 collateral |
| docs/ja/index.md | pass | Added a localized Latest release block for the Japanese v0.1.0 collateral |
| docs/.vitepress/config.mts | pass | Added localized Releases nav/sidebar entry points and kept canonical notes separate from the walkthrough surface |

## QA Inventory

| criterion_id | status | evidence |
| --- | --- | --- |
| compare_range | pass | `powershell -ExecutionPolicy Bypass -File D:\Prj\gh-release-notes-skill\scripts\collect-release-context.ps1 -Target HEAD` reported initial release mode from root commit `c00afca5608cad6887e48bb42c54ff333f209815`; `git ls-remote --tags origin` and `gh release list --limit 10` showed no prior tags/releases before publish |
| release_claims_backed | pass | Inspected `Source/TimberBoostControlStarter.cs`, `Source/TimberBoostControlSettingsStore.cs`, `Source/TimberBoostControlSettings.cs`, `Source/TimberBoostControlGenerator.cs`, `Source/TimberBoostControlBottomBarButton.cs`, `Source/TimberBoostControlPanel.cs`, `build.ps1`, and `manifest.json` |
| docs_release_notes | pass | `docs/releases/index.md, docs/releases/v0.1.0.md, docs/ja/releases/index.md, docs/ja/releases/v0.1.0.md` |
| companion_walkthrough | pass | `docs/guide/articles/timberboostcontrol-v0-1-0.md, docs/ja/guide/articles/timberboostcontrol-v0-1-0.md` |
| operator_claims_extracted | pass | Claim matrix completed above and truth-sync mirrored into `README.md`, `README.ja.md`, `docs/index.md`, `docs/ja/index.md`, and `docs/.vitepress/config.mts` |
| impl_sensitive_claims_verified | pass | Verified startup bootstrap, JSON fallback, legacy key mapping, generator cleanup, bottom-bar icon fallback, and reload messaging against source files; rebuilt `Code.dll` with `powershell -ExecutionPolicy Bypass -File .\build.ps1` |
| steady_state_docs_reviewed | pass | `README.md, README.ja.md, docs/index.md, docs/ja/index.md, docs/.vitepress/config.mts` reviewed and updated in the table above |
| claim_scope_precise | pass | Canonical release notes explicitly say the tag covers the full initial shipped history; GitHub release body and docs call out that in-game smoke testing was not run |
| latest_release_links_updated | pass | Added v0.1.0 collateral links in `README.md`, `README.ja.md`, `docs/index.md`, `docs/ja/index.md`, and localized Releases nav/sidebar entries in `docs/.vitepress/config.mts` |
| svg_assets_validated | pass | `powershell -ExecutionPolicy Bypass -File D:\Prj\gh-release-notes-skill\scripts\verify-svg-assets.ps1 -RepoPath . -Path docs/public/brand/boost-emblem.svg,docs/public/releases/release-header-v0.1.0.svg` |
| docs_assets_committed_before_tag | pass | Commit `d4bee65e6def62325a4e0b2b31d353e5270d365e` containing the release collateral was pushed to `origin/main` before `git tag -a v0.1.0 d4bee65e6def62325a4e0b2b31d353e5270d365e` |
| docs_deployed_live | pass | GitHub Actions run `23689832815` (`Deploy Docs`) completed successfully; `Invoke-WebRequest` returned `200` for the English/Japanese release notes, walkthrough pages, and `releases/release-header-v0.1.0.svg` |
| tag_local_remote | pass | `git tag -a v0.1.0 d4bee65e6def62325a4e0b2b31d353e5270d365e -m "v0.1.0"` and `git push origin refs/tags/v0.1.0` |
| github_release_verified | pass | `gh release create v0.1.0 --title "v0.1.0" --notes-file <temp>` published the release; verified with `gh api repos/Sunwood-ai-labs/TimberBoostControl/releases/tags/v0.1.0 --jq .body` and `gh release view v0.1.0` |
| validation_commands_recorded | pass | `npm run validate`, `powershell -ExecutionPolicy Bypass -File .\scripts\validate-repo.ps1`, `powershell -ExecutionPolicy Bypass -File D:\Prj\gh-release-notes-skill\scripts\verify-svg-assets.ps1 -RepoPath . -Path docs/public/brand/boost-emblem.svg,docs/public/releases/release-header-v0.1.0.svg`, `powershell -ExecutionPolicy Bypass -File .\build.ps1` |
| publish_date_verified | pass | `gh release view v0.1.0` reported `published: 2026-03-28T16:56:01Z` |

## Notes

- blockers: none
- waivers: in-game smoke testing was not run during this release task; the release body and docs collateral call this out explicitly
- follow-up docs tasks: consider updating `actions/configure-pages` and `actions/deploy-pages` for the Node 20 deprecation warnings surfaced in GitHub Actions
