# IndustrialControls.Avalonia

Libreria Avalonia riutilizzabile per interfacce industriali e sale controllo, con estetica anni Novanta.

## Baseline M6

Include le milestone M0–M5 validate e aggiunge:

- `TimeSeriesControlBase`;
- `SignalTraceSeries`;
- `SignalSample`;
- `SignalQuality`;
- `TrendChart`;
- `OscilloscopeDisplay`;
- `StripChartRecorder`;
- `SignalQualityIndicator`;
- `IndustrialScreen`;
- trend multicanale;
- cursore temporale e lettura dei valori;
- auto-scaling opzionale;
- qualità dei campioni;
- oscilloscopio con trigger;
- registratore a carta continua;
- demo dinamica e deterministica;
- test xUnit v3 con Microsoft Testing Platform.

## Validazione

```powershell
.\scripts\validate.ps1
```

## Demo

```powershell
dotnet run --project src/IndustrialControls.Avalonia.Demo
```

## Stato

M0–M5 Hotfix 3: **VALIDATED**  
M6 Hotfix 2: **CANDIDATE**


## M6 Hotfix 1

- `SignalQualityIndicator`: lampada grafica ridisegnata come luce circolare dedicata, senza deformazioni;
- `TrendChart`: resa dei campioni aggiornata con punti al posto delle marcature a X;
- `TrendChart`: decimazione grafica automatica quando i campioni visibili sono più densi dei pixel disponibili;
- `TimeSeriesControlBase`: se `MaxSamplesPerSeries` viene abbassato, le serie esistenti vengono subito ritagliate alla nuova capacità;
- memoria dei campioni sempre limitata da capacità per serie.


## M6 Hotfix 2

- i punti del `TrendChart` sono ora cerchi pieni senza bordo;
- anche i campioni `Bad` e `Unavailable` usano un marcatore circolare pieno;
- nessuna modifica al limite di memoria o alla decimazione;
- versione libreria `0.6.2`.

## Prossima milestone

### M7 — Alarm Indicators & Static Panel Elements

Controlli previsti:

- `BacklitAlarmIndicator`;
- `AlarmIndicatorPanel`;
- pannelli statici con viti agli angoli;
- icone di sicurezza e avvertimento;
- targhette dati imbullonate;
- varianti di materiale, colore e livello di attenzione.

La stabilizzazione e il rilascio 1.0 vengono spostati a M8.
