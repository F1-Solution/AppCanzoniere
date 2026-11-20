# Verifiche esterne e professionisti

Questo documento elenca ciò che solo il proprietario può fare. Sono verifiche su siti di terzi, console, account, commercialista e legale. L'agente non può farle, oppure può farle solo in parte.

Stato al 20/11/2026.

Fonti:
- `docs/FASI.md`, sezione 6 (V-01 - V-11) e sezione 0 (decisioni DEC e D10-D13).
- `docs/verifiche/V-01-V-02-pubblicita.md`.
- `docs/verifiche/V-03-V-04-V-05-store-e-fisco.md`.
- `docs/verifiche/V-07-mlkit.md`.
- `docs/business/05-PIANO-COMMERCIALE.md`, sezioni 9.2, 9.3 e 11.

ATTENZIONE: i documenti di verifica non sono consulenza legale o fiscale. Il parere di un professionista ha la precedenza su quanto scritto qui.

---

## 1. Stato delle verifiche

| Codice | Argomento | Stato | Fase bloccata | Chi fa il passo successivo |
|---|---|---|---|---|
| V-01 | Regole AdMob: formato "apertura app", banner, condizioni per annunci reali | Verificata dall'agente l'11/11/2026. Restano 7 controlli a mano (il punto 8 serve solo per Windows) | F11, F15 | Proprietario |
| V-02 | Pubblicità su Windows | Verificata dall'agente l'11/11/2026. Conclusione: nessuna pubblicità su Windows in v1 | F11 | Proprietario: conferma |
| V-03 | Donazioni nelle regole di Google Play e Microsoft Store | Verificata sul testo. Resta 1 controllo: una domanda scritta a Google | F12, F15 | Proprietario |
| V-04 | Requisiti del Microsoft Store per un'app MAUI; costo e tipo di account | Verificata in gran parte. Restano 4 controlli a mano | F12, F13, F14 | Proprietario |
| V-05 | Aspetti fiscali, dichiarazione trader (DSA) | Parzialmente verificata. Restano 4 controlli a mano e 10 domande al commercialista | F12, F14, F15 | Proprietario e commercialista |
| V-06 | Consenso nel SEE, minori, età del consenso digitale | Non verificata. Nessun documento in `docs/verifiche/` | F11, F14 | Proprietario e legale |
| V-07 | Binding .NET 10 per OCR, pubblicità, consenso, acquisti | OCR (ML Kit): fatta dall'agente il 19/11/2026. Restano Google Mobile Ads, UMP e Play Billing | F11, F12 | Agente (in un round); non serve il proprietario |
| V-08 | Pacchetto Windows del pilota: firma, sede delle release | Non verificata | F13 | Proprietario (con decisione D12) |
| V-09 | Piano GitHub: minuti Actions per repo privato, emulatore, protezione del branch | Non verificata | F1 (in corso), F5, F13 | Proprietario |
| V-10 | Play: livello API, pagine da 16 KB, primo caricamento, tempi dell'account | Non verificata | F13 | Proprietario |
| V-11 | Legale: tabella di soli riferimenti del lezionario | Non verificata | F9, F14 | Proprietario e legale |

**Priorità consigliata:** V-09 (subito, perché F1 è in corso), poi V-05 domanda 6 e commercialista (prima di aprire account), poi V-04 e V-10 (account), poi V-08 e V-06, V-11 e parere legale.

---

## 2. Date da rispettare

| Data | Cosa | Fonte |
|---|---|---|
| 20/01/2027 | Un'ora con il commercialista (attività 12 del Gate 0) | `05-PIANO-COMMERCIALE.md` §5 |
| 01/02/2027 | Google Play: dichiarazione trader per gli account già esistenti | V-05 |
| 28/02/2027 | D10: nome, `ApplicationId` e tipo di account Play e Partner Center | `FASI.md` §0 |
| 01/03/2027 | Google Play: telefono e indirizzo dell'account trader diventano pubblici | V-05 |
| Marzo 2027 | D12: sede delle release Windows del pilota (vedi sezione 7) | `FASI.md` §10 |
| 02/05/2027 | Inizio dell'anteprima ai cori. Il test chiuso Play (12 tester per 14 giorni) deve partire prima | `FASI.md` §1.2 |
| 30/06/2027 | Parere legale per il lancio (≈ 400 €) | `05-PIANO-COMMERCIALE.md` §9.2 |
| 12/09/2027 | Inizio del pilota completo: pubblicità con consenso, acquisti in prova | `FASI.md` §1.2 |
| 24/10/2027 | Gate Pilota | `FASI.md` §1.2 |
| 21/11/2027 | Lancio (entro questa data) | `FASI.md` §1.2 |
| 31/05/2028 | Parere legale per il Piano Coro (≈ 800 €) | `05-PIANO-COMMERCIALE.md` §9.2 |

ATTENZIONE: alcune date non coincidono nei documenti. Vedi la sezione 7.

---

## 3. Incontro con il commercialista

**Scopo:** Decidere la forma fiscale prima di aprire account e incassare.

**Cosa serve:**
- Un'ora. Costo previsto: 100-200 € (attività 12) oppure ≈ 150 € (`05-PIANO-COMMERCIALE.md` §9.3).
- Il tuo contratto di lavoro, o una descrizione scritta delle ore e dell'esclusiva.
- Le stime di ricavo del piano: `05-PIANO-COMMERCIALE.md` §8.
- Una pagina con il modello di ricavo: app gratuita con pubblicità; Pro a 4,99 € una tantum che toglie la pubblicità; donazioni facoltative (DEC 10 e DEC 12 in `FASI.md` §0).
- Questo documento, con le domande.

**Tempo stimato:** 1 ora di incontro (`05` §9.3). Tempo di preparazione: [IPOTESI] 1 ora.

**Passi:**

1. Scrivi la data dell'incontro nella sezione 8 di questo documento.
2. Porta le domande del gruppo 1. Sono le domande da risolvere entro il Gate 0.
3. Porta i gruppi 2 e 3 se il commercialista può trattarli. Altrimenti, chiedi un secondo incontro.
4. Chiedi una risposta scritta per le domande 1, 6 e 8.
5. Annota le risposte. Non metterle nel repository, perché contengono dati personali.
6. Riporta all'agente solo l'esito di ogni domanda, con il numero.

**Gruppo 1 — entro il Gate 0 (20/01/2027):**

1. Con il mio reddito da dipendente, è possibile il regime forfettario? Con quale codice ATECO e quale coefficiente? (`05` §9.3, 1)
2. Contributi: gestione separata INPS o altra cassa? Ci sono minimi fissi? (`05` §9.3, 2)
3. Le lettere d'intento senza incasso creano obblighi? (`05` §9.3, 3)
4. Quando va aperta la partita IVA: al primo incasso, o basta una prestazione occasionale? Serve la ritenuta d'acconto se paga un ente? (`05` §9.3, 4)
5. Qual è il costo annuo reale: tenuta, dichiarazione, fatturazione elettronica? (`05` §9.3, 5)
6. Pubblicità e acquisti in app sono attività abituale (partita IVA) o redditi occasionali? Serve a scegliere il tipo di account Microsoft e la dichiarazione trader su Google. (V-05, 1) — **blocca F13, F14, F15**

**Gruppo 2 — prima del pilota (12/09/2027):**

7. Il forfettario è compatibile nel 2027, visto che la soglia di 35.000 € è prevista per il 2026? (V-05, 2) — **blocca F15**
8. Pagamenti AdMob da Google Ireland senza partita IVA: come si dichiarano? Con partita IVA: serve il VIES e la fattura in inversione contabile? (V-05, 4) — **blocca F14, F15**
9. Ricavi da Play e Microsoft Store: fatturo la mia cessione allo store? Come cambia per Google dal 01/03/2027 ("online marketplace model")? (V-05, 5) — **blocca F12, F15**
10. Le mance comprate tramite store sono ricavi come il Pro, o liberalità? (V-05, 6) — **blocca F12**
11. Donazioni ricevute sul sito con un servizio di pagamento esterno: liberalità, redditi diversi o ricavi d'impresa? Servono ricevute? (V-05, 7; `05` §9.3, 9) — **blocca F15**
12. Trader DSA senza attività registrata: posso usare un indirizzo diverso da quello di residenza, come una casella postale? Quali rischi ci sono se mi dichiaro "non trader"? (V-05, 8) — **blocca F14, F15**
13. Il contratto di lavoro prevede esclusiva o autorizzazioni per attività esterne? Verifica anche con l'ufficio del personale. (V-05, 9) — **blocca F14**
14. Quali obblighi ci sono per gli incassi del pilota (settembre-ottobre 2027), se restano sotto qualche centinaio di euro? (V-05, 10) — **blocca F14**

**Gruppo 3 — prima del Piano Coro (31/05/2028):**

15. Fatture a parrocchie ed enti senza partita IVA: codice destinatario; marca da bollo di 2 € sopra 77,47 €. (`05` §9.3, 7)
16. Vendere lo stesso Piano Coro in app e con fattura: quali effetti fiscali? (`05` §9.3, 8)

ATTENZIONE: il commercialista deve rispondere per iscritto alle domande 1, 6 e 8. Senza risposta scritta, la decisione non è chiusa.

**Risultato atteso:** Una decisione per la forma fiscale. La domanda 6 è decisa prima di aprire l'account Microsoft.

**Cosa fare se non funziona:** Se il commercialista non può rispondere a una domanda, scrivi "non risposta" nella sezione 8. Chiedi un secondo incontro. Non aprire l'account Microsoft finché la domanda 6 non è chiusa.

---

## 4. Parere legale (D11)

**Scopo:** Avere un parere scritto prima del pilota e prima del Piano Coro.

**Cosa serve:**
- Un legale con esperienza in privacy, diritto d'autore e contratti digitali. [DA VERIFICARE: il piano non indica il profilo preciso].
- Budget: ≈ 400 € prima del pilota; ≈ 800 € prima del Piano Coro.
- I documenti `docs/verifiche/V-01-V-02-pubblicita.md` e `docs/verifiche/V-03-V-04-V-05-store-e-fisco.md`.

**Tempo stimato:** [DA VERIFICARE: il piano non indica il tempo di preparazione]. Il resto dipende dal legale.

**Passi:**

1. Scegli il legale. Chiedi un preventivo scritto.
2. Invia le domande del gruppo A (sotto) e i documenti di `docs/verifiche/`.
3. Chiedi una risposta scritta per ogni domanda.
4. Riporta all'agente il numero di domanda e l'esito.
5. Se il legale chiede modifiche al piano, riportale all'agente nella richiesta del round.

**Gruppo A — prima del pilota (parere entro 30/06/2027):**

A1. Importare in app, per uso personale, un libretto PDF di cui si possiede una copia: quali limiti ha l'utente e quali responsabilità ha lo sviluppatore, che non ospita nulla? (`05` §9.2, 1)

A2. Condividere testi tra dispositivi, con file o QR, senza server: lo sviluppatore ha responsabilità? Come si scrivono i termini d'uso e la dichiarazione all'invio? (`05` §9.2, 2)

A3. Canti di dominio pubblico: come si documenta la verifica di ogni canto (autore morto da oltre 70 anni, traduzioni, armonizzazioni)? (`05` §9.2, 3)

A4. Termini d'uso e informativa per un'app senza account, usata anche da minori. Qual è l'età del consenso digitale in Italia? [DA VERIFICARE: il piano indica 14 anni]. (`05` §9.2, 4)

A5. La dichiarazione di "trader" su Google Play per un'app gratuita di una persona fisica è dovuta? Quali dati diventano pubblici? (`05` §9.2, 5)

A6. Nome dell'app e marchio: rischi del termine generico "canzoniere". Conviene registrare il marchio? (`05` §9.2, 6)

A7. Pubblicità: banner e annuncio di apertura, con le regole di posizionamento. (`FASI.md` F11; V-01, sezione "Effetti proposti")

A8. Consenso nel SEE (piattaforma di consenso di Google) e voce "Privacy e pubblicità" nelle impostazioni. (V-06)

A9. Minori nei cori giovani: consenso, pubblico di destinazione su Play, norme "Famiglie". (V-06; V-01, punto 5)

A10. Acquisti in app: termini del Pro e delle donazioni. Il testo "Sostieni lo sviluppo" è una donazione o un acquisto? (V-03, conclusione)

A11. Tabella di soli riferimenti del lezionario (non i testi) e nomi delle celebrazioni: uso consentito? (V-11; `FASI.md` F9)

**Gruppo B — prima del Piano Coro (parere entro 31/05/2028):**

B1. Un server che conserva solo la scaletta (titoli, autori, ordine, tonalità, note, eventuale incipit) può ricadere nell'art. 17 della Direttiva DSM (D.Lgs. 177/2021)? Quali obblighi DSA si applicano? (`05` §9.2, 7)

B2. Un indice liturgico pubblico con titoli, autori, momenti e incipit è libero? (`05` §9.2, 8)

B3. Contratto di licenza diocesana: chi firma, responsabilità, durata, recesso. (`05` §9.2, 9)

B4. Account per i responsabili e per i membri: obblighi GDPR minimi (registro dei trattamenti, nomina del fornitore di hosting, conservazione, cancellazione). (`05` §9.2, 10)

B5. Prezzo fondatore e sconti agli enti: si applica la norma sul prezzo più basso dei 30 giorni? (`05` §9.2, 11)

B6. Se arriva un editore: schema di contratto di distribuzione (royalty, rendiconto, esclusive). (`05` §9.2, 12)

**Risultato atteso:** Una risposta scritta per ogni domanda del gruppo A, prima del 30/06/2027.

**Cosa fare se non funziona:** Se il legale non risponde entro il 30/06/2027, riporta il ritardo all'agente. Il Gate Pilota non si può superare senza il parere (`FASI.md` §1.2).

---

## 5. Account da aprire

Apri gli account in quest'ordine. Il primo punto blocca gli altri.

**Regola:** Un account individuale e un account aziendale non si convertono. Scegli il tipo una volta sola (`V-03-V-04-V-05-store-e-fisco.md`, sezione "Effetti proposti").

| Ordine | Account | Quando | Cosa serve | Decisione collegata |
|---|---|---|---|---|
| 1 | Nessun account: decisione sul tipo di account | Prima di tutto. Entro 28/02/2027 | Risposta del commercialista (domanda 6) | D10 |
| 2 | Dominio ed email dedicata | Prima del test chiuso, e prima di F14 | Un dominio; un'email di progetto. Il dominio serve a `app-ads.txt` e alla pagina di donazione | D12; V-01 punto 4 |
| 3 | Google Play Console (account personale) | Febbraio-marzo 2027. Il test chiuso dura 14 giorni con 12 tester prima della produzione | Documento d'identità con verifica; 25 $ (`FASI.md` F13); email; indirizzo; telefono | D10; V-10 (tempi di verifica) |
| 4 | Profilo dei pagamenti Google (dentro Play Console) | Prima di F12 (aprile-maggio 2027) | Dati fiscali. Verifica se la partita IVA è facoltativa (V-05, controllo 2) | Commercialista (domanda 1, 6, 8) |
| 5 | Microsoft Partner Center (individuale o Company) | Dopo la risposta del commercialista. Prima di F13 parte 2 (aprile 2027), quando l'app va prenotata | Documento e selfie; telefono e indirizzo pubblicabili; per Company: DUNS o documenti d'impresa | D10; V-04 |
| 6 | Tenant Entra ID e app con ruolo Manager | Solo se vuoi l'invio automatico dalla CI. Dopo la prima pubblicazione manuale | Verifica se un account individuale lo permette (V-04, controllo 3) | V-04 |
| 7 | AdMob | Dopo che l'app è registrata su Play. Prima di F11 (maggio 2027) | Collegamento al profilo dei pagamenti; unità pubblicitarie; blocco delle categorie (V-01) | DEC 11; V-01 |
| 8 | Apple Developer | Non ora. Solo se si apre F16 | 99 $ all'anno (`FASI.md` §5) | Gate 1 |

Non servono altri account. V-02 conclude che non c'è una rete pubblicitaria per Windows. Non apri Liftoff o Vungle.

ATTENZIONE: su Play, il telefono e l'indirizzo dello sviluppatore trader diventano pubblici dal 01/03/2027. Scegli un indirizzo pubblicabile prima di compilare il profilo. La domanda 12 del commercialista riguarda proprio questo.

ATTENZIONE: V-04 dice che la registrazione del Partner Center è gratuita per gli individui. `FASI.md` (V-04) dice "storicamente circa 19 $ una tantum". Verifica il costo attuale sulla pagina di registrazione prima di pagare. Vedi la sezione 7.

**Risultato atteso per ogni account:** Account attivo, con verifica completata. Il nome e l'identificativo sono quelli decisi con D10.

**Cosa fare se non funziona:** Se la verifica dell'identità è bloccata, annota il messaggio della console. Riporta all'agente il testo esatto. Non ripetere la verifica più volte nello stesso giorno.

---

## 6. Passi per le verifiche a mano

### V-01 — AdMob e regole della pubblicità

**Scopo:** Confermare le regole e configurare AdMob prima della pubblicità reale.

**Passi** (fonte: `docs/verifiche/V-01-V-02-pubblicita.md`, sezione "Cosa resta da verificare a mano", punti 1-7; il punto 8 riguarda Vungle e serve solo per Windows):

1. In AdMob, apri **Blocking controls → Manage sensitive categories**. Decidi cosa bloccare. Le categorie sono: Religion, Dating, Politics, Reference to sex, Get rich quick, Social Casino Games. Poi apri **Blocking controls → Ad content** e imposta il rating massimo (G o PG consigliato).
2. Apri **Payments → Payments info**. Inserisci il profilo di pagamento. Verifica l'identità se richiesto. Inserisci il PIN postale. Controlla la soglia in euro mostrata nel tuo account.
3. Dopo la pubblicazione su Play, controlla **App → Overview → App readiness** in AdMob.
4. Registra un dominio. Pubblica `https://<dominio>/app-ads.txt`. Aggiungi il sito in Play Console (**Presenza nello store → Dettagli dello store → Sito web**). Controlla in AdMob (**Apps → app-ads.txt**).
5. In Play Console, apri **Contenuti dell'app → Pubblico di destinazione e contenuti**. Decidi se includere i minori di 13 anni (cori giovanili). Includerli cambia le regole.
6. In AdMob, crea il messaggio GDPR (UMP) per il SEE. Testalo con un dispositivo di prova.
7. Chiedi al supporto AdMob, per iscritto: (a) una lista di canti creata dall'utente è "publisher-content"? (b) un banner subito dopo l'annuncio di apertura viola la regola?

**Risultato atteso:** Categorie bloccate; sito con `app-ads.txt`; messaggio UMP testato; risposte scritte di AdMob.

**Cosa riportare:** "V-01: punti 1-7, esito per punto. Risposta AdMob punto 7a e 7b: testo."

### V-02 — Pubblicità su Windows

**Scopo:** Confermare che non ci sono annunci su Windows in v1.

**Passi:** Non c'è un passo tecnico. Leggi la conclusione in `docs/verifiche/V-01-V-02-pubblicita.md` e conferma per iscritto che Windows resta senza pubblicità in v1.

**Risultato atteso:** Conferma scritta. Vale per F11.

### V-03 — Donazioni

**Scopo:** Chiarire con Google se un pagamento volontario deve passare dal suo sistema.

**Passi** (fonte: `docs/verifiche/V-03-V-04-V-05-store-e-fisco.md`, sezione "Cosa resta da verificare", punto 9):

1. Apri il modulo di supporto delle policy di Google Play.
2. Chiedi, per iscritto: una mancia allo sviluppatore, senza contenuti in cambio, rientra nel caso "peer-to-peer" o richiede Play Billing?
3. Conserva la risposta. Riportala all'agente.

**Risultato atteso:** Risposta scritta di Google. Decide i testi di F12 e il collegamento di F15.

### V-04 — Microsoft Store e account

**Scopo:** Verificare il tipo di account e le capacità dell'app.

**Passi** (fonte: `docs/verifiche/V-03-V-04-V-05-store-e-fisco.md`, sezione "Cosa resta da verificare", punti 1-4):

1. Apri `https://storedeveloper.microsoft.com`. Scegli **Individual**. Verifica che l'Italia sia tra i paesi e che non ci siano costi. Prima di confermare, rileggi la frase sul tipo di account. Decidi con il commercialista se serve un account Company.
2. Crea una sottomissione di prova, riservata e mai pubblica. Controlla il campo di giustificazione per `runFullTrust`. Controlla che "Private audience" sia disponibile. Crea un add-on durevole e uno consumabile, per vedere i tipi offerti.
3. Verifica se un account individuale può collegare un tenant Entra ID e creare un'app con ruolo Manager. Senza questo, niente invio automatico dalla CI.
4. Leggi l'elenco dei paesi con imposta gestita da Microsoft (Allegato A). Conferma che l'Italia e l'UE siano presenti.

ATTENZIONE: la sottomissione di prova non deve mai diventare pubblica. Cancellala o lasciala in bozza.

**Risultato atteso:** Tipo di account scelto; risposte ai punti 2-4.

### V-05 — Fisco, dichiarazione trader e dati pubblici

**Scopo:** Sapere quali dati diventano pubblici e quali obblighi fiscali esistono prima di incassare.

**Passi** (fonte: `docs/verifiche/V-03-V-04-V-05-store-e-fisco.md`, sezione "Cosa resta da verificare", punti 5-8):

1. Apri in Play Console la dichiarazione trader. Annota quali campi diventano pubblici. Verifica se puoi indicare un indirizzo diverso dalla residenza (casella postale o domiciliazione). Fai uno screenshot. Non pubblicarlo nel repository.
2. In Play Console, apri il profilo dei pagamenti. Verifica se la partita IVA è facoltativa per un profilo individuale italiano. Verifica quale indirizzo viene mostrato con la monetizzazione.
3. In Play Console, crea in bozza un prodotto consumabile "Sostieni lo sviluppo". Leggi gli avvisi della console. Non pubblicarlo.
4. In AdMob, apri **Pagamenti**. Verifica quali dati fiscali chiede a una persona fisica italiana senza partita IVA. Verifica come sarà intestato il documento di pagamento.

**Risultato atteso:** Elenco dei campi pubblici; risposta su indirizzo e partita IVA; screenshot in un luogo privato.

**Cosa fare se non funziona:** Se un campo non è chiaro, chiedi al commercialista (domanda 12). Non scegliere l'indirizzo di residenza solo per fretta.

### V-06 — Consenso nel SEE e minori

**Scopo:** Attivare il consenso richiesto da Google nel SEE e decidere il pubblico.

**Passi:**

1. Fai il punto 6 di V-01: il messaggio UMP in AdMob, testato su un dispositivo di prova.
2. Porta al legale le domande A8 e A9.
3. Verifica l'età del consenso digitale in Italia. [DA VERIFICARE: il piano indica 14 anni].

**Risultato atteso:** Messaggio UMP attivo in test. Parere del legale su minori.

### V-07 — Binding per pubblicità, consenso e acquisti

**Stato:** La parte OCR è fatta. Restano Google Mobile Ads, UMP e Play Billing.

**Passi per te:** Nessuno. È una verifica sui pacchetti NuGet e sulla documentazione. Chiedi all'agente di farla nel round F11, oppure prima, se vuoi.

**Cosa riportare:** Chiedi all'agente: "Completa V-07: Google Mobile Ads, UMP, Play Billing."

Nota: il binding OCR richiede Android 6.0 (API 23). Il tablet del Gate 0 deve avere questa versione.

### V-08 — Pacchetto Windows del pilota

**Scopo:** Decidere dove si pubblicano le release Windows del pilota, e come si firmano.

**Passi:**

1. Decidi la sede delle release (D12). Opzioni in `05` §11: codice aperto (GPL) subito; chiuso fino al Gate 2; chiuso sempre. `FASI.md` (V-08) propone anche un repository pubblico di sole release. Decisione a marzo 2027 (vedi sezione 7).
2. Scegli il certificato di firma. Le opzioni sono un certificato, Azure Trusted Signing o simili. Verifica i costi. [DA VERIFICARE: costi e tempi].
3. Metti il certificato nei segreti del repository, solo dopo la decisione.

**Risultato atteso:** Sede scelta; certificato scelto; costo noto.

### V-09 — Piano GitHub e minuti Actions

**Scopo:** Sapere quanto costano i job Android e Windows su un repository privato.

**Passi:**

1. Apri le impostazioni del piano GitHub dell'organizzazione. [DA VERIFICARE: percorso preciso nel sito GitHub].
2. Leggi i minuti Actions inclusi per un repository privato. Annota il moltiplicatore per i runner Windows.
3. Verifica se l'emulatore Android con accelerazione è disponibile sui runner ospitati.
4. Verifica se puoi proteggere il branch `main` con i job obbligatori.

**Risultato atteso:** Numeri scritti: minuti inclusi, moltiplicatore Windows, accelerazione Android, protezione del branch.

**Cosa riportare:** "V-09: minuti inclusi X; moltiplicatore Windows Y; accelerazione sì/no; protezione sì/no." Questo sblocca la decisione su F1 e F5.

### V-10 — Play Console: requisiti tecnici e account

**Scopo:** Preparare il primo caricamento e sapere quanto dura la verifica.

**Passi:**

1. Dopo la creazione dell'account, apri Play Console. Verifica il livello di API di destinazione richiesto.
2. Verifica se le pagine da 16 KB sono richieste per l'app.
3. Verifica se serve un primo caricamento manuale prima di usare l'API.
4. Annota il tempo di verifica dell'account.

**Risultato atteso:** Requisiti annotati. Tempo di verifica noto.

### V-11 — Tabella dei riferimenti

**Scopo:** Sapere se una tabella di soli riferimenti (non i testi) è consentita.

**Passi:** Porta al legale la domanda A11. Non usare la tabella nell'app prima della risposta.

**Risultato atteso:** Parere scritto. Decide la fase F9.

---

## 7. Conflitti nei documenti

Questi punti non coincidono. Decidi tu, poi riporta la decisione all'agente.

| Tema | Un documento dice | Un altro dice | Proposta |
|---|---|---|---|
| D11, parere legale | `05` §11 (tabella): entro 30/04/2027 | `05` §9.2: entro 30/06/2027 | Usa 30/06/2027 (ultima data utile) |
| D12, sede e codice | `05` §11: entro 30/06/2027 | `FASI.md` §10: serve a marzo 2027 per le release del pilota | Decidi entro marzo 2027. La prima data è la scadenza |
| D13, forma giuridica | `FASI.md` §0 e F15: prima del lancio (21/11/2027) | `05` §11: nessuna fino al Gate 2, entro 31/05/2028 | Segui `FASI.md`, che è la fonte aggiornata (DEC 10) |
| Costo Partner Center | V-04 (sintesi): registrazione gratuita per gli individui | `FASI.md` F13: "storicamente circa 19 $ una tantum" | Verifica sulla pagina di registrazione prima di pagare |
| Sede delle release | `FASI.md` V-08: repository pubblico di sole release | `05` §11 (D12): tre opzioni, senza repository pubblico di sole release | Aggiungi l'opzione a D12 se la vuoi |

---

## 8. Registro degli incontri

Compila questa tabella quando fai un incontro o ricevi una risposta. Non scrivere nomi o contatti nel repository. Usa i codici.

| Data | Con chi (codice) | Argomento | Domanda numero | Esito | Riportato all'agente? (sì/no) |
|---|---|---|---|---|---|
| | | | | | |

Esempio (da cancellare): `10/01/2027 | C-01 | Commercialista | 6 | Account individuale, risposta scritta attesa | no`

---

## 9. Riepilogo: cosa riportare all'agente

Usa questo modello per ogni verifica chiusa:

`Verifica V-0N: fatta il [data]. Esito: [testo breve]. Fase sbloccata o bloccata: [F#]. Decisione presa: [sì/no, quale].`

Esempio: `Verifica V-09: fatta il 25/11/2026. Esito: minuti inclusi 2.000; moltiplicatore Windows 2; accelerazione Android sì; protezione del branch sì. Fase sbloccata: F1 e F5. Decisione presa: no.`

[Il numero di minuti dell'esempio è inventato. Usalo solo come modello di frase.]
