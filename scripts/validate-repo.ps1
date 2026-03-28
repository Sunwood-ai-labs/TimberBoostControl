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
    "docs/ja/index.md",
    "docs/ja/getting-started.md",
    "docs/ja/settings.md",
    "docs/ja/architecture.md",
    "docs/ja/troubleshooting.md",
    "docs/.vitepress/config.mts",
    "docs/.vitepress/theme/index.ts",
    "docs/.vitepress/theme/styles.css",
    "docs/public/brand/boost-emblem.svg",
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
Assert-Contains "README.ja.md" "README\.md" "English language switch link"
Assert-Contains "docs/index.md" "/getting-started" "English getting started action"
Assert-Contains "docs/ja/index.md" "/ja/getting-started" "Japanese getting started action"
Assert-Contains "docs/.vitepress/config.mts" "/TimberBoostControl/" "GitHub Pages base path"
Assert-Contains ".github/workflows/deploy-docs.yml" "actions/deploy-pages@v4" "GitHub Pages deploy action"
Assert-Contains ".github/workflows/deploy-docs.yml" "docs/.vitepress/dist" "Pages artifact path"
Assert-Contains ".github/workflows/ci.yml" "npm run validate" "combined repo QA verification"

Write-Host "Repository polish validation passed for $resolvedRepoRoot"
