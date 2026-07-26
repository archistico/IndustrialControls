# Roadmap

## M0 — Foundation

**VALIDATED**

## M1 — Industrial90 visual foundation

**VALIDATED**

## M2 — Lamps and illuminated buttons

**VALIDATED**

## M3 — LED displays and alarm systems

**VALIDATED**

## M4 — Gauges

**VALIDATED — HOTFIX 4**

## M5 — Operator controls

**VALIDATED — HOTFIX 3**

## M6 — Trends and screens

**VALIDATED — HOTFIX 2**

## M7 — Alarm Indicators & Static Panel Elements

**VALIDATED — HOTFIX 3**

## M8 — Stabilization and release

Release candidate corrente:

- versione `1.0.0-rc.7`;
- contratto API pubblico;
- metadati di accessibilità;
- live region per allarmi;
- navigazione tastiera;
- focus adorner;
- test di copertura del tema;
- verifica long-run dei buffer;
- benchmark smoke;
- pack NuGet;
- ispezione automatica del pacchetto;
- documentazione di integrazione e rilascio.

Stato: **CANDIDATE — RC5**

## Gate finale

Dopo la validazione locale di M8 RC1 verrà preparata la release stabile `1.0.0`.


### RC2 — Allocation & update-path optimization

- buffer circolare O(1);
- lookup serie tramite dizionario;
- overload diretto con `SignalTraceSeries`;
- cursore trend lazy;
- cache di pennelli, formati e label;
- metadati di accessibilità coalescenti;
- benchmark con bytes/operazione.


### RC3 — Demo catalog & startup hardening

- `FocusAdornerTemplate` corretto;
- diagnostica di avvio;
- finestra di fallback;
- demo completa in sette schede;
- test di copertura del catalogo.


### RC4 — Lamps & LED visual refinement

- marquee adattivo alla larghezza;
- ingresso del testo dal bordo destro;
- annunciatori legacy con palette neutra;
- lente circolare esplicita;
- cursore a mano sui controlli cliccabili.


### RC5 — Dispatcher-independent alarm palette

- `AlarmAnnunciator.PriorityColor`;
- test palette non dipendente da `SolidColorBrush`;
- rendering legacy invariato.


### RC6-A — Functional safety fixes

- DeviationGauge basato sullo scostamento;
- deadband operativo;
- memoria latched per AlarmAnnunciator;
- pointer capture del SpringReturnSwitch;
- rilascio su interlock via binding;
- interlock completo del pulsante illuminato.

Stato: **CANDIDATE**


### RC6-A Hotfix 1

Correzione namespace `Avalonia.Styling` per le pseudo-classi.


### RC6-A Hotfix 2

Gestione pseudo-classi tramite `IPseudoClasses.Add/Remove`.


### RC6-B — Avalonia property and reactive contracts

- side effect spostati negli handler AvaloniaProperty;
- coercizione binding-safe di selettore e manopola;
- contatori allarmi osservabili;
- cursore trend correttamente invalidato;
- lampeggio non riavviato dai testi;
- fallback accessibilità senza SynchronizationContext;
- gate validate fail-fast e compatibile con MTP.

Stato: **CANDIDATE**


### RC6-B Hotfix 1

Correzione literal C# nel test del gate di validazione.
