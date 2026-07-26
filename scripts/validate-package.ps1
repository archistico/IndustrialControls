$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $PSScriptRoot
$projectPath = Join-Path $root 'src\IndustrialControls.Avalonia\IndustrialControls.Avalonia.csproj'
$packageDirectory = Join-Path $root 'artifacts\packages'

$versionNode = Select-Xml `
    -Path $projectPath `
    -XPath '/Project/PropertyGroup/Version' |
    Select-Object -First 1

$packageIdNode = Select-Xml `
    -Path $projectPath `
    -XPath '/Project/PropertyGroup/PackageId' |
    Select-Object -First 1

if ($null -eq $versionNode -or
    $null -eq $packageIdNode) {
    throw 'Package metadata not found in the library project.'
}

$expectedVersion = $versionNode.Node.InnerText
$packageId = $packageIdNode.Node.InnerText
$packageName = "$packageId.$expectedVersion.nupkg"
$packagePath = Join-Path $packageDirectory $packageName

if (-not (Test-Path -LiteralPath $packagePath)) {
    throw "Expected NuGet package not found: $packageName"
}

Add-Type -AssemblyName System.IO.Compression.FileSystem

$archive = [System.IO.Compression.ZipFile]::OpenRead($packagePath)

try {
    $entries = @(
        $archive.Entries |
        ForEach-Object {
            $_.FullName.Replace('\', '/')
        }
    )

    $requiredEntries = @(
        "$packageId.nuspec",
        'README.md',
        'CHANGELOG.md',
        'docs/PACKAGE_USAGE.md',
        'docs/PUBLIC_API.md',
        'docs/ACCESSIBILITY.md',
        'docs/DEMO_CATALOG.md',
        'docs/CONTROL_CATALOG.md',
        'docs/PERFORMANCE.md',
        'docs/RELEASE_CHECKLIST.md',
        'screenshot/01-foundation.png',
        'screenshot/02-lamps-and-led.png',
        'screenshot/03-gauges.png',
        'screenshot/04-operator-controls.png',
        'screenshot/05-trends-and-screens.png',
        'screenshot/06-alarm-indicators.png',
        'screenshot/07-static-and-release.png',
        'lib/net10.0/IndustrialControls.Avalonia.dll',
        'lib/net10.0/IndustrialControls.Avalonia.xml'
    )

    foreach ($entry in $requiredEntries) {
        if ($entries -notcontains $entry) {
            throw "Package entry missing: $entry"
        }
    }

    $nuspecEntry = $archive.GetEntry(
        "$packageId.nuspec"
    )

    if ($null -eq $nuspecEntry) {
        throw 'NuSpec entry not found.'
    }

    $stream = $nuspecEntry.Open()
    $reader = New-Object System.IO.StreamReader($stream)

    try {
        [xml] $nuspec = $reader.ReadToEnd()
    }
    finally {
        $reader.Dispose()
        $stream.Dispose()
    }

    $actualVersion =
        [string] $nuspec.package.metadata.version

    if ($actualVersion -ne $expectedVersion) {
        throw "NuSpec version mismatch: expected $expectedVersion, actual $actualVersion"
    }

    Write-Host "PACKAGE CONTENT PASSED: $packageName"
}
finally {
    $archive.Dispose()
}
