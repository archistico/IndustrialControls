# Project handoff

## Baseline ufficiale

M3 è la baseline validata.

## Candidate corrente

M4 Hotfix 4 — Test source structure correction.

## Controlli aggiunti

- `GaugeBase`
- `RadialGauge`
- `LinearGauge`
- `DigitalGauge`
- `DeviationGauge`

## Contratto comune

Tutti gli strumenti condividono:

- minimo e massimo;
- valore;
- unità;
- decimali;
- normalizzazione;
- soglie caution e warning;
- stato operativo;
- stato indisponibile.

## Gate

```powershell
.\scripts\validate.ps1
```

La candidate M4 diventa validata solo dopo conferma dell'utente che build, test e demo sono riusciti.

## Prossima milestone

M5 — Operator controls.

## Consegna

Ogni consegna deve essere uno ZIP completo dell'intero progetto pronto per compilazione e test.


## Hotfix 1

Correzione mirata:

- `DeviationGauge.FormattedDeviation` usa `CultureInfo.InvariantCulture`;
- il separatore decimale del contratto visuale resta `.` anche su sistemi `it-IT`;
- aggiunto un test di regressione esplicito con cultura italiana;
- versione libreria aggiornata a `0.4.1`.


## Hotfix 2

Correzione visuale strutturale di `RadialGauge`:

- nuovo renderer vettoriale `RadialGaugeDial`;
- lancetta calcolata e disegnata dal perno centrale;
- tacche maggiori e minori;
- etichette numeriche della scala;
- bande verde, gialla e rossa derivate dalle soglie operative;
- API aggiunte: `MinorTicksPerInterval`, `ScaleDecimalPlaces`,
  `ShowScaleLabels`, `ShowOperatingBands`, `GetAngleForValue`;
- test di regressione sulla geometria angolare;
- versione libreria `0.4.2`.


## Hotfix 3

Affinamenti visuali richiesti dal controllo manuale:

- il testo verde degli strumenti radiali è spostato più in alto;
- la riga di stato non tocca più il bordo inferiore;
- nei pulsanti `IlluminatedPushButton` la lampada è più grande e circolare;
- testo principale e secondario non si sovrappongono più alla lampada;
- versione libreria `0.4.3`.


## Hotfix 4

Correzione esclusivamente strutturale:

- rimosso da `M2ControlContractTests.cs` un test ridondante inserito fuori dalla classe;
- risolti gli errori `CS1519` e `CS1513`;
- nessuna variazione al tema Industrial90 o ai controlli visuali di Hotfix 3;
- versione libreria `0.4.4`.
