param(
    [string]$GameRoot = ""
)

$ErrorActionPreference = "Stop"

function Resolve-GameRoot {
    param(
        [string]$ProvidedPath
    )

    if (-not [string]::IsNullOrWhiteSpace($ProvidedPath)) {
        return $ProvidedPath
    }

    $candidates = @(
        "D:\SteamLibrary\steamapps\common\Timberborn",
        "C:\SteamLibrary\steamapps\common\Timberborn",
        "C:\Program Files (x86)\Steam\steamapps\common\Timberborn"
    )

    foreach ($candidate in $candidates) {
        if (Test-Path (Join-Path $candidate "Timberborn_Data\Managed")) {
            return $candidate
        }
    }

    throw "Could not locate Timberborn automatically. Pass -GameRoot with your Timberborn install path."
}

$modRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$sourceDir = Join-Path $modRoot "Source"
$outputDll = Join-Path $modRoot "Code.dll"
$GameRoot = Resolve-GameRoot $GameRoot
$managedDir = Join-Path $GameRoot "Timberborn_Data\Managed"
$csc = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
$frameworkDir = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319"
$netStandard = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\netstandard.dll"

if (-not (Test-Path $csc)) {
    throw "csc.exe was not found at $csc"
}

if (-not (Test-Path $managedDir)) {
    throw "Managed DLL directory was not found: $managedDir"
}

$references = @(
    (Join-Path $managedDir "Bindito.Core.dll"),
    (Join-Path $managedDir "Newtonsoft.Json.dll"),
    (Join-Path $managedDir "System.Memory.dll"),
    (Join-Path $managedDir "Timberborn.ModManagerScene.dll"),
    (Join-Path $managedDir "Timberborn.QuickNotificationSystem.dll"),
    (Join-Path $managedDir "Timberborn.SingletonSystem.dll"),
    (Join-Path $managedDir "Timberborn.UILayoutSystem.dll"),
    (Join-Path $managedDir "UnityEngine.CoreModule.dll"),
    (Join-Path $managedDir "UnityEngine.TextRenderingModule.dll"),
    (Join-Path $managedDir "UnityEngine.UIElementsModule.dll"),
    (Join-Path $frameworkDir "System.IO.Compression.dll"),
    (Join-Path $frameworkDir "System.IO.Compression.FileSystem.dll"),
    $netStandard
)

$sourceFiles = Get-ChildItem $sourceDir -Filter *.cs | Sort-Object Name | Select-Object -ExpandProperty FullName
if (-not $sourceFiles) {
    throw "No C# source files found in $sourceDir"
}

$responsePath = Join-Path $modRoot "build.rsp"
$responseLines = @(
    "/target:library",
    "/nologo",
    "/out:`"$outputDll`""
)

foreach ($reference in $references) {
    if (-not (Test-Path $reference)) {
        throw "Missing reference: $reference"
    }
    $responseLines += "/reference:`"$reference`""
}

foreach ($sourceFile in $sourceFiles) {
    $responseLines += "`"$sourceFile`""
}

Set-Content -Path $responsePath -Encoding ASCII -Value $responseLines
& $csc "@$responsePath"
$exitCode = $LASTEXITCODE
Remove-Item $responsePath -Force

if ($exitCode -ne 0) {
    throw "Compilation failed with exit code $exitCode"
}

Write-Host "Built $outputDll"
