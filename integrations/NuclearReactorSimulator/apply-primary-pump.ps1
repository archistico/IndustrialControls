param(
    [Parameter(Mandatory = $true)]
    [string] $SimulatorRoot
)

$ErrorActionPreference = 'Stop'
$patchPath = Join-Path $PSScriptRoot 'primary-pump.patch'
$targetPath = (Resolve-Path -LiteralPath $SimulatorRoot).Path
$appProject = Join-Path $targetPath 'src\NuclearReactorSimulator.App\NuclearReactorSimulator.App.csproj'

if (-not (Test-Path -LiteralPath $appProject -PathType Leaf)) {
    throw "NuclearReactorSimulator.App project not found in '$targetPath'."
}

$previousErrorPreference = $ErrorActionPreference
try {
    $ErrorActionPreference = 'Continue'
    & git -C $targetPath apply --reverse --check -- $patchPath 2>$null
    $alreadyApplied = $LASTEXITCODE -eq 0
}
finally {
    $ErrorActionPreference = $previousErrorPreference
}
if ($alreadyApplied) {
    Write-Host 'Primary pump panel is already applied.'
    exit 0
}

& git -C $targetPath apply --check -- $patchPath
if ($LASTEXITCODE -ne 0) {
    throw 'Patch does not apply cleanly. No integration files were changed.'
}

& git -C $targetPath apply -- $patchPath
if ($LASTEXITCODE -ne 0) {
    throw 'Applying the primary pump panel failed.'
}

Write-Host 'Primary pump panel applied. Build and test NuclearReactorSimulator.App before use.'
