@echo off
setlocal

pushd "%~dp0.."

for /d /r %%D in (bin obj TestResults) do @if exist "%%D" rd /s /q "%%D"

dotnet restore IndustrialControls.Avalonia.sln --force-evaluate
if errorlevel 1 goto :failure

dotnet build IndustrialControls.Avalonia.sln -c Release --no-restore
if errorlevel 1 goto :failure

dotnet test IndustrialControls.Avalonia.sln -c Release --no-build
if errorlevel 1 goto :failure

echo.
echo VALIDATION PASSED
popd
exit /b 0

:failure
echo.
echo VALIDATION FAILED
popd
exit /b 1
