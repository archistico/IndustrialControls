$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
Push-Location $root

try {
    Get-ChildItem -Path . -Directory -Recurse -Force |
        Where-Object { $_.Name -in @('bin', 'obj', 'TestResults', 'artifacts') } |
        Sort-Object FullName -Descending |
        Remove-Item -Recurse -Force

    dotnet restore IndustrialControls.Avalonia.sln --force-evaluate
    dotnet build IndustrialControls.Avalonia.sln -c Release --no-restore
    dotnet test tests\IndustrialControls.Avalonia.Tests\IndustrialControls.Avalonia.Tests.csproj -c Release --no-build
    dotnet pack src\IndustrialControls.Avalonia\IndustrialControls.Avalonia.csproj -c Release --no-build -o artifacts\packages

    & "$PSScriptRoot\validate-package.ps1"

    Write-Host ''
    Write-Host 'M8 RC1 VALIDATION PASSED'
}
finally {
    Pop-Location
}
