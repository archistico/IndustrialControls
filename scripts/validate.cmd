@echo off
setlocal
set ROOT=%~dp0..
pushd "%ROOT%"

for /d /r %%D in (bin obj TestResults artifacts) do (
    if exist "%%D" rmdir /s /q "%%D"
)

dotnet restore IndustrialControls.Avalonia.sln --force-evaluate
if errorlevel 1 goto :fail

dotnet build IndustrialControls.Avalonia.sln -c Release --no-restore
if errorlevel 1 goto :fail

dotnet test --project tests\IndustrialControls.Avalonia.Tests\IndustrialControls.Avalonia.Tests.csproj -c Release --no-build
if errorlevel 1 goto :fail

dotnet pack src\IndustrialControls.Avalonia\IndustrialControls.Avalonia.csproj -c Release --no-build -o artifacts\packages
if errorlevel 1 goto :fail

powershell -NoProfile -ExecutionPolicy Bypass -File scripts\validate-package.ps1
if errorlevel 1 goto :fail

powershell -NoProfile -ExecutionPolicy Bypass -File scripts\validate-package-consumer.ps1
if errorlevel 1 goto :fail

echo.
echo M8 RC6-D VALIDATION PASSED
popd
exit /b 0

:fail
set CODE=%ERRORLEVEL%
popd
exit /b %CODE%
