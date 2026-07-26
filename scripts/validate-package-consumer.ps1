$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $PSScriptRoot
$projectPath = Join-Path $root 'src\IndustrialControls.Avalonia\IndustrialControls.Avalonia.csproj'
$packageDirectory = Join-Path $root 'artifacts\packages'
$smokeRoot = Join-Path $root 'artifacts\package-consumer-smoke'
$smokeProject = Join-Path $smokeRoot 'PackageConsumerSmoke.csproj'
$nugetConfig = Join-Path $smokeRoot 'NuGet.Config'
$programPath = Join-Path $smokeRoot 'Program.cs'
$packagesCache = Join-Path $smokeRoot '.packages'

function Invoke-DotNet {
    param(
        [Parameter(Mandatory = $true)]
        [string[]] $Arguments
    )

    & dotnet @Arguments

    if ($LASTEXITCODE -ne 0) {
        throw "dotnet $($Arguments -join ' ') failed with exit code $LASTEXITCODE."
    }
}

$versionNode = Select-Xml `
    -Path $projectPath `
    -XPath '/Project/PropertyGroup/Version' |
    Select-Object -First 1

if ($null -eq $versionNode) {
    throw 'Package version not found.'
}

$expectedVersion = $versionNode.Node.InnerText
$escapedPackageDirectory =
    [System.Security.SecurityElement]::Escape(
        $packageDirectory)

if (Test-Path -LiteralPath $smokeRoot) {
    Remove-Item `
        -LiteralPath $smokeRoot `
        -Recurse `
        -Force
}

New-Item `
    -ItemType Directory `
    -Path $smokeRoot `
    -Force |
    Out-Null

$projectSource = @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="IndustrialControls.Avalonia"
                      Version="$expectedVersion" />
  </ItemGroup>
</Project>
"@

$configSource = @"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="IndustrialControls.Local"
         value="$escapedPackageDirectory" />
    <add key="NuGet.org"
         value="https://api.nuget.org/v3/index.json"
         protocolVersion="3" />
  </packageSources>
</configuration>
"@

$programSource = @"
using IndustrialControls.Avalonia;
using IndustrialControls.Avalonia.Controls;

var gauge = new DigitalGauge
{
    Title = "PACKAGE SMOKE",
    Minimum = 0,
    Maximum = 10,
    Value = 5,
    Unit = "MWe"
};

var switchControl = new IndustrialToggleSwitch
{
    Title = "BREAKER",
    IsInterlocked = true
};

if (!string.Equals(
        IndustrialControlsRelease.Version,
        "$expectedVersion",
        StringComparison.Ordinal))
{
    return 2;
}

if (!string.Equals(
        gauge.FormattedValue,
        "5.0 MWe",
        StringComparison.Ordinal))
{
    return 3;
}

if (switchControl.TryToggle())
{
    return 4;
}

Console.WriteLine(
    "PACKAGE CONSUMER PASSED: " +
    IndustrialControlsRelease.Version);

return 0;
"@

Set-Content `
    -LiteralPath $smokeProject `
    -Value $projectSource `
    -Encoding UTF8

Set-Content `
    -LiteralPath $nugetConfig `
    -Value $configSource `
    -Encoding UTF8

Set-Content `
    -LiteralPath $programPath `
    -Value $programSource `
    -Encoding UTF8

Invoke-DotNet -Arguments @(
    'restore',
    $smokeProject,
    '--configfile',
    $nugetConfig,
    '--packages',
    $packagesCache
)

Invoke-DotNet -Arguments @(
    'build',
    $smokeProject,
    '-c',
    'Release',
    '--no-restore'
)

Invoke-DotNet -Arguments @(
    'run',
    '--project',
    $smokeProject,
    '-c',
    'Release',
    '--no-build'
)
