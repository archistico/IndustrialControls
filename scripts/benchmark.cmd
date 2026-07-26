@echo off
setlocal
set ROOT=%~dp0..
pushd "%ROOT%"

dotnet run --project benchmarks\IndustrialControls.Avalonia.Benchmarks\IndustrialControls.Avalonia.Benchmarks.csproj -c Release
set CODE=%ERRORLEVEL%

popd
exit /b %CODE%
