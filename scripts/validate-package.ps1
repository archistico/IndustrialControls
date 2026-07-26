$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $PSScriptRoot
$packageDirectory = Join-Path $root 'artifacts\packages'

$package = Get-ChildItem -Path $packageDirectory -Filter '*.nupkg' |
    Where-Object { $_.Name -notlike '*.symbols.nupkg' } |
    Sort-Object LastWriteTimeUtc -Descending |
    Select-Object -First 1

if ($null -eq $package) {
    throw 'NuGet package not found.'
}

Add-Type -AssemblyName System.IO.Compression.FileSystem

$archive = [System.IO.Compression.ZipFile]::OpenRead($package.FullName)

try {
    $entries = @($archive.Entries | ForEach-Object { $_.FullName.Replace('\', '/') })

    $requiredEntries = @(
        'README.md',
        'CHANGELOG.md',
        'docs/PACKAGE_USAGE.md',
        'docs/PUBLIC_API.md',
        'docs/ACCESSIBILITY.md',
        'lib/net10.0/IndustrialControls.Avalonia.dll',
        'lib/net10.0/IndustrialControls.Avalonia.xml'
    )

    foreach ($entry in $requiredEntries) {
        if ($entries -notcontains $entry) {
            throw "Package entry missing: $entry"
        }
    }

    Write-Host "PACKAGE CONTENT PASSED: $($package.Name)"
}
finally {
    $archive.Dispose()
}
