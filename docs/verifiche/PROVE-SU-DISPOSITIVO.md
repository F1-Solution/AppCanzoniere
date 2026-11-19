# Prove su dispositivo — round 8

Questa guida elenca le prove su tablet Android e su PC Windows che il proprietario deve ancora eseguire. Esegui le prove nell'ordine scritto. Ogni prova ha un ID. Alla fine, annota gli esiti nella tabella della sezione "Come riportare gli esiti".

Come leggere la guida:
- Ogni passo è un'istruzione. Eseguila prima di leggere il passo successivo.
- Le righe "ATTENZIONE:" vengono prima del passo a cui si riferiscono.

## Cosa è già provato in automatico

Non ripetere queste prove. Girano in CI o sul PC di sviluppo.

| Area | Prova | Dove |
|---|---|---|
| OCR | Il riconoscimento del testo con ML Kit, con il modello incluso nell'app, funziona | Emulatore Android (job android-emulator) |
| Canto | La schermata del canto si apre | Emulatore Android |
| Tasti | Pagina giù, Spazio, Freccia giù e Freccia destra girano avanti di una schermata meno una riga | Emulatore Android |
| Tasti | Pagina su, Freccia su e Freccia sinistra girano indietro di una schermata meno una riga | Emulatore Android |
| Tasti | Il primo tasto dopo un tocco sullo schermo gira pagina | Emulatore Android |
| Volume | I tasti del volume girano pagina nella schermata del canto e regolano il volume altrove | Emulatore Android |
| Tasti | Una pressione lunga gira una sola pagina | Emulatore Android |
| Schermo | Lo schermo resta acceso solo nella schermata del canto | Emulatore Android |
| Lettore PDF | Il lettore PDF è nella cartella dell'app, anche nella versione Release, e legge un PDF di prova | Runner Windows (job windows) e PC di sviluppo |
| Tasti | Avanti e indietro funzionano, anche subito dopo l'apertura senza clic | Runner Windows (job windows) e PC di sviluppo |
| Tasti | Un tasto tenuto premuto gira una sola pagina | Runner Windows (job windows) e PC di sviluppo |
| Campi | Spazio e Freccia destra in un campo di testo non girano pagina | Runner Windows (job windows) e PC di sviluppo |
| Schermo | La richiesta di schermo acceso è attiva nella schermata del canto e si rilascia quando l'app è ridotta a icona o si chiude il canto | Runner Windows (job windows) e PC di sviluppo |

## Preparazione

Esegui questi passi una sola volta, prima della prima prova.

### Scaricare i file

1. Apri la pull request del round 8: https://github.com/F1-Solution/AppCanzoniere/pulls
2. Apri la scheda "Checks".
3. Apri l'ultima esecuzione di "CI".
4. Nella sezione "Artifacts", scarica `android-debug-apk` e `windows-debug-build`.

### Tablet

5. Verifica che il tablet usi Android 6.0 o una versione successiva.
6. Copia il file APK sul tablet.
7. Apri il file APK sul tablet.
8. Se Android lo chiede, consenti l'installazione da origini sconosciute.

ATTENZIONE: se una versione precedente è installata e l'installazione fallisce, disinstalla prima quella versione.

9. Installa l'app.

### PC

10. Crea una cartella nuova sul PC.
11. Estrai `windows-debug-build` nella cartella nuova.
12. Apri la cartella estratta.
13. Avvia `AppCanzoniere.MAUI.exe`.
14. Se Windows SmartScreen mostra un avviso, scegli "Ulteriori informazioni".
15. Scegli "Esegui comunque".
16. Se l'app non si avvia, annota il messaggio di errore.

### Pedale

17. Carica il pedale.
18. Imposta il pedale sulla modalità tastiera. Consulta il manuale del pedale.
19. Il pedale deve inviare Pagina giù e Pagina su, oppure le frecce.
20. Accoppia il pedale al tablet tramite Bluetooth.
21. Accoppia il pedale al PC tramite Bluetooth.

### Canto di prova e materiale

22. Nell'elenco della Home compare il "Canto di prova". Compare solo nelle versioni di prova.
23. Il Canto di prova è lungo: ha più schermate.
24. Prepara un telefono per i video e un timer.

## Prove sul tablet

### T1 — Pedale avanti e indietro

**Scopo:** Controllare che il pedale giri pagina avanti e indietro sul tablet.

**Preparazione:** Tablet acceso, app aperta, pedale accoppiato al tablet.

**Passi:**
1. Apri il Canto di prova.
2. Annota l'ultima riga visibile in fondo allo schermo.
3. Premi il tasto avanti del pedale.
4. Controlla che la riga annotata sia la prima riga in alto.
5. Premi il tasto indietro del pedale.
6. Controlla che lo schermo torni alla posizione del passo 1.

**Risultato atteso:** Il tasto avanti gira una schermata meno una riga. L'ultima riga letta resta in alto. Il tasto indietro torna alla posizione di partenza.

**Cosa annotare:** OK o KO. Per KO, scrivi il numero del passo che non è riuscito.

### T2 — Primo tasto dopo un tocco

**Scopo:** Controllare che il primo tasto del pedale giri pagina dopo un tocco sul testo.

**Preparazione:** Canto di prova aperto, pedale accoppiato al tablet.

**Passi:**
1. Tocca il testo del canto una volta.
2. Premi il tasto avanti del pedale una volta.
3. Controlla se la pagina gira.
4. Ripeti i passi 1, 2 e 3 altre due volte, per un totale di tre prove.

**Risultato atteso:** La pagina gira al primo tasto in tutte e tre le prove.

**Cosa annotare:** Numero di prove riuscite, su tre. Per ogni prova non riuscita, scrivi cosa è successo.

### T3 — Tasto tenuto premuto

**Scopo:** Controllare che un tasto tenuto premuto giri una sola pagina.

**Preparazione:** Canto di prova aperto sulla prima pagina.

**Passi:**
1. Annota la prima riga in alto.
2. Tieni premuto il tasto avanti del pedale per 3 secondi.
3. Rilascia il tasto.
4. Conta le pagine girate.

**Risultato atteso:** Gira una sola pagina.

**Cosa annotare:** Numero di pagine girate.

### T4 — Tasti del volume

**Scopo:** Controllare che i tasti del volume girino pagina solo nella schermata del canto.

**Preparazione:** Imposta il volume a metà e annota il livello.

**Passi:**
1. Apri il Canto di prova.
2. Annota la prima riga in alto.
3. Premi il tasto Volume giù del tablet.
4. Controlla che la pagina avanzi e che il volume non cambi.
5. Premi il tasto Volume su del tablet.
6. Controlla che la pagina torni indietro e che il volume non cambi.
7. Torna alla Home dell'app.
8. Premi il tasto Volume su del tablet.
9. Controlla che il volume cambi e che nessuna lista scorra.

**Risultato atteso:** Nel Canto di prova i tasti del volume girano pagina e il volume non cambia. In Home i tasti regolano il volume e nulla scorre.

**Cosa annotare:** OK o KO per ogni gruppo di passi (passi 3 a 6, passi 8 e 9).

### T5 — Schermo acceso

**Scopo:** Controllare che lo schermo resti acceso solo nella schermata del canto.

**Preparazione:** Timer pronto. Nelle impostazioni Android, cerca la voce "Spegnimento schermo".

ATTENZIONE: questa prova cambia l'impostazione "Spegnimento schermo". Annota il valore attuale prima di cambiarlo.

1. Annota il valore attuale di "Spegnimento schermo".
2. Imposta "Spegnimento schermo" su 30 secondi.
3. Apri il Canto di prova.
4. Non toccare lo schermo per 2 minuti.
5. Controlla che lo schermo resti acceso.
6. Torna alla Home dell'app.
7. Non toccare lo schermo.
8. Controlla che lo schermo si spenga dopo circa 30 secondi.

ATTENZIONE: ripristina il valore annotato al passo 1, anche se la prova non riesce.

9. Ripristina "Spegnimento schermo" al valore annotato al passo 1.

**Risultato atteso:** Nel Canto di prova lo schermo resta acceso per 2 minuti. In Home lo schermo si spegne dopo circa 30 secondi.

**Cosa annotare:** OK o KO. Scrivi il valore ripristinato. Scrivi dopo quanti secondi lo schermo si è spento in Home.

### T6 — Video dello scorrimento

**Scopo:** Controllare che lo scorrimento sia fluido sul tablet.

**Preparazione:** Telefono per il video, appoggiato in modo che inquadri lo schermo del tablet.

**Passi:**
1. Apri il Canto di prova sul tablet.
2. Avvia la registrazione video sul telefono.
3. Premi il tasto avanti del pedale.
4. Attendi 2 secondi.
5. Ripeti i passi 3 e 4 per un totale di dieci pressioni.
6. Ferma la registrazione.
7. Guarda il video a velocità normale.
8. Controlla che non ci siano salti, fotogrammi vuoti o scatti visibili.
9. Conserva il video.

**Risultato atteso:** Il video non mostra salti, fotogrammi vuoti o scatti.

**Cosa annotare:** OK o KO. Scrivi il nome del file video. Scrivi quante volte hai visto uno scatto, e a quale pagina.

### T7 — Prova in chiesa poco illuminata (facoltativa, richiesta dal Gate 0)

**Scopo:** Controllare se il testo e gli accordi del canto si leggono con poca luce.

**Preparazione:** Tablet su un leggio o un supporto, a distanza di lettura.

**Passi:**
1. Appoggia il tablet sul leggio, a distanza di lettura.
2. Spegni le luci principali della stanza.
3. Apri il Canto di prova.
4. Leggi il testo del canto.
5. Controlla se leggi gli accordi.
6. Annota la luminosità dello schermo.

**Risultato atteso:** Il testo e gli accordi si leggono.

**Cosa annotare:** Sì o no per il testo. Sì o no per gli accordi. Il livello di luminosità dello schermo. Il tipo di luce presente nella stanza.

## Prove sul PC

### W1 — Pedale e tastiera

**Scopo:** Controllare che il pedale e la tastiera girino pagina avanti e indietro sul PC.

**Preparazione:** App aperta sul PC, pedale accoppiato al PC.

**Passi:**
1. Apri il Canto di prova con un clic.
2. Premi il tasto avanti del pedale. Controlla che la pagina avanzi.
3. Premi il tasto indietro del pedale. Controlla che la pagina torni indietro.
4. Premi il tasto Pagina giù. Controlla che la pagina avanzi.
5. Premi il tasto Spazio. Controlla che la pagina avanzi.
6. Premi il tasto Freccia giù. Controlla che la pagina avanzi.
7. Premi il tasto Freccia destra. Controlla che la pagina avanzi.
8. Premi il tasto Pagina su. Controlla che la pagina torni indietro.
9. Premi il tasto Freccia su. Controlla che la pagina torni indietro.
10. Premi il tasto Freccia sinistra. Controlla che la pagina torni indietro.

**Risultato atteso:** Ogni tasto gira pagina nella direzione indicata.

**Cosa annotare:** OK o KO per ogni tasto. Per KO, scrivi il numero del passo.

### W2 — Tasto subito dopo l'apertura

**Scopo:** Controllare che il pedale giri pagina senza un clic sul canto.

**Preparazione:** Nessuna. Il PC deve avere l'app chiusa.

**Passi:**
1. Avvia l'app dalla cartella estratta.
2. Apri il Canto di prova con un clic.
3. Non fare clic sul testo del canto.
4. Premi il tasto avanti del pedale. Controlla che la pagina giri.
5. Fai clic su una voce del menu a sinistra.
6. Torna al Canto di prova con un clic sulla sua voce.
7. Non fare clic sul testo del canto.
8. Premi il tasto avanti del pedale. Controlla che la pagina giri.

**Risultato atteso:** La pagina gira al passo 4 e al passo 8.

**Cosa annotare:** OK o KO per il passo 4 e per il passo 8. Per KO al passo 8, scrivi quale voce del menu hai scelto al passo 5.

### W3 — Tasto tenuto premuto

**Scopo:** Controllare che un tasto tenuto premuto giri una sola pagina sul PC.

**Preparazione:** Canto di prova aperto sulla prima pagina.

**Passi:**
1. Annota la prima riga in alto.
2. Tieni premuto il tasto avanti del pedale per 3 secondi.
3. Rilascia il tasto.
4. Conta le pagine girate.

**Risultato atteso:** Gira una sola pagina.

**Cosa annotare:** Numero di pagine girate.

### W4 — Campo di testo

**Scopo:** Controllare che Spazio e Freccia destra in un campo di testo non girino pagina.

**Preparazione:** Home aperta.

**Passi:**
1. Fai clic nel campo di ricerca.
2. Premi il tasto Spazio.
3. Controlla che il campo riceva lo spazio.
4. Premi il tasto Freccia destra.
5. Controlla che il campo riceva la freccia e che nessuna pagina giri.

**Risultato atteso:** Il campo riceve Spazio e Freccia destra. Nessuna pagina gira.

**Cosa annotare:** OK o KO. Per KO, scrivi quale tasto ha girato pagina.

### W5 — Schermo acceso

**Scopo:** Controllare che lo schermo resti acceso solo nella schermata del canto sul PC.

**Preparazione:** Timer pronto. Nelle impostazioni di Windows, cerca "Spegni lo schermo dopo".

ATTENZIONE: questa prova cambia l'impostazione "Spegni lo schermo dopo". Annota il valore attuale prima di cambiarlo.

1. Annota il valore attuale di "Spegni lo schermo dopo" in Impostazioni > Sistema > Alimentazione.
2. Imposta "Spegni lo schermo dopo" su 1 minuto.
3. Apri il Canto di prova con un clic.
4. Non toccare il PC per 3 minuti.
5. Controlla che lo schermo resti acceso.
6. Riduci a icona l'app.
7. Attendi.
8. Controlla che lo schermo si spenga dopo circa 1 minuto.

ATTENZIONE: ripristina il valore annotato al passo 1, anche se la prova non riesce.

9. Ripristina "Spegni lo schermo dopo" al valore annotato al passo 1.

**Risultato atteso:** Nel Canto di prova lo schermo resta acceso per 3 minuti. Ridotta a icona, l'app lascia spegnere lo schermo dopo circa 1 minuto.

**Cosa annotare:** OK o KO. Scrivi l'ora in cui hai ridotto l'app a icona. Scrivi l'ora in cui lo schermo si è spento. Scrivi il valore ripristinato.

### W6 — Video dello scorrimento

**Scopo:** Controllare che lo scorrimento sia fluido sul PC.

**Preparazione:** Telefono per il video, appoggiato in modo che inquadri lo schermo del PC.

**Passi:**
1. Apri il Canto di prova sul PC.
2. Avvia la registrazione video sul telefono.
3. Premi il tasto avanti del pedale.
4. Attendi 2 secondi.
5. Ripeti i passi 3 e 4 per un totale di dieci pressioni.
6. Ferma la registrazione.
7. Guarda il video a velocità normale.
8. Controlla che non ci siano salti, fotogrammi vuoti o scatti visibili.
9. Conserva il video.

**Risultato atteso:** Il video non mostra salti, fotogrammi vuoti o scatti.

**Cosa annotare:** OK o KO. Scrivi il nome del file video. Scrivi quante volte hai visto uno scatto, e a quale pagina.

### W7 — Avvio dalla cartella scaricata

**Scopo:** Controllare che l'app parta dalla cartella estratta e che il lettore PDF sia nella cartella.

**Preparazione:** La cartella estratta da `windows-debug-build` deve essere pronta. La schermata di import dei PDF non è ancora nell'app, quindi questa è l'unica prova del lettore PDF.

**Passi:**
1. Apri la cartella estratta.
2. Controlla che esista la sottocartella `PdfWorker`.
3. Avvia `AppCanzoniere.MAUI.exe` dalla cartella estratta.
4. Controlla che l'app si apra.

**Risultato atteso:** La sottocartella `PdfWorker` esiste. L'app si apre.

**Cosa annotare:** OK o KO. Se la sottocartella ha un nome diverso, scrivi il nome.

## Prova dell'import (traccia B)

### B1 — Tempo di correzione dei libretti (Gate 0)

**Scopo:** Misurare quanto tempo serve per correggere un canto importato da un libretto.

**Preparazione:** PC di sviluppo, con il report del banco di prova. Timer pronto. Due tabelle vuote: una per i PDF con testo, una per le scansioni. La schermata di import non è ancora nell'app, quindi usa il report.

**Passi:**
1. Apri `corpus-locale/report-pdf/index.md`. Questo report contiene i PDF con testo.
2. Apri `corpus-locale/report-ocr/index.md`. Questo report contiene le scansioni.
3. Per ciascuno dei 10 libretti, scegli 5 canti.
4. Confronta il risultato di un canto con il PDF originale.
5. Avvia il timer.
6. Correggi il canto fino a renderlo uguale al PDF originale.
7. Ferma il timer. Scrivi i minuti.
8. Scrivi "sì" se la correzione richiede 2 minuti o meno. Scrivi "no" se richiede più di 2 minuti.
9. Ripeti i passi 4 a 8 per ogni canto scelto.
10. Scrivi i PDF con testo e le scansioni nelle due tabelle separate.

**Risultato atteso:** Per ogni canto, un tempo in minuti e un sì o un no.

**Soglie per la decisione:**
- Almeno 70% di canti con "sì": OCR nella prima versione.
- Dal 50% al 70% di canti con "sì": solo PDF con testo e "incolla".
- Meno del 50% di canti con "sì": import fuori dalla prima versione.

La percentuale è il numero di canti con "sì" diviso per il numero di canti provati.

**Cosa annotare:** Per ogni canto: libretto, numero del canto, minuti, sì o no. Scrivi le due percentuali separate: PDF con testo e scansioni.

## Prove non ancora possibili

- OCR su scansioni reali sul tablet: la schermata di import non è ancora nell'app.
- Import dei PDF nell'app su Windows: la schermata di import non è ancora nell'app.
- Temi Leggio e Palco: arrivano con la fase F4.

## Come riportare gli esiti

Compila questa tabella. Per KO, scrivi il numero del passo e cosa è successo.

| ID | Esito (OK/KO) | Note |
|---|---|---|
| T1 | | |
| T2 | | |
| T3 | | |
| T4 | | |
| T5 | | |
| T6 | | |
| T7 | | |
| W1 | | |
| W2 | | |
| W3 | | |
| W4 | | |
| W5 | | |
| W6 | | |
| W7 | | |
| B1 | | |

Copia questa riga nella richiesta successiva, e sostituisci i valori tra parentesi:

`Esito prove: T1 ok, T2 ko (nota: passo 3, la pagina non gira), T3 ok, T4 ok, T5 ok, T6 ok, T7 ok, W1 ok, W2 ok, W3 ok, W4 ok, W5 ok, W6 ok, W7 ok, B1 (PDF con testo: 62%; scansioni: 41%).`
