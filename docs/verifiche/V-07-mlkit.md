# V-07 (parte OCR): ML Kit per il riconoscimento del testo su Android

Data della verifica: **19/11/2026**. La parte di V-07 su Google Mobile Ads, UMP e Play Billing resta aperta.

## Esito in breve

- Il binding per .NET 10 esiste e si installa: `Xamarin.Google.MLKit.TextRecognition` **116.0.1.9** (autore Microsoft, repository dotnet/android-libraries).
- Il pacchetto corrisponde alla libreria Maven `com.google.mlkit:text-recognition:16.0.1`, cioè la variante con **modello Latin incluso nell'app** (DEC 32). Non scarica nulla da Play Services.
- Il pacchetto ha un gruppo di dipendenze per `net10.0-android36.0`. Il codice è nel progetto MAUI, solo per il target Android.
- La versione è fissata in `Directory.Packages.props`. Il riferimento sta nel progetto MAUI con condizione sul target Android.

## Effetto sull'app: Android minimo 23

Le librerie di ML Kit (tramite `androidx.tracing`) dichiarano `minSdkVersion` 23. L'app era a 21, e la build in CI è fallita con XAAMM0000. Il minimo è stato alzato a **Android 6.0 (API 23)**: si perdono i dispositivi con Android 5.x, ormai trascurabili.

## Dati del pacchetto

| Voce | Valore |
|---|---|
| Identificativo | `Xamarin.Google.MLKit.TextRecognition` |
| Versione scelta | 116.0.1.9 (le 116.0.1.6-9 esistono; la 116.0.1.7 era già nella cache locale) |
| Modello | Incluso nell'app (artefatto `text-recognition`), funziona offline |
| Licenza del binding | File `LICENSE.md` nel pacchetto (Microsoft, MIT); la libreria Google è soggetta ai termini di ML Kit e di Android |
| Dipendenze dirette (net10.0-android36.0) | `Xamarin.GooglePlayServices.Basement` e `.Base` 118.10.0.3, `Xamarin.GooglePlayServices.MLKit.Text.Recognition` 119.0.1.9, `Xamarin.Google.MLKit.Common` 118.11.0.9, `Xamarin.Google.MLKit.TextRecognition.Bundled.Common` 117.0.0.9, `Xamarin.Build.Download` 0.11.4 |
| Dipendenze indirette notevoli | `Xamarin.Google.MLKit.Vision.Common`, `Xamarin.GooglePlayServices.Tasks`, `Xamarin.GooglePlayServices.MLKit.Text.Recognition.Common` |

Punto da sapere: il binding .NET mette le classi pubbliche (`TextRecognition`, `TextRecognizerOptions`, `Text`) nel pacchetto `Xamarin.GooglePlayServices.MLKit.Text.Recognition`, che di per sé è la variante senza modello incluso. Il modello incluso arriva dal pacchetto `Bundled.Common` e dal descrittore `text-latin` del pacchetto principale. Che a runtime venga usata la variante inclusa si controlla con il valore `IsThickClient` di `TextRecognizerOptions.DefaultOptions`. Il test automatico scrive `bundled=True` o `bundled=False` nel log (riga `OCR INFO`). Se risulta `False`, la decisione DEC 32 non è rispettata e va riaperta.

## Misure di CI (esecuzione 44519461572, verde)

Sul portatile `dotnet restore` per Android fallisce con NETSDK1147 (workload `macos` mancante): è un problema locale. Le misure vengono dalla CI.

| Misura | Valore |
|---|---|
| Restore e avvisi NU1902-NU1904 | Nessun avviso di vulnerabilità; il restore è passato |
| Artefatto APK di debug prima (main) | 94,86 MB |
| Artefatto APK di debug dopo | 110,45 MB (circa +15,6 MB, APK universale di debug) |
| Build Android, Windows, core | Tutte verdi |
| `IsThickClient` / `bundled=` a runtime | Da leggere dal test su emulatore (riga `OCR INFO`) |

Due problemi di build emersi e risolti: il pacchetto aggiornava `androidx.fragment` e produceva classi duplicate con `fragment-ktx` (D8, `FragmentKt` definito due volte); in `Directory.Packages.props` il pacchetto `Xamarin.AndroidX.Fragment.Ktx` è fissato a 1.9.1. In più serviva `minSdk` 23 (vedi sopra). L'APK per lo Store (formato `aab`, una architettura per dispositivo) sarà più leggero di quello di debug universale.

## Come funziona nell'app

- `AndroidPdfPageRenderer` disegna la pagina con `PdfRenderer` a 200 dpi, con tetto di 4.096 pixel per lato e 12 milioni di pixel in tutto (circa 48 MB come ARGB_8888). Le regole stanno in `OcrRenderPlan` e sono provate dai test.
- `AndroidMlKitOcrEngine` implementa `IOcrEngine`. Riduce un'immagine troppo grande, riconosce, e riporta le coordinate alla dimensione dell'immagine ricevuta.
- Parole, righe e riquadri hanno la stessa forma della prova su Windows. L'adattatore `OcrWordAdapter` li porta nel flusso accordi-sopra-parole, con la gestione degli accordi in maiuscolo.
- La registrazione è nell'iniezione delle dipendenze, solo su Android. Nessuna schermata di importazione è collegata: **aperto**.

## Processo separato per i PDF (DEC 26)

**Non fatto in questo round, resta in processo.** `PdfRenderer` gira nel processo dell'app, dietro l'interfaccia `IOcrEngine`. Un servizio con `android:process=":pdf"` richiede un canale (Messenger o AIDL) e il passaggio del file per descrittore, e non si può provare senza emulatore. Va costruito in F7 insieme al servizio di lettura PDF. Fino ad allora un PDF di scansione costruito ad arte può chiudere l'app. I tetti sulla dimensione riducono il rischio di memoria esaurita, ma non lo eliminano.

## Test automatico su emulatore (solo build Debug)

All'avvio, se l'intent contiene la stringa `selftest=ocr`, l'app disegna due righe ("Do Re Mi" e "Alleluia canto nuovo") su un'immagine, le fa leggere a ML Kit e scrive nel log. Non esiste nelle build Release (`#if DEBUG`).

```
adb logcat -c
adb shell am start -n "$(adb shell cmd package resolve-activity --brief -a android.intent.action.MAIN -c android.intent.category.LAUNCHER com.companyname.appcanzoniere.maui | tail -1 | tr -d '')" --es selftest ocr
adb logcat -d -s AppCanzoniereSelfTest:I
```

L'attività è quella di avvio che il gestore dei pacchetti restituisce, quindi il nome non è fisso. Se l'attività è già aperta, l'intent arriva a `OnNewIntent` e il test parte lo stesso. L'app va installata prima con `adb install -r` sull'APK firmato; il test gira in un secondo, ma ML Kit può metterne alcuni alla prima chiamata, quindi conviene attendere la riga con un ciclo di ripetizione.

Righe attese nel log con tag `AppCanzoniereSelfTest`:

- `OCR INFO bundled=True` (modello incluso)
- `OCR OK Do Re Mi | Alleluia canto nuovo` (il testo può variare di poco: il test chiede solo "Alleluia" e "nuovo")
- oppure `OCR FAIL <motivo>`

## Punti aperti

1. Verificare su emulatore che il test scriva `bundled=True` (DEC 32) e `OCR OK`.
2. Servizio in processo `:pdf` per Android (F7).
3. Collegare motore e rendering a una schermata di importazione.
4. Qualità su scansioni reali di spartiti: da provare con il banco di prova (non incluso in questo round).
5. Il resto di V-07: Google Mobile Ads, UMP, Play Billing.

## Traffico di rete (R9-21)

Data: **21/11/2026**. Esecuzioni CI di misura: 44912944426 (prima della correzione) e 44914831304 (dopo la correzione). Emulatore API 34, immagine `google_apis`.

### Metodo

- L'opzione `-tcpdump` dell'emulatore non vede il traffico della macchina virtuale: una prova di controllo (`ping example.com` dalla shell) non compare nel file. Il metodo è quindi la cattura `tcpdump` sul computer che ospita l'emulatore, letta con `tools/android-network-analyze.py` (nomi DNS e nomi TLS).
- `tools/android-network-capture.sh` divide la prova in finestre con orari: controllo, 60 s senza app, test OCR, 60 s di attesa, lavori forzati (`cmd jobscheduler run -f`) per 90 s, 60 s finali.
- L'app ha un solo lavoro di Google programmato (servizio di trasporto dati, ripetizione ogni 24 ore). Il lavoro è stato forzato e si è concluso.
- Per separare l'app dal sistema: nella finestra senza app non compare nessun nome `firebaselogging`, e `dumpsys netstats` conta i byte per UID dell'app.

### Risultato prima della correzione (esecuzione 44912944426)

| Voce | Esito |
|---|---|
| Finestra senza app | Nessun traffico verso `firebaselogging.googleapis.com` |
| Dopo il test OCR (circa 22 s dopo la fine) | Richieste DNS e TLS verso `firebaselogging.googleapis.com` |
| Byte dell'app (UID 10192) | Da zero a 5.239 byte ricevuti e 2.230 inviati, solo in quel periodo |
| Lavoro forzato | Nessun traffico in più |
| Altri nomi nella cattura | Servizi di sistema Google dell'immagine (`android.googleapis.com`, `www.gstatic.com`, `mtalk.google.com` e simili) e GitHub: non attribuiti all'app |

Conclusione: ML Kit invia le metriche d'uso a Google anche con il modello incluso. Il contenuto non è stato letto (è cifrato). Il rilievo R9-21 era fondato.

### Correzione

Il manifest dell'app toglie la registrazione del servizio di invio della libreria di trasporto di Google (`tools:node="remove"`), e il progetto usa l'unificatore di manifest di Google. Senza servizio di invio gli eventi vengono scartati.

### Risultato dopo la correzione (esecuzione 44914831304)

| Voce | Esito |
|---|---|
| `firebaselogging.googleapis.com` | Assente in tutte le finestre |
| Byte dell'app (UID 10192) | Nessuna riga nelle statistiche: zero byte |
| Test OCR | `OCR INFO bundled=True` e `OCR OK Do Re Mi \| Alleluia canto nuovo` |
| `play.googleapis.com` | Resta (solo DNS), ma l'app ha zero byte: viene dal sistema dell'immagine |

Conclusione: dopo la correzione l'app non genera traffico di rete durante l'OCR. La dichiarazione "nessuna rete durante l'import" è vera per ML Kit.

### Controllo nel tempo

La cattura gira solo nelle esecuzioni manuali di CI (`workflow_dispatch`) e fallisce se compare `firebaselogging`. Limite: l'invio ritardato di 24 ore è provato solo con il lavoro forzato, non con un'attesa reale.
