# 05 — Piano commerciale unificato

Data: 10 novembre 2026. Autore: Product Manager (agente).
Unisce e risolve: `docs/PROMPT-INIZIALE.md` **[PROMPT]**, `docs/VALUTAZIONE.md` **[VAL §x]**, `docs/business/01-RICERCA-MERCATO.md` **[RIC §x]**, `02-STRATEGIA.md` **[STR §x]**, `03-PREZZI.md` **[PRE §x]**, `04-DUELLO-STRATEGICO.md` **[DUE §x]**, `docs/design/ux/CONFRONTO-UX.md` **[UX]**, `docs/design/ui/CONFRONTO-UI.md` **[UI]**.

**Come leggere il documento**
- **[FATTO]**: preso da uno dei documenti citati o da una fonte nuova elencata in fondo ([Nn]).
- **[IPOTESI]**: calcolo o stima di questo documento, con le premesse scritte.
- **[DA VERIFICARE]**: va confermato da legale, commercialista o Play Console prima di agire.
- **Questo documento non decide al posto del proprietario.** Raccomanda e isola le decisioni (§11). Le parti marcate "raccomandazione" valgono solo dopo la sua approvazione.

---

## 1. Sintesi per chi decide

**Verdetto: fare subito il Gate 0, non lo sviluppo.** Come attività per guadagnare, il progetto non regge. Il valore atteso è di circa **+1,0 K€ in quasi quattro anni** (da oggi ad agosto 2030), al netto di imposte, commercialista e legale. È circa **0,75 € per ora di lavoro**. Lo scenario più probabile (45%) chiude a **−1,1 K€**. Il progetto si giustifica come **servizio ai cori con un'opzione commerciale verso parrocchie e diocesi**. Quell'opzione si compra a basso costo: meno di 300 € e circa 80 ore entro il 31 gennaio 2027.

**Raccomandazione, in sei punti**
1. **Prodotto**: "il leggio della Messa". Libreria privata in ChordPro, accordi sopra il testo, trasposizione con Do-Re-Mi e capotasto, scalette con il modello Messa, Modalità Palco con pedale, import del libretto PDF della parrocchia. Tutto funziona offline, su Android. Niente catalogo pubblico, niente AI sugli accordi, niente riconoscimento audio, niente scraping (§3).
2. **Prezzo al lancio: zero.** Nessuna quota sui canti, nessun paywall, nessuna pubblicità, pedale gratuito. "Suonare sarà sempre gratis" va scritto nella scheda dello store (§4).
3. **Chi paga e quando**: le istituzioni, per coordinare i cori. La prima offerta a pagamento è la **licenza diocesana** (25 €/coro/anno, minimo 20 cori), venduta insieme al **Piano Coro** (49 €/anno per coro, 99 €/anno per parrocchia). Il Piano Coro sincronizza **solo la scaletta**: i testi non passano mai dal nostro server. Si parte con fattura, non prima di ottobre 2028, e solo se al Gate 2 ci sono almeno **2 K€/anno impegnati per iscritto**. Gli "Strumenti Pro" a 7,99 € arrivano dopo, come funzioni nuove e mai come limite (§4).
4. **Calendario**: Gate 0 il 31/01/2027 → traguardo tecnico il 28/02/2027 → Gate Pilota il 30/06/2027 → lancio Android gratuito entro il 19/09/2027 → Gate 1 il 09/01/2028 → Gate 2 il 31/05/2028 → Piano Coro dal 01/10/2028 → Gate 3 il 30/09/2029 (§6).
5. **Nessuna partita IVA e nessuna spesa ricorrente** prima del Gate 2. Prima del lancio servono solo un'ora di commercialista e un parere legale mirato (circa 0,8 K€ in tutto) (§8).
6. **Se l'obiettivo è il reddito, la risposta onesta è non commercializzare** e scegliere lo scenario del §12.

**Decisioni richieste ora** (elenco completo e ordinato nel §11)

| # | Decisione | Entro | Raccomandazione |
|---|---|---|---|
| D1 | Obiettivo del progetto: reddito, servizio con opzione commerciale, oppure hobby/open source | 16/11/2026 (provvisoria), 31/01/2027 (definitiva) | Servizio con opzione commerciale |
| D2 | Accettare un prodotto diverso dal prompt iniziale (tolti catalogo pubblico, AI accordi, audio, scraping, sync multi-cloud) | 16/11/2026 | Sì |
| D3 | Avviare il Gate 0 con budget ≤ 300 € e ore misurate | 16/11/2026 (l'Avvento inizia il 29/11) | Sì |
| D4 | Direzione UX per i test con i cori | 30/11/2026 | B "Scaletta" + modello Messa |
| D5 | Modello di ricavo: gratis al lancio, poi istituzioni | 31/01/2027 | Sì |

---

## 2. Problema, utente, proposta di valore

### 2.1 Il problema
Ogni domenica, in circa 25.600 parrocchie, qualcuno suona da 6 a 8 canti da accordi: chitarra o tastiera, spesso un volontario [RIC §2.1, STR §1.1]. La scaletta si decide in settimana e arriva su WhatsApp. Gli accordi stanno nel libretto parrocchiale stampato o in un PDF che sul telefono non si legge, e che non si traspone. Se il celebrante cambia un canto all'ultimo momento, si sfoglia un raccoglitore. **Il concorrente da battere è il PDF o il libretto, non un'altra app** [STR §1.1].

### 2.2 Chi lo usa, chi decide, chi paga
| Ruolo | Chi è | Cosa gli serve | Paga? |
|---|---|---|---|
| **Utente** | Musicista volontario (chitarra, tastiera) di coro parrocchiale, gruppo giovani, scout o oratorio | Leggere e trasporre senza fogli, offline, con le mani occupate | **No**, e non deve mai pagare per suonare [PRE §2.1, DUE §2.4] |
| **Decisore** | Animatore musicale o responsabile del coro | Preparare la scaletta della domenica e farla arrivare a tutti con tonalità e capotasto giusti | Raramente di tasca sua |
| **Pagante** | Parrocchia o unità pastorale; diocesi o ufficio liturgico; in prospettiva un movimento o un editore | Uniformità del repertorio, coordinamento, fattura intestata, formazione degli animatori | **Sì, se il coordinamento vale più di WhatsApp + PDF.** È l'ipotesi da cui dipende tutto il piano [DUE §7] |

Mercato raggiungibile in Italia: 30-100 mila musicisti liturgici, valore centrale 50 mila [RIC §2.2, STIMA]. Ottenibile da uno sviluppatore singolo in 3 anni: da qualche centinaio di euro a qualche decina di migliaia di euro l'anno [RIC §2.2].

### 2.3 Proposta di valore
- **Per il musicista**: "Il libretto della tua parrocchia sul tablet, nella tonalità giusta, in Do-Re-Mi, senza rete e senza toccare lo schermo se hai un pedale. Gratis, per sempre."
- **Per l'animatore**: "Dal PDF del libretto alla scaletta di domenica in 15 minuti la prima volta [STR §5.1]; la mandi al coro con un QR o un file."
- **Per la parrocchia e la diocesi** (dal 2028, se il Gate 2 passa): "La scaletta si aggiorna da sola su tutti i leggii, anche durante la Messa, con tonalità e capotasto per ciascun musicista. In più lo storico di cosa si è cantato e la fattura intestata."

### 2.4 Posizionamento
**Per** chi suona da accordi nella liturgia italiana, **AppCanzoniere è** il leggio della Messa: scaletta per momenti liturgici, Do-Re-Mi, offline, import dei libretti italiani. **A differenza di** SongbookPro e OnSong, che sono generalisti, in inglese e senza modello liturgico, **e delle** app italiane di canti, che sono archivi senza strumenti per suonare dal vivo, solo iOS o legati a un movimento [RIC §1.2, STR §1.3].

Il "fossato" è stretto e non è tecnologico [STR §1.5, DUE §2.1]. Consiste nell'import dei libretti italiani, nell'indice liturgico, nella scaletta condivisa per la Messa e nelle relazioni con diocesi e cori. **Oggi nessuna di queste cose esiste** [VAL §3]: il fossato va costruito.

### 2.5 Il comunicato di lancio (scritto prima dei requisiti)
> *Settembre 2027.* Da oggi su Google Play c'è un leggio gratuito per chi anima la Messa. Importi il libretto PDF della tua parrocchia e prepari la scaletta della domenica per momenti: ingresso, offertorio, comunione, finale. In chiesa la suoni a schermo intero, in Do-Re-Mi, nella tonalità del coro, senza rete e girando pagina con un pedale. La scaletta arriva a tutto il coro con un QR. I canti restano sul tuo dispositivo, nessuno li pubblica. Suonare è gratis e lo resterà.

Se dopo il Gate 0 questo paragrafo non convince 10 animatori su 15, il prodotto va ripensato prima di scrivere codice.

### 2.6 Cosa NON è il prodotto
| Non è | Perché |
|---|---|
| Un catalogo pubblico di testi e accordi | È il rischio principale, non il valore principale: notifica e azione del DSA, probabile art. 17 della Direttiva DSM, diffide degli editori [STR §4.2, RIC §4.2] |
| Un generatore di accordi con l'AI | Riproduce opere protette e lo fanno già Ultimate Guitar, Chordify e Moises [RIC §3.2, STR §4.4] |
| Un "Shazam dei canti" | Costa, il repertorio liturgico è poco presente nei database e Shazam è gratis [STR §3.1] |
| Uno strumento per scaricare accordi dai siti | Viola i termini d'uso dei siti e attira segnalazioni degli editori [PROMPT §6, DUE §2.3] |
| Un software di proiezione dei testi | È un altro mercato (Worship Extreme e simili) [RIC §1.1] |
| Un leggio generalista per band | SongbookPro, OnSong e MobileSheets sono più maturi: lo si può usare anche così, ma non è il messaggio [STR §1.5] |
| Un lezionario o un messale | I testi CEI sono protetti: si mostrano solo i riferimenti biblici [RIC §4.1] |
| Un social network o una community | Dipende da una comunità che non c'è [UX] |

---

## 3. Perimetro del prodotto

### 3.1 I tre livelli
- **v1, prima versione pubblicabile** (lancio Android entro il 19/09/2027): tutto in locale, senza backend, senza account, senza pagamenti. Circa 330 ore di sviluppo [IPOTESI, §8.4].
- **v2, versione successiva** (dal 2028): ogni blocco ha una condizione. Si costruisce solo se scatta il gate indicato.
- **Escluso**: tolto dal perimetro commerciale. Si riapre solo con un fatto nuovo, indicato nella colonna "Perché".

Stato nel codice per la v1, da [VAL §3, §6]: **Tenere** (esiste e funziona), **Rifare** (esiste ma è rotto o mal collegato), **Creare** (assente, finto o solo nel modello dati).

### 3.2 Requisiti del prompt iniziale: dove finiscono

| # | Requisito [PROMPT] | Destinazione | Perché | Stato per la v1 [VAL] |
|---|---|---|---|---|
| 1 | Testi + accordi (ChordPro) leggibili | **v1** | È il cuore del leggio | Parser: **Tenere** (45 test veri). Accordi sopra il testo: **Creare** (oggi si vede il ChordPro grezzo) |
| 2 | Tablature per chitarra | **v1 minima**: i blocchi tab si mostrano come sono, in monospazio, senza trasposizione. Trasposizione delle tab: **Escluso** | Servono a non rompere i file importati da SongbookPro e OnSong [DUE §4.15]; nella liturgia sono rare | **Creare** (il parser non gestisce `{start_of_tab}`) |
| 3 | Accordi per pianoforte (note dell'accordo) | **v2** | Il tastierista di chiesa legge le sigle; in v1 la modalità "Tastiera" mostra le sigle senza capotasto | Assente |
| 4 | Trasposizione su/giù | **v1** | Funzione di base presso tutti i concorrenti | Logica: **Tenere**. Interfaccia e DI: **Rifare** (`Render` crea un transposer proprio e ignora i bemolle) |
| 5 | Scelta chitarra/pianoforte | **v1**, come modalità di lettura: Chitarra (con capotasto), Tastiera, Solo testo | Serve anche ai cantori (Solo testo) [UX] | **Creare** (l'impostazione si salva ma nessuno la legge) |
| 6 | Notazione Do-Re-Mi o C-D-E, diesis/bemolle | **v1** | Differenza dichiarata rispetto ai generalisti | **Creare** |
| 7 | DB privato locale | **v1** | Tutto il prodotto v1 è locale | **Rifare**: un solo set di migrazioni EF, `Migrate()` atteso, niente `EnsureCreated` |
| 8 | DB pubblico/condiviso, canzoni pubbliche/private | **Escluso.** Sostituito da: condivisione con file o QR (v1); nucleo di 20-40 canti di dominio pubblico verificati, inclusi nell'app (v1); indice liturgico pubblico **senza testi** (v2) | Rischio legale asimmetrico [STR §4.2]; gli editori sono possibili partner, non bersagli [DUE §2.3] | Backend: fermo, non usarlo. `DownloadService`, `OfflineSong`: **Buttare** |
| 9 | Sync del DB privato su Dropbox/Drive/OneDrive | **Escluso** come integrazione. **v1**: backup manuale su file, salvabile ovunque dal selettore di sistema (anche in una cartella di Drive). **v2**: backup automatico in una cartella scelta (Strumenti Pro) | Tre integrazioni OAuth costano molto per un valore che il selettore di file dà quasi gratis | `CloudSyncService`: **Rifare** come backup su file (oggi copia il `.db` aperto) |
| 10 | Export/import del DB o di un sottoinsieme | **v1**: backup completo e ripristino; export ChordPro di canto, raccolta e scaletta | I dati sono dell'utente [PRE §2.2] | Backup: **Rifare**. Export: **Creare** |
| 11 | Metadati: titolo, autori, lingua, tonalità, tipo, tag | **v1** | Ricerca e filtri | Entità: **Tenere**. UI per lingua e tipo: **Creare** |
| 12 | Link YouTube | **v1** (link che apre l'app esterna) | Utile alle prove, costo minimo | Entità: **Tenere**. UI: **Creare** |
| 13 | MP3 e PDF allegati | **v2** (Strumenti Pro) | Pesano nei backup, servono poco la domenica | Entità: **Tenere** |
| 14 | Momenti della Messa | **v1** | Sono il posizionamento | UI, DB e seed: **Tenere** (il seed oggi probabilmente non parte: dipende dalle migrazioni) |
| 15 | Riferimenti biblici | **v1**, solo il riferimento, mai il testo | I testi CEI sono protetti [RIC §4.1] | **Tenere** |
| 16 | Riconoscimento audio | **Escluso** | Costo senza valore distintivo [PRE §5] | Mock: toglierlo dalla DI |
| 17 | Accordi suggeriti dall'AI | **Escluso** | Copyright; funzione già offerta dai grandi [STR §4.4] | Mock e client Perplexity: togliere (modello dismesso [VAL §4.2]) |
| 18 | OCR di PDF testuali e scansioni | **v1**: estrazione da PDF testuale e OCR sul dispositivo, un canto o una pagina alla volta, con revisione affiancata. **v2**: import in blocco del libretto (Strumenti Pro). **Condizionato alla prova tecnica del Gate 0** (§5) | È la funzione simbolo e la più rischiosa [DUE §2.5] | **Creare** (oggi mock; Tesseract è uno stub) |
| 19 | Scraping privato da siti di accordi | **Escluso**. **v1**: "incolla testo con accordi", con riconoscimento delle righe di accordi | Termini d'uso e diritti; l'incolla copre l'uso legittimo | Assente; non crearlo |
| 20 | Album manuali con ordinamento | **v1**, come Raccolte | Organizzazione di base | **Tenere** |
| 21 | Album automatici con l'AI | **Escluso** in forma AI. **v1**: filtri per momento e "non usato di recente". **v2**: raccolte automatiche con regole locali | Niente AI in cloud sui testi [DUE §4.3] | **Creare** |
| 22 | Offline per tutto, raccolta o canzone | **v1**, per costruzione: tutto è locale | Requisito del leggio | Nessun lavoro (il codice di download si butta) |
| 23 | T&C, consenso all'upload, stato legale, tracciamento dell'uploader | **v1**: T&C, informativa privacy, dichiarazione **non preselezionata** all'import e all'invio, stato legale dichiarato per canto. Tracciamento dell'uploader: non serve senza server (v2 solo per le scalette) | Obbligatorio per lo store; utile come prova di buona fede | **Creare** (c'è solo `CopyrightStatus` nel dominio) |
| 24 | Privacy (account, note personali) | **v1** senza account e senza dati raccolti dallo sviluppatore | Riduce l'obbligo a informativa e scheda "Sicurezza dei dati" | **Creare** i documenti |
| 25 | Multilingua IT/EN | **v1**: infrastruttura resx dal primo giorno, interfaccia in italiano. **v2**: inglese | Estrarre le stringhe dopo costa di più [VAL §7.9]; l'inglese oggi non porta utenti | **Creare** |
| 26 | CRUD canzoni | **v1** | Flusso base | **Rifare**: navigazione (`IQueryAttributable`) e salvataggio di una canzone nuova |
| 27 | Ricerca per titolo, autore, tag | **v1** (più ricerca sull'incipit) | — | **Tenere** |
| 28 | Filtri per tonalità, tipo, lingua, momento, riferimento biblico | **v1**: momento e tonalità. **v2**: tipo, lingua, riferimento biblico | Bastano per preparare la Messa | `AdvancedSearchAsync`: **Buttare e rifare** (conversione int→Guid errata) |
| 29 | Login e account | **Escluso in v1.** **v2**: solo per i responsabili del Piano Coro | Ogni account porta GDPR, cancellazione e assistenza | Backend Identity: **Rifare** solo se il Gate 2 passa |
| 30 | Android | **v1** | Costo d'ingresso minimo; da confermare col censimento | Build da verificare [VAL §2.3] |
| 31 | Windows desktop | **v2**, dopo il Gate 1 e solo se lo chiedono almeno 3 cori | Pacchetto, firma, secondo store e assistenza costano più del "compila già" [DUE §7.12] | Compila solo in una copia ridotta |
| 32 | iOS/macOS | **Condizionato**: passa prima di Windows se nel censimento più del 40% usa un iPad al leggio [DUE §7.11] | 99 $/anno, serve un Mac (o un runner macOS in CI) | Non verificato |
| 33 | Funzioni attivabili per contesto legale | **v1** (feature flag) | Rischio 6 di [VAL §7] | **Creare** |

### 3.3 Funzioni imposte dal piano, assenti dal prompt
| Funzione | Livello | Stato |
|---|---|---|
| Scalette con modello "Messa" a momenti, scalette libere, "scelte della sera" (tonalità e strumento per evento) [UX] | v1 | **Creare** (album e momenti esistono, la scaletta per momenti no) |
| Modalità Palco: schermo intero, anteprima del successivo, ricerca d'emergenza, schermo sempre acceso | v1 | **Creare** |
| Pedale Bluetooth (emulazione tastiera) e tasti del volume, **gratuiti** | v1 | **Creare** |
| Capotasto | v1 | **Creare** |
| Condivisione: scaletta via QR, canti e raccolte via file (foglio di condivisione di Android) | v1 | **Creare** |
| Compatibilità ChordPro con SongbookPro e OnSong in import ed export | v1 | Parziale (parser) |
| Nucleo di canti di dominio pubblico verificati | v1 | Ricerca, non codice |
| Indice liturgico pubblico senza testi (aggiornato con l'app, senza server) | v2, dopo il Gate 1 | Creare |
| Calendario liturgico e suggerimenti per slot [UX, proposta C] | v2 o oltre, dopo il Gate 1 | Creare |
| Piano Coro: sincronizzazione della sola scaletta | v2, se passa il Gate 2 | Creare (backend nuovo e piccolo) |
| Strumenti Pro: import in blocco, modelli di scaletta, stampa ed export PDF della scaletta, backup automatico, allegati | v2, dopo il Gate 2 | Creare |

**Criterio di "finito" per ogni funzione v1** [DUE §2.5]: provata su un tablet Android reale ed economico. Per tutto ciò che tocca la Modalità Palco: **usata in una Messa vera**. "I test passano" non basta: è lo stesso errore che ha portato all'"87% completato" contro un 25-30% reale [VAL §1].

### 3.4 Effetti sulle decisioni UX e UI ancora aperte
- La UX raccomandata (B "Scaletta" + modello Messa da C + ricerca da A) è coerente con questo piano [UX]. Tre elementi "comuni a tutte" le proposte UX escono dalla v1: pubblicazione in tre passi, "Bozza AI" e moderazione del catalogo. Le decisioni UX 4, 5 e 7 trovano risposta qui: condivisione nel gruppo con file e QR in v1 e sincronizzazione in v2; nessun account in v1; nessun catalogo da moderare.
- La UI raccomandata, "Palco", è coerente con il momento critico: la lettura a un braccio di distanza con poca luce [UI]. Raccomando "Palco" sia per il tema scuro sia per quello chiaro in v1. L'ibrido con il tema chiaro di "Leggio" costa un secondo sistema di temi: si valuta in v2. "Vetrata" richiede il calendario liturgico e non è compatibile con la v1.

---

## 4. Modello di ricavo e prezzi definitivi

### 4.1 Conflitti tra i documenti e come li risolvo

| Tema | Strategia [STR] / Prezzi [PRE] | Duello [DUE] | **Decisione di questo piano** | Perché |
|---|---|---|---|---|
| Monetizzazione al lancio | Pro una tantum da subito [STR §3.5] | Lancio gratuito | **Lancio gratuito** | Il Pro con quota rende circa 30 €/mese in più nello scenario base [DUE §2.4]. Costa però partita IVA e 400-800 €/anno di commercialista prima del primo euro [PRE §6.4], frena le diocesi, che sono il canale principale [STR §2], e si fa pagare per funzioni che non esistono [DUE §7.3]. Il saldo è negativo |
| Quota sui canti | 30 canti propri [PRE §2.2] | Nessuna quota al lancio | **Nessuna quota, mai**, neanche più avanti | Si aggira esportando e reimportando [DUE §2.4], e l'unica app liturgica italiana a pagamento ha 3,49★ [RIC §1.2]. Togliere un limite dopo è facile; metterlo dopo un lancio gratuito ripete l'errore di OnSong [RIC §3.1] |
| Pedale | Pro [PRE §2.2] | Sempre gratis | **Sempre gratis** | Se la verifica dell'acquisto fallisce senza rete, il pedale si ferma durante la Messa: contraddice la regola "mai bloccare la domenica" [DUE §7.6] |
| Prezzo del Pro | 9,99 €, lancio a 6,99 € per 90 giorni [PRE §3.1] | 7,99 € fissi, solo funzioni nuove, nella seconda fase | **7,99 € fissi, senza promozioni a tempo, dopo il Gate 2** | Allineato a SongbookPro (~7 $) [PRE §1.1]. La curva è piatta tra 8,99 e 10,99 € [PRE §3.3]. Il prezzo di lancio porta l'obbligo del "prezzo più basso nei 30 giorni" [DUE §7.17] |
| Piano Coro: cosa sincronizza | Repertorio e scalette, con testi sul server [PRE §4.2] | Solo la scaletta | **Solo la scaletta**: titoli, ordine, momenti, tonalità, capotasto, note brevi | I testi sul nostro server, a pagamento, sono la configurazione più esposta (DSA art. 16, possibile art. 17 DSM) [DUE §2.3]. In più il parere legale è più breve e l'hosting più piccolo |
| AI in cloud per pulire l'import | Nel Piano Coro, con limite [PRE §5] | Esclusa | **Esclusa** fino a nuovo parere legale | Manda testi protetti a un intermediario e a un fornitore esterno [DUE §7.15] |
| Prima offerta a pagamento | Pro; Piano Coro dopo il Gate 2 [STR §3.5] | Licenza diocesana | **Licenza diocesana, venduta insieme al Piano Coro** | Una diocesi da 20 cori vale circa 72 Pro [DUE §2.6]. **Correggo il duello su un punto**: senza un server la licenza non ha nulla da consegnare, perché il suo contenuto è il coordinamento. Il primo incasso è quindi ottobre 2028, non prima |
| Soglia del Gate 2 | ≥10 cori lo chiedono [STR §5.5]; ≥25 preordini [PRE §8] | ≥25 lettere d'intento **oppure** una diocesi, più parere legale | **≥2 K€/anno impegnati per iscritto, da almeno 10 pagatori distinti, più parere legale favorevole** | **Correggo il duello**: una sola diocesi (500 €/anno) non copre commercialista (600 €), hosting (275 €) e parere (800 €). Il primo anno chiuderebbe a circa −1,2 K€. 2 K€ è anche la soglia per aprire la partita IVA [DUE §4.10]: così le due regole coincidono. I 10 pagatori evitano di dipendere da un solo cliente |
| Preordini al Gate 0 | Preordini rimborsabili [PRE §7.1] | Lettere d'intento | **Lettere d'intento con cifra e data, nessun incasso** | Incassare prima di avere un inquadramento fiscale è il problema che segnalano gli stessi prezzi [DUE §7.14] |
| Prezzo fondatore | 35 € per i primi 30 cori, bloccato per 2 rinnovi [PRE §4.2] | Prezzo fisso senza promozioni | **35 €, riservato a chi firma una lettera d'intento**, bloccato per 2 rinnovi | Non è una promozione al consumatore ma una condizione scritta nella lettera. Applicabilità delle norme sui prezzi agli enti: **[DA VERIFICARE]** |
| Parere legale | 2.000 € in blocco prima del Piano Coro [PRE §6.1] | 1-2 ore mirate prima del lancio | **Due passaggi**: ≈ 400 € prima del lancio (4 domande), ≈ 800 € prima del Piano Coro | Con i testi fuori dal server le domande si riducono. Il lancio gratuito ha comunque bisogno di T&C solidi |
| Piattaforme | Android → Windows → iOS [STR §5.2] | Solo Android il primo anno; iPad prima di Windows se il censimento lo indica | **Come il duello** | Windows "compila già" solo in una copia ridotta [VAL §2.3]; il leggio è un uso da tablet [DUE §7.11] |
| Tempi | MVP in 3-5 mesi [STR §5.1] | 6-9 mesi, lancio a settembre 2027 | **Come il duello**, ma con un Gate Pilota aggiunto il 30/06/2027 | Google Play richiede agli account personali un test chiuso con almeno 12 tester per 14 giorni prima della produzione [N1]: i cori pilota servono anche a questo |
| Ore e rendimento orario | Non contate | 300-500 ore, 2-3 €/ora | **≈ 1.300-1.700 ore in quasi quattro anni**, < 1 €/ora nello scenario base | Il duello contava solo lo sviluppo. Assistenza, canali, vendita e manutenzione triplicano le ore (§8.4) |

### 4.2 Listino definitivo e condizioni di attivazione

| Offerta | Prezzo | Cosa contiene | Si attiva quando… | Prima data possibile |
|---|---|---|---|---|
| **App gratuita** | 0 €, per sempre, senza pubblicità | Tutta la v1: libreria senza limiti, lettura, trasposizione, Do-Re-Mi, capotasto, scalette Messa, Palco, pedale, import di un canto alla volta, incolla, condivisione QR/file, backup manuale, export ChordPro | Al lancio | 19/09/2027 |
| **Licenza diocesana** (prima offerta a pagamento) | **25 €/coro/anno**, minimo 20 cori (≥ 500 €/anno), fattura unica alla diocesi | Piano Coro per tutti i cori iscritti della diocesi; una sessione di formazione online l'anno per gli animatori; pacchetto diocesano (indice dei canti suggeriti, solo metadati oppure testi di cui la diocesi ha i diritti) | Gate 2 superato (§6) **e** una diocesi in cui almeno 10 cori usano già l'app gratuita **e** partita IVA aperta **e** contratto rivisto dal legale | 01/10/2028 |
| **Convenzione diocesana** (strumento di canale, gratis per la diocesi) | −20% alle parrocchie della diocesi: Coro 39 €, Parrocchia 79 € | Codice annuale rinnovabile | La diocesi presenta l'app nei corsi per animatori | 01/10/2028 |
| **Piano Coro** | **49 €/anno** (1 gruppo, fino a 25 membri, Strumenti Pro per 2 responsabili) | Scaletta sincronizzata in diretta su tutti i dispositivi; tonalità e capotasto per strumentista; storico "cosa abbiamo cantato"; notifica "scaletta pronta"; fattura intestata. **Mai i testi sul server** | Gate 2 superato | 01/10/2028 |
| **Piano Parrocchia** | **99 €/anno** (fino a 4 gruppi e 100 membri, Pro per 6 responsabili) | Come il Coro, per più gruppi | Gate 2 superato | 01/10/2028 |
| **Prezzo fondatore** | 35 €/anno sul Coro, bloccato per 2 rinnovi | — | Solo per chi ha firmato una lettera d'intento entro il Gate 2 | 01/10/2028 |
| **Abbonamento Coro in app** (Google Play Billing) | 49 €/anno | Come il Coro | Solo se almeno il 20% dei cori paganti chiede di pagare con carta personale, **e** la policy di Play consente di riconoscere sia l'acquisto in app sia la fattura [PRE §4.4, DA VERIFICARE] | Gate 3 |
| **Strumenti Pro** | **7,99 €** una tantum per piattaforma, prezzo fisso | Solo funzioni nuove che fanno risparmiare tempo: import in blocco del libretto, modelli di scaletta (feste, matrimoni, funerali), stampa ed export PDF della scaletta, backup automatico, allegati PDF/MP3. **Mai un limite sui canti** | Gate 2 superato (partita IVA già aperta), funzioni esistenti e provate, **e** almeno 3 cori le hanno chieste | 01/2029 |
| **Edizione B2B** (movimento, editore, diocesi con repertorio proprio) [STR §3.2 opzione C] | Su preventivo | Repertorio licenziato o "canzoniere ufficiale" | Interesse scritto di un interlocutore, contratto rivisto dal legale, nessuna esclusiva che blocchi altri contenuti | Quando si presenta; mai prima del Gate 1 |

**Regole permanenti**: niente quota sui canti; niente pubblicità; niente blocchi durante la Messa (un acquisto non verificabile senza rete non toglie mai una funzione già usata); niente abbonamento personale; niente vendita di testi; niente AI in cloud sui testi; ogni sconto con scadenza e motivazione scritte [PRE §4.5].

**Netto unitario** [IPOTESI, PRE §1.3]: Strumenti Pro 7,99 € → ≈ 5,57 € (IVA 22%, Play 15%). Coro con fattura in regime forfettario → ≈ 48 €. Licenza diocesana → 25 €/coro.

---

## 5. Piano di validazione prima di scrivere codice (Gate 0)

**Periodo**: 16/11/2026 – 31/01/2027. Comprende Avvento e Natale: si usano per osservare Messe vere, non per le interviste [DUE §7.10].
**Regola**: nessun codice di prodotto. Sono ammesse solo le due prove tecniche usa e getta (attività 5 e 6).
**Costo totale**: ≤ 300 € (più 100-150 € per un tablet Android economico, se il proprietario non ne ha uno). **Ore**: circa 80, quasi tutte del proprietario.

| # | Attività | Chi | Costo | Soglia di esito | Entro |
|---|---|---|---|---|---|
| 1 | **Contare le proprie ore** per 4 settimane (foglio settimanale) | Proprietario | 0 € | ≥ 10 h/sett: calendario principale. 8-10: calendario lungo (lancio in Avvento 2027). 6-8: v1 ridotta (senza OCR) e calendario lungo. < 6: non commerciale (§12) | 13/12/2026 |
| 2 | **Censimento dei dispositivi**: modulo online di 5 domande (strumento, telefono o tablet, marca e sistema, pedale, chi sceglie i canti), diffuso da 2-3 uffici liturgici e gruppi di animatori | Proprietario + ambasciatore | 0 € | ≥ 40 risposte; ≥ 60% usa già un dispositivo al leggio; ≥ 55% di questi usa Android. Se più del 40% usa un iPad: rivedere l'ordine delle piattaforme | 10/01/2027 |
| 3 | **Osservare 5 Messe** in Avvento e a Natale (come si legge, chi cambia cosa, quanta luce c'è) | Proprietario | 0 € | Note scritte su 5 celebrazioni | 27/12/2026 |
| 4 | **Interviste** ad animatori e musicisti (copione di [PRE §7.1], domande 1-5 e 10-12, **senza preordini**), con la wireframe UX-B come prototipo cliccabile | Proprietario | 0 € | ≥ 15 interviste; ≥ 10 su 15 riconoscono il problema del §2.5 come proprio | 24/01/2027 |
| 5 | **Prova tecnica dell'import** su 10 libretti PDF reali (fonti in [RIC S42]) | Proprietario con agenti AI | 0 € | ≥ 70% dei canti importati con ≤ 2 minuti di correzione ciascuno. Tra 50 e 70%: v1 senza OCR delle scansioni (PDF testuale + incolla). < 50%: import fuori dalla v1 e messaggio riscritto | 17/01/2027 |
| 6 | **Prova tecnica della Modalità Palco**: canto di 2 pagine con accordi sopra il testo, scorrimento e pedale su un tablet Android economico, in MAUI | Proprietario con agenti AI | 30-60 € (pedale usato) | Fluida; il pedale gira pagina senza configurazioni particolari. Se fallisce: una settimana di tentativi, poi valutare un'alternativa tecnica prima di procedere | 24/01/2027 |
| 7 | **Verifica dei concorrenti**: installare SongbookPro e la prova di MobileSheets (notazione, lingua, scalette, import PDF). Mostrare a 5 musicisti il video di SongbookPro e la wireframe UX-B | Proprietario | 0-7 € | La maggioranza sceglie la nostra per la Messa. Se SongbookPro ha già italiano e Do-Re-Mi: messaggi riscritti sulle differenze vere [DUE §2.1] | 15/12/2026 (installazione), 24/01/2027 (confronto) |
| 8 | **Contatto con un editore e con l'Ufficio giuridico CEI**, con una domanda neutra: "Esiste una licenza per i testi in un'app? A quali condizioni?" | Proprietario | 0 € | Domande inviate entro il 15/12. Al gate: nessuna obiezione esplicita. Un'obiezione blocca le funzioni contestate e va al legale | 15/12/2026 (invio) |
| 9 | **Proposta a 3 uffici liturgici diocesani**: una pagina + video di 2 minuti della wireframe | Proprietario | 0 € | ≥ 1 ufficio offre uno spazio in un corso, un incontro o una newsletter | 31/01/2027 |
| 10 | **Lettere d'intento** di parroci o responsabili per il futuro Piano Coro (cifra e data, nessun incasso) | Proprietario | 0 € | ≥ 5 lettere a ≥ 35 €/anno, **oppure** interesse scritto di una diocesi | 31/01/2027 |
| 11 | **5 cori pilota** disposti a provare da Pasqua 2027, con almeno 12 musicisti in tutto (servono come tester per Play [N1]) | Proprietario | 0 € | ≥ 5 cori, ≥ 3 con tablet Android, ≥ 12 persone | 31/01/2027 |
| 12 | **Un'ora con un commercialista** (domande nel §9.3) | Commercialista | 100-200 € | Costo fisso annuo previsto con partita IVA ≤ 1 K€; forfettario compatibile con la situazione del proprietario | 20/01/2027 |
| 13 | **Trovare un animatore-ambasciatore** che porti l'app nei corsi e faccia il primo livello di assistenza [DUE §2.5] | Proprietario | 0 € | Una persona con nome e cognome | 31/01/2027 |
| 14 | **Decisione scritta**: procedo (commerciale) / procedo come servizio gratuito (§12) / mi fermo | Proprietario | 0 € | Vedi regola sotto | **31/01/2027** |

**Regola di esito**
- **Fermarsi del tutto** se fallisce una di queste: ore < 6 h/sett; < 60% usa un dispositivo al leggio; meno di 3 cori pilota; prova Palco fallita senza alternativa.
- **Procedere come servizio gratuito (§12)** se gli usi sono confermati ma falliscono le soglie commerciali: attività 10 (lettere d'intento), 9 (uffici liturgici) o 12 (costo fiscale).
- **Procedere con il piano commerciale** se passano entrambi i gruppi.
- Censimento con iPad > 40%: si procede, ma prima di sviluppare si ridecide l'ordine delle piattaforme (decisione D9).

---

## 6. Tappe e gate successivi

### 6.1 Calendario principale (con ≥ 10 h/settimana misurate)

| Data | Tappa | Proseguire se… | Cambiare rotta se… | Fermarsi se… |
|---|---|---|---|---|
| **31/01/2027** | **Gate 0** (§5) | Tutte le soglie del §5 | Vedi la regola di esito | Vedi la regola di esito |
| Feb 2027 | Rifare persistenza, navigazione e CI (§10), poi il primo flusso completo | — | — | — |
| **28/02/2027** | **Traguardo tecnico T1** [DUE §2.5]: un canto vero da un PDF vero, importato, trasposto in Do-Re-Mi e suonato in Palco con il pedale su un tablet economico | Fatto, su dispositivo reale | Non fatto: togliere l'OCR dalla v1 e nuova data al 15/04/2027 | Non fatto neanche il 15/04: §12 oppure stop |
| 28/03/2027 (Pasqua) | Uso nella propria parrocchia; test chiuso Play con i cori pilota | — | — | — |
| **30/06/2027** | **Gate Pilota** (aggiunto da questo piano) | ≥ 3 cori pilota usano l'app ≥ 3 domeniche su 4 per 6 settimane; 0 perdite di dati; ≥ 99,5% di sessioni senza crash (Android vitals); ≥ 12 tester per ≥ 14 giorni con accesso alla produzione ottenuto [N1]; parere legale mirato ricevuto | 1-2 soglie mancate: lancio spostato in Avvento (28/11/2027), Gate 1 in Quaresima 2028 | Perdite di dati non risolte oppure nessun coro pilota attivo |
| Lug-ago 2027 | Rifinitura, FAQ, 3 video, canti di dominio pubblico, scheda dello store. **Nessun lancio d'estate** | — | — | — |
| **Entro 19/09/2027** | **Lancio pubblico Android gratuito**, presentazioni in 1-2 diocesi | — | — | — |
| **09/01/2028** | **Gate 1**, misurato in Avvento (28/11-19/12/2027) [DUE §4.7] | ≥ 300 dispositivi con l'app installata; ≥ 40% di questi attivi in ≥ 3 domeniche su 4; ≥ 10 cori che usano la scaletta condivisa (registro volontario dei cori); voto ≥ 4,3 con ≥ 20 recensioni | Una correzione (prodotto o canali), poi **Gate 1-bis il 30/04/2028**, misurato in Quaresima (01/03-09/04/2028). In quel caso il Gate 2 slitta di un anno | Gate 1-bis fallito: app gratuita in manutenzione (§12) |
| Gen-mag 2028 | Lettere d'intento per il Piano Coro; proposta di licenza diocesana alle diocesi in cui l'app è usata | — | — | — |
| **31/05/2028** | **Gate 2**. Anticipato rispetto al duello (settembre) perché il backend va costruito in estate per partire a ottobre | **≥ 2 K€/anno impegnati per iscritto da ≥ 10 pagatori distinti**; parere legale favorevole sulla scaletta sincronizzata; commercialista: partita IVA apribile; ≥ 8 h/sett nelle ultime 8 settimane | Tra 1 e 2 K€ impegnati: si resta gratuiti un altro anno e si ripete il 31/05/2029 | < 1 K€ impegnati per due anni: niente Piano Coro, app gratuita (§12) |
| Giu-set 2028 | Partita IVA; backend minimo per le scalette; account solo per i responsabili | — | — | — |
| **01/10/2028** | **Piano Coro e licenza diocesana attivi**; primi incassi | — | — | — |
| 01/2029 | Strumenti Pro a 7,99 € (se scattano le condizioni del §4.2) | — | — | — |
| **30/09/2029** | **Gate 3**, dopo il primo ciclo di rinnovi | Rinnovi ≥ 70% delle unità paganti; ricavo netto degli ultimi 12 mesi ≥ 2,5 K€; margine dopo il fisco ≥ 0; assistenza ≤ 2 h/sett in media | Margine < 0 ma rinnovi ≥ 70%: tagliare i costi (hosting, commercialista) e niente funzioni nuove per un anno | Rinnovi < 50%: chiudere il Piano Coro con 60 giorni di preavviso e tornare al §12 |
| Ago 2030 | Revisione finale dei 36 mesi dal lancio | Decidere: investire (iOS, inglese, B2B), mantenere oppure cedere o rendere open source | — | — |

### 6.2 Calendario lungo (8-10 h/settimana misurate al Gate 0)
Gate 0 e T1 restano uguali. Gate Pilota il 30/09/2027; lancio entro il 28/11/2027 (Avvento); Gate 1 il 30/04/2028 (misurato in Quaresima); Gate 2 il 30/11/2028; Piano Coro dal 01/2029; Gate 3 il 31/01/2030. **Nel conto economico del §8 tutti i ricavi slittano di circa 4 mesi.**

---

## 7. Go-to-market a costo quasi zero

### 7.1 Principio
Il prezzo non è la leva: lo è il numero di cori che usano l'app la domenica [PRE §3.3]. Il canale vero sono le persone: uffici liturgici, corsi per animatori, passaparola tra cori. Il vincolo è il tempo del proprietario, quindi ogni canale ha un budget di ore [DUE §7.16].

### 7.2 I primi cori pilota (5, scelti nel Gate 0)
| Profilo | Perché |
|---|---|
| Coro adulti della propria parrocchia | Uso settimanale osservabile, dispositivo del proprietario sul leggio |
| Coro giovani o gruppo scout | Pubblico più incline al tablet; **minori**: verificare il consenso al trattamento dei dati anche senza account [§9.2] |
| Una parrocchia dell'unità pastorale vicina | Primo passaparola fuori dalla propria cerchia |
| Il coro dell'ambasciatore (altra parrocchia o diocesi) | Prova lontano dal proprietario: l'assistenza deve funzionare a distanza |
| Un coro con tastierista | Verifica la modalità "Tastiera" e il Do-Re-Mi |

**Cosa ricevono**: accesso anticipato da Pasqua 2027; il prezzo fondatore in caso di Piano Coro; il loro nome nei ringraziamenti, se lo vogliono.
**Cosa danno**: partecipazione al test chiuso di Play (≥ 12 persone in tutto), una breve nota ogni settimana, un caso di studio a giugno 2027.

### 7.3 Canali e ore
| Fase | Canale | Azione | Ore stimate [IPOTESI] |
|---|---|---|---|
| Gate 0 | Rete personale, 3 uffici liturgici | Censimento, interviste, proposta di una pagina [STR §5.3] | Incluse nelle 80 h del Gate 0 |
| Pre-lancio (lug-ago 2027) | Contenuti propri | 3 video brevi: "dal PDF al leggio in 5 minuti", "il pedale alla Messa", "la scaletta al coro con un QR". **Solo canti propri o di dominio pubblico** [DUE §2.3] | 15 |
| Pre-lancio | Scheda dello store | Parole chiave "canti liturgici accordi", "canti messa", "scaletta coro"; screenshot della scaletta della Messa; frase "suonare sarà sempre gratis" | 8 |
| Lancio (set-ott 2027) | Diocesi | Presentazione di 10 minuti nei corsi per animatori di 1-2 diocesi (la propria e quella dell'ambasciatore) [RIC §5] | 20 |
| Lancio | Siti cristiani | Proposta a NuoviConvertiti, Animatamente, SpazioAccordi: link reciproci, "apri nell'app" [STR §5.3] | 5 |
| Lancio | Produttori di pedali | Richiesta di inserimento nella lista di app compatibili AirTurn [RIC §5] | 2 |
| Anno 1 | Gruppi WhatsApp di animatori, tramite l'ambasciatore | Video e FAQ | 1 h/mese |
| Anno 1 | Corsi CEI (MLO, COPERLIM), incontri diocesani dei cori, Settimana Liturgica Nazionale (date 2027 [DA VERIFICARE]) | Presenza solo dove l'ambasciatore o un ufficio offre lo spazio | 20 |
| Gen-mag 2028 | Parroci e diocesi in cui l'app è usata | Lettere d'intento, proposta di licenza diocesana | 30 |
| Escluso | Pubblicità a pagamento, mercato anglofono, pubblico non liturgico | Costo senza ritorno nella nicchia [DUE §2.6a] | — |

### 7.4 Messaggi per pubblico
| Pubblico | Come descrive il problema | Messaggio | Prova |
|---|---|---|---|
| Musicista | "Il PDF sul telefono non si legge, e non si traspone" | "Il tuo libretto sul tablet, in Do-Re-Mi, nella tonalità giusta. Gratis, per sempre." | Video di 60 secondi |
| Animatore | "Ogni settimana mando la scaletta su WhatsApp e qualcuno ha la tonalità sbagliata" | "Prepari la Messa per momenti e la mandi al coro con un QR." | Caso di studio di un coro pilota |
| Parroco ed economo (dal 2028) | "Fotocopie, raccoglitori, ogni coro fa da sé" | "Una scaletta per tutta la parrocchia, aggiornata su ogni leggio, con fattura intestata." | Lettere d'intento, costo per membro ≈ 2 €/anno [PRE §4.2] |
| Ufficio liturgico | "Formiamo gli animatori, ma ognuno usa materiali diversi" | "Uno strumento gratuito per i vostri corsi; con la licenza diocesana il coordinamento arriva a tutti i cori." | Numero di cori della diocesi che già la usano |
| Editore | "Le app riproducono i nostri canti senza licenza" | "L'app non pubblica testi. Se volete distribuire il vostro repertorio in digitale, possiamo farlo insieme." | T&C, assenza del catalogo, scaletta senza testi |

---

## 8. Conto economico a 36 mesi

### 8.1 Periodo e ipotesi
- **Periodo**: Anno 0 di preparazione (16/11/2026 – 31/08/2027), poi **36 mesi dal lancio**: Anno 1 (set 2027 – ago 2028), Anno 2 (set 2028 – ago 2029), Anno 3 (set 2029 – ago 2030). Calendario principale del §6.1.
- **Ricavi**: netti di IVA e commissioni dello store; in regime forfettario la fattura alle parrocchie non ha IVA [PRE §4.4, DA VERIFICARE].
- **Imposte e contributi**: 21% del ricavo (forfettario, coefficiente 67%, imposta sostitutiva 5%, gestione separata INPS) [PRE §6.4, DA VERIFICARE].
- **Commercialista**: 150 € di consulenza nell'Anno 0 (compresa nei 300 € del Gate 0) e nell'Anno 1; 600 €/anno dall'apertura della partita IVA (Anno 2) [PRE §6.4].
- **Legale**: ≈ 400 € prima del lancio (Anno 0); ≈ 800 € prima del Piano Coro (Anno 2) [IPOTESI, §4.1].
- **Costi vivi**: account Play 25 $ (≈ 23 €); dominio, sito ed email 50 €/anno; hosting della sola scaletta 225 €/anno dal Piano Coro (400 € nello scenario favorevole all'Anno 3); fattura elettronica e incassi 50 €/anno; Gate 0 300 €. Tablet di prova escluso (+100-150 € se serve).
- **Non inclusi**: valore del tempo del proprietario (le ore sono nel §8.4), iOS, Windows, mercato anglofono.
- **Probabilità** degli scenari: 45% / 40% / 15%, riprese da [STR §3.4]. Sono soggettive e **condizionate al superamento del Gate 0**.

| Ipotesi di volume | Prudente | Base | Favorevole |
|---|---|---|---|
| Gate 2 | Non superato: l'app resta gratuita | Superato di poco | Superato largamente |
| Licenze diocesane (cori) Anno 2 / Anno 3 | 0 / 0 | 1 (20) / 2 (45) | 2 (50) / 4 (110) |
| Cori e parrocchie con fattura diretta, Anno 2 / Anno 3 | 0 / 0 | 40 / 50 | 60 / 100 |
| Netto medio per unità diretta (mix Coro, Parrocchia, fondatore 35 €, convenzione −20%) | — | 38 € / 42 € | 38 € / 42 € |
| Strumenti Pro venduti (solo Anno 3, prudenziale) | 0 | 100 | 300 |
| Contratto B2B | — | — | 1 da 2 K€ nell'Anno 3 |

### 8.2 Risultati (euro, arrotondati)

**Prudente (45%)**: il Gate 2 non passa, l'app resta gratuita.

| | Anno 0 | Anno 1 | Anno 2 | Anno 3 | Totale |
|---|---|---|---|---|---|
| Ricavi | 0 | 0 | 0 | 0 | **0** |
| Costi (Gate 0, Play, dominio, legale, commercialista) | −800 | −200 | −50 | −50 | −1.100 |
| Imposte | 0 | 0 | 0 | 0 | 0 |
| **Netto** | −800 | −200 | −50 | −50 | **−1.100** |

**Base (40%)**

| | Anno 0 | Anno 1 | Anno 2 | Anno 3 | Totale |
|---|---|---|---|---|---|
| Licenze diocesane | — | — | 500 | 1.125 | 1.625 |
| Coro e Parrocchia con fattura | — | — | 1.520 | 2.100 | 3.620 |
| Strumenti Pro | — | — | — | 557 | 557 |
| **Ricavi** | 0 | 0 | 2.020 | 3.782 | **5.802** |
| Costi (commercialista 600, hosting 225, dominio 50, incassi 50, legale 800 nell'Anno 2) | −800 | −200 | −1.725 | −925 | −3.650 |
| Imposte e contributi (21%) | 0 | 0 | −424 | −794 | −1.218 |
| **Netto** | −800 | −200 | **−129** | **+2.063** | **+934** |

**Favorevole (15%)**

| | Anno 0 | Anno 1 | Anno 2 | Anno 3 | Totale |
|---|---|---|---|---|---|
| Licenze diocesane | — | — | 1.250 | 2.750 | 4.000 |
| Coro e Parrocchia con fattura | — | — | 2.280 | 4.200 | 6.480 |
| Strumenti Pro | — | — | — | 1.671 | 1.671 |
| Contratto B2B | — | — | — | 2.000 | 2.000 |
| **Ricavi** | 0 | 0 | 3.530 | 10.621 | **14.151** |
| Costi | −800 | −200 | −1.725 | −1.200 | −3.925 |
| Imposte e contributi (21%) | 0 | 0 | −741 | −2.230 | −2.971 |
| **Netto** | −800 | −200 | +1.064 | +7.191 | **+7.255** |

**Prova di stress** (Gate 2 superato sulla carta, poi paga solo il 60% di chi ha firmato e i rinnovi sono scarsi): ricavi 1.200 € nell'Anno 2 e 1.000 € nell'Anno 3 → **netto totale ≈ −1,9 K€**. È lo scenario peggiore possibile seguendo le regole del piano, e il motivo per cui la soglia del Gate 2 è fissata in euro impegnati e non in cori interessati.

**Fermarsi al Gate 0**: ≈ −0,3 K€ e circa 80 ore.

### 8.3 Valore atteso e confronto con i documenti precedenti

| | Prudente | Base | Favorevole | Valore atteso |
|---|---|---|---|---|
| **Questo piano** (da oggi ad agosto 2030, dopo fisco, commercialista e legale) | **−1,1 K€** | **+0,9 K€** | **+7,3 K€** | **≈ +1,0 K€** |
| Piano originale, 36 mesi, dopo fisco e commercialista, senza legale mirato [PRE §6.4] | −1,7 K€ | +0,5 K€ | +10 K€ | ≈ +0,9 K€ [DUE §2.6c] |

**Lettura senza ammorbidire**
- Il piano rivisto **non guadagna più** di quello originale: il valore atteso è quasi uguale, circa 1 K€.
- **Perde meno nello scenario più probabile** (−1,1 contro −1,7 K€), perché non apre la partita IVA prima che ci siano ricavi.
- **Rinuncia a parte del rialzo** (+7,3 contro +10 K€): niente Pro nel primo anno, Pro a 7,99 € e più tardi.
- **Tiene aperta l'opzione** di crescere verso diocesi ed editori.
- Il pareggio cumulato arriva solo nell'Anno 3, e solo negli scenari base e favorevole. **In nessuno scenario c'è un reddito.**

### 8.4 Ore di lavoro stimate [IPOTESI]

| Voce | Anno 0 | Anno 1 | Anno 2 | Anno 3 |
|---|---|---|---|---|
| Gate 0 | 80 | — | — | — |
| Sviluppo v1: persistenza 30, navigazione e CRUD 25, rendering e notazione 40, scalette 35, Palco e pedale 40, import 60, condivisione e backup 30, resx 10, documenti legali e feature flag 10, dominio pubblico 10, CI e store 25, prove su dispositivi 15 | 330 | — | — | — |
| Preparazione del lancio (FAQ, video, scheda, ricerca sul dominio pubblico) | 50 | — | — | — |
| Manutenzione e correzioni | — | 120 | 80 | 100 |
| Assistenza | — | 80 | 100 | 120 |
| Canali e vendita | — | 90 | 50 | 60 |
| Backend per le scalette, licenze, codici (Anno 2); Strumenti Pro (Anno 3) | — | — | 200 | 100 |
| **Totale, scenario base** | **460** | **290** | **430** | **380** |
| Prudente (Anni 2-3: solo manutenzione e assistenza) | 460 | 290 | 100 | 80 |
| Favorevole (più vendita e personalizzazione B2B) | 460 | 290 | 450 | 480 |

| Scenario | Ore totali | Netto | Euro per ora |
|---|---|---|---|
| Prudente | ≈ 930 | −1,1 K€ | **< 0** |
| Base | ≈ 1.560 | +0,9 K€ | **≈ 0,6 €/h** |
| Favorevole | ≈ 1.680 | +7,3 K€ | **≈ 4,3 €/h** |
| Atteso | ≈ 1.300 | +1,0 K€ | **≈ 0,75 €/h** |

Carico medio: circa 11 h/settimana nell'Anno 0 (per lanciare a settembre 2027), 5-6 nell'Anno 1, 8 nell'Anno 2. Con 8-10 h/settimana vale il calendario lungo (§6.2).

---

## 9. Rischi, legale e commercialista

### 9.1 Rischi principali

| # | Rischio | Prob. / Impatto | Mitigazione | Segnale d'allarme |
|---|---|---|---|---|
| 1 | **Le istituzioni non pagano**: il coordinamento non vale più di WhatsApp + PDF [DUE §7] | Alta / Alto | Lettere d'intento al Gate 0; Gate 2 in euro impegnati; nessuna spesa ricorrente prima | < 5 lettere d'intento al Gate 0 |
| 2 | **Abbandono da parte del proprietario** (tempo, motivazione, prodotto diverso da quello immaginato) [DUE §2.5] | Alta / Alto | Decisione esplicita D2; ore misurate; traguardi brevi con data; ambasciatore; ambito minimo | Media sotto le 6 h/sett per 4 settimane |
| 3 | **Perdita di dati la domenica** (migrazione fallita, crash) [STR §6.4] | Media / Molto alto | Backup locale automatico prima di ogni migrazione; test di aggiornamento da ogni versione pubblicata; rilascio graduale; nessun aggiornamento da venerdì a domenica | Una sola segnalazione di dati persi blocca il rilascio |
| 4 | **Segnalazione di un editore a Google Play** [DUE §2.3] | Media / Alto | Nessun testo sul server; nessuna funzione venduta come "riproduzione del libretto"; screenshot e video con soli canti di dominio pubblico o propri; contatto preventivo nel Gate 0; procedura di segnalazione pronta (§10.8) | Lettera o segnalazione di un titolare di diritti |
| 5 | **Falsa completezza** del lavoro fatto con agenti AI (il "87%" contro il 25-30% reale) [VAL §7.1] | Alta / Medio | "Finito" vuol dire provato su un tablet reale e in una Messa vera; test di integrazione su SQLite reale; CI verde come condizione | Funzioni dichiarate finite senza prova su dispositivo |
| 6 | **Import dei libretti di qualità insufficiente** | Media / Alto | Prova tecnica nel Gate 0; ripiego su PDF testuale + incolla | < 70% dei canti con ≤ 2 minuti di correzione |
| 7 | **Il pubblico usa l'iPad**, non Android [DUE §7.11] | Media / Alto | Censimento nel Gate 0; decisione D9 | iPad > 40% nei cori pilota |
| 8 | **Un concorrente gratuito o diocesano** (Canta e Cammina su Android, app di un editore) [DUE §2.2] | Media / Alto | Offrirsi come motore per il repertorio di diocesi ed editori; velocità sulle relazioni | Annuncio di un'app diocesana o di un editore |
| 9 | **Vincoli di Google Play**: 12 tester per 14 giorni [N1]; dichiarazione di "trader" con indirizzo e telefono visibili agli utenti UE se si vende [N2, DA VERIFICARE]; applicationId permanente dopo il primo caricamento | Certa / Medio | Cori pilota come tester; restare "non trader" finché l'app è gratuita, se il commercialista e il legale lo confermano; scegliere nome e applicationId prima di marzo 2027 (D10) | Richiesta di accesso alla produzione respinta |
| 10 | **Dipendenza da una diocesi** o da un solo ufficio liturgico | Media / Medio | Gate 2 con almeno 10 pagatori distinti | Una diocesi pesa più del 50% del ricavo |
| 11 | **Minori nei cori giovani** e dati personali | Media / Medio | Nessun account in v1; nel Piano Coro i membri entrano con un codice, senza account né dati personali; account solo per gli adulti responsabili | Richiesta di account per i membri |
| 12 | **Vulnerabilità note e stack non aggiornato** [VAL §4.6] | Certa / Medio | Aggiornamento a 10.0.x più recente e controllo delle vulnerabilità in CI prima di qualsiasi rilascio | `dotnet list package --vulnerable` non vuoto |
| 13 | **Assistenza la domenica mattina** [DUE §2.5] | Alta / Medio | FAQ e 3 video prima del lancio; ambasciatore come primo livello; nessun canale di assistenza "in tempo reale" promesso | > 2 h/sett di assistenza in media |

### 9.2 Domande per il legale

**Prima del lancio: parere mirato, ≈ 400 €, entro il 30/06/2027**
1. Importare nell'app, per uso personale, un libretto PDF di cui si possiede una copia: quali limiti ha l'utente e quali responsabilità ha lo sviluppatore, che non ospita nulla?
2. Condividere testi tra dispositivi (file o QR), senza passare dai nostri server: lo sviluppatore ha qualche responsabilità? Come vanno scritti T&C e dichiarazione all'invio?
3. Un nucleo di canti di dominio pubblico incluso nell'app: come documentare la verifica canto per canto (autore morto da oltre 70 anni, traduzioni, armonizzazioni)?
4. T&C e informativa privacy per un'app senza account e senza raccolta di dati, usata anche da minori. Età del consenso digitale in Italia (14 anni [DA VERIFICARE]).
5. Dichiarazione di "trader" su Google Play (DSA) per un'app gratuita di una persona fisica: è dovuta? Quali dati diventano pubblici [N2]?
6. Nome dell'app e marchio: rischi del termine generico "canzoniere"; conviene registrare il marchio?

**Prima del Piano Coro: ≈ 800 €, entro il 31/05/2028**
7. Un server che conserva solo la scaletta (titoli, autori, ordine, tonalità, note brevi, eventuale incipit) può ricadere nell'art. 17 della Direttiva DSM (D.Lgs. 177/2021)? Quali obblighi DSA si applicano (punto di contatto, notifica e azione, motivazione delle decisioni) [RIC §4.2]?
8. Un indice liturgico pubblico con titoli, autori, momenti e incipit è libero?
9. Contratto di licenza diocesana e condizioni del Piano Coro per enti ecclesiastici e parrocchie: chi firma, responsabilità, durata, recesso.
10. Account per i responsabili e membri senza account: obblighi GDPR minimi (registro dei trattamenti, nomina del fornitore di hosting come responsabile, conservazione, cancellazione).
11. Prezzo fondatore e sconti a enti: si applica la norma sul "prezzo più basso dei 30 giorni"?
12. Se arriva un editore: schema di contratto di distribuzione (royalty, rendicontazione, esclusive).

### 9.3 Domande per il commercialista

**Gate 0, un'ora, ≈ 150 €**
1. Con l'attuale situazione lavorativa del proprietario, è possibile il regime forfettario (soglia di reddito da lavoro dipendente, cause di esclusione)? Con quale codice ATECO e quale coefficiente?
2. Contributi: gestione separata INPS o altra cassa? Ci sono minimi fissi?
3. Le lettere d'intento senza incasso creano qualche obbligo? (Ipotesi: no.)
4. Quando va aperta la partita IVA: al primo incasso, o basta la prestazione occasionale per un singolo contratto con una diocesi? Ritenuta d'acconto se paga un ente sostituto d'imposta?
5. Costo annuo realistico di gestione: tenuta, dichiarazione, fatturazione elettronica.

**Prima del Gate 2**
6. Incassi da Google (società irlandese): trattamento IVA, VIES, inversione contabile sulle commissioni, elenchi riepilogativi.
7. Fatture a parrocchie ed enti ecclesiastici senza partita IVA: codice destinatario, marca da bollo di 2 € sopra 77,47 € (Piano Parrocchia).
8. Vendita dello stesso Piano Coro sia in app sia con fattura: effetti fiscali.
9. Nello scenario del §12: le donazioni a una persona fisica per un progetto gratuito come vanno trattate?

---

## 10. Requisiti per lo sviluppo e la CI

Elenco per chi farà la pianificazione tecnica. **R-** = requisito; tra parentesi il livello (v1, v2) e l'origine.

### 10.1 Piattaforme e ordine
- **R-01 (v1)** Solo Android, telefono e tablet, in v1. Windows dopo il Gate 1 e solo se lo chiedono almeno 3 cori. iOS/iPadOS prima di Windows se il censimento rileva più del 40% di iPad (D9).
- **R-02 (v1)** Il tablet Android economico (fascia 100-150 €) è il dispositivo di riferimento per prestazioni e leggibilità; il telefono è il caso d'emergenza [UX].
- **R-03 (v1)** Tutte le scelte devono lasciare aperto iOS: niente API solo Android nel codice condiviso; pedale e tasti dietro un'interfaccia per piattaforma.

### 10.2 Cosa deve funzionare senza backend
- **R-04 (v1)** Tutta la v1 funziona senza rete e senza alcun server dello sviluppatore: libreria, lettura, trasposizione, Palco, pedale, scalette, import PDF e OCR **sul dispositivo**, condivisione con file e QR, backup e ripristino, export.
- **R-05 (v1)** Nessuna chiave API nell'app; nessuna chiamata a servizi esterni in v1. I servizi esterni (AI, OCR in cloud, audio) restano interfacce con feature flag **spenti per default** [VAL §7.6].
- **R-06 (v2)** Con il Piano Coro, se il server non risponde l'app usa l'ultima scaletta ricevuta e non blocca mai la lettura. Il server non conserva mai testi né accordi. Le note sono limitate a una lunghezza breve (es. 280 caratteri) per non diventare un canale di testi.
- **R-07 (v2)** Strumenti Pro: il diritto d'uso si verifica con Play Billing e resta valido senza rete a tempo indeterminato. Un acquisto non verificabile non toglie mai una funzione già usata.

### 10.3 Dati e persistenza
- **R-08 (v1)** Un solo DbContext per la MAUI, un solo set di migrazioni generate da EF, `Migrate()` atteso all'avvio, nessun `EnsureCreated`, nessuna eccezione ingoiata [VAL §4.3].
- **R-09 (v1)** Backup locale automatico **prima** di ogni migrazione; ripristino guidato se la migrazione fallisce.
- **R-10 (v1)** Test di aggiornamento in CI da un database di ogni versione pubblicata alla nuova, su SQLite reale (non EF InMemory).
- **R-11 (v1)** ChordPro è la fonte di verità del canto. Import ed export compatibili con SongbookPro e OnSong, blocchi tab compresi. Formato di backup: archivio con file ChordPro e un manifest JSON (metadati, scalette, raccolte), leggibile anche fuori dall'app.
- **R-12 (v1)** Il QR trasporta solo la scaletta (titoli, autori, ordine, momenti, tonalità, capotasto). I canti viaggiano come file. Chi riceve una scaletta vede subito quali canti gli mancano.

### 10.4 Esperienza d'uso imposta dal piano
- **R-13 (v1)** Modalità Palco: schermo sempre acceso, schermo intero, pedale Bluetooth come tastiera (frecce, pagina su/giù) e tasti del volume, anteprima del canto successivo, ricerca d'emergenza; tutto gratuito.
- **R-14 (v1)** Notazione Do-Re-Mi e C-D-E, diesis/bemolle da preferenza, capotasto, modalità Chitarra / Tastiera / Solo testo.
- **R-15 (v1)** Tutte le stringhe in resx dal primo commit; interfaccia italiana; nomi dei momenti liturgici localizzabili.
- **R-16 (v1)** Tema "Palco" (scuro e chiaro), contrasto WCAG AA come minimo e AAA nella Modalità Palco [UI].

### 10.5 Pubblicazione sugli store e firma
- **R-17 (v1)** Scegliere `ApplicationId` e nome **prima** del primo caricamento (oggi `com.companyname.appcanzoniere.maui` [VAL §4.4]). Su Play il package name è permanente.
- **R-18 (v1)** Formato AAB, Play App Signing; chiave di caricamento nei segreti della CI, mai nel repository; `versionCode` automatico (oggi fisso a 1 [VAL §5]).
- **R-19 (v1)** Livello di API target e compatibilità con pagine di memoria da 16 KB come richiesti da Play al momento del caricamento [DA VERIFICARE in Play Console].
- **R-20 (v1)** Percorso di rilascio: test interno → test chiuso (≥ 12 tester per ≥ 14 giorni, obbligatorio per gli account personali [N1]) → produzione con rilascio graduale (es. 10% → 50% → 100%).
- **R-21 (v1)** La CI rifiuta promozioni in produzione da venerdì a domenica (ora italiana).
- **R-22 (v1)** Scheda dello store: "Sicurezza dei dati" coerente con "nessun dato raccolto", classificazione dei contenuti, URL dell'informativa privacy, screenshot con soli canti propri o di dominio pubblico.
- **R-23 (v2)** Windows: MSIX tramite Microsoft Store (firma dello store), solo quando scatta R-01. iOS: runner macOS in CI, certificati e provisioning nei segreti, solo quando scatta R-01.

### 10.6 Pagamenti
- **R-24 (v1)** Nessuna libreria di pagamento, nessun prezzo e nessun link a pagamenti esterni nell'app.
- **R-25 (v2)** Piano Coro venduto **fuori dall'app**, con fattura; l'app riceve un codice di attivazione e non contiene link o inviti all'acquisto esterno (modello "solo consumo" della policy Play [PRE §4.4, DA VERIFICARE]).
- **R-26 (v2)** Strumenti Pro con Google Play Billing (prodotto una tantum); niente prezzi di lancio né promozioni a tempo.

### 10.7 Privacy
- **R-27 (v1)** Nessun account, nessun SDK di analisi o pubblicità, nessun dato personale inviato allo sviluppatore. Crash e uso si leggono dalle statistiche di Play Console (Android vitals, utenti attivi giornalieri) [DA VERIFICARE la granularità per giorno].
- **R-28 (v1)** Le metriche dei gate si misurano senza dati personali: dispositivi con installazione attiva e utenti attivi la domenica da Play Console; "cori che usano la scaletta" da un registro volontario (modulo sul sito).
- **R-29 (v2)** Piano Coro: account solo per i responsabili adulti; membri con codice, senza account; hosting nell'UE; cancellazione dell'account dall'app e da una pagina web (richiesta da Play per le app con account [DA VERIFICARE]); registro dei trattamenti.

### 10.8 Segnalazioni e assistenza
- **R-30 (v1)** Indirizzo email dedicato e modulo sul sito per le segnalazioni di diritti; risposta entro 72 ore, decisione motivata entro 7 giorni; registro delle segnalazioni. Rimozione di eventuali contenuti contestati nel nucleo di dominio pubblico con la versione successiva.
- **R-31 (v2)** Sul server del Piano Coro: punto di contatto, meccanismo di notifica e azione, motivazione delle decisioni (DSA artt. 11, 12, 16, 17) [RIC §4.2, legale 9.2.7].
- **R-32 (v1)** FAQ e 3 video prima del lancio; versione dell'app e ID anonimo del dispositivo visibili nella schermata "Informazioni", per l'assistenza.

### 10.9 CI (da rifare da zero, piccola) [VAL §5]
- **R-33** `global.json` con SDK fissato e workload set fissato (`dotnet workload restore` con versione).
- **R-34** Job separati: "core" (librerie e test) su Linux con il restore dei soli progetti non MAUI; "android" con workload MAUI.
- **R-35** Test di integrazione su SQLite reale; i test che oggi falliscono non possono stare nel job principale finché non sono corretti.
- **R-36** Controlli bloccanti: `dotnet format --verify-no-changes`, `dotnet list package --vulnerable`, copertura in Cobertura con soglia.
- **R-37** Rilascio su tag: AAB firmato, caricamento sulla traccia interna di Play tramite API (account di servizio nei segreti), promozione manuale.
- **R-38** Restare su FluentAssertions 7.x o passare a un'alternativa con licenza aperta (dalla 8.x la licenza è commerciale) [VAL §4.6].

### 10.10 Pulizia prima di pianificare
- **R-39** Togliere o archiviare i documenti che dichiarano cose false (README, TEST_AND_COMPLETE, KNOWN_ISSUES, FUTURE_IMPLEMENTATIONS, `docs/plan.md`) e correggere CLAUDE.md ("77 passing", "Automatic migrations on startup") [VAL §6]. Le basi della pianificazione restano PROMPT-INIZIALE, VALUTAZIONE e questo documento.
- **R-40** Buttare: migrazioni scritte a mano, migrazione del backend, stub Tesseract, `AdvancedSearchAsync`, `DownloadService`, `OfflineSongRepository`, `OfflineSong`, test vuoti, `MainPage.xaml` del template [VAL §6].

---

## 11. Decisioni del proprietario

In ordine di urgenza. "Blocca" indica cosa non può partire finché la decisione non è presa.

| # | Domanda | Opzioni | Raccomandazione | Blocca | Entro |
|---|---|---|---|---|---|
| **D1** | Qual è l'obiettivo del progetto? | (a) reddito; (b) servizio ai cori con opzione commerciale; (c) hobby o open source | **(b)**. (a) non è realistico: il valore atteso è circa 1 K€ in quasi quattro anni (§8). Se l'obiettivo è (a), raccomando di non commercializzare | Tutto: gate, budget, ore | 16/11/2026 provvisoria; 31/01/2027 definitiva |
| **D2** | Accetti un prodotto diverso dal prompt iniziale? | Sì / No | **Sì**. Tolti catalogo pubblico, AI sugli accordi, riconoscimento audio, scraping, sync multi-cloud, login in v1 (§3) | Copione delle interviste, perimetro, pianificazione tecnica | 16/11/2026 |
| **D3** | Avvii il Gate 0? | Sì, con ≤ 300 € e circa 80 ore / No | **Sì**, subito: l'Avvento (29/11) serve per osservare le Messe | Ogni altra attività | 16/11/2026 |
| **D4** | Quante ore a settimana dichiari per i prossimi 12 mesi? | Numero, poi misurato per 4 settimane | Dichiararle subito e misurarle | Scelta tra calendario principale e lungo | 16/11/2026 (dichiarate), 13/12/2026 (misurate) |
| **D5** | Direzione UX | A Libreria / **B Scaletta + modello Messa** / C Liturgia | **B**, con la ricerca di A; il calendario di C in v2 [UX] | Prototipo per interviste e confronto con SongbookPro | 30/11/2026 |
| **D6** | Modello di ricavo | Pro una tantum subito con quota [STR/PRE] / **gratis al lancio, poi istituzioni** [DUE, §4] / sempre gratis (§12) | **Gratis al lancio, poi licenza diocesana e Piano Coro** | Scheda dello store, pagamenti nel codice, inquadramento fiscale | 31/01/2027 (Gate 0) |
| **D7** | Rinuncia definitiva al catalogo pubblico di testi | Confermare / Rinviare al legale | **Confermare**; resta aperto solo l'accordo con un editore | Architettura (backend sì/no), T&C | 31/01/2027 |
| **D8** | Direzione UI e notazione predefinita | Palco / Leggio / Spartito / ibrido; "Lam" o "La-" | **Palco** per entrambi i temi in v1; Do-Re-Mi con minore "Lam" e opzione "La-" [UI] | Inizio dello sviluppo (feb 2027) | 31/01/2027 |
| **D9** | Ordine delle piattaforme | Android → Windows / Android → iOS / iOS per primo | **Android per primo**; il secondo lo decide il censimento (iPad > 40% → iOS) | Scelte tecniche cross-platform, budget Apple | 31/01/2027 |
| **D10** | Nome dell'app, `ApplicationId`, tipo di account Play | Personale / organizzazione (serve un D-U-N-S) | **Account personale** ora (gratis, 12 tester richiesti); nome scelto dopo una verifica rapida del marchio | Primo caricamento in test chiuso | 28/02/2027 |
| **D11** | Budget e momento del parere legale mirato | ≈ 400 € prima del lancio / niente | **≈ 400 € entro giugno 2027** | Lancio pubblico | 30/04/2027 |
| **D12** | Codice aperto o chiuso | Open source subito (GPL) / chiuso fino al Gate 2 / chiuso sempre | **Chiuso fino al Gate 2**: aprire è irreversibile, chiudere no. Si riapre se D1 = (b) e serve alle diocesi, oppure se si passa al §12 | Licenza del repository, contributi esterni | 30/06/2027 |
| **D13** | Forma giuridica e apertura della partita IVA | Nessuna / prestazione occasionale / partita IVA forfettaria | **Nessuna fino al Gate 2**; poi quella indicata dal commercialista | Incassi, fatture, licenza diocesana | 31/05/2028 |
| **D14** | Avvio del Piano Coro e della licenza diocesana | Sì se passa il Gate 2 / No | Secondo il Gate 2 (≥ 2 K€ impegnati da ≥ 10 pagatori + parere legale) | Backend, account, hosting | 31/05/2028 |
| **D15** | Introduzione degli Strumenti Pro | Sì a 7,99 € / No | Solo se scattano le condizioni del §4.2 | Play Billing nel codice | 01/2029 |
| **D16** | Investimenti successivi: iOS, Windows, inglese, B2B | Per ciascuno | Secondo il Gate 3 | Roadmap 2030 | 30/09/2029 |

---

## 12. Scenario alternativo: non commercializzare (hobby od open source)

**Quando sceglierlo**: D1 = (a) oppure (c); oppure il Gate 0 conferma l'uso ma non la disponibilità a pagare; oppure il Gate 1-bis o il Gate 2 falliscono.

| Elemento del piano | Piano commerciale | Hobby / open source |
|---|---|---|
| Perimetro v1 | §3 | **Quasi uguale.** Catalogo pubblico, AI sugli accordi e scraping restano esclusi: il rischio legale (DSA, diritti) non dipende dallo scopo di lucro. L'art. 17 DSM sì, ma la notifica e azione del DSA no [RIC §4.2]. Audio e AI restano esclusi perché costano senza ricavi |
| Gate 0 | 14 attività | Senza lettere d'intento né commercialista (attività 10 e 12); restano ore, censimento, prove tecniche, interviste ridotte (≥ 8) |
| Calendario | Date fisse, lancio a settembre 2027 | Nessuna data commerciale. Resta solo il traguardo tecnico (un canto dal PDF al leggio), utile per non disperdersi |
| Gate 1, 2, 3 | Soglie economiche | Spariscono. Resta un solo criterio: "la uso io ogni domenica e la usano almeno 3 cori" |
| Piano Coro | Server per le scalette, a pagamento | Nessun server. Coordinamento con file e QR, eventualmente con una cartella condivisa nel cloud dell'animatore (nessun dato presso lo sviluppatore) |
| Ricavi | Licenza diocesana, Coro, Pro | Nessuno. Eventuali donazioni solo dopo il parere del commercialista (domanda 9.3.9) |
| Costi in quasi quattro anni | ≈ 3,7-3,9 K€ nei casi base e favorevole, imposte escluse (§8) | **≈ 0,1-0,3 K€**: account Play 25 $, dominio facoltativo, nessun commercialista, nessun legale (T&C da modello) |
| Ore | ≈ 1.300-1.700 | **≈ 450-550**: v1 (≈ 330, o meno con ritmo libero) più 50-100 l'anno di manutenzione |
| Netto | Valore atteso ≈ +1,0 K€ | ≈ −0,1/−0,3 K€, cioè il costo di un hobby |
| Google Play | Possibile dichiarazione di "trader" con dati pubblici quando si vende [N2] | Probabilmente "non trader" [DA VERIFICARE] |
| Codice | Chiuso fino al Gate 2 (D12) | **Open source GPL-3.0** con il marchio tenuto dal proprietario. Contributi con DCO o CLA, se si vuole lasciarsi aperta la strada per tornare commerciali. Utile per la fiducia di diocesi e movimenti e per non dipendere da una persona sola [DUE §2.6b] |
| Diocesi | Clienti | Possibili adottanti o manutentori: un ufficio liturgico può prendere in carico il progetto |
| Ritorno al commerciale | — | Possibile con un doppio licenziamento se il proprietario ha tenuto i diritti sul codice; il canale diocesano conquistato gratis resta un vantaggio |

**Cosa si guadagna**: circa 800-1.200 ore in meno, nessun rischio fiscale, rischio legale minimo, nessun obbligo verso clienti paganti.
**Cosa si perde**: il valore atteso di circa +1 K€ e la (piccola) probabilità di arrivare a qualche migliaio di euro l'anno con diocesi o un editore. Nessuno dei due numeri è un reddito.

---

## Fonti nuove di questo documento

Consultate il 10/11/2026. Per le altre fonti: [Sn] in `01-RICERCA-MERCATO.md`, [Tn] in `02-STRATEGIA.md`, [Pn] in `03-PREZZI.md`.

- [N1] Google Play Console Help — "App testing requirements for new personal developer accounts" (account personali creati dopo il 13/11/2023: test chiuso con almeno 12 tester per almeno 14 giorni consecutivi prima dell'accesso alla produzione; non si applica agli account di organizzazione): https://support.google.com/googleplay/android-developer/answer/14151465
- [N2] Guida di terze parti sulla dichiarazione dello stato di "trader" in Play Console (DSA): nome, indirizzo, telefono ed email mostrati agli utenti UE per chi si dichiara trader. **Fonte non ufficiale: da verificare in Play Console**: https://www.fdaytalk.com/google-play-trader-status/ ; contesto Apple sullo stesso obbligo DSA: https://www.macrumors.com/2024/10/17/developers-eu-app-store-trader-requirements/
