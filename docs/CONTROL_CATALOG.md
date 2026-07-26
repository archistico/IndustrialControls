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
