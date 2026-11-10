# Industrial90 visual language

## Principi

- Aspetto industriale anni Novanta, non semplice tema scuro.
- Superfici robuste, bordi netti e profondità leggibile.
- Plastiche grigie, metallo freddo, vetro fumé.
- Raggi ridotti e cornici marcate.
- Illuminazione interna controllata, evitando effetti neon moderni.
- Testo tecnico ad alto contrasto.
- Stati mai comunicati soltanto dal colore.

## Palette iniziale

| Risorsa | Impiego |
|---|---|
| `Industrial90.PanelBrush` | superficie principale |
| `Industrial90.PanelDarkBrush` | testate e zone profonde |
| `Industrial90.RecessBrush` | incassi e strumenti |
| `Industrial90.EdgeLightBrush` | bordo illuminato |
| `Industrial90.EdgeDarkBrush` | bordo in ombra |
| `Industrial90.TextBrush` | testo principale |
| `Industrial90.MutedTextBrush` | testo secondario |
| `Industrial90.GlassBrush` | vetro fumé |

## Targhette

Le targhette possono essere:

- nere;
- rosse;
- alluminio;
- con o senza fissaggi.

## Evoluzione

M2 introdurrà i colori funzionali delle lampade. M3 introdurrà la matrice LED rossa e gli stati allarme.


## Colori funzionali M2

- rosso: arresto, allarme o condizione critica;
- ambra: attenzione;
- giallo: cautela;
- verde: marcia, pronto o condizione normale;
- blu: comando, remoto o informazione;
- bianco: stato neutro o disponibilità.

L'applicazione ospitante mantiene la responsabilità semantica: la libreria non assegna automaticamente un significato operativo al colore.

## Interruttore a leva

Il toggle usa esclusivamente colori pieni della palette Industrial90. Le viti
sono dischi grigi da 9 px con bordo scuro da 1 px, come `BoltedDataPlate`.
La cornice usa un bordo da 4 px e un filetto interno da 1 px; la leva riprende
lo stelo da 8 px con contorno da 12 px del selettore. Non usa gradienti,
riflessi cromati o dentellature minute. La posizione alta/bassa distingue ON
e OFF; l'interlock attenua il grigio della leva. L'assieme vettoriale si
ridimensiona in modo proporzionale.

Per confrontare le quattro varianti grafiche:

```powershell
dotnet run --project src/IndustrialControls.Avalonia.Demo -- --render-toggle toggle-preview.png
```
