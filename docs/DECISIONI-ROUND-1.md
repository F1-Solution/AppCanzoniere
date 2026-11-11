# Decisioni del round 1: ripresa di AppCanzoniere

Data: 11/11/2026
Fonte: intervista di progettazione "Ripresa AppCanzoniere: decisioni di fine round 1" (16 domande, 5 termini). Questo documento riporta le decisioni prese dal proprietario e le motivazioni registrate nell'intervista. Chi legge non ha accesso all'intervista: il documento è completo per lavorare senza altre fonti.

Convenzioni: "Q1"…"Q16" indicano il numero della domanda. "Raccomandazione" è la scelta proposta dall'agente con la sua motivazione. "Vincolante" significa decisione durevole, da rispettare nei documenti e nel codice finché non viene riaperta. Le citazioni tra virgolette sono testo esatto del proprietario.

---

## 1. Sintesi

Il round ha chiuso le decisioni che sbloccano la ripresa del progetto. Lo scopo è un servizio ai cori che si autofinanzia, con un'opzione commerciale verso parrocchie e diocesi; non è stato scelto il reddito come obiettivo principale (Q1). Il proprietario ha dichiarato più di 15 ore a settimana (Q2, risposta finale; la prima risposta A "Fino a 5 ore" è stata sostituita) e, in una risposta libera successiva, di aver aumentato le ore (Q5). La prima versione resta più piccola del perimetro originale del prompt (Q4), ma con due differenze rilevanti rispetto alla raccomandazione: include Windows insieme ad Android fin da subito, e include tutte e tre le modalità (Libreria, Scaletta, Liturgia) con calendario liturgico completo (Q4, Q7). Restano cinque temi visivi, con Ibrido (tema chiaro di Leggio, tema scuro di Palco) come predefinito (Q8). La notazione predefinita è Do-Re-Mi con minore "Lam" per l'italiano, C-D-E per l'inglese (Q9). Il calendario liturgico è calcolato sul dispositivo, solo per il rito romano con il calendario italiano (Q10). Il modello di ricavo è gratuito con pubblicità, donazioni volontarie e un Pro a 4,99 € che toglie la pubblicità; la raccomandazione prevedeva invece gratuito senza pubblicità e ricavi da parrocchie e diocesi (Q11). La pubblicità compare in banner nelle schermate di elenco e di gestione e in un annuncio a tutto schermo all'apertura, mai sul canto, in modalità leggio o in una scaletta aperta (Q13). Le donazioni sono acquisti facoltativi nell'app più una pagina sul sito (Q14). Su Windows il pacchetto scaricabile arriva durante il pilota e il Microsoft Store al lancio (Q12). Il proprietario ha approvato l'installazione di taste-skill per la specifica di token e componenti, senza rifare le proposte di design (Q15). Il lavoro del round 2 parte in parallelo: riscrittura di FASI.md e avvio di F1 su rami con pull request (Q16). I documenti del round 1 sono già stati pubblicati su main (Q3).

Diverse scelte si discostano dalla raccomandazione (Q2, Q4, Q5, Q7, Q11, Q13, Q14). Nel complesso allargano la prima versione rispetto al piano commerciale e a docs/FASI.md: FASI.md e le date vanno riscritti (Q16). Le conseguenze sono elencate nella sezione 7 e le domande ancora aperte nella sezione 9.

---

## 2. Termini

**Gate 0**: validazione commerciale prima di scrivere codice: interviste ad animatori, cori pilota, prova dell'import su libretti PDF veri, verifiche legali e fiscali minime; scadenza proposta 31/01/2027.
Parole da evitare: "fase 0", "MVP".

**Prima versione**: la versione pubblicabile proposta dal piano commerciale: solo Android, tutto offline, senza backend, account né pagamenti. Nota: le decisioni Q4, Q11 e Q13 hanno cambiato questa definizione (Windows, pagamenti in app e pubblicità); la definizione resta quella del piano commerciale finché FASI.md non viene riscritto.
Parole da evitare: "MVP", "v1 completa".

**Round**: un ciclo di orchestrazione: l'agente principale lancia 2-3 sotto-agenti e chiude con tabella delle fasi, richieste al proprietario e prompt successivo.
Parole da evitare: "sprint", "wave".

**Modalità**: una delle tre organizzazioni della schermata iniziale e della navigazione (Libreria, Scaletta, Liturgia) che l'utente sceglie nelle impostazioni; i dati sono gli stessi.
Parole da evitare: "tema", "profilo", "skin".

**Tema**: una delle cinque direzioni visive (Leggio, Palco, Spartito, Vetrata, Analogico), ciascuna con variante chiara e scura, che l'utente sceglie nelle impostazioni; non cambia la navigazione.
Parole da evitare: "modalità", "skin".

Attenzione: l'intervista usa la parola "profilo" in due risposte del proprietario (Q6, Q8) per indicare dove cambiare modalità e tema. Il termine "profilo" è da evitare per queste definizioni; il punto è registrato nella sezione 9.

---

## 3. Perché

Lo scopo dell'intervista, nelle parole del campo `intent`: "Continuare il progetto senza ricominciare da capo, con nuova valutazione e nuova pianificazione; il progetto ha scopo commerciale. Servono le decisioni che sbloccano le fasi."

---

## 4. Decisioni vincolanti

Sono le otto domande con decisione durevole: Q1, Q4, Q6, Q7, Q8, Q10, Q11, Q13.

### Q1. Qual è l'obiettivo del progetto?

Contesto: il piano commerciale stima, al netto di imposte, commercialista e legale, da -1,1 K€ (scenario prudente, 45%) a +7,3 K€ (favorevole, 15%) in circa 4 anni, per 900-1.700 ore di lavoro.

**Decisione: B.** "Servizio ai cori che si autofinanzia, con opzione commerciale verso parrocchie e diocesi". Accettata la raccomandazione.

Opzioni scartate:
- **A, "Reddito: deve ripagare il mio tempo"**: perde perché, secondo la motivazione registrata, i numeri non lo sostengono: anche nello scenario favorevole il ricavo è di circa 4 € l'ora, e gli analisti dicono apertamente che per il reddito conviene non commercializzare.
- **C, "Hobby o open source, senza obiettivo economico"**: perde perché B tiene aperta la vendita a parrocchie e diocesi, dove si concentrano i ricavi possibili. Il costo di B rispetto a C, segnalato dalla raccomandazione, è il lavoro di validazione e di vendita, circa un terzo delle ore totali. La motivazione non discute C oltre a questo.

### Q4. Accetti una prima versione più piccola di quanto chiedeva il prompt iniziale?

Contesto: la prima versione proposta è solo Android, tutto offline, senza backend, account né pagamenti. Include libreria ChordPro, trasposizione con Do-Re-Mi e capotasto, scalette con i momenti della Messa, modalità leggio con pedale, import di un canto alla volta da PDF/OCR, condivisione con QR o file, backup ed export. Restano fuori le richieste esplicite del prompt: database pubblico, accordi suggeriti dall'AI, riconoscimento audio, scraping, sincronizzazione con più cloud, Windows. Il perimetro originale costa circa 25-30 round in più; con fino a 5 ore a settimana significa oltre un anno aggiuntivo.

**Decisione: B.** "Ridotta, ma con Windows insieme ad Android fin da subito". Il proprietario ha scelto diversamente dalla raccomandazione, che era A (ridotta, Windows escluso per ora, il resto riaperto solo se un gate lo giustifica).

Costo segnalato dalla raccomandazione per la scelta fatta: Windows, che oggi compila, "raddoppierebbe prove e pubblicazione". La raccomandazione giudicava B "ragionevole solo se tu stesso usi l'app su Windows"; la risposta non registra se il proprietario la usa su Windows.

Opzioni scartate:
- **A, "Sì: prima versione ridotta; il resto si riapre solo se un gate lo giustifica"** (raccomandazione): perde perché il proprietario vuole Windows fin dall'inizio. La motivazione della raccomandazione per A era: rinunciare, per ora, a Windows e alle funzioni richieste esplicitamente.
- **C, "No: mantengo il perimetro originale del prompt"**: perde perché, secondo la motivazione registrata, il perimetro originale non arriva a un rilascio in tempi utili: backend, account e catalogo pubblico sono le parti oggi rotte e quelle con il rischio legale più alto.

Nota: il testo della risposta aggiunge Windows; non dice esplicitamente se database pubblico, accordi AI, riconoscimento audio, scraping e sincronizzazione restano fuori. Il punto è nella sezione 9.

### Q6. Quale direzione UX scegli?

Contesto: tre proposte in docs/design/ux/. A Libreria: home con ricerca e filtri, costo basso-medio. B Scaletta: home con la prossima scaletta, stato di preparazione, tonalità e strumento per serata, modalità Palco; costo medio. C Liturgia: home con la prossima celebrazione, momenti della Messa e canti suggeriti, pianificatore su desktop; costo alto per il calendario liturgico.

**Decisione: tutte e tre le modalità, scelte modificabili dal profilo utente.** Testo esatto della risposta: "Mi piacciono tutte e 3 le modalità. Vorrei poterle modificare dal mio profilo utente". La risposta non sceglie una sola modalità né una schermata iniziale. La raccomandazione era B (Scaletta, con il modello dei momenti della Messa preso dalla C), con la sezione Canti della A.

Punto da chiarire: la risposta dice "profilo utente", ma la nota dell'agente registra che la prima versione non ha account: la scelta sta nelle impostazioni dell'app sul dispositivo, non in un profilo sul server. La definizione del termine "Modalità" usa le impostazioni. Il conflitto è nella sezione 9.

Opzioni non scelte come soluzione unica:
- **A, Libreria**: la risposta non la esclude, ma non la sceglie da sola. La raccomandazione non spiega perché la Libreria da sola non basti, a parte che la sezione Canti della Scaletta è quella della Libreria.
- **B, Scaletta**: non è stata scelta come unica modalità. Il costo segnalato dalla raccomandazione resta valido per la modalità Palco: va prototipata presto su un tablet vero con un pedale vero.
- **C, Liturgia**: non è stata scelta come unica modalità. Il costo segnalato era il calendario liturgico (costo alto), che entra con Q7 e Q10.

### Q7. Le tre modalità arrivano tutte nella prima versione o a scaglioni?

Contesto: Libreria e Scaletta condividono quasi tutto. Liturgia richiede il calcolo del calendario liturgico e la tabella dei riferimenti delle letture (solo riferimenti: i testi CEI sono protetti), stimati in 4-6 round in più, e apre la scelta tra rito romano e ambrosiano.

**Decisione: B.** "Tutte e tre nella prima versione, con calendario liturgico completo". Il proprietario ha scelto diversamente dalla raccomandazione, che era C.

Costo segnalato dalla raccomandazione per la scelta fatta: B "mette il pezzo più costoso e meno verificato prima di aver provato l'app in una chiesa". Costo aggiuntivo registrato dall'agente: "vale circa 4-6 round in più rispetto alla mia raccomandazione"; il calendario è una fase a sé, sviluppata con i test scritti prima, perché il calcolo delle date è deterministico.

Opzioni scartate:
- **A, "Prima versione con Scaletta e Libreria; Liturgia dopo il lancio"**: perde perché il proprietario vuole le tre modalità nella prima versione. La motivazione registrata non spiega il motivo specifico contro A.
- **C, "Tutte e tre nella prima versione, ma Liturgia semplificata"** (raccomandazione): perde perché il proprietario vuole il calendario completo. La motivazione della raccomandazione per C era: costa circa 1-2 round invece di 4-6, ma la modalità Liturgia non propone da sola la celebrazione della domenica né le letture; il calendario si aggiunge dopo, se i cori pilota lo chiedono. La risposta non registra perché il proprietario preferisce il calendario subito.

### Q8. Quale direzione UI scegli?

Contesto: cinque proposte in docs/design/ui/index.html, ognuna con tema chiaro e scuro: 1 Leggio, 2 Palco, 3 Spartito, 4 Vetrata, 5 Analogico.

**Decisione: tutti e cinque i temi, con predefinito B (Ibrido: tema chiaro di Leggio, tema scuro di Palco), e scelta del tema modificabile dal profilo utente.** Testo esatto della risposta: "vorrei mantenere tutti e 5 i temi sono molto belli. tieni come default la raccomandazione B ma consenti all'utente di scegliere il tema dal profilo". La raccomandazione era B; il proprietario l'ha accettata come predefinito.

Punto da chiarire: anche qui compare "profilo", mentre la prima versione non ha account (vedi Q6 e sezione 9).

Costi registrati dall'agente: tutta l'interfaccia è costruita su un unico insieme di token (colori, caratteri, raggi, spaziature), così un tema è un file di valori. I caratteri di cinque temi aumentano il peso dell'app di qualche megabyte. Vetrata è il tema più costoso in MAUI; il suo accento che segue il tempo liturgico è fattibile perché il calendario entra nella prima versione. Ogni rilascio va controllato su cinque temi per due varianti, quindi servono prove automatiche con schermate di confronto nella CI.

Opzioni non scelte come predefinito:
- **A, "2 · Palco in entrambi i temi"**: la raccomandazione preferisce B perché il tema scuro di Palco è quello pensato per la lettura a un braccio di distanza con poca luce, e il tema chiaro di Leggio dà un carattere riconoscibile e vicino al mondo del coro, che a Palco manca.
- **C, "3 · Spartito"**: la raccomandazione dice che rende meglio su Windows ma è la meno adatta al leggio. Il proprietario lo tiene comunque tra i cinque temi disponibili.
- **D, "Un'altra (1 Leggio, 4 Vetrata o 5 Analogico)"**: non è stata scelta come predefinito, né discussa nella raccomandazione. I temi restano tutti e cinque disponibili.

### Q10. Come si ottiene il calendario liturgico, e per quale rito?

Contesto: la modalità Liturgia deve sapere quale celebrazione cade in una data, il tempo liturgico e il ciclo delle letture (anno A/B/C, feriale I/II), e mostrare i riferimenti delle letture senza i testi. Deve funzionare offline. Il rito ambrosiano ha un calendario e un lezionario diversi: è un secondo motore, non una variante.

**Decisione: A.** "Calcolo nostro, sul dispositivo, solo rito romano con il calendario proprio dell'Italia; ambrosiano dopo se richiesto". Accettata la raccomandazione.

Motivazione: il calendario romano si calcola da regole pubbliche a partire dalla data della Pasqua, quindi si scrive con i test prima, funziona offline e non dipende dalla licenza di nessuno; i riferimenti delle letture sono citazioni, non testi. Costo segnalato: compilare e verificare a mano la tabella dei riferimenti del lezionario.

Opzioni scartate:
- **B, "Calcolo nostro, rito romano e ambrosiano dalla prima versione"**: perde perché l'ambrosiano raddoppierebbe il lavoro di tabella per una sola area geografica; la raccomandazione dice di aggiungerlo se un coro pilota lo usa.
- **C, "Dati presi da una fonte o libreria esterna, da verificare per licenza e uso offline"**: perde perché risparmia tempo solo se esiste una fonte con licenza chiara, e la raccomandazione dice che va cercata prima di decidere. Nessuna ricerca della fonte risulta registrata nell'intervista.

### Q11. Quale modello di ricavo adotti?

Contesto: il piano commerciale proponeva lancio gratuito senza limiti né pubblicità; poi licenza diocesana (25 € per coro all'anno, minimo 20 cori) e Piano Coro (49 € l'anno) con fattura, solo quando almeno 10 pagatori si impegnano per iscritto per 2.000 € l'anno; strumenti Pro più avanti. L'alternativa studiata era far pagare subito: Pro a 9,99 € una tantum con limite di 30 canti nella versione gratuita.

**Decisione (testo libero, esatto):** "gratuito con donazioni volontarie ma con ads, pro a 4.99 che toglie gli advertising". Il proprietario ha scelto diversamente dalla raccomandazione, che era A (gratuito al lancio, senza pubblicità; prime entrate da parrocchie e diocesi con fattura).

Costo segnalato dalla raccomandazione per A, cioè per la scelta non fatta: "nessun incasso per almeno un anno e ricavi che dipendono dal convincere istituzioni, cioè da lavoro di relazione tuo". Costo della scelta fatta, registrato dall'agente nella discussione, con tre cambiamenti rispetto al piano commerciale:
1. servono acquisti in app su Google Play e su Microsoft Store già al lancio, quindi una fase in più e prove di acquisto nella CI;
2. la pubblicità richiede il consenso per il trattamento dei dati (GDPR) e una dichiarazione sui dati negli store; l'app non è più del tutto offline: senza rete non mostra annunci;
3. dal primo euro incassato serve una posizione fiscale, da chiarire con un commercialista prima del lancio.

Stima dell'agente, dichiarata come non dato: con poche migliaia di utenti la pubblicità rende poche decine di euro l'anno; il suo ruolo reale è dare un motivo per comprare il Pro.

Opzioni scartate:
- **A, "Gratuito al lancio; prime entrate da parrocchie e diocesi con fattura"** (raccomandazione): perde perché il proprietario preferisce incassare subito con pubblicità, donazioni e Pro. Il costo segnalato è quello sopra. La risposta non include la licenza diocesana né il Piano Coro: vedi sezione 9.
- **B, "Pro una tantum a 9,99 € fin dal lancio, con limite di canti nella versione gratuita"**: perde, secondo la motivazione registrata della raccomandazione, perché il limite di 30 canti si aggira facilmente e un solo Pro basta a tutto il coro, mentre pagamenti in app e partita IVA portano costi fissi prima di avere utenti. La risposta del proprietario non include il limite di canti.
- **C, "Gratuito, con donazioni volontarie"**: non è stata scelta da sola; la risposta include le donazioni, più la pubblicità e il Pro. La raccomandazione non discute C.

### Q13. Dove compare la pubblicità?

Contesto: un annuncio sulla schermata del canto o in modalità leggio durante la Messa sarebbe il modo più rapido per ricevere recensioni negative; gli annunci a tutto schermo tra un canto e l'altro interrompono l'esecuzione. Su Android la rete di riferimento è Google AdMob. Su Windows non risulta una rete pubblicitaria mantenuta per le app MAUI.

**Decisione: B.** "Come A, più un annuncio a tutto schermo all'apertura dell'app", cioè: banner nelle schermate di elenco e di gestione, più un annuncio a tutto schermo all'apertura; mai sul canto, in modalità leggio o in una scaletta aperta. Il proprietario ha scelto diversamente dalla raccomandazione, che era A (solo banner, nessun annuncio a tutto schermo).

Costo segnalato dalla raccomandazione per A, cioè per la scelta non fatta: un incasso pubblicitario più basso di B e C; B è più rischiosa per l'esecuzione, perché l'annuncio a tutto schermo interrompe chi suona.

Vincoli registrati dall'agente per B, da verificare prima di costruire: le regole di AdMob vietano gli annunci interstiziali classici al caricamento dell'app e prevedono il formato "apertura app", da usare dopo aver letto le regole attuali; propone di non mostrare l'annuncio quando l'app si riapre mentre è in corso una scaletta.

Opzioni scartate:
- **A, solo banner** (raccomandazione): non scelta, per la scelta del proprietario sopra.
- **C, "Banner anche sulla schermata del canto, ma non in modalità leggio"**: perde, secondo la motivazione registrata, perché un banner sulla schermata del canto è il modo più rapido per ricevere recensioni negative e contraddice la promessa di affidabilità dell'app.

Windows: la raccomandazione proponeva nessuna pubblicità su Windows finché non si trova una rete affidabile, con la sola donazione. La risposta non dice se questa parte è accettata: vedi sezione 9.

---

## 5. Scelte ordinarie

Sono le otto domande con risposta non durevole: Q2, Q3, Q5, Q9, Q12, Q14, Q15, Q16.

**Q2. Quante ore a settimana puoi dedicare al progetto?**
Decisione: D, "Più di 15 ore" (risposta finale). Il proprietario aveva prima risposto A, "Fino a 5 ore", che è stata sostituita con D. Il file registra solo la risposta finale; il cambio è quello indicato dalla richiesta di scrittura. Raccomandazione: "Indica la cifra realistica, non quella desiderata", senza un'opzione specifica. Il piano assume 12-15 ore a settimana; D è sopra questo intervallo, quindi le date vanno ricalcolate (vedi sezione 9). Il testo di Q5 dice che il proprietario ha poi aumentato le ore.
Opzioni scartate: A, "Fino a 5 ore", sostituita da D; B, "6-10 ore" e C, "11-15 ore" non scelte. La raccomandazione non motiva le opzioni singole; registra solo che con meno di 6 ore Gate 0 e sviluppo non possono procedere in parallelo.

**Q3. Posso fare commit e push dei documenti di questo round su origin/main?**
Decisione: A, "Sì, commit e push diretti su main". Accettata la raccomandazione. Motivazione: sono solo documenti, il proprietario è l'unico sviluppatore e main non ha protezioni; la pull request aggiungerebbe un passaggio senza revisore. Il ramo con pull request diventa utile dalla fase F1. Il push ha fatto ripartire la CI attuale, che fallirà come ad agosto: è atteso.
Opzioni scartate: B, "Sì, ma su un ramo con pull request", perde per la motivazione sopra. C, "No, per ora tutto resta in locale", non discussa dalla raccomandazione. Stato: eseguito, vedi sezione 6.

**Q5. Con fino a 5 ore a settimana, cosa viene prima?**
Decisione (testo libero, esatto): "Ho aumentato le ore settimanal del progetto, quindi possiamo fare sia B che C". La risposta non è tra le opzioni e il suo testo è ambiguo: B prevede round tecnici a ritmo ridotto e il Gate 0 nel tempo che resta; C prevede solo sviluppo, con la validazione fatta con l'app in mano ai cori. Raccomandazione: B. Costo di B segnalato: il Gate 0 slitta verso la primavera 2027 e il lancio verso l'inizio del 2028. La risposta non dice se questo slittamento resta, dato l'aumento delle ore (vedi sezione 9).
Opzioni scartate: A, "Prima il Gate 0, sviluppo fermo fino al suo esito", più prudente ma lascia il codice fermo per mesi (motivazione della raccomandazione). C, "Solo sviluppo", salta la verifica dell'unica ipotesi su cui poggia il piano: che l'app valga più di WhatsApp più PDF. La risposta del proprietario non riprende C in modo esclusivo.

**Q9. Quale notazione degli accordi è predefinita?**
Decisione: A. "Segue la lingua dell'app: Do-Re-Mi in italiano con minore scritto "Lam", C-D-E in inglese". Accettata la raccomandazione. Motivazione: chi suona in un coro italiano legge Do-Re-Mi, e "Lam" è la scrittura più diffusa nei canzonieri e nei libretti. Costo segnalato: un caso in più da provare. Stato nel codice: la scelta esiste nelle impostazioni, ma nessuna parte dell'app la legge (vedi sezione 6).
Opzioni scartate: B, "Segue la lingua dell'app, con minore scritto "La-"", perde perché "La-" si confonde con il trattino di sillabazione nei testi. C, "Sempre C-D-E finché l'utente non la cambia", non discussa dalla raccomandazione.

**Q12. Come si distribuisce l'app su Windows?**
Decisione: C. "Pacchetto scaricabile durante il pilota, Microsoft Store al lancio". Accettata la raccomandazione. Motivazione: durante il pilota i tester sono pochi e noti, quindi un pacchetto scaricabile basta e non richiede di aspettare la revisione dello Store; al lancio lo Store toglie il costo annuale del certificato di firma e gli avvisi di sicurezza. Costo segnalato: configurare due pipeline di rilascio per Windows in momenti diversi. Da verificare: costo attuale dell'account sviluppatore Microsoft e requisiti dello Store per le app MAUI.
Opzioni scartate: A, "Microsoft Store", perde perché il pilota non deve aspettare la revisione dello Store. B, "Pacchetto scaricabile dalle release di GitHub, con certificato di firma", perde per il costo annuale del certificato e gli avvisi di sicurezza, che scoraggerebbero un pubblico poco tecnico.

**Q14. Come si raccolgono le donazioni?**
Decisione: C. "Entrambe: acquisti facoltativi nell'app e pagina di donazione sul sito". Il proprietario ha scelto diversamente dalla raccomandazione, che era A (acquisti facoltativi nell'app, per esempio 2, 5 e 10 €, con il sistema dello store). Costo segnalato per la parte nell'app, che resta: la commissione dello store, circa il 15% su ogni donazione. La raccomandazione diceva che C "si può aggiungere dopo, quando esiste un sito". Registrato dall'agente: la pagina sul sito richiede un dominio e un servizio di pagamento esterno, e va nella fase di lancio, non prima. Nell'app non ci sarà un collegamento alla pagina esterna finché le regole degli store non sono verificate. In ogni caso le donazioni sono incassi: il commercialista deve dire come dichiararle.
Opzioni scartate: B, "Solo dal sito del progetto, con un servizio esterno", perde perché quasi nessuno arriverà alla pagina senza un collegamento nell'app. A non è stata scelta da sola: C la include.

**Q15. Rifaccio le proposte di design con la skill taste-skill?**
Decisione: B. "Installo taste-skill e la usi da qui in avanti, per la specifica dei token e dei componenti, senza rifare le proposte". Accettata la raccomandazione. Motivazione: le proposte sono piaciute tutte, quindi rifarle costerebbe un round senza una decisione da cambiare; la skill serve nel passo successivo, quando i cinque temi diventano un unico sistema di token e componenti reali. Costo: il proprietario deve installarla. Stato: taste-skill non risultava installata nell'elenco delle skill attive; vedi sezione 6 e sezione 9.
Opzioni scartate: A, "No: tengo le proposte attuali", non scelta; la raccomandazione dice che non blocca nulla. C, "Installo taste-skill e rifai le proposte da capo", perde perché rifarle costerebbe un round senza una decisione da cambiare.

**Q16. Come parte il round 2?**
Decisione: A. "In parallelo: un agente riscrive FASI.md con le nuove decisioni, altri due avviano F1 su un ramo con pull request". Accettata la raccomandazione. Motivazione: F1 (risanamento del repo e CI verde) non dipende dalle decisioni, salvo una: la CI deve compilare anche Windows. Da F1 in poi si lavora su rami con pull request e si unisce solo con la CI verde. Costo segnalato: a fine round due cose da verificare insieme, il nuovo piano e la pull request. I tre agenti restano nel limite di 2-3 alla volta.
Opzioni scartate: B, "Prima la riscrittura di FASI.md, che approvo; poi F1", perde perché ritarda di un round la CI, oggi rossa. C, "Solo F1 adesso; FASI.md si aggiorna dopo", non discussa dalla raccomandazione.

---

## 6. Fatti verificati

Questi fatti sono stati stabiliti esplorando il repo e i sistemi, non chiedendo al proprietario:

1. Il repo locale è collegato a github.com/F1-Solution/AppCanzoniere, repository privato (verificato con `gh repo view`: visibilità PRIVATE). I documenti del round 1 sono stati pubblicati su main con il commit 155b696 ("docs: add restart assessment, design proposals, business plan and new phase plan"); origin/main punta a quel commit.

2. La skill taste-skill non era installata sulla macchina il 10/11/2026.
   Nota di verifica dell'11/11/2026: una copia di taste-skill (versione 2.5.1) è presente nella cache dei plugin, con data di cartella 10/11/2026, ma la skill non compare tra quelle attive della sessione. Il punto è nella sezione 9.

3. Secondo docs/VALUTAZIONE.md:
   - 79 test passano e 10 falliscono (totale 89; il "77/77" del README esclude gli integration test);
   - il backend avviato risponde con errore 500 sia a `GET /api/songs` (`no such table: Songs`, le migrazioni non sono applicate all'avvio) sia a `POST /api/account/register`;
   - la scelta della notazione nelle impostazioni non è letta da nessuna parte del codice (il documento dice "nessuno legge le impostazioni"). Verifica di conferma: `ChordNotation` compare solo nel modello `UserSettings`, nella pagina e nel ViewModel delle impostazioni e nelle migrazioni; nessun altro codice lo legge.

---

## 7. Rischi

Ricavati dalle motivazioni e dalle discussioni dell'intervista.

- **Perimetro allargato rispetto al piano commerciale.** Windows insieme ad Android (Q4), tre modalità con calendario liturgico completo (Q7), cinque temi (Q8), pubblicità, acquisti in app e donazioni (Q11, Q13, Q14). Il testo di Q16 dice che FASI.md e le date vanno riscritti. Il costo di Windows, segnalato dalla raccomandazione, è il raddoppio di prove e pubblicazione.
- **Acquisti in app e posizione fiscale dal primo incasso** (Q11). Servono acquisti in app su Google Play e Microsoft Store già al lancio, prove di acquisto nella CI, e un chiarimento fiscale con il commercialista prima del lancio.
- **Consenso per la pubblicità** (Q11, Q13). Serve la raccolta del consenso (GDPR) e la dichiarazione sui dati negli store. L'app non è più del tutto offline: senza rete non mostra annunci.
- **Cinque temi per due varianti da provare a ogni rilascio** (Q8). Servono prove automatiche con schermate di confronto nella CI. Il peso dell'app cresce di qualche megabyte con i caratteri. Vetrata è il tema più costoso in MAUI.
- **Calendario liturgico e tabella dei riferimenti delle letture** (Q7, Q10). La tabella va compilata e verificata a mano; la raccomandazione stima 4-6 round in più per la modalità Liturgia completa. Il calcolo è deterministico e si scrive con i test prima.
- **Modalità Palco da provare su tablet vero con pedale vero** (Q6). La raccomandazione dice che va prototipata presto.
- **Regole AdMob e degli store ancora da verificare** (Q13, Q14). Il formato "apertura app" deve essere confermato sulle regole attuali; le regole degli store sui pagamenti esterni non sono state verificate sul testo aggiornato.
- **Commissione dello store sulle donazioni** (Q14): circa il 15% su ogni donazione nell'app.
- **Pubblicità su Windows forse non praticabile** (Q13): non risulta una rete pubblicitaria mantenuta per le app MAUI su Windows.
- **Date del piano** (Q2, Q5, Q16). La risposta D (più di 15 ore) è sopra l'intervallo di 12-15 ore su cui il piano è stimato; le date vanno ricalcolate.
- **CI rossa e Windows nella CI** (Q3, Q16). Il push su main fa ripartire una CI che fallirà come ad agosto; la CI deve compilare anche Windows prima di unire le pull request.

---

## 8. Rinviate

Nessuna. Tutte e 16 le domande hanno stato "answered"; nessuna è rinviata (deferred).

---

## 9. Punti aperti

Emersi nelle discussioni e rimasti senza decisione o da verificare:

1. **"Profilo utente" contro impostazioni** (Q6, Q8). Il proprietario chiede di cambiare modalità e tema "dal profilo utente", ma la prima versione non ha account e la nota dell'agente, nella stessa domanda Q6, dice che la scelta sta nelle impostazioni sul dispositivo. Da decidere dove stanno modalità e tema.
2. **Windows nella prima versione** (Q4). La raccomandazione diceva che B è ragionevole solo se il proprietario usa l'app su Windows; la risposta non lo registra. Inoltre non è chiaro se database pubblico, accordi AI, riconoscimento audio, scraping e sincronizzazione restano fuori.
3. **Gate 0 e sviluppo** (Q5). La risposta libera dice "sia B che C", ma B e C sono in conflitto sulla validazione. Non è chiaro se il Gate 0 resta nel tempo che resta, né quali date valgono dopo l'aumento delle ore.
4. **Date e FASI.md** (Q2, Q16). Le date del piano vanno ricalcolate con la risposta D. FASI.md va riscritto con le nuove decisioni, da un agente in parallelo a F1.
5. **Licenza diocesana e Piano Coro** (Q11). La risposta non dice se restano nel modello di ricavo; la raccomandazione dell'agente li prevedeva come prime entrate da parrocchie e diocesi.
6. **Rete pubblicitaria su Windows** (Q13). Da verificare se esiste una rete utilizzabile. Se non esiste, la raccomandazione prevede nessuna pubblicità su Windows e la sola donazione; la risposta non registra questa parte.
7. **Formato dell'annuncio all'apertura** (Q13). Va verificato sulle regole AdMob attuali il formato "apertura app", e va deciso se non mostrarlo quando l'app si riapre durante una scaletta (proposta dell'agente, non confermata).
8. **Collegamento dall'app alla pagina di donazione esterna** (Q14). Non ci sarà finché le regole degli store non lo consentono; la verifica non è stata fatta.
9. **Costo dell'account sviluppatore Microsoft e requisiti dello Store per le app MAUI** (Q12). Da verificare.
10. **Rito ambrosiano** (Q10). Rinviato: si aggiunge se un coro pilota lo usa.
11. **Fonte del calendario e dei riferimenti** (Q10). La raccomandazione chiede di cercare prima una fonte con licenza chiara; nell'intervista non risulta che la ricerca sia stata fatta.
12. **Installazione di taste-skill** (Q15). A carico del proprietario. Da confermare: la verifica dell'11/11/2026 ha trovato una copia nella cache dei plugin con data 10/11/2026, mentre la skill non compare tra quelle attive; quindi non è chiaro quando e come è stata installata.
13. **Regole degli store sui pagamenti esterni e dichiarazione fiscale delle donazioni** (Q14). Da verificare sul testo attuale delle regole e con il commercialista.
14. **Posizione fiscale dal primo incasso** (Q11). Da chiarire con il commercialista prima del lancio.
