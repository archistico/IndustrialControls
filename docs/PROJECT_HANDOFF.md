# Project handoff

## Baseline ufficiale

M6 Hotfix 2 è la baseline validata.

## Candidate corrente

M7 Hotfix 3 — off-state annunciator readability.

## Controlli aggiunti

- `BacklitAlarmIndicator`
- `AlarmIndicatorPanel`
- `SafetyPlacard`
- `BoltedDataPlate`

## Contratto allarmi M7

Il ciclo operativo è:

1. `Activate()` crea un nuovo allarme e annulla l'ACK precedente;
2. `Acknowledge()` riconosce l'allarme;
3. `ClearCondition()` rappresenta il rientro della condizione;
4. se `IsLatched` è attivo, l'indicatore resta memorizzato;
5. `Reset()` è consentito solo dopo rientro e ACK.

Gli stati derivati sono:

- `Clear`;
- `NewAlarm`;
- `AcknowledgedActive`;
- `ReturnedUnacknowledged`;
- `ReadyToReset`;
- `Disabled`.

## Elementi statici

`SafetyPlacard` fornisce:

- cinque livelli di attenzione;
- sei icone;
- viti agli angoli;
- testo statico.

`BoltedDataPlate` fornisce:

- contenuto libero;
- titolo, sottotitolo e identificativo;
- quattro materiali;
- quattro fissaggi.

## Gate

```powershell
.\scripts\validate.ps1
```

M7 diventa validata solo dopo conferma dell'utente che build, test e controllo manuale della demo sono riusciti.

## Prossima milestone

M8 — Stabilization and Release.

## Consegna

Ogni consegna deve essere uno ZIP completo dell'intero progetto pronto per compilazione e test.


## Hotfix 1

Correzione di compilazione:

- `Panel.Render(DrawingContext)` non viene più sovrascritto;
- `AlarmIndicatorPanel` è un `ItemsControl`;
- il rendering del pannello è demandato a `ControlTheme`;
- il layout usa `UniformGrid` con colonne e spaziature configurabili;
- i test usano `Items.Add` invece di `Children.Add`;
- versione libreria `0.7.1`.


## Hotfix 2

Correzione dei test concorrenti:

- `ItemsControl.Items` non viene più usato come archivio logico;
- `AlarmIndicatorPanel.Indicators` è una collection .NET indipendente dal thread UI;
- il template visuale consuma la collection tramite `ItemsSource`;
- demo e test usano la proprietà `Indicators`;
- le API collettive operano esclusivamente sulla collection logica;
- versione libreria `0.7.2`.


## Hotfix 3

Correzione visuale:

- quando l'annunciatore è in `CLEAR`, il testo non usa più il colore scuro da stato attivo;
- `CLEAR` usa un foreground chiaro caldo, leggibile sul fondo attenuato;
- `DISABLED` usa un foreground grigio chiaro;
- opacità `CLEAR` alzata leggermente da `0.16` a `0.18`;
- aggiunto test di regressione.
