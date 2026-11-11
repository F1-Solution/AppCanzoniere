# AppCanzoniere — Piano per fasi (ripartenza)

Data: 11 novembre 2026 (sostituisce la versione del 10/11/2026) · Commit di partenza: `c5ce11c` · Repo: `F1-Solution/AppCanzoniere` (privato, già `origin`)

**Fonti e sigle**
- **[PROMPT]** `docs/PROMPT-INIZIALE.md` — richieste originali del proprietario.
- **[VAL §x]** `docs/VALUTAZIONE.md` — stato reale del codice, verificato eseguendolo.
- **[FP]** `docs/FASI-PRECEDENTI.md` — le 17 fasi del vecchio piano (solo riferimento).
- **[PC §x]** `docs/business/05-PIANO-COMMERCIALE.md` — gate, requisiti R-xx (§10), decisioni Dx (§11).
- **[UX-A]**, **[UX-B]**, **[UX-C]** `docs/design/ux/UX-*.md` e **[UX]** `CONFRONTO-UX.md`.
- **[UI-1]** Leggio, **[UI-2]** Palco, **[UI-3]** Spartito, **[UI-4]** Vetrata, **[UI-5]** Analogico in `docs/design/ui/`, e **[UI]** `CONFRONTO-UI.md`.
- **[DEC n]** decisione n del proprietario dell'11/11/2026 (§0).

**Avvertenza.** Questo piano segue le decisioni del proprietario dell'11/11/2026, che prevalgono su ogni raccomandazione di [PC], [UX] e [UI]. Le parti marcate **[IPOTESI]** sono scelte di questo documento da confermare. **[DA VERIFICARE]** e i codici **V-xx** (§6) indicano ciò che va controllato fuori dal repository: store, reti pubblicitarie, GitHub, commercialista, legale. Questo documento non li risolve.

---

## 0. Decisioni del proprietario dell'11/11/2026

| # | Decisione | Effetto sul piano |
|---|---|---|
| DEC 1 | Obiettivo: servizio ai cori che si autofinanzia, con opzione commerciale verso parrocchie e diocesi (= D1 (b) di [PC]) | v1 con ricavi propri dal lancio; Piano Coro e licenza diocesana restano fasi condizionate (F17) |
| DEC 2 | Più di 15 ore a settimana (D4) | Circa 2 round a settimana fino al 31/01/2027 (con il Gate 0), poi circa 3 (§2.2) |
| DEC 3 | Perimetro v1 ridotto rispetto al prompt: niente backend, account, catalogo pubblico, accordi via AI, riconoscimento audio, scraping, sync multi-cloud (D2, D7) | Il backend resta congelato; i client esterni escono dal codice. Le sezioni "perimetro originale" del piano di ieri sono tolte |
| DEC 4 | Android **e** Windows insieme fin da subito (D9) | Due piattaforme in ogni fase con UI; job Windows in CI da F1 |
| DEC 5 | Round tecnici e Gate 0 in parallelo; validazione anche con l'app in mano ai cori (D3) | Nuovo **Gate Anteprima** con l'app installata ai cori (F14) |
| DEC 6 | Tre modalità UX (Libreria, Scaletta, Liturgia) nella v1, scelte nelle impostazioni del dispositivo (D5) | Un solo modello dati e tre navigazioni (F6, F8, F10) |
| DEC 7 | Calendario liturgico completo nella v1: calcolato sul dispositivo, offline, rito romano con il calendario proprio dell'Italia; solo riferimenti delle letture, mai i testi. Ambrosiano solo dopo, se richiesto | Nuova fase F9 |
| DEC 8 | Cinque temi (Leggio, Palco, Spartito, Vetrata, Analogico), chiaro e scuro, scelti nelle impostazioni; predefiniti Leggio chiaro e Palco scuro; un unico sistema di token; prove automatiche con confronto di schermate in CI (D8) | Nuove fasi F4 e F5 |
| DEC 9 | Notazione predefinita legata alla lingua: Do-Re-Mi in italiano con minore "Lam", C-D-E in inglese; sempre modificabile (D8) | F2, F6 |
| DEC 10 | Ricavi: app gratuita con pubblicità e donazioni volontarie; **Pro a 4,99 € una tantum** che toglie la pubblicità; acquisti in app su Google Play e Microsoft Store al lancio; consenso per la pubblicità; posizione fiscale chiarita con un commercialista prima del lancio (D6, D13) | Nuove fasi F11 e F12 |
| DEC 11 | Pubblicità: banner solo nelle schermate di elenco e gestione, più un annuncio all'apertura dell'app (formato "apertura app" di AdMob, da verificare), non mostrato se l'app riparte con una scaletta in corso; mai su canto, Palco o scaletta aperta. Su Windows nessuna pubblicità se non c'è una rete utilizzabile | F11; V-01, V-02 |
| DEC 12 | Donazioni: acquisti facoltativi nell'app con il sistema dello store, più una pagina sul sito (dominio e servizio di pagamento esterno, nella fase di lancio); nessun collegamento dall'app alla pagina esterna finché le regole degli store non sono verificate | F12, F15; V-03 |
| DEC 13 | Windows: pacchetto scaricabile dalle release di GitHub durante il pilota, Microsoft Store al lancio | F13; V-04, V-08 |
| DEC 14 | Design: la **taste-skill** si usa da qui in avanti per specificare token e componenti (da installare a cura del proprietario); le proposte esistenti non si rifanno | F4 |
| DEC 15 | Metodo: da F1 rami con pull request, unione solo con CI verde; 2-3 sotto-agenti alla volta (Haiku documentazione, Sonnet codice, Opus ragionamento, Opus xhigh review); sviluppo guidato dai test; test e pubblicazione tramite CI | §2 |

**Decisioni ancora aperte** (numerazione di [PC §11]): **D10** nome, `ApplicationId`, tipo di account Play e account Partner Center (entro il 28/02/2027); **D11** parere legale, ora più ampio (pubblicità, consenso, acquisti, minori); **D12** codice aperto o chiuso, che ora decide anche **dove si pubblicano le release Windows del pilota** (un repo privato non le rende scaricabili ai cori, V-08); **D13** forma fiscale, ora **prima del lancio**; D14-D16 restano dopo la v1.

### 0.1 Punti in cui le decisioni si discostano dal piano commerciale

| Tema | Piano commerciale [PC] | Decisione | Conseguenze |
|---|---|---|---|
| Pubblicità | Esclusa per sempre ([PC §4.2] "regole permanenti"; R-27: nessun SDK di pubblicità) | Banner e annuncio all'apertura (DEC 10-11) | Scheda "Sicurezza dei dati" non più "nessun dato raccolto"; informativa privacy e consenso nel SEE (V-06); parere legale più ampio; rischio di annunci inadatti al contesto liturgico e di freddezza delle diocesi, che sono il canale principale [PC §7]; le pubblicità richiedono rete, l'app no: in chiesa senza campo l'incasso è nullo |
| Pagamenti e partita IVA | Nessuna libreria di pagamento in v1 (R-24); nessuna partita IVA e nessuna spesa ricorrente prima del Gate 2 (D13) | Acquisti in app e pubblicità al lancio (DEC 10) | Profilo pagamenti su Play e Partner Center; probabile partita IVA **prima del lancio** e commercialista annuale (circa 600 €/anno secondo [PC §8.1]) già dall'Anno 1 [DA VERIFICARE, V-05]; probabile dichiarazione di "trader" (DSA) con nome, indirizzo e telefono pubblici su entrambi gli store [PC rischio 9]. Lo scenario prudente del conto economico di [PC §8] peggiora: va ricalcolato (non in questo documento) |
| Pro | 7,99 € dopo il Gate 2 (2029), solo funzioni nuove, mai un limite | 4,99 € una tantum al lancio, toglie la pubblicità | Nessuna funzione è bloccata: "suonare sarà sempre gratis" resta vero se la pubblicità non tocca mai canto e Palco. Senza account il Pro **vale per un solo store**: chi lo compra su Android lo ricompra su Windows. Il prezzo di eventuali funzioni a pagamento future (F18) va deciso di nuovo |
| Donazioni | Solo nello scenario non commerciale, dopo il commercialista [PC §12, §9.3 domanda 9] | Acquisti facoltativi nell'app e pagina sul sito | Dominio, sito e servizio di pagamento esterno anticipati alla fase di lancio; trattamento fiscale delle donazioni a una persona fisica da chiarire (V-05) |
| Piattaforme | Solo Android in v1; Windows dopo il Gate 1 se lo chiedono ≥ 3 cori (R-01, R-23) | Android e Windows insieme (DEC 4, 13) | Job Windows in CI; prove su un PC Windows in ogni round con UI; firma e distribuzione del pacchetto pilota (V-08); account Partner Center e certificazione dello Store (V-04); i minuti Actions di Windows costano di più su un repo privato [DA VERIFICARE, V-09] |
| UX | Solo B "Scaletta" + modello Messa; calendario liturgico in v2 o oltre [PC §3.3, D5] | A, B e C, calendario completo in v1 (DEC 6-7) | Il costo di C è "ALTO" [UX-C §7]: motore del calendario, tabella dei riferimenti delle letture, suggerimenti, storico. Nuova domanda per il legale sulla tabella dei riferimenti (V-11) |
| UI | Solo "Palco", chiaro e scuro; l'ibrido con Leggio "costa un secondo sistema di temi, in v2"; Vetrata "non compatibile con la v1" [PC §3.4] | Cinque temi, chiaro e scuro, con un unico sistema di token (DEC 8) | Dieci combinazioni da provare su due piattaforme: servono le prove visive in CI (F5). Vetrata diventa realizzabile perché il calendario entra in v1 |
| Gate 0 | Nessun codice di prodotto prima del 31/01/2027 [PC §5] | Round tecnici in parallelo (DEC 5) | Se il Gate 0 dice "fermarsi", il lavoro tecnico fatto fino ad allora (≈ 16 round) è in parte perso. Il piano lo limita a parti riusabili (§3) |
| Gate Pilota e date | Gate Pilota 30/06/2027; lancio entro 19/09/2027; Gate 1 09/01/2028; Gate 2 31/05/2028 | — | Gate Pilota **24/10/2027**, lancio **entro il 21/11/2027**, Gate 1 **30/04/2028**, Gate 2 **30/11/2028** (§1.2). Il pilota completo non si può misurare d'estate: i cori sono fermi |
| Metriche dei gate | Solo da Play Console e da un registro volontario, senza SDK (R-27, R-28) | Restano senza SDK di analisi | Si aggiungono le metriche di incasso (pubblicità, Pro, donazioni) dalle console degli store e di AdMob; l'autofinanziamento entra nel Gate 1 [IPOTESI, §1.2] |
| Lettere d'intento (Gate 0, attività 10) | Condizione per il percorso commerciale | Utili solo per l'opzione diocesana (F17) | Il loro esito non ferma più la v1 |

---

## 1. Tabella delle fasi

Stati: **completata** · **in corso** · **da iniziare** · **condizionata** (si apre solo se scatta un gate). "Bloccata da" indica ciò che oggi impedisce di partire o di chiudere la fase. "Round" è la dimensione stimata (§2.2).

| ID | Titolo | Stato | Dipende da | Bloccata da | Round |
|---|---|---|---|---|---|
| F0 | Ricognizione: valutazione del codice, proposte UX e UI, piano commerciale, collegamento a GitHub | **completata** | — | — | — |
| G0 | Validazione commerciale e d'uso (Gate 0, lavoro del proprietario, in parallelo) | da iniziare | F0 | Avvio entro il 16/11/2026; copione delle interviste aggiornato a tre modalità, pubblicità e Pro | — (≈ 80 h del proprietario) |
| F1 | Risanamento del repository e CI verde (build Android e Windows) | **in corso** (ramo `f1/risanamento-ci`, PR) | F0 | Protezione del branch e minuti Actions sul piano GitHub (V-09); workload della macchina locale (proprietario) | 4 |
| F2 | Nucleo musicale: ChordPro, trasposizione, notazione legata alla lingua, "incolla" | da iniziare | F1 (job core) | Nulla | 3 |
| F3 | Prove tecniche del Gate 0: leggio su tablet Android e su Windows, import da PDF | da iniziare | F1 (job Android e Windows), F2 | Tablet economico, pedale Bluetooth, PC Windows di prova; 10 libretti PDF reali | 4 |
| F4 | Sistema di token, cinque temi e componenti | da iniziare | F1 | Installazione della taste-skill (proprietario) | 5 |
| F5 | Prove visive automatiche (confronto di schermate) in CI | da iniziare | F1, F4 | Cattura su emulatore Android e su Windows nei runner (V-09) | 3 |
| F6 | Fondamenta: persistenza, impostazioni sul dispositivo, navigazione a tre modalità, libreria | da iniziare | F1, F2; F4 per la UI | Persistenza: nulla. UI: i due temi predefiniti di F4 | 6 |
| F7 | Leggio e import su Android e Windows (traguardo T1) | da iniziare | F3, F6 | Esito della prova import (G0) | 6 |
| F8 | Scalette, repertori, Palco in sequenza, condivisione, backup ed export | da iniziare | F6, F7 | Nulla oltre le dipendenze | 6 |
| F9 | Calendario liturgico: motore e riferimenti delle letture | da iniziare | F1, F2 (formato dei riferimenti biblici) | Calendari ufficiali di riferimento per i test (proprietario); V-11 | 6 |
| F10 | Modalità Liturgia: celebrazioni, slot, suggerimenti locali, storico | da iniziare | F8, F9 | Nulla oltre le dipendenze | 4 |
| F11 | Pubblicità e consenso | da iniziare | F6, F13 (identificativo dell'app) | Account AdMob; V-01, V-02, V-06, V-07 | 3 |
| F12 | Acquisti in app: Pro e donazioni | da iniziare | F6, F13 | Profilo pagamenti Play, account Partner Center; V-03, V-04, V-05, V-07 | 4 |
| F13 | Canali di rilascio: Google Play; Windows su GitHub, poi Microsoft Store | da iniziare | F1 (in parallelo a F7-F10) | D10, account Play e Partner Center, segreti; V-04, V-08, V-10 | 5 |
| F14 | Anteprima ai cori e pilota (Gate Anteprima e Gate Pilota) | da iniziare | Anteprima: F7, F8, F13. Pilota: anche F10, F11, F12 | Cori e ≥ 12 tester (G0); D11 parere legale; V-05; dominio ed email | 6 |
| F15 | Lancio: Google Play, Microsoft Store, sito con pagina di donazione | da iniziare | F14 | Gate Pilota; D12; posizione fiscale definita; V-03 | 3 |
| F16 | iPad / iOS | condizionata | F15 | Gate 1 e censimento (iPad > 40%); account Apple e runner macOS | 5-6 |
| F17 | Piano Coro e licenza diocesana (server della sola scaletta) | condizionata | F15 | Gate 2, D13, D14, secondo parere legale | 8-10 |
| F18 | Nuove funzioni a pagamento | condizionata | F17 | D15 e richieste dei cori | 4-6 |
| F19 | Estensioni di contenuto (rito ambrosiano, note per pianoforte, allegati, pianificatore a settimane, indice pubblico senza testi) | condizionata | F15 | Gate 1 e richieste esplicite | 3-10 secondo la scelta |

### 1.1 Dimensione della v1 rispetto al piano di ieri

| | Piano del 10/11/2026 | Questo piano | Differenza |
|---|---|---|---|
| Round della v1 | 34 (F1-F9) | **68** (F1-F15) + **8 di riserva** | **+34 round (+100%)**; +42 (+124%) con la riserva |
| Ore del proprietario per round [IPOTESI] | 2-4 h | 4-6 h (prove su tablet **e** PC Windows, revisione di PR) | Circa 270-410 h di round fino al lancio, più ≈ 80 h di Gate 0 e ≈ 100 h di pilota e preparazione del lancio |
| Fasi della v1 | 9 | 15 | Nuove: token e temi (F4), prove visive (F5), calendario (F9), Liturgia (F10), pubblicità (F11), acquisti (F12). Ampliate: F1, F3, F6, F7, F8, F13 (Windows), F14, F15 |

Il carico medio (≈ 9-11 h/settimana da qui al lancio [IPOTESI]) sta sotto le 15 h dichiarate. Il percorso si allunga meno di quanto cresce il lavoro perché le ore sono di più: da aprile il vincolo non sono più le ore ma il calendario dei cori (§1.2). La riserva di 8 round sta nei mesi estivi.

### 1.2 Gate e traguardi: nuove date

| Tappa | Piano di ieri | Nuova data | Scarto | Cosa si misura |
|---|---|---|---|---|
| **Gate 0** | 31/01/2027 | **31/01/2027** | invariato | Attività di [PC §5], adattate (§3) |
| **T1** — un canto dal PDF al leggio, trasposto in Do-Re-Mi, suonato con il pedale | 28/02/2027, solo Android | **21/03/2027**, tablet Android **e** PC Windows | +3 settimane | Video dello schermo su entrambe le piattaforme |
| **Gate Anteprima** (nuovo, DEC 5) | — | Anteprima dal **02/05/2027**; gate il **20/06/2027** | nuovo | ≥ 3 cori usano l'app ≥ 3 domeniche su 4 per 6 settimane (modalità Libreria e Scaletta, Liturgia appena pronta); 0 perdite di dati; ≥ 99,5% di sessioni senza crash; test chiuso Play con ≥ 12 tester per ≥ 14 giorni → richiesta di accesso alla produzione |
| **Gate Pilota** | 30/06/2027 | **24/10/2027** (pilota completo dal 12/09/2027) | +16 settimane | Soglie di [PC §6.1] con l'app completa (pubblicità, consenso, acquisti in prova), più: nessun avviso di policy AdMob o Play; acquisti provati da capo a fondo su entrambi gli store; parere legale ricevuto; posizione fiscale definita |
| **Lancio** | entro 19/09/2027 | **entro il 21/11/2027** (Cristo Re), prima dell'Avvento (28/11/2027) | +9 settimane | Produzione su Play con rilascio graduale; pubblicazione sul Microsoft Store |
| **Gate 1** | 09/01/2028 (misurato in Avvento) | **30/04/2028** (misurato in Quaresima, 01/03-09/04/2028) | +16 settimane | Soglie d'uso di [PC §6.1] più **autofinanziamento** [IPOTESI]: ricavi netti degli ultimi 3 mesi, riportati all'anno, ≥ costi fissi annui (commercialista, dominio, account) |
| **Gate 2** (opzione diocesana) | 31/05/2028 | **30/11/2028** | +26 settimane | Soglie di [PC §6.1]; apre F17 |

Ripiego: se il Gate Pilota manca 1-2 soglie, il lancio passa alla Quaresima 2028 (entro il 27/02/2028) e il Gate 1 al 30/09/2028. Non si lancia d'estate né tra Avvento e Natale.

---

## 2. Come si lavora

### 2.1 Orchestrazione e rami
- **Un ramo e una pull request per round** (DEC 15), con nome `f<n>/<argomento>`. Si unisce a `main` solo con CI verde e review chiusa. La protezione del branch `main` (controlli obbligatori) nasce in F1 [DA VERIFICARE la disponibilità sul piano GitHub, V-09].
- Un agente principale per round, con **al massimo 2-3 sotto-agenti alla volta** su file disgiunti:
  - **Opus**: progettazione e ragionamento (impaginazione, schema dati e migrazioni, motore del calendario, euristiche di import, regole di posizionamento della pubblicità, pipeline di rilascio).
  - **Sonnet**: test e codice.
  - **Haiku**: documentazione (CLAUDE.md, CHANGELOG, note di rilascio, FAQ, traduzioni delle risorse).
  - **Opus con effort xhigh**: review a fine round; nessun CRITICAL o HIGH aperto alla chiusura.
- **taste-skill** (DEC 14): specifica di token e componenti in F4; ogni nuovo componente dopo F4 passa dalla stessa specifica. Le proposte UI e UX esistenti sono l'input, non si rifanno.
- **Sviluppo guidato dai test**: prima il test che fallisce, poi il codice, poi il refactoring. Per ciò che non si testa in unità (disegno, eventi tasto, store) il "test" è una prova su dispositivo con esito scritto nella PR, più il confronto di schermate di F5.
- **Il proprietario** decide, prova su tablet Android e PC Windows, procura account e segreti. Non scrive codice.

### 2.2 Unità di misura: il round
Un **round** parte da un obiettivo scritto e finisce con: PR unita con CI verde, review chiusa, documentazione aggiornata, una prova dimostrabile. **[IPOTESI]** Un round richiede al proprietario 4-6 ore (brief, decisioni, prova su due piattaforme, lettura della PR). Con più di 15 h/settimana: circa **2 round a settimana** fino al 31/01/2027 (il Gate 0 assorbe ≈ 7 h/settimana; 1 round a settimana tra il 21/12 e il 06/01), poi circa **3 round a settimana**, meno nella Settimana Santa e in agosto.

### 2.3 Definizione di "finito", valida per ogni fase
1. CI verde sulla PR e su `main` dopo l'unione, con tutti i controlli bloccanti attivi in quel momento.
2. Test scritti prima del codice; nessun test vuoto o che controlla solo i valori predefiniti di un POCO [VAL §2.4].
3. Review Opus xhigh senza CRITICAL o HIGH aperti.
4. Per tutto ciò che si vede: **provato su un tablet Android economico e su un PC Windows reali**, nei due temi predefiniti; le schermate di riferimento di F5 si aggiornano solo con un cambio voluto e motivato nella PR.
5. Per Palco e Liturgia: **usati in una Messa vera** prima del lancio [PC §3.3]. "I test passano" non basta [VAL §1].
6. Mai pubblicità su canto, Palco, scaletta aperta o celebrazione (controllo automatico da F11 in poi).
7. CLAUDE.md e CHANGELOG aggiornati con frasi brevi; nessuna dichiarazione di stato non verificata.

### 2.4 Tracciamento
`ROADMAP.md` e `PROGRESS.md` nascono con F1 (protocollo `PHASE:<id> WAVE:<id> STATUS:<…> TS:<…>`) e vanno riallineati a questa versione al primo round dopo F1. Questo documento resta la fonte del perimetro e non li modifica.

---

## 3. Gate 0 in parallelo e validazione con l'app in mano

Il Gate 0 (16/11/2026 – 31/01/2027) resta lavoro del proprietario [PC §5], con questi adattamenti:
- **Interviste** (attività 4): si mostrano le tre wireframe UX e un esempio di schermata con banner; si chiede se la pubblicità nelle schermate di elenco è accettabile e se il Pro a 4,99 € interessa. Nessun incasso.
- **Censimento** (attività 2): si aggiunge una domanda sull'uso di un PC Windows per preparare le scalette.
- **Commercialista** (attività 12): diventa obbligatoria per la v1 (pubblicità, acquisti, donazioni, partita IVA, V-05).
- **Lettere d'intento e uffici liturgici** (attività 9-10): servono all'opzione diocesana (F17), non alla v1.
- **Esito**: "procedo" oppure "mi fermo" (regola di [PC §5]: ore, uso del dispositivo, cori pilota, prova Palco). Il ramo "servizio gratuito" non c'è più: il modello di ricavo è deciso (DEC 10).

| Fase | Prima del 31/01/2027? | Perché il lavoro non si spreca |
|---|---|---|
| F1 Risanamento e CI | **Sì, in corso** | Serve in ogni esito |
| F2 Nucleo musicale | **Sì** | Libreria pura, riusabile |
| F3 Prove tecniche | **Sì, è parte del Gate 0** | Dà gli esiti delle attività 5 e 6; il disegno e la conversione restano nel nucleo |
| F4 Token, prima parte (specifica, Leggio chiaro, Palco scuro) | **Sì** | Serve a ogni schermata; gli altri otto dizionari dopo il Gate 0 |
| F6 Persistenza | **Sì, se restano round** | Serve in ogni scenario [VAL §4.3] |
| Il resto | No | Dipende dall'esito del Gate 0 |

**Validazione con l'app in mano** (DEC 5): l'anteprima ai cori parte il **02/05/2027** con le modalità Libreria e Scaletta (Liturgia aggiunta appena F10 è unita) e si chiude con il **Gate Anteprima del 20/06/2027** (F14). Il pilota completo, con pubblicità e acquisti, va dal 12/09 al 24/10/2027.

Se il Gate 0 dice **fermarsi**: si chiude dopo i round in corso (≈ 16). Resta un repository pulito, con CI verde su due piattaforme, un nucleo musicale testato e un sistema di token riusabile.

---

## 4. Fasi della prima versione (v1)

### F1 — Risanamento del repository e CI verde (build Android e Windows)

**Stato.** **In corso** sul ramo `f1/risanamento-ci`, con pull request, affidata a un altro agente. Il contenuto definitivo è quello della PR; qui si riporta il perimetro concordato.

**Obiettivo.** Rendere il repository onesto, compilabile ovunque e verificato da una CI verde, su Android e Windows, prima di scrivere qualsiasi funzione.

**Risultato dimostrabile.** Una PR fa partire la CI: job "core" (librerie e test su Linux), job "android" (APK di debug come artefatto) e job "windows" (build di debug come artefatto) finiscono in verde; l'unione su `main` è possibile solo così. L'APK si installa su un tablet e la build Windows si avvia su un PC.

**Contenuto principale.**
1. Tag `legacy-2026-02` su `c5ce11c`.
2. `global.json` con SDK fissato; workload set fissato [PC R-33]; `Directory.Packages.props` [VAL §4.6].
3. Aggiornamento a 10.0.x più recente; vulnerabilità High transitive eliminate; FluentAssertions fissato a 7.x o alternativa con licenza aperta [PC R-38].
4. Target MAUI: `net10.0-android` e `net10.0-windows10.0.19041.0`; iOS e MacCatalyst fuori dai TFM (rientrano in F16). Nessuna API solo Android o solo Windows nel codice condiviso [PC R-03].
5. Backend congelato fuori dalla soluzione principale e dalla CI obbligatoria; con DEC 3 si potrà cancellare (resta nel tag).
6. Eliminazione del codice morto [PC R-40, VAL §6] e dei client esterni non più in perimetro (Perplexity, ACRCloud, Azure) dalla DI e dalla Shell.
7. Pulizia documentale [PC R-39]: README breve; documenti non affidabili in `docs/archivio/`; CLAUDE.md corretto.
8. CI nuova (`.github/workflows/ci.yml`) al posto di `build-and-test.yml` [VAL §5]: job **core** (format, vulnerabilità, copertura), job **android** (workload fissato, cache, APK), job **windows** su `windows-latest` (build non firmata come artefatto); action aggiornate; protezione di `main` con i tre job obbligatori.

**Riuso / rifare / eliminare.** Tiene Domain, Application, Infrastructure (repository), MAUI e i 45 test veri [VAL §6]. Rifà la CI da zero. Elimina quanto al punto 6 e congela il backend [VAL §2.5, §4.3].

**Richieste del prompt coperte.** P34, P35 (base di build), P38 (interfacce come contratti).

**Dipendenze.** F0.

**Dal proprietario.** Riparare i workload in locale da amministratore [VAL §2.1]; verificare minuti Actions e protezione del branch sul piano GitHub (V-09); ordinare tablet (100-150 €) e pedale (30-60 €); avere un PC Windows 10/11 per le prove.

**Criteri di accettazione.**
- Build e test della soluzione principale riescono in locale e in CI; tre job verdi in 3 PR consecutive; nessun job "continue-on-error".
- Nessun pacchetto vulnerabile; `dotnet format` senza differenze; copertura ≥ 80% delle righe su Domain e Application.
- Artefatti APK e Windows presenti in ogni run e avviabili su dispositivi reali.
- `main` accetta unioni solo con i job obbligatori verdi.

**Rischi.** Workload MAUI lenti o instabili sui runner; minuti Actions insufficienti (Windows conta di più [DA VERIFICARE]); piano GitHub senza protezione del branch su repo privato.

**Dimensione.** 4 round (uno in più di ieri per il job Windows).

---

### F2 — Nucleo musicale: ChordPro, trasposizione, notazione legata alla lingua, "incolla"

**Obiettivo.** Avere in una libreria senza dipendenze MAUI, testata a fondo, tutta la logica musicale della v1.

**Risultato dimostrabile.** Un test di accettazione prende un canto "accordi sopra il testo" incollato da un libretto, lo converte in ChordPro, lo traspone di +2 con preferenza bemolle e lo rende in Do-Re-Mi (minore "Lam") con capotasto 3; lo stesso canto in inglese esce in C-D-E ("Am"). Il risultato compare nel report dei test.

**Contenuto principale.**
1. Parser ChordPro esteso: blocchi `{start_of_tab}` in monospazio senza trasposizione; direttive di SongbookPro e OnSong; round-trip senza perdite [PC R-11].
2. Trasposizione: diesis/bemolle, minori con alterazioni corrette, accordi con basso, capotasto (tonalità suonata e sentita) [VAL §4.2].
3. Notazione: C-D-E e Do-Re-Mi in lettura e scrittura; minore "Lam" o "La-"; **valore predefinito dalla lingua** (italiano → Do-Re-Mi con "Lam"; inglese → C-D-E con "Am") (DEC 9).
4. Modello di impaginazione indipendente dalla UI, con misura del testo iniettabile: il controllo di disegno sarà solo un "pittore".
5. Conversione "accordi sopra il testo" → ChordPro (base di "incolla" e import).
6. Correzione del difetto di DI del rendering [VAL §3.1 2a].

**Riuso / rifare / eliminare.** Tiene `ChordProParser`, `ChordTransposer` e i 45 test. Rifà il rendering [VAL §3.1 1a].

**Richieste del prompt coperte.** P01, P02 (visualizzazione), P05, P07, base di P21-P23.

**Dipendenze.** F1 (job core).

**Dal proprietario.** 10-20 canti reali con l'esito atteso scritto a mano per almeno 5.

**Criteri di accettazione.** Copertura ≥ 90% sui servizi musicali; round-trip su ≥ 10 file SongbookPro/OnSong; test parametrici sulle 12 tonalità maggiori e minori, due notazioni, con e senza capo; test del valore predefinito per lingua; "incolla" riconosce ≥ 90% delle righe di accordi dei canti forniti; CI verde, review chiusa.

**Rischi.** Ambiguità della notazione italiana ("Si"/"Sib", "La-" contro il trattino nel testo).

**Dimensione.** 3 round.

---

### F3 — Prove tecniche del Gate 0: leggio su tablet Android e su Windows, import da PDF

**Obiettivo.** Rispondere con dati, entro le date del Gate 0, alle due domande che possono fermare il progetto: il leggio è fluido con il pedale su un tablet economico **e su Windows**? L'import dai libretti reali è abbastanza buono?

**Risultato dimostrabile.**
- **Traccia A (entro il 24/01/2027)**: un canto di due pagine nel tema Palco scuro scorre fluido e gira pagina con il pedale Bluetooth sul tablet Android (anche con i tasti del volume) e su un PC Windows, a schermo sempre acceso.
- **Traccia B (entro il 17/01/2027)**: report con la percentuale di canti importati da 10 libretti PDF reali con ≤ 2 minuti di correzione ciascuno, separata tra PDF testuali e scansioni.

**Contenuto principale.**
- Traccia A: controllo basato su `GraphicsView` che disegna il modello di F2 (una vista per riga) [UI-2]; gestione dei tasti dietro un'interfaccia di piattaforma (`MainActivity` su Android, `KeyDown` sulla finestra WinUI su Windows) [UX-B §7]; `KeepScreenOn`; misura dei tempi di disegno. I colori arrivano da un dizionario provvisorio con le chiavi dei token, così F4 li sostituisce senza toccare il controllo.
- Traccia B: estrazione da PDF testuale **[IPOTESI: PdfPig, Apache 2.0]**; OCR sul dispositivo **[IPOTESI: Google ML Kit su Android tramite binding .NET; su Windows l'OCR di sistema `Windows.Media.Ocr`; alternativa Tesseract su entrambe]**; banco di prova da riga di comando; nessuna chiamata in cloud [PC R-04, R-05].

**Riuso / rifare / eliminare.** Riusa F2. La pagina di prova è usa e getta; il controllo di disegno passa in F7 se supera la review. Si confermano fuori i mock OCR e gli stub Tesseract e Azure.

**Richieste del prompt coperte.** P21, P22 (prova), base di P01, P06, P35.

**Dipendenze.** F1 (job Android e Windows), F2.

**Dal proprietario.** Tablet, pedale (emulazione tastiera), PC Windows; 10 libretti PDF reali; prova in una chiesa poco illuminata; cronometro per i minuti di correzione.

**Criteri di accettazione.**
- Traccia A: nessun frame perso visibile (video su entrambe le piattaforme); pedale funzionante senza configurazioni dopo l'accoppiamento; test unitari sul modello e test di disegno senza errori in CI.
- Traccia B: soglie di [PC §5] (≥ 70% → OCR in v1; 50-70% → solo PDF testuale e "incolla"; < 50% → import fuori dalla v1), valutate **per piattaforma** se l'OCR Windows rende diversamente.
- APK e pacchetto Windows della prova prodotti dalla CI.

**Rischi.** Prestazioni di `GraphicsView` su tablet economici e su WinUI; binding ML Kit per .NET 10 non aggiornato (V-07); due motori OCR con qualità diverse; pedali con codici tasto diversi.

**Dimensione.** 4 round (uno in più di ieri per Windows).

---

### F4 — Sistema di token, cinque temi e componenti

**Obiettivo.** Un solo sistema di token da cui nascono tutti i temi: un tema è **solo un insieme di valori**, e i cinque temi chiari e scuri si scelgono nelle impostazioni (DEC 8).

**Risultato dimostrabile.** Una pagina "catalogo dei componenti" (solo nelle build di debug) mostra pulsanti, chip, elenco, campo di ricerca, barra dei controlli, navigazione, riga accordi e spazio del banner. Cambiando tema nelle impostazioni la pagina passa per le **10 combinazioni** (5 temi × chiaro/scuro) su Android e su Windows senza riavvio. Il test dei contrasti in CI elenca i rapporti di ogni coppia.

**Contenuto principale.**
1. **Specifica con la taste-skill** (DEC 14) dei token e dei componenti, partendo dalle tavole dei token di [UI-1]…[UI-5], che hanno già le stesse 16 chiavi di colore (`bg`, `surface`, `surface2`, `lyricsBg`, `text`, `text2`, `chord`, `chordBg`, `accent`, `onAccent`, `accentSoft`, `onAccentSoft`, `border`, `danger`, `focus`, `rule`).
2. Token **oltre il colore**, perché i temi differiscono anche nella forma: famiglie e dimensioni dei caratteri per ruolo (liriche, accordi, interfaccia, tonalità), raggi, spessori dei bordi, altezze dei controlli (44 e 56 dp), densità, ombre; e **varianti di disegno** come valori: stile dell'accordo (semplice o pastiglia [UI-4]), stile del ritornello (corsivo [UI-1], grassetto con barra [UI-2], riquadro tinto [UI-4]), numeri di riga [UI-3], display della tonalità [UI-5], filetto doppio [UI-1]. Le chiavi specifiche (`s-gold`, `lcdBg`, `lcdFg`) diventano slot "decorativi" comuni.
3. **Fonte unica**: un file di definizione dei token da cui la build genera i 10 `ResourceDictionary` **[IPOTESI: generatore di codice o script in CI]**; un controllo in CI fallisce se un file XAML contiene un colore, un carattere o una misura scritti a mano.
4. Cambio di tema a runtime con `DynamicResource`; impostazioni "tema per il chiaro", "tema per lo scuro" e "aspetto" (chiaro, scuro, come il sistema) **[IPOTESI]**; predefiniti Leggio chiaro e Palco scuro.
5. Caratteri: Literata, Source Sans 3, Atkinson Hyperlegible Next e Mono, IBM Plex Sans e Mono, Bricolage Grotesque, Figtree, Barlow nelle tre larghezze (circa 30 file, tutti OFL), registrati con alias separati per peso [UI-1 §Fattibilità]; un solo set di icone al tratto come font.
6. Il pittore della riga accordi di F3 legge i token e le varianti di disegno.
7. Ordine: prima Leggio chiaro e Palco scuro (prima del Gate 0), poi gli altri otto dizionari.

**Riuso / rifare / eliminare.** Riusa le tavole dei token e le misure di contrasto delle cinque proposte, senza rifarle. Elimina le emoji usate come icone [UI-1].

**Richieste del prompt coperte.** Nessuna richiesta esplicita; requisito di DEC 8 e di [PC R-16].

**Dipendenze.** F1. Il pittore di F3 per il punto 6.

**Dal proprietario.** Installare la taste-skill prima del round; scegliere il set di icone (Phosphor o Lucide) e, se serve, il ritornello per ciascun tema [UI decisione 3].

**Criteri di accettazione.**
- Test dei contrasti su tutte le coppie di testo delle 10 combinazioni: ≥ 4,5:1 ovunque; **≥ 7:1 per liriche e accordi nelle schermate di lettura e nel Palco** [PC R-16]. Dove una proposta resta sotto (accordi di Spartito chiaro 6,39:1 [UI-3]; accordi di Analogico chiaro 5,89:1 [UI-5]), la specifica corregge il valore del token, senza rifare la proposta.
- Nessun colore o carattere fuori dai token (controllo in CI).
- Aumento della dimensione dell'APK misurato e scritto nella PR; soglia **[IPOTESI: ≤ 8 MB in più]**, altrimenti sottoinsiemi dei caratteri.
- Catalogo dei componenti provato su tablet e PC nei due temi predefiniti.

**Rischi.** Le varianti di forma (pastiglie, tasti a doppio strato, ombre) moltiplicano il lavoro dei componenti [UI-4, UI-5]; peso dei caratteri; temi che divergono e rompono il principio "un tema è un insieme di valori".

**Dimensione.** 5 round (3 prima del Gate 0, 2 dopo).

---

### F5 — Prove visive automatiche (confronto di schermate) in CI

**Obiettivo.** Accorgersi in CI di ogni cambiamento visivo non voluto, nelle 10 combinazioni di tema e sulle due piattaforme, senza provarle a mano una per una.

**Risultato dimostrabile.** Una PR che cambia per errore un token fa fallire la CI e allega le immagini "prima / dopo / differenza" come artefatti; una PR che aggiorna di proposito le schermate di riferimento passa dopo la review.

**Contenuto principale.**
1. **Livello 1, veloce (job core su Linux)**: disegno senza schermo della riga accordi e del catalogo dei componenti per un insieme fisso di canti, nelle 10 combinazioni e in 3 larghezze (telefono, tablet, desktop), confrontato con immagini di riferimento nel repository **[IPOTESI: Microsoft.Maui.Graphics con backend Skia, utilizzabile senza schermo]**.
2. **Livello 2, sul dispositivo**: l'app in "modalità prova" con dati fissi su emulatore Android e su Windows; schermate di libreria, canto, scaletta, Palco, celebrazione e impostazioni catturate e confrontate con tolleranza **[IPOTESI: Appium con driver Android e Windows; DA VERIFICARE la stabilità sui runner ospitati, V-09]**. Nelle PR: i due temi predefiniti; ogni notte: la matrice completa.
3. Procedura scritta per aggiornare le immagini di riferimento (solo in una PR dedicata, con le differenze allegate).
4. Base per il controllo "nessuna pubblicità sulle schermate protette" di F11.

**Riuso / rifare / eliminare.** Nulla da riusare; nessuna eliminazione.

**Richieste del prompt coperte.** Nessuna richiesta esplicita; requisito di DEC 8 e DEC 15.

**Dipendenze.** F1, F4 (catalogo dei componenti). Il livello 2 si estende a ogni schermata nuova da F6 in poi.

**Dal proprietario.** Verificare il costo in minuti di emulatore e Windows sul piano GitHub (V-09).

**Criteri di accettazione.** Livello 1 bloccante su ogni PR; livello 2 bloccante sui due temi predefiniti; esecuzione notturna che apre una segnalazione se fallisce; falsi positivi < 1 su 20 esecuzioni sulla stessa versione **[IPOTESI]**.

**Rischi.** Differenze di disegno tra macchine (caratteri, antialiasing) che danno falsi allarmi; emulatore lento o instabile; costo delle immagini di riferimento da mantenere quando la UI cambia spesso.

**Dimensione.** 3 round.

---

### F6 — Fondamenta: persistenza, impostazioni sul dispositivo, navigazione a tre modalità, libreria

**Obiettivo.** Un'app affidabile su Android e Windows in cui si creano, cercano e ritrovano i canti, con dati che sopravvivono ad aggiornamenti, e in cui l'utente sceglie modalità, temi e notazione.

**Risultato dimostrabile.** Sul tablet e sul PC: al primo avvio si sceglie la modalità; si crea un canto incollando il testo, lo si ritrova dalla ricerca; si cambia modalità (la navigazione cambia senza riavvio) e tema; si installa una build più nuova con una migrazione: dati e impostazioni ci sono ancora e il backup pre-migrazione esiste.

**Contenuto principale.**
1. **Persistenza** (anticipabile prima del Gate 0): un solo DbContext; un solo set di migrazioni EF; `Migrate()` atteso all'avvio; niente `EnsureCreated`; nessuna eccezione ingoiata; seed dei momenti idempotente; **backup automatico prima di ogni migrazione** e ripristino guidato [PC R-08, R-09]; DbContext a vita breve [VAL §4.3]. Percorsi dei dati corretti sia per Android sia per Windows (pacchettizzato e non).
2. **Modello dati comune alle tre modalità**: canto; raccolta/repertorio (senza data); scaletta (con data, voci con scelte della sera); celebrazione (scaletta legata a un giorno liturgico, riempita in F10). Un solo motore, tre presentazioni [UX-B §2].
3. Test su SQLite reale e test di aggiornamento tra versioni [PC R-10].
4. ViewModel in una libreria `net10.0` senza MAUI, testati nel job core **[IPOTESI]**.
5. **Impostazioni sul dispositivo** (niente account): modalità (Libreria, Scaletta, Liturgia), temi e aspetto (F4), notazione e minore con predefinito dalla lingua (DEC 9), diesis/bemolle, lingua; enum al posto delle stringhe [VAL §4.1].
6. **Tre navigazioni Shell** che riusano le stesse pagine: Libreria (Libreria · Raccolte · Impostazioni) [UX-A]; Scaletta (Scalette · Canti · Impostazioni) [UX-B]; Liturgia (Prossima · Calendario · Canti · Impostazioni) [UX-C], con le sezioni liturgiche vuote fino a F10. "Scopri" e "Account" spariscono (DEC 3). Telefono con barra inferiore; tablet e Windows con barra laterale e due pannelli [UX-A §2]. Parametri con `IQueryAttributable`; niente route duplicate [VAL §4.4]. Modalità predefinita al primo avvio: **[IPOTESI] Scaletta, con scelta esplicita nella prima schermata**.
7. Libreria: CRUD (correzione del salvataggio [VAL §3.2]), ricerca per titolo, autore, incipit, tag; metadati (lingua, tipo, tonalità, YouTube, momenti, riferimenti biblici, stato legale dichiarato); raccolte manuali con ordinamento (pulsanti ↑/↓ anche su Windows [UX-B §7]).
8. Infrastruttura: tutte le stringhe in resx, interfaccia italiana, nomi dei momenti localizzabili [PC R-15]; feature flag con AI, OCR in cloud e audio spenti, e flag per pubblicità, acquisti e collegamento esterno alle donazioni (spento) [PC R-05].
9. Eliminazione dei warning MVVMTK0045 e CS0618 [VAL §2.3].

**Riuso / rifare / eliminare.** Tiene Domain, `SongRepository`/`AlbumRepository`, `SongMetadataViewModel`, `DatabaseSeeder`, struttura MVVM/DI [VAL §6]. Rifà persistenza, migrazioni, navigazione, salvataggio, impostazioni. Cancella pagine e codice di AI, OCR mock, audio, cloud sync, canzoni pubbliche e login (DEC 3).

**Richieste del prompt coperte.** P07, P08, P12, P13, P16 (metadati), P17, P18, P24 (raccolte), P26, P27 (stato legale), P29 (infrastruttura), P30, P31, P37 (meccanismo).

**Dipendenze.** F1, F2; F4 (i due temi predefiniti) per i punti 5-7.

**Dal proprietario.** 20-30 canti propri; prova di aggiornamento su tablet e PC.

**Criteri di accettazione.**
- Test di migrazione su SQLite reale (vuoto → ultima; con dati → ultima senza perdite; fallimento → ripristino).
- Test dei ViewModel di apertura, creazione, modifica, ricerca, impostazioni e cambio di modalità.
- Controllo in CI sui testi visibili fuori dalle risorse **[IPOTESI: script sugli attributi `Text`/`Title`]**.
- Schermate di libreria, raccolte e impostazioni nel livello 2 di F5.
- Prova su tablet e PC: creazione, riavvio, aggiornamento con migrazione, cambio di modalità.

**Rischi.** Migrazioni su dati reali [PC rischio 3]; tre navigazioni che divergono nel tempo (mitigazione: pagine condivise, Shell sottili); percorsi dei dati diversi tra pacchetto pilota e Store su Windows (vedi F13).

**Dimensione.** 6 round (2 anticipabili prima del Gate 0).

---

### F7 — Leggio e import su Android e Windows (traguardo T1)

**Obiettivo.** Portare un canto vero da un PDF vero al leggio, trasposto e suonato con il pedale, su entrambe le piattaforme.

**Risultato dimostrabile.** **T1, entro il 21/03/2027**: sul tablet economico e sul PC si importa un canto dal libretto PDF della parrocchia, lo si corregge nella revisione affiancata, lo si traspone in Do-Re-Mi con capotasto e lo si suona in Palco girando pagina con il pedale. Ripiego: senza OCR, entro il 18/04/2027.

**Contenuto principale.**
1. Schermata canzone con il controllo di F3, guidato dai token e dalle varianti di disegno di F4: accordi sopra il testo, ritornello secondo il tema, tonalità originale visibile, trasposizione, capotasto, modalità **Chitarra / Tastiera / Solo testo** [PC R-14].
2. **Palco** per il singolo canto: schermo intero, schermo acceso, pedale, tasti del volume su Android, tastiera e rotella su Windows, contrasto ≥ 7:1, controlli da 56 dp [PC R-13; UI-2]. Due pagine affiancate su tablet orizzontale e su Windows solo se F3 lo regge **[IPOTESI: altrimenti a scorrimento]**.
3. **Import** da PDF testuale e, se il Gate 0 lo consente, OCR sul dispositivo; revisione affiancata; **dichiarazione non preselezionata** a ogni import [PC §3.2 righe 18, 23]. Selettore di file nativo su entrambe le piattaforme.
4. **Incolla** con la conversione di F2; blocchi tab in monospazio.
5. Layout desktop della canzone su Windows (finestra ridimensionabile, scorciatoie: `Ctrl+F`, `+`/`-`, `F11`) [UX-A §2].

**Riuso / rifare / eliminare.** Riusa controllo e tasti di F3, nucleo F2, `IOcrService` come contratto. Rifà la pagina canzone [VAL §3.1 1a, 1c].

**Richieste del prompt coperte.** P01, P02, P05, P06, P21, P22 (se il Gate 0 lo consente), P23 sostituita da "incolla", P27 (dichiarazione all'import), P35.

**Dipendenze.** F3, F6. Esito G0 sulla soglia dell'import.

**Dal proprietario.** Prova in chiesa con poca luce; 5 canti importati e cronometrati per piattaforma; pedale di riferimento confermato.

**Criteri di accettazione.**
- T1 su tablet e PC, con video.
- Test dell'import (fixture PDF in `tests/`), dei ViewModel di revisione e lettura.
- Canto e Palco nel livello 2 di F5, in tutte le 10 combinazioni nell'esecuzione notturna.
- Nessuna richiesta di rete durante import e lettura (modalità aereo su tablet, rete disattivata sul PC).

**Rischi.** OCR sotto soglia su una delle piattaforme; impaginazione a due pagine costosa [UX-B §7]; differenze di misura del testo tra Android e WinUI.

**Dimensione.** 6 round.

---

### F8 — Scalette, repertori, Palco in sequenza, condivisione, backup ed export

**Obiettivo.** Completare le modalità Scaletta e Libreria: preparare la Messa per momenti, suonarla in sequenza, passarla al coro, non perdere mai i dati.

**Risultato dimostrabile.** Dal modello "Messa" si crea la scaletta di domenica con le scelte della sera; la si condivide con un QR a un tablet (o come file al PC), che vede subito i canti mancanti e li riceve come file; in Palco si passa da un canto al successivo con il pedale, con l'anteprima del successivo. Un backup completo fatto su Android si ripristina su Windows e viceversa.

**Contenuto principale.**
1. Scalette con data e **modello Messa**, scalette libere, separatori, **scelte della sera** (tonalità, capotasto, strumento) [UX-B §3]; repertori senza data come motore comune con le raccolte di F6.
2. Palco in sequenza: anteprima del successivo, ricerca d'emergenza con inserimento dopo il corrente, scheda "Pronta per il palco?", uscita protetta [UX-B §4].
3. Filtri per momento, tonalità, tipo, lingua e "non usato di recente" [PC §3.2 righe 21, 28].
4. **Condivisione**: QR con la sola scaletta [PC R-12]; canti e raccolte come file ChordPro (foglio di condivisione su Android, salvataggio e apertura di file su Windows; su Windows il QR si mostra, l'import è da file **[IPOTESI]**); dichiarazione non preselezionata all'invio.
5. **Backup e ripristino**: archivio con ChordPro e manifest JSON, leggibile fuori dall'app, uguale sulle due piattaforme [PC R-11]. È anche la via di passaggio dal pacchetto Windows del pilota a quello dello Store (F13).
6. **Export** ChordPro di canto, raccolta e scaletta.

**Riuso / rifare / eliminare.** Riusa `AlbumRepository` e `AlbumDetailViewModel` [VAL §3.1 7a]. Rifà `CloudSyncService` come backup su file [VAL §4.3]. Elimina `LocalStorageProvider` e i provider cloud.

**Richieste del prompt coperte.** P10 (sostituita dal backup), P11, P16 (scaletta per momenti), P24, P25 (filtri al posto dell'AI), P26, P27 (all'invio), P32 (momento, tonalità, tipo, lingua), P33.

**Dipendenze.** F6, F7.

**Dal proprietario.** Due dispositivi Android e il PC; una scaletta reale; prova nella propria parrocchia.

**Criteri di accettazione.** Round-trip backup → ripristino su SQLite reale, anche tra piattaforme; test del formato QR (scaletta di 15 voci); export/import ChordPro compatibili con F2; prova su due dispositivi senza rete salvo il trasferimento dei file; schermate nel livello 2 di F5.

**Rischi.** Dimensione del QR; confusione tra scaletta condivisa e canti privati [UX-B §3.9]; differenze di gestione dei file tra Android e Windows.

**Dimensione.** 6 round.

---

### F9 — Calendario liturgico: motore e riferimenti delle letture

**Obiettivo.** Calcolare sul dispositivo, offline, il calendario del rito romano con il calendario proprio dell'Italia, e associare a ogni giorno i **riferimenti** delle letture, mai i testi (DEC 7).

**Risultato dimostrabile.** Un test confronta il calendario calcolato per ogni giorno dal 2026 al 2036 con un calendario di riferimento: tempo, settimana, ciclo festivo (A/B/C) e feriale (I/II), celebrazione con grado, colore liturgico e riferimenti delle letture; per esempio la domenica 15/11/2026 è la XXXIII del Tempo Ordinario, anno A, con Vangelo Mt 25,14-30 [UX-C §1]. Il report è nella CI.

**Contenuto principale.**
1. **Motore** in una libreria senza MAUI: data della Pasqua, tempi, numerazione delle domeniche, cicli, solennità, feste e memorie del calendario romano generale e del **calendario proprio dell'Italia**, tabella delle precedenze e regole di trasferimento (per esempio Ascensione alla domenica in Italia; solennità che cadono in una domenica di Avvento, Quaresima o Pasqua) [UX-C §6]. Memorie facoltative indicate come tali.
2. **Tabella dei riferimenti del lezionario** (prima lettura, salmo, seconda lettura, Vangelo) per domeniche e solennità dei tre cicli, feste, ferie dei due cicli, proprio dei santi: circa 1.000-1.500 voci [UX-C §7], in un file di dati versionato con la fonte di ogni voce. Formato dei riferimenti uguale a quello di `BibleReference` (F2, F6).
3. Nessun testo delle letture né delle orazioni; nomi delle celebrazioni in italiano (e in inglese solo se l'interfaccia inglese entra in v1, §8).
4. Aggiornabile solo con una nuova versione dell'app (nessun server).
5. Rito ambrosiano escluso (F19, se richiesto).

**Riuso / rifare / eliminare.** Riusa `BibleReference` e `LiturgicalMoment` del dominio. Nulla da eliminare.

**Richieste del prompt coperte.** P17 (base del collegamento con le letture); funzione di DEC 7 non presente nel prompt.

**Dipendenze.** F1; F2 per il formato dei riferimenti. Può procedere in parallelo a F6-F8 (file disgiunti).

**Dal proprietario.** Calendari ufficiali di riferimento per i test (calendario liturgico annuale CEI o un Ordo per almeno 2026-2028) e un controllo a campione di 30 giorni; risposta del legale sull'uso di una tabella di soli riferimenti (V-11).

**Criteri di accettazione.**
- Copertura ≥ 95% delle righe del motore.
- **Zero differenze** con il calendario di riferimento sugli anni disponibili; per gli anni successivi, test sulle date delle feste mobili e sui trasferimenti.
- Ogni voce della tabella ha la fonte; un test verifica che nessuna voce superi la lunghezza di un riferimento (niente testi per errore).
- Review Opus xhigh mirata alle regole di precedenza.

**Rischi.** Errori nel calendario o in un riferimento: tolgono credibilità agli animatori al primo errore (mitigazione: test sugli anni ufficiali, segnalazione rapida); lavoro di compilazione e verifica della tabella più lungo del previsto; nuove memorie decise dalla Santa Sede o dalla CEI che richiedono aggiornamenti; dubbi legali sulla tabella (V-11).

**Dimensione.** 6 round (3 motore, 3 tabella).

---

### F10 — Modalità Liturgia: celebrazioni, slot, suggerimenti locali, storico

**Obiettivo.** Completare la terza modalità: l'app parte dalla prossima celebrazione e aiuta a scegliere i canti adatti, senza AI e senza rete [UX-C].

**Risultato dimostrabile.** In modalità Liturgia la home mostra "Domenica … · XXXIII del Tempo Ordinario · Anno A" con i riferimenti delle letture e lo schema della Messa a slot (senza Gloria in Avvento e Quaresima, salvo le solennità); toccando "Comunione" compaiono i canti adatti al momento e al tempo, collegati al Vangelo per riferimento biblico e non usati di recente; si sceglie, si apre "Celebra" e si suona con il pedale passando da un momento all'altro. Il tema Vetrata prende il colore del tempo liturgico.

**Contenuto principale.**
1. Home "Prossima", vista "Calendario" (settimane, tempi, feste; occasioni come matrimonio e funerale come modelli).
2. **Celebrazione** = scaletta di F8 legata a un giorno di F9; slot per momento con schema che dipende dal tempo e dal grado [UX-C §1].
3. **Suggerimenti locali** (nessuna AI, DEC 3): momento + tempo + vicinanza del riferimento biblico alle letture del giorno + "non usato di recente" + presenza nel repertorio; ricerca per Vangelo ("Mt 25").
4. **Storico** "cosa abbiamo cantato" dalle celebrazioni passate; "Duplica per un'altra data".
5. **Celebra**: il Palco di F8 guidato dai momenti, con salto a un momento e slot vuoti saltati [UX-C §4].
6. Accento del tema Vetrata legato al tempo liturgico, con opzione per bloccarlo [UI-4].
7. Fuori dalla v1 **[IPOTESI]**: profili per più cori, "Proponi tutto con l'AI", pianificatore a settimane su Windows (F19).

**Riuso / rifare / eliminare.** Riusa scalette e Palco di F8, motore di F9, metadati di F6.

**Richieste del prompt coperte.** P16, P17 (filtro e collegamento alle letture), P25 (raccolte suggerite in forma locale), P32 (riferimento biblico).

**Dipendenze.** F8, F9.

**Dal proprietario.** Uso per due domeniche nella propria parrocchia; elenco dei momenti "parlati" più comuni.

**Criteri di accettazione.** Test dello schema per tempo e grado; test dei suggerimenti su una libreria di prova (ordinamento atteso scritto a mano per 5 casi); schermate nel livello 2 di F5; uso in una Messa vera.

**Rischi.** Suggerimenti poveri con librerie poco etichettate [UX-C §6] (mitigazione: momenti e riferimenti proposti all'import); curva di apprendimento più alta.

**Dimensione.** 4 round.

---

### F11 — Pubblicità e consenso

**Obiettivo.** Mostrare pubblicità solo dove il proprietario l'ha ammessa, con il consenso raccolto come richiesto, senza mai disturbare la lettura né bloccare l'app senza rete (DEC 10-11).

**Risultato dimostrabile.** Su Android, al primo avvio compare la richiesta di consenso; nelle schermate di elenco e gestione c'è un banner (di prova in debug); all'avvio a freddo compare l'annuncio di apertura, **ma non** se l'app riparte con una scaletta o una celebrazione in corso; canto, Palco, scaletta aperta e celebrazione non hanno mai annunci. Con il Pro attivo non parte nessun SDK pubblicitario. Senza rete l'app si comporta come se la pubblicità non ci fosse.

**Contenuto principale.**
1. Interfaccia `IAdService` con implementazione Android **[IPOTESI: SDK Google Mobile Ads tramite binding .NET; V-07]** e implementazione "nessuna pubblicità" per Windows, salvo esito positivo di V-02.
2. **Consenso**: piattaforma di consenso richiesta da Google nel SEE **[IPOTESI: UMP di Google; V-06]**, prima di qualsiasi richiesta di annunci; voce "Privacy e pubblicità" nelle impostazioni per rivedere la scelta; annunci limitati senza consenso.
3. **Regole di posizionamento come codice**: una tabella "schermata → formati ammessi"; test che falliscono se una schermata protetta chiede un annuncio. Spazio del banner riservato nei layout (F4) per non spostare i contenuti.
4. Annuncio di apertura solo all'avvio a freddo e mai con "scaletta in corso" **[IPOTESI: Palco, scaletta o celebrazione aperti alla chiusura dell'app, oppure scaletta con data di oggi]**; regole di frequenza secondo V-01.
5. Identificativi reali solo nei segreti della CI; annunci di prova in debug e nell'anteprima.
6. Impostazioni della console AdMob (lavoro del proprietario): classificazione massima dei contenuti e categorie sensibili bloccate, adatte a un'app usata in parrocchia e da minori (V-06).

**Riuso / rifare / eliminare.** Nulla da riusare.

**Richieste del prompt coperte.** P28 (consenso e dati personali), in parte; funzione di DEC 10-11.

**Dipendenze.** F6 (schermate e flag), F13 (app registrata). Da completare prima del pilota completo (12/09/2027).

**Dal proprietario.** Account AdMob collegato al profilo pagamenti; unità pubblicitarie; impostazioni di blocco delle categorie; esito di V-01, V-02, V-06.

**Criteri di accettazione.**
- Test di unità sulle regole di posizionamento e sull'annuncio di apertura (avvio a freddo, ripresa, scaletta in corso, Pro).
- Livello 2 di F5: nessun elemento pubblicitario nelle schermate protette.
- Nessun annuncio prima del consenso (test su dispositivo con il registro di rete).
- Avvio senza rete identico in tempi e comportamento (misura su tablet).

**Rischi.** Violazioni delle policy (clic accidentali vicino ai controlli, annuncio di apertura usato male) con sospensione dell'account AdMob; annunci inadatti in chiesa; incasso basso perché l'app si usa offline; nessuna rete pubblicitaria su Windows.

**Dimensione.** 3 round.

---

### F12 — Acquisti in app: Pro e donazioni

**Obiettivo.** Vendere il Pro a 4,99 € (toglie la pubblicità) e ricevere donazioni facoltative tramite gli store, senza mai togliere una funzione a chi è senza rete (DEC 10, 12).

**Risultato dimostrabile.** Con un account di prova su Play e su Partner Center: si compra il Pro, la pubblicità sparisce, si riavvia senza rete e il Pro resta; si ripristina l'acquisto su una reinstallazione; si fa una donazione e compare un ringraziamento. Il pulsante verso la pagina esterna di donazione non esiste finché il flag resta spento.

**Contenuto principale.**
1. Interfaccia `IStoreService`; Android con Google Play Billing **[IPOTESI: binding .NET o Plugin.InAppBilling; versione minima della libreria richiesta da Play, V-07]**; Windows con `Windows.Services.Store`, **attivo solo nella build dello Store** (nel pacchetto pilota da GitHub niente acquisti).
2. Prodotti: **Pro** non consumabile a 4,99 €; donazioni come prodotti consumabili **[IPOTESI: tre importi, per esempio 1,99 €, 4,99 € e 9,99 €]**, senza vantaggi in cambio.
3. **Diritto valido senza rete a tempo indeterminato**: il Pro si conserva in locale; una verifica fallita non rimette la pubblicità finché non c'è una risposta certa dello store; un rimborso toglie solo il Pro, mai una funzione [PC R-07].
4. Conferma degli acquisti nei tempi richiesti da Play; ripristino degli acquisti; gestione dei rimborsi.
5. Il Pro vale per lo store in cui è comprato (niente account): la schermata lo dice prima dell'acquisto.
6. Flag spento per il collegamento alla pagina esterna di donazione, fino all'esito di V-03.

**Riuso / rifare / eliminare.** Nulla da riusare.

**Richieste del prompt coperte.** Nessuna; funzione di DEC 10 e DEC 12.

**Dipendenze.** F6, F13 (app creata in Play Console e prenotata in Partner Center). Da completare prima del pilota completo.

**Dal proprietario.** Profilo pagamenti Google e Microsoft; prodotti creati nelle due console; tester delle licenze; esito di V-03, V-04, V-05.

**Criteri di accettazione.** Test di unità della logica del diritto (acquisto, ripristino, rimborso, assenza di rete); acquisto vero di prova su entrambi gli store; nessun prezzo scritto nel codice (prezzi letti dagli store); scheda "Sicurezza dei dati" aggiornata.

**Rischi.** Due sistemi di acquisto da mantenere; acquisti Windows provabili solo con l'app nello Store (anche in pubblico privato, V-04); rimborsi e contestazioni; obblighi fiscali (V-05).

**Dimensione.** 4 round.

---

### F13 — Canali di rilascio: Google Play; Windows su GitHub, poi Microsoft Store

**Obiettivo.** Pubblicare ogni versione esclusivamente tramite CI, su due canali: Play (test interno → chiuso → produzione) e Windows (pacchetto su GitHub durante il pilota, Microsoft Store al lancio) (DEC 13, 15).

**Risultato dimostrabile.** Un tag `v0.x.y` su `main` produce in CI un AAB firmato caricato sulla traccia interna di Play e un pacchetto Windows allegato a una release GitHub scaricabile dai cori; da luglio lo stesso tag produce anche un MSIX inviato al Microsoft Store.

**Contenuto principale.**
- **Parte 1 (marzo 2027)**:
  1. Nome e `ApplicationId` definitivi [PC R-17], decisi con D10.
  2. AAB in Release, Play App Signing, chiave di caricamento nei segreti [PC R-18]; `versionCode` automatico; livello di API e pagine da 16 KB come richiesti da Play [PC R-19, V-10].
  3. Caricamento sulla traccia interna via API con account di servizio [PC R-37]; ambiente GitHub con approvazione manuale.
  4. Pacchetto Windows del pilota: **[IPOTESI] MSIX firmato, installabile in sideload**, pubblicato in una release GitHub; firma e visibilità da decidere con V-08 e D12 (un repo privato non rende le release scaricabili ai cori: serve un repo pubblico di sole release oppure un'altra sede).
  5. Prova di fumo in CI su emulatore Android e su Windows: installazione, avvio a freddo, apertura di un canto.
- **Parte 2 (aprile-luglio 2027)**:
  6. Test chiuso Play con ≥ 12 tester per ≥ 14 giorni [PC R-20, N1], durante l'anteprima.
  7. App prenotata in Partner Center; MSIX per lo Store (firmato dallo Store), invio da CI **[IPOTESI: strumento a riga di comando del Microsoft Store o API di invio; V-04]**; pubblico privato dello Store per il pilota completo, se disponibile (V-04).
  8. **Passaggio dal pacchetto pilota allo Store**: identità del pacchetto diversa → dati in una cartella diversa; procedura con backup e ripristino di F8, scritta e provata.
  9. Blocco delle promozioni in produzione da venerdì a domenica su entrambi i canali [PC R-21]; note di rilascio in italiano (Haiku).

**Riuso / rifare / eliminare.** Nulla da riusare dal vecchio workflow [VAL §5.5-5.7].

**Richieste del prompt coperte.** P34, P35 (pubblicazione).

**Dipendenze.** F1; si sviluppa in parallelo a F7-F10.

**Dal proprietario.**
- Account Play (25 $) con verifica d'identità; chiave di caricamento e segreti (`ANDROID_KEYSTORE_BASE64`, `ANDROID_KEYSTORE_PASSWORD`, `ANDROID_KEY_ALIAS`, `ANDROID_KEY_PASSWORD`, `PLAY_SERVICE_ACCOUNT_JSON`); primo caricamento manuale se richiesto (V-10).
- Account sviluppatore Microsoft (costo da verificare, V-04); credenziali di invio nei segreti.
- Certificato di firma per il pacchetto pilota, se scelto (V-08); decisione sulla sede delle release (D12).
- Indirizzi Google di ≥ 12 tester.

**Criteri di accettazione.** Il job di rilascio fallisce se manca un segreto, se la versione non cresce, se la firma non è valida; nessuna chiave nel repository (scansione dei segreti); AAB installato dal Play Store sul tablet; pacchetto Windows installato da un membro di un coro senza assistenza (prova scritta); procedura di passaggio allo Store provata su un PC con dati reali.

**Rischi.** `ApplicationId` permanente; verifica degli account lenta; avvisi di SmartScreen sul pacchetto pilota; certificazione dello Store respinta per capacità riservate (V-04); costo in minuti dei job Windows.

**Dimensione.** 5 round (3 + 2).

---

### F14 — Anteprima ai cori e pilota (Gate Anteprima e Gate Pilota)

**Obiettivo.** Validare con l'app in mano ai cori (DEC 5) e renderla pubblicabile: legale, misurabile, senza perdite di dati, con pubblicità e acquisti verificati.

**Risultato dimostrabile.**
- **Gate Anteprima, 20/06/2027**: ≥ 3 cori usano l'app ≥ 3 domeniche su 4 per 6 settimane; 0 perdite di dati; ≥ 99,5% di sessioni senza crash su Play; ≥ 12 tester per ≥ 14 giorni e accesso alla produzione richiesto; note di ogni coro sulle tre modalità.
- **Gate Pilota, 24/10/2027**: soglie di [PC §6.1] misurate dal 12/09 al 17/10/2027 con l'app completa, più i criteri del §1.2.

**Contenuto principale.**
1. **Anteprima** (02/05 – 20/06/2027): test chiuso Play e pacchetto Windows da GitHub; modalità Libreria e Scaletta, poi Liturgia; annunci di prova; nessun aggiornamento da venerdì a domenica; correzioni settimanali.
2. **Pilota completo** (12/09 – 24/10/2027): pubblicità con consenso, acquisti con tester delle licenze; Windows tramite pubblico privato dello Store se disponibile (V-04), altrimenti pacchetto GitHub senza acquisti.
3. Schermate legali: Termini, informativa privacy **con pubblicità, consenso e acquisti**, stato legale dei canti, dichiarazioni già introdotte in F7 e F8 [PC §3.2 righe 23-24].
4. Nucleo di 20-40 canti di dominio pubblico verificati e documentati [PC §3.3].
5. "Informazioni": versione e ID anonimo del dispositivo [PC R-30, R-32].
6. Verifica dei flag: AI, OCR in cloud, audio spenti; collegamento esterno alle donazioni spento salvo V-03.
7. Test di aggiornamento in CI dal database della prima versione di test; prove di prestazione su tablet e PC [PC R-02, R-10].
8. Accessibilità nelle 10 combinazioni (contrasto, dimensioni dei controlli, lettore di schermo sui flussi principali), con l'aiuto di F5.
9. Interfaccia in inglese tramite le risorse, tradotta da Haiku e rivista **[IPOTESI]**.

**Riuso / rifare / eliminare.** Riusa `CopyrightStatus` [VAL §3.1 9].

**Richieste del prompt coperte.** P26 (verifica offline), P27, P28, P29 (inglese, se confermato), P34, P35, P37 (verifica).

**Dipendenze.** Anteprima: F7, F8, F13 parte 1. Pilota: F10, F11, F12, F13 parte 2.

**Dal proprietario.** Cori e ambasciatore; **parere legale** (D11) sulle domande di [PC §9.2] più pubblicità, consenso, minori, acquisti e tabella dei riferimenti; **commercialista** e, se indicato, apertura della partita IVA prima del lancio (V-05); dominio, sito minimo con informativa e modulo di segnalazione; email dedicata; canti di dominio pubblico; note settimanali.

**Criteri di accettazione.** Soglie dei due gate; **zero perdite di dati** (ogni segnalazione blocca il rilascio successivo) [PC §9.1 rischio 3]; scheda "Sicurezza dei dati" e informativa coerenti con AdMob e acquisti; nessun avviso di policy aperto su Play, AdMob o Partner Center; test di aggiornamento verdi.

**Rischi.** Cori che abbandonano; minori nei cori giovani con pubblicità e consenso [PC rischio 11]; parere legale che chiede modifiche; pilota estivo inutile (per questo il pilota completo è a settembre).

**Dimensione.** 6 round (3 nell'anteprima, 3 nel pilota completo).

---

### F15 — Lancio: Google Play, Microsoft Store, sito con pagina di donazione

**Obiettivo.** Pubblicare la v1 gratuita, con pubblicità, Pro e donazioni, su Google Play e sul Microsoft Store, con un'assistenza sostenibile.

**Risultato dimostrabile.** Entro il 21/11/2027 l'app è in produzione su Play (rilascio graduale 10% → 50% → 100% dalla CI, mai nel fine settimana) e pubblicata sul Microsoft Store; il sito del progetto ha informativa, termini, segnalazioni e la pagina di donazione.

**Contenuto principale.** Schede degli store in italiano (parole chiave, screenshot con soli canti propri o di dominio pubblico, frase "suonare sarà sempre gratis" [PC §7.3], indicazione "contiene annunci" e "acquisti in app"); classificazione dei contenuti su entrambi gli store; dichiarazione di "trader" se dovuta (V-05); promozione graduale; monitoraggio di Android vitals e dei rapporti dello Store; unità pubblicitarie reali attive; **sito con dominio e pagina di donazione** con servizio di pagamento esterno scelto dal proprietario (DEC 12), **senza collegamento dall'app** salvo esito positivo di V-03; FAQ e 3 video; procedura di segnalazione dei diritti [PC R-30].

**Riuso / rifare / eliminare.** Riusa le pipeline di F13.

**Richieste del prompt coperte.** P34, P35.

**Dipendenze.** F14 (Gate Pilota superato). Decisioni: D12, D13.

**Dal proprietario.** Testi delle schede e video; dominio e servizio di pagamento per le donazioni; dichiarazioni fiscali e di "trader"; presenza in 1-2 corsi diocesani.

**Criteri di accettazione.** Accesso alla produzione ottenuto; app approvata dallo Store; rilascio graduale senza regressioni; il job di promozione rifiuta l'esecuzione da venerdì a domenica (test del job); tag e note di rilascio pubblicati.

**Rischi.** Segnalazione di un editore [PC rischio 4]; rifiuto della certificazione dello Store; assistenza della domenica mattina su due piattaforme [PC rischio 13].

**Dimensione.** 3 round.

---

## 5. Fasi successive alla v1 (condizionate)

Si dettagliano quando scatta il gate indicato.

| ID | Obiettivo | Si apre quando | Contenuto principale | Riuso | Dal proprietario | Round |
|---|---|---|---|---|---|---|
| **F16** iPad / iOS | Portare l'app su iPad | Gate 1 (30/04/2028) e censimento con iPad > 40% (D9) | TFM iOS; tasti per piattaforma; acquisti con App Store; pubblicità con lo stesso SDK se disponibile; job CI con runner macOS e firma | Codice condiviso di F2-F12; token e temi di F4 | Account Apple Developer (99 $/anno), certificati nei segreti | 5-6 |
| **F17** Piano Coro e licenza diocesana | Opzione commerciale verso parrocchie e diocesi (DEC 1): sincronizzare **solo la scaletta** | Gate 2 (30/11/2028), D13, D14 | Backend piccolo senza testi [PC R-06]; account solo per i responsabili adulti [PC R-29]; codici di attivazione [PC R-25]; obblighi DSA [PC R-31]; hosting UE; eventuale versione senza pubblicità per i cori della diocesi | Dal backend congelato solo idee e pezzi isolati [VAL §4.3, §4.5] | Hosting, secondo parere legale (≈ 800 €), contratti | 8-10 |
| **F18** Nuove funzioni a pagamento | Funzioni che fanno risparmiare tempo (import in blocco, stampa ed export PDF della scaletta, backup automatico) | D15, Gate 2, richieste di ≥ 3 cori | Prezzo e rapporto con il Pro da decidere (§0.1) | Billing di F12 | — | 4-6 |
| **F19** Estensioni di contenuto | Ampliare uso e pubblico | Gate 1 e richieste esplicite | Rito ambrosiano (se richiesto, DEC 7); note degli accordi per pianoforte; allegati MP3 e PDF; pianificatore a settimane su Windows [UX-C §2]; profili per più cori; raccolte a regole locali; indice liturgico pubblico senza testi | Motore di F9, scalette di F8 | Fonti per il calendario ambrosiano | 3-10 secondo la scelta |

---

## 6. Verifiche esterne aperte

Questo documento le segnala e non le risolve. Ognuna blocca la fase indicata.

| Codice | Cosa verificare | Dove | Blocca |
|---|---|---|---|
| **V-01** | Regole AdMob sul formato "apertura app" (avvio a freddo e ritorno in primo piano, schermata di caricamento, frequenza) e sul posizionamento dei banner vicino ai controlli; condizioni per servire annunci reali (app pubblicata e collegata allo store) | Documentazione e policy AdMob | F11, F15 |
| **V-02** | Esiste una rete pubblicitaria utilizzabile da un'app MAUI/WinUI 3 su Windows? (l'SDK pubblicitario Microsoft per UWP risulta ritirato da anni [DA VERIFICARE]). Se no, nessuna pubblicità su Windows | Reti pubblicitarie, Partner Center | F11 |
| **V-03** | Regole sulle donazioni: Play (donazioni come acquisti in app, collegamenti a pagine di donazione esterne per chi non è un ente benefico); Microsoft Store (pagamenti e collegamenti esterni nelle app che non sono giochi) | Policy di Google Play e del Microsoft Store | F12, F15 |
| **V-04** | Requisiti dello Store per un'app MAUI (MSIX, capacità riservate come `runFullTrust`, architetture x64/ARM64, certificazione, classificazione dei contenuti, informativa); **costo dell'account sviluppatore individuale** (storicamente circa 19 $ una tantum: verificare il costo attuale); pubblico privato per il pilota; invio automatico da CI | Partner Center | F12, F13, F14 |
| **V-05** | Aspetti fiscali: partita IVA e regime per pubblicità, Pro e donazioni da Google e Microsoft; IVA sugli acquisti in app; donazioni a una persona fisica (in app e sul sito); dichiarazione di "trader" (DSA) su Play e Microsoft Store e dati resi pubblici | Commercialista (Gate 0, attività 12), poi legale | F12, F14, F15 |
| **V-06** | Consenso nel SEE: obbligo di una piattaforma di consenso certificata da Google, comportamento senza consenso; minori nei cori giovani, età del consenso digitale (14 anni in Italia [DA VERIFICARE]), politica "Famiglie" e pubblico di destinazione su Play | Policy AdMob e Play, legale | F11, F14 |
| **V-07** | Binding .NET 10 per Google Mobile Ads, UMP e Play Billing (versione minima della Billing Library richiesta da Play); supporto di WinUI in Plugin.InAppBilling oppure uso diretto di `Windows.Services.Store`; binding ML Kit per l'OCR | NuGet, documentazione | F3, F11, F12 |
| **V-08** | Pacchetto Windows del pilota: firma (certificato, Azure Trusted Signing o simili, costi), installazione in sideload, avvisi SmartScreen; sede delle release scaricabili dai cori con un repo privato (repo pubblico di sole release o decisione D12) | GitHub, Microsoft | F13 |
| **V-09** | Piano GitHub: minuti Actions per repo privato (moltiplicatore dei runner Windows), emulatore Android con accelerazione sui runner ospitati, protezione del branch | GitHub | F1, F5, F13 |
| **V-10** | Play: livello di API target, pagine da 16 KB, primo caricamento manuale prima dell'API, tempi di verifica dell'account | Play Console | F13 |
| **V-11** | Legale: uso di una tabella di soli riferimenti del lezionario CEI e dei nomi delle celebrazioni; domande di [PC §9.2] ampliate a pubblicità, consenso, minori e acquisti | Legale (D11) | F9, F14 |

---

## 7. Rischi nuovi introdotti dal perimetro allargato

I rischi di [PC §9.1] restano validi. Si aggiungono:

| # | Rischio | Prob. / Impatto | Mitigazione |
|---|---|---|---|
| N1 | **Lavoro raddoppiato** (68 round contro 34): cresce il rischio di abbandono [PC rischio 2] | Media / Alto | Traguardi con data (T1, Anteprima); riserva di 8 round; proposta di linea di taglio qui sotto |
| N2 | **Matrice di prova esplosa**: 10 combinazioni di tema × 3 modalità × 2 piattaforme × telefono, tablet e desktop | Alta / Medio | Token unici (F4), prove visive (F5), matrice completa solo di notte |
| N3 | **Errori nel calendario o nei riferimenti delle letture** | Media / Alto | Test sugli anni ufficiali, fonte per ogni voce, review mirata (F9) |
| N4 | **Pubblicità**: policy AdMob violate, annunci inadatti in chiesa, freddezza delle diocesi, incasso basso perché l'app si usa offline | Media / Alto | Regole di posizionamento testate, categorie bloccate, mai annunci su lettura e Palco, Pro per toglierli |
| N5 | **Obblighi fiscali e di "trader" anticipati**: partita IVA e commercialista prima di avere ricavi; dati personali pubblici negli store | Alta / Medio | Commercialista nel Gate 0; decisione D13 prima del Gate Pilota |
| N6 | **Due store e due sistemi di acquisto**; Pro non trasferibile tra piattaforme | Certa / Medio | Un'interfaccia sola; testo chiaro prima dell'acquisto |
| N7 | **Windows**: firma del pacchetto pilota, SmartScreen, passaggio dei dati dal pacchetto pilota allo Store, certificazione | Media / Medio | Procedura di backup e ripristino provata (F8, F13) |
| N8 | **Peso dell'app** per circa 30 file di caratteri | Media / Basso | Misura in F4; sottoinsiemi latini |
| N9 | **Dipendenze di terze parti per .NET 10** (pubblicità, consenso, billing, OCR) non aggiornate | Media / Alto | V-07 prima di F3 e F11; interfacce che permettono di cambiare libreria |
| N10 | **Costo della CI** su repo privato con emulatore e Windows | Media / Medio | V-09; matrice ridotta nelle PR |

**Proposta di linea di taglio** (serve una nuova decisione del proprietario; si usa solo se T1 slitta oltre il 18/04/2027 o il Gate Anteprima oltre il 18/07/2027), nell'ordine: (1) due pagine affiancate su tablet e Windows; (2) OCR delle scansioni (resta PDF testuale + incolla); (3) storico e "Duplica" della modalità Liturgia; (4) interfaccia inglese; (5) temi Vetrata e Analogico rinviati a un aggiornamento dopo il lancio.

---

## 8. Matrice delle richieste del prompt iniziale

Destinazioni: **F#** = fase della v1; **dopo la v1 (F#)** = fase condizionata; **Tolta** = fuori dal perimetro per DEC 3, con il motivo.

| # | Richiesta [PROMPT] | Destinazione | Nota |
|---|---|---|---|
| P01 | Testi e accordi leggibili (ChordPro) | F2, F7 | Accordi sopra il testo con `GraphicsView`, guidato dai token |
| P02 | Tablature per chitarra (visualizzazione) | F2, F7 | Blocchi tab in monospazio [PC §3.2 riga 2] |
| P03 | Trasposizione delle tab | **Tolta** | Rara nella liturgia, costo alto |
| P04 | Accordi per pianoforte (note dell'accordo) | Dopo la v1 (F19) | In v1 la modalità "Tastiera" mostra le sigle |
| P05 | Trasposizione su/giù | F2, F7 | Logica esistente e testata |
| P06 | Scelta chitarra/pianoforte | F7 | Chitarra / Tastiera / Solo testo |
| P07 | Notazione Do-Re-Mi o C-D-E, diesis/bemolle | F2, F6 | Predefinita dalla lingua (DEC 9) |
| P08 | DB privato locale SQLite | F6 | Persistenza rifatta |
| P09 | DB pubblico, canzoni pubbliche/private | **Tolta** (DEC 3) | Rischio legale; sostituita da file e QR (F8) e canti di dominio pubblico (F14) |
| P10 | Sync su Dropbox/Drive/OneDrive | **Tolta** come integrazione (DEC 3); F8 backup su file | Il selettore di file salva anche in una cartella cloud |
| P11 | Export/import del DB o di un sottoinsieme | F8 | Archivio ChordPro + manifest JSON, uguale su Android e Windows |
| P12 | Metadati | F6 | — |
| P13 | Link YouTube | F6 | Apre l'app esterna |
| P14 | MP3 | Dopo la v1 (F19) | Pesano nei backup |
| P15 | PDF associati | Dopo la v1 (F19) | Come P14 |
| P16 | Momenti della Messa | F6, F8, F10 | Metadati, scaletta per momenti, slot della celebrazione |
| P17 | Riferimenti biblici | F6, F9, F10 | Solo riferimenti; collegati alle letture del giorno |
| P18 | Tag generici | F6 | — |
| P19 | Riconoscimento audio | **Tolta** (DEC 3) | — |
| P20 | Accordi suggeriti dall'AI | **Tolta** (DEC 3) | Copyright; modello dismesso [VAL §4.2] |
| P21 | Import da PDF testuale | F3, F7 | Sul dispositivo, su entrambe le piattaforme |
| P22 | OCR di PDF scansionati | F3, F7 se la soglia del Gate 0 lo consente | Import in blocco dopo la v1 (F18) |
| P23 | Scraping da siti di accordi | **Tolta** (DEC 3); "incolla" in F2 e F7 | — |
| P24 | Album manuali con ordinamento | F6 (raccolte), F8 (scalette e repertori) | — |
| P25 | Album automatici con l'AI | **Tolta** in forma AI; F8 filtri, F10 suggerimenti locali | Nessuna AI in cloud |
| P26 | Offline | F6 per costruzione, verifica in F14 | La pubblicità usa la rete ma non blocca nulla |
| P27 | Copyright: avvisi, T&C, dichiarazione, stato legale, uploader | F6, F7, F8, F14; uploader dopo la v1 (F17) | Senza server non c'è un uploader |
| P28 | Privacy | F11 (consenso), F14 (informativa) | Nessun account; dati raccolti solo dall'SDK pubblicitario e dagli store |
| P29 | Multilingua IT/EN | F6 (resx, italiano); inglese in F14 **[IPOTESI]** | Il predefinito della notazione segue la lingua |
| P30 | CRUD canzoni | F6 | — |
| P31 | Ricerca per titolo, autore, tag | F6 (+ incipit) | — |
| P32 | Filtri per tonalità, tipo, lingua, momento, riferimento biblico | F8 (tonalità, tipo, lingua, momento), F10 (riferimento biblico) | — |
| P33 | Export di canzone e album | F8 | ChordPro; JSON nel manifest |
| P34 | Android | F1, F13, F14, F15 | — |
| P35 | Windows desktop | F1, F3, F7, F13, F14, F15 | **In v1** (DEC 4); prima del piano commerciale era condizionata |
| P36 | iOS/macOS | Dopo la v1 (F16) | Solo se il censimento lo giustifica |
| P37 | Funzioni attivabili per contesto legale | F6 (meccanismo), F14 (verifica) | Flag anche per pubblicità, acquisti e collegamento alle donazioni |
| P38 | Interfacce dei servizi esterni | F1 (contratti); `IOcrService` in F7 | Le implementazioni HTTP escono (DEC 3) |
| P39 | Account e login | **Tolta** in v1 (DEC 3); dopo la v1 (F17) solo per i responsabili | — |

Le richieste del prompt sul **formato della risposta** (domande, architettura, modello dati, flussi UX, interfacce, esempi di codice) sono coperte dai documenti di F0 e da questo piano.

---

## 9. Mappa dalle 17 fasi vecchie alle nuove

| Vecchia fase [FP] | Stato dichiarato → reale [VAL] | Dove finisce |
|---|---|---|
| 1 Setup e architettura | Completa → build fragile | **F1** |
| 2 Modello dati e database | Completa → Domain buono, migrazioni rotte | Domain tenuto; persistenza e modello comune alle tre modalità in **F6** |
| 3 UI base e navigazione | Completa → bug di navigazione | **F6** (tre navigazioni), **F4** (temi) |
| 4 Funzionalità core canzoni | Parziale → parser buono, rendering grezzo, salvataggio rotto | **F2**, **F6**, **F7** |
| 5 Album e raccolte | Completa → funzionante | **F6** (raccolte), **F8** (scalette e repertori) |
| 6 Tag e metadati | Completa → filtri avanzati no | **F6**, **F8** (filtri), **F10** (riferimento biblico) |
| 7 Servizi esterni AI | Completa → solo mock | **Tolta** (DEC 3) |
| 8 Servizi esterni OCR | Completa → mock e stub | **F3** (prova), **F7** (OCR sul dispositivo) |
| 9 Audio recognition | → solo mock | **Tolta** (DEC 3) |
| 10 Sync cloud multi-provider | Parziale → cartella locale | **F8** (backup su file); provider **tolti** |
| 11 Database pubblico e backend | Completa → risponde 500 | **Congelato in F1**; eventuale server della sola scaletta in **F17** |
| 12 Testing backend | Completa → mock che nascondono il bug | Esce dalla CI con il backend (F1) |
| 13 Client MAUI per il backend | Completa → URL sbagliati | **Tolta** (DEC 3) |
| 14 Testing e refinement | 40% → test rossi | **F1** (CI e copertura), **F5** (prove visive), **F6** (SQLite reale) |
| 15 Modalità offline | 70% → codice morto | Offline per costruzione (**F6**); codice eliminato in **F1** |
| 16 Funzionalità avanzate | 25% → ricerca avanzata rotta | **F8** (filtri), **F4** (cinque temi chiari e scuri); raccolte AI tolte |
| 17 Production deployment | → CI mai verde | **F1** (CI), **F13** (Play e Windows), **F15** (produzione e Store) |

---

## 10. Calendario di riferimento

Con più di 15 h/settimana (DEC 2) e la capacità del §2.2. I numeri tra parentesi sono round.

| Periodo | Capacità | Tecnica | Traguardi | Proprietario |
|---|---|---|---|---|
| 11/11/2026 – 31/01/2027 | ≈ 16 | F1 (4, in corso), F2 (3), F3 (4), F4 prima parte (3: specifica, Leggio chiaro, Palco scuro), F6 persistenza (2) | **Gate 0** 31/01 | Attività del Gate 0; taste-skill; tablet, pedale, PC Windows; commercialista; V-07, V-09 |
| Febbraio 2027 | 12 | F4 resto (2), F5 (3), F6 resto (4), F9 motore (3) | — | D10 entro il 28/02; calendari di riferimento per F9 |
| Marzo 2027 (Settimana Santa 21-28/03) | ≈ 12 | F7 (6), F13 parte 1 (3), F9 tabella (3) | **T1** entro il 21/03 | Account Play e segreti; V-08, V-10; D12 per la sede delle release |
| Aprile 2027 | 12 | F8 (6), F10 (4), F13 parte 2: test chiuso (2) | Anteprima dal 02/05 | Cori e ≥ 12 tester; account AdMob e Partner Center |
| Maggio – giugno 2027 | ≈ 10 + riserva | F11 (3), F12 (4), F14 anteprima (3) | **Gate Anteprima** 20/06 | V-01-V-06; profili pagamenti; incarico al legale |
| Luglio – agosto 2027 | riserva (≈ 8) | Correzioni dall'anteprima; invio di prova allo Store; schermate legali | Nessun rilascio in agosto | Parere legale; partita IVA se indicata (V-05); dominio e sito |
| Settembre – ottobre 2027 | 3 | F14 pilota completo (3) | **Gate Pilota** 24/10 | Cori pilota, note settimanali, 3 video |
| Novembre 2027 | 3 | F15 (3) | **Lancio** entro il 21/11 | Schede, pagina di donazione, presenza in 1-2 corsi diocesani |

Totale: 68 round più 8 di riserva. **[IPOTESI]** Se le ore reali misurate nel Gate 0 sono vicine a 15 e non oltre, la capacità da febbraio scende a circa 2,5 round a settimana: la riserva estiva si consuma e il lancio resta possibile entro il 21/11/2027 solo senza altri ritardi; altrimenti vale il ripiego della Quaresima 2028 (§1.2).

---

## 11. Ordine dei prossimi tre round

F1 è in corso su `f1/risanamento-ci` con un altro agente: i round qui sotto partono dopo l'unione della sua PR (job core verde su `main`) e non toccano i file di F1.

| Round | Fase | Contenuto | Sotto-agenti | Risultato | Dal proprietario prima di iniziare |
|---|---|---|---|---|---|
| **1** | F2 | Ramo `f2/nucleo-musicale`: notazione Do-Re-Mi e C-D-E con predefinito dalla lingua, minore "Lam", bemolle, capotasto, blocchi tab (test prima); progetto del modello di impaginazione | 1 Sonnet (TDD notazione e trasposizione), 1 Opus (modello di impaginazione), 1 Haiku (riallineamento di `ROADMAP.md` a questo piano, in un ramo suo); review Opus xhigh | PR unita con CI verde; report dei test di notazione | Avvio del Gate 0 (16/11); 10-20 canti reali come casi di test; tablet, pedale e PC Windows ordinati |
| **2** | F2 + F3 | Chiusura di F2 (impaginazione, "incolla"); avvio di F3: controllo `GraphicsView` e tasti su Android e Windows; banco di prova dell'import | 2 Sonnet (traccia A, traccia B), 1 Opus (euristica di import); review Opus xhigh | Primo canto disegnato su tablet e PC; primo report parziale dell'import | 10 libretti PDF reali; esito di V-07 per ML Kit |
| **3** | F3 + F4 | Chiusura della traccia A con il pedale; specifica dei token e dei componenti con la taste-skill; dizionari Leggio chiaro e Palco scuro generati dalla fonte unica; test dei contrasti | 1 Sonnet (F3), 1 Opus con la taste-skill (specifica dei token), 1 Sonnet (generazione dei dizionari e test); review Opus xhigh | Video del pedale su tablet e PC; catalogo dei componenti nei due temi predefiniti | **taste-skill installata**; scelta del set di icone |
