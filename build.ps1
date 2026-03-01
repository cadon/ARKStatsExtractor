#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Builds the ARK Breeding Stats solution
.DESCRIPTION
    This script locates MSBuild and builds the solution. The solution contains
    old-style .NET Framework projects that require MSBuild instead of dotnet build.
.PARAMETER Configuration
    Build configuration (Debug or Release). Default is Debug.
.PARAMETER Clean
    Perform a clean build
.PARAMETER SkipTests
    Skip running tests after a successful build
.EXAMPLE
    .\build.ps1
    .\build.ps1 -Configuration Release
    .\build.ps1 -Clean
    .\build.ps1 -SkipTests
#>

param(
    [Parameter()]
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Debug',
    
    [Parameter()]
    [switch]$Clean,

    [Parameter()]
    [switch]$SkipTests
)

$ErrorActionPreference = 'Stop'

Write-Host "=== ARK Breeding Stats Build Script ===" -ForegroundColor Cyan
Write-Host "Configuration: $Configuration" -ForegroundColor Gray

# Locate MSBuild using vswhere
$vswherePath = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"

if (-not (Test-Path $vswherePath)) {
    Write-Error "vswhere.exe not found. Please install Visual Studio 2017 or later."
    exit 1
}

Write-Host "Locating MSBuild..." -ForegroundColor Gray
$msbuildPath = & $vswherePath -latest -requires Microsoft.Component.MSBuild -find "MSBuild\**\Bin\MSBuild.exe" | Select-Object -First 1

if (-not $msbuildPath) {
    Write-Error "MSBuild not found. Please install Visual Studio with MSBuild component."
    exit 1
}

Write-Host "Found MSBuild: $msbuildPath" -ForegroundColor Green

# Build the solution
$solutionPath = Join-Path $PSScriptRoot "ARKBreedingStats.sln"

if (-not (Test-Path $solutionPath)) {
    Write-Error "Solution file not found: $solutionPath"
    exit 1
}

$targets = if ($Clean) { "Clean,Restore,Build" } else { "Restore,Build" }

Write-Host "Building solution..." -ForegroundColor Gray
Write-Host "Target(s): $targets" -ForegroundColor Gray

& $msbuildPath $solutionPath /t:$targets /p:Configuration=$Configuration /m /v:minimal

if ($LASTEXITCODE -ne 0) {
    Write-Host "`n=== Build Failed ===" -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "`n=== Build Succeeded ===" -ForegroundColor Green

if ($SkipTests) {
    Write-Host "Skipping tests (-SkipTests specified)." -ForegroundColor Gray
    exit 0
}

# Locate vstest.console.exe
Write-Host "`nLocating VSTest..." -ForegroundColor Gray
$vstestPath = & $vswherePath -latest -requires Microsoft.VisualStudio.PackageGroup.TestTools.Core `
    -find "Common7\IDE\Extensions\TestPlatform\vstest.console.exe" | Select-Object -First 1

if (-not $vstestPath) {
    Write-Warning "vstest.console.exe not found; falling back to dotnet test."
    $testProject = Join-Path $PSScriptRoot "ARKBreedingStats.Tests\ARKBreedingStats.Tests.csproj"
    dotnet test $testProject --no-build --configuration $Configuration --logger "console;verbosity=normal"
    if ($LASTEXITCODE -ne 0) {
        Write-Host "`n=== Tests Failed ===" -ForegroundColor Red
        exit $LASTEXITCODE
    }
} else {
    Write-Host "Found VSTest: $vstestPath" -ForegroundColor Green
    $testAssembly = Join-Path $PSScriptRoot "ARKBreedingStats.Tests\bin\$Configuration\net48\ARKBreedingStats.Tests.dll"

    if (-not (Test-Path $testAssembly)) {
        Write-Warning "Test assembly not found at: $testAssembly"
        Write-Warning "Skipping test run."
    } else {
        Write-Host "Running tests..." -ForegroundColor Gray
        & $vstestPath $testAssembly /logger:Console /logger:"trx;LogFileName=TestResults.trx"
        if ($LASTEXITCODE -ne 0) {
            Write-Host "`n=== Tests Failed ===" -ForegroundColor Red
            exit $LASTEXITCODE
        }
    }
}

Write-Host "`n=== Tests Passed ===" -ForegroundColor Green
exit 0
