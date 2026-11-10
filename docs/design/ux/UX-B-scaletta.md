# Proposta UX B — "Scaletta": dal leggio all'indietro

> Wireframe: [UX-B-scaletta.html](UX-B-scaletta.html) · Confronto: [CONFRONTO-UX.md](CONFRONTO-UX.md)

## 1. Concetto

L'app è progettata **a partire dal momento in cui si suona**. L'oggetto centrale non è la canzone ma la **scaletta**: una sequenza ordinata di canti legata a un evento (una messa, una prova, un concerto, un matrimonio), con le *scelte di quella sera* — tonalità, strumento, capotasto, note ("solo strofe 1 e 3", "intro 4 battute").

Modello mentale dell'utente: *"Sabato suono. Preparo la scaletta a casa, poi sul leggio vado avanti senza pensare."*

La libreria esiste, ma è il **magazzino** da cui si pesca; la home è la **prossima scaletta**, con il suo stato di prontezza.

**A chi serve meglio**
- Chi suona dal vivo con regolarità: animatori liturgici, band, cori, chi accompagna matrimoni e feste.
- Chi suona con le mani occupate e vuole andare avanti con pedale o tocchi grandi.
- Chi condivide lo stesso programma con altri musicisti.

**Chi serve peggio**: chi usa l'app come archivio da consultare, senza eventi; chi vuole una preparazione liturgica guidata dalle letture (qui la Messa è solo un modello di scaletta).

### Principi guida
1. **La scaletta è il documento.** Ogni scelta fatta "per sabato" vale per sabato, senza sporcare la canzone originale.
2. **Prontezza visibile.** Una scaletta dice da sola se è pronta: tutto offline? tutti i canti hanno accordi? tonalità decise?
3. **Il palco è una modalità a sé**, non una variante della pagina canzone: niente menu, solo avanti/indietro, tonalità e "fuori programma".
4. **Mai bloccati dal vivo**: la ricerca d'emergenza ("il celebrante ha cambiato canto") è a un tocco anche in modalità palco.

## 2. Architettura dell'informazione

```
Scalette (home)
 ├─ Prossima scaletta (in evidenza, con prontezza)
 ├─ Scaletta
 │   ├─ Voci ordinate (canto + scelte della sera) / separatori ("Dopo l'omelia")
 │   ├─ Controllo prontezza  → scarica offline, chiedi accordi AI, decidi tonalità
 │   ├─ Modalità Palco       ← cuore dell'app
 │   └─ Condividi con il gruppo
 ├─ Modelli: Messa domenicale, Matrimonio, Concerto, Prova
 └─ Repertori (scalette senza data = album manuali)
Canti                     ← libreria: ricerca, filtri, canzone, modifica, import, AI
Scopri                    ← catalogo pubblico
Profilo                   ← strumento, notazione, lingua, pedale, offline, account, Termini
```

**Album e raccolte** diventano due cose distinte con lo stesso motore: *scaletta* (con data, ordinata, con scelte della sera) e *repertorio* (senza data, es. "Canti mariani del coro"). Le raccolte automatiche dell'AI producono una **bozza di scaletta o di repertorio**.

### Navigazione per dispositivo

| Dispositivo | Struttura | Note |
|---|---|---|
| **Telefono** | Barra inferiore: *Scalette · Canti · Scopri · Profilo*. Pulsante grande **▶ Palco** sulla scaletta. | La modalità Palco è a schermo intero, senza barra inferiore; uscita con "Esci" esplicito (non con un gesto accidentale). |
| **Tablet** | Preparazione: scaletta a sinistra, canto a destra. **Palco orizzontale: due pagine affiancate** come un leggio aperto; la seconda pagina può mostrare l'inizio del canto successivo. | Il tablet è il dispositivo "principe" di questa proposta. |
| **Desktop Windows** | **Tavolo di preparazione**: libreria a sinistra, scaletta al centro, anteprima a destra; trascinamento libreria → scaletta (alternativa: pulsante "Aggiungi"/`Invio`). | Stampa/export PDF della scaletta per chi non usa il tablet. |

## 3. Flussi principali

### 3.1 Cercare e aprire una canzone
- **In preparazione**: *Canti* → ricerca con filtri → canzone (come in un'app libreria) → **Aggiungi a scaletta…** (sceglie la prossima per default).
- **Dentro una scaletta**: tocco su una voce → canto con le scelte della sera.
- **Dal vivo, fuori programma**: in Palco, pulsante **Cerca** sempre visibile → ricerca a tutto schermo con tastiera e filtri recenti → il canto si apre *inserito dopo quello corrente* (con "Annulla" per toglierlo). La scaletta resta coerente.

### 3.2 Trasporre
1. Dentro la scaletta: **− / tonalità / +** nel canto.
2. Al primo cambio compare la scelta: **Solo per questa scaletta** (default) o **Sempre per questo canto**.
3. Nella scaletta ogni voce mostra la tonalità della sera ("Re ← Do") così il gruppo vede subito cosa è cambiato.
4. In Palco: tocco lungo sulla tonalità → ±1 semitono rapido, senza aprire menu.

### 3.3 Cambiare strumento e notazione
1. Ogni scaletta ha uno **strumento predefinito** (es. "Messa 15/11 · Piano"), ereditato dal profilo.
2. Ogni voce può fare eccezione (es. "Salmo: solo testo").
3. Notazione Do-Re-Mi / C-D-E nel profilo; una scaletta condivisa si mostra a ciascuno nella sua notazione.

### 3.4 Importare da PDF / OCR
Due ingressi:
- **Canti → + → Da PDF o foto**: import di un singolo canto, revisione affiancata (come in A), salvataggio privato.
- **Scalette → + → Da libretto PDF**: il foglietto/libretto della celebrazione viene letto per intero; l'app riconosce i titoli, **li abbina ai canti già in libreria** ("Mira il tuo popolo → trovato") e propone di importare quelli mancanti con OCR. Il risultato è una scaletta già ordinata.

### 3.5 Chiedere accordi all'AI
1. Il **controllo prontezza** della scaletta segnala: "1 canto senza accordi".
2. Tocco → modulo AI precompilato con titolo/autore e **tonalità della sera**.
3. Avviso fisso (suggerimento automatico, non garantito, possibile copyright); risultato come **Bozza AI** che nella scaletta resta marcato finché non viene verificato.

### 3.6 Creare un album manuale (repertorio o scaletta)
1. **Scalette → + Nuova** → scegli modello: *Vuota · Messa domenicale · Matrimonio · Concerto · Repertorio (senza data)*.
2. Il modello Messa crea già i **separatori dei momenti** (Ingresso, Offertorio, Comunione, Finale…), da riempire.
3. Aggiunta canti: ricerca dentro la scaletta, o da *Canti* con "Aggiungi a scaletta".
4. Riordino con maniglie e pulsanti ↑/↓; aggiunta di note per voce.

### 3.7 Creare un album automatico
1. **+ Nuova → Componi con l'AI**.
2. Richiesta guidata: occasione, durata o numero di canti, strumento, vincoli ("tonalità comode per voci femminili", "non ripetere i canti delle ultime 3 settimane").
3. L'AI restituisce una **bozza di scaletta** con il motivo di ogni scelta ("scelto per Comunione; usato l'ultima volta 6 settimane fa").
4. L'utente sostituisce singole voci ("Proponi un'alternativa") e conferma.

### 3.8 Preparare l'uso offline
- **Automatico**: ogni scaletta con data viene scaricata appena creata o modificata (testi + PDF; MP3 opzionali).
- La scheda **Pronta per il palco?** mostra: ✓ 12/12 canti sul dispositivo · ✓ accordi presenti · ✓ tonalità decise · pedale collegato · batteria.
- *Profilo → Offline*: scaricare tutta la libreria, singoli repertori o singoli canti, con spazio occupato.

### 3.9 Pubblicare una canzone con dichiarazione di responsabilità
- Dalla canzone in *Canti*: **⋯ → Pubblica** → stato dei diritti → anteprima → dichiarazione *"Sono responsabile del contenuto e dei relativi diritti"* (caselle non preselezionate, link ai Termini).
- **Condividere una scaletta** con il gruppo è un'azione diversa: condivide titoli, ordine e scelte della sera; i canti *privati* restano visibili solo come titolo agli altri, a meno che non siano pubblicati. L'app lo spiega prima di inviare.

## 4. Comportamento in esecuzione dal vivo (cuore della proposta)

- **Schermo**: tema scuro ad alto contrasto, schermo sempre acceso, luminosità ricordata per la modalità.
- **Zone di tocco grandi**: terzo destro = avanti, terzo sinistro = indietro, centro = mostra/nasconde i controlli. Avanzamento per **pagina**, poi per canto: alla fine del canto, il tocco successivo passa al canto dopo.
- **Pedale Bluetooth e tastiera** (frecce, PagGiù, spazio); su Android opzionalmente i **tasti volume**.
- **Anteprima del successivo** sempre visibile in basso ("Dopo: Salve Regina · Re").
- **Indicatore di posizione**: "Comunione · 9/12".
- **Note della sera** in evidenza in testa al canto ("solo strofe 1 e 3").
- **Fuori programma**: Cerca → inserisci dopo il corrente.
- **Tablet orizzontale**: due pagine affiancate, giro "di mezza pagina" (si sostituisce solo la metà già letta) per non perdere il punto.
- **Uscita protetta**: "Esci dal palco" chiede conferma; il gesto Indietro del sistema non esce per errore.

## 5. Punti di forza
- **Il migliore per l'uso dal vivo**, che è il momento più critico e quello in cui un PDF non basta.
- Le **scelte della sera** separate dalla canzone risolvono un problema reale (tonalità diverse per cori diversi).
- **Generale**: funziona per la messa, ma anche per la band, il matrimonio, il concerto.
- **Prontezza offline** risolve il problema "in chiesa non c'è campo" senza che l'utente ci pensi.
- Import da libretto PDF → scaletta: un flusso che nessun archivio offre.

## 6. Punti deboli
- Chi vuole solo consultare un canto ha un passaggio in più (la home non è la ricerca).
- Due concetti da capire (scaletta con data / repertorio senza data) al posto di "album".
- La preparazione liturgica è aiutata dal modello Messa ma **non guidata dalle letture del giorno**.
- La modalità Palco va curata molto: se è mediocre, l'intera proposta perde valore.

## 7. Costo di implementazione in MAUI: **MEDIO**
- Navigazione Shell standard; la modalità Palco è una pagina modale a schermo intero.
- `DeviceDisplay.KeepScreenOn` è già in MAUI. Pedale e tastiera richiedono gestione eventi tasto **per piattaforma** (override di `DispatchKeyEvent` nella `MainActivity` Android, `KeyDown` sulla finestra WinUI): poco codice, ma da testare con pedali reali.
- Due pagine affiancate e "mezza pagina": impaginazione custom del ChordPro (misurare il testo e spezzarlo in pagine) — è la parte più costosa.
- Riordino: `CollectionView` con `CanReorderItems` funziona su Android; su Windows serve l'alternativa con pulsanti ↑/↓ (prevista comunque per accessibilità).
- Modello dati: nuova entità *voce di scaletta* con tonalità, strumento, note; non stravolge lo schema esistente (Album → Scaletta/Repertorio).
