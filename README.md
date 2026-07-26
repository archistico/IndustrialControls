# IndustrialControls.Avalonia

Libreria Avalonia riutilizzabile per interfacce industriali e sale controllo, con estetica anni Novanta.

## M4 Hotfix 4

Include le milestone M0–M3 validate e aggiunge:

- `GaugeBase`;
- `RadialGauge`;
- `LinearGauge`;
- `DigitalGauge`;
- `DeviationGauge`;
- normalizzazione del valore;
- formattazione con unità ingegneristiche;
- soglie di cautela e allarme;
- stati normale, cautela, allarme, fuori scala e indisponibile;
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

M0–M3: **VALIDATED**  
M4 Hotfix 4: **CANDIDATE**

La hotfix rende il formato dello scostamento indipendente dalla cultura del sistema operativo.


## RadialGauge Hotfix 2

Il quadrante radiale usa ora rendering vettoriale:

- lancetta ancorata al perno centrale;
- scala coerente fra lancetta, tacche e valori;
- tacche maggiori e minori;
- valori numerici configurabili;
- bande operative verdi, gialle e rosse ricavate dalle soglie;
- ridimensionamento senza disallineamenti geometrici.


## Hotfix 3 visual refinements

- `RadialGauge`: valore e stato sollevati per evitare contatto col bordo inferiore;
- `IlluminatedPushButton`: lampada più grande, circolare e separata dal testo;
- `IlluminatedPushButton`: layout verticale corretto, senza sovrapposizione tra luce e didascalie.


## Hotfix 4 build correction

- corretto `M2ControlContractTests.cs`;
- rimosso il metodo di test accidentalmente collocato dopo la chiusura della classe;
- nessuna modifica ai controlli o alla resa grafica approvata con Hotfix 3.
