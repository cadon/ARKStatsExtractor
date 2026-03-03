#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Builds the ARK Breeding Stats solution.
.PARAMETER Configuration
    Build configuration (Debug or Release). Default is Debug.
.PARAMETER Clean
    Perform a clean build.
.PARAMETER SkipTests
    Skip running tests after a successful build.
.EXAMPLE
    .\build.ps1
    .\build.ps1 -Configuration Release
    .\build.ps1 -Clean
    .\build.ps1 -SkipTests
#>

param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Debug',
    [switch]$Clean,
    [switch]$SkipTests
)

$ErrorActionPreference = 'Stop'

$RepoRoot = $PSScriptRoot
$WorkPath = Join-Path $RepoRoot '.work'
New-Item -ItemType Directory -Path $WorkPath -ErrorAction SilentlyContinue | Out-Null

# Pinned Inno Setup version — update here to upgrade
$InnoSetupVersion = '6.7.1'
$InnoSetupDir     = Join-Path $WorkPath 'innosetup'
$InnoSetupExe     = Join-Path $InnoSetupDir 'ISCC.exe'

# ── Tool discovery ────────────────────────────────────────────────────────────

function Get-InnoSetup {
    # 1. Already downloaded locally
    if (Test-Path $InnoSetupExe) { return $InnoSetupExe }

    # 2. System install
    $system = "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe"
    if (Test-Path $system) { return $system }

    # 3. Download from GitHub releases and install to local tools folder
    $tag       = "is-$($InnoSetupVersion.Replace('.', '_'))"
    $url       = "https://github.com/jrsoftware/issrc/releases/download/$tag/innosetup-$InnoSetupVersion.exe"
    $installer = Join-Path $WorkPath "innosetup-$InnoSetupVersion.exe"

    Write-Host "  Downloading Inno Setup $InnoSetupVersion..." -ForegroundColor Gray
    Invoke-WebRequest -Uri $url -OutFile $installer -UseBasicParsing

    Write-Host "  Installing Inno Setup to $InnoSetupDir ..." -ForegroundColor Gray
    & $installer /VERYSILENT /SUPPRESSMSGBOXES /NORESTART "/DIR=$InnoSetupDir"
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Inno Setup installation failed (exit $LASTEXITCODE)."
        exit 1
    }

    return $InnoSetupExe
}

# ── Build steps ───────────────────────────────────────────────────────────────

function Invoke-GenerateManifest {
    Write-Host "`nGenerating _manifest.json..." -ForegroundColor Gray

    $projectDir   = Join-Path $PSScriptRoot 'ARKBreedingStats'
    $project      = Join-Path $projectDir 'ARKBreedingStats.csproj'
    $asbVersion   = (dotnet msbuild $project -getProperty:FileVersion).Trim()

    $namePatterns = Get-Content (Join-Path $projectDir 'json\namePatternTemplates.json') -Raw
    $npVersion    = [regex]::Match($namePatterns, '"version"\s*:\s*"([\d.]+)"').Groups[1].Value

    $imagePacks   = Get-Content (Join-Path $projectDir 'json\imagePacks.json') -Raw
    $ipVersion    = [regex]::Match($imagePacks, '"version"\s*:\s*"([\d.]+)"').Groups[1].Value

    $manifest = @"
{
  "format": "1.0",
  "modules":{
    "ARK Smart Breeding": {
      "version": "$asbVersion"
    },
    "NamePatternTemplates": {
      "Category": "Name Pattern Templates",
      "Name": "Name Pattern Templates",
      "Description": "Templates for naming patterns",
      "Url": "https://raw.githubusercontent.com/cadon/ARKStatsExtractor/refs/heads/master/ARKBreedingStats/json/namePatternTemplates.json",
      "LocalPath": "json/namePatternTemplates.json",
      "optional": true,
      "version": "$npVersion"
    },
    "SpeciesImagePacks": {
      "Category": "Images",
      "Name": "Species image packs",
      "Url": "https://raw.githubusercontent.com/cadon/ARKStatsExtractor/refs/heads/master/ARKBreedingStats/json/imagePacks.json",
      "LocalPath": "json/imagePacks.json",
      "version": "$ipVersion"
    }
  }
}
"@

    [System.IO.File]::WriteAllText(
        (Join-Path $projectDir '_manifest.json'),
        $manifest.TrimStart(),
        [System.Text.Encoding]::UTF8)

    Write-Host "  version=$asbVersion  namePatterns=$npVersion  imagePacks=$ipVersion" -ForegroundColor Gray
}

function Invoke-Build {
    $solution = Join-Path $PSScriptRoot "ARKBreedingStats.sln"
    if (-not (Test-Path $solution)) {
        Write-Error "Solution file not found: $solution"
        exit 1
    }

    $cleanArgs = if ($Clean) { @('--no-incremental') } else { @() }
    Write-Host "`nBuilding solution..." -ForegroundColor Gray

    dotnet build $solution --configuration $Configuration @cleanArgs
    if ($LASTEXITCODE -ne 0) {
        Write-Host "`n=== Build Failed ===" -ForegroundColor Red
        exit $LASTEXITCODE
    }
    Write-Host "`n=== Build Succeeded ===" -ForegroundColor Green
}

function Invoke-PackageRelease {
    $project    = Join-Path $PSScriptRoot "ARKBreedingStats\ARKBreedingStats.csproj"
    $publishDir = Join-Path $WorkPath "bin"
    $outputDir  = Join-Path $WorkPath "publish"

    Write-Host "`nPublishing..." -ForegroundColor Gray
    dotnet publish $project --configuration Release --output "$publishDir" --no-build
    if ($LASTEXITCODE -ne 0) {
        Write-Host "`n=== Publish Failed ===" -ForegroundColor Red
        exit $LASTEXITCODE
    }

    Write-Host "`nPackaging release..." -ForegroundColor Cyan

    Get-ChildItem $publishDir -Filter *.pdb | Remove-Item -Force
    Get-ChildItem $publishDir -Filter *.xml | Remove-Item -Force

    $version = (Get-Item (Join-Path $publishDir "ARK Smart Breeding.exe")).VersionInfo.FileVersion
    if (-not (Test-Path $outputDir)) { New-Item -ItemType Directory -Path $outputDir | Out-Null }
    $zipPath = Join-Path $outputDir "ARK.Smart.Breeding_$version.zip"
    Compress-Archive -Force -Path "$publishDir\*" -DestinationPath $zipPath
    Write-Host "  Created: $zipPath" -ForegroundColor Green

    $iscc = Get-InnoSetup
    Write-Host "  Running Inno Setup..." -ForegroundColor Gray
    & $iscc (Join-Path $PSScriptRoot "setup.iss")
    if ($LASTEXITCODE -ne 0) {
        Write-Host "`n=== Installer Build Failed ===" -ForegroundColor Red
        exit $LASTEXITCODE
    }
}

function Invoke-Tests {
    $testProject = Join-Path $PSScriptRoot "ARKBreedingStats.Tests\ARKBreedingStats.Tests.csproj"
    Write-Host "`nRunning tests..." -ForegroundColor Gray

    dotnet test $testProject --no-build --configuration $Configuration --logger "console;verbosity=normal"
    if ($LASTEXITCODE -ne 0) {
        Write-Host "`n=== Tests Failed ===" -ForegroundColor Red
        exit $LASTEXITCODE
    }
    Write-Host "`n=== Tests Passed ===" -ForegroundColor Green
}

# ── Main ──────────────────────────────────────────────────────────────────────

Write-Host "=== ARK Breeding Stats Build Script ===" -ForegroundColor Cyan
Write-Host "Configuration: $Configuration" -ForegroundColor Gray

Invoke-GenerateManifest
Invoke-Build

if ($Configuration -eq 'Release')  { Invoke-PackageRelease }
if (-not $SkipTests)               { Invoke-Tests }
else { Write-Host "Skipping tests (-SkipTests specified)." -ForegroundColor Gray }

exit 0
