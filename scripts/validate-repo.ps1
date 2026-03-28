param(
    [string]$RepoRoot = "."
)

$ErrorActionPreference = "Stop"

$resolvedRepoRoot = (Resolve-Path $RepoRoot).Path

function Assert-Exists {
    param(
        [string]$RelativePath
    )

    $fullPath = Join-Path $resolvedRepoRoot $RelativePath
    if (-not (Test-Path $fullPath)) {
        throw "Required path was not found: $RelativePath"
    }
}

function Assert-Contains {
    param(
        [string]$RelativePath,
        [string]$Pattern,
        [string]$Description
    )

    $fullPath = Join-Path $resolvedRepoRoot $RelativePath
    $content = Get-Content -Raw $fullPath
    if ($content -notmatch $Pattern) {
        throw "$RelativePath is missing expected content: $Description"
    }
}

$requiredPaths = @(
    "README.md",
    "README.ja.md",
    "LICENSE",
    "manifest.json",
    "package.json",
    "docs/index.md",
    "docs/getting-started.md",
    "docs/settings.md",
    "docs/architecture.md",
    "docs/troubleshooting.md",
    "docs/releases/index.md",
    "docs/releases/v0.1.0.md",
    "docs/guide/articles/timberboostcontrol-v0-1-0.md",
    "docs/ja/index.md",
    "docs/ja/getting-started.md",
    "docs/ja/settings.md",
    "docs/ja/architecture.md",
    "docs/ja/troubleshooting.md",
    "docs/ja/releases/index.md",
    "docs/ja/releases/v0.1.0.md",
    "docs/ja/guide/articles/timberboostcontrol-v0-1-0.md",
    "docs/.vitepress/config.mts",
    "docs/.vitepress/theme/index.ts",
    "docs/.vitepress/theme/styles.css",
    "docs/public/brand/boost-emblem.svg",
    "docs/public/releases/release-header-v0.1.0.svg",
    "docs/public/screenshots/timberboostcontrol-bottom-bar-launcher.png",
    "docs/public/screenshots/timberboostcontrol-control-panel.png",
    "docs/public/screenshots/timberboostcontrol-settings-json.png",
    "Assets/Screenshots/timberboostcontrol-bottom-bar-launcher.png",
    "Assets/Screenshots/timberboostcontrol-control-panel.png",
    "Assets/Screenshots/timberboostcontrol-settings-json.png",
    ".github/workflows/ci.yml",
    ".github/workflows/deploy-docs.yml"
)

foreach ($requiredPath in $requiredPaths) {
    Assert-Exists $requiredPath
}

$manifest = Get-Content -Raw (Join-Path $resolvedRepoRoot "manifest.json") | ConvertFrom-Json
$requiredManifestFields = @("Name", "Version", "Id", "MinimumGameVersion", "Description")
foreach ($field in $requiredManifestFields) {
    if (-not ($manifest.PSObject.Properties.Name -contains $field)) {
        throw "manifest.json is missing required field: $field"
    }
}

$package = Get-Content -Raw (Join-Path $resolvedRepoRoot "package.json") | ConvertFrom-Json
$requiredScripts = @("docs:build", "docs:dev", "docs:preview", "lint:repo")
foreach ($scriptName in $requiredScripts) {
    if (-not ($package.scripts.PSObject.Properties.Name -contains $scriptName)) {
        throw "package.json is missing required script: $scriptName"
    }
}

Assert-Contains "README.md" "README\.ja\.md" "Japanese language switch link"
Assert-Contains "README.md" "https://sunwood-ai-labs\.github\.io/TimberBoostControl/" "published docs link"
Assert-Contains "README.md" "timberborn-modding-skill" "skill provenance note"
Assert-Contains "README.md" "Assets/Screenshots/timberboostcontrol-control-panel\.png" "README screenshot section"
Assert-Contains "README.md" "releases/v0\.1\.0" "English v0.1.0 release link"
Assert-Contains "README.md" "guide/articles/timberboostcontrol-v0-1-0" "English v0.1.0 walkthrough link"
Assert-Contains "README.ja.md" "README\.md" "English language switch link"
Assert-Contains "README.ja.md" "timberborn-modding-skill" "Japanese skill provenance note"
Assert-Contains "README.ja.md" "Assets/Screenshots/timberboostcontrol-settings-json\.png" "Japanese README screenshot section"
Assert-Contains "README.ja.md" "releases/v0\.1\.0" "Japanese v0.1.0 release link"
Assert-Contains "README.ja.md" "guide/articles/timberboostcontrol-v0-1-0" "Japanese v0.1.0 walkthrough link"
Assert-Contains "docs/index.md" "/getting-started" "English getting started action"
Assert-Contains "docs/index.md" "timberborn-modding-skill" "English docs home provenance note"
Assert-Contains "docs/index.md" "/releases/v0.1.0" "English v0.1.0 release notes link"
Assert-Contains "docs/index.md" "/guide/articles/timberboostcontrol-v0-1-0" "English v0.1.0 walkthrough article link"
Assert-Contains "docs/releases/index.md" "/releases/v0.1.0" "English release index entry"
Assert-Contains "docs/releases/v0.1.0.md" "Initial release note covers the full shipped history" "English initial release scope note"
Assert-Contains "docs/guide/articles/timberboostcontrol-v0-1-0.md" "Release notes for v0.1.0" "English walkthrough backlink"
Assert-Contains "docs/ja/index.md" "/ja/getting-started" "Japanese getting started action"
Assert-Contains "docs/ja/index.md" "timberborn-modding-skill" "Japanese docs home provenance note"
Assert-Contains "docs/ja/index.md" "/ja/releases/v0.1.0" "Japanese v0.1.0 release notes link"
Assert-Contains "docs/ja/index.md" "/ja/guide/articles/timberboostcontrol-v0-1-0" "Japanese v0.1.0 walkthrough article link"
Assert-Contains "docs/ja/releases/index.md" "/ja/releases/v0.1.0" "Japanese release index entry"
Assert-Contains "docs/ja/releases/v0.1.0.md" "Source/TimberBoostControlStarter\.cs" "Japanese release note code reference"
Assert-Contains "docs/ja/guide/articles/timberboostcontrol-v0-1-0.md" "/ja/releases/v0.1.0" "Japanese walkthrough backlink"
Assert-Contains "docs/getting-started.md" "/screenshots/timberboostcontrol-control-panel\.png" "English docs screenshots"
Assert-Contains "docs/settings.md" "/screenshots/timberboostcontrol-settings-json\.png" "English settings screenshot"
Assert-Contains "docs/architecture.md" "timberborn-modding-skill" "English architecture provenance note"
Assert-Contains "docs/ja/getting-started.md" "/screenshots/timberboostcontrol-control-panel\.png" "Japanese docs screenshots"
Assert-Contains "docs/ja/settings.md" "/screenshots/timberboostcontrol-settings-json\.png" "Japanese settings screenshot"
Assert-Contains "docs/ja/architecture.md" "timberborn-modding-skill" "Japanese architecture provenance note"
Assert-Contains "docs/.vitepress/config.mts" "/TimberBoostControl/" "GitHub Pages base path"
Assert-Contains "docs/.vitepress/config.mts" "/releases/" "Release index route in docs config"
Assert-Contains "docs/.vitepress/config.mts" "timberboostcontrol-v0-1-0" "Walkthrough route in docs config"
Assert-Contains ".github/workflows/deploy-docs.yml" "actions/deploy-pages@v4" "GitHub Pages deploy action"
Assert-Contains ".github/workflows/deploy-docs.yml" "docs/.vitepress/dist" "Pages artifact path"
Assert-Contains ".github/workflows/ci.yml" "npm run validate" "combined repo QA verification"

Write-Host "Repository polish validation passed for $resolvedRepoRoot"
