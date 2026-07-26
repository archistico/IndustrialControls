# Architecture

## Obiettivo

`IndustrialControls.Avalonia` è una libreria UI autonoma. Non contiene logica applicativa, servizi, ViewModel specifici o dipendenze dalla demo.

## Regole

- I controlli riutilizzabili espongono proprietà Avalonia.
- Il comportamento e il rendering restano separati dall'applicazione ospitante.
- Il tema è caricato tramite `IndustrialControlsTheme`.
- La demo dipende dalla libreria; la libreria non dipende dalla demo.
- I controlli non dipendono da framework MVVM.
- Le API pubbliche devono restare coerenti fra le famiglie di controlli.
- I rendering ad alta densità useranno `DrawingContext`, non migliaia di elementi visuali.

## Progetti

### IndustrialControls.Avalonia

Assembly distribuibile, risorse XAML, controlli e temi.

### IndustrialControls.Avalonia.Demo

Catalogo interattivo e progetto consumer reale.

### IndustrialControls.Avalonia.Tests

Contratti delle proprietà, logica e regressioni.

## Distribuzione

La libreria è predisposta per:

- `ProjectReference`;
- pacchetto NuGet locale;
- pacchetto NuGet pubblico o privato.


## M5 interaction model

I controlli operatore espongono proprietà Avalonia bindabili e metodi deterministici per i comandi:

- `TrySetValue`, `Increase`, `Decrease`;
- `Select`, `SelectNext`, `SelectPrevious`;
- `TryToggle`;
- `PressLeft`, `PressRight`, `Release`.

I metodi restituiscono `false` quando un interlock impedisce il comando. Questa separazione consente di testare la semantica senza dipendere dagli eventi grafici.


## M6 time-series architecture

`TimeSeriesControlBase` possiede soltanto buffer e contratti di visualizzazione. Non avvia thread, timer o acquisizioni autonome.

L'applicazione chiama esplicitamente:

- `AddSeries`;
- `AddSample`;
- `SetSeriesVisibility`;
- `ClearSamples`.

`TrendChart` e `StripChartRecorder` condividono il modello delle serie, ma mantengono renderer distinti. `OscilloscopeDisplay` usa un buffer a singola traccia ottimizzato per finestre dense.

La demo genera segnali sintetici tramite `DispatcherTimer`; questa dipendenza resta confinata nel progetto Demo.


## M7 alarm-state architecture

`BacklitAlarmIndicator` separa la condizione di processo dalla memoria dell'allarme:

- `IsConditionActive` descrive il processo;
- `IsAcknowledged` descrive l'azione operatore;
- `HasLatchedAlarm` descrive la memoria;
- `VisualState` è uno stato derivato per il renderer.

`AlarmIndicatorPanel` è un `Panel` di layout, non un archivio applicativo degli allarmi. Può ospitare indicatori dichiarati direttamente in XAML e offre operazioni collettive deterministiche.

`SafetyPlacard` e `BoltedDataPlate` sono controlli statici: non avviano timer e non contengono logica di processo.


## M7 dispatcher-independent collection

`AlarmIndicatorPanel` separa:

- `Indicators`, raccolta logica e testabile;
- `ItemsControl` interno, usato soltanto dal template per il rendering.

Questa separazione evita che test e logica applicativa dipendano da `ItemCollection`, che appartiene al dispatcher Avalonia.
