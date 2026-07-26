$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $PSScriptRoot
Push-Location $root

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

try {
    Get-ChildItem -Path . -Directory -Recurse -Force |
        Where-Object {
            $_.Name -in @(
                'bin',
                'obj',
                'TestResults',
                'artifacts'
            )
        } |
        Sort-Object FullName -Descending |
        Remove-Item -Recurse -Force

    Invoke-DotNet -Arguments @(
        'restore',
        'IndustrialControls.Avalonia.sln',
        '--force-evaluate'
    )

    Invoke-DotNet -Arguments @(
        'build',
        'IndustrialControls.Avalonia.sln',
        '-c',
        'Release',
        '--no-restore'
    )

    Invoke-DotNet -Arguments @(
        'test',
        '--project',
        'tests\IndustrialControls.Avalonia.Tests\IndustrialControls.Avalonia.Tests.csproj',
        '-c',
        'Release',
        '--no-build'
    )

    Invoke-DotNet -Arguments @(
        'pack',
        'src\IndustrialControls.Avalonia\IndustrialControls.Avalonia.csproj',
        '-c',
        'Release',
        '--no-build',
        '-o',
        'artifacts\packages'
    )

    & "$PSScriptRoot\validate-package.ps1"
    & "$PSScriptRoot\validate-package-consumer.ps1"

    Write-Host ''
    Write-Host '1.0.0 VALIDATION PASSED'
}
finally {
    Pop-Location
}
