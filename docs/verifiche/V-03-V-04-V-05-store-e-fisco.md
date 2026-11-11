# V-03, V-04, V-05: regole degli store per donazioni, Microsoft Store e aspetti fiscali

Data della verifica: **11/11/2026**. Tutte le pagine citate sono state lette in quella data, salvo dove indicato.

Legenda della confidenza:
- **Verificato**: fonte primaria (Google, Microsoft, testi di legge) letta oggi.
- **Probabile**: fonte secondaria (stampa specializzata, blog, forum) oppure fonte primaria con una lettura ambigua.
- **Non verificabile online**: serve la console, l'account o un professionista.

Questo documento non è una consulenza fiscale o legale. La parte V-05 descrive il quadro e termina con domande per il commercialista.

## Sintesi

| Voce | Esito | Confidenza | Impatto sulle fasi |
|---|---|---|---|
| V-03 Google Play: donazioni in app | Un pagamento in app verso lo sviluppatore che non è un ente esente da imposte passa per la fatturazione di Play. L'esenzione vale solo per le "tax exempt donations" e per le mance peer-to-peer al 100% verso un creatore, senza contenuti in cambio. | Verificato (testo), probabile (applicazione al nostro caso) | F12: le donazioni in app usano Play Billing (prodotto consumabile). F15: nessun collegamento alla pagina esterna. |
| V-03 Google Play: collegamento esterno | Vietato ("may not lead users to a payment method other than Google Play's billing system"). Il programma "external offers" nel SEE richiede di essere "registered as a business". | Verificato | F12/F15: il collegamento dall'app resta escluso finché lo sviluppatore non ha un'attività registrata. Anche in quel caso si pagano commissioni. |
| V-03 Microsoft Store: donazioni | Per le app non-gioco su PC le **donazioni volontarie senza nulla in cambio vanno raccolte con la Payment Request API o con un servizio di pagamento terzo**, non con gli add-on dello Store. Se la donazione dà qualcosa (es. rimozione pubblicità) serve l'acquisto in app dello Store. | Verificato (policy 10.8.2) | F12: su Windows i "consumabili mancia" dello Store sono incoerenti con la 10.8.2. Il collegamento esterno di donazione invece è ammesso, con dei requisiti. |
| V-03 Microsoft Store: commissioni | 15% sugli acquisti con il commercio Microsoft per le app; 0% con un commercio di terzi. | Verificato (ADA v8.11) | F12, F15 |
| V-04 Account Partner Center | Registrazione **gratuita** per individui e aziende, con verifica del documento e selfie. Però un account individuale vale solo se la distribuzione "is not in relation to their business, trade, or profession"; altrimenti serve un account Company con verifica aziendale (DUNS o documenti d'impresa). | Verificato | **Rischio alto per F13/F15**: un'app monetizzata da un privato senza attività registrata è in una zona grigia. Decisione da prendere con il commercialista. |
| V-04 Pacchetto MSIX MAUI | `runFullTrust` è una capability ristretta: si giustifica nella sottomissione in Partner Center. Store firma il pacchetto gratuitamente. Bundle multi-architettura consigliato, non obbligatorio. Windows 10 1809 minimo per MAUI 10. | Verificato | F13 |
| V-04 Certificazione | Informativa privacy obbligatoria (le app desktop/Win32 "must always have privacy policies"). Questionario IARC obbligatorio alla prima sottomissione. Dichiarazione degli acquisti in app nella descrizione. | Verificato | F13, F15 |
| V-04 Pilota su Store | "Private audience" fino a 10.000 indirizzi per gruppo, invisibile agli altri; dopo il passaggio a "Public" non si torna indietro. Gli acquisti in app (`Windows.Services.Store`) si provano solo con un'app pubblicata nello Store, anche in pubblico privato. | Verificato | F14: con Windows distribuito da GitHub gli acquisti Store **non si possono provare** nel pilota. |
| V-04 Automazione CI | `msstore` CLI (ancora "preview") e azione GitHub `microsoft/microsoft-store-apppublisher@v1.1`. Prima pubblicazione manuale; servono un tenant Entra ID collegato, un'app Entra con ruolo Manager e il segreto. L'API di sottomissione non gestisce i consumabili gestiti dallo Store. | Verificato (doc), non verificabile online (tenant con account individuale) | F13 |
| V-04 Add-on in app WinUI 3 | Durevoli e consumabili supportati per app desktop impacchettate, con inizializzazione della finestra (`InitializeWithWindow`). | Verificato | F12 |
| V-05 IVA sulle vendite in app | Google: "responsible for determining, charging, and remitting VAT" per gli acquisti UE con Play Billing. Microsoft: commissionario/agente; riscuote l'IVA nei paesi elencati nell'Allegato A (elenco su pagina esterna, non letto). | Verificato (Google), probabile (Microsoft per l'Italia) | F12, F15 |
| V-05 Trader DSA | Google: dichiarazione obbligatoria (subito per account nuovi, entro **01/02/2027** per gli esistenti); chi monetizza (pubblicità, acquisti) rientra nei criteri. Dal **01/03/2027** per un account personale trader sono pubblici **telefono e indirizzo**. Microsoft accetta casella postale o indirizzo di un agente registrato. | Verificato | **F14/F15**: dati personali pubblici. Da decidere prima di attivare pubblicità e acquisti. |
| V-05 Regime italiano | Forfettario: ricavi fino a 85.000 €; nel 2026 esclusi i dipendenti con più di 35.000 € di reddito da lavoro (L. 199/2025). Abitualità, INPS, VIES e trattamento delle donazioni: da chiarire con il commercialista. | Probabile (fonti secondarie) | F14 (pilota con ricavi), F15 |

## V-03 Regole sulle donazioni

### Google Play

**Pagamenti in app.** La policy Payments impone la fatturazione di Play per i beni digitali e le funzioni dell'app. Le donazioni compaiono solo come caso in cui la fatturazione di Play **non** si deve usare, e solo se esenti da imposte. [Verificato]
- Fonte: https://support.google.com/googleplay/android-developer/answer/9858738 (letta il 11/11/2026). Sezione 3.2: *"payments include peer-to-peer payments, online auctions, and tax exempt donations"*. Sezione 4: *"apps may not lead users to a payment method other than Google Play's billing system"*. La pagina non riporta una data di aggiornamento.

**Mance (tip).** La FAQ della policy tratta la mancia diretta da utente a creatore come pagamento peer-to-peer se il 100% va al creatore e se non dà accesso a contenuti. Se una delle due condizioni manca, serve la fatturazione di Play. [Verificato il testo; applicazione al nostro caso **probabile**]
- Fonte: https://support.google.com/googleplay/android-developer/answer/10281818 (11/11/2026): *"In cases where 100% of the tip or contribution from a user goes to the creator … and the payment does not grant access to any digital content or services, then we regard this as a peer-to-peer payment and use of Google Play's billing system is not required."*
- Lettura: la FAQ riguarda le app che ospitano creatori terzi. Una mancia allo sviluppatore stesso dell'app non è chiaramente "peer-to-peer". La via prudente è usare Play Billing.

**Donazioni non esenti.** Un avviso di Google del 2021 inoltrato da un progetto open source dice che, per un ente non esente, la funzione di donazione va rimossa o fatta con Play Billing. [Probabile: fonte secondaria, anno 2021]
- Fonte: https://groups.google.com/g/anki-android/c/eP4nJMMuzvc/m/_c319Z_wBwAJ (post del 10/09/2021): *"Google Play's billing system must not be used in cases where payments include tax exempt donations."* Lo stesso avviso chiede la prova dello stato di esenzione.

**Collegamento esterno e programmi SEE.**
- Il programma "external offers" nel SEE richiede *"Developer must be registered as a business."* Effective June 4, 2026: commissione continuativa del 20% (standard), 15% (nuovi programmi Apps & Games) o 10% (primo milione di dollari) sugli altri beni digitali. [Verificato] Fonte: https://support.google.com/googleplay/android-developer/answer/14372887 (11/11/2026).
- Il "billing choice program" e le nuove commissioni partono il 30/06/2026 per il SEE: *"Until the updated service fees take effect on June 30, 2026"*. [Verificato] Fonte: https://support.google.com/googleplay/android-developer/answer/17161464 (11/11/2026).
- Commissioni SEE con Play Billing dal 30/06/2026: sul primo milione di dollari annuo 10% più una billing fee del 5% (*"the billing fee is set at 5%"*). Le tabelle sono di lettura incerta. [Probabile] Fonte: https://support.google.com/googleplay/android-developer/answer/16954621 (11/11/2026).
- FAQ: è vietato *"directly linking to a webpage that could lead to an alternate payment method"*. Si possono indicare pagine amministrative che non portano a un pagamento. [Verificato] Fonte: answer/10281818.

**Prodotto "mancia".** Nessun testo Google descrive esplicitamente un prodotto mancia. Un prodotto a pagamento singolo consumabile si può acquistare più volte, ed è la forma naturale. [Probabile]

### Microsoft Store

**Policy 10.8 (Store Policies versione 7.20, ms.date 14/09/2026).** Fonte: https://learn.microsoft.com/en-us/windows/apps/publish/store-policies (11/11/2026). [Verificato]
- 10.8.1: *"Non-game products made available on PC devices may either use a secure third-party purchase API or the Microsoft Store in-product purchase API for in-app purchases of digital items or services"*. Le donazioni che danno qualcosa in cambio (*"including but not limited to additional features or removal of advertising"*) sono acquisti di beni digitali.
- 10.8.2: *"You must use the Microsoft payment request API or a secure third-party purchase API to receive voluntary donations from users. However, if the user receives digital goods or services in return … you must use the Microsoft Store in-product purchase API instead."*
- 10.8.2, per i pagamenti con servizi terzi: l'app deve *"identify the commerce transaction provider, authenticate the user, and obtain the user's confirmation"*, e *"You must note the use of a secure third-party purchase API in Partner Center"*.
- 10.8.4: l'app e la scheda devono indicare i tipi di acquisti in app e la fascia di prezzo.

**Commissioni.** App Developer Agreement v8.11 (pubblicato il 17/03/2026, in vigore dal 17/04/2026). Fonte: https://go.microsoft.com/fwlink/?linkid=528905 (PDF letto il 11/11/2026). [Verificato]
- 6(b): *"Fifteen percent (15%) of Net Receipts for any Apps (and any In-App Products in such Apps …)"*; 12% per i giochi.
- 5(e): *"Purchases made on a third-party commerce engine are not subject to the Store Fee"*.

### Conclusione V-03 (proposta)

- **Google Play**: le donazioni in app possono esistere come prodotti consumabili con Play Billing. Il collegamento alla pagina esterna **non** è ammesso; gli "external offers" non sono accessibili a un privato senza attività registrata.
- **Microsoft Store**: la 10.8.2 dice che una donazione pura passa per la Payment Request API o un servizio terzo, non per gli add-on dello Store. Un add-on "mancia" che non dà nulla sembra quindi **in contrasto** con la lettera della policy. Invece il collegamento alla pagina esterna è ammesso, se l'app identifica il fornitore e se lo si dichiara in Partner Center. Due opzioni da decidere:
  1. su Windows nessuna donazione in app, solo il collegamento esterno (dopo F15, quando la pagina esiste);
  2. un add-on Store che dà un piccolo bene digitale (es. un tema o un badge "sostenitore"). Allora è un acquisto e non una donazione, e su Play va descritto allo stesso modo.
- **Vincoli di testo proposti**: niente parola "donazione" o "beneficenza" per i prodotti venduti tramite store (lo sviluppatore non è un ente esente). Usare "Sostieni lo sviluppo", con la frase "Non sblocca funzioni. Non è una donazione deducibile." Su Play nessun riferimento al sito per pagare. Su Windows indicare il fornitore del pagamento esterno e, se richiesto, che Microsoft non è il destinatario.

## V-04 Requisiti del Microsoft Store per un'app MAUI

**Pacchetto e firma.** Fonte: https://learn.microsoft.com/en-us/windows/apps/publish/publish-your-app/msix/app-package-requirements (aggiornata il 24/08/2026, letta il 11/11/2026). [Verificato]
- *"The Microsoft Store will automatically re-sign your MSIX/AppX packages with a Microsoft certificate"*. Non serve un certificato a pagamento per lo Store. Per il pacchetto su GitHub (pilota) serve invece una firma propria.
- Bundle: *"App bundles should be generated whenever possible"*. È una raccomandazione; non è scritto che x64 e ARM64 siano entrambi obbligatori.
- Ultima cifra della versione riservata allo Store: *"must be left as 0"*.

**Versione minima.** .NET MAUI 10: *"Windows 11 and Windows 10 version 1809 or higher"*. Fonte: https://learn.microsoft.com/en-us/dotnet/maui/supported-platforms?view=net-maui-10.0 (11/11/2026). [Verificato]

**`runFullTrust`.** Fonte: https://learn.microsoft.com/en-us/windows/apps/package-and-deploy/app-capability-declarations (ms.date 02/10/2026). [Verificato]
- Le capability ristrette *"require approval for Microsoft Store submission"*. La richiesta si fa nella sottomissione: *"We now allow you to provide this info in Partner Center as part of the submission process"*. Se non approvata, la certificazione fallisce.
- Forum Microsoft Q&A riportano che per le app desktop la giustificazione viene accettata di norma. [Probabile] Esempio: https://learn.microsoft.com/en-us/answers/questions/1115168/why-does-a-net-maui-app-for-windows-10-require-the

**Certificazione.**
- Privacy (10.5.1): *"Product types that inherently have access to Personal Information must always have privacy policies. These include, but are not limited to, Desktop Bridge and Win32 products."* L'URL va inserito in Partner Center. [Verificato]
- Età (11.11): *"You must obtain an age rating for your product when you submit it in Partner Center"* con il questionario IARC. Il nome e l'email dell'editore sono condivisi con IARC. Fonti: Store Policies e https://learn.microsoft.com/en-us/windows/apps/publish/publish-your-app/msix/age-ratings (ms.date 21/08/2026). [Verificato]
- Acquisti: l'ADA 5(e) chiede di *"prominently disclose in your product description that in-app purchase functionality is available"*. [Verificato]

**Account sviluppatore.** Fonti: https://learn.microsoft.com/en-us/windows/apps/publish/partner-center/partner-center-developer-account (ms.date 29/10/2026) e https://learn.microsoft.com/en-us/windows/apps/publish/whats-new-individual-developer. [Verificato]
- *"there are no registration fees for either account type"*, solo partendo da https://storedeveloper.microsoft.com. *"The $19 registration fee is waived in the new flow"*, *"available in nearly 200 markets"*. Non è indicato l'elenco dei paesi; l'Italia non è esclusa esplicitamente. [Disponibilità in Italia: probabile]
- Verifica d'identità: *"government-issued ID and selfie"*. Un account individuale usa un account Microsoft personale.
- **Tipo di account**: l'individuale è per *"Independent developers whose distribution of apps through the Store is not in relation to their business, trade, or profession"* e per *"Small scale creators producing content for non-commercial purposes"*. L'ADA, sezione 2: *"If your intended distribution … is in relation to your business, trade, or profession, your Store Account must be a Company account type."* La policy 10.14 dice lo stesso. Il passaggio da Individual a Company **non è possibile**: serve un nuovo account. L'account Company richiede DUNS o documenti d'impresa, quindi un'attività registrata.

**Pilota con pubblico ristretto.** Fonte: https://learn.microsoft.com/en-us/windows/apps/publish/publish-your-app/msix/visibility-options (ms.date 28/08/2026). [Verificato]
- "Private audience": l'app *"will not be discoverable or available to anyone other than people in the group(s) you specify"*. I tester usano un account Microsoft **personale** (no account di lavoro).
- *"If you submit a product with this option set to Public audience, you can't choose Private audience in a later submission."*
- Le recensioni del pubblico privato non diventano pubbliche. La sottomissione passa comunque la certificazione.
- Gruppi di utenti noti: *"a maximum of 10,000"* indirizzi. Fonte: https://learn.microsoft.com/en-us/windows/apps/publish/create-customer-groups.
- "Available but not discoverable" con "Stop acquisition" permette di scaricare solo con codice promozionale. Microsoft sconsiglia questa opzione per i test ristretti.

**Acquisti in app e loro test.** Fonte: https://learn.microsoft.com/en-us/windows/uwp/monetize/in-app-purchases-and-trials (aggiornata il 27/11/2025). [Verificato]
- Tipi: durevole (adatto al "Pro"), consumabile gestito dallo sviluppatore, consumabile gestito dallo Store.
- App desktop (WinUI 3/MAUI): serve `WinRT.Interop.InitializeWithWindow.Initialize(context, hwnd)`; *"In-app purchase functionality is not currently supported in elevated applications."*
- Test: *"you must publish your app to the Store and download the app to your development device to use its license for testing"*. Non esiste un simulatore per `Windows.Services.Store`.

**Automazione CI.** [Verificato]
- `msstore` CLI, fonte https://learn.microsoft.com/en-us/windows/apps/publish/msstore-dev-cli/overview (ms.date 01/10/2026): *"You must complete your app's first publication to the Microsoft Store through Partner Center"*. Servono un tenant Entra ID collegato a Partner Center e l'accesso con credenziali Entra. *"App update operations … currently supported for free products only"*: un'app gratuita con add-on dovrebbe rientrare. [Probabile] `init` e `package` supportano .NET MAUI.
- Azione GitHub, fonte https://learn.microsoft.com/en-us/windows/apps/publish/msstore-dev-cli/commands: `microsoft/microsoft-store-apppublisher@v1.1`, poi `msstore reconfigure` con TenantId, SellerId, ClientId e ClientSecret (o certificato).
- Submission API, fonte https://learn.microsoft.com/en-us/windows/uwp/monetize/create-and-manage-submissions-using-windows-store-services: richiede Global administrator del tenant e un'app Entra con ruolo **Manager**. *"This API cannot be used with apps or add-ons that use mandatory app updates and Store-managed consumable add-ons"*. Conviene quindi usare consumabili gestiti dallo sviluppatore.
- Collegare un tenant Entra a un account **individuale**: le pagine non lo escludono né lo confermano. [Non verificabile online]

## V-05 Aspetti fiscali e di trader (Italia)

**IVA sulle vendite in app ai consumatori UE.**
- Google: *"Google is responsible for determining, charging, and remitting VAT for all Google Play Store purchases of digital content and services by EU customers using Google Play's billing system."* Fonte: https://support.google.com/googleplay/android-developer/answer/138000 (11/11/2026). [Verificato]
- Attenzione: Google annuncia la *"transition to an online marketplace model in the EEA and the UK effective March 1, 2027"* (answer/17594752). Le pagine lette non dicono se cambia chi versa l'IVA. [Non verificabile online]
- Microsoft: l'ADA nomina Microsoft *"agent or commissionaire"* (sezione 4) e prevede che riscuota e versi l'IVA *"only in the country(s) and to the extent described in the attached Exhibit A"*. L'Allegato A rimanda a un elenco esterno (http://go.microsoft.com/fwlink/p/?LinkId=529042) che non è stato letto. Una fonte terza dice che l'UE è coperta. [Probabile] Senza il commercio Microsoft: *"Microsoft will not be responsible for collecting or remitting any taxes"* (6(h)). [Verificato]

**AdMob.** La pagina sulle informazioni fiscali (https://support.google.com/admob/answer/14135099) chiede dati fiscali *"Depending on your location"* e non tratta la partita IVA. Fonti secondarie indicano Google Ireland come controparte, con fattura in inversione contabile (art. 7-ter DPR 633/72) per chi ha partita IVA. Esempio: https://pasqualepillitteri.it/news/5991/fattura-pagamenti-adsense-dati-iva-reverse-charge. [Probabile] Il caso di una persona senza partita IVA non è trattato da nessuna fonte ufficiale. [Non verificabile online]

**Partita IVA richiesta dalle piattaforme?** Google Play, per gli account personali: *"If you decide to monetize on Google Play then Google will display your full address"*. Il profilo pagamenti è obbligatorio; la partita IVA non è citata. Fonte: https://support.google.com/googleplay/android-developer/answer/13628312. [Verificato] Partner Center individuale: la partita IVA non è richiesta per registrarsi; serve il profilo fiscale per i pagamenti. [Probabile]

**DAC7.** Le fonti lette indicano come attività segnalate: affitto di immobili, servizi personali, vendita di beni e noleggio di mezzi di trasporto. Esempio: https://www.fiscoetasse.com/approfondimenti/16928-dac7-la-nuova-frontiera-della-trasparenza-fiscale-nelleconomia-digitale. La vendita di app e contenuti digitali non sembra compresa. Il testo della direttiva su EUR-Lex non si è caricato. [Probabile]

**Quadro italiano (descrittivo).**
- Forfettario: ricavi o compensi fino a 85.000 €. Per il 2026 la causa di esclusione per redditi da lavoro dipendente sale a 35.000 € (L. 199/2025, art. 1 c. 27, che modifica la L. 207/2024). Fonte: https://www.informazionefiscale.it/passaggio-regime-forfettario-ordinario-opzione (10/01/2026): *"La conferma della soglia dei 35.000 euro per dipendenti e pensionati"*. Le pagine dell'Agenzia delle Entrate non si sono caricate. [Probabile]
- La soglia del lavoro dipendente non conta se il rapporto è cessato; dal 2027 tornerebbe a 30.000 € salvo proroghe. [Probabile, stessa fonte]
- Abitualità contro occasionalità, iscrizione INPS (gestione commercianti o separata), VIES e trattamento delle donazioni ricevute da un privato: nessuna fonte ufficiale trovata che tratti app e mance. [Non verificabile online] Nota: una "mancia" pagata tramite store passa per un contratto di vendita di un prodotto in app, quindi è difficile considerarla una liberalità. Questo è un ragionamento, non una fonte.

**Trader (DSA).**
- Google, fonte https://support.google.com/googleplay/android-developer/answer/17594752 (11/11/2026). [Verificato]
  - Definizione: *"any natural or legal person acting for purposes relating to their trade, business, craft or profession"*. Tra i criteri: *"you receive or intend to receive remuneration for your apps … (for example, paid apps, in-app purchases, ad-sponsored apps)"*.
  - Account personale trader: *"a phone number and address will now be displayed"* e *"you must provide a contact phone number to be displayed on Google Play"*.
  - Scadenze: dichiarazione immediata per i nuovi account; *"Existing developer accounts must submit the declaration by February 1, 2027"*. Visualizzazione dal 01/03/2027.
  - Conseguenze: la mancata dichiarazione *"may trigger enforcement including app update blocks and removal from the Play Store globally"*.
  - Chi si dichiara non trader e distribuisce nel SEE: agli utenti può apparire l'avviso che *"consumer rights stemming from consumer protection laws do not apply"*.
  - Indirizzo alternativo (casella postale, domiciliazione): la pagina non ne parla. [Non verificabile online]
- Microsoft, fonte https://learn.microsoft.com/en-us/windows/apps/publish/store-business-verification-reqs (ms.date 27/02/2025). [Verificato] La verifica DSA è descritta per gli account **Company** (DUNS o documento d'impresa). Il telefono, l'indirizzo fisico e l'email sono richiesti. Microsoft accetta però una casella postale o un agente registrato: *"Some developers have created post office (P.O.) boxes for physical addresses … Microsoft also accepts addresses for registered agents."*

**Altro emerso (Google Play, influisce su F14).** Gli account personali creati dopo il 13/11/2023 devono fare un test chiuso *"with a minimum of 12 testers who have been opted in continuously for at least 14 days"* prima della produzione. Fonte: https://support.google.com/googleplay/android-developer/answer/14151465. [Verificato]

## Cosa resta da verificare a mano dal proprietario

1. **Partner Center**: aprire https://storedeveloper.microsoft.com, scegliere Individual e controllare che l'Italia sia tra i paesi e che non ci siano costi. Prima di confermare, rileggere la frase sul tipo di account e decidere con il commercialista (domanda 1) se serve un account Company.
2. **Partner Center, sottomissione di prova** (app riservata, mai pubblica): controllare il campo di giustificazione per `runFullTrust`. Controllare che l'opzione "Private audience" sia disponibile. Creare un add-on durevole e uno consumabile e vedere quali tipi offre la console.
3. **Partner Center, impostazioni account**: verificare se un account individuale può collegare un tenant Entra ID e creare un'app Entra con ruolo Manager. Senza questo, niente `msstore` in CI.
4. **Partner Center, profilo fiscale**: leggere l'elenco dei "Microsoft managed tax remittance countries" (link dell'Allegato A) e confermare che l'Italia e l'UE ci siano.
5. **Play Console**: aprire la dichiarazione trader. Annotare quali campi diventano pubblici e se si può indicare un indirizzo diverso da quello di residenza (casella postale o domiciliazione). Fare uno screenshot.
6. **Play Console, profilo pagamenti**: verificare se la partita IVA è facoltativa per un profilo individuale italiano e quale indirizzo viene mostrato con la monetizzazione.
7. **Play Console, prodotti in app**: creare un prodotto consumabile "Sostieni lo sviluppo" in bozza e leggere gli avvisi della console.
8. **AdMob, Pagamenti**: controllare quali dati fiscali chiede a una persona fisica italiana senza partita IVA e come sarà intestato il documento di pagamento.
9. **Supporto Google Play** (modulo policy): chiedere per iscritto se una mancia allo sviluppatore, senza contenuti in cambio, rientra nel caso "peer-to-peer" o richiede Play Billing. Conservare la risposta.

## Domande per il commercialista

1. **Pubblicità e acquisti in app ricorrenti da un dipendente: sono attività abituale (partita IVA) o redditi diversi occasionali?** Serve a scegliere il tipo di account Microsoft (Individual o Company), la dichiarazione trader su Google e la data di apertura dell'eventuale partita IVA. Blocca: F13 (account), F14 (pilota con ricavi), F15.
2. **Se serve la partita IVA: forfettario compatibile con il mio reddito da dipendente nel 2027, visto che la soglia di 35.000 € è prevista per il 2026?** Serve a stimare tasse e convenienza. Blocca: F15.
3. **Quale codice ATECO e quale gestione INPS (commercianti o separata), con quali contributi minimi?** Un minimale fisso può superare i ricavi attesi. Blocca: F15 (decisione di monetizzare).
4. **Pagamenti AdMob da Google Ireland senza partita IVA: come si dichiarano? Con partita IVA: serve l'iscrizione al VIES e la fattura in inversione contabile?** Blocca: F14 (primi incassi del pilota) e F15.
5. **Proventi da Google Play e Microsoft Store: con IVA versata dagli store verso i consumatori, io fatturo la mia cessione allo store? Come cambia per Google dal 01/03/2027 ("online marketplace model")?** Blocca: F12, F15.
6. **Mance comprate tramite store: sono ricavi come il "Pro" o possono essere liberalità?** Blocca: F12 (testi e categorie dei prodotti).
7. **Donazioni ricevute sul sito personale con un servizio di pagamento esterno: liberalità esenti, redditi diversi o ricavi d'impresa? Servono ricevute?** Blocca: F15 (pagina di donazione).
8. **Trader DSA senza attività registrata: posso usare un indirizzo diverso da quello di residenza (casella postale, domiciliazione)? Che rischi ci sono a dichiararmi "non trader" con un'app monetizzata?** Blocca: F14 (account Play nuovo con dichiarazione immediata) e F15.
9. **Il mio contratto di lavoro dipendente prevede esclusiva o autorizzazioni per attività esterne?** Domanda da verificare anche con l'ufficio del personale. Blocca: F14.
10. **Quali obblighi ci sono per gli incassi del pilota (settembre-ottobre 2027) se restano sotto qualche centinaio di euro?** Serve a decidere se attivare pubblicità e acquisti già nel pilota. Blocca: F14.

## Effetti proposti sul piano (proposte, non decisioni)

- **F12**: su Android, "Pro" come prodotto non consumabile e mance come consumabili con Play Billing, con testi che non parlano di donazione. Su Windows, "Pro" come add-on durevole. Per le mance su Windows due strade: nessun add-on (solo la pagina esterna da F15) oppure un add-on che dà un piccolo bene digitale. Usare consumabili **gestiti dallo sviluppatore**, compatibili con l'API di sottomissione.
- **F13**: prevedere la giustificazione di `runFullTrust` nella prima sottomissione manuale; `msstore` CI solo dopo la prima pubblicazione e dopo aver verificato il punto 3. Bundle x64 + ARM64 consigliato. Firma propria per il pacchetto GitHub.
- **F13/F15**: rendere bloccante la decisione sul tipo di account Microsoft. Individual e Company non si convertono l'uno nell'altro, quindi la scelta sbagliata costa un nuovo account e una nuova identità di pubblicazione.
- **F14**: se il pilota Windows resta su GitHub, gli acquisti Store non si possono provare. Proposta: pilota Windows con "Private audience" sullo Store (gratuito, fino a 10.000 tester) oppure acquisti Windows provati solo al lancio. Ricordare che dopo il passaggio a "Public" non si torna indietro. Su Android: 12 tester per 14 giorni consecutivi prima della produzione.
- **F14/F15**: dichiarazione trader su Google prima di attivare pubblicità e acquisti. Preparare un indirizzo e un telefono pubblicabili (casella postale, numero con segreteria) se le domande 1 e 8 lo consentono.
- **F15**: il collegamento dall'app alla pagina di donazione è possibile solo su Windows (dichiarazione in Partner Center e indicazione del fornitore). Su Android resta escluso; "external offers" solo con un'attività registrata e pagando commissioni.
- **Generale**: aggiungere al piano la domanda di parere fiscale come prerequisito di F14, non solo di F15, perché pubblicità e acquisti partono nel pilota.
