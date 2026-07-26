# IndustrialControls.Avalonia

Libreria Avalonia riutilizzabile per interfacce industriali e sale controllo, con estetica anni Novanta.

## Baseline M7

Include le milestone M0–M6 validate e aggiunge:

- `BacklitAlarmIndicator`;
- `AlarmIndicatorPanel`;
- `AlarmIndicatorVisualState`;
- `SafetyPlacard`;
- `SafetyPlacardLevel`;
- `SafetyPlacardIcon`;
- `BoltedDataPlate`;
- `DataPlateMaterial`;
- ciclo allarme con attivazione, ACK, rientro e RESET;
- memoria latched opzionale;
- pannello allarmi con layout multicolonna;
- pannelli statici di sicurezza con icone e viti;
- targhette dati imbullonate;
- demo interattiva;
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

M0–M6 Hotfix 2: **VALIDATED**  
M7 Hotfix 3: **CANDIDATE**


## M7 Hotfix 1

- `AlarmIndicatorPanel` ora deriva da `ItemsControl`;
- rimosso l'override non consentito di `Panel.Render`;
- cornice, intestazione e viti sono definite nel tema Industrial90;
- gli indicatori sono disposti da un `UniformGrid`;
- API collettive `Activate`, `AcknowledgeAll`, `ClearAllConditions` e `ResetAll` invariate;
- versione libreria `0.7.1`.


## M7 Hotfix 2

- `AlarmIndicatorPanel` ora deriva da `TemplatedControl`;
- introdotta la raccolta logica `Indicators`;
- `Indicators` è una `ObservableCollection<BacklitAlarmIndicator>` indipendente da `ItemsControl.Items`;
- il template usa un `ItemsControl` interno soltanto per la visualizzazione;
- eliminata la dipendenza dei test dal dispatcher Avalonia;
- versione libreria `0.7.2`.


## M7 Hotfix 3

- migliorata la leggibilità di `BacklitAlarmIndicator` quando è spento;
- negli stati `CLEAR` e `DISABLED` il testo usa ora un tono chiaro ad alto contrasto;
- la retroilluminazione `CLEAR` resta debole ma leggermente più leggibile;
- aggiunto test di regressione per il foreground dello stato spento;
- versione libreria `0.7.3`.
