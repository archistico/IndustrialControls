# Release checklist

## Automated gate

```powershell
.\scripts\validate.ps1
```

The gate performs:

- clean restore;
- release build;
- complete test suite;
- NuGet package creation;
- package-content inspection.

## Optional benchmark

```powershell
.\scripts\benchmark.ps1
```

## Manual gate

- start the demo;
- verify every M7 visual component;
- navigate interactive controls using only the keyboard;
- verify focus visibility;
- test alarm activation, ACK, return and RESET;
- inspect the generated package in `artifacts\packages`;
- integrate the package into a separate Avalonia application;
- verify the theme is loaded through `IndustrialControlsTheme`.

## Stable release

After the release candidate passes all gates:

- change version from `1.0.0-rc.6` to `1.0.0`;
- update the changelog;
- rebuild, test and pack;
- archive the validated complete ZIP.
