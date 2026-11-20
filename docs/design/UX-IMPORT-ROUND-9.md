# Proposta UX — schermata di import da PDF (round 9)

> Onda R9-1, lavoro U1 di [PIANO-ROUND-9.md](../PIANO-ROUND-9.md). Segue la navigazione e il vocabolario di [UX-B](ux/UX-B-scaletta.md) (§3.4) e [UX-A](ux/UX-A-libreria.md) (§3.4). Riferimenti: [FASI.md](../FASI.md) F3 e F7, DEC 36-38 in [DECISIONI-ROUND-9.md](../DECISIONI-ROUND-9.md), piano commerciale [PC] §3.2 righe 18 e 23, R-04, R-05.

## 1. Scopo e vincoli

La schermata porta un PDF dal disco alla libreria: un canto singolo o un libretto intero. Sostituisce la pagina OCR simulata (DEC 37).

Vincoli del round:
- **Si costruisce in un round.** Controlli MAUI standard, più il controllo di disegno del canto già esistente per l'anteprima.
- **Stile provvisorio.** Colori e misure vengono dal dizionario provvisorio con le chiavi dei token. F4 li sostituisce senza toccare la pagina.
- **Nessuna rete** (R-04, R-05). Lettura, OCR e salvataggio avvengono sul dispositivo. La schermata lo dice una volta, in modo chiaro.
- **Nessuna icona emoji.** La pagina attuale ne usa: vanno tolte. Ogni stato si dice con una parola, più un glifo se serve.
- **Android tablet** (verticale e orizzontale) e **Windows** (finestra ridimensionabile, tastiera, mouse). Il telefono funziona con lo stesso layout stretto, ma non è un obiettivo.

Vocabolario (da UX-A e UX-B): *canto*, *libreria*, *libretto*, *revisione affiancata*, *dichiarazione*, *salva*. Non si usa "OCR" nei testi visibili: si dice "scansione" e "riconoscimento del testo".

## 2. Ingresso

| Dove | Round 9 | Da F6 in poi |
|---|---|---|
| Barra delle schede attuale | La scheda "OCR Import" diventa **"Importa"**, stessa posizione, nuova rotta `import`. La vecchia rotta `ocrimport` sparisce. | La scheda sparisce. L'import diventa **Canti → + → Da PDF** (UX-B §3.4). |
| Windows | `Ctrl+O` sulla pagina Importa apre il selettore di file. | `Ctrl+O` ovunque nella sezione Canti. |
| Libretto → scaletta | Non in questo round. | UX-B §3.4, in F8. |

La pagina Importa è una sola pagina con stati interni. Non si naviga tra pagine diverse, tranne per aprire un canto salvato. Così il ritorno indietro è semplice e lo stato non si perde.

## 3. Flusso

```
S0 Avvio ──► selettore di file ──► S1 Lettura ──┬─► S2 Libretto: riepilogo ed elenco ──► S3 Revisione ──┐
   ▲              │ annulla                      │                │  salva selezionati            │ salva
   │              ▼                              │                ▼                               ▼
   └──────── resta in S0                         ├─► (un solo canto) ──► S3 Revisione ──────► S4 Dichiarazione
                                                 │                                                │ salva
                                                 ├─► S6 Errore ──► S0                             ▼
                                                 └─► S7 Annullato ──► S0               S5 Salvato ──► canto (songdetail)
                                                                                                  ◄── indietro: S2
```

Passi:
1. **S0 Avvio.** L'utente tocca **Scegli un PDF**. Si apre il selettore di file nativo (solo `.pdf`).
2. **S1 Lettura.** L'app legge le pagine di testo, poi riconosce le scansioni. Mostra "Pagina n di m" e un pulsante **Annulla** sempre attivo.
3. **Esito.**
   - Un solo canto → si va diretti a **S3 Revisione**.
   - Più canti → **S2**: riepilogo del documento ed elenco dei canti trovati.
   - Nessun canto, file illeggibile, OCR non disponibile → **S6 Errore**.
   - Annullato → **S7**.
4. **S2 Elenco.** Spunte, "Tutti" e "Nessuno", avvisi visibili su ogni riga. Tocco su una riga → S3 per quel canto. **Salva selezionati** → S4.
5. **S3 Revisione affiancata.** Testo originale, ChordPro modificabile, anteprima dal vivo. **Salva questo canto** → S4. Frecce per il canto precedente e successivo.
6. **S4 Dichiarazione.** Casella non preselezionata (DEC 38). **Salva** è disattivo finché non si spunta.
7. **S5 Salvato.** Messaggio di conferma con **Apri**. Nell'elenco la riga diventa "Salvato" e non è più selezionabile. **Apri** porta alla schermata del canto (rotta `songdetail`). Indietro torna all'elenco, con scorrimento e spunte intatti.
8. **Nuovo import.** Il pulsante **Altro PDF** riporta a S0. Se ci sono canti modificati e non salvati, chiede conferma.

## 4. Larghezze e layout

Le soglie sono sulla larghezza della pagina, non sul tipo di dispositivo. Così la finestra di Windows si comporta come il tablet.

| Classe | Larghezza | Casi tipici | S2 elenco | S3 revisione |
|---|---|---|---|---|
| Stretta | < 700 dp | Telefono, finestra Windows stretta | Elenco a tutta larghezza | Un pannello, con selettore **Originale · ChordPro · Anteprima** |
| Media | 700–1199 dp | Tablet verticale (≈ 800 dp) | Elenco a tutta larghezza, riepilogo in alto | Due pannelli: a sinistra **Originale · Anteprima** a scelta, a destra ChordPro |
| Larga | ≥ 1200 dp | Tablet orizzontale (≈ 1280 dp), Windows | Elenco a sinistra (≈ 360 dp), revisione del canto scelto a destra | Tre colonne: Originale · ChordPro · Anteprima |

Nella classe larga S2 e S3 convivono: elenco a sinistra, revisione a destra (master-detail di UX-A §2). Nelle altre classi S3 copre l'elenco e ha un pulsante **← Elenco**.

## 5. Wireframe per stato

### S0 Avvio (tablet verticale)

```
┌──────────────────────────────────────────────┐
│ Importa                                      │
├──────────────────────────────────────────────┤
│                                              │
│   Importa canti da un PDF                    │
│   Un canto singolo o un libretto intero.     │
│   Tutto avviene sul dispositivo, senza rete. │
│                                              │
│   ┌────────────────────────────────────┐     │
│   │        Scegli un PDF               │ 56dp│
│   └────────────────────────────────────┘     │
│                                              │
│   Le pagine scansionate richiedono più       │
│   tempo: anche un minuto su un tablet        │
│   economico.                                 │
│                                              │
│   [ ] Misura i tempi di correzione           │
│       (prova Gate 0, vedi §9)                │
│                                              │
└──────────────────────────────────────────────┘
```

Su Windows il pulsante mostra anche "Ctrl+O".

### S1 Lettura

```
┌──────────────────────────────────────────────┐
│ Importa                                      │
├──────────────────────────────────────────────┤
│   libretto-parrocchia-2025.pdf               │
│                                              │
│   Riconoscimento del testo (scansione)       │
│   Pagina 12 di 48                            │
│   ████████████░░░░░░░░░░░░░░░░░  25%         │
│   Circa 1 minuto rimanente                   │
│                                              │
│   Pagine di testo: 40 lette                  │
│   Scansioni: 3 di 8 riconosciute             │
│                                              │
│   ┌────────────────────────────────────┐     │
│   │            Annulla                 │ 56dp│
│   └────────────────────────────────────┘     │
└──────────────────────────────────────────────┘
```

Regole:
- Barra determinata (pagina n di m), mai uno spinner senza numeri.
- Due fasi con nomi diversi: "Lettura delle pagine di testo" e "Riconoscimento del testo (scansione)".
- Il tempo rimanente compare solo dopo 3 pagine scansionate, come media. Se costa troppo, si omette.
- Lo schermo resta acceso durante la lettura.
- Il lettore di schermo annuncia l'avanzamento ogni 5 pagine, non a ogni pagina.

### S2 Libretto: riepilogo ed elenco (tablet verticale, classe media)

```
┌──────────────────────────────────────────────┐
│ Importa                          Altro PDF   │
├──────────────────────────────────────────────┤
│ libretto-parrocchia-2025.pdf                 │
│ 48 pagine: 40 di testo, 8 scansioni          │
│ 151 canti trovati · l'indice ne conta 171  ⓘ │
│ ! 2 pagine non lette: 12, 40          Dettagli│
├──────────────────────────────────────────────┤
│ [Tutti] [Nessuno]   [ ] Solo da controllare  │
│ 151 selezionati                              │
├──────────────────────────────────────────────┤
│ [x] Mira il tuo popolo          p. 3         │
│     Nessun avviso                          > │
├──────────────────────────────────────────────┤
│ [x] Alleluia, passeranno i cieli p. 4–5      │
│     ! 2 righe da controllare  · Scansione  > │
├──────────────────────────────────────────────┤
│ [ ] Symbolum 77                 p. 6         │
│     Salvato · Apri                           │
├──────────────────────────────────────────────┤
│                  ...                         │
├──────────────────────────────────────────────┤
│ ┌──────────────────────────────────────────┐ │
│ │        Salva 150 canti selezionati       │ │
│ └──────────────────────────────────────────┘ │
└──────────────────────────────────────────────┘
```

Regole:
- **Riepilogo** sempre in alto, in quattro righe al massimo. Il tocco su ⓘ spiega: "L'indice può contare anche intestazioni di sezione e titoli tradotti." Questo copre lo scarto del libretto 14.
- **Dettagli** apre un foglio con le pagine non lette e il motivo di ciascuna (riconoscimento fallito, pagina ruotata, risoluzione troppo bassa).
- **Riga**: spunta (56 dp di area), titolo, pagine, stato. Gli avvisi usano il segno "!" più la parola, mai solo il colore.
- "Scansione" compare solo per i canti presi da pagine scansionate, che hanno più errori.
- **Solo da controllare** filtra le righe con avvisi.
- Il pulsante in basso dice sempre quanti canti salva. È disattivo con zero selezionati.
- Ordine dell'elenco: quello delle pagine. Nessun ordinamento alternativo in questo round.

### S2 + S3 insieme (tablet orizzontale e Windows, classe larga)

```
┌─────────────────────┬───────────────────────────────────────────────────────────────────┐
│ Importa   Altro PDF │ Alleluia, passeranno i cieli · p. 4–5 · Scansione   < 2 di 151 >  │
│ 48 pag. · 151 canti │ ! 2 righe da controllare: 7, 12                     ⏱ 0:48        │
│ [Tutti] [Nessuno]   ├─────────────────────┬─────────────────────┬───────────────────────┤
├─────────────────────┤ Originale           │ ChordPro            │ Anteprima             │
│[x] Mira il tuo p. 3 │                     │                     │                       │
│[x] Alleluia,  p. 4 ◄│ Do       Sol        │ {title: Alleluia,   │ Do        Sol         │
│    ! 2 · Scansione  │ Alleluia, passeran- │  passeranno i cieli}│ Alleluia, passeranno  │
│[ ] Symbolum   p. 6  │ no i cieli          │ [Do]Alleluia, pas-  │ i cieli               │
│    Salvato · Apri   │ ? Lam  Fa           │ seranno i [Sol]cieli│ Lam    Fa             │
│[x] Resta con  p. 7  │ ...                 │ ...                 │ ...                   │
│  ...                │ (righe con "?" =    │ (editor monospazio) │ (controllo di disegno │
│                     │  da controllare)    │                     │  del canto)           │
├─────────────────────┼─────────────────────┴─────────────────────┴───────────────────────┤
│ Salva 150 selez.    │ [ Ripristina ]                               [ Salva questo canto ]│
└─────────────────────┴───────────────────────────────────────────────────────────────────┘
```

### S3 Revisione (tablet verticale, classe media)

```
┌──────────────────────────────────────────────┐
│ ← Elenco    Alleluia, passeranno…   < 2/151 >│
│ ! 2 righe da controllare: 7, 12     ⏱ 0:48   │
├──────────────────────┬───────────────────────┤
│ [Originale|Anteprima]│ ChordPro              │
│                      │                       │
│ Do       Sol         │ {title: Alleluia, …}  │
│ Alleluia, passeran-  │ [Do]Alleluia, pas-    │
│ no i cieli           │ seranno i [Sol]cieli  │
│ ? Lam  Fa            │ ...                   │
│                      │                       │
├──────────────────────┴───────────────────────┤
│ [ Ripristina ]          [ Salva questo canto ]│
└──────────────────────────────────────────────┘
```

### S3 Revisione (classe stretta)

```
┌──────────────────────────────┐
│ ← Elenco  Alleluia…  < 2/151 >│
│ ! 2 righe: 7, 12      ⏱ 0:48 │
│ [Originale|ChordPro|Anteprima]│
├──────────────────────────────┤
│ (un solo pannello alla volta)│
│                              │
├──────────────────────────────┤
│ [Ripristina] [Salva il canto]│
└──────────────────────────────┘
```

Regole di S3:
- **Originale**: il testo sorgente del canto, riga per riga, in monospazio, sola lettura. Le righe dubbie hanno un "?" a margine e uno sfondo tenue. Serve che il motore esponga i numeri di riga dubbi (vedi §10, D-U3). Senza questo dato si mostra solo il conteggio in testata.
- **ChordPro**: `Editor` multilinea in monospazio, senza correttore ortografico e senza suggerimenti di testo. Il titolo si cambia nella riga `{title: …}`; il titolo salvato viene da lì.
- **Anteprima**: il controllo di disegno del canto, con la notazione e gli accidenti delle impostazioni. Si aggiorna 400 ms dopo l'ultima battitura. Se la conversione fallisce, resta l'ultima anteprima valida con la scritta "Anteprima non aggiornata: controlla la riga n".
- **Avvisi in testata**: elenco breve dei numeri di riga. Il tocco su un numero porta il cursore a quella riga nell'editor, se `CursorPosition` funziona su entrambe le piattaforme. Altrimenti l'elenco resta solo informativo.
- **Ripristina** riporta il ChordPro a quello proposto dal motore, con conferma.
- **< n di m >** passa al canto successivo dell'elenco. Le modifiche restano in memoria finché la sessione è aperta. Il canto modificato e non salvato mostra "Modificato" nell'elenco.
- Un canto già salvato si apre in sola lettura, con **Apri nella libreria**.

### S4 Dichiarazione (foglio modale, tutte le classi)

```
┌──────────────────────────────────────────────┐
│ Salva 150 canti nella libreria               │
├──────────────────────────────────────────────┤
│ I canti restano solo su questo dispositivo.  │
│                                              │
│ [ ] Dichiaro di poter usare questi canti:    │
│     sono di pubblico dominio, li ho scritti  │
│     io, oppure ho il libretto originale o il │
│     permesso di chi ne ha i diritti.         │
│                                              │
│   Annulla                    [ Salva ]       │
│                              (disattivo      │
│                               senza spunta)  │
└──────────────────────────────────────────────┘
```

Regole:
- La casella **non è mai preselezionata** e non si ricorda tra un salvataggio e l'altro (DEC 38, [PC §3.2] riga 23).
- Una dichiarazione per ogni azione di salvataggio: copre un canto oppure tutti i selezionati. Il titolo del foglio dice quanti.
- Area di tocco della casella: tutta la riga, almeno 56 dp. Il testo è l'etichetta della casella per il lettore di schermo.
- Il testo esatto è provvisorio: va rivisto dal legale (D11).
- Durante il salvataggio di molti canti: "Salvataggio 37 di 150", senza annullamento. Se un canto fallisce, gli altri continuano; alla fine si dice quali non sono stati salvati.

### S5 Salvato

```
┌──────────────────────────────────────────────┐
│ ✓ 150 canti salvati nella libreria.          │
│   1 canto non selezionato resta da salvare.  │
│                                    [Chiudi]  │
└──────────────────────────────────────────────┘
  (banner in alto nella pagina, resta finché non si chiude)

  Dopo il salvataggio di un solo canto:
┌──────────────────────────────────────────────┐
│ ✓ "Alleluia, passeranno i cieli" salvato.    │
│                       [Apri]  [Prossimo >]   │
└──────────────────────────────────────────────┘
```

Regole:
- Banner, non un avviso che sparisce da solo: l'utente decide quando chiuderlo.
- **Apri** porta alla schermata del canto con il canto appena salvato. Indietro torna a Importa nello stesso stato.
- **Prossimo** apre in revisione il primo canto non salvato dopo quello corrente.
- Per un PDF con un solo canto, dopo il salvataggio restano **Apri** e **Altro PDF**.

### S6 Errore (modello comune)

```
┌──────────────────────────────────────────────┐
│ Importa                                      │
├──────────────────────────────────────────────┤
│   ! Non riesco a leggere questo PDF          │
│                                              │
│   Il file è protetto da password o           │
│   danneggiato.                               │
│                                              │
│   Prova ad aprirlo e a salvarlo di nuovo     │
│   con un lettore PDF.                        │
│                                              │
│   [ Scegli un altro PDF ]                    │
└──────────────────────────────────────────────┘
```

Ogni errore dice: **cosa** è successo, **perché**, **cosa fare**. Testi proposti:

| Caso | Titolo | Spiegazione | Azione |
|---|---|---|---|
| PDF illeggibile (danneggiato, protetto, oltre i limiti contro i file costruiti ad arte) | Non riesco a leggere questo PDF | Il file è protetto da password, danneggiato o troppo grande. | Scegli un altro PDF |
| Scansione a risoluzione troppo bassa (tutte le pagine) | La scansione è troppo sgranata | Il testo è troppo piccolo per essere riconosciuto. Serve una scansione ad almeno 200 punti per pollice. [IPOTESI: soglia dal motore] | Scegli un altro PDF |
| Nessun canto trovato, ma c'è testo | Non ho trovato canti | Il testo non sembra un canto con accordi. | **Importa tutto come un canto** (porta a S3 con il testo grezzo) · Scegli un altro PDF |
| Nessun testo (pagine vuote o ruotate) | Il PDF non contiene testo leggibile | Le pagine sono vuote, solo immagini non riconosciute, o ruotate. | Scegli un altro PDF |
| OCR non disponibile, PDF solo scansioni | Il riconoscimento del testo non è disponibile | Windows: "Installa la lingua italiana in Impostazioni > Ora e lingua > Lingua." Android: "Il componente di riconoscimento non risponde. Riavvia l'app." | Scegli un altro PDF |
| OCR non disponibile, PDF misto | (non è un errore: si va a S2) | Riepilogo: "! 8 scansioni non lette: riconoscimento non disponibile." | Come sopra, nei Dettagli |
| Errore inatteso | Qualcosa è andato storto | Nessun canto è stato salvato. | Riprova · Scegli un altro PDF |

Gli errori parziali (alcune pagine non lette) non fermano il flusso. Compaiono nel riepilogo di S2 con **Dettagli**.

### S7 Annullato

```
┌──────────────────────────────────────────────┐
│   Lettura annullata. Nessun canto importato. │
│   [ Scegli un PDF ]                          │
└──────────────────────────────────────────────┘
```

Annullare scarta tutto ciò che è stato letto. È più semplice e non lascia elenchi parziali che sembrano completi.

## 6. Regole di interazione per piattaforma

### Comuni
- Un solo pulsante primario per stato: **Scegli un PDF**, **Annulla**, **Salva … selezionati**, **Salva questo canto**, **Salva** nel foglio.
- Durante S1 il resto della pagina non risponde, tranne **Annulla**.
- Uscire da Importa (cambio scheda, indietro) con canti modificati e non salvati chiede conferma: "Hai 3 canti modificati non salvati. Uscire e perderli?" Con la sessione solo letta e nulla modificato, si esce senza chiedere e la sessione resta finché l'app è aperta.
- La sessione di import vive in memoria. Se il sistema chiude l'app, si perde. Non lo si scrive in S0, per non appesantirla: il rischio si accetta fino a F7.

### Android tablet
- Selettore di file di sistema, filtro `application/pdf`.
- Tasto o gesto **Indietro**: in S1 equivale ad Annulla, con conferma; in S3 stretta/media torna all'elenco; in S4 chiude il foglio.
- Tutti i bersagli toccabili: **almeno 56 dp** di altezza, 8 dp di spazio tra uno e l'altro. Vale per spunte, righe, frecce, pulsanti del selettore.
- In verticale la tastiera copre metà schermo durante la modifica: nella classe media la colonna Originale/Anteprima si accorcia ma resta visibile.
- La rotazione non perde lo stato: elenco, canto aperto, testo modificato e timer restano.

### Windows
- Selettore di file nativo, filtro `*.pdf`.
- Finestra ridimensionabile: il layout cambia classe dal vivo (§4) senza perdere lo stato.
- Bersagli: almeno 40 px di altezza; si tengono i 56 dp dove costa uguale (pulsanti primari).
- Ordine del tasto Tab: riepilogo → Tutti/Nessuno → elenco → Originale → ChordPro → Anteprima → pulsanti in basso. Focus sempre visibile.
- Scorciatoie, tramite il gestore dei tasti della finestra già usato per il pedale in F3:

| Tasto | Azione | Dove |
|---|---|---|
| `Ctrl+O` | Scegli un PDF | S0, S2 (chiede conferma se ci sono modifiche) |
| `Esc` | Annulla la lettura / chiudi il foglio | S1, S4 |
| `Ctrl+S` | Salva questo canto (apre S4) | S3 |
| `Alt+←` / `Alt+→` | Canto precedente / successivo | S3 |
| `Spazio` | Spunta o togli la spunta | Riga dell'elenco con il focus |

`Ctrl+A` resta all'editor (seleziona il testo). "Tutti" e "Nessuno" sono pulsanti raggiungibili con Tab. Se il gestore dei tasti non si riusa a basso costo, le scorciatoie passano a F7, tranne `Esc`.

## 7. Accessibilità

- **Bersagli**: 56 dp su tablet (§6). Nessun controllo solo icona: ogni pulsante ha una parola.
- **Contrasto**: testo ≥ 4,5:1, glifi e bordi dei controlli ≥ 3:1, nel dizionario provvisorio chiaro e scuro. Gli avvisi usano "!" o "?" più testo, mai solo il colore.
- **Lettore di schermo** (TalkBack, Narrator): ogni riga dell'elenco si legge come una frase: "Alleluia, passeranno i cieli, pagine 4 e 5, scansione, 2 righe da controllare, selezionato". L'avanzamento di S1 si annuncia ogni 5 pagine e alla fine. Il banner di S5 si annuncia senza spostare il focus.
- **Focus**: all'arrivo di S2 il focus va al riepilogo; all'apertura di S3 al titolo del canto; alla chiusura di S4 torna al pulsante che l'ha aperto; in S6 al titolo dell'errore.
- **Testo grande**: le righe dell'elenco vanno a capo, non si troncano. I titoli lunghi in testata di S3 si troncano con i puntini, il titolo intero resta nel campo `{title:}`.
- **Movimento**: nessuna animazione oltre al cambio di stato standard.
- **Testi**: in italiano, raccolti in un solo file di risorse se il costo è minimo (F6 punto 8 lo chiede comunque).

## 8. Etichette degli avvisi

Il motore restituisce avvisi per canto. Nella testata di S3 e nei Dettagli si mostrano così (testi proposti, brevi):

| Avviso del motore | Testo visibile |
|---|---|
| Nota isolata ambigua | Nota da sola su una riga: accordo o testo? |
| Parole simili ad accordi | Riga di testo con parole simili ad accordi |
| Parola estranea su una riga di accordi | Parola tenuta come nota su una riga di accordi |
| Accordi nell'altra notazione | Accordi in un'altra notazione, tenuti come testo |
| Parentesi tolte | Accordo tra parentesi riscritto senza |
| Accordo spostato | Accordo spostato all'inizio della parola |
| Tabulazioni | Tabulazioni convertite in spazi |
| Ritornello richiamato senza ritornello | Ritornello richiamato ma non scritto prima |
| Tonalità e capotasto insieme | Trovati tonalità e capotasto: controlla la tonalità |
| Metadato illeggibile | Riga di intestazione tenuta come commento |
| Testo che inizia con # | Riga di testo che inizia con # |
| Parentesi quadre o graffe | Parentesi quadre o graffe sostituite |
| Riga di accordi tenuta a parte | Riga di accordi tenuta separata |

L'avviso informativo "accordo dentro una parola" non si mostra, come già fa il motore.

## 9. Misura per il Gate 0, traccia B (facoltativa, costo basso)

Il proprietario deve misurare quanti canti richiedono **≤ 2 minuti di correzione** ([PC §5] attività 5). La schermata può aiutare con un cronometro per canto.

- **Interruttore** in S0: "Misura i tempi di correzione". Spento per impostazione predefinita. Non si ricorda tra le sessioni.
- **Cronometro**: parte quando un canto si apre in S3. Si ferma quando si passa a un altro canto, si torna all'elenco o l'app va in secondo piano. Riparte se il canto si riapre. Si chiude al salvataggio.
- **Visibile** in testata di S3 come "⏱ m:ss", in cifre a larghezza fissa. Nessun allarme a 2 minuti: non deve mettere fretta.
- **Elenco**: con il cronometro attivo, ogni riga salvata mostra il tempo. Un canto salvato senza aprirlo mostra "0:00, non rivisto".
- **Copia i tempi**: un pulsante in S2 copia negli appunti una tabella di testo: titolo, pagine, testo o scansione, minuti, "≤ 2 min: sì/no", piattaforma. Il proprietario la incolla nel report.
- Costo stimato: un cronometro per canto nel ViewModel, un'etichetta, un pulsante che scrive negli appunti. Nessun salvataggio su disco.
- Limite: misura il tempo passato sulla schermata, non il tempo di pensiero fuori. Basta per una stima di Gate 0.

## 10. Dipendenze dal motore (lavoro C1)

La schermata ha bisogno di questi dati. Se mancano, va concordato con chi costruisce il servizio di import.

| # | Dato | Uso |
|---|---|---|
| D-U1 | Avanzamento: fase (testo / scansione), pagina n di m, annullamento | S1 |
| D-U2 | Riepilogo: pagine di testo, pagine scansionate, canti trovati, canti nell'indice, pagine non lette con motivo, motivo di fallimento | S2, S6 |
| D-U3 | Per canto: righe sorgente e **numeri delle righe dubbie** | Il "?" nella colonna Originale. Oggi il canto convertito porta solo il conteggio |
| D-U4 | Per canto: provenienza (pagine di testo o scansione) | Etichetta "Scansione" |
| D-U5 | Motivo distinto per: protetto/danneggiato, oltre i limiti, risoluzione bassa, nessun testo, OCR non disponibile | Testi di S6 |
| D-U6 | Conversione del ChordPro modificato in modello di disegno, senza eccezioni sul testo errato | Anteprima dal vivo |

## 11. Cosa rimandare a F7

- **Immagine della pagina** nella colonna Originale al posto del testo estratto (UX-A §3.4). In questo round basta il testo.
- **Evidenziazione dentro l'editor** delle righe dubbie e salto preciso alla riga, se `CursorPosition` non basta.
- **Metadati** oltre al titolo: autore, tonalità, momenti, stato dei diritti per canto (il dominio ha già lo stato dei diritti). Oggi si salva con stato "Sconosciuto".
- **Doppioni**: segnalare "già in libreria" per i titoli uguali.
- **Trascinare un PDF** sulla finestra di Windows e **"Apri con AppCanzoniere"** dal gestore file di Android.
- **Foto e fotocamera** come fonte.
- **Sessione persistente**: riprendere un import dopo che il sistema ha chiuso l'app.
- **Processo separato** per la lettura su Android (DEC 26, DEC 36).
- **Token e temi** (F4) al posto del dizionario provvisorio; prove visive (F5).
- **Ingresso definitivo** da Canti → + → Da PDF, con la navigazione di F6.
- **Libretto → scaletta** (UX-B §3.4), in F8.
- **Scorciatoie** Windows diverse da `Esc`, se il gestore dei tasti non si riusa subito.
- **Cronometro**: si toglie o si nasconde dopo il Gate 0.

## 12. Decisioni da confermare dal proprietario

| # | Domanda | Raccomandazione |
|---|---|---|
| U-1 | Import del libretto intero in v1? [PC §3.2] riga 18 lo mette in v2 come funzione Pro; il Pro di DEC 10 toglie solo la pubblicità. | Sì in questo round, perché serve alla traccia B del Gate 0. Decidere in F7 se resta per tutti. |
| U-2 | Una dichiarazione per ogni salvataggio, anche di 150 canti insieme, oppure una per canto? | Una per salvataggio, con il numero dei canti nel titolo. Da far vedere al legale (D11). |
| U-3 | Testo della dichiarazione (§5, S4). | Usare quello proposto finché il legale non risponde. |
| U-4 | Nell'elenco, canti selezionati all'inizio: tutti o nessuno? | Tutti, così chi vuole tutto il libretto non tocca 150 caselle. La dichiarazione resta il punto di controllo. |
| U-5 | Annullare la lettura scarta tutto? | Sì: niente elenchi parziali. |
| U-6 | Cronometro per il Gate 0 nella schermata (§9)? | Sì, spento per impostazione predefinita, da togliere dopo il Gate 0. |
| U-7 | La scheda "Importa" resta nella barra attuale fino a F6? | Sì: è temporanea e la navigazione si rifà in F6. |
| U-8 | Canti importati salvati con stato dei diritti "Sconosciuto"? | Sì, fino a F7. Ricordare che con DEC 37 potrebbero perdersi in F6. |
