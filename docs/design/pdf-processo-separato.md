# Lettura dei PDF in un processo separato

## Perché
Alcuni PDF costruiti ad arte fanno chiudere PdfPig o lo fanno girare senza fine, prima dei nostri controlli.
Se la lettura avviene nel processo dell'app, il canto sbagliato ferma tutta l'app.
Decisione del titolare: leggere i PDF fuori dall'app. Un guasto uccide solo il processo figlio.

## Windows (fatto)
- Il programma `AppCanzoniere.PdfWorker` legge il PDF da stdin e scrive un unico JSON su stdout.
- Il lettore dell'app avvia il programma, gli manda i byte e aspetta con un limite di tempo.
- Limite di memoria: si campiona il working set del figlio e si imposta anche un tetto all'heap gestito.
- Tempo scaduto, annullamento, memoria o chiusura dell'app: si uccide tutto l'albero dei processi.
- Esito tipizzato: troppo grande, tempo scaduto, illeggibile (figlio chiuso male o risposta non fidata).
- La risposta è sempre ricontrollata con gli stessi limiti del lettore interno: pagine, caratteri, coordinate finite.
- Il formato ha un numero di versione; oggi è 1.

## Android (da fare in F7)
- Un eseguibile figlio non esiste su Android.
- Si usa un servizio dichiarato con `android:process=":pdf"`, e `android:isolatedProcess="true"` se la libreria lo permette.
- L'app si lega al servizio, gli passa il PDF con un descrittore di file e riceve lo stesso JSON.
- Stessi limiti e stessi esiti tipizzati; il processo del servizio si ferma con `Process.killProcess`.
- Il codice di lettura e di controllo è lo stesso di Windows, solo il trasporto cambia.

## Rischi aperti
- Android può uccidere il servizio mentre lavora: serve un nuovo tentativo e un messaggio chiaro.
- Sui tablet economici la classe di memoria è bassa: il tetto di memoria va tarato sul dispositivo.
- Un processo isolato non può usare gli stessi file dell'app: il PDF deve arrivare per descrittore.
- Il programma va impacchettato dentro l'MSIX e dentro la versione non impacchettata di Windows, accanto all'app.
- Il processo figlio parte in circa un secondo (avvio di .NET): sulla raccolta di prova costa da 1 a 2,5 secondi per PDF.
- Se l'app si chiude di colpo, il figlio si ferma da solo dopo il suo tempo massimo; un Job Object lo fermerebbe subito.
- Il tetto di memoria è campionato ogni 100 ms: un'allocazione molto rapida può superarlo prima del controllo.
- Il confine protegge da arresti anomali, blocchi e memoria (negazione del servizio), non da un lettore compromesso: il figlio riceve solo un ambiente minimo, senza i segreti dell'app.
- Su Windows è previsto un Job Object, che fermerebbe anche i processi discendenti rimasti.
