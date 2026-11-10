# Proposta UX A — "Libreria": il canzoniere come archivio personale

> Wireframe: [UX-A-libreria.html](UX-A-libreria.html) · Confronto: [CONFRONTO-UX.md](CONFRONTO-UX.md)

## 1. Concetto

L'app è prima di tutto **un archivio di canti ben indicizzato**, come la libreria musicale di un telefono o un raccoglitore ad anelli ben tenuto. L'oggetto centrale è la **canzone**; tutto il resto (raccolte, contenuti pubblici, import, AI) serve ad alimentare o a filtrare la libreria.

Modello mentale dell'utente: *"Ho tanti canti. Voglio trovarne uno in tre secondi e suonarlo."*

**A chi serve meglio**
- Chi suona molto repertorio diverso (chiesa, cantautori, musical) senza una struttura fissa.
- Chi apre l'app all'ultimo momento: "il parroco ha detto *Mira il tuo popolo*, dov'è?".
- Chi sta costruendo il proprio archivio (import massivo da PDF, OCR, catalogo pubblico).

**Chi serve peggio**: chi prepara una messa precisa ogni settimana (deve ricostruire la sequenza a mano in una raccolta) e chi suona dal vivo una lunga scaletta senza toccare lo schermo.

### Principi guida
1. **La ricerca è la home.** Il campo di ricerca è sempre in alto, con filtri a chip immediatamente sotto (momento della Messa, tonalità, lingua, tipo, tag).
2. **Filtri, non cartelle.** Le raccolte esistono, ma la via principale per restringere la libreria è filtrare per metadati.
3. **Una canzone, una pagina completa.** La schermata canzone contiene tutto: lettura, tonalità, strumento, media, metadati, azioni.
4. **Privato / pubblico / offline sempre dichiarati** con un'icona e una parola (mai solo colore).

## 2. Architettura dell'informazione

```
Libreria (home)          ← ricerca + filtri + elenco canzoni
 ├─ Canzone              ← lettura, trasposizione, strumento, media, metadati
 │   ├─ Modifica         ← editor ChordPro + metadati
 │   ├─ Suggerisci accordi (AI)
 │   └─ Pubblica         ← flusso responsabilità
Raccolte
 ├─ Raccolta (manuale)   ← ordinabile
 ├─ Raccolta intelligente (regola su filtri)
 └─ Proposte AI          ← bozze da accettare
Scopri                   ← catalogo pubblico condiviso
Aggiungi (+)             ← nuova / da PDF / da foto (OCR) / da audio / da AI
Profilo e impostazioni   ← strumento, notazione, lingua, account, sync, offline, Termini
```

### Navigazione per dispositivo

| Dispositivo | Struttura | Note |
|---|---|---|
| **Telefono** (portrait) | Barra inferiore con 4 voci: *Libreria · Raccolte · Scopri · Profilo*; pulsante flottante **+ Aggiungi** in Libreria e Raccolte. | 4 voci, sotto il limite di 5. Import/OCR/audio non sono più tab (oggi lo sono) ma azioni di "Aggiungi". |
| **Tablet** (≥ 768 px) | **Master-detail**: a sinistra elenco filtrato (≈ 320 px), a destra la canzone aperta. Navigation rail verticale a sinistra con le 4 voci. | Cambiare canzone non fa perdere filtri né scorrimento. |
| **Desktop Windows** | Tre colonne: rail/sidebar con Raccolte espanse, elenco, canzone. Scorciatoie: `Ctrl+F` ricerca, `+`/`-` trasposizione, `Ctrl+N` nuova, `F11` lettura. | Trascinamento canzone → raccolta, con alternativa da menu contestuale "Aggiungi a raccolta". |

## 3. Flussi principali

### 3.1 Cercare e aprire una canzone
1. L'app si apre su **Libreria**, con il cursore *non* nel campo (per non aprire la tastiera inutilmente); in alto "Recenti" (ultimi 5 aperti) in riga orizzontale.
2. Tocco su **Cerca**: si digita "mira". Risultati istantanei su titolo, autore, prima riga del testo, riferimento biblico.
3. Opzionale: chip filtro **Comunione**, **Tonalità: Re**, **Solo offline**.
4. Tocco sul risultato → **Canzone** in modalità lettura. Su tablet si apre nel pannello destro.

### 3.2 Trasporre
1. Nella canzone, la barra in basso mostra **− 1 · Re (orig. Do) · + 1**.
2. Ogni tocco sposta di un semitono; la tonalità attuale e quella originale restano visibili.
3. Tocco sulla tonalità → foglio con le 12 tonalità, opzione "♯ / ♭", "Capotasto suggerito" (solo chitarra: es. "Capo 2, accordi in Do").
4. La trasposizione è **salvata per quella canzone** (preferenza personale), con "Ripristina originale".

### 3.3 Cambiare strumento e notazione
1. Selettore segmentato nella barra della canzone: **Chitarra · Piano · Solo testo**.
2. Chitarra: accordi sopra il testo + diagrammi in fondo + tab se presente. Piano: accordi con note (es. "Re = re-fa♯-la") e mini-tastiera al tocco sull'accordo.
3. La notazione (**Do-Re-Mi** / **C-D-E**) si imposta in Profilo; scorciatoia nel menu "Aa" della canzone (dimensione testo, notazione, colonne).

### 3.4 Importare da PDF / OCR
1. **+ Aggiungi → Da PDF o foto**.
2. Selezione file (PDF testuale, PDF scansione, foto da fotocamera). L'app riconosce il tipo e lo dice: "PDF di testo: estrazione diretta" oppure "Scansione: riconoscimento ottico (richiede rete)".
3. Se il PDF contiene più canti → elenco delle pagine/canti rilevati con spunte.
4. **Revisione affiancata**: a sinistra l'immagine originale, a destra il ChordPro proposto; righe con bassa confidenza evidenziate con bordo tratteggiato e icona "?".
5. Metadati suggeriti (titolo, tonalità rilevata). Salvataggio come **privata**; il PDF originale resta allegato.

### 3.5 Chiedere accordi all'AI
1. Dalla canzone senza accordi (o da **+ Aggiungi → Chiedi all'AI**): banner "Questa canzone non ha accordi. Chiedi un suggerimento".
2. Modulo precompilato: titolo, autore, lingua, eventuale testo; scelta tonalità desiderata.
3. Avviso fisso prima dell'invio: *"Suggerimento automatico, non garantito. Verifica prima di suonarlo. Il brano potrebbe essere protetto da diritto d'autore."*
4. Risultato mostrato come **bozza** con etichetta "Bozza AI" su ogni pagina finché l'utente non preme **Ho verificato, salva**. Le bozze AI non sono pubblicabili finché restano bozze.

### 3.6 Creare una raccolta manuale
1. **Raccolte → + Nuova raccolta** → nome, descrizione, icona.
2. Dalla libreria: selezione multipla (pressione prolungata) → **Aggiungi a raccolta**; oppure dalla canzone, menu ⋯ → **Aggiungi a raccolta**.
3. Nella raccolta: **Riordina** (maniglie trascinabili + pulsanti ↑/↓ come alternativa accessibile).

### 3.7 Creare una raccolta automatica
1. **Raccolte → Proposte AI**: carte con proposte generate dal catalogo ("Canti mariani in Re/Sol (12)", "Natale per voce sola (8)").
2. Ogni carta spiega il **criterio** in linguaggio naturale e mostra le prime 3 canzoni.
3. **Accetta** → diventa raccolta modificabile; **Trasforma in regola** → diventa *raccolta intelligente* che si aggiorna da sola (es. "tag = Mariano AND tonalità in {Re, Sol}").
4. In alternativa l'utente scrive la richiesta: "canti per un matrimonio, non troppo difficili".

### 3.8 Preparare l'uso offline
1. Ogni canzone/raccolta ha l'icona offline (nuvola barrata = solo online, spunta = sul dispositivo).
2. Su una raccolta: **⋯ → Rendi disponibile offline** (testi + PDF + MP3 opzionali, con dimensione stimata).
3. **Profilo → Offline**: "Tutta la libreria (312 canti, 48 MB)", "Solo testi", elenco raccolte con interruttore, spazio usato.
4. Filtro **Solo offline** in libreria per verificare cosa c'è prima di uscire.

### 3.9 Pubblicare una canzone con dichiarazione di responsabilità
1. Canzone privata → **⋯ → Pubblica nel catalogo condiviso** (richiede account).
2. **Passo 1 — Stato dei diritti** (obbligatorio): *Dominio pubblico · Composizione mia · Ho licenza/permesso · Non so* ("Non so" blocca la pubblicazione e spiega perché).
3. **Passo 2 — Anteprima** di come apparirà nel catalogo (autore, fonti, licenza).
4. **Passo 3 — Dichiarazione**: casella non preselezionata *"Sono responsabile del contenuto e dei relativi diritti"* + link ai Termini. Pulsante **Pubblica** attivo solo dopo la spunta.
5. Conferma: la canzone porta l'etichetta "Pubblica", con data e possibilità di ritirarla.

## 4. Comportamento in esecuzione dal vivo

- **Modalità lettura** (pulsante "Leggio" nella canzone o `F11`): nasconde barre, schermo sempre acceso, tema scuro ad alto contrasto, testo grande.
- **Scorrimento**: tocco nella metà inferiore = pagina giù, metà superiore = pagina su; scorrimento automatico opzionale con velocità regolabile.
- **Pedale Bluetooth / tastiera**: pagina giù/su.
- **Limite del modello**: il passaggio alla canzone successiva esiste solo se la canzone è stata aperta *da una raccolta* (frecce "precedente/successiva" della raccolta). Dalla ricerca non c'è un "dopo".
- Indicatore offline sempre visibile; se il canto non è sul dispositivo e manca rete, messaggio chiaro con alternativa ("Apri il PDF allegato" se presente).

## 5. Punti di forza
- **Il più familiare**: ricorda un'app musicale o di note; curva di apprendimento minima.
- **Apertura rapida di un canto qualunque**: ricerca + filtri in primo piano.
- **Scala bene da telefono a desktop** con un master-detail classico.
- Il catalogo pubblico ("Scopri") si integra naturalmente: è "un'altra libreria".
- Riutilizza gran parte delle pagine esistenti (Search, SongDetail, AlbumList), riordinandole.

## 6. Punti deboli
- La **preparazione** (della domenica o di un concerto) non ha un luogo proprio: si arrangia con le raccolte.
- L'**uso dal vivo** è buono per una canzone, debole per una sequenza.
- Il valore liturgico (momenti, riferimenti biblici) è solo un filtro tra tanti: il coro parrocchiale non trova "la sua" app.
- Rischio di schermata canzone sovraccarica (lettura + media + metadati + azioni).

## 7. Costo di implementazione in MAUI: **BASSO-MEDIO**
- Shell con `TabBar` su telefono e `FlyoutBehavior="Locked"` su desktop: componenti standard.
- Master-detail su tablet: non esiste un controllo pronto, ma un `Grid` adattivo con `VisualStateManager` per larghezza è lavoro contenuto.
- La parte costosa è comune a tutte le proposte: rendering ChordPro (accordi allineati sopra le sillabe) con un controllo custom o `GraphicsView`, e la revisione OCR affiancata.
- Raccolte intelligenti: una regola serializzata sui filtri della ricerca, riusando la stessa query.
