# AppCanzoniere — Piano per fasi (ripartenza)

Data: 10 novembre 2026 · Commit di partenza: `c5ce11c` (unico commit, 17/02/2026) · Repo: `F1-Solution/AppCanzoniere` (privato, già `origin`)

**Fonti e sigle**
- **[PROMPT]** `docs/PROMPT-INIZIALE.md` — richieste originali del proprietario.
- **[VAL §x]** `docs/VALUTAZIONE.md` — stato reale del codice, verificato eseguendolo.
- **[FP]** `docs/FASI-PRECEDENTI.md` — le 17 fasi del vecchio piano (solo riferimento).
- **[PC §x]** `docs/business/05-PIANO-COMMERCIALE.md` — perimetro v1, gate, requisiti R-xx (§10), decisioni Dx (§11), scenario non commerciale (§12).
- **[UX]** `docs/design/ux/CONFRONTO-UX.md` (+ `UX-B-scaletta.md`) · **[UI]** `docs/design/ui/CONFRONTO-UI.md` (+ `UI-2-palco.md`).

**Avvertenza.** Il proprietario **non ha ancora approvato** nessuna raccomandazione (obiettivo D1, perimetro D2, UX D5, UI D8, piattaforme D9, ecc.). Questo piano segue il percorso raccomandato da [PC], [UX] e [UI]; ogni fase dichiara da quali decisioni dipende. Il §9 spiega come cambia il piano con il perimetro originale del prompt o senza commercializzazione. Le parti marcate **[IPOTESI]** sono scelte di questo documento da confermare; **[DA VERIFICARE]** va controllato su Play Console, GitHub o con un professionista.

---

## 1. Tabella delle fasi

Stati: **completata** · **da iniziare** · **condizionata** (si apre solo se scatta un gate). "Bloccata da" indica ciò che, oggi, impedisce di partire o di chiudere la fase.

| ID | Titolo | Stato | Dipende da | Bloccata da |
|---|---|---|---|---|
| F0 | Ricognizione: valutazione del codice, proposte UX e UI, piano commerciale, collegamento del repo a GitHub | **completata** | — | — |
| G0 | Validazione commerciale (Gate 0, lavoro del proprietario, in parallelo) | da iniziare | F0 | D1 provvisoria, D3 (avvio), D4 (ore dichiarate) |
| F1 | Risanamento del repository e CI verde | da iniziare | F0 | Nulla: può partire subito. Per la build Android **in locale** serve riparare i workload della macchina (azione del proprietario, da amministratore) |
| F2 | Nucleo musicale: ChordPro, trasposizione, notazione, "incolla" | da iniziare | F1 (job "core" della CI) | Nulla. D8 fissa solo i valori predefiniti (Do-Re-Mi, "Lam"/"La-"), modificabili dopo |
| F3 | Prove tecniche del Gate 0: leggio "Palco" su tablet e import da PDF | da iniziare | F1 (job Android), F2 (modello di impaginazione) | Tablet Android economico e pedale Bluetooth; 10 libretti PDF reali (proprietario) |
| F4 | Fondamenta dell'app: persistenza, navigazione, libreria | da iniziare | F1, F2 | Persistenza: nulla (anticipabile prima del Gate 0). Navigazione e UI: esito G0 "procedo", D2, D5, D8 |
| F5 | Leggio e import: primo flusso completo (traguardo T1) | da iniziare | F3, F4 | Esito G0 (prova import, D2), D5, D8 |
| F6 | Scalette della Messa, condivisione, backup ed export | da iniziare | F4, F5 | D5, D7, decisioni UX 3-5 |
| F7 | Canale di rilascio Android: firma, versioni, Play test interno e chiuso | da iniziare | F1 (si sviluppa in parallelo a F5-F6) | D10 (nome, `ApplicationId`, account Play), segreti, account Play verificato |
| F8 | Pilota e prontezza della v1 (Gate Pilota) | da iniziare | F5, F6, F7 | Cori pilota e ≥ 12 tester (G0), D11 (parere legale), dominio ed email per privacy e segnalazioni |
| F9 | Lancio in produzione su Google Play | da iniziare | F8 | Gate Pilota (30/06/2027), D6, D12 |
| F10 | Seconda piattaforma (iPad o Windows) | condizionata | F9 | Gate 1 + D9 (iPad > 40%) oppure ≥ 3 cori che chiedono Windows; per iPad account Apple e runner macOS |
| F11 | Piano Coro e licenza diocesana (server della sola scaletta) | condizionata | F9 | Gate 2, D13, D14, secondo parere legale |
| F12 | Strumenti Pro | condizionata | F11 | D15 e condizioni di [PC §4.2] |
| F13 | Estensioni di contenuto (inglese, accordi per pianoforte, filtri estesi, indice liturgico, calendario) | condizionata | F9 | Gate 1; richieste esplicite dei cori |

Dimensione totale indicativa della v1 (F1-F9): **34 round** (§2.2). Le fasi F10-F13 non sono dimensionate in dettaglio finché il loro gate non scatta.

---

## 2. Come si lavora

### 2.1 Orchestrazione
- Un **agente principale** per round, che lancia **al massimo 2-3 sotto-agenti alla volta**, su file disgiunti:
  - **Opus**: progettazione e ragionamento complesso (modello di impaginazione, schema dati e migrazioni, euristiche di import, pipeline di rilascio).
  - **Sonnet**: scrittura di test e codice.
  - **Haiku**: documentazione (CLAUDE.md, CHANGELOG, note di rilascio, FAQ).
  - **Opus con effort xhigh**: review del codice a fine round. Nessun problema CRITICAL o HIGH aperto alla chiusura.
- **Sviluppo guidato dai test**: prima il test che fallisce, poi il codice, poi il refactoring. Per le parti MAUI non testabili in unità (disegno, eventi tasto) il "test" è una prova scritta su dispositivo, con esito registrato.
- **Il proprietario lavora nel tempo libero**: decide, prova su dispositivo, procura account e segreti. Non scrive codice.

### 2.2 Unità di misura: il round
Un **round** è una sessione di orchestrazione che parte da un obiettivo scritto e finisce con: CI verde, review chiusa, documentazione aggiornata, un commit (o una PR) e una prova dimostrabile. **[IPOTESI]** Un round richiede al proprietario 2-4 ore (brief, decisioni, prova su tablet). Con ≥ 10 h/settimana si fanno 2-3 round a settimana; con 6-8 h, 1-2.

### 2.3 Definizione di "finito", valida per ogni fase
1. CI verde sul commit finale, con tutti i controlli bloccanti attivi in quel momento.
2. Test scritti prima del codice; nessun test vuoto o che controlla solo i valori predefiniti di un POCO [VAL §2.4].
3. Review Opus xhigh senza CRITICAL o HIGH aperti.
4. Per tutto ciò che si vede: **provato su un tablet Android economico reale**; per la Modalità Palco: **usato in una Messa vera** prima del lancio [PC §3.3]. "I test passano" non basta: è l'errore che ha prodotto l'"87%" contro un 25-30% reale [VAL §1].
5. CLAUDE.md e CHANGELOG aggiornati con frasi brevi; nessuna dichiarazione di stato che non sia verificata.

### 2.4 Tracciamento
All'avvio di F1 si creano `ROADMAP.md` (derivato da questo documento) e `PROGRESS.md` con il protocollo `PHASE:<id> WAVE:<id> STATUS:<…> TS:<…>`. Questo documento resta la fonte del perimetro; non li crea.

---

## 3. Gate 0 in parallelo: cosa può partire prima del suo esito

Il Gate 0 (16/11/2026 – 31/01/2027) è lavoro del proprietario in persona [PC §5]. Le fasi tecniche si dividono così:

| Fase | Prima del Gate 0? | Perché non si spreca lavoro |
|---|---|---|
| F1 Risanamento e CI | **Sì, subito** | Serve in ogni scenario, anche con il perimetro originale e anche se il progetto diventa open source. Se il Gate 0 dice "stop", resta un repository pulito e onesto |
| F2 Nucleo musicale | **Sì** | Parser, trasposizione e notazione servono a qualunque versione del prodotto, commerciale o no |
| F3 Prove tecniche | **Sì, è parte del Gate 0** | Producono gli esiti delle attività 5 e 6 di [PC §5]. Il codice di impaginazione e di conversione sta nel nucleo testato e si tiene; la pagina di prova MAUI è usa e getta |
| F4 Persistenza (prima metà) | **Sì, se F1-F3 sono chiuse e restano ore** | Un solo DbContext, migrazioni EF e backup prima delle migrazioni servono in tutti gli scenari [VAL §4.3] |
| F4 Navigazione e UI | No | Dipende da D2, D5 e D8: con un'altra UX la Shell cambia |
| F5 Leggio e import | No (eccetto il controllo di disegno già nato in F3) | L'esito della prova import decide se l'OCR entra in v1 |
| F6 Scalette | No | Esiste solo con la UX "Scaletta" (D5) |
| F7 Canale di rilascio | Solo la parte senza nome: job di firma con una chiave di caricamento e AAB come artefatto | Il caricamento su Play richiede `ApplicationId` e account (D10), che dipendono dall'esito |
| F8, F9 | No | Richiedono cori pilota, parere legale e decisione "procedo" |

Regola: se al 31/01/2027 il Gate 0 dice **fermarsi**, si chiude dopo F1-F3 (≈ 9 round). Se dice **servizio gratuito**, si prosegue con il §9.2. Se dice **procedo**, si prosegue con F4.

---

## 4. Fasi della prima versione (v1)

### F1 — Risanamento del repository e CI verde

**Obiettivo.** Rendere il repository onesto, compilabile ovunque e verificato da una CI verde prima di scrivere qualsiasi funzione.

**Risultato dimostrabile.** Una push su `main` fa partire la CI su GitHub: il job "core" (librerie e test su Linux) e il job "android" (APK di debug come artefatto) finiscono in verde. L'APK scaricato dalla pagina della run si installa su un tablet e l'app si apre.

**Contenuto principale.**
1. Tag `legacy-2026-02` su `c5ce11c`: ogni eliminazione resta recuperabile.
2. `global.json` con SDK fissato; workload set fissato (`dotnet workload` con versione) [PC R-33]; `Directory.Packages.props` per le versioni centralizzate [VAL §4.6].
3. Aggiornamento a 10.0.x più recente; eliminazione delle vulnerabilità High transitive (Microsoft.OpenApi, SQLitePCLRaw) [VAL §4.6]; FluentAssertions fissato a 7.x **[IPOTESI]** (alternativa: AwesomeAssertions) [PC R-38].
4. **[IPOTESI]** Target del progetto MAUI ridotti a `net10.0-android`; il target Windows resta attivabile con una proprietà, solo per il ciclo di sviluppo locale; iOS e MacCatalyst tolti dai TFM (si riaggiungono in F10). Il codice condiviso resta privo di API solo Android [PC R-03].
5. **Backend congelato, non cancellato**: Backend, Backend.Tests e Backend.IntegrationTests escono dalla soluzione principale (soluzione separata), non entrano nella CI obbligatoria. Si cancellano solo dopo D2 = sì e D7 = conferma.
6. Eliminazione del codice morto in ogni scenario [PC R-40, VAL §6]: migrazioni scritte a mano di Infrastructure, `DownloadService`, `OfflineSongRepository`, `OfflineSong`, `AdvancedSearchAsync` e i relativi DTO in `Domain/Interfaces`, stub `TesseractOcrService`, tre `UnitTest1.cs` vuoti, `MainPage.xaml` del template.
7. Pulizia documentale [PC R-39]: README riscritto in breve; TEST_AND_COMPLETE, KNOWN_ISSUES, FUTURE_IMPLEMENTATIONS e `docs/plan.md` archiviati in `docs/archivio/` con un'intestazione "non affidabile, vedi VALUTAZIONE"; CLAUDE.md corretto ("77 passing", "Automatic migrations on startup", stato delle fasi).
8. CI nuova da zero (`.github/workflows/ci.yml`), al posto di `build-and-test.yml` [VAL §5]:
   - job **core** su `ubuntu-latest`: restore e build dei soli progetti non MAUI, test, copertura Cobertura, `dotnet format --verify-no-changes`, `dotnet list package --vulnerable --include-transitive`;
   - job **android** su `ubuntu-latest` con workload MAUI Android fissato e cache NuGet: build Debug, APK come artefatto;
   - action aggiornate a versioni che girano sul runtime Node supportato [VAL §5.9].

**Riuso / rifare / eliminare.** Tiene Domain, Application, Infrastructure (repository), MAUI e i 45 test veri di parser e trasposizione [VAL §6]. Rifà la CI da zero [VAL §5]. Elimina quanto al punto 6 e congela il backend (rotto: Identity su un DbContext senza tabelle utenti, migrazioni mai applicate [VAL §2.5, §4.3]).

**Richieste del prompt coperte.** P34 (base di build Android); P38 (le interfacce dei servizi esterni restano come contratti).

**Dipendenze.** Fasi: F0. Decisioni: nessuna per partire. La **cancellazione** del backend e dei client esterni (Perplexity, ACRCloud, Azure) aspetta D2: finché D2 è aperta restano nel codice, ma fuori da DI, Shell e CI.

**Dal proprietario.**
- Riparare i workload sulla propria macchina da prompt amministratore (`dotnet workload restore` o `update` con la versione fissata in `global.json`) [VAL §2.1].
- Verificare il piano GitHub dell'organizzazione: minuti di Actions per repo privati e disponibilità della protezione del branch `main` (sui repo privati può richiedere un piano a pagamento) [DA VERIFICARE].
- Ordinare tablet Android economico (100-150 €) e pedale Bluetooth (30-60 €), servono in F3.

**Criteri di accettazione.**
- `dotnet build` e `dotnet test` della soluzione principale riescono in locale (dopo la riparazione dei workload) e in CI.
- La CI su `main` è verde in 3 push consecutive; nessun job è "continue-on-error".
- Nessun pacchetto vulnerabile segnalato; `dotnet format` senza differenze.
- Soglia di copertura attiva sul job core: **≥ 80% delle righe** su Domain e Application (si esclude il codice generato).
- L'artefatto APK di debug esiste in ogni run del job android e si installa su un dispositivo reale.
- `grep` dei file archiviati: nessun documento attivo dichiara fasi completate non verificate.

**Rischi.** Workload MAUI su Linux instabile o lento (mitigazione: versione fissata, cache, timeout); minuti di Actions insufficienti su repo privato; aggiornamento a EF 10.0.x che rompe test sui repository (si scopre subito grazie alla CI).

**Dimensione.** 3 round.

---

### F2 — Nucleo musicale: ChordPro, trasposizione, notazione, "incolla"

**Obiettivo.** Avere in una libreria senza dipendenze MAUI, testata a fondo, tutta la logica musicale della v1.

**Risultato dimostrabile.** Un test di accettazione prende un canto in formato "accordi sopra il testo" incollato da un libretto, lo converte in ChordPro, lo traspone di +2 con preferenza bemolle e lo rende in Do-Re-Mi con capotasto 3: il risultato atteso (righe di accordi allineate alle sillabe) compare nel report dei test della CI.

**Contenuto principale.**
1. Parser ChordPro esteso: blocchi `{start_of_tab}`/`{end_of_tab}` mostrati come sono (monospazio, senza trasposizione); direttive comuni di SongbookPro e OnSong (titolo, artista, tonalità, capo, sezioni, commenti); round-trip import → export senza perdite [PC R-11].
2. Trasposizione: preferenza diesis/bemolle, tonalità minori con alterazioni corrette, accordi con basso (`Re/Fa#`), capotasto (tonalità suonata vs tonalità sentita) [VAL §4.2].
3. Notazione: C-D-E e Do-Re-Mi in lettura e scrittura; minore come "Lam" o "La-" da preferenza [UI decisione 2, D8].
4. **Modello di impaginazione** indipendente dalla UI: righe composte da segmenti (accordo, sillaba), con un'interfaccia di misura del testo iniettabile, così che il controllo di disegno di F3/F5 sia solo un "pittore" e la logica di a-capo sia testata in unità.
5. Conversione "accordi sopra il testo" → ChordPro con riconoscimento delle righe di accordi (base per "incolla" e per l'import da PDF) [PC §3.2 riga 19].
6. Correzione del difetto di DI: il rendering non crea un proprio transposer [VAL §3.1 2a].

**Riuso / rifare / eliminare.** Tiene `ChordProParser`, `ChordTransposer` e i 45 test [VAL §4.2, §6]. Rifà il rendering (oggi produce ChordPro grezzo con accordi in linea [VAL §3.1 1a]). Nessuna eliminazione.

**Richieste del prompt coperte.** P01, P02 (visualizzazione tab), P05, P07, base di P21-P23 (conversione testo → ChordPro).

**Dipendenze.** Fasi: F1. Decisioni: D8 solo per i valori predefiniti (Do-Re-Mi, "Lam"); la logica supporta tutte le varianti.

**Dal proprietario.** 10-20 canti reali (libretto della propria parrocchia) come casi di test, con l'esito atteso scritto a mano per almeno 5; risposta a "minore come Lam o La-".

**Criteri di accettazione.**
- Copertura ≥ 90% delle righe sui servizi musicali (sopra la soglia generale).
- Test di round-trip su almeno 10 file ChordPro esportati da SongbookPro o OnSong (o costruiti secondo la loro documentazione).
- Test parametrici di trasposizione su tutte le 12 tonalità maggiori e minori, nelle due notazioni, con e senza capo.
- Conversione "incolla": ≥ 90% delle righe di accordi riconosciute correttamente sui canti forniti dal proprietario.
- CI verde; review senza CRITICAL/HIGH.

**Rischi.** Ambiguità della notazione italiana (es. "Si" vs "Sib", "La-" vs trattino nel testo) che rende fragile la conversione; varianti non standard dei file importati.

**Dimensione.** 3 round.

---

### F3 — Prove tecniche del Gate 0: leggio "Palco" su tablet e import da PDF

**Obiettivo.** Rispondere con dati, entro le date del Gate 0, alle due domande tecniche che possono fermare il progetto: il leggio è fluido su un tablet economico con il pedale? L'import dai libretti reali è abbastanza buono?

**Risultato dimostrabile.**
- **Traccia A (attività 6, entro il 24/01/2027)**: sul tablet economico, un canto di due pagine con accordi sopra il testo nel tema "Palco" scorre in modo fluido e gira pagina con il pedale Bluetooth e con i tasti del volume, a schermo sempre acceso.
- **Traccia B (attività 5, entro il 17/01/2027)**: un report con la percentuale di canti importati da 10 libretti PDF reali con ≤ 2 minuti di correzione ciascuno, separata tra PDF testuali e scansioni.

**Contenuto principale.**
- Traccia A (Opus progetta, Sonnet implementa): controllo dedicato basato su `GraphicsView` con `Microsoft.Maui.Graphics`, una vista per riga, che disegna il modello di F2 [UI-2-palco.md; non una `Label` per sillaba]; gestione dei tasti in `MainActivity` (frecce, Pagina giù, spazio, volume) dietro un'interfaccia di piattaforma [UX-B]; `KeepScreenOn`; misura di tempi di disegno e scorrimento su un canto lungo.
- Traccia B (Opus progetta l'euristica, Sonnet il banco di prova): estrazione da PDF testuale **[IPOTESI: PdfPig, licenza Apache 2.0]**; OCR sul dispositivo **[IPOTESI: riconoscimento testo di Google ML Kit tramite binding .NET; alternativa Tesseract]**; conversione con F2; un banco di prova da riga di comando che produce il report. Nessuna chiamata a servizi in cloud [PC R-04, R-05].

**Riuso / rifare / eliminare.** Riusa il modello e la conversione di F2. La pagina di prova MAUI è usa e getta, ma il controllo di disegno, se supera la review, passa in F5. Si conferma l'eliminazione dei mock OCR e degli stub Tesseract e Azure come base dell'import [VAL §3.1 6a].

**Richieste del prompt coperte.** P21, P22 (solo come prova), base di P01 e P06.

**Dipendenze.** Fasi: F1 (job Android), F2 (modello di impaginazione e conversione). Decisioni: nessuna; UX decisione 3 (pedale in v1) di fatto viene anticipata dalla prova.

**Dal proprietario.** Tablet Android economico e pedale Bluetooth (emulazione tastiera); 10 libretti PDF reali (fonti in [PC §5, RIC S42]); prova in una chiesa poco illuminata; misura dei minuti di correzione per canto (cronometro).

**Criteri di accettazione.**
- Traccia A: nessun frame perso visibile allo scorrimento (registrazione video dello schermo); il pedale gira pagina senza configurazioni dopo l'accoppiamento; il controllo di disegno ha test unitari sul modello e un test di disegno che non va in errore (CI).
- Traccia B: il report applica le soglie di [PC §5]: ≥ 70% → OCR in v1; 50-70% → v1 solo PDF testuale e "incolla"; < 50% → import fuori dalla v1.
- APK della prova prodotto dalla CI, non da una build locale.

**Rischi.** Prestazioni di `GraphicsView` su tablet economici; binding ML Kit per .NET 10 non aggiornato (ripiego: Tesseract con dati linguistici italiani, più lento); pedali che inviano codici tasto diversi.

**Dimensione.** 3 round (le due tracce in parallelo, 2 sotto-agenti Sonnet + 1 Opus).

---

### F4 — Fondamenta dell'app: persistenza, navigazione, libreria

**Obiettivo.** Avere un'app Android affidabile in cui si creano, modificano, cercano e ritrovano i canti, con dati che sopravvivono a riavvii e aggiornamenti.

**Risultato dimostrabile.** Sul tablet: si crea un canto (incollando il testo), lo si apre dalla ricerca per titolo o incipit, si modificano tonalità, momenti della Messa, riferimenti biblici e tag; si chiude e si riapre l'app; si installa una build più nuova con una migrazione: i dati ci sono ancora e il backup pre-migrazione è stato creato.

**Contenuto principale.**
1. **Persistenza** (anticipabile prima del Gate 0): un solo DbContext per la MAUI; un solo set di migrazioni generate da EF; `Migrate()` atteso all'avvio; niente `EnsureCreated`; nessuna eccezione ingoiata; seed dei momenti liturgici idempotente; **backup automatico prima di ogni migrazione** e ripristino guidato se fallisce [PC R-08, R-09]; DbContext a vita breve (factory) al posto delle registrazioni singleton [VAL §4.3].
2. Test su **SQLite reale** (in memoria e su file), non EF InMemory; test di aggiornamento da database fittizi di ogni versione futura pubblicata [PC R-10].
3. **[IPOTESI]** ViewModel spostati in una libreria `net10.0` senza dipendenze MAUI (navigazione e dialoghi dietro interfacce), così da testarli nel job core su Linux.
4. Navigazione Shell secondo la UX "Scaletta": sezioni Canti, Scalette (vuota fino a F6), Profilo; passaggio dei parametri con `IQueryAttributable`; nessuna route duplicata; icone presenti [VAL §4.4].
5. Libreria: CRUD canti (correzione del salvataggio di un canto nuovo [VAL §3.2]); ricerca per titolo, autore, incipit e tag; metadati (lingua, tipo, tonalità originale, link YouTube che apre l'app esterna, momenti, riferimenti biblici solo come riferimento, stato legale dichiarato); repertori (album manuali con ordinamento).
6. Impostazioni con enum al posto delle stringhe magiche: notazione, diesis/bemolle, modalità di lettura, lingua [VAL §4.1].
7. Infrastruttura: tutte le stringhe in resx dal primo commit, interfaccia italiana, nomi dei momenti localizzabili [PC R-15]; tema "Palco" scuro e chiaro come dizionario di risorse con token (cambiare direzione UI costa poco) [PC R-16]; meccanismo di feature flag, con AI, OCR in cloud e audio **spenti e non registrati** [PC R-05].
8. Eliminazione dei warning MVVMTK0045 e delle chiamate `DisplayAlert` obsolete nel codice che resta [VAL §2.3].

**Riuso / rifare / eliminare.** Tiene Domain (entità ed enum), `SongRepository`/`AlbumRepository` per il CRUD di base, `SongMetadataViewModel` e la gestione di tag, momenti e riferimenti biblici, `DatabaseSeeder`, struttura MVVM/DI [VAL §6]. Rifà persistenza, migrazioni, navigazione, salvataggio, impostazioni. Elimina dalla Shell e dalla DI le pagine di AI, OCR mock, audio, cloud sync, canzoni pubbliche e login; il loro codice si cancella se D2 = sì, altrimenti resta fuori dalla build in attesa (ed è comunque nel tag `legacy-2026-02`).

**Richieste del prompt coperte.** P08, P12, P13, P16 (metadati), P17, P18, P24, P26 (offline per costruzione), P27 (stato legale per canto), P29 (infrastruttura e italiano), P30, P31, P37 (meccanismo).

**Dipendenze.** Fasi: F1, F2. Decisioni: punti 1-2 nessuna; punti 3-8 esito G0 "procedo" (o §9.2), D2, D5, D8.

**Dal proprietario.** Conferma di D5 e D8 (anche provvisoria); 20-30 canti propri per popolare il tablet; prova di un aggiornamento su dispositivo.

**Criteri di accettazione.**
- Test di migrazione su SQLite reale in CI: database vuoto → ultima versione; database con dati → ultima versione senza perdite; migrazione che fallisce → ripristino del backup.
- Test dei ViewModel per apertura, creazione, modifica, ricerca (job core).
- Un controllo in CI fallisce se un file XAML contiene testo visibile non preso dalle risorse **[IPOTESI: script semplice su attributi `Text`/`Title`]**.
- Nessun warning MVVMTK0045 o CS0618 nel progetto MAUI; build Android in CI verde.
- Prova su tablet: creazione, riapertura dopo riavvio, aggiornamento con migrazione.

**Rischi.** Migrazioni su dati reali che si corrompono (rischio 3 di [PC §9]); refactoring dei ViewModel più ampio del previsto; D5 o D8 cambiano dopo l'inizio (mitigazione: token del tema e Shell piccola).

**Dimensione.** 5 round (2 anticipabili prima del Gate 0).

---

### F5 — Leggio e import: primo flusso completo (traguardo T1)

**Obiettivo.** Portare un canto vero da un PDF vero al leggio, trasposto e suonato con il pedale.

**Risultato dimostrabile.** **Traguardo T1** di [PC §6.1]: sul tablet economico si importa un canto dal libretto PDF della parrocchia, lo si corregge nella revisione affiancata, lo si traspone in Do-Re-Mi con capotasto e lo si suona in Modalità Palco girando pagina con il pedale. Obiettivo: **28/02/2027**; ripiego di [PC §6.1]: senza OCR, entro il 15/04/2027.

**Contenuto principale.**
1. Schermata canzone di produzione con il controllo `GraphicsView` nato in F3: accordi sopra il testo, ritornello evidenziato (UI decisione 3), tonalità originale sempre visibile, trasposizione, capotasto, modalità **Chitarra / Tastiera / Solo testo** [PC R-14].
2. **Modalità Palco**: schermo intero, schermo sempre acceso, pedale e tasti del volume, contrasto AAA, controlli da 56 dp [PC R-13, R-16; UI-2-palco.md]. Due pagine affiancate su tablet orizzontale solo se la prova F3 lo regge **[IPOTESI: altrimenti a scorrimento in v1]**.
3. **Import**: da PDF testuale e, se il Gate 0 lo consente, OCR sul dispositivo; un canto o una pagina alla volta; revisione affiancata (immagine/testo e ChordPro); **dichiarazione non preselezionata** "ho i diritti o è per uso personale" a ogni import [PC §3.2 righe 18, 23].
4. **Incolla** testo con accordi, con la conversione di F2.
5. Blocchi tab mostrati in monospazio.

**Riuso / rifare / eliminare.** Riusa il controllo di disegno e la gestione tasti di F3, il nucleo F2, `IOcrService` come contratto [VAL §4.2]. Rifà la pagina canzone (oggi ChordPro grezzo, selettore strumento senza comando [VAL §3.1 1a, 1c]). Elimina `AiChordSuggestionPage` e la pagina OCR mock se D2 = sì.

**Richieste del prompt coperte.** P01, P02, P05, P06, P07, P21, P22 (se il Gate 0 lo consente), P23 sostituita da "incolla", P27 (dichiarazione all'import).

**Dipendenze.** Fasi: F3, F4. Decisioni: esito G0 (soglia dell'import, D2), D5, D8, UI decisioni 3 e 5 (ritornello, barra di navigazione), UX decisione 3 (pedale in v1).

**Dal proprietario.** Prova in chiesa con poca luce; 5 canti reali importati e cronometrati; pedale di riferimento confermato.

**Criteri di accettazione.**
- T1 dimostrato su tablet, con video dello schermo.
- Test di unità sull'import (fixture PDF testuali in `tests/`, risultato atteso in ChordPro); test dei ViewModel di revisione e lettura.
- Contrasto AAA nella Modalità Palco verificato sui token del tema (test che calcola i rapporti di contrasto).
- L'app non fa alcuna richiesta di rete durante import e lettura (verifica con modalità aereo su dispositivo).
- CI verde; review senza CRITICAL/HIGH.

**Rischi.** Data T1 stretta: richiede che la persistenza di F4 sia partita prima del Gate 0 **[IPOTESI]**; qualità OCR sotto soglia; impaginazione a due pagine costosa [UX-B].

**Dimensione.** 5 round.

---

### F6 — Scalette della Messa, condivisione, backup ed export

**Obiettivo.** Rendere l'app il leggio della domenica: preparare la Messa per momenti, suonarla in sequenza, passarla al coro e non perdere mai i dati.

**Risultato dimostrabile.** L'animatore crea dal modello "Messa" la scaletta di domenica (ingresso, offertorio, comunione, finale), con tonalità e strumento "della sera"; la condivide con un QR a un secondo tablet, che vede subito quali canti gli mancano e li riceve come file; in Palco si passa da un canto al successivo con il pedale, con l'anteprima del successivo. Un backup completo si salva su file e si ripristina su un dispositivo vuoto.

**Contenuto principale.**
1. Scalette con data e **modello Messa** a momenti, scalette libere, separatori, **scelte della sera** (tonalità, capotasto, strumento per evento) [UX-B; PC §3.3].
2. In Palco: anteprima del successivo, ricerca d'emergenza, scheda "Pronta per il palco?" [UX-B].
3. Filtri per momento, tonalità e "non usato di recente" [PC §3.2 righe 21, 28].
4. **Condivisione**: QR che trasporta solo la scaletta (titoli, autori, ordine, momenti, tonalità, capotasto); canti e raccolte come file ChordPro tramite il foglio di condivisione di Android; dichiarazione non preselezionata all'invio [PC R-12].
5. **Backup e ripristino**: archivio con file ChordPro e manifest JSON, leggibile fuori dall'app, salvabile ovunque dal selettore di sistema (anche in una cartella di Drive) [PC R-11; §3.2 righe 9, 10].
6. **Export** ChordPro di canto, raccolta e scaletta.

**Riuso / rifare / eliminare.** Riusa `AlbumRepository`, `AlbumDetailViewModel` (aggiunta, rimozione, ordinamento) come motore comune di scalette e repertori [VAL §3.1 7a]. Rifà `CloudSyncService` come backup su file, senza copiare il `.db` aperto e senza dipendenze "prigioniere" [VAL §4.3]. Elimina `LocalStorageProvider` e i provider cloud commentati se D2 = sì.

**Richieste del prompt coperte.** P10 (sostituita dal backup su file), P11, P16 (scaletta per momenti), P24, P25 (sostituita da filtri), P26, P27 (dichiarazione all'invio), P32 (momento, tonalità), P33.

**Dipendenze.** Fasi: F4, F5. Decisioni: D5 (esiste solo con la UX "Scaletta"), D7 (nessun backend: condivisione senza server), UX decisioni 4 e 5 (condivisione nel gruppo, niente account).

**Dal proprietario.** Due dispositivi Android per la prova del QR; una scaletta reale di una domenica; prova d'uso nella propria parrocchia.

**Criteri di accettazione.**
- Test di round-trip backup → ripristino su SQLite reale (dati identici, conteggi e contenuti).
- Test del formato QR (dimensione massima per una scaletta di 15 voci; codifica e decodifica).
- Test di export/import ChordPro compatibili con F2.
- Prova su due dispositivi in modalità aereo, salvo il trasferimento dei file.
- CI verde; review senza CRITICAL/HIGH.

**Rischi.** Dimensione del QR per scalette lunghe (ripiego: più QR o file); confusione tra scaletta condivisa e canti privati (testo esplicativo prima dell'invio [UX-B]).

**Dimensione.** 5 round.

---

### F7 — Canale di rilascio Android: firma, versioni, Play test interno e chiuso

**Obiettivo.** Pubblicare ogni versione esclusivamente tramite CI: AAB firmato, caricato sulla traccia di test di Google Play, promosso a mano.

**Risultato dimostrabile.** Un tag `v0.x.y` su `main` produce in CI un AAB firmato con `versionCode` automatico e lo carica sulla traccia interna di Play; il proprietario lo promuove al test chiuso dalla Console e lo installa dal Play Store sul tablet.

**Contenuto principale.**
1. Nome e `ApplicationId` definitivi al posto di `com.companyname.appcanzoniere.maui` [VAL §4.4, PC R-17].
2. AAB in Release, Play App Signing, chiave di caricamento solo nei segreti della CI [PC R-18]; `versionCode` dal numero di run o dal tag, `ApplicationDisplayVersion` dal tag [VAL §5.8].
3. Livello di API target e compatibilità con pagine di memoria da 16 KB come richiesti da Play al momento del caricamento [PC R-19, DA VERIFICARE].
4. Job di rilascio su tag: caricamento sulla traccia interna tramite API di Play con account di servizio [PC R-37]; ambiente GitHub `play` con approvazione manuale; promozioni successive manuali.
5. Blocco delle promozioni in produzione da venerdì a domenica (ora italiana) [PC R-21] (attivo da F9).
6. **[IPOTESI]** Prova di fumo su emulatore Android in CI: installazione, avvio a freddo, apertura di un canto, nessun crash nel log.
7. Note di rilascio generate in italiano (Haiku) dal CHANGELOG.

**Riuso / rifare / eliminare.** Nulla da riusare: il vecchio workflow non firma e cerca `*.apk` [VAL §5.5]. Elimina i job iOS/macOS/Windows del vecchio workflow [VAL §5.6-5.7].

**Richieste del prompt coperte.** P34 (pubblicazione Android).

**Dipendenze.** Fasi: F1; si sviluppa in parallelo a F5-F6 (file diversi). Decisioni: **D10** (nome, `ApplicationId`, account personale o di organizzazione) entro il 28/02/2027; esito G0 "procedo" o §9.2.

**Dal proprietario.**
- Account sviluppatore Google Play (25 $), verifica d'identità completata [DA VERIFICARE i tempi]; per un account di organizzazione serve un D-U-N-S.
- Chiave di caricamento generata e custodita (copia in un gestore di password) e segreti GitHub: `ANDROID_KEYSTORE_BASE64`, `ANDROID_KEYSTORE_PASSWORD`, `ANDROID_KEY_ALIAS`, `ANDROID_KEY_PASSWORD`.
- Progetto Google Cloud con account di servizio autorizzato su Play Console; JSON nel segreto `PLAY_SERVICE_ACCOUNT_JSON`.
- Creazione dell'app in Play Console e **primo caricamento manuale** dell'AAB, se Play lo richiede prima dell'uso dell'API [DA VERIFICARE].
- Elenco degli indirizzi Google dei tester (≥ 12) per il test chiuso.

**Criteri di accettazione.**
- Il job di rilascio fallisce se manca un segreto, se il `versionCode` non cresce, se la firma non è valida.
- Nessuna chiave o password nel repository (scansione dei segreti attiva).
- Un AAB caricato da CI è installabile dal Play Store sul tablet.
- La prova di fumo su emulatore passa a ogni push su `main`.

**Rischi.** Verifica dell'account Play lenta; `ApplicationId` permanente dopo il primo caricamento; politiche di Play cambiate rispetto alle ipotesi [PC §9.1 rischio 9].

**Dimensione.** 3 round.

---

### F8 — Pilota e prontezza della v1 (Gate Pilota)

**Obiettivo.** Portare la v1 nelle mani di 5 cori pilota da Pasqua 2027 e renderla pubblicabile: legale, misurabile, senza perdite di dati.

**Risultato dimostrabile.** Al 30/06/2027 i dati di Play Console mostrano ≥ 12 tester attivi per ≥ 14 giorni consecutivi e sessioni senza crash ≥ 99,5%; almeno 3 cori usano l'app ≥ 3 domeniche su 4 per 6 settimane; l'accesso alla produzione è concesso [PC §6.1].

**Contenuto principale.**
1. Test chiuso con i cori pilota; correzioni settimanali; nessun aggiornamento da venerdì a domenica.
2. Schermate legali: Termini e condizioni, informativa privacy (app senza account e senza raccolta dati), stato legale dei canti, dichiarazioni già introdotte in F5 e F6 [PC §3.2 righe 23-24].
3. Nucleo di 20-40 canti di dominio pubblico verificati, inclusi nell'app, con la documentazione della verifica [PC §3.3].
4. Schermata "Informazioni": versione e ID anonimo del dispositivo per l'assistenza; link a FAQ e alla procedura di segnalazione dei diritti [PC R-30, R-32].
5. Verifica dei feature flag: AI, OCR in cloud e audio spenti; nessuna libreria di pagamento, analisi o pubblicità [PC R-24, R-27].
6. Test di aggiornamento in CI dal database della prima versione di test alla corrente [PC R-10]; prove di prestazione sul tablet di riferimento [PC R-02].
7. Revisione di accessibilità (contrasto, dimensioni dei controlli, lettore di schermo sui flussi principali).

**Riuso / rifare / eliminare.** Riusa `CopyrightStatus` del dominio, finalmente impostabile dalla UI [VAL §3.1 9]. Crea il resto. Se D2 = sì, a questo punto non restano nel codice client esterni né mock fuori uso.

**Richieste del prompt coperte.** P26 (verifica offline completa), P27, P28, P37 (verifica), P34.

**Dipendenze.** Fasi: F5, F6, F7. Decisioni: G0 (5 cori pilota, ≥ 12 persone), **D11** (parere legale ≈ 400 € entro il 30/06/2027), D12 (licenza del codice, da citare nei T&C).

**Dal proprietario.** Gestione dei cori pilota e dell'ambasciatore; parere legale sulle 6 domande di [PC §9.2]; dominio, sito minimo con informativa privacy e modulo di segnalazione; email dedicata; ricerca dei canti di dominio pubblico (con l'ambasciatore); raccolta delle note settimanali.

**Criteri di accettazione.**
- Soglie del Gate Pilota di [PC §6.1], misurate su Play Console.
- **Zero perdite di dati** segnalate; ogni segnalazione blocca il rilascio successivo finché non è risolta [PC §9.1 rischio 3].
- Scheda "Sicurezza dei dati" compilata coerente con "nessun dato raccolto" [PC R-22].
- CI: test di aggiornamento tra versioni pubblicate verdi; nessuna dipendenza di rete nei test di avvio.

**Rischi.** Cori pilota che abbandonano; minori nei cori giovani [PC §9.1 rischio 11]; parere legale che chiede modifiche ai flussi di import.

**Dimensione.** 5 round, distribuiti su aprile-giugno 2027.

---

### F9 — Lancio in produzione su Google Play

**Obiettivo.** Pubblicare la v1 gratuita su Google Play con un rilascio graduale e un'assistenza sostenibile.

**Risultato dimostrabile.** Entro il 19/09/2027 l'app è in produzione su Google Play con la scheda in italiano; la promozione al 100% è avvenuta per gradi (10% → 50% → 100%) dalla CI, mai nel fine settimana.

**Contenuto principale.** Scheda dello store (parole chiave, screenshot con soli canti propri o di dominio pubblico, frase "suonare sarà sempre gratis") [PC R-22, §7.3]; classificazione dei contenuti; promozione in produzione con rilascio graduale e blocco del fine settimana [PC R-20, R-21]; monitoraggio di Android vitals; FAQ e link ai 3 video (prodotti dal proprietario); procedura di segnalazione dei diritti operativa [PC R-30].

**Riuso / rifare / eliminare.** Riusa la pipeline di F7. Nulla da eliminare.

**Richieste del prompt coperte.** P34.

**Dipendenze.** Fasi: F8. Decisioni: Gate Pilota superato; D6 (gratis al lancio); D12 (codice chiuso o aperto). Con 1-2 soglie mancate, lancio in Avvento (28/11/2027) [PC §6.1].

**Dal proprietario.** Testi della scheda e video; dichiarazione di "trader" secondo il parere legale [PC §9.2 domanda 5]; presenza in 1-2 corsi diocesani.

**Criteri di accettazione.** Accesso alla produzione ottenuto; rilascio graduale completato senza regressioni negli Android vitals; il job di promozione rifiuta l'esecuzione da venerdì a domenica (test del job); tag e note di rilascio pubblicati.

**Rischi.** Segnalazione di un editore [PC §9.1 rischio 4]; assistenza della domenica mattina [rischio 13].

**Dimensione.** 2 round.

---

## 5. Fasi successive alla v1 (condizionate)

Si aprono solo se scatta il gate indicato; i dettagli si scrivono quando il gate passa. Per ciascuna: obiettivo, condizione, contenuto, riuso, proprietario, criteri di massima, dimensione indicativa.

| ID | Obiettivo | Si apre quando | Contenuto principale | Riuso dal codice esistente | Dal proprietario | Criteri di accettazione di massima | Round |
|---|---|---|---|---|---|---|---|
| **F10** Seconda piattaforma | Portare il leggio su iPad **oppure** su Windows | Gate 1 (09/01/2028) e: iPad > 40% nel censimento (D9) → iPadOS; ≥ 3 cori che lo chiedono → Windows [PC R-01, R-23] | TFM aggiunto; gestione tasti per piattaforma; job CI dedicato con firma (runner macOS e provisioning per iOS; MSIX tramite Microsoft Store per Windows) | Codice condiviso di F2-F6; la UI MAUI compilava su Windows in copia ridotta [VAL §2.3] | iOS: account Apple Developer (99 $/anno), certificati nei segreti; Windows: account Microsoft Store | Stesso flusso T1 sulla nuova piattaforma; pubblicazione solo via CI | 5-6 |
| **F11** Piano Coro e licenza diocesana | Sincronizzare **solo la scaletta** tra i dispositivi di un coro, a pagamento con fattura | Gate 2 (31/05/2028): ≥ 2 K€/anno impegnati da ≥ 10 pagatori, parere legale, partita IVA (D13, D14) | Backend nuovo e piccolo (mai testi né accordi sul server, note ≤ 280 caratteri) [PC R-06]; account solo per i responsabili adulti, membri con codice [PC R-29]; codici di attivazione senza link d'acquisto [PC R-25]; obblighi DSA [PC R-31]; hosting UE; deploy via CI | Dal backend congelato solo idee e pezzi isolati (JWT, `SecureTokenStorage`); Identity, migrazioni e autorizzazioni vanno rifatte [VAL §4.3, §4.5] | Hosting, dominio API, segreti JWT, contratti, secondo parere legale (≈ 800 €) | L'app legge l'ultima scaletta ricevuta senza rete; nessun testo nel DB del server (test); test di integrazione su DB reale verdi | 8-10 |
| **F12** Strumenti Pro | Vendere a 7,99 € funzioni nuove che fanno risparmiare tempo | D15 e condizioni di [PC §4.2] (Gate 2 passato, ≥ 3 cori le chiedono) | Import in blocco del libretto, modelli di scaletta, stampa ed export PDF, backup automatico in una cartella scelta, allegati PDF/MP3; Play Billing con diritto valido senza rete [PC R-07, R-26] | Entità `Mp3Attachment`, `PdfAttachment` [VAL §3.1 4] | Profilo pagamenti Play, partita IVA | Nessuna funzione già usata si blocca senza rete (test) | 5-6 |
| **F13** Estensioni di contenuto | Ampliare il pubblico e la preparazione liturgica | Gate 1 e richieste esplicite | Interfaccia in inglese; note degli accordi per pianoforte; filtri per tipo, lingua, riferimento biblico; raccolte automatiche a regole locali (senza AI in cloud); indice liturgico pubblico senza testi; calendario liturgico (UX "C") | Infrastruttura resx di F4; `AlbumType.AIGenerated` come tipo di raccolta automatica | Risposte a UX decisione 2 (fonte del calendario, rito) | Per voce, definiti all'apertura | 3-8 secondo la scelta |

---

## 6. Matrice delle richieste del prompt iniziale

Destinazioni: **F#** = fase della v1; **v2 (F#)** = rinviata a dopo la prima versione; **Tolta** = fuori dal perimetro raccomandato, con il motivo. Tutte le "Tolte" dipendono da D2 (§9.1).

| # | Richiesta [PROMPT] | Destinazione | Motivo / nota |
|---|---|---|---|
| P01 | Testi e accordi leggibili (ChordPro) | F2, F5 | Cuore del leggio; accordi sopra il testo con `GraphicsView` |
| P02 | Tablature per chitarra (visualizzazione) | F2, F5 | Blocchi tab mostrati in monospazio, per non rompere i file importati [PC §3.2 riga 2] |
| P03 | Trasposizione delle tab | **Tolta** | Rara nella liturgia; costo alto [PC §3.2 riga 2] |
| P04 | Accordi per pianoforte (note dell'accordo) | v2 (F13) | In v1 la modalità "Tastiera" mostra le sigle senza capotasto [PC §3.2 riga 3] |
| P05 | Trasposizione su/giù | F2, F5 | Logica esistente e testata [VAL §3.1 2a] |
| P06 | Scelta chitarra/pianoforte | F5 | Come modalità Chitarra / Tastiera / Solo testo |
| P07 | Notazione Do-Re-Mi o C-D-E, diesis/bemolle | F2, F4 | Differenza rispetto ai concorrenti generalisti |
| P08 | DB privato locale SQLite | F4 | Persistenza rifatta [VAL §4.3] |
| P09 | DB pubblico/condiviso, canzoni pubbliche/private | **Tolta** (D7) | Rischio legale asimmetrico (DSA, art. 17 DSM, editori); sostituita da condivisione file/QR (F6), canti di dominio pubblico inclusi (F8), indice senza testi (v2, F13) [PC §3.2 riga 8] |
| P10 | Sync del DB privato su Dropbox/Drive/OneDrive | **Tolta** come integrazione; F6 backup su file; v2 (F12) backup automatico | Tre integrazioni OAuth costano molto; il selettore di file dà quasi lo stesso valore [PC §3.2 riga 9] |
| P11 | Export/import del DB o di un sottoinsieme | F6 | Archivio ChordPro + manifest JSON [PC R-11] |
| P12 | Metadati: titolo, autori, lingua, tonalità, tipo | F4 | UI di lingua e tipo da creare [VAL §3.1 4] |
| P13 | Link YouTube | F4 | Link che apre l'app esterna |
| P14 | MP3 (file o URL) | v2 (F12) | Pesano nei backup, servono poco la domenica |
| P15 | PDF associati | v2 (F12) | Come P14 |
| P16 | Momenti della Messa | F4 (metadati), F6 (scaletta per momenti) | Posizionamento del prodotto |
| P17 | Riferimenti biblici | F4 (solo il riferimento); filtro in v2 (F13) | I testi CEI sono protetti [PC §3.2 riga 15] |
| P18 | Tag generici | F4 | Esistenti [VAL §3.2] |
| P19 | Riconoscimento audio | **Tolta** | Costo senza valore distintivo; repertorio liturgico poco presente nei database [PC §3.2 riga 16] |
| P20 | Accordi suggeriti dall'AI con avviso | **Tolta** | Copyright; funzione già offerta dai grandi; modello Perplexity dismesso [PC §3.2 riga 17, VAL §4.2] |
| P21 | Import da PDF testuale | F3 (prova), F5 | Sul dispositivo, un canto alla volta |
| P22 | OCR di PDF scansionati | F3 (prova), F5 se la soglia del Gate 0 lo consente; import in blocco v2 (F12) | Funzione simbolo e la più rischiosa [PC §3.2 riga 18] |
| P23 | Scraping privato da siti di accordi | **Tolta**; sostituita da "incolla" (F2, F5) | Termini d'uso dei siti; l'incolla copre l'uso legittimo [PC §3.2 riga 19] |
| P24 | Album manuali con ordinamento | F4 (repertori), F6 (scalette) | Esistenti [VAL §3.1 7a] |
| P25 | Album automatici con l'AI | **Tolta** in forma AI; F6 filtri "non usato di recente"; v2 (F13) regole locali | Niente AI in cloud sui testi [PC §3.2 riga 21] |
| P26 | Offline: tutto, raccolta, canzone | F4 (per costruzione), verifica in F8 | Tutto è locale; il codice di download si butta (F1) |
| P27 | Copyright: avvisi, T&C, dichiarazione all'upload, stato legale, tracciamento dell'uploader | F4 (stato legale), F5 (dichiarazione all'import), F6 (all'invio), F8 (T&C, segnalazioni); tracciamento dell'uploader v2 (F11) | Senza server non c'è un uploader da tracciare [PC §3.2 riga 23] |
| P28 | Privacy dei dati personali | F8 | Nessun account e nessun dato raccolto in v1 |
| P29 | Multilingua IT/EN, lingua delle canzoni | F4 (resx, italiano, `Song.Language`); inglese v2 (F13) | Estrarre le stringhe dopo costa di più [VAL §7.9] |
| P30 | CRUD canzoni | F4 | Navigazione e salvataggio da rifare [VAL §3.2] |
| P31 | Ricerca per titolo, autore, tag | F4 (+ incipit) | Esistente [VAL §3.2] |
| P32 | Filtri per tonalità, tipo, lingua, momento, riferimento biblico | F6 (momento, tonalità); v2 (F13) gli altri | `AdvancedSearchAsync` eliminato in F1 [VAL §3.2] |
| P33 | Export di canzone e album (JSON/ChordPro) | F6 | ChordPro; JSON nel manifest del backup |
| P34 | Android (telefono e tablet) | F1, F7, F8, F9 | Unica piattaforma della v1 [PC R-01] |
| P35 | Windows desktop | v2 (F10), condizionata | Solo se ≥ 3 cori lo chiedono dopo il Gate 1 |
| P36 | iOS/macOS (non prioritario nel prompt) | v2 (F10), condizionata | Prima di Windows se il censimento dà iPad > 40% (D9) |
| P37 | Funzioni attivabili/disattivabili per contesto legale | F4 (meccanismo), F8 (verifica) | Feature flag spenti per default [PC R-05] |
| P38 | Interfacce dei servizi esterni (`IAudioRecognitionService`, `IChordSuggestionService`, `IOcrService`, `ICloudSyncService`) | F1 (conservate come contratti); `IOcrService` usata in F5 | Le implementazioni HTTP in Application violano la Clean Architecture e si tolgono con D2 [VAL §4.2] |
| P39 | Account e login (citati nel prompt per privacy e uploader) | **Tolta** in v1; v2 (F11) solo per i responsabili | Ogni account porta obblighi GDPR e assistenza [PC §3.2 riga 29] |

Le richieste del prompt sul **formato della risposta** (domande di chiarimento, architettura, modello dati, flussi UX, interfacce, esempi di codice) sono coperte dai documenti già prodotti in F0 e da questo piano, non da fasi di sviluppo.

---

## 7. Mappa dalle 17 fasi vecchie alle nuove

| Vecchia fase [FP] | Stato dichiarato → reale [VAL] | Dove finisce |
|---|---|---|
| 1 Setup e architettura | Completa → struttura valida, build fragile (workload, nessun `global.json`) | **F1** (build riproducibile) |
| 2 Modello dati e database | Completa → Domain buono; migrazioni rotte | Domain tenuto; persistenza in **F4** |
| 3 UI base e navigazione | Completa → bug di navigazione e route duplicate | **F4** (Shell secondo UX "Scaletta") |
| 4 Funzionalità core canzoni | Parziale → parser e trasposizione buoni; rendering grezzo; salvataggio rotto | **F2** (nucleo), **F4** (CRUD), **F5** (rendering) |
| 5 Album e raccolte | Completa → funzionante | **F4** (repertori), **F6** (scalette) |
| 6 Tag e metadati | Completa → UI e DB ok, filtri avanzati no | **F4** (metadati), **F6** (filtri) |
| 7 Servizi esterni AI | Completa → solo mock; modello dismesso | **Tolta** (D2); riapribile nel §9.1 |
| 8 Servizi esterni OCR | Completa → solo mock; Tesseract stub | **F3** (prova), **F5** (import sul dispositivo) |
| 9 Audio recognition | Completa/roadmap → solo mock, microfono assente | **Tolta** (D2); riapribile nel §9.1 |
| 10 Sync cloud multi-provider | Parziale → solo cartella locale | **F6** (backup su file); provider **tolti** |
| 11 Database pubblico e backend | Completa → backend che risponde 500 | **Congelato in F1**; eventuale backend nuovo e piccolo in **F11** |
| 12 Testing backend | Completa → test con mock che nascondono il bug | Esce dalla CI con il backend (F1) |
| 13 Client MAUI per il backend | Completa → URL sbagliati, backend rotto | **Tolta** in v1 (login e canzoni pubbliche) |
| 14 Testing e refinement | 40% → integration test rossi, nessuna copertura | **F1** (CI e copertura), **F4** (test su SQLite reale) |
| 15 Modalità offline | 70% → codice morto | Offline **per costruzione** (F4); codice eliminato in **F1** |
| 16 Funzionalità avanzate | 25% → ricerca avanzata con errore int→Guid | **F6** (filtri), **F4/F5** (tema scuro "Palco"); raccolte AI tolte |
| 17 Production deployment | "Infrastruttura pronta" → CI mai verde | **F1** (CI), **F7** (firma e Play), **F9** (produzione) |

---

## 8. Calendario di riferimento

Calendario principale di [PC §6.1] (≥ 10 h/settimana misurate); con 8-10 h vale il calendario lungo di [PC §6.2] e tutto slitta di circa 2-5 mesi.

| Periodo | Tecnica (round) | Proprietario |
|---|---|---|
| 16/11/2026 – 31/01/2027 | F1 (3), F2 (3), F3 (3); se resta tempo, persistenza di F4 (2) | Gate 0: ore, censimento, Messe, interviste, cori pilota, commercialista |
| Febbraio 2027 | F4 (resto, 3), F5 (5) → **T1 il 28/02/2027** | D5, D8, D10 |
| Marzo – aprile 2027 | F6 (5) in parallelo a F7 (3) | Account Play, segreti, tester; uso a Pasqua (28/03/2027) |
| Aprile – giugno 2027 | F8 (5) | Cori pilota, parere legale → **Gate Pilota il 30/06/2027** |
| Luglio – settembre 2027 | F9 (2) | Scheda, video, FAQ → **lancio entro il 19/09/2027** |

**[IPOTESI]** Febbraio richiede 8 round in 4 settimane: è realistico solo se la persistenza di F4 parte prima del Gate 0. Altrimenti vale il ripiego di [PC §6.1] (T1 al 15/04/2027).

---

## 9. Come cambia il piano con decisioni diverse del proprietario

### 9.1 Il proprietario mantiene il perimetro originale del prompt (D2 = no)

Catalogo pubblico, backend con account, AI per gli accordi, riconoscimento audio, sync su cloud multi-provider, raccolte AI, Windows desktop.

**Cosa non cambia.** F1, F2, F3 e la persistenza di F4 restano identiche: per questo sono le fasi sicure da avviare prima del Gate 0. Anche F5 (leggio, import) e F7 (canale di rilascio) restano valide.

**Cosa cambia nelle fasi esistenti.**
- **F1**: il backend non si congela; si rimette nella CI in un job separato appena F-B (sotto) lo ripara.
- **F4**: aggiunge login, `SecureTokenStorage`, flag pubblico/privato e tracciamento dell'uploader; la Shell riprende le sezioni "Pubbliche" e "Account".
- **F6**: la condivisione passa dal server; il backup resta.
- **F8**: parere legale più ampio (≈ 2 K€ secondo [PC §4.1]); informativa con account, cancellazione dell'account dall'app e dal web, registro dei trattamenti, gestione dei minori; DSA (punto di contatto, notifica e azione).

**Fasi aggiuntive** (da inserire tra F6 e F8):

| ID | Titolo | Contenuto | Riuso [VAL] | Dal proprietario | Round |
|---|---|---|---|---|---|
| F-B | Backend e catalogo pubblico | DbContext di Identity separato, migrazioni rigenerate e applicate all'avvio, ruoli creati, ownership, nessuna chiave JWT di fallback, CORS ristretto, validazione, limiti di frequenza, paginazione; pubblicazione in 3 passi con dichiarazione non preselezionata; moderazione; notifica e azione DSA; deploy via CI in container su hosting UE; test di integrazione su DB reale | Controller e DTO come traccia; `SecureTokenStorage`; `AuthService` da correggere (URL per piattaforma, refresh token) [VAL §3.2, §4.5] | Hosting, dominio, segreti di produzione, procedura di moderazione | 8-10 |
| F-S | Servizi esterni tramite il backend | Accordi AI (modello attuale, chiave solo sul server [PC R-05]), riconoscimento audio con registrazione dal microfono e permessi Android, OCR in cloud opzionale; limiti di costo per utente; avvisi di copyright | Interfacce e DTO; client ACRCloud e Azure da spostare in Infrastructure; client Perplexity da riscrivere [VAL §4.2] | Contratti e chiavi (Perplexity o altro LLM, ACRCloud, Azure), budget mensile | 5-6 |
| F-C | Sync su cloud dell'utente | Provider OneDrive, Google Drive, Dropbox con OAuth; risoluzione dei conflitti; merge | Interfacce `ICloudStorageProvider`/`ICloudSyncService` [VAL §4.2] | Registrazione delle app presso i tre provider; verifica OAuth di Google per gli scope di Drive [DA VERIFICARE] | 5-6 |
| F-D | Raccolte AI e Windows desktop | Agente che propone raccolte; target Windows con MSIX e firma | `AlbumType.AIGenerated`; build Windows già riuscita in copia [VAL §2.3] | Account Microsoft Store | 4-5 |

**Effetto complessivo**: circa +25-30 round; lancio spostato di 6-9 mesi (non prima della primavera 2028) **[IPOTESI]**; costi ricorrenti (hosting, API a consumo) prima di qualsiasi ricavo; rischio legale più alto [PC §2.6]. Lo **scraping resta escluso** anche qui: il prompt stesso chiede di rispettare i termini d'uso, e "incolla" copre l'uso legittimo. Il Gate 0 va ripensato (copione delle interviste con catalogo e AI).

### 9.2 Il proprietario sceglie di non commercializzare (hobby od open source, [PC §12])

- **F1-F6 identiche.** Il perimetro v1 resta quasi uguale: il rischio legale del catalogo non dipende dallo scopo di lucro [PC §12].
- **F7**: identica (anche un account personale gratuito richiede 12 tester per 14 giorni). Se il repository diventa pubblico, i minuti di Actions per i repo pubblici non sono un vincolo [DA VERIFICARE sul piano dell'organizzazione].
- **F8**: senza parere legale a pagamento (T&C da modello), senza soglie economiche; il criterio diventa "la uso io ogni domenica e la usano almeno 3 cori". Si aggiungono: licenza GPL-3.0 (D12), CONTRIBUTING con DCO, scansione della cronologia per segreti prima di rendere pubblico il repo (la valutazione non ne ha trovati nei file tracciati [VAL §4.5]).
- **F9**: nessuna data commerciale; dichiarazione "non trader" se confermata [PC §12].
- **F10-F12 spariscono**; F13 resta aperta a contributi esterni o a un ufficio liturgico che adotti il progetto.
- Risparmio: circa 3-4 round nella v1 e tutte le fasi v2 a pagamento.

### 9.3 Il Gate 0 dice "fermarsi"
Si chiude dopo F1-F3. Resta un repository pulito, con CI verde e un nucleo musicale testato, riapribile in futuro. Lavoro speso: ≈ 9 round.

---

## 10. Ordine consigliato dei prossimi tre round

| Round | Fase | Contenuto | Sotto-agenti | Risultato | Dal proprietario prima di iniziare |
|---|---|---|---|---|---|
| **1** | F1 | Tag `legacy-2026-02`, creazione di `ROADMAP.md` e `PROGRESS.md`; `global.json`, workload fissato, `Directory.Packages.props`, aggiornamento delle dipendenze; backend fuori dalla soluzione principale; eliminazione del codice morto; job CI "core" | 2 Sonnet (build e dipendenze; CI core), 1 Haiku (archivio documenti, CLAUDE.md corretto); review Opus xhigh | Job core verde su `main` | D1 provvisoria e D3 (16/11/2026); verificare il piano GitHub (minuti, protezione del branch); riparare i workload in locale |
| **2** | F1 + F2 | Job "android" con APK di debug; controlli bloccanti (format, vulnerabilità, copertura ≥ 80%). In parallelo inizia F2: notazione, preferenza bemolle, capotasto, blocchi tab (test prima) | 1 Sonnet (CI android), 1 Sonnet (F2 TDD), 1 Opus (progetto del modello di impaginazione); review Opus xhigh | CI completa verde; APK installato sul tablet | Tablet e pedale ordinati; 10-20 canti reali come casi di test |
| **3** | F2 + F3 | Chiusura di F2 (impaginazione, conversione "incolla"); avvio di F3 con le due tracce: controllo `GraphicsView` e gestione del pedale; banco di prova dell'import | 2 Sonnet (traccia A, traccia B), 1 Opus (euristica di import e verifica del modello); review Opus xhigh | Primo canto disegnato sul tablet; primo report parziale dell'import | 10 libretti PDF reali; risposta su "Lam" o "La-" |
