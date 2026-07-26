# IndustrialControls.Avalonia

`IndustrialControls.Avalonia` è una libreria riutilizzabile per Avalonia che
fornisce controlli grafici ispirati ai pannelli di comando industriali degli
anni Novanta.

La libreria è pensata per:

- sinottici di impianto;
- simulatori tecnici;
- sale controllo;
- applicazioni SCADA o HMI;
- software di collaudo;
- pannelli diagnostici;
- applicazioni educative che richiedono strumenti industriali leggibili.

I controlli sono separati dalla demo e possono essere importati in qualunque
applicazione Avalonia compatibile con `.NET 10`.

## Requisiti

- .NET 10 SDK
- Avalonia 12
- progetto Avalonia desktop, browser o altro host compatibile
- C# con nullable reference types consigliati

## Installazione

### Da pacchetto NuGet locale

Compila e genera il pacchetto:

```powershell
.\scripts\validate.ps1
```

Il pacchetto viene creato in:

```text
artifacts\packages
```

Aggiungi quella cartella come sorgente NuGet locale oppure installa direttamente
il file generato.

Esempio con una sorgente locale configurata:

```powershell
dotnet add package IndustrialControls.Avalonia --version 1.0.0-rc.8
```

### Da riferimento al progetto

Durante lo sviluppo puoi aggiungere direttamente il riferimento al progetto:

```xml
<ItemGroup>
  <ProjectReference
      Include="..\IndustrialControls.Avalonia\src\IndustrialControls.Avalonia\IndustrialControls.Avalonia.csproj" />
</ItemGroup>
```

## Aggiungere il tema

Nel file `App.axaml` dell'applicazione che usa la libreria importa il namespace
del tema:

```xml
<Application
    x:Class="MyApplication.App"
    xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:industrialTheme="using:IndustrialControls.Avalonia.Themes">

  <Application.Styles>
    <FluentTheme />
    <industrialTheme:IndustrialControlsTheme />
  </Application.Styles>
</Application>
```

`IndustrialControlsTheme` carica il tema completo `Industrial90` e tutti i
`ControlTheme` richiesti dai controlli.

L'ordine consigliato è:

1. tema generale dell'applicazione;
2. `IndustrialControlsTheme`;
3. eventuali override definiti dall'applicazione.

## Aggiungere il namespace dei controlli

Nel file AXAML in cui vuoi usare i controlli:

```xml
<Window
    xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:industrial="using:IndustrialControls.Avalonia.Controls">
```

A quel punto puoi utilizzare i controlli con il prefisso `industrial:`.

## Primo esempio

```xml
<industrial:IndustrialPanel
    Header="GENERATORE"
    Subtitle="PANNELLO DI CONTROLLO">

  <StackPanel Spacing="16">
    <industrial:RadialGauge
        Title="POTENZA"
        Minimum="0"
        Maximum="10"
        Value="{Binding GeneratorPowerMWe}"
        Unit="MWe"
        DecimalPlaces="2"
        CautionHigh="8.5"
        WarningHigh="9.5" />

    <industrial:IlluminatedPushButton
        Content="START"
        SecondaryCaption="GENERATORE"
        LampColor="Green"
        IsLampOn="{Binding IsGeneratorRunning}"
        Command="{Binding StartGeneratorCommand}" />
  </StackPanel>
</industrial:IndustrialPanel>
```

## Struttura consigliata di una schermata

Per mantenere un aspetto coerente:

- usa `IndustrialPanel` per raggruppare strumenti appartenenti allo stesso
  sottosistema;
- usa `InstrumentBezel` quando uno strumento necessita di una cornice dedicata;
- usa `EngravedLabel`, `SafetyPlacard` e `BoltedDataPlate` per elementi statici;
- evita di mescolare troppi colori nello stesso pannello;
- assegna significato operativo ai colori, non soltanto valore decorativo;
- mantieni titoli, unità e intervalli sempre visibili.

## Controlli disponibili

### Pannelli ed elementi statici

#### IndustrialPanel

Contenitore principale per un gruppo funzionale.

Proprietà comuni:

```xml
<industrial:IndustrialPanel
    Header="CIRCUITO PRIMARIO"
    Subtitle="POMPE E COLLETTORI"
    Depth="Raised">
  <!-- contenuto -->
</industrial:IndustrialPanel>
```

`Depth` supporta le varianti `Flat`, `Raised` e `Recessed`.

#### InstrumentBezel

Cornice per uno strumento o un piccolo gruppo di indicatori.

```xml
<industrial:InstrumentBezel
    Title="PRESSIONE VAPORE"
    Unit="MPa"
    Shape="Rounded"
    ShowGlass="True">
  <industrial:DigitalGauge
      Minimum="0"
      Maximum="8"
      Value="6.85"
      Unit="MPa" />
</industrial:InstrumentBezel>
```

#### EngravedLabel

Targhetta incisa per identificazione statica.

```xml
<industrial:EngravedLabel
    Text="CONTROLLO LOCALE"
    Variant="Aluminum"
    ShowFasteners="True" />
```

#### SafetyPlacard

Cartello di sicurezza con livello e pittogramma.

```xml
<industrial:SafetyPlacard
    Level="Danger"
    Icon="ElectricalHazard"
    Title="PERICOLO"
    Text="ALTA TENSIONE. ISOLARE PRIMA DELL'ACCESSO." />
```

Livelli disponibili:

- `Information`
- `Notice`
- `Caution`
- `Warning`
- `Danger`

Icone disponibili:

- `Information`
- `Warning`
- `ElectricalHazard`
- `Radiation`
- `HotSurface`
- `Mandatory`

#### BoltedDataPlate

Targhetta dati con viti agli angoli e contenuto personalizzato.

```xml
<industrial:BoltedDataPlate
    Title="GENERATORE"
    Subtitle="DATI NOMINALI"
    Identifier="GEN-01"
    Material="Aluminum">

  <Grid ColumnDefinitions="*,Auto">
    <TextBlock Text="POTENZA NOMINALE" />
    <TextBlock Grid.Column="1"
               Text="10 MWe"
               FontWeight="Bold" />
  </Grid>
</industrial:BoltedDataPlate>
```

Materiali disponibili:

- `Aluminum`
- `Brass`
- `Black`
- `Red`

### Lampade e pulsanti

#### IndustrialLamp

Indicatore luminoso configurabile.

```xml
<industrial:IndustrialLamp
    Label="POMPA IN SERVIZIO"
    IsOn="{Binding IsPumpRunning}"
    LampColor="Green"
    Shape="Round" />
```

Forme disponibili:

- `Round`
- `Square`
- `Rectangular`
- `Capsule`

Gli stati possono essere spenti, accesi o lampeggianti.

#### IlluminatedPushButton

Pulsante con lampada integrata.

```xml
<industrial:IlluminatedPushButton
    Content="ARRESTO"
    SecondaryCaption="POMPA"
    LampColor="Red"
    IsLampOn="{Binding IsStopRequested}"
    ActionMode="Momentary"
    IsInterlocked="{Binding IsStopCommandInterlocked}"
    InterlockReason="COMANDO NON CONSENTITO"
    Command="{Binding StopPumpCommand}" />
```

`ActionMode` supporta il comportamento momentaneo o toggle.

Il pulsante espone inoltre:

- `IsInterlocked`;
- `InterlockReason`;
- `CanInvoke`;
- `StatusText`;
- `TryInvoke()`.

Quando è interbloccato non modifica lo stato toggle, non genera `Click` e non
esegue il comando associato. Il controllo è utilizzabile con tastiera e mostra
il cursore a mano.

### Display LED

#### LedMatrixDisplay

Display testuale statico.

```xml
<industrial:LedMatrixDisplay
    Text="SISTEMA PRONTO"
    LedColor="Green"
    MatrixSize="Font7x9" />
```

#### LedMarqueeDisplay

Display testuale scorrevole.

```xml
<industrial:LedMarqueeDisplay
    Text="ALLARME PRESSIONE VAPORE — VERIFICARE IL CIRCUITO"
    LedColor="Red"
    IsRunning="True"
    AutoFitVisibleCharacters="True"
    ScrollIntervalMilliseconds="120" />
```

Con `AutoFitVisibleCharacters="True"` il numero di caratteri visibili viene
calcolato dalla larghezza effettiva del controllo tramite un'unica legge,
utilizzata sia dai cambi proprietà sia dal layout. Il testo entra dal bordo
destro anche dopo il ridimensionamento della finestra.

La sorgente completa dello scorrimento viene ricostruita soltanto quando
cambiano testo, capacità o pausa finale; ogni tick riutilizza la sorgente e il
buffer della finestra.

Proprietà utili:

- `VisibleCharacters`: capacità manuale usata quando l'adattamento automatico è
  disattivato;
- `AutoFitVisibleCharacters`: abilita l'adattamento alla larghezza;
- `ScrollIntervalMilliseconds`: velocità di scorrimento;
- `EndPauseCharacters`: pausa fra una ripetizione e la successiva;
- `DisplayText`: porzione attualmente visualizzata.

#### SevenSegmentDisplay

Display numerico.

```xml
<industrial:SevenSegmentDisplay
    Value="{Binding TurbineSpeedRpm}"
    Digits="5"
    DecimalPlaces="0"
    ShowLeadingZeros="False"
    Unit="rpm" />
```

### Annunciatori di allarme

#### AlarmAnnunciator

Annunciatore compatto con priorità, lampeggio e stato testuale.

```xml
<industrial:AlarmAnnunciator
    Text="PRESSIONE OLIO BASSA"
    Priority="Critical"
    IsActive="{Binding IsOilPressureAlarm}"
    IsAcknowledged="{Binding IsOilPressureAlarmAcknowledged}" />
```

Priorità disponibili:

- `Advisory`
- `Caution`
- `Warning`
- `Critical`

La proprietà `PriorityColor` espone il colore logico come valore `Color`,
utilizzabile anche nei test senza accedere al brush grafico.

#### AlarmAnnunciatorPanel

Raggruppa annunciatori legacy in una griglia adattiva.

```xml
<industrial:AlarmAnnunciatorPanel Columns="3">
  <industrial:AlarmAnnunciator
      Text="PRESSIONE VAPORE BASSA"
      Priority="Warning" />
  <industrial:AlarmAnnunciator
      Text="VUOTO CONDENSATORE BASSO"
      Priority="Critical" />
</industrial:AlarmAnnunciatorPanel>
```

#### BacklitAlarmIndicator

Annunciatore retroilluminato con sequenza completa:

- nuovo allarme;
- riconoscimento;
- rientro della condizione;
- memoria latched;
- reset.

```xml
<industrial:BacklitAlarmIndicator
    AlarmId="STEAM_LOW"
    Text="VAPORE PRINCIPALE"
    SecondaryText="PRESSIONE BASSA"
    Priority="Warning"
    IsLatched="True" />
```

Metodi principali:

```csharp
indicator.Activate();
indicator.Acknowledge();
indicator.ClearCondition();
indicator.Reset();
```

L'annunciatore legacy `AlarmAnnunciator` applica la stessa regola di memoria:
un allarme transitorio con `IsLatched="True"` resta visibile dopo il rientro
finché non viene riconosciuto e ripristinato.

#### AlarmIndicatorPanel

Pannello per annunciatori retroilluminati.

```xml
<industrial:AlarmIndicatorPanel
    x:Name="AlarmPanel"
    Title="ALLARMI IMPIANTO"
    Columns="3">

  <industrial:AlarmIndicatorPanel.Indicators>
    <industrial:BacklitAlarmIndicator
        AlarmId="STEAM_LOW"
        Text="VAPORE PRINCIPALE"
        SecondaryText="PRESSIONE BASSA" />

    <industrial:BacklitAlarmIndicator
        AlarmId="PUMP_TRIP"
        Text="POMPA PRIMARIA"
        SecondaryText="SCATTO" />
  </industrial:AlarmIndicatorPanel.Indicators>
</industrial:AlarmIndicatorPanel>
```

Gestione collettiva:

```csharp
AlarmPanel.Activate("STEAM_LOW");
AlarmPanel.AcknowledgeAll();
AlarmPanel.ClearAllConditions();
AlarmPanel.ResetAll();
```

Contatori osservabili disponibili:

```csharp
AlarmPanel.ActiveConditionCount;
AlarmPanel.LatchedAlarmCount;
AlarmPanel.UnacknowledgedCount;
```

I tre contatori sono `DirectProperty<int>` e notificano automaticamente i
binding quando cambia lo stato di un indicatore o la raccolta viene modificata.

### Gauge

Tutti i gauge condividono le proprietà:

- `Minimum`
- `Maximum`
- `Value`
- `Title`
- `Unit`
- `DecimalPlaces`
- `CautionLow`
- `CautionHigh`
- `WarningLow`
- `WarningHigh`
- `IsAvailable`

Stati calcolati:

- `Normal`
- `Caution`
- `Warning`
- `OutOfRange`
- `Unavailable`

#### RadialGauge

```xml
<industrial:RadialGauge
    Title="PRESSIONE VAPORE"
    Minimum="0"
    Maximum="8"
    Value="{Binding SteamPressureMPa}"
    Unit="MPa"
    MajorTickCount="9"
    MinorTicksPerInterval="4"
    CautionHigh="7.2"
    WarningHigh="7.6" />
```

#### LinearGauge

```xml
<industrial:LinearGauge
    Title="PORTATA ACQUA DI ALIMENTAZIONE"
    Minimum="0"
    Maximum="25"
    Value="{Binding FeedwaterFlow}"
    Unit="kg/s"
    CautionHigh="21"
    WarningHigh="24" />
```

#### DigitalGauge

```xml
<industrial:DigitalGauge
    Title="FREQUENZA RETE"
    Minimum="47"
    Maximum="53"
    Value="{Binding GridFrequencyHz}"
    Unit="Hz"
    DecimalPlaces="2"
    CautionLow="49.5"
    CautionHigh="50.5"
    WarningLow="48.5"
    WarningHigh="51.5" />
```

#### DeviationGauge

Mostra la differenza rispetto a un valore di riferimento.

`Minimum`, `Maximum` e le soglie vengono applicati alla deviazione
`Value - Setpoint`, non al valore di processo assoluto. `Deadband` centra
l'indicatore quando lo scostamento è sufficientemente piccolo, mentre
`Deviation` continua a conservare il valore fisico non filtrato.

```xml
<industrial:DeviationGauge
    Title="ERRORE DI POTENZA"
    Minimum="-5"
    Maximum="5"
    Value="{Binding ActualPowerMWe}"
    Setpoint="{Binding RequestedPowerMWe}"
    Deadband="0.2"
    Unit="MWe"
    DecimalPlaces="2" />
```

Valori disponibili:

```csharp
gauge.Deviation;          // scostamento fisico
gauge.EffectiveDeviation; // scostamento usato da scala e soglie
```

### Comandi operatore

#### IndustrialSlider

```xml
<industrial:IndustrialSlider
    Title="PORTATA ACQUA DI ALIMENTAZIONE"
    Minimum="0"
    Maximum="25"
    Value="{Binding FeedwaterSetpoint}"
    SmallChange="0.1"
    LargeChange="1"
    TickFrequency="0.1"
    Unit="kg/s"
    DecimalPlaces="1"
    IsInterlocked="{Binding IsFeedwaterControlInterlocked}"
    InterlockReason="CONTROLLO NON IN MANUALE" />
```

#### RotaryKnob

```xml
<industrial:RotaryKnob
    Title="POTENZA GENERATORE"
    Minimum="0"
    Maximum="10"
    Value="{Binding RequestedPowerMWe}"
    SmallChange="0.25"
    TickCount="11"
    Unit="MWe"
    DecimalPlaces="2"
    IsInterlocked="{Binding IsLoadControlInterlocked}"
    InterlockReason="INTERRUTTORE APERTO" />
```

#### SelectorSwitch

```xml
<industrial:SelectorSwitch
    Title="MODALITÀ CONTROLLO"
    PositionCount="3"
    Position="{Binding ControlModeIndex}"
    PositionLabels="OFF|AUTO|MANUALE"
    IsInterlocked="{Binding IsModeTransferBlocked}" />
```

#### IndustrialToggleSwitch

Interruttore a leva.

```xml
<industrial:IndustrialToggleSwitch
    Title="INTERRUTTORE GENERATORE"
    OnCaption="CHIUSO"
    OffCaption="APERTO"
    IsChecked="{Binding IsGeneratorBreakerClosed}"
    IsInterlocked="{Binding IsBreakerInterlocked}"
    InterlockReason="GENERATORE NON SINCRONIZZATO" />
```

#### IndustrialRockerSwitch

Interruttore basculante.

```xml
<industrial:IndustrialRockerSwitch
    Title="POMPA CONDENSATO"
    OnCaption="ON"
    OffCaption="OFF"
    IsChecked="{Binding IsCondensatePumpRunning}" />
```

#### SpringReturnSwitch

Comando momentaneo a ritorno elastico.

```xml
<industrial:SpringReturnSwitch
    Title="REGOLAZIONE VELOCITÀ"
    LeftCaption="RIDUCI"
    CenterCaption="MANTIENI"
    RightCaption="AUMENTA"
    IsInterlocked="{Binding IsSpeedTrimInterlocked}" />
```

Con tastiera:

- freccia sinistra o giù: comando sinistro;
- freccia destra o su: comando destro;
- rilascio del tasto: ritorno al centro.

#### InterlockIndicator

Riepilogo dei permissivi.

```xml
<industrial:InterlockIndicator
    Title="PERMISSIVI GENERATORE"
    IsInterlocked="{Binding IsGeneratorInterlocked}"
    SatisfiedPermissiveCount="{Binding SatisfiedPermissives}"
    RequiredPermissiveCount="{Binding RequiredPermissives}"
    Reason="{Binding InterlockReason}" />
```

### Trend e strumenti temporali

#### TrendChart

```xml
<industrial:TrendChart
    x:Name="ProcessTrend"
    Title="POTENZA E PRESSIONE"
    Minimum="0"
    Maximum="10"
    TimeWindowSeconds="60"
    MaxSamplesPerSeries="600"
    AutoScale="True"
    ShowCursor="True"
    ShowGrid="True"
    ShowLegend="True" />
```

Configurazione in C#:

```csharp
var powerSeries = ProcessTrend.AddSeries(
    "POWER",
    "MWe",
    Colors.Green);

var pressureSeries = ProcessTrend.AddSeries(
    "PRESSURE",
    "MPa",
    Colors.Blue);
```

Acquisizione consigliata ad alta frequenza:

```csharp
ProcessTrend.AddSample(
    powerSeries,
    timestampSeconds,
    generatorPowerMWe,
    SignalQuality.Good);
```

È disponibile anche la ricerca per nome:

```csharp
ProcessTrend.AddSample(
    "POWER",
    timestampSeconds,
    generatorPowerMWe);
```

L'handle `SignalTraceSeries` evita la ricerca ripetuta ed è preferibile nei loop
di acquisizione.

I campioni vengono conservati in un buffer circolare limitato da
`MaxSamplesPerSeries`.

#### StripChartRecorder

```xml
<industrial:StripChartRecorder
    x:Name="Recorder"
    Title="REGISTRATORE TERMICO"
    Minimum="0"
    Maximum="100"
    TimeWindowSeconds="45"
    MaxSamplesPerSeries="600"
    PaperSpeed="10"
    MajorGridSeconds="10"
    IsRunning="True" />
```

Il registratore applica una decimazione legata alla larghezza del plot: una
serie molto densa non genera un segmento per ogni campione, ma mantiene un
budget prossimo a un punto per pixel, preservando campioni incerti e
interruzioni dovute a qualità `Bad` o `Unavailable`.

`MajorGridSeconds` controlla realmente la spaziatura della griglia temporale.
`PaperSpeed` resta una velocità nominale visualizzata nell'intestazione; la
finestra temporale sullo schermo è determinata da `TimeWindowSeconds`.

#### OscilloscopeDisplay

```xml
<industrial:OscilloscopeDisplay
    x:Name="Scope"
    Title="ERRORE DI FASE"
    VerticalMinimum="-1"
    VerticalMaximum="1"
    TriggerLevel="0"
    TimebaseMilliseconds="200"
    MaxSamples="256"
    TraceColor="#58D46C" />
```

Aggiornamento:

```csharp
Scope.SetSamples(samples);
Scope.Quality = SignalQuality.Good;
```

#### SignalQualityIndicator

```xml
<industrial:SignalQualityIndicator
    SignalName="FREQUENZA RETE"
    Source="FT-GRID-01"
    Quality="{Binding GridFrequencyQuality}" />
```

Qualità disponibili:

- `Good`
- `Uncertain`
- `Bad`
- `Unavailable`

#### IndustrialScreen

Cornice per trend, grafici, diagnostica o contenuto applicativo.

```xml
<industrial:IndustrialScreen
    Title="TREND IMPIANTO"
    StatusText="LIVE"
    IsOnline="True">

  <industrial:TrendChart
      Title="POTENZA"
      TimeWindowSeconds="60" />
</industrial:IndustrialScreen>
```

## Data binding e MVVM

I controlli espongono proprietà Avalonia e possono essere collegati con binding
standard.

```xml
<industrial:DigitalGauge
    Title="POTENZA"
    Value="{Binding GeneratorPowerMWe}"
    Unit="MWe" />
```

Per i comandi derivati da `Button` o `ToggleButton` usa normalmente `Command`:

```xml
<industrial:IlluminatedPushButton
    Content="START"
    Command="{Binding StartCommand}" />
```

Per controlli con metodi operativi, come gli annunciatori, puoi:

- richiamare i metodi dal code-behind;
- incapsularli in un servizio applicativo;
- esporre comandi dal ViewModel;
- mantenere il controllo visuale separato dal modello di dominio.

## Accessibilità

La libreria assegna automaticamente:

- nome accessibile;
- testo di aiuto;
- automation ID;
- visibilità nell'albero di accessibilità;
- live region assertiva per i nuovi allarmi;
- focus visuale sui controlli interattivi.

Per ottenere descrizioni utili, valorizza sempre proprietà come:

- `Title`
- `Text`
- `SecondaryText`
- `SignalName`
- `Source`
- `AlarmId`

Non affidarti esclusivamente al colore. Mantieni sempre anche testo, unità e
stato operativo.

## Tastiera e puntatore

I controlli interattivi supportano il focus da tastiera.

Il tema mostra un indicatore di focus dedicato e i controlli cliccabili usano il
cursore a mano.

Controlli principali:

- slider: comportamento standard Avalonia;
- manopola: frecce, `Home`, `End`;
- selettore: frecce, `Home`, `End`;
- toggle e rocker: comportamento standard dei toggle button;
- spring-return: tasti freccia mantenuti premuti;
- pulsanti: `Invio` e `Spazio`.

## Prestazioni

I controlli temporali usano buffer limitati.

Per scegliere la capacità:

```text
campioni = frequenza di acquisizione × durata desiderata
```

Esempio: per conservare 60 secondi a 10 campioni al secondo:

```xml
MaxSamplesPerSeries="600"
```

Per acquisizioni dense:

- conserva l'handle restituito da `AddSeries`;
- usa `AddSample(series, ...)`;
- evita di aggiornare il controllo più spesso del necessario;
- separa frequenza di simulazione e frequenza di refresh grafico;
- scegli capacità coerenti con la memoria disponibile;
- usa `StripChartRecorder` quando serve una traccia continua con decimazione
  automatica per pixel.

Benchmark diagnostico:

```powershell
.\scripts\benchmark.ps1
```

## Personalizzazione

Puoi sovrascrivere risorse e proprietà dopo aver incluso
`IndustrialControlsTheme`.

Esempio:

```xml
<Application.Styles>
  <FluentTheme />
  <industrialTheme:IndustrialControlsTheme />

  <Style Selector="industrial|IndustrialPanel">
    <Setter Property="Margin" Value="8" />
  </Style>
</Application.Styles>
```

Per personalizzazioni profonde è consigliabile:

1. mantenere `IndustrialControlsTheme` come base;
2. aggiungere stili applicativi separati;
3. non copiare i template nella tua applicazione se non è strettamente
   necessario;
4. verificare sempre focus, contrasto e stati di allarme.

## Demo completa

Avvia il catalogo:

```powershell
dotnet run --project .\src\IndustrialControls.Avalonia.Demo\
```

La demo contiene tutte le famiglie di controlli e le organizza nelle schede:

- Foundation
- Lamps & LED
- Gauges
- Operator Controls
- Trends & Screens
- Alarm Indicators
- Static & Release

Gli esempi dinamici permettono di verificare:

- resize del marquee LED;
- interlock dei comandi;
- trend e qualità dei segnali;
- oscilloscopio;
- registratore;
- sequenza degli allarmi;
- riconoscimento, rientro e reset.

In caso di errore di avvio della demo, il log viene scritto in:

```text
%LOCALAPPDATA%\IndustrialControls.Avalonia.Demo\startup-error.log
```

## Compilazione e test

Gate completo:

```powershell
.\scripts\validate.ps1
```

Il gate esegue:

1. pulizia di `bin`, `obj`, `TestResults` e `artifacts`;
2. restore;
3. build Release;
4. suite completa dei test tramite Microsoft Testing Platform;
5. controllo del codice di uscita di ogni comando;
6. creazione del pacchetto NuGet;
7. verifica del contenuto del pacchetto.

Qualunque errore interrompe immediatamente lo script: il messaggio finale di
successo viene stampato soltanto dopo test e package validation completati.

Comandi separati:

```powershell
dotnet restore
dotnet build
dotnet test --project .\tests\IndustrialControls.Avalonia.Tests\IndustrialControls.Avalonia.Tests.csproj
dotnet run --project .\src\IndustrialControls.Avalonia.Demo\
```

## Documentazione aggiuntiva

- `docs/PACKAGE_USAGE.md`: integrazione del pacchetto;
- `docs/PUBLIC_API.md`: contratto delle API;
- `docs/ACCESSIBILITY.md`: accessibilità e tastiera;
- `docs/PERFORMANCE.md`: buffer e benchmark;
- `docs/DEMO_CATALOG.md`: catalogo visuale;
- `docs/RELEASE_CHECKLIST.md`: procedura di rilascio.

## Licenza

Consulta il file `LICENSE` incluso nel progetto.






