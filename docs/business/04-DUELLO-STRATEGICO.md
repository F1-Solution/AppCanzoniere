# 04 — Duello strategico: strategia e prezzi messi alla prova da avversari

Data: 10 novembre 2026. Autore: Strategy Duel Agent (agente).
Documenti messi alla prova: `docs/business/02-STRATEGIA.md` **[STR §x]** e `docs/business/03-PREZZI.md` **[PRE §x]**.
Basi di fatto: `docs/business/01-RICERCA-MERCATO.md` **[RIC §x]** (fonti [Sn]), `docs/VALUTAZIONE.md` **[VAL §x]**, `docs/PROMPT-INIZIALE.md` **[PROMPT]**.

**Come è stato condotto.** Non c'era un interlocutore umano: ho giocato io sia gli avversari sia la difesa. In ogni duello l'avversario fa la mossa più dannosa *plausibile*, non la più spettacolare. La difesa risponde solo con ciò che è già scritto in 02 e 03; poi valuto se la risposta basta. Non ho fatto nuove ricerche: uso i dati dei documenti e segnalo ogni dato nuovo che servirebbe.

**Come leggere il documento**
- **[DOC]**: fatto preso dai documenti citati.
- **[IPOTESI]**: mio ragionamento o calcolo, con le premesse scritte.
- **[DA VERIFICARE]**: punto che va controllato prima di decidere.
- **Esito** di ogni duello: **REGGE** (la strategia attuale risponde già), **REGGE CON MODIFICHE** (risponde se si cambia qualcosa), **CEDE** (la risposta attuale non basta e il punto va riscritto).
- **Danno**: da 0 (nessun effetto) a 5 (il punto colpito non sta più in piedi).

---

## 0. Sintesi in una pagina

| # | Duello | Esito | Danno |
|---|---|---|---|
| 1 | SongbookPro/MobileSheets imparano l'italiano, il Do-Re-Mi e la Messa | REGGE CON MODIFICHE | 3 |
| 2 | Un concorrente fa lo stesso gratis | REGGE CON MODIFICHE (il posizionamento regge, la quota di 30 canti no) | 3 |
| 3 | Un editore contesta testi condivisi e importati | REGGE CON MODIFICHE (l'app locale regge, il Piano Coro com'è progettato no) | 3 |
| 4 | Il pubblico non paga | **CEDE** | 5 |
| 5 | Il proprietario come punto debole | **CEDE** (tempi e capacità) | 4 |
| 6 | Alternative radicali | **CEDE** contro "gratis + servizi" nella fase di lancio; batte l'anglofono; contro "non commercializzare" dipende dall'obiettivo | 3 |
| 7 | Coerenza interna dei documenti | **CEDE** | 4 |

**Punteggio finale: nessun duello retto così com'è, 3 retti con modifiche, 4 ceduti.**

**Cosa regge**: il posizionamento ("il leggio della Messa" per la liturgia italiana), la rinuncia al catalogo pubblico, l'idea che paghi il gruppo e non il singolo, l'esclusione di audio e AI sugli accordi.
**Cosa non regge**: far pagare subito il singolo con una quota di 30 canti; un Piano Coro che tiene i testi sul nostro server; un prodotto minimo in 3-5 mesi; un verdetto "si ripaga i costi" che non conta commercialista e imposte.

**Le 5 modifiche più importanti** (elenco completo nel §4):
1. **Gate 0 allargato e quasi gratuito, prima di scrivere codice**: censimento dei dispositivi (compresi gli iPad), prova tecnica su 10 libretti PDF veri, verifica di cosa fa già SongbookPro, un contatto con un editore, un'ora con un commercialista, lettere d'intento firmate dai parroci.
2. **Lancio gratuito, senza quota e senza paywall**, finché l'uso la domenica non è dimostrato. Nessuna partita IVA per incassare poche centinaia di euro. Pedali sempre gratis.
3. **Piano Coro ridisegnato**: il server sincronizza solo la scaletta (titoli, ordine, tonalità, note), mai i testi. Si vende prima alla parrocchia e alla diocesi con fattura, non al singolo in app.
4. **Ambito e calendario realistici**: solo Android il primo anno; prodotto minimo stimato in 6-9 mesi; lancio pubblico a settembre 2027, alla ripresa dell'anno pastorale; Gate 1 in Avvento 2027.
5. **Piano economico onesto**: commercialista e imposte in tutti gli scenari. Valore atteso ≈ +0,9 K€ in 36 mesi, cioè 2-3 € l'ora: il progetto si giustifica come servizio alla comunità con un'entrata accessoria, non come reddito.

**Verdetto**: **PROCEDERE, MA NON CON QUESTO PIANO.** Prima la domenica, poi i soldi: app gratuita che conquista l'uso, monetizzazione solo verso parrocchie e diocesi quando l'uso c'è. Se l'obiettivo del proprietario è il reddito, la risposta onesta è no (§8).

---

## 1. Il campo di gioco

```
═══════════════════════════════════════════════════════════════
  DUELLO STRATEGICO INIZIALIZZATO
═══════════════════════════════════════════════════════════════
  Tipo di gioco : ripetuto (ogni domenica), informazione incompleta,
                  molti giocatori con interessi diversi
  Difesa        : strategia 02 + prezzi 03 (sviluppatore singolo)
  Avversari     : concorrenti, editori, pubblico, il proprietario
                  stesso, le alternative radicali, la coerenza interna
  Duelli        : 7
═══════════════════════════════════════════════════════════════
```

**Le regole del campo** (valgono per tutti i duelli) [DOC]:
- Il costo di una copia in più dell'app è zero, per noi e per qualsiasi concorrente.
- Le alternative gratuite sono ovunque: PDF diocesani, siti, app di movimento [RIC §1.2].
- Il budget non sta nel singolo volontario ma in parrocchie, diocesi e movimenti [STR §2].
- I diritti sul repertorio corrente sono degli editori; non esiste in Italia una licenza collettiva per riprodurre i testi in un'app [RIC §4.1].
- Il codice funziona davvero per il 25-30% dei requisiti originali [VAL §1].

**I concetti di teoria dei giochi usati, in una riga ciascuno**
| Concetto | Cosa vuol dire qui |
|---|---|
| Strategia dominante | La mossa migliore per un giocatore qualunque cosa facciano gli altri |
| Equilibrio di Nash | Situazione in cui nessuno ha interesse a cambiare mossa da solo: è dove il gioco tende a fermarsi |
| Minaccia credibile | Una minaccia conta solo se chi la fa ha davvero interesse a eseguirla |
| Concorrenza sul prezzo a costo zero ("alla Bertrand") | Se due prodotti sono uguali e copiarli non costa nulla, il prezzo scende a zero; si incassa solo con qualcosa che gli altri non hanno |
| Free rider (chi viaggia gratis) | Uno paga e molti godono del beneficio senza pagare |
| Dispositivo di impegno | Una regola che ci si impone prima, per non cambiare idea quando conviene nel breve |
| Valore d'opzione | Rimandare una decisione costosa ha un valore se intanto si impara qualcosa |

Gli stratagemmi cinesi sono citati per nome e numero solo quando danno un'immagine utile della mossa; il ragionamento non dipende da loro.

---

## 2. I duelli

### Duello 1 — Il generalista impara l'italiano

```
───────────────────────────────────────────────────────────────
  DUELLO 1/7 · SongbookPro (o MobileSheets) entra nella nicchia
  Concetto    : deterrenza all'ingresso, strategia dominante
  Stratagemma : #9 Osservare l'incendio dall'altra riva (avversario)
───────────────────────────────────────────────────────────────
```

**Mossa dell'avversario.** SongbookPro (692K installazioni, 4,63★, circa 7 $ una tantum, Android + Windows + iOS [RIC §1.1]) pubblica l'interfaccia in italiano, la notazione Do-Re-Mi e un modello di scaletta con le sezioni della Messa. Costo per loro: qualche settimana, oppure quasi zero se la traduzione la fa la comunità.

**Risposta della strategia attuale.** La strategia stessa dice che questa mossa la sconfigge: è la condizione 3 del verdetto, "SongbookPro aggiunge italiano + Do-Re-Mi + scalette liturgiche → non procedere" [STR §6.2]. Il vantaggio residuo sarebbe "liturgia + import" [STR §1.2].

**Analisi.**
- **Probabilità bassa, ma l'avversario non ha bisogno di muovere.** Per SongbookPro la nicchia liturgica italiana vale poche decine di migliaia di persone [RIC §2.2]: la sua strategia dominante è ignorarla. Il problema è un altro: un chitarrista esperto può *già oggi* usare SongbookPro creando una scaletta chiamata "Messa 15 novembre" con i canti in ordine. La concorrenza vera è "abbastanza buono adesso", non una mossa futura.
- **Un'affermazione chiave non è mai stata controllata.** La strategia dice che SongbookPro non ha la notazione Do-Re-Mi "[IPOTESI: non verificata nell'app]" [STR §1.2]. Se l'avesse già, metà della differenza dichiarata non esiste oggi. **[DA VERIFICARE: 30 minuti, installando la versione gratuita di SongbookPro e la prova di MobileSheets.]**
- **Cosa non si copia da un ufficio nel Regno Unito.** L'import dei libretti PDF italiani (impaginazioni con accordi sopra le righe, due colonne, numerazione per raccolta), l'indice liturgico italiano, le relazioni con uffici liturgici e cori. Nessuna di queste tre cose esiste oggi: il fossato dichiarato è ancora da costruire [VAL §3].
- **Effetto sul prezzo.** Il Pro a 9,99 € sta sopra SongbookPro (circa 7 $, versione gratuita con 12 canti) [PRE §1.1], che è più maturo. Con SongbookPro in italiano, 9,99 € per uno strumento meno maturo è difficile da difendere davanti al singolo.
- **La condizione 3 è scritta male.** Questa mossa colpirebbe la vendita del Pro al singolo, non il coordinamento dei cori né gli accordi con diocesi ed editori. Non è un motivo per fermare tutto.

**Esito: REGGE CON MODIFICHE. Danno 3.**

**Modifiche proposte.**
- Verificare subito cosa fanno oggi SongbookPro e MobileSheets (notazione, lingua, scalette, import PDF) e riscrivere le differenze in base a quello che si vede, non a quello che si suppone.
- Riscrivere la condizione 3: "se SongbookPro copre il caso d'uso, si rinuncia alla vendita al singolo e si resta su cori, parrocchie e diocesi".
- Concentrare la differenza su ciò che un generalista non ha interesse a fare: import dei libretti italiani, indice liturgico, scaletta condivisa per la Messa, rapporto con le diocesi.
- Import ed export ChordPro compatibili con SongbookPro e OnSong: chi arriva da lì porta la propria libreria in pochi minuti.
- Prezzo del Pro, quando ci sarà, allineato a SongbookPro: 7,99 € (§4, modifica 9).

---

### Duello 2 — Lo stesso servizio, gratis

```
───────────────────────────────────────────────────────────────
  DUELLO 2/7 · Concorrente gratuito
  Concetto    : concorrenza sul prezzo a costo zero (Bertrand)
  Stratagemma : #30 Scambiare il ruolo di ospite con quello di padrone (difesa)
───────────────────────────────────────────────────────────────
```

**Mossa dell'avversario.** Quattro varianti, in ordine di pericolosità [IPOTESI]:
1. **Un'app diocesana o di un editore con il repertorio ufficiale licenziato**, gratuita per i cori. Ha il contenuto che noi non possiamo avere e occupa il canale su cui contiamo (gli uffici liturgici).
2. **Canta e Cammina** (gratuita, scalette con QR, canti personali, oggi solo iOS [RIC §1.2]) esce su Android.
3. **NuoviConvertiti** (gratuito, trasposizione, "modalità live", playlist [RIC §1.2]) fa un'app o una pagina installabile con funzionamento offline.
4. **Un progetto open source** fatto in poche settimane con l'AI: la strategia stessa valuta "alto" il rischio di nuovi entranti [STR §1.4].

**Risposta della strategia attuale.** Tutto ciò che serve a suonare è gratuito; si paga il volume (oltre 30 canti propri), il tempo risparmiato e il coordinamento del gruppo; niente pubblicità [PRE §2.1].

**Analisi.**
- Con un costo per copia nullo, due prodotti simili si fanno concorrenza fino al prezzo zero. Si incassa solo con qualcosa che il concorrente gratuito non ha, oppure da qualcuno che ha un budget e un motivo per pagare.
- La quota di 30 canti diventa la differenza più visibile: "l'altra è gratis e senza limiti". Un animatore con 80-200 canti di repertorio [PRE §2.2] la raggiunge nel primo mese.
- La variante 1 è la più grave e la strategia la cita solo di passaggio [STR §1.5]. La risposta giusta non è combatterla ma offrirsi come motore: "il vostro repertorio, nella nostra app".
- Le varianti 3 e 4 si battono con l'esecuzione (offline, Messa, pedale, import), non con il prezzo.

**Esito: REGGE CON MODIFICHE. Danno 3.** Il posizionamento regge; il Pro con quota no.

**Modifiche proposte.**
- Nessuna quota sui canti al lancio (§4, modifica 2).
- Offrire per primi alle diocesi e ai movimenti una "edizione" con il loro repertorio o indice (opzione C della strategia [STR §3.2]), anticipandola: chi entra come ospite (fornitore dello strumento) diventa padrone del canale.
- Scrivere nella scheda dello store e nell'app "suonare sarà sempre gratis": è una promessa che difende sia dal concorrente gratuito sia dalle recensioni negative.

---

### Duello 3 — L'editore contesta

```
───────────────────────────────────────────────────────────────
  DUELLO 3/7 · Editore di musica liturgica o titolare di diritti
  Concetto    : minaccia credibile; chi agisce per primo
  Stratagemmi : #3 Uccidere con un coltello preso in prestito (avversario)
                #21 La cicala dorata muta la pelle (difesa)
───────────────────────────────────────────────────────────────
```

**Mossa dell'avversario.** Un editore (Paoline, San Paolo, Elledici, RnS [RIC §4.1]) non fa causa: **segnala l'app a Google Play per violazione di copyright** (è lo "coltello preso in prestito"), allegando:
- le schermate dello store con i testi di un suo canto;
- la descrizione del Pro: "import dell'intero libretto", "stampa ed export PDF del canzoniere impaginato" [PRE §2.2];
- il Piano Coro, che conserva sul nostro server i testi e li distribuisce a 25 persone dietro pagamento [PRE §4.2].

Google sospende l'app finché la questione non è chiarita. Uno sviluppatore singolo non ha strumenti per sostenere una contestazione lunga [IPOTESI].

**Risposta della strategia attuale.** Niente catalogo pubblico; gruppi chiusi con un massimo di membri; parere legale prima del Piano Coro; procedura di segnalazione e rimozione; editori trattati da partner; proposta agli editori "appena ci sono utenti da mostrare" [STR §4.4, §6.4].

**Analisi.**
- **Sull'app locale la difesa regge.** Se i testi stanno solo sul dispositivo dell'utente e passano da un dispositivo all'altro con QR o file, lo sviluppatore non ospita né distribuisce nulla. Per l'editore agire costa più di quanto renda: la minaccia non è credibile.
- **Tre punti la rendono credibile, e li abbiamo scelti noi:**
  1. il Pro **fa pagare proprio le funzioni che sembrano riproduzione** ("libretto intero", "canzoniere impaginato"); il prezzo del Piano Coro è giustificato anche con le "fotocopie evitate" [PRE §4.2];
  2. il Piano Coro mette i testi protetti **sul nostro server**, a pagamento: è la configurazione più esposta (DSA art. 16; possibile art. 17 della Direttiva DSM [RIC §4.2]);
  3. la pulizia dell'import con AI in cloud [PRE §5] manda gli stessi testi a un nostro intermediario e a un fornitore AI.
- **Ordine delle mosse sbagliato.** La strategia propone di contattare gli editori dopo il lancio. A quel punto l'editore ci incontra come problema. Prima del lancio lo incontra come proposta, e una telefonata costa zero.

**Esito: REGGE CON MODIFICHE. Danno 3.** L'app locale regge; il Piano Coro così com'è progettato cede.

**Modifiche proposte** (la "cicala che muta la pelle": il prodotto lascia il guscio dei testi e tiene il corpo dello strumento):
- **Il server del Piano Coro non conserva mai i testi.** Sincronizza solo la scaletta: titoli, ordine, momenti, tonalità, capotasto, note. I testi arrivano a ogni membro una volta sola, da dispositivo a dispositivo, oppure da una cartella nel cloud dell'animatore. Effetti: rischio legale molto più basso, hosting più piccolo, parere legale più breve.
- **Niente AI in cloud sui testi** nella prima e nella seconda fase; solo regole locali.
- **Scheda dello store e video solo con canti di dominio pubblico o propri.**
- **Riformulare le funzioni**: "stampa della scaletta" sì; "canzoniere impaginato" solo per canti propri o di dominio pubblico, e mai nel messaggio di vendita.
- **Contattare un editore e l'Ufficio giuridico CEI durante il Gate 0**, con una domanda e non con un prodotto: "esiste una licenza per i testi in un'app? a quali condizioni?".
- **Parere legale mirato** (1-2 ore su 4 domande precise) invece di 2.000 € in blocco: con i testi fuori dal server le domande si riducono.

---

### Duello 4 — Il pubblico non paga

```
───────────────────────────────────────────────────────────────
  DUELLO 4/7 · Il pubblico non paga
  Concetto    : free rider dentro il coro; sostituto gratuito
  Stratagemma : #16 Lasciare libero per catturare (difesa proposta)
───────────────────────────────────────────────────────────────
```

**Mossa dell'avversario** (il pubblico, senza alcuna malizia):
1. **La quota si aggira da sola.** I canti "ricevuti" non contano [PRE §2.2]. Basta esportare i propri canti in un file e reimportarli, o farseli mandare da un amico: diventano "ricevuti". Senza un server non c'è modo di distinguere [IPOTESI].
2. **Un solo Pro per coro.** Per design l'animatore con il Pro manda 200 canti a tutti i membri, che non pagano mai. Il ricavo di un intero coro è un Pro: 9,99 € una volta.
3. **Recensioni:** "app a pagamento mascherata" alla prima richiesta di sblocco. L'unica app liturgica italiana a pagamento ha 3,49★ [RIC §1.2].
4. **Il Piano Coro non raggiunge i preordini.** L'animatore oggi manda la scaletta nel gruppo WhatsApp: gratis, immediato, già lo usano tutti. Con la condivisione gratuita in app tramite QR o file, quello che resta da vendere è "si aggiorna da sola".

**Risposta della strategia attuale.** Quota morbida (si blocca solo l'aggiunta, mai la lettura); Gate 2 alzato a ≥25 cori con preordine; se il Piano Coro non parte, si resta sul solo Pro [PRE §2.2, §6.3, §8].

**Analisi con i numeri dei documenti.**
- **Il ripiego non è in attivo.** Solo Pro, con commercialista: servono circa 97 vendite l'anno per pareggiare [PRE §6.4]. Lo scenario base ne prevede 80 nel primo anno; il prudente 16-20. Il cumulato a 36 mesi dopo fisco e commercialista è −1,7 K€ (prudente) e +0,5 K€ (base) [PRE §6.4].
- **Il Piano Coro fa concorrenza a sé stesso.** Pro una tantum per l'animatore + condivisione gratuita coprono gran parte del suo valore. La differenza rispetto a WhatsApp + QR deve essere dimostrata, non supposta. Eppure vale il 70-81% dei ricavi del terzo anno [PRE §6.2].
- **Quanto vale la quota.** Secondo i prezzi, senza quota si perdono circa 1,1 K€ in 36 mesi nello scenario base [PRE §8], cioè circa 30 € al mese. È poco rispetto a ciò che costa: recensioni, attrito con le diocesi (vedi duello 7, punto 8) e una regola che si aggira comunque.
- **Il gioco dentro il coro.** Se il beneficio è di tutti e paga uno solo, nessuno vuole essere quell'uno. Ha senso far pagare solo chi ha un budget e un motivo istituzionale: la parrocchia (fattura, uniformità) o la diocesi (formazione, repertorio comune).

**Esito: CEDE. Danno 5.** È il duello che sposta di più il piano.

**Modifiche proposte.**
- **"Lasciare libero per catturare":** al lancio nessuna quota e nessun paywall. Si cattura l'uso domenicale; si fa pagare dopo, e solo per funzioni *nuove*, mai togliendo qualcosa che era gratuito (l'errore di OnSong [RIC §3.1]).
- **Il Piano Coro deve vendere ciò che WhatsApp non fa:** scaletta che cambia in diretta su tutti i dispositivi anche durante la Messa; tonalità e capotasto per strumentista; storico "cosa abbiamo cantato" della parrocchia; fattura intestata. Se nelle interviste questi quattro punti non convincono, il Piano Coro non si costruisce.
- **Al posto dei preordini, lettere d'intento** firmate dal parroco o dal responsabile, con cifra e data. Si raccolgono senza incassare nulla e quindi senza un inquadramento fiscale. I preordini veri solo dopo il parere del commercialista.
- **Il ripiego, se il Gate 2 fallisce, non è "solo Pro con partita IVA"**: è restare gratuiti (vedi duello 6).

---

### Duello 5 — Il proprietario come punto debole

```
───────────────────────────────────────────────────────────────
  DUELLO 5/7 · Tempo, motivazione, canali, assistenza, codice
  Concetto    : dispositivi di impegno; vincolo di capacità
  Stratagemmi : #4 Attendere riposati il nemico stanco
                #36 Se tutto fallisce, ritirarsi (regola scritta prima)
───────────────────────────────────────────────────────────────
```

**Mossa dell'avversario** (il proprietario contro sé stesso):
1. **Tempo**: lavora nel tempo libero; l'Avvento comincia il 29 novembre 2026, poi Natale, poi Quaresima dal 10 febbraio 2027.
2. **Codice**: quasi tutto il prodotto minimo è ancora da fare (tabella sotto).
3. **Velocità apparente**: la fase precedente, fatta con forte aiuto di agenti AI, ha prodotto documenti che dichiaravano "87%, pronto per la produzione" a fronte di un 25-30% reale [VAL §1, §7]. Il rischio si ripete se "finito" vuol dire "i test passano" invece di "funziona sul tablet la domenica".
4. **Canali**: i canali previsti (corsi diocesani, incontri dei cori, Settimana Liturgica [STR §5.3]) chiedono presenza fisica, spesso nel fine settimana.
5. **Assistenza**: le richieste arrivano la domenica tra le 8 e le 11, quando il proprietario stesso potrebbe essere a Messa. Con 6,96 € netti a vendita, 14 minuti di assistenza azzerano il margine [PRE §3.2].
6. **Motivazione**: la strategia ha tolto quasi tutto ciò che il proprietario voleva costruire (database pubblico, accordi via AI, riconoscimento audio, import dai siti di accordi, sincronizzazione con più cloud, tablature [PROMPT; STR §0]). Il prodotto raccomandato è diverso da quello immaginato. Il ricavo del primo anno sarebbe tra 0,1 e 1,5 K€ [PRE §6.2].

**Il prodotto minimo contro lo stato del codice** [DOC: VAL §3, §4]

| Funzione del prodotto minimo [STR §5.1] | Stato oggi |
|---|---|
| Libreria locale (creare, aprire, salvare un canto) | Parziale: persistenza e migrazioni da rifare; aprire e salvare un canto probabilmente non funzionano |
| Accordi sopra il testo | Assente: si vede il ChordPro grezzo |
| Trasposizione | **Funziona** (logica testata); interfaccia parziale |
| Do-Re-Mi, diesis/bemolle | Assente: l'impostazione si salva ma nessuno la usa |
| Scaletta "Messa" per momenti | Parziale: album manuali e momenti liturgici esistono, la scaletta per momenti no |
| Modalità Palco, pedale | Assente |
| Import da PDF testuale | Assente |
| OCR sul dispositivo | Finto (servizio simulato) |
| Condivisione con QR o file | Assente |
| Backup ed export | Parziale (copia del database, con rischio di file bloccati); export assente |
| Interfaccia in italiano | Assente: testi in inglese scritti nel codice, nessun file di traduzione |
| Sblocco tramite store | Assente |
| Canti di dominio pubblico verificati | Assente (lavoro di ricerca, non di codice) |
| Build Android affidabile, CI | Non verificata in locale; CI mai verde |

Su quattordici voci: una funziona, tre sono parziali, nove sono assenti o finte, una non è verificata.

**Risposta della strategia attuale.** Ambito minimo, niente backend nella prima versione, gate con date, "una funzione nuova solo se la chiedono almeno 3 cori", prodotto minimo in 3-5 mesi part-time [STR §5.1, §6.4].

**Analisi.**
- 3-5 mesi non è credibile per quella tabella. La stima [IPOTESI] è **6-9 mesi a 8-10 ore a settimana (circa 250-350 ore)**, con il rischio maggiore sull'import dei libretti, che è insieme la funzione simbolo e quella tecnicamente più difficile.
- Il vincolo vero non è lo sviluppo ma tutto il resto: interviste, diocesi, assistenza, store, fisco. Nessun documento assegna ore a queste attività.
- La regola dei 3 cori protegge dall'ampliamento, ma niente protegge dall'abbandono a metà.

**Esito: CEDE. Danno 4.**

**Modifiche proposte.**
- **Un primo traguardo verificabile con data fissa** (fine febbraio 2027): un canto vero, preso da un libretto PDF vero, importato, trasposto in Do-Re-Mi e suonato in Modalità Palco su un tablet Android economico con un pedale. Se a quella data non c'è, si ridimensiona o ci si ferma (#36, scritto ora e non deciso allora).
- **Definizione di "finito"**: provato su un dispositivo reale e usato in una Messa vera, non "i test passano".
- **Ore dichiarate e misurate**: tenere il conto delle ore per 4 settimane durante il Gate 0. Sotto le 6 ore a settimana di media, togliere altro ambito.
- **Un animatore-ambasciatore**: una persona del mondo liturgico che porta l'app nei corsi e fa la prima assistenza. Non costa nulla e copre i punti 4 e 5. È un ruolo da trovare nel Gate 0.
- **Regole di servizio**: nessun aggiornamento da venerdì a domenica; FAQ e tre video brevi prima del lancio.
- **Decisione esplicita del proprietario**: accettare che il prodotto sia diverso da quello del prompt iniziale, oppure fermarsi adesso. Andare avanti senza averlo deciso è la via più probabile verso l'abbandono.

---

### Duello 6 — Le alternative radicali

```
───────────────────────────────────────────────────────────────
  DUELLO 6/7 · Tre piani alternativi contro la strategia attuale
  Concetto    : valore d'opzione; costo fisso di entrata
  Stratagemma : #17 Lanciare un mattone per ottenere giada (6b)
───────────────────────────────────────────────────────────────
```

**6a — Puntare subito al mercato anglofono.**
- *A favore*: un mercato da 10 a 20 volte più grande [RIC §2.2]; abitudine a pagare (OnSong, Planning Center).
- *Contro*: lì il posto è occupato da OnSong, SongbookPro, Planning Center con l'integrazione CCLI/SongSelect [RIC §3.4]; nel mondo cattolico statunitense ci sono già ecosistemi di editori e licenze (OneLicense [RIC §4.1]). Il proprietario non ha né relazioni né conoscenza del dominio là, e l'interfaccia in inglese oggi nel codice non è un vantaggio: è l'impostazione di tutti.
- **Batte la strategia attuale? No.** Va tenuta come opzione a costo quasi nullo dopo: scheda dello store e interfaccia in inglese quando l'app sarà tradotta.

**6b — App gratuita (eventualmente open source), ricavi solo da servizi a parrocchie e diocesi.**

| Criterio | Strategia attuale (Pro subito, poi Coro) | 6b (gratis subito, poi parrocchie e diocesi) |
|---|---|---|
| Ricavo nei primi 12 mesi | 0,1-1,5 K€ lordi di store [PRE §6.2] | 0 |
| Costi fissi nei primi 12 mesi | Partita IVA, commercialista (400-800 €) prima della prima vendita [PRE §6.4] | Nessuno finché non arriva un incasso |
| Netto nei primi 12 mesi dopo fisco | Da circa −0,5 K€ (prudente) a +0,6 K€ (favorevole) [IPOTESI su PRE §6.4] | 0 |
| Canale diocesano | Frenato: una diocesi preferisce promuovere uno strumento gratuito [STR §2] | Aperto |
| Recensioni e passaparola | Rischio "a pagamento mascherata" | Nessuna frizione |
| Rischio legale | Funzioni a pagamento che sembrano riproduzione (duello 3) | Più basso: nessun incasso sui testi |
| Tempo del proprietario | Paywall, licenze, fisco, assistenza a chi ha pagato | Solo prodotto e canali |
| Dal secondo anno | Coro + Pro | Uguale, se l'uso lo giustifica: licenza diocesana, Piano Coro, strumenti Pro nuovi |

- Una sola licenza diocesana nel listino attuale (25 € × 20 cori = 500 €/anno [PRE §4.5]) vale quanto circa 72 vendite del Pro (500 ÷ 6,96 €), cioè quasi tutto il primo anno dello scenario base. Il mattone lanciato (l'app gratuita) serve a ottenere la giada (la diocesi).
- Open source: utile per la fiducia di diocesi e movimenti e per non dipendere da una persona sola; non cambia i ricavi, perché copiare l'app costa già poco. È una scelta del proprietario, non una condizione. Se lo si fa, licenza GPL e marchio tenuto.
- **Batte la strategia attuale? Sì, nella fase di lancio (12-18 mesi)**: stesso ricavo atteso a meno di qualche centinaio di euro, meno costi fissi, canale aperto, rischio più basso, e conserva l'opzione di monetizzare dopo. Nel lungo periodo non la batte da sola: serve la seconda fase a pagamento. Il piano raccomandato è quindi un ibrido: **6b subito, Piano Coro e licenza diocesana dopo.**

**6c — Non commercializzare affatto** (strumento per la propria parrocchia, eventualmente pubblicato gratis).
- Valore atteso della strategia attuale a 36 mesi, con le probabilità della strategia (45% / 40% / 15% [STR §3.4]) e i risultati dopo fisco dei prezzi (−1,7 / +0,5 / +10 K€ [PRE §6.4]): **0,45 × −1,7 + 0,40 × 0,5 + 0,15 × 10 ≈ +0,9 K€** [IPOTESI]. Su 300-500 ore di lavoro sono **circa 2-3 € l'ora**, e il risultato dipende quasi tutto dallo scenario favorevole al 15%.
- 6c rinuncia a quei 0,9 K€ attesi ma risparmia forse un terzo delle ore (fisco, store a pagamento, assistenza commerciale, vendita) e azzera i rischi legali e fiscali.
- **Batte la strategia attuale? Sì, se l'obiettivo è il reddito o l'hobby**: il reddito non è raggiungibile in modo realistico, e per un hobby i costi commerciali sono inutili. **No, se l'obiettivo è il servizio ai cori con la possibilità di farlo crescere**: in quel caso vince 6b, che tiene aperta l'opzione.

**Esito: CEDE** contro 6b nella fase di lancio; **REGGE** contro 6a; contro 6c dipende dall'obiettivo del proprietario. **Danno 3.**

**Osservazione**: la strategia attuale non è la migliore per nessuno dei tre obiettivi possibili (reddito, servizio, hobby). È la migliore solo per "verificare subito se il singolo paga", ed è proprio l'ipotesi che i dati già indicano come debole [RIC §3.5].

---

### Duello 7 — Coerenza interna

```
───────────────────────────────────────────────────────────────
  DUELLO 7/7 · I documenti contro sé stessi
  Concetto    : un equilibrio regge solo se le regole sono coerenti
  Stratagemma : #25 Sostituire le travi con legno marcio (le travi
                marce sono già dentro la casa)
───────────────────────────────────────────────────────────────
```

**Mossa dell'avversario.** Un revisore scettico legge i quattro documenti uno accanto all'altro.

| # | Dove | Contraddizione | Gravità | Correzione |
|---|---|---|---|---|
| 1 | STR §0, §6.1 contro PRE §6.4 | La strategia dice "micro-prodotto che si ripaga i costi". Dopo commercialista e imposte lo scenario prudente (probabilità 45%) chiude a −1,7 K€ in 36 mesi | Alta | Riscrivere il verdetto con i numeri dopo il fisco |
| 2 | STR §6.1 contro PRE §6.4 | "Reddito accessorio": il valore atteso è circa +0,9 K€ in 36 mesi, 2-3 € l'ora | Alta | Dichiararlo nel verdetto; decidere l'obiettivo (STR §7, decisione 1) prima di tutto |
| 3 | PRE §2.2 contro VAL §3 | Le cinque funzioni che giustificano il Pro (import in blocco, pedali, modelli di scaletta, stampa ed export PDF, backup automatico) **non esistono nel codice**. Anche quasi tutte le funzioni gratuite promesse sono assenti | Alta | Non promettere e non dare un prezzo a funzioni che non esistono; il prezzo si decide quando si vede il prodotto |
| 4 | STR §5.1 contro VAL §3 | Prodotto minimo in 3-5 mesi contro 1 funzione su 14 funzionante | Alta | 6-9 mesi; traguardo intermedio a febbraio 2027 (duello 5) |
| 5 | STR §5.5 contro PRE §8 | Gate 2: "≥10 cori lo chiedono" contro "≥25 cori con preordine". Le due soglie convivono senza una decisione | Media | Una sola soglia: ≥25 lettere d'intento o una licenza diocesana, **e** parere legale favorevole |
| 6 | PRE §2.1 contro PRE §2.2 | Regola "mai a pagamento nulla che blocchi un canto durante la Messa", ma il pedale è nel Pro. Se la verifica dell'acquisto fallisce senza rete o su un dispositivo nuovo, il pedale smette di funzionare durante la Messa | Media | Pedale sempre gratuito |
| 7 | PRE §2.2 contro PRE §4 | Condivisione gratuita illimitata e canti ricevuti esclusi dalla quota tolgono valore al Piano Coro, che dovrebbe essere il 70-81% dei ricavi | Alta | Ridisegnare il valore del Piano Coro (duello 4) |
| 8 | STR §2 e §5.3 contro PRE §0 | Il canale principale sono le diocesi, che secondo la strategia "più probabilmente promuovono uno strumento gratuito"; i prezzi mettono una quota a pagamento dal primo giorno | Alta | Lancio gratuito (duello 6) |
| 9 | STR §5.4-5.5 contro il calendario | Con un lancio in primavera, il Gate 1 ("3 mesi dopo") cade in estate, quando molti cori si fermano: la misura "3 domeniche su 4" fallirebbe per stagione, non per il prodotto | Media | Lancio a settembre 2027, Gate 1 in Avvento 2027 |
| 10 | STR §5.5 contro il calendario | Il Gate 0 "in 4-6 settimane" cade tra Avvento e Natale, il periodo più pieno per gli animatori | Bassa | Chiudere il Gate 0 entro il 31 gennaio 2027; usare l'Avvento per osservare Messe vere |
| 11 | STR §5.2 | "Android, dove sono i volontari" non è verificato, e il leggio è soprattutto un uso da tablet, dove l'iPad è molto diffuso [IPOTESI] | Alta | Censire i dispositivi nel Gate 0: se nei cori pilota più del 40% usa un iPad, va rivisto l'ordine delle piattaforme |
| 12 | STR §5.2 contro VAL §2.3 | Windows al secondo posto "perché compila già": compila solo in una copia ridotta del progetto; il costo vero sono pacchetto, firma, secondo store, test e assistenza | Media | Windows dopo il Gate 1 e solo se richiesto |
| 13 | STR §4.5 contro PRE §0 | Il parere legale è previsto "prima del Piano Coro", ma già la prima versione vende import in blocco e stampa del canzoniere (duello 3) | Media | Un parere breve e mirato prima del lancio |
| 14 | PRE §7.1 | Preordini nel Gate 0, ma incassare prima di avere un inquadramento fiscale è il problema segnalato dagli stessi prezzi | Media | Lettere d'intento al posto dei preordini |
| 15 | PRE §5 contro STR §4.4 | Pulizia AI in cloud nel Piano Coro: manda testi protetti a un intermediario e a un fornitore esterno, contro la linea "il rischio principale sono i testi" | Media | Nessuna AI in cloud sui testi |
| 16 | PRE §3.3 | L'analisi dice che il prezzo non è la leva e conta il volume di installazioni, ma nessun documento ha un piano con ore e numeri per arrivare a 2.000-3.500 installazioni | Media | Piano dei canali con ore assegnate e un ambasciatore (duello 5) |
| 17 | PRE §0, §3.1 | Prezzo di lancio a 6,99 € per 90 giorni: introduce l'obbligo di indicare il prezzo più basso dei 30 giorni precedenti, già segnalato come dubbio [PRE §9] | Bassa | Prezzo fisso senza promozioni a tempo |

**Esito: CEDE. Danno 4.** Le contraddizioni 1, 2, 3, 7 e 8 da sole cambiano il verdetto.

---

## 3. Tabella riassuntiva degli esiti

| # | Avversario | Mossa più dannosa | Risposta attuale | Esito | Danno | Modifica principale |
|---|---|---|---|---|---|---|
| 1 | SongbookPro / MobileSheets | Italiano + Do-Re-Mi + modello Messa | "Non procedere" (la strategia si dà per sconfitta) | REGGE CON MODIFICHE | 3 | Verificare oggi cosa fanno; differenza su import italiano, indice, diocesi; la condizione 3 ferma solo la vendita al singolo |
| 2 | App diocesana, Canta e Cammina su Android, NuoviConvertiti, open source | Stesso servizio a zero | Suonare gratis, quota sul volume | REGGE CON MODIFICHE | 3 | Niente quota; offrirsi come motore delle diocesi |
| 3 | Editore | Segnalazione a Google Play | Niente catalogo, parere prima del Piano Coro | REGGE CON MODIFICHE | 3 | Testi mai sul server; niente AI in cloud sui testi; contatto con l'editore prima del lancio |
| 4 | Il pubblico | Aggira la quota, un Pro per coro, WhatsApp al posto del Piano Coro | Quota morbida, Gate 2 a 25 preordini | **CEDE** | 5 | Lancio gratuito; Piano Coro venduto a parrocchie e diocesi con un valore che WhatsApp non dà |
| 5 | Il proprietario | Tempo, assistenza, canali, motivazione, codice da rifare | Ambito minimo, 3-5 mesi | **CEDE** | 4 | 6-9 mesi; traguardo a febbraio 2027; ambasciatore; decisione esplicita sull'obiettivo |
| 6 | Alternative radicali | Gratis + servizi; anglofono; non commercializzare | — | **CEDE** (a 6b nella fase di lancio) | 3 | Adottare 6b come prima fase |
| 7 | Coerenza interna | 17 contraddizioni | — | **CEDE** | 4 | Riscrivere verdetto, gate, funzioni Pro e calendario |

---

## 4. Modifiche da apportare, in ordine di priorità

Ogni voce indica se tocca la strategia **[STR]**, i prezzi **[PRE]** o entrambi, e il duello da cui nasce.

1. **[STR] Gate 0 allargato, entro il 31 gennaio 2027, prima di scrivere codice oltre le prove tecniche** (duelli 1, 3, 4, 5, 7). Oltre a interviste e cori pilota: censimento dei dispositivi (marca, telefono o tablet, pedale); prova tecnica dell'import su 10 libretti PDF reali; verifica di SongbookPro e MobileSheets; un contatto con un editore e con l'Ufficio giuridico CEI; un'ora con un commercialista; lettere d'intento dei parroci; misura delle ore disponibili.
2. **[PRE] Lancio gratuito senza quota né paywall** (duelli 2, 4, 6). Suonare, scalette, condivisione, import di un canto alla volta, backup manuale: tutto gratuito e per sempre, scritto nella scheda dello store. Il costo stimato è circa 1,1 K€ in 36 mesi [PRE §8], in cambio del canale diocesano.
3. **[STR+PRE] Piano Coro ridisegnato** (duelli 3, 4). Il server sincronizza solo la scaletta; i testi non passano mai dal nostro server; nessuna AI in cloud sui testi. Valore da vendere: scaletta in diretta su tutti i dispositivi, tonalità per strumentista, storico della parrocchia, fattura. Prezzo confermato a 49 €/anno per coro e 99 €/anno per parrocchia, ma **venduto prima con fattura alla parrocchia o alla diocesi** che in app.
4. **[STR] Licenza diocesana come prima offerta a pagamento** (duelli 2, 6). 25 €/coro/anno, minimo 20 cori, già nel listino [PRE §4.5]: una diocesi vale quanto circa 72 vendite del Pro. Da proporre alle diocesi in cui l'app è già usata.
5. **[STR] Ambito e calendario** (duelli 5, 7). Solo Android (telefono e tablet) nel primo anno; Windows dopo il Gate 1 e solo se richiesto; l'iPad viene prima di Windows se il censimento lo indica. Calendario nel §6.
6. **[STR] Riscrivere il verdetto e gli scenari dopo commercialista e imposte** (duello 7, punti 1-2). Valore atteso ≈ +0,9 K€ in 36 mesi; scrivere chiaramente che non è un reddito.
7. **[STR+PRE] Gate unici e coerenti** (duello 7, punti 5, 9, 14). Gate 1 in Avvento 2027: ≥300 installazioni, ≥40% degli installati attivi in almeno 3 domeniche su 4, ≥10 cori che usano la scaletta condivisa, voto ≥4,3. Gate 2: ≥25 lettere d'intento oppure una diocesi, **e** parere legale favorevole.
8. **[PRE] Pedale sempre gratuito** (duello 7, punto 6).
9. **[PRE] "Strumenti Pro" solo nella seconda fase, a 7,99 € fissi, senza prezzo di lancio** (duelli 1, 7). Comprendono solo funzioni nuove che fanno risparmiare tempo (import in blocco, modelli di scaletta, stampa delle scalette, backup automatico), mai un limite sul numero di canti. 7,99 € allinea il prezzo a SongbookPro; secondo l'analisi di sensibilità dei prezzi la differenza con 9,99 € è di circa il 7% nel caso peggiore [PRE §3.3]. Si introducono solo quando esistono e funzionano.
10. **[STR] Nessuna partita IVA finché non c'è un incasso che la giustifica** (duelli 4, 6). Soglia indicativa: almeno 2 K€ l'anno impegnati (lettere d'intento o un contratto). Prima di ogni incasso, parere del commercialista, anche sulla compatibilità con l'eventuale lavoro dipendente del proprietario (il regime forfettario ha una soglia di reddito da lavoro dipendente [PRE §4.4]).
11. **[STR] Editori: contatto prima del lancio, schermate con soli canti di dominio pubblico o propri, nessun messaggio di vendita sulla riproduzione dei libretti** (duello 3).
12. **[STR] Parere legale mirato** (1-2 ore su 4 domande) prima del lancio, al posto di 2.000 € in blocco prima del Piano Coro (duelli 3, 7). Le domande: import privato di un libretto; condivisione da dispositivo a dispositivo; scaletta senza testi sul server; indice con titoli e incipit.
13. **[STR] Organizzazione del proprietario** (duello 5). Traguardo verificabile a fine febbraio 2027; ore misurate; un animatore-ambasciatore; nessun aggiornamento da venerdì a domenica; definizione di "finito" = provato in una Messa vera.
14. **[STR] Riscrivere la condizione 3 del verdetto** (duello 1): un SongbookPro in italiano ferma la vendita al singolo, non il progetto.
15. **[STR] Compatibilità ChordPro con SongbookPro e OnSong** in import ed export (duello 1).

---

## 5. Le ipotesi più fragili e come verificarle al minor costo

Ordinate da quella che, se falsa, fa più danni.

| # | Ipotesi | Perché è fragile | Verifica più economica | Costo | Tempo | Soglia per proseguire |
|---|---|---|---|---|---|---|
| 1 | Una parrocchia o una diocesi paga ogni anno per coordinare il coro | Nessun dato; WhatsApp + PDF sono gratis e già in uso [RIC §3.5] | Mostrare a 10 animatori e 5 parroci un prototipo cliccabile (o un video di 2 minuti) della scaletta in diretta; chiedere una lettera d'intento con cifra | 0 € | 4-6 settimane | ≥5 lettere d'intento a ≥35 €/anno, oppure una diocesi interessata per iscritto |
| 2 | I musicisti usano già un dispositivo al leggio, e su Android | Non verificata [STR §6.3]; il leggio è soprattutto un uso da tablet, dove l'iPad è forte | Modulo online di 5 domande diffuso da 2-3 uffici liturgici e gruppi di animatori; osservare 5 Messe in Avvento | 0 € | 3 settimane | ≥60% usa un dispositivo; ≥55% di quelli usa Android |
| 3 | L'import dai libretti PDF italiani funziona abbastanza bene | È la funzione simbolo e oggi non esiste [VAL §3]; i libretti hanno impaginazioni varie | Prova tecnica di 2-3 giorni su 10 PDF reali delle fonti [S42] | 0 € | 1 settimana | ≥70% dei canti importati con ≤2 minuti di correzione ciascuno |
| 4 | Il proprietario ha 8-10 ore a settimana per 9 mesi, più il tempo per canali e assistenza | Nessun documento conta le ore oltre lo sviluppo | Contare le ore per 4 settimane durante il Gate 0 | 0 € | 4 settimane | Media ≥8 ore/settimana; altrimenti tagliare ambito |
| 5 | SongbookPro non copre già il caso d'uso (Do-Re-Mi, italiano) | Mai verificato [STR §1.2] | Installare SongbookPro e la prova di MobileSheets; far provare a 5 musicisti l'una e il nostro prototipo | 0 € | 1 giorno + 1 settimana | La maggioranza sceglie il nostro per la Messa |
| 6 | Gli editori tollerano un'app di sola libreria privata | Nessun contatto finora [STR §4] | Email o telefonata a un editore e all'Ufficio giuridico CEI con una domanda neutra | 0 € | 2-4 settimane | Nessuna obiezione esplicita; meglio ancora un interesse per un accordo |
| 7 | Le diocesi promuovono l'app nei corsi per animatori | Solo una deduzione [STR §5.3] | Proposta di una pagina a 3 uffici liturgici con il video del prototipo | 0 € | 4-8 settimane | ≥1 ufficio offre uno spazio in un corso o una newsletter |
| 8 | Il proprietario può incassare con costi fissi bassi | Dipende dalla sua situazione fiscale [PRE §4.4] | Un'ora con un commercialista | 100-200 € [IPOTESI] | 1 settimana | Costo fisso annuo ≤ ricavo atteso del primo anno di vendite |
| 9 | MAUI su Android regge la Modalità Palco con il pedale | Nessuna prova su dispositivo [VAL §8] | Prova tecnica: canto di 2 pagine con accordi sopra il testo, scorrimento e pedale su un tablet economico | 0 € (o ≈ 30-60 € per un pedale usato) | 3 giorni | Fluido, e il pedale gira pagina senza configurazioni strane |

**Costo totale delle verifiche: meno di 300 €** e circa 8 settimane, quasi tutte in parallelo. Ogni ipotesi qui sopra, se falsa, costerebbe mesi di sviluppo.

---

## 6. Calendario rivisto

| Quando | Cosa | Dispositivo di impegno |
|---|---|---|
| 10 nov 2026 – 31 gen 2027 | Gate 0 allargato (§5). In Avvento e a Natale: osservare Messe vere. Prove tecniche 3 e 9 | Decisione scritta "procedo / mi fermo / resto hobby" entro il 31 gennaio |
| Feb 2027 | Traguardo verificabile (un canto dal PDF al leggio con pedale) entro il 28 febbraio | Se manca, ridimensionare o fermarsi |
| Mar – giu 2027 | Prodotto minimo; uso nella propria parrocchia da Pasqua (28 marzo 2027) tramite la distribuzione di prova di Google Play | Nessuna nuova funzione senza 3 cori che la chiedono |
| Lug – ago 2027 | Rifinitura, FAQ, video, canti di dominio pubblico, parere legale mirato | Nessun lancio d'estate |
| Set – ott 2027 | Lancio pubblico Android gratuito alla ripresa dell'anno pastorale; presentazioni in 1-2 diocesi | — |
| Dic 2027 (Avvento) | **Gate 1** (soglie nel §4, modifica 7) | Se fallisce due volte, l'app resta gratuita in manutenzione |
| Gen – giu 2028 | Lettere d'intento per il Piano Coro; proposta di licenza diocesana; eventuali "Strumenti Pro" a 7,99 € | Partita IVA solo sopra la soglia della modifica 10 |
| Set 2028 | **Gate 2**; eventuale avvio del Piano Coro (solo scalette sul server) alla ripresa dell'anno pastorale | — |

Conseguenza sui numeri dei prezzi: il primo incasso significativo si sposta all'autunno 2028. Le proiezioni a 36 mesi di 03-PREZZI vanno lette come "36 mesi dal lancio di settembre 2027", con il primo anno a ricavo zero.

---

## 7. Controllo dell'equilibrio (Nash)

Dove tende a fermarsi il gioco, se ogni giocatore fa la cosa migliore per sé?

| Giocatore | Miglior risposta al piano attuale | Miglior risposta al piano rivisto |
|---|---|---|
| Musicista singolo | Usa la versione gratuita, aggira la quota o passa a un'alternativa gratuita | Usa l'app gratis; nessun motivo per andarsene |
| Animatore | Compra un Pro per tutto il coro, oppure resta su WhatsApp | Usa la scaletta condivisa; chiede alla parrocchia di pagare se il valore c'è |
| Parrocchia / diocesi | Esita a promuovere un'app con paywall | Promuove uno strumento gratuito; paga per coordinamento e uniformità se i cori lo usano |
| Editore | Tollera la libreria locale; ha motivo di colpire il Piano Coro con i testi sul server | Tollera; può diventare partner |
| SongbookPro | Ignora la nicchia | Ignora la nicchia |
| Proprietario | Rischia di abbandonare: molte ore, pochi euro, prodotto diverso da quello voluto | Traguardi brevi e verificabili, senza obblighi fiscali prima dei ricavi |

**Equilibrio del piano attuale**: il singolo non paga, il coro paga al massimo un Pro, la diocesi non spinge, l'editore ha un bersaglio. È un equilibrio stabile, ma a ricavo quasi nullo e con un rischio legale.
**Equilibrio del piano rivisto**: app gratuita usata, diocesi che promuovono, istituzioni che pagano il coordinamento, editori che tollerano. È stabile **solo se** il coordinamento vale per una parrocchia più di WhatsApp + PDF. È l'ipotesi 1 del §5: tutto il piano poggia lì, e si verifica con 0 € entro gennaio.

---

## 8. Verdetto finale

```
═══════════════════════════════════════════════════════════════
  VERDETTO DELL'ARBITRO
═══════════════════════════════════════════════════════════════
  Esito       : PROCEDERE, MA NON CON QUESTO PIANO
  Punteggio   : retti 0 · retti con modifiche 3 · ceduti 4
  Nash        : il piano attuale converge su "uso gratis, ricavo
                quasi nullo"; quello rivisto è stabile solo se il
                coordinamento vale più di WhatsApp
  Consiglio   : prima la domenica, poi i soldi
═══════════════════════════════════════════════════════════════
```

1. **Il posizionamento regge** e va confermato: liturgia italiana, scaletta della Messa, Do-Re-Mi, offline, import dei libretti, nessun catalogo pubblico, nessuna AI sugli accordi protetti, nessun riconoscimento audio.
2. **Il modello di monetizzazione iniziale non regge.** Far pagare subito il singolo con una quota di 30 canti rende circa 30 € al mese in più nello scenario base, costa partita IVA e commercialista prima del primo euro, frena le diocesi, si aggira da solo e attira recensioni negative.
3. **L'unico modello di business plausibile è far pagare le istituzioni** (parrocchie, diocesi, eventualmente un editore o un movimento) **per il coordinamento**, con un server che non tocca mai i testi. Ma non è dimostrato: va verificato a costo zero entro gennaio 2027.
4. **I tempi non reggono.** Il prodotto minimo è da costruire quasi da zero: 6-9 mesi, lancio a settembre 2027, primi incassi non prima dell'autunno 2028.
5. **I numeri vanno detti per quello che sono.** Anche con tutte le modifiche il valore atteso a 36 mesi resta dell'ordine di 1 K€, cioè 2-3 € l'ora. **Se l'obiettivo del proprietario è il reddito, la risposta onesta è non commercializzare** (6c) o fermarsi. Se l'obiettivo è il servizio ai cori con la possibilità di farlo crescere, la strada è 6b: gratis subito, istituzioni dopo.

**Cosa deve decidere il proprietario, in quest'ordine:**
1. L'obiettivo: reddito, servizio o hobby (STR §7, decisione 1). Senza questa risposta nessun altro punto è decidibile.
2. Se accetta un prodotto diverso da quello del prompt iniziale.
3. Se adottare il piano rivisto (§4) e il calendario (§6), a partire dal Gate 0 allargato entro il 31 gennaio 2027.
