# Proposta UX C — "Liturgia": il calendario guida la scelta dei canti

> Wireframe: [UX-C-liturgia.html](UX-C-liturgia.html) · Confronto: [CONFRONTO-UX.md](CONFRONTO-UX.md)

## 1. Concetto

L'app parte da **quando** e **perché** si canta: il calendario liturgico. La home è **la prossima celebrazione** (es. *Domenica 15 novembre 2026 · XXXIII del Tempo Ordinario · Anno A*), con i riferimenti delle letture del giorno e lo **schema della Messa** a slot: Ingresso, Kyrie, Gloria, Salmo, Acclamazione al Vangelo, Offertorio, Santo, Agnello di Dio, Comunione, Finale.

Ogni slot si riempie con **suggerimenti ragionati**: canti adatti a quel momento, coerenti con il tempo liturgico, collegati alle letture tramite i riferimenti biblici, non ripetuti di recente, già nel repertorio del coro. Il risultato è un **libretto** che si condivide con il coro e si usa in celebrazione.

Modello mentale dell'utente: *"Che domenica è? Cosa dice il Vangelo? Quali canti ci stanno bene, e quali sa il coro?"*

**A chi serve meglio**
- L'animatore liturgico / responsabile del coro che ogni settimana sceglie i canti.
- Parrocchie con più cori o più messe (stessa domenica, scelte diverse).
- Chi vuole sfruttare davvero i metadati ricchi (momenti, riferimenti biblici, tempi liturgici).

**Chi serve peggio**: chi non suona in chiesa (cantautori, musical, band) — per loro l'app è solo la sezione *Canti*; chi apre al volo un canto qualsiasi.

### Principi guida
1. **Il calendario è la struttura.** Avvento, Natale, Quaresima, Pasqua, Tempo Ordinario e feste determinano cosa proporre (es. niente Gloria in Avvento e Quaresima, niente Alleluia in Quaresima).
2. **Letture come chiave di ricerca.** Il Vangelo del giorno (es. Mt 25,14-30) viene confrontato con i riferimenti biblici dei canti.
3. **Memoria del coro.** L'app ricorda cosa si è cantato e quando, per evitare ripetizioni e per far emergere il repertorio conosciuto.
4. **Solo riferimenti, non testi.** L'app mostra i riferimenti delle letture (dati di fatto), non i testi della traduzione liturgica, che sono protetti.

## 2. Architettura dell'informazione

```
Prossima (home)              ← celebrazione in arrivo + schema Messa a slot
 ├─ Celebrazione
 │   ├─ Slot (momento)       → suggerimenti, scelta, tonalità, note
 │   ├─ Proponi tutto con l'AI
 │   ├─ Libretto             → condividi con il coro / esporta PDF
 │   └─ Celebra              ← modalità esecuzione guidata dai momenti
Calendario                   ← settimane, feste, tempi; occasioni (matrimonio, funerale, battesimo, prima comunione)
Canti                        ← libreria con filtri liturgici in primo piano; import, AI, modifica, pubblica
Profilo                      ← coro/gruppo, rito, messe abituali, strumento, notazione, lingua, offline, Termini
```

**Album e raccolte** diventano: *Repertorio del coro* (manuale), *Raccolte per tempo liturgico* (automatiche: "Avvento", "Mariani"), *Libretti* (celebrazioni passate, riutilizzabili).

### Navigazione per dispositivo

| Dispositivo | Struttura | Note |
|---|---|---|
| **Telefono** | Barra inferiore: *Prossima · Calendario · Canti · Profilo*. | La celebrazione è una lista verticale di slot; ogni slot si apre a tutto schermo con i suggerimenti. |
| **Tablet** | Colonna sinistra con gli slot della celebrazione, destra con suggerimenti o canto. In **Celebra**: canto a sinistra, elenco momenti a destra (o due pagine in orizzontale). | |
| **Desktop Windows** | **Pianificatore a settimane**: colonne = domeniche del mese, righe = momenti. Si vede in un colpo d'occhio cosa si ripete. | Il punto di forza su desktop: preparare un mese intero o un tempo liturgico. |

## 3. Flussi principali

### 3.1 Cercare e aprire una canzone
- **Dalla celebrazione**: tocco su uno slot → suggerimenti già filtrati (momento + tempo + letture) → tocco su un canto → anteprima → **Usa per Comunione**.
- **Libero**: *Canti* → ricerca con filtri liturgici in evidenza (momento, tempo, riferimento biblico) → canzone.
- **Ricerca per Vangelo**: in *Canti*, scrivendo "Mt 25" si trovano i canti con quel riferimento o con riferimenti vicini.

### 3.2 Trasporre
1. Nel canto dentro la celebrazione: **− / tonalità / +**.
2. La tonalità scelta si salva **per il coro** (es. "Coro giovani: Re", "Coro adulti: Do") e viene riproposta alle celebrazioni successive.

### 3.3 Cambiare strumento e notazione
- Strumento e notazione nel profilo; ogni coro/gruppo può avere il suo strumento predefinito (es. organo/piano per la messa delle 10:30, chitarra per quella dei giovani).
- Nel canto: selettore **Chitarra · Piano · Solo testo** (il "solo testo" è utile per i coristi).

### 3.4 Importare da PDF / OCR
1. **Canti → + → Da PDF o foto** (singolo canto o raccolta): revisione affiancata, metadati proposti **compresi i liturgici** (momento suggerito, tempo, riferimento biblico se citato nel PDF).
2. **Calendario → Importa libretti passati**: un PDF di un vecchio foglietto popola lo storico ("cosa abbiamo cantato il 12 aprile"), utile per le ripetizioni.

### 3.5 Chiedere accordi all'AI
- Dal canto senza accordi, o dallo slot ("questo canto non ha accordi"): modulo precompilato, avviso fisso su affidabilità e copyright, risultato in **Bozza AI** da verificare.
- **Uso distinto dell'AI per la scelta**: "Proponi tutto" chiede all'AI di riempire gli slot motivando ogni scelta con letture e tempo. L'AI sceglie solo tra canti esistenti in libreria o catalogo.

### 3.6 Creare un album manuale
- *Canti → Repertorio del coro → + Aggiungi* (selezione multipla).
- Ogni celebrazione salvata è anche un libretto riutilizzabile: **Duplica per un'altra data**.

### 3.7 Creare un album automatico
1. **Canti → Raccolte → Proponi**: l'AI raggruppa per tempo liturgico, momento o tema biblico ("Canti sul seminare e raccogliere", "Avvento per coro di voci bianche").
2. Ogni proposta mostra il criterio; si accetta come raccolta o la si trasforma in regola automatica.

### 3.8 Preparare l'uso offline
- **Automatico per le prossime 2 celebrazioni** (impostabile): tutti i canti scelti, con PDF e MP3 opzionali.
- **Profilo → Offline**: tutta la libreria, il repertorio del coro, raccolte singole, canti singoli; indicatore di spazio.
- La celebrazione mostra "Pronta offline ✓" accanto alla data.

### 3.9 Pubblicare una canzone con dichiarazione di responsabilità
- Da un canto privato: **⋯ → Pubblica** → stato dei diritti (dominio pubblico / mia composizione / ho licenza / non so → bloccato) → anteprima → dichiarazione *"Sono responsabile del contenuto e dei relativi diritti"* + Termini, non preselezionati.
- **Avviso specifico**: molti canti liturgici recenti sono protetti anche se cantati ogni domenica; il flusso lo ricorda esplicitamente ("Cantato in chiesa non significa libero da diritti").
- **Condivisione del libretto** con il coro: titoli e ordine sempre; i testi dei canti privati restano privati.

## 4. Comportamento in esecuzione dal vivo: modalità **Celebra**

- Segue l'**ordine della Messa**: in alto il momento corrente in grande ("COMUNIONE"), sotto il canto.
- **Avanti / indietro per momento** con pedale, tocco sui bordi o tasti; un tocco sul nome del momento apre l'elenco di tutti i momenti per saltare (il celebrante salta il Gloria? un tocco).
- Gli slot vuoti o "parlati" (es. Kyrie recitato) restano come segnaposto e si saltano da soli.
- Tema scuro, schermo sempre acceso, offline.
- Per i coristi: la stessa celebrazione in **solo testo**, sincronizzabile al momento corrente del direttore (opzione futura, richiede rete locale).

## 5. Punti di forza
- **Massimo valore per il pubblico principale** (animatori e cori parrocchiali): nessuna app generica fa questo.
- **Sfrutta tutti i metadati** che i requisiti chiedono di raccogliere (momenti, riferimenti biblici, tag liturgici).
- Lo **storico** evita ripetizioni ed è un supporto reale alla programmazione.
- Il **pianificatore a settimane** su desktop è uno strumento di lavoro unico.
- Differenziazione forte per un eventuale catalogo pubblico: i canti pubblicati arrivano già "liturgicamente indicizzati".

## 6. Punti deboli
- **Esclude** (o mette in secondo piano) chi non suona in chiesa, nonostante i requisiti dicano "soprattutto di chiesa, ma non solo".
- **Curva di apprendimento** più alta: slot, tempi, celebrazioni, cori.
- I suggerimenti sono buoni **solo se i canti sono ben taggati**: con una libreria appena importata gli slot restano poveri (serve un aiuto AI al tagging).
- **Dati liturgici**: calcolare il calendario (Pasqua mobile, anni A/B/C, feste, specificità italiane, rito ambrosiano) e mantenere una tabella dei riferimenti delle letture è un lavoro non banale e soggetto a errori; i testi non si possono includere.

## 7. Costo di implementazione in MAUI: **ALTO**
- La UI in sé è standard (liste, schede, Shell), ma richiede più schermate delle altre proposte.
- **Motore del calendario liturgico**: calcolo del calendario romano generale + calendario italiano, cicli A/B/C, tempi; da scrivere e testare a fondo (o da integrare da una fonte esterna con licenza compatibile).
- **Tabella dei riferimenti del lezionario** (solo riferimenti, ~1000 voci) da costruire e verificare.
- **Motore di suggerimento** (momento + tempo + vicinanza biblica + storico) e storico delle esecuzioni per coro.
- Concetto di **coro/gruppo** con preferenze proprie: nuovo nel modello dati.
- Il pianificatore a griglia su desktop richiede un layout dedicato.
