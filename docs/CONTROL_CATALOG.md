# Control catalog

## IndustrialPanel

Contenitore con intestazione, sottotitolo, fissaggi e superficie industriale.

## InstrumentBezel

Cornice da incasso per strumenti e display.

## EngravedLabel

Targhetta industriale con varianti nera, rossa e alluminio.

## IndustrialLamp

Spia industriale con:

- stato acceso separato dal colore;
- sei colori funzionali;
- quattro forme;
- intensità;
- etichetta;
- lampeggio lento o veloce;
- stato guasto;
- stato indisponibile.

## IlluminatedPushButton

Pulsante industriale con:

- lampada incorporata;
- stato lampada separato dallo stato meccanico;
- modalità momentanea;
- modalità toggle;
- testo principale;
- testo secondario;
- `Command` e `CommandParameter` ereditati da `Button`.

## Pianificati

- `LedMatrixDisplay`
- `LedMarqueeDisplay`
- `AlarmAnnunciatorPanel`
- `RadialGauge`
- `LinearGauge`
- `DigitalGauge`
- `DeviationGauge`
- `RotaryKnob`
- `RotarySelector`
- `IndustrialSlider`
- `ToggleSwitch`
- `IndustrialTrendChart`
- `OscilloscopeDisplay`
- `StripChartRecorder`


## LedMatrixDisplay

Display testuale industriale statico, con colore LED, luminosità e matrice nominale 5×7 o 7×9.

## LedMarqueeDisplay

Display LED con finestra di caratteri e scorrimento temporizzato.

## SevenSegmentDisplay

Display numerico con numero di cifre, decimali, zeri iniziali e unità ingegneristica.

## AlarmAnnunciator

Tessera di allarme con priorità, stato attivo, riconoscimento, memorizzazione e ripristino.

## AlarmAnnunciatorPanel

Contenitore industriale per matrici di annunciatori.


## GaugeBase

Contratto comune per strumenti analogici e digitali: campo, valore, unità, formattazione, normalizzazione e soglie operative.

## RadialGauge

Strumento circolare vettoriale con:

- lancetta ancorata al centro;
- tacche maggiori e minori;
- etichette numeriche;
- bande operative verde, gialla e rossa;
- soglie condivise con `GaugeBase`;
- scala e densità delle tacche configurabili.

## LinearGauge

Barra industriale per livelli, portate e posizioni.

## DigitalGauge

Indicatore numerico ad alta leggibilità con unità e stato.

## DeviationGauge

Indicatore centrato sul setpoint per mostrare scostamenti positivi e negativi.


## IndustrialSlider

Slider industriale per valori di riferimento e comandi analogici. Mostra titolo, valore formattato, unità, estremi del campo e stato di interlock.

## RotaryKnob

Manopola rotativa vettoriale con campo numerico, incremento discreto, indicatore angolare e supporto per click, rotella e tastiera.

## SelectorSwitch

Selettore rotativo configurabile da due a cinque posizioni, con etichette e blocco interlock.

## IndustrialToggleSwitch

Interruttore a leva bistabile con didascalie ON/OFF personalizzabili e comando impedito quando interbloccato.

## SpringReturnSwitch

Comando momentaneo a tre posizioni sinistra-centro-destra. Al rilascio ritorna sempre nella posizione centrale.

## InterlockIndicator

Indicatore centralizzato di permissivi soddisfatti, stato consentito/interbloccato e motivo operativo.


## IndustrialRockerSwitch

Interruttore ON/OFF a bilanciere in stile pannello industriale, con simbologia `I / O`, stato bistabile e interlock.


## TrendChart

Trend multicanale con finestra temporale, griglia, legenda, cursore, auto-scaling e qualità dei campioni.

## OscilloscopeDisplay

Oscilloscopio a singola traccia con scala verticale, trigger, base dei tempi, capacità limitata e qualità del segnale.

## StripChartRecorder

Registratore multicanale a carta continua. Accetta nuovi campioni solo quando `IsRunning` è attivo, mostra la finestra configurata da `TimeWindowSeconds`, usa `MajorGridSeconds` per la griglia e decima le serie dense in funzione della larghezza del plot.

## SignalQualityIndicator

Indicatore visuale per stati `Good`, `Uncertain`, `Bad` e `Unavailable`.

## IndustrialScreen

Cornice riutilizzabile per monitor operativi, con titolo, stato online e sovrapposizione scanline opzionale.

## TimeSeriesControlBase

Contratto comune per serie temporali, capacità, range, auto-scaling, visibilità e gestione dei campioni.


# Planned M7 controls

## BacklitAlarmIndicator

Indicatore di allarme retroilluminato con testo, priorità, lampeggio, riconoscimento e stato latched.

## AlarmIndicatorPanel

Matrice configurabile di indicatori di allarme retroilluminati.

## SafetyPlacard

Pannello statico con viti agli angoli, icona e testo di attenzione o sicurezza.

## BoltedDataPlate

Targhetta statica per dati tecnici, valori nominali, identificativi o istruzioni, con fissaggi agli angoli.


## BacklitAlarmIndicator

Indicatore rettangolare retroilluminato con priorità, lampeggio, ACK, rientro, memoria latched e RESET.

## AlarmIndicatorPanel

Pannello industriale multicolonna con cornice, intestazione, fissaggi e comandi collettivi per ACK, rientro e RESET.

## SafetyPlacard

Pannello statico di sicurezza con livello, icona, testo e fissaggi agli angoli.

## BoltedDataPlate

Targhetta dati con contenuto libero, titolo, identificativo, materiale e quattro viti.


# M8 release services

## IndustrialControlsRelease

Static release metadata containing the candidate version and supported theme resource URI.

## Industrial90.FocusAdorner

Shared keyboard-focus visual used by interactive controls.
