# Project handoff

## Baseline ufficiale

M7 Hotfix 3 / versione `0.7.3` è la baseline validata.

## Candidate corrente

M8 RC2 — Allocation & Update-Path Optimization / versione `1.0.0-rc.2`.

## Contenuto

- contratto API pubblico;
- `IndustrialControlsRelease`;
- metadati Avalonia Automation;
- live region per annunciatori;
- keyboard contract completo;
- focus visuale condiviso;
- theme-coverage gate;
- bounded-buffer long-run gate;
- benchmark console;
- pack NuGet e validazione del contenuto;
- documentazione release.

## Gate

```powershell
.\scripts\validate.ps1
```

Risultati richiesti:

- build Release senza warning;
- suite completa superata;
- package `.nupkg` e `.snupkg` generati;
- contenuto del package validato;
- demo verificata manualmente;
- navigazione da tastiera verificata.

## Dopo la validazione

La release stabile richiede soltanto:

- versione `1.0.0`;
- changelog finale;
- nuova esecuzione completa del gate;
- ZIP completo archiviato come baseline.

## Consegna

Ogni consegna deve essere uno ZIP completo dell'intero progetto pronto per compilazione e test.


## M8 RC1 Hotfix 1

Correzione di compilazione esclusivamente nel progetto di test:

- il riferimento `Avalonia.Media.Colors.Green` era risolto come
  `IndustrialControls.Avalonia.Media` a causa del namespace del test;
- il test usa ora il qualificatore globale
  `global::Avalonia.Media.Colors.Green`;
- la libreria e il contratto release restano invariati.


## M8 RC2

Optimization candidate based on RC1 Hotfix 1:

- no public API removals or renames;
- time-series storage is now a circular buffer;
- series lookup is dictionary-backed;
- direct-series ingestion is available;
- trend cursor text is generated only when read;
- gauge and selector accessibility updates are coalesced;
- stable brushes, pens, formats and selector labels are cached;
- benchmark output includes bytes per operation.

Validation requires the complete release gate and a new benchmark run on the same machine used for RC1.
