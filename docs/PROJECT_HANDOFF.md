# Project handoff

## Baseline ufficiale

M5 Hotfix 3 è la baseline validata.

## Candidate corrente

M6 Hotfix 2 — borderless trend points and roadmap extension.

## Controlli e modelli aggiunti

- `SignalQuality`
- `SignalSample`
- `SignalTraceSeries`
- `TimeSeriesControlBase`
- `TrendChart`
- `OscilloscopeDisplay`
- `StripChartRecorder`
- `SignalQualityIndicator`
- `IndustrialScreen`

## Principi M6

- i controlli non generano autonomamente dati di processo;
- l'applicazione fornisce campioni espliciti;
- ogni serie ha capacità limitata;
- il trimming è deterministico;
- i campioni conservano la qualità della misura;
- `Bad` e `Unavailable` interrompono la traccia;
- `Uncertain` usa una rappresentazione di cautela;
- il registratore rifiuta campioni quando è in pausa;
- la demo usa un timer soltanto per mostrare l'integrazione.

## Gate

```powershell
.\scripts\validate.ps1
```

M6 diventa validata solo dopo conferma dell'utente che build, test e controllo manuale della demo sono riusciti.

## Prossima milestone

M7 — Alarm Indicators & Static Panel Elements.

La stabilizzazione e il rilascio finale diventano M8.

## Consegna

Ogni consegna deve essere uno ZIP completo dell'intero progetto pronto per compilazione e test.


## Hotfix 1

Correzioni richieste dal controllo manuale:

- l'indicatore di qualità usa ora una lampada tonda dedicata nel template;
- il trend usa punti e marcatori circolari al posto delle X;
- i campioni visualizzati vengono decimati se troppo densi per la larghezza disponibile;
- riducendo `MaxSamplesPerSeries`, i buffer esistenti vengono immediatamente ritagliati;
- il contenimento della memoria resta deterministico e per-serie.


## Hotfix 2

Correzione visuale:

- i punti del `TrendChart` non hanno più bordo;
- il colore del campione riempie interamente il cerchio;
- memoria, capacità e decimazione restano invariati.

Roadmap estesa:

- M7 dedicata a indicatori di allarme retroilluminati e componenti statici da pannello;
- M8 dedicata alla stabilizzazione e al rilascio 1.0;
- versione libreria `0.6.2`.
