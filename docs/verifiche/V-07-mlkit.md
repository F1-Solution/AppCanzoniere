# V-07 (parte OCR): ML Kit per il riconoscimento del testo su Android

Data della verifica: **19/11/2026**. La parte di V-07 su Google Mobile Ads, UMP e Play Billing resta aperta.

## Esito in breve

- Il binding per .NET 10 esiste e si installa: `Xamarin.Google.MLKit.TextRecognition` **116.0.1.9** (autore Microsoft, repository dotnet/android-libraries).
- Il pacchetto corrisponde alla libreria Maven `com.google.mlkit:text-recognition:16.0.1`, cioè la variante con **modello Latin incluso nell'app** (DEC 32). Non scarica nulla da Play Services.
- Il pacchetto ha un gruppo di dipendenze per `net10.0-android36.0`. Il codice è nel progetto MAUI, solo per il target Android.
- La versione è fissata in `Directory.Packages.props`. Il riferimento sta nel progetto MAUI con condizione sul target Android.

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

## Controlli che non si sono potuti fare in locale

Sul portatile `dotnet restore` del progetto MAUI per Android fallisce con NETSDK1147 (workload `macos` mancante): è un problema locale del carico di lavoro, non del codice. Per questo i punti seguenti si leggono dall'esecuzione di CI del ramo:

- Esito di `dotnet restore` e assenza di NU1902-NU1904 (avvisi di vulnerabilità, errori in questo repository).
- Dimensione dell'APK di debug con e senza il pacchetto.
- Comportamento di `IsThickClient`.

I valori misurati sono riportati nella tabella seguente.

| Misura | Valore |
|---|---|
| Restore e avvisi NU19xx | da compilare dopo la CI |
| APK di debug prima | da compilare dopo la CI |
| APK di debug dopo | da compilare dopo la CI |

Il modello Latin incluso pesa circa 4 MB per architettura secondo la documentazione di ML Kit; il binding aggiunge alcuni MB di librerie. L'APK finale per lo Store (con `AndroidPackageFormat=aab`, un'architettura per dispositivo) sarà più leggero di quello di debug universale.

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
adb shell am start -n com.companyname.appcanzoniere.maui/com.companyname.appcanzoniere.maui.MainActivity --es selftest ocr
adb logcat -d -s AppCanzoniereSelfTest:I
```

Il nome dell'attività è fisso (`com.companyname.appcanzoniere.maui.MainActivity`). Se l'attività è già aperta, l'intent arriva a `OnNewIntent` e il test parte lo stesso. L'app va installata prima con `adb install -r` sull'APK firmato; il test gira in un secondo, ma ML Kit può metterne alcuni alla prima chiamata, quindi conviene attendere la riga con un ciclo di ripetizione.

Righe attese nel log con tag `AppCanzoniereSelfTest`:

- `OCR INFO bundled=True` (modello incluso)
- `OCR OK Do Re Mi | Alleluia canto nuovo` (il testo può variare di poco: il test chiede solo "Alleluia" e "nuovo")
- oppure `OCR FAIL <motivo>`

## Punti aperti

1. Misure di CI (restore, avvisi, dimensione APK, `bundled=True`) da leggere dopo l'esecuzione.
2. Servizio in processo `:pdf` per Android (F7).
3. Collegare motore e rendering a una schermata di importazione.
4. Qualità su scansioni reali di spartiti: da provare con il banco di prova (non incluso in questo round).
5. Il resto di V-07: Google Mobile Ads, UMP, Play Billing.
