# V-01 / V-02 — Verifica regole pubblicità (AdMob su Android, pubblicità su Windows)

Data della verifica: 2026-11-11. Tutte le pagine citate sono state lette in questa data, salvo diversa indicazione.

Legenda confidenza:
- **Verificato**: letto oggi su una fonte primaria (Google, Microsoft, NuGet, sito del fornitore).
- **Probabile**: da fonte secondaria (risultati di ricerca, Q&A, stampa) o da interpretazione di una fonte primaria che non tratta esplicitamente il caso.
- **Non verificabile online**: serve un account/console o una persona.

Questo documento non è una consulenza legale.

## Riepilogo

| Voce | Esito | Confidenza | Impatto sul piano F11/F15 |
|---|---|---|---|
| V-01 app open ad | Ammessa all'apertura o al ritorno in primo piano, prima che l'utente interagisca con i contenuti; con splash/loading screen; non insieme ad altri annunci. | Verificato | DEC 11 compatibile, con due ritocchi: mostrare l'annuncio solo all'avvio a freddo e non caricare il banner subito dopo l'annuncio di apertura. |
| V-01 banner | Vietati accanto ai controlli interattivi, in sovrapposizione ai contenuti e se si muovono durante lo scorrimento; vietati su schermate senza contenuto o di sola navigazione. | Verificato (regole); Probabile (applicazione alle nostre liste) | Banner ancorato fuori dall'area scorrevole e distanziato dai pulsanti; evitare i banner su schermate di sola navigazione o vuote. |
| V-01 condizioni per annunci reali | App pubblicata su Play e collegata in AdMob, poi revisione di idoneità; fino ad allora pubblicazione limitata. app-ads.txt richiede un sito sviluppatore nella scheda Play. Pagamento da €70. | Verificato | F15: serve un sito/dominio per app-ads.txt prima del lancio; durante lo sviluppo vanno usati solo annunci di prova. |
| V-01 contesto religioso / minori | La categoria "Religione" è consentita per default e va bloccata a mano. Se il pubblico include minori si applicano le norme Famiglie: le app open ad sono vietate nel programma Designed for Families. | Verificato | Decidere il pubblico di destinazione su Play (13+ oppure misto); bloccare in AdMob le categorie sensibili. |
| V-01 binding .NET 10 | Esistono binding ufficiali Microsoft per .NET 10: Xamarin.GooglePlayServices.Ads.Lite 124.0.0.9 e Xamarin.Google.UserMessagingPlatform 4.0.0.4, aggiornati il 2026-09-02. | Verificato | F11 fattibile senza plugin a pagamento; il wrapper MAUI va scritto da noi. |
| V-02 Microsoft | L'SDK Microsoft Advertising per UWP è stato chiuso il 2020-06-01. L'"in-app ads SDK" per WinUI 3 annunciato nel 2024 non risulta rilasciato. Microsoft Store Ads serve solo a promuovere un'app, non a mostrare annunci al suo interno. | Verificato (chiusura e redirect della pagina); Probabile (mancato rilascio) | Nessuna opzione Microsoft. |
| V-02 AdMob su Windows | Non supportato: nessun SDK Windows e Microsoft Store non tra gli store collegabili. | Verificato | Nessuna. |
| V-02 terze parti | L'unica opzione mantenuta è Vungle/Liftoff (SDK per WinUI 3 v7.2.0, 2026-10-29), che però serve solo annunci interstitial e rewarded, senza banner. Pubfinity è solo per UWP, AdDuplex ha chiuso nel 2023, Unity Ads e Start.io non supportano Windows. Gli annunci web dentro un'app desktop sono vietati da AdSense. | Verificato (NuGet, AdSense, Unity); Probabile (formati Vungle) | Confermare: niente pubblicità su Windows in v1. |

## V-01 — Regole AdMob

### 1. App open ad

| Regola | Fonte |
|---|---|
| Momento ammesso: «App open ad units should only appear when the user opens or switches back to an app.» | [App open ad guidance](https://support.google.com/admob/answer/9341964) |
| Prima dei contenuti: «The app open ad should appear before the user can interact with the app's content.» | idem |
| Splash/caricamento: «Integrate app open ads into your splash or loading screens.» Senza splash: «display ads immediately on app open or switch». | idem |
| Divieti: «Don't display ads immediately before or after app open ads.» e «Don't display app open ads on top of other ads (for example, content with banner ads).» | idem |
| Sanzione: il mancato rispetto «may result in Google disabling ad serving to your app». | idem |
| Programma Famiglie: le app nel programma Designed for Families non possono usare le app open ad. | idem |
| Frequenza: nessun limite numerico; «adjust ad frequency caps». Le app aperte più di una volta ogni 4 ore rendono di più. | idem |
| Avvio a freddo: «The preferred way to use app open ads on cold starts is to use a loading screen.» e «If your app has completed loading and has sent the user to the main content of your app, don't show the ad.» | [Guida sviluppatori Android – App open](https://developers.google.com/admob/android/app-open) |
| Prima esposizione: «Show your first app open ad after your users have used your app a few times.» (raccomandazione, non obbligo) | idem |
| Scadenza: «Ads rendered more than four hours after request time will no longer be valid and may not earn revenue.» | idem |

Annunci all'uscita: le pagine sulle app open ad non ne parlano. La policy sui contenuti vieta però gli annunci su schermate «dead end» come quelle di uscita, ringraziamento o errore ([Publisher Policies – screens without publisher-content](https://support.google.com/publisherpolicies/answer/11112688?hl=en)). Verificato.

Confronto con DEC 11 (interpretazione, confidenza **probabile**):
- Mostrare l'annuncio solo all'avvio e saltarlo se l'app riparte con una scaletta aperta è compatibile con la regola «before the user can interact with the app's content».
- Se l'annuncio comparisse anche al ritorno in primo piano su una lista con banner, violerebbe la regola che vieta di mostrarlo sopra contenuti con banner. Va quindi limitato all'avvio a freddo.
- «Don't display ads immediately before or after app open ads»: se la prima schermata dopo l'annuncio è una lista con banner, il banner compare subito dopo l'annuncio di apertura. Il caso è ambiguo; la scelta prudente è ritardare il caricamento del banner sulla prima schermata o non mostrarlo lì.

### 2. Banner

| Regola | Fonte |
|---|---|
| «banner ads should not be placed immediately next to interactive elements and app content» (sono citati pulsanti di navigazione e menu). | [Discouraged banner implementations](https://support.google.com/admob/answer/6275345) |
| «Close proximity of banner ads to other elements within an app is one of the biggest causes of accidental clicks.» | idem e [Banner ad guidance](https://support.google.com/admob/answer/6128877) |
| «Banner ads should not float or hover over app content.» e «banner ads should not move as a user scrolls.» | [Discouraged banner implementations](https://support.google.com/admob/answer/6275345) |
| «banner ads should not be placed next to interactive buttons, such as a "next" button or a custom app menu bar» | [Banner ad guidance](https://support.google.com/admob/answer/6128877) |
| Divieto di annunci su schermate «without publisher-content or with low-value content» e su schermate «used for alerts, navigation or other behavioral purposes». | [Publisher Policies – screens without publisher-content](https://support.google.com/publisherpolicies/answer/11112688?hl=en) |
| Divieto di schermate con più annunci che contenuti (dai risultati di ricerca, pagina della policy non aperta). | Probabile, [Google Publisher Policies](https://support.google.com/admob/answer/10502938?hl=en) |
| Sanzione per le schermate senza contenuto: limitazione della pubblicazione degli annunci (dai risultati di ricerca). | Probabile, [Policy issues and ad serving statuses](https://support.google.com/admob/answer/15697162?hl=en) |
| Numero di banner per schermata: non c'è un tetto numerico nelle pagine lette. | Verificato (assenza) |

Rischio per AppCanzoniere (**probabile**): le liste di canti create dall'utente potrebbero essere considerate contenuto oppure schermate di navigazione. La policy non distingue i contenuti generati dall'utente. Le schermate vuote (prima installazione, nessun canto) sono a rischio certo.

### 3. Condizioni per servire annunci reali

| Requisito | Fonte |
|---|---|
| L'app deve essere in uno store supportato e collegata: «Your app must be listed in a supported store.» Durante la revisione «apps may experience limited ad serving». | [About app readiness](https://support.google.com/admob/answer/10564477) |
| Un'app non pubblicata si può aggiungere ad AdMob «if you still need to test your app», ma riceve «limited ad serving until it's approved during AdMob's readiness process». | [Set up an app in AdMob](https://support.google.com/admob/answer/9989980) |
| app-ads.txt: «The app store listing must include a developer website»; il file va nella radice del sito («root directory»). La pagina non dice che sia obbligatorio per servire annunci. | [Set up app-ads.txt](https://support.google.com/admob/answer/9363762) |
| Annunci di prova: «If you click too many ads without being in test mode, you risk your account being flagged for invalid activity.» ID demo per le app open ad: `ca-app-pub-3940256099942544/9257395921`. | [Enable test ads](https://developers.google.com/admob/android/test-ads) |
| Soglie (riga EUR della tabella): verifica «$0 / $10 equivalent» e «€10»; pagamento «€70». | [Payment thresholds](https://support.google.com/admob/answer/2772208?hl=en) |
| PIN postale e tempi (2–3 settimane, 4 mesi per inserirlo; 3 errori sospendono gli annunci): dai risultati di ricerca, pagina non aperta. | Probabile, [Address verification (PIN)](https://support.google.com/admob/answer/2772302?hl=en) |
| Consenso nello SEE: serve un CMP certificato Google con IAB TCF dal 16 gennaio 2024. Senza CMP certificato si perdono gli annunci personalizzati e restano possibili solo annunci non personalizzati o limitati. La UMP di Google è un CMP certificato. | [Google-certified CMP requirement](https://support.google.com/admob/answer/13554116?hl=en) |

### 4. Contesto religioso e pubblico giovane

| Fatto | Fonte |
|---|---|
| «Standard sensitive categories are allowed by default.» "Religion" è una categoria standard: «Includes religious ads and ads advocating for or against religious views». Tra le categorie standard ci sono anche Dating, Politics, Reference to sex, Get rich quick, Weight loss e Social Casino Games. Gambling e Alcohol sono bloccate per default. | [List of sensitive categories](https://support.google.com/admob/answer/3150953?hl=en) |
| Il blocco si fa da Blocking controls (per app o per account); il filtro non è garantito al 100% e può ridurre i ricavi (dai risultati di ricerca). | Probabile, [Block ads by sensitive category](https://support.google.com/admob/answer/3150176?hl=en) |
| Norme Famiglie: se il pubblico include bambini si devono usare solo SDK pubblicitari autocertificati Famiglie, senza pubblicità basata sugli interessi. Con pubblico misto serve «a neutral age screen». | [Google Play Families policies](https://support.google.com/googleplay/android-developer/answer/9893335?hl=en) |
| `setTagForChildDirectedTreatment()` e TFUA sono deprecati a favore di `setAgeRestrictedTreatment()`. `maxAdContentRating` accetta G, PG, T e MA. | [Targeting – Android](https://developers.google.com/admob/android/targeting) |

Nota: l'esempio "Religion" non riguarda una religione in particolare. Bloccarlo evita annunci di altre confessioni o anti-religiosi, ma blocca anche quelli cattolici.

### 5. Binding .NET 10 MAUI (in sintesi)

| Pacchetto | Versione / data | TFM | Proprietari | Fonte |
|---|---|---|---|---|
| Xamarin.GooglePlayServices.Ads.Lite | 124.0.0.9 (play-services-ads-lite 24.0.0), 2026-09-02 | net10.0-android36.0, net9.0-android35.0 | Microsoft, dotnetframework, Xamarin | [nuget.org](https://www.nuget.org/packages/Xamarin.GooglePlayServices.Ads.Lite) |
| Xamarin.Google.UserMessagingPlatform | 4.0.0.4, 2026-09-02 | net10.0-android36.0, net9.0-android35.0 | Microsoft, dotnetframework, Xamarin | [nuget.org](https://www.nuget.org/packages/Xamarin.Google.UserMessagingPlatform) |
| Plugin.MauiMTAdmob (terza parte) | 3.0.0, 2026-10-16 | net10.0 | marcojak | [nuget.org](https://www.nuget.org/packages/Plugin.MauiMTAdmob) |

Plugin.MauiMTAdmob supporta la UMP solo nella versione con licenza a pagamento. Google pubblica ora un «GMA Next-Gen SDK for Android» accanto al «Mobile Ads SDK for Android (Legacy)» ([developers.google.com/admob](https://developers.google.com/admob)). Il binding Microsoft copre l'SDK classico «-lite»; da verificare se e quando seguirà quello Next-Gen.

## V-02 — Pubblicità su Windows

| Opzione | Stato | Modelli app | Confidenza / fonte |
|---|---|---|---|
| Microsoft Advertising SDK (Ad Monetization) | «As of June 1, 2020, the Microsoft Ad Monetization platform for Windows UWP apps will be shut down.» Pagina archiviata. | Solo UWP | Verificato, [Interstitial ads (archivio)](https://learn.microsoft.com/en-us/windows/uwp/monetize/interstitial-ads) |
| "In-app ads SDK" Microsoft per WinUI 3 | Nel 2024 un operatore Microsoft su Q&A scriveva: «In-app ads SDK is coming later this year for apps and games built on Windows App SDK + WinUI 3 (and also UWP)». Oggi l'URL `windows/apps/publish/in-app-ads` reindirizza a "Attract customers and promote your apps" (aggiornata il 2026-08-24), che non nomina alcun SDK. | WinUI 3 / UWP (annunciati) | Redirect verificato; mancato rilascio probabile. [Q&A 2024](https://learn.microsoft.com/en-us/answers/questions/1844451/how-to-put-ads-banner-in-windows-app-via-net-maui), [pagina attuale](https://learn.microsoft.com/en-us/windows/apps/publish/in-app-ads) |
| Microsoft Store Ads | Programma per acquistare campagne che promuovono la propria app desktop; non è un SDK per mostrare annunci nell'app. | n/d | Probabile (blog Microsoft 2023 visto solo nei risultati di ricerca) |
| Google AdMob | Piattaforme SDK elencate: Android (Next-Gen e Legacy), iOS, Unity, Flutter, C++, "Other platforms"; Windows assente. Store collegabili: Google Play, App Store, Amazon, OPPO, Samsung, VIVO, Xiaomi; Microsoft Store assente. | Nessun modello Windows | Verificato, [developers.google.com/admob](https://developers.google.com/admob), [Link to an app store](https://support.google.com/admob/answer/10037806?hl=en) |
| Vungle / Liftoff Monetize | NuGet `VungleSDKForWinUI3` 7.2.0, 2026-10-29: «monetization and ad mediation in Windows desktop applications, built on WinUI 3». TFM net8.0-windows10.0.19041; net10 solo calcolato. Secondo la documentazione Vungle (pagina 403, letta solo nei risultati di ricerca) la v7.x per Win32/WinUI 3/WinForms/WPF serve solo interstitial e rewarded: «Banner, MREC, and Native placements will not fill on Windows». | WinUI 3, Win32, WPF, WinForms | Pacchetto verificato; formati probabili. [nuget.org](https://www.nuget.org/packages/VungleSDKForWinUI3), [support.vungle.com](https://support.vungle.com/hc/en-us/articles/45026606409371) |
| Pubfinity | Il sito si presenta come «Advertising SDK For Windows UWP Games And Apps»; nessun cenno a WinUI 3 o Win32; copyright 2024. | Solo UWP | Verificato, [pubfinity.com](https://pubfinity.com/) |
| AdDuplex | Chiuso a luglio/agosto 2023. | n/d | Probabile (stampa: [devclass](https://devclass.com/2023/07/07/adduplex-shuts-down-with-short-notice-late-casualty-of-microsofts-windows-phone-and-windows-8-issues/)) |
| Unity Ads | «Unity Ads is an ad network for iOS (iTunes) and Android (Google Play).» | Nessun modello Windows | Verificato, [Unity Ads FAQ](https://docs.unity.com/en-us/grow/ads/references/faq) |
| Start.io | La documentazione SDK copre Android, iOS, Unity e Flutter; nessun SDK Windows trovato. | Nessun modello Windows | Probabile, [support.start.io](https://support.start.io/hc/en-us/categories/115000508293-SDK-documentation) |
| Annunci web in WebView2 | AdSense: «Publishers are not permitted to distribute Google ads … through software applications … including … desktop applications. AdSense code may only be implemented on web-based pages and approved WebView technologies.» Le WebView approvate sono solo Android WebView, iOS WKWebView, Custom Tabs, TWA e SFSafariViewController; WebView2 non c'è. | Vietato | Verificato, [AdSense ad placement policies](https://support.google.com/adsense/answer/1346295?hl=en), [Web content viewing frames](https://support.google.com/admanager/answer/6310245?hl=en) |

Policy Microsoft Store (versione 7.20, ms.date 2026-09-14), [Store Policies](https://learn.microsoft.com/en-us/windows/apps/publish/store-policies), verificato:
- 10.10.1: «The primary purpose of your product must not be to get users to click ads.» e «Your product must respect advertising ID settings that the user has selected.»
- 10.10.4: «advertising must be clearly distinguishable from other content in your product.»
- 10.10.5: l'informativa privacy deve dire che i dati personali vanno al fornitore di annunci e come rifiutare la pubblicità basata sugli interessi.
- 10.10.6: per prodotti destinati a minori di 13 anni bisogna dichiararlo in Partner Center.
- Rilevante per il "Pro" su Windows (10.8.1): le app non-gioco su PC possono usare per gli acquisti in-app un sistema di pagamento di terze parti sicuro oppure l'API Microsoft Store. La rimozione della pubblicità rientra esplicitamente tra i beni digitali.

**Conclusione V-02**: niente pubblicità su Windows in v1. L'unico SDK mantenuto e compatibile con WinUI 3 (Vungle) non offre banner, solo annunci a schermo intero, e richiede un account publisher Liftoff. Il suo TFM dichiarato è .NET 8, quindi la compatibilità con MAUI .NET 10 non è provata. Gli annunci web in WebView2 sono vietati da Google. L'SDK Microsoft per WinUI 3 annunciato non risulta disponibile.

## Cosa resta da verificare a mano dal proprietario

1. **AdMob – Blocking controls**: admob.google.com → App → (app) → Blocking controls → Manage sensitive categories. Controllare l'elenco attuale e decidere cosa bloccare (Religion, Dating, Politics, Reference to sex, Get rich quick, Social Casino Games). Poi Blocking controls → Ad content: impostare il rating massimo (consigliato G o PG).
2. **AdMob – Pagamenti**: Payments → Payments info. Inserire il profilo di pagamento, verificare l'identità quando richiesto e inserire il PIN postale. Controllare le soglie EUR effettive mostrate nel proprio account.
3. **AdMob – App readiness**: dopo la pubblicazione su Play, App → App settings → Link to store; controllare lo stato in App → Overview → "App readiness".
4. **app-ads.txt**: registrare un dominio o sito sviluppatore, inserirlo in Play Console → Presenza nello store → Dettagli dello store → Sito web, pubblicare `https://<dominio>/app-ads.txt` e controllare in AdMob → Apps → app-ads.txt.
5. **Play Console – Pubblico di destinazione**: Contenuti dell'app → Pubblico di destinazione e contenuti. Scegliere se includere i minori di 13 anni (cori giovanili). Includerli comporta norme Famiglie, SDK certificati, niente pubblicità basata sugli interessi e possibile divieto delle app open ad.
6. **AdMob – Privacy & messaging**: creare il messaggio GDPR (UMP) per lo SEE e testarlo con un dispositivo di prova.
7. **Interpretazione delle policy (persona)**: chiedere al supporto AdMob o al forum ufficiale se (a) una lista di canti creata dall'utente conta come «publisher-content» e (b) un banner sulla prima schermata subito dopo l'app open ad viola «Don't display ads immediately before or after app open ads».
8. **Vungle/Liftoff (solo se si vorrà rivalutare Windows)**: aprire un account publisher Liftoff, leggere la pagina di integrazione Windows (bloccata ai crawler) e provare `VungleSDKForWinUI3` in un progetto MAUI .NET 10 su Windows.

## Effetti proposti sul piano (proposte, non decisioni)

- **DEC 11 – app open**: precisare "solo avvio a freddo, mai al ritorno in primo piano", con splash o schermata di caricamento e annuncio mostrato solo se già pronto entro un tempo massimo.
- **DEC 11 – app open**: aggiungere "non al primo avvio": prima esposizione dopo alcune sessioni, come raccomanda Google.
- **DEC 11 – app open**: limite di frequenza (es. una volta ogni 4 ore). Scartare gli annunci caricati da più di 4 ore.
- **DEC 11 – banner**: niente banner sulla prima schermata mostrata subito dopo l'app open ad, oppure caricamento ritardato.
- **DEC 11 – banner**: niente banner su schermate vuote o di sola navigazione (stato vuoto, impostazioni, menu); banner ancorato fuori dall'area scorrevole, con margine dai pulsanti e dalla barra di navigazione.
- **DEC 11 – Windows**: confermare "nessuna pubblicità su Windows in v1". Valutare se su Windows il Pro abbia senso solo come sostegno o per funzioni extra, visto che non c'è pubblicità da rimuovere.
- **F11 – Android**: usare i binding Microsoft `Xamarin.GooglePlayServices.Ads.Lite` e `Xamarin.Google.UserMessagingPlatform` con un wrapper MAUI proprio, invece di un plugin a pagamento.
- **F11 – Android**: integrare la UMP prima della prima richiesta di annuncio; in sviluppo e CI usare solo ID di prova e dispositivi di test.
- **F11 – Android**: impostare `maxAdContentRating` e bloccare le categorie sensibili scelte.
- **F15 – lancio**: aggiungere sito sviluppatore e app-ads.txt, collegamento allo store, attesa della revisione di idoneità (pochi giorni di pubblicazione limitata) e configurazione dei pagamenti.
- **F15 – Play Console**: decidere il pubblico di destinazione prima di attivare gli annunci.
- **F15 – Store**: aggiornare l'informativa privacy secondo Store Policies 10.10.5 e GDPR, anche se su Windows non ci sono annunci (la parte Android lo richiede comunque).
