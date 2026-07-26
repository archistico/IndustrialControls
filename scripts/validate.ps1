$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
Push-Location $root

try {
    Get-ChildItem -Path . -Directory -Recurse -Force |
        Where-Object { $_.Name -in @('bin', 'obj', 'TestResults') } |
        Sort-Object FullName -Descending |
        Remove-Item -Recurse -Force

    dotnet restore IndustrialControls.Avalonia.sln --force-evaluate
    dotnet build IndustrialControls.Avalonia.sln -c Release --no-restore
    dotnet test IndustrialControls.Avalonia.sln -c Release --no-build
    Write-Host ''
    Write-Host 'VALIDATION PASSED'
}
finally {
    Pop-Location
}
