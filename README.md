# IndustrialControls.Avalonia

Libreria Avalonia riutilizzabile per interfacce industriali e sale controllo, con estetica anni Novanta.

## M5 Hotfix 3

Include le milestone M0–M4 validate e aggiunge:

- `IndustrialSlider`;
- `RotaryKnob`;
- `SelectorSwitch`;
- `IndustrialToggleSwitch`;
- `SpringReturnSwitch`;
- `InterlockIndicator`;
- slider con `Track` e `Thumb` industriali;
- manopola vettoriale con mouse, rotella e tastiera;
- selettori da due a cinque posizioni;
- comando bistabile ON/OFF;
- comando momentaneo con ritorno automatico al centro;
- interlock e permissivi espliciti;
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

M0–M4 Hotfix 4: **VALIDATED**  
M5 Hotfix 3: **CANDIDATE**


## Hotfix 1 visual refinements

- `RotaryKnob`: il valore numerico verde è ora sotto la manopola e sopra lo stato;
- `RotaryKnobDial`: aggiunta indicazione colorata del livello attuale;
- `SelectorSwitch`: la didascalia della posizione selezionata è stata abbassata;
- `ToggleSwitchDial`: leva ridisegnata in stile losanga, più grande e con movimento quasi verticale;
- versione libreria aggiornata a `0.5.3`.


## Hotfix 2 visual refinements

- `IndustrialToggleSwitch` è stato ridisegnato come vero interruttore a leva, ispirato al riferimento allegato;
- nuovo `IndustrialRockerSwitch` in stile ON/OFF a bilanciere con simbologia `I / O`;
- `SelectorSwitchDial` e `SpringReturnSwitch` hanno etichette e testi con più margine dal bordo;
- la demo mostra ora entrambi i tipi di interruttore;
- versione libreria aggiornata a `0.5.3`.


## Hotfix 3 layout refinements

- `SelectorSwitchDial`: etichette delle posizioni nuovamente all'esterno del quadrante;
- `SpringReturnSwitch`: `LOWER`, `HOLD` e `RAISE` all'esterno con margine maggiore;
- `IndustrialToggleSwitch`: leva resa verticale, con stato alto/basso più coerente.
