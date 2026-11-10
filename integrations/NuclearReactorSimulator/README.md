# Pannello pilota della pompa del primario

**Esperimento differito.** La patch è conservata per riferimento e non è
applicata alla copia locale di NuclearReactorSimulator. Il lavoro corrente
riguarda esclusivamente i componenti riutilizzabili di IndustrialControls;
l'integrazione verrà ripresa dal repository del simulatore.

Integrazione per NuclearReactorSimulator, baseline pubblica/localmente verificata
`11b66caf89efc8b151f90cee66ccad33fce7c437`.

`primary-pump.patch` contiene l'implementazione e i test per il repository
consumer. La libreria IndustrialControls rimane indipendente dal simulatore.
La patch è conservata qui per poter riprodurre il lavoro dopo la pulizia degli
artefatti di verifica.

## Funzioni

- Pannello nel workspace PRIMARY CIRCUIT, collegato alla selezione PUMP TARGET.
- START/STOP momentanei instradati al dispatcher Application già esistente.
- Stato e lampada aggiornati soltanto dagli snapshot della pompa selezionata.
- Comandi disabilitati per stato non disponibile, pompa non comandabile o
  stato già raggiunto; anche l'esecuzione programmatica rispetta questi vincoli.
- Tre strumenti: velocità, portata massica, incremento di pressione.
- Testo numerico e unità copiati dal simulatore, qualità e provenienza esplicite.
- Un dato assente/non finito/non disponibile appare come `—` e `UNAVAILABLE`.
- Scala, setpoint, limiti e fuori scala conservati come metadati testuali.

I readout usano `InstrumentBezel` e `SignalQualityIndicator`; non attribuiscono
bande colorate o soglie inventate ai dati. Il selettore e i comandi precedenti
restano disponibili durante la qualificazione del pannello aggiunto.

## Applicazione

Lo script verifica l'applicabilità della patch prima di scrivere. Una patch già
applicata viene riconosciuta; conflitti con modifiche locali interrompono il
comando.

```powershell
.\integrations\NuclearReactorSimulator\apply-primary-pump.ps1 `
    -SimulatorRoot 'E:\sviluppo\2026 NuclearReactorSimulator\NuclearReactorSimulator'
```

Il riferimento a IndustrialControls usa la disposizione delle cartelle locale:

```text
sviluppo/
  2026 NuclearReactorSimulator/NuclearReactorSimulator/
  2026 IndustrialControls/IndustrialControls/
```

Per altre disposizioni passare la proprietà MSBuild `IndustrialControlsProject`
con il percorso del file `src/IndustrialControls.Avalonia/IndustrialControls.Avalonia.csproj`.

Nel repository consumer:

```powershell
dotnet build src/NuclearReactorSimulator.App/NuclearReactorSimulator.App.csproj -c Release
dotnet test --project tests/NuclearReactorSimulator.App.Tests/NuclearReactorSimulator.App.Tests.csproj -c Release
dotnet run --project src/NuclearReactorSimulator.App/NuclearReactorSimulator.App.csproj -c Release
```

Aprire PRIMARY CIRCUIT, selezionare la pompa e osservare la variazione del
feedback dopo la pubblicazione del simulatore. I comandi mantengono i vincoli
dello scenario corrente; un rigetto compare nel percorso di stato comandi
già presente.

## Verifica automatica

Nella copia di verifica, build Release dell'app completata senza errori o avvisi.
Suite App: 154 test superati, 0 falliti, 1 ignorato dal contratto preesistente
di accettazione manuale HMI. La patch aggiunge 15 casi su feedback, comandi,
qualità, dati assenti, fuori scala, cambio target e rifiuto del dispatcher.
L'inventario dei controlli include il nuovo gruppo di tre readout. Raccolta e
istanze dei readout restano stabili tra gli snapshot; dati identici non generano
notifiche aggiuntive per gli strumenti.

Questo passaggio non completa la migrazione degli allarmi o dei gauge analogici.
La qualificazione visiva con resize/DPI/tastiera e le misure di rendering
restano attività successive; la suite completa della simulazione non è stata
eseguita per questa modifica di presentazione.
