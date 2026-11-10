# Confronto delle tre proposte UX

| | Proposta | Documento | Wireframe |
|---|---|---|---|
| **A** | Libreria — il canzoniere come archivio | [UX-A-libreria.md](UX-A-libreria.md) | [UX-A-libreria.html](UX-A-libreria.html) |
| **B** | Scaletta — dal leggio all'indietro | [UX-B-scaletta.md](UX-B-scaletta.md) | [UX-B-scaletta.html](UX-B-scaletta.html) |
| **C** | Liturgia — il calendario guida la scelta | [UX-C-liturgia.md](UX-C-liturgia.md) | [UX-C-liturgia.html](UX-C-liturgia.html) |

## Perché questi tre concetti

Le tre proposte rispondono a tre domande diverse che lo stesso utente si pone in momenti diversi:
- **A**: *"Dov'è quel canto?"* — l'oggetto centrale è la **canzone**.
- **B**: *"Cosa suono sabato, e in che ordine?"* — l'oggetto centrale è la **scaletta** (evento + scelte della sera).
- **C**: *"Cosa è giusto cantare domenica?"* — l'oggetto centrale è la **celebrazione** nel calendario liturgico.

Sono state scartate alternative più deboli: un'app "centrata sull'editor/import" (utile solo nella fase di costruzione dell'archivio) e una "social/catalogo pubblico" (dipende da una comunità che oggi non c'è e aumenta il rischio copyright).

Elementi **comuni a tutte** (non sono criteri di scelta): rendering ChordPro, trasposizione con tonalità originale sempre visibile, selettore Chitarra / Piano / Solo testo, notazione Do-Re-Mi o C-D-E, revisione affiancata dell'OCR, "Bozza AI" non pubblicabile finché non verificata, pubblicazione in 3 passi con dichiarazione non preselezionata, stato offline indicato con icona e parola, IT/EN.

## Tabella comparativa

Scala: ●●●●● = ottimo, ● = scarso.

| Criterio | A · Libreria | B · Scaletta | C · Liturgia |
|---|---|---|---|
| **Rapidità nell'aprire un canto qualsiasi** | ●●●●● ricerca in home, 2 tocchi | ●●●● un tab in più; ricerca d'emergenza anche dal palco | ●●● ottima *dentro* uno slot, più lenta fuori contesto |
| **Uso dal vivo** | ●●● leggio buono per un canto; sequenza solo se aperto da una raccolta | ●●●●● modalità Palco, anteprima del successivo, scelte della sera, fuori programma, due pagine su tablet | ●●●● ottima per la Messa (salto per momento); inadatta a concerti e band |
| **Preparazione** | ●● raccolte manuali, nessun luogo per "l'evento" | ●●●● scalette con modelli, prontezza, libretto PDF → scaletta, AI | ●●●●● calendario, letture, suggerimenti per slot, storico, pianificatore a settimane |
| **Scoperta dei contenuti pubblici** | ●●●● "Scopri" come seconda libreria | ●●● catalogo + abbinamento automatico del libretto PDF | ●●●● il catalogo entra nei suggerimenti degli slot, già indicizzato liturgicamente |
| **Curva di apprendimento** | ●●●●● familiare a chiunque | ●●●● un concetto nuovo (scaletta / repertorio) | ●● slot, tempi, cori, celebrazioni; ovvio per un animatore, opaco per gli altri |
| **Costo MAUI** | **Basso-medio** | **Medio** (pedale/tasti per piattaforma, impaginazione a pagine) | **Alto** (calendario liturgico, tabella lezionario, motore suggerimenti, storico per coro) |
| **Adattamento telefono / tablet / desktop** | ●●●● master-detail classico su tutti | ●●●●● telefono per il palco d'emergenza, tablet come leggio a due pagine, desktop come tavolo di preparazione | ●●●● ottimo il pianificatore su desktop; telefono più affollato (molti slot) |
| **Copre il "non solo chiesa"** | ●●●●● | ●●●●● | ●● |

## Raccomandazione

**Scegliere B — "Scaletta" come struttura dell'app, prendendo da C il modello "Messa" a momenti e da A la ricerca della sezione Canti.** Il calendario liturgico completo di C va rimandato a una fase successiva, come estensione delle scalette.

Motivi:
1. **Il momento critico è il leggio.** È lì che l'app deve battere un PDF o un raccoglitore: con le mani occupate, poca luce e senza rete. B è l'unica proposta progettata a partire da quel momento, e le sue scelte (offline automatico per le scalette con data, anteprima del successivo, ricerca d'emergenza) rispondono direttamente al contesto d'uso descritto.
2. **Copre tutti i pubblici dei requisiti** ("soprattutto di chiesa, ma non solo"): la stessa scaletta serve alla messa, alla band, al matrimonio. C escluderebbe metà degli usi.
3. **Il valore liturgico arriva comunque a basso costo**: il modello "Messa domenicale" con i momenti come sezioni, i filtri per momento e riferimento biblico, il "non usato di recente" nella ricerca. Sono l'80% del vantaggio di C senza il motore del calendario.
4. **Costo medio, con un percorso incrementale chiaro**: v1 = Canti (A) + Scalette e Palco (B); v2 = calendario, letture, suggerimenti per slot e pianificatore a settimane (C), che si innestano sulle scalette senza rifare la navigazione.
5. **Le "scelte della sera"** (tonalità e strumento per evento, non per canzone) risolvono un problema concreto che A e C gestiscono peggio.

Rischio principale della raccomandazione: la qualità della modalità Palco. Va prototipata per prima e provata su un tablet reale, con un pedale reale, in una chiesa poco illuminata.

## Decisioni richieste al proprietario

1. **Concetto**: confermare B (+ modello Messa da C), oppure scegliere un'altra proposta.
2. **Calendario liturgico** (se ora o in v2): calcolarlo in proprio o usare una fonte esterna (quale, con quale licenza)? Solo rito romano o anche ambrosiano? Mostrare solo i riferimenti delle letture (consigliato: i testi CEI sono protetti).
3. **Pedale e tasti volume**: supportarli già in v1? Serve un modello di pedale Bluetooth di riferimento per i test.
4. **Condivisione di scalette con il gruppo**: è richiesta? Implica account e backend per gruppi, oltre al catalogo pubblico.
5. **Account obbligatorio o no**: si propone uso completo senza account; account solo per pubblicare, condividere e sincronizzare.
6. **Notazione predefinita**: Do-Re-Mi per l'italiano e C-D-E per l'inglese, modificabile? (si propone sì).
7. **Moderazione del catalogo pubblico**: pubblicazione immediata con segnalazioni, oppure revisione prima della pubblicazione?
