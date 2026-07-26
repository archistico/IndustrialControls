$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
Push-Location $root

try {
    dotnet run --project benchmarks\IndustrialControls.Avalonia.Benchmarks\IndustrialControls.Avalonia.Benchmarks.csproj -c Release
}
finally {
    Pop-Location
}
