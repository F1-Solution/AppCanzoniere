# Gate 0 — validazione commerciale e d'uso

Questo documento spiega il Gate 0 al proprietario. Il lavoro si fa a mano, con interviste, osservazioni e conteggi. L'agente non lo esegue al posto tuo. L'agente prepara i materiali e conta i risultati, se glielo chiedi.

Stato: **in corso**, avviato il 18/11/2026 (fonte: `ROADMAP.md`).
Scadenza della decisione scritta: **31/01/2027**.

Fonti:
- `docs/FASI.md`, sezioni 1.2, 3 e 10.
- `docs/business/05-PIANO-COMMERCIALE.md`, sezione 5 (attività e soglie).
- `docs/business/03-PREZZI.md`, sezione 7.1 (copione delle domande).
- `docs/verifiche/PROVE-SU-DISPOSITIVO.md`, prova B1 (libretti).

---

## 1. Cosa è il Gate 0

Il Gate 0 risponde a una domanda: **procedere con il progetto, oppure fermarsi.**

- Il lavoro tecnico (round) procede in parallelo. Il gate non dipende dai round.
- Il gate non ammette codice di prodotto. Le sole prove tecniche sono usa e getta: la prova del leggio e la prova dell'import.
- Costo massimo: 300 €. In più, 100-150 € per un tablet Android economico, se non ne hai uno.
- Tempo stimato: circa 80 ore, quasi tutte tue.

ATTENZIONE: l'esito finale è "procedo" oppure "mi fermo". Le prove di import e di Palco possono ridurre la v1, ma non cambiano l'esito. La regola è nella sezione 6.

---

## 2. Attività e scadenze

| # | Attività | Scadenza | Soglia di esito | Cosa sblocca |
|---|---|---|---|---|
| 1 | Conta le tue ore per 4 settimane | 13/12/2026 | Vedi sezione 6 | Tipo di calendario (principale o lungo) |
| 2 | Censimento dei dispositivi (modulo online) | 10/01/2027 | ≥ 40 risposte; ≥ 60% usa un dispositivo al leggio; ≥ 55% di questi usa Android | Ordine delle piattaforme (F7, F13) |
| 3 | Osserva 5 Messe | 27/12/2026 | Note scritte su 5 celebrazioni | Dati per Modalità Liturgia (F10) |
| 4 | Interviste ad animatori e musicisti | 24/01/2027 | ≥ 15 interviste; ≥ 10 su 15 riconoscono il problema | Scelta di pubblicità e Pro (F11, F12) |
| 5 | Prova dell'import su 10 libretti PDF | 17/01/2027 | ≥ 70% dei canti con ≤ 2 minuti di correzione | Import in v1: OCR, solo PDF con testo, o niente (F7) |
| 6 | Prova del Palco su tablet e pedale | 24/01/2027 | Scorrimento fluido; il pedale gira pagina | Modalità Palco (F3, F7) |
| 7 | Verifica dei concorrenti (SongbookPro, MobileSheets) | 15/12/2026 (installazione); 24/01/2027 (confronto) | Maggioranza sceglie la nostra per la Messa | Messaggi di vendita |
| 8 | Domanda neutra a un editore e all'Ufficio giuridico CEI | Invio entro 15/12/2026 | Nessuna obiezione esplicita | Funzioni sui testi (F14) |
| 9 | Proposta a 3 uffici liturgici diocesani | 31/01/2027 | ≥ 1 ufficio offre uno spazio | Presenza nei corsi (F15) |
| 10 | Lettere d'intento per il Piano Coro | 31/01/2027 | ≥ 5 lettere a ≥ 35 €/anno, oppure interesse scritto di una diocesi | Sola opzione diocesana (F17), non la v1 |
| 11 | Cori pilota | 31/01/2027 | ≥ 5 cori; ≥ 3 con tablet Android; ≥ 12 persone | Anteprima e test chiuso (F14) |
| 12 | Un'ora con il commercialista | 20/01/2027 | Costo fisso annuo previsto ≤ 1.000 €; forfettario compatibile | Partita IVA, acquisti, donazioni (F12, F15) |
| 13 | Un animatore-ambasciatore | 31/01/2027 | Una persona con nome e cognome | Primo livello di assistenza (F14) |
| 14 | Decisione scritta: procedo, o mi fermo | **31/01/2027** | Vedi sezione 6 | Tutto il resto |

Per le attività 1, 3, 4, 5, 6 e 11 le procedure sono nelle sezioni 4 e 5.

---

## 3. Cosa è cambiato rispetto al piano di ottobre

Il copione e le soglie vengono da `05-PIANO-COMMERCIALE.md`, con queste modifiche da `FASI.md` §3:

- Le interviste parlano anche di **pubblicità** e del **Pro a 4,99 €** una tantum.
- Le interviste mostrano le tre modalità (Libreria, Scaletta, Liturgia), non una sola.
- Nessun incasso durante il gate.
- Il censimento chiede anche se l'animatore usa un **PC Windows** per preparare le scalette.
- Il commercialista è obbligatorio, perché la v1 avrà pubblicità e acquisti.
- Le lettere d'intento servono all'opzione diocesana (F17), non alla v1.
- Il ramo "procedo come servizio gratuito" è stato tolto (decisione DEC 10).

ATTENZIONE: due documenti non sono allineati. Vedi la sezione 10 prima di usare il copione.

---

## 4. Procedura A — Preparare le interviste

**Scopo:** Avere un copione breve, con le domande da fare e l'ordine giusto.

**Cosa serve:**
- `docs/business/03-PREZZI.md`, sezione 7.1.
- Le wireframe dell'app: `docs/design/ux/UX-A-libreria.html`, `UX-B-scaletta.html`, `UX-C-liturgia.html`.
- Un foglio per la tabella della sezione 5.

**Tempo stimato:** 2-3 ore, in una sola volta.

**Passi:**

1. Apri `docs/business/03-PREZZI.md`, sezione 7.1.
2. Tieni le domande 1-5. Riguardano il comportamento attuale.
3. Togli le domande 6-9 (curva di Van Westendorp). Con 15 interviste la curva non è leggibile. Vedi la sezione 10, punto 9.
4. Aggiungi una domanda: "Ti interessa un'app con tre modalità: Libreria, Scaletta, Liturgia?"
5. Mostra le tre wireframe. Chiedi quale parte capisci e quale no.
6. Aggiungi una domanda: "Accetteresti un banner nelle schermate di elenco e un annuncio all'apertura dell'app?"
7. Aggiungi una domanda: "Il Pro a 4,99 € una tantum, che toglie la pubblicità, ti interessa?"
8. Tieni le domande 10-12 solo per animatori e parroci. Riguardano il Piano Coro.
9. Non usare la domanda 13 (preordini). Vedi la sezione 10, punto 1.
10. Scrivi il copione finale su una pagina. Non superare 15 domande.

ATTENZIONE: non dire che l'app esiste già. Le wireframe sono disegni. Non sono un prodotto finito.

**Risultato atteso:** Una pagina con le domande in ordine, pronte da leggere.

**Cosa fare se non funziona:** Se una domanda non è chiara, chiedi all'agente di riscriverla. Scrivi nella richiesta: "Riscrivi la domanda 6 del copione Gate 0 con parole semplici."

---

## 5. Procedura B — Condurre le interviste e registrare le risposte

**Scopo:** Avere 15 interviste con le risposte scritte.

**Cosa serve:** Il copione della procedura A, le wireframe sul tablet o sul PC, la tabella qui sotto, un timer.

**Tempo stimato:** 20-30 minuti per intervista. Circa 8 ore in totale.

**Passi:**

1. Scegli persone che suonano o scelgono i canti in un coro, un gruppo giovani o una parrocchia. Cerca almeno 15 persone diverse.
2. Chiedi il permesso di usare le risposte in forma anonima.
3. Chiedi il permesso di registrare. Se non lo concedono, prendi appunti.
4. Assegna a ogni intervistato un codice: I-01, I-02, e così via.
5. Fai le domande 1-5 nell'ordine del copione.
6. Mostra la wireframe della Scaletta (UX-B). Non mostrare altro.
7. Fai le domande rimanenti.
8. Scrivi la riga nella tabella entro 24 ore.
9. Non vendere. Non chiedere soldi. Non chiedere preordini.

ATTENZIONE: non scrivere nomi di persone, nomi di cori o indirizzi nel repository. Usa solo i codici.

**Tabella delle interviste (modello):**

| Codice | Data | Ruolo (animatore, musicista, altro) | Coro (sigla) | Usa un dispositivo al leggio? (sì/no) | Dispositivo (tablet, PC, telefono, altro) | Riconosce il problema? (sì/no) | Pubblicità accettabile? (sì/no/non so) | Pro a 4,99 € interessa? (sì/no) | Note (poche parole) |
|---|---|---|---|---|---|---|---|---|---|
| I-01 | 28/11/2026 | Musicista | Sigla A | sì | Tablet Android | sì | no | sì | Chiede il Do-Re-Mi |
| I-02 | | | | | | | | | |

La riga I-01 è un esempio. Non contarla. Cancellala prima di iniziare.

**Risultato atteso:** 15 righe compilate, con un sì o un no per ogni colonna di risposta.

**Cosa fare se non funziona:**
- Se una persona rifiuta, scrivi "rifiutata" nella riga. Scegli un'altra persona.
- Se una risposta non è chiara, scrivi la frase esatta tra virgolette nella colonna Note.

---

## 6. Procedura C — Contare i risultati e applicare le soglie

**Scopo:** Decidere, con i numeri, se una soglia è superata.

**Cosa serve:** Le tabelle compilate e questa tabella.

**Tempo stimato:** 30 minuti per attività.

**Formula:** Percentuale = numero di sì diviso per numero totale, poi per 100.

**Esempio:** 11 interviste su 15 riconoscono il problema. 11 / 15 = 73%. La soglia è "≥ 10 su 15". Superata.

**Soglie per attività:**

| Attività | Soglia | Esito |
|---|---|---|
| 1. Ore | ≥ 10 h a settimana: calendario principale. 8-10 h: calendario lungo. 6-8 h: v1 ridotta, senza OCR. < 6 h: fermarsi | Vedi regola di esito |
| 2. Censimento | ≥ 40 risposte; ≥ 60% usa un dispositivo al leggio; ≥ 55% di questi usa Android. Più del 40% usa un iPad: rivedere l'ordine delle piattaforme | Vedi sotto |
| 4. Interviste | ≥ 15 interviste; ≥ 10 su 15 riconoscono il problema | Superata se entrambe sono vere |
| 5. Import | ≥ 70%: OCR nella v1. Tra 50% e 70%: v1 senza OCR delle scansioni. < 50%: import fuori dalla v1 | Vedi procedura D |
| 10. Lettere d'intento | ≥ 5 lettere a ≥ 35 €/anno, oppure interesse scritto di una diocesi | Solo per F17 |
| 11. Cori pilota | ≥ 5 cori; ≥ 3 con tablet Android; ≥ 12 persone | Superata se tutte sono vere |
| 12. Commercialista | Costo fisso annuo previsto ≤ 1.000 €; forfettario compatibile | Vedi sotto |

**Regola di esito (attività 14):**

- **Fermati** se una di queste è vera: ore < 6 h a settimana; meno del 60% usa un dispositivo al leggio; meno di 3 cori pilota; la prova del Palco fallisce e non c'è alternativa.
- **Procedi** se le soglie sono superate.
- Se l'import è tra 50% e 70%, la v1 va senza OCR delle scansioni. L'esito resta "procedo".

ATTENZIONE: il piano di ottobre prevedeva un terzo esito, "servizio gratuito". Questo esito è stato tolto (DEC 10). Il documento `05-PIANO-COMMERCIALE.md` lo riporta ancora. Vedi la sezione 10, punto 5.

**Risultato atteso:** Per ogni attività, una riga con il numero, la percentuale e "superata" o "non superata".

**Cosa fare se non funziona:** Se un numero non torna, ricontrolla la tabella. Se resta un dubbio, riporta la domanda all'agente. Non arrotondare.

---

## 7. Procedura D — Attività 5: contare l'import dei libretti

**Scopo:** Misurare quanto tempo serve per correggere un canto importato da un libretto.

**Cosa serve:** I 10 libretti PDF, il PC di sviluppo con il report, un timer, due tabelle vuote (una per i PDF con testo, una per le scansioni).

**Tempo stimato:** 6-8 ore.

**Passi:** Segui la prova B1 in `docs/verifiche/PROVE-SU-DISPOSITIVO.md`, sezione "Prova dell'import (traccia B)". I passi sono quelli. Qui sotto trovi solo i punti da ricordare.

1. Scegli 5 canti per ogni libretto. In totale sono 50 canti.
2. Per ogni canto, avvia il timer, correggi fino a renderlo uguale al PDF, ferma il timer.
3. Scrivi i minuti. Scrivi "sì" se sono 2 minuti o meno. Scrivi "no" se sono più di 2.
4. Scrivi i PDF con testo e le scansioni in due tabelle separate.
5. Calcola due percentuali: una per i PDF con testo e una per le scansioni.

ATTENZIONE: il piano (`05-PIANO-COMMERCIALE.md`, attività 5) conta tutti i canti insieme. La prova B1 chiede due percentuali separate. Scrivi le due percentuali e la percentuale totale. Vedi la sezione 10, punto 3.

**Risultato atteso:** Due percentuali, una per tipo di PDF. Una riga per canto.

**Cosa fare se non funziona:** Se un libretto non si apre, annotalo e scegli un altro libretto della stessa fonte. Scrivi il motivo nella tabella.

---

## 8. Procedura E — Cosa riportare all'agente e quando

**Scopo:** Dare all'agente i dati nel momento giusto, così può aggiornare i documenti.

**Quando:**

| Data | Cosa riportare |
|---|---|
| 13/12/2026 | Attività 1 (ore) |
| 15/12/2026 | Attività 7 (installazione dei concorrenti) e attività 8 (invio delle domande agli editori) |
| 27/12/2026 | Attività 3 (note sulle 5 Messe) |
| 10/01/2027 | Attività 2 (censimento) |
| 17/01/2027 | Attività 5 (import, entrambe le percentuali) |
| 20/01/2027 | Attività 12 (esito del commercialista) |
| 24/01/2027 | Attività 4 (interviste), attività 6 (prova del Palco), attività 7 (confronto) |
| 31/01/2027 | Attività 9, 10, 11, 13 e 14 (decisione scritta) |

**Come riportare:** Scrivi una riga per attività, con i valori e l'esito. Usa questo modello:

`Gate 0, attività 4: 15 interviste; 11 su 15 riconoscono il problema (73%); pubblicità accettabile: 9 sì; Pro interessa: 6 sì. Esito: superata.`

Quando riporti, scrivi anche la data della decisione. Il Gate 0 si chiude con la decisione scritta (attività 14).

**Risultato atteso:** L'agente aggiorna `docs/FASI.md` e `ROADMAP.md` con l'esito. Il round successivo parte con i dati giusti.

**Cosa fare se non funziona:** Se l'agente non trova la riga, riporta i dati nella richiesta con la frase "Esito Gate 0 attività N: ...".

---

## 9. Cosa sblocca il Gate 0

| Esito | Cosa parte o si ferma |
|---|---|
| Attività 5 ≥ 70% | OCR delle scansioni nella v1 (F7). Il traguardo T1 resta al 21/03/2027. |
| Attività 5 tra 50% e 70% | v1 senza OCR. Il T1 si fa con PDF con testo e "incolla". Ripiego: 18/04/2027. |
| Attività 5 < 50% | Import fuori dalla v1. Il messaggio di vendita va riscritto. |
| Attività 11 superata | Anteprima ai cori (F14) dal 02/05/2027. |
| Attività 11 non superata (meno di 3 cori pilota) | Fermarsi. È una regola di esito. |
| Attività 12 superata | Il commercialista conferma la forma fiscale (D13). F12 e F15 possono partire. |
| Attività 14 "procedo" | Round tecnici fino a F15 secondo `docs/FASI.md` §10. |
| Attività 14 "mi fermo" | Il lavoro si chiude dopo i round in corso (circa 16). Resta un repository pulito. |

Il Gate 0 non sblocca da solo F14 o F7. Ogni fase ha anche altre condizioni. Vedi `docs/handoff/08-COMPITI-DELLE-FASI-FUTURE.md`.

---

## 10. Punti da chiarire prima di usare il copione

Questi punti sono contraddizioni nei documenti. Decidi tu. Poi riporta la decisione all'agente.

1. **Preordini.** `05-PIANO-COMMERCIALE.md` (attività 4) dice "senza preordini". `03-PREZZI.md` (domanda 13) prevede preordini rimborsabili a 6,99 €, con obiettivo 20 preordini. Questo documento suggerisce di non usare la domanda 13.
2. **Prezzo del Pro.** `03-PREZZI.md` usa 6,99 € nella domanda 13 e nelle curve di prezzo. La decisione DEC 10 dice 4,99 €. Questo documento usa 4,99 €.
3. **Soglia della tabella dei libretti.** `05-PIANO-COMMERCIALE.md` conta tutti i canti insieme. `PROVE-SU-DISPOSITIVO.md` (B1) chiede due percentuali separate.
4. **Riferimento al problema.** `05-PIANO-COMMERCIALE.md` attività 4 cita "il problema del §2.5". Il problema è descritto in §2.1. Il §2.5 è il comunicato di lancio. Questo documento usa il testo di §2.1: la scaletta arriva su WhatsApp, e il PDF del libretto non si legge e non si traspone.
5. **Esito "servizio gratuito".** Tolto da `FASI.md` §3, ma ancora presente in `05-PIANO-COMMERCIALE.md` (§5, regola di esito).
6. **Data di avvio.** `ROADMAP.md` e il piano del round 7 dicono 18/11/2026. Il piano commerciale dice 16/11/2026. Questo documento usa 18/11/2026.
7. **Attività 2, censimento.** `FASI.md` §3 aggiunge una domanda sul PC Windows. [DA VERIFICARE: il modulo online contiene già questa domanda].
8. **Requisito Android.** Dal round 8, l'app richiede Android 6.0 o superiore (`V-07-mlkit.md`). Nel censimento chiedi la versione di Android. Chi ha un dispositivo più vecchio non può usare l'app.
9. **Curva dei prezzi.** `03-PREZZI.md` prevede la curva di Van Westendorp (domande 6-9). Il piano del Gate 0 chiede solo se il Pro a 4,99 € interessa. Questo documento toglie la curva, perché con 15 interviste non è leggibile. Se la vuoi, usa la domanda con il prezzo a 4,99 €.

---

## 11. Checklist prima della scadenza finale

- [ ] Tablet Android acquistato, con Android 6.0 o superiore (100-150 €).
- [ ] Pedale Bluetooth acquistato e accoppiato (30-60 €), in modalità tastiera.
- [ ] PC Windows disponibile per le prove.
- [ ] 10 libretti PDF reali raccolti.
- [ ] Copione finale scritto, con i punti della sezione 10 decisi.
- [ ] Tabelle pronte, fuori dal repository.
- [ ] 15 interviste fatte e registrate.
- [ ] Commercialista consultato (attività 12).
- [ ] Decisione scritta (attività 14) riportata all'agente.
