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

- versione `1.0.0-rc.9`;
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

Stato: **CANDIDATE — RC6-D**

## Gate finale

Dopo la validazione locale di RC6-D verrà preparata la release stabile `1.0.0`.


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

Stato: **VALIDATED**


### RC6-B Hotfix 1

Correzione literal C# nel test del gate di validazione.


### RC6-C — Rendering & Performance Hardening

- capacità marquee deterministica;
- sorgente marquee in cache;
- decimazione per pixel dello strip chart;
- risorse grafiche riutilizzate;
- griglia temporale basata su MajorGridSeconds;
- benchmark render-plan da 100.000 campioni;
- pseudo-classe interlocked comune.

Stato: **VALIDATED — HOTFIX 2**


### RC6-C Hotfix 1

Correzione `cref` XML della proprietà ereditata `TimeWindowSeconds`.


### RC6-C Hotfix 2

Diagnostica esplicita delle discontinuità e dei punti incerti nello strip chart.


### RC6-D — Final API Cleanup & Release Gate

- rimozione dell'API decorativa `PaperSpeed`;
- header strip-chart basato su finestra e griglia reali;
- comportamento bistabile condiviso internamente;
- normalizzazione robusta degli automation ID;
- validazione esatta del `.nupkg`;
- smoke test con applicazione consumer indipendente;
- checklist finale e documentazione API riallineate.

Stato: **VALIDATED — HOTFIX 3**


### RC6-D Hotfix 1

Correzione del literal C# nel test del `PackageReference` consumer.


### RC6-D Hotfix 2

Riallineamento del test XML-doc al contratto finale dello strip chart.


### RC6-D Hotfix 3

Isolamento del progetto consumer temporaneo dalla Central Package Management
del repository.


### IndustrialControls.Avalonia 1.0.0

- promozione della versione stabile;
- metadati NuGet e runtime allineati;
- changelog stabile;
- gate rinominato `1.0.0 VALIDATION PASSED`;
- package consumer configurato sulla versione stabile;
- checklist finale convertita in gate di accettazione della baseline.

Stato: **VALIDATED — 167 TESTS, PACKAGE AND CONSUMER GATES PASSED**


### IndustrialControls.Avalonia 1.0.0 Docs1

- cartella radice `screenshot`;
- sette schermate del catalogo demo;
- galleria visuale nel README;
- immagini incluse e verificate nel pacchetto NuGet.

Stato: **DOCUMENTATION CANDIDATE — PACKAGE REVALIDATION REQUIRED**
