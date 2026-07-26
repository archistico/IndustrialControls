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
