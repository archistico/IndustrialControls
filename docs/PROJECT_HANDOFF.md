# Project handoff

## Baseline ufficiale

M4 Hotfix 4 è la baseline validata.

## Candidate corrente

M5 Hotfix 3 — External dial labels and vertical lever refinement.

## Controlli aggiunti

- `IndustrialSlider`
- `RotaryKnob`
- `SelectorSwitch`
- `IndustrialToggleSwitch`
- `SpringReturnSwitch`
- `InterlockIndicator`

## Principi M5

- comando e stato visuale restano separati;
- un interlock impedisce la modifica del comando;
- il motivo dell'interlock resta visibile;
- il comando a molla ritorna sempre al centro;
- i valori numerici usano `InvariantCulture`;
- i renderer vettoriali mantengono la geometria a qualsiasi dimensione.

## Gate

```powershell
.\scripts\validate.ps1
```

La candidate M5 diventa validata solo dopo conferma dell'utente che build, test e prova manuale della demo sono riusciti.

## Prossima milestone

M6 — Trends and screens.

## Consegna

Ogni consegna deve essere uno ZIP completo dell'intero progetto pronto per compilazione e test.


## Hotfix 1

Correzioni visuali richieste dal controllo manuale:

- `RotaryKnob`: il testo del valore non si sovrappone più al quadrante;
- `RotaryKnobDial`: aggiunto arco segmentato con indicazione colorata del livello;
- `SelectorSwitch`: la posizione selezionata non entra più nel bordo del quadrante;
- `ToggleSwitchDial`: leva più grande, a losanga, con corsa quasi verticale coerente con `CLOSED` sopra e `OPEN` sotto;
- nessuna modifica al contratto comportamentale dei controlli;
- versione libreria `0.5.3`.


## Hotfix 2

Correzioni ed estensioni richieste dal controllo manuale:

- `IndustrialToggleSwitch` ora adotta una resa da interruttore a leva con piastra quadrata, sede circolare e testa colorata;
- introdotto `IndustrialRockerSwitch` per il classico comando ON/OFF a bilanciere;
- `SelectorSwitchDial` usa etichette più interne con maggiore distanza dal bordo;
- `SpringReturnSwitch` e `SelectorSwitch` hanno il testo di stato più in basso rispetto al quadrante;
- nessuna modifica alla logica di interlock già validata;
- versione libreria `0.5.3`.


## Hotfix 3

Ulteriori affinamenti visuali:

- etichette di `SelectorSwitch` e `SpringReturnSwitch` riportate all'esterno del quadrante;
- aumentato il margine tra quadrante e testo di stato inferiore;
- `IndustrialToggleSwitch` aggiornato con leva verticale;
- nessuna modifica alla logica dei controlli;
- versione libreria `0.5.3`.
