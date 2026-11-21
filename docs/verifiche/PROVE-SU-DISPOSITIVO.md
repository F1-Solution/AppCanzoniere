# Prove su dispositivo — round 9

Questa guida elenca le prove su tablet Android e su PC Windows che il proprietario deve ancora eseguire. Esegui le prove nell'ordine scritto. Ogni prova ha un ID. Alla fine, annota gli esiti nella tabella della sezione "Come riportare gli esiti".

Il round 9 aggiunge la schermata Importa. Le prove della schermata Importa hanno ID `I-T` (tablet) e `I-W` (PC). Le prove T, W e B restano con lo stesso ID del round 8.

Per la chiusura della fase F3 leggi anche [CHIUSURA-F3.md](CHIUSURA-F3.md) e [docs/handoff/09-PROVE-IMPORT-E-CHIUSURA-F3.md](../handoff/09-PROVE-IMPORT-E-CHIUSURA-F3.md).

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
| Import | Canto singolo, mini libretto, scansione e PDF danneggiato vengono letti come previsto | Runner Windows (job windows) |
| Import | Annullare durante la lettura e durante il riconoscimento del testo | Runner Windows (job windows) |
| Import | Esc, Ctrl+S, un clic vero sulla casella della dichiarazione, foglio modale | Runner Windows (job windows) |
| Import | Gli stessi casi di import della versione Windows, con i PDF di prova | Emulatore Android (job android-emulator) |

## Preparazione

Esegui questi passi una sola volta, prima della prima prova.

### Scaricare i file

1. Apri la pull request del round 9: https://github.com/F1-Solution/AppCanzoniere/pulls
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
25. Prepara tre PDF di cui hai il diritto di usare una copia:
    - un PDF con un canto singolo e testo selezionabile;
    - un PDF con un libretto di più canti e testo selezionabile;
    - una scansione: una foto o una copia scansionata di una pagina con un canto.

ATTENZIONE: non copiare i PDF nella cartella del progetto e non caricarli su GitHub.

26. Crea un file non valido chiamato `danneggiato.pdf`. Sul PC: apri il Blocco note, scrivi la parola `prova`, salva con tipo "Tutti i file".
27. Copia i quattro file sul PC in una cartella nota, per esempio "Documenti".
28. Copia i quattro file sul tablet nella cartella "Download".

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

### I-T1 — Scelta del PDF con il selettore di Android

**Scopo:** Controllare che il selettore di Android apra il PDF scelto, e che un file non valido dia un messaggio.

**Preparazione:** App aperta. I quattro file sono nella cartella "Download" del tablet (passo 28).

**Passi:**
1. Apri la scheda "Importa" nella barra.
2. Tocca "Scegli un PDF".
3. Nel selettore di Android, apri la cartella "Download".
4. Tocca il PDF con il canto singolo.
5. Controlla che compaia il nome del file.
6. Controlla che la lettura mostri "Pagina" con un numero di pagina e un totale.
7. Attendi la fine della lettura.
8. Torna alla scheda "Importa", tocca "Scegli un PDF" e scegli `danneggiato.pdf`.
9. Controlla che compaia un messaggio con un'azione. L'app non deve chiudersi.

**Risultato atteso:** Il PDF valido si legge fino alla fine. Il file non valido dà un messaggio e nessun canto.

**Cosa annotare:** OK o KO per il PDF valido e per il file non valido. Per KO, scrivi il numero del passo.

### I-T2 — Canto singolo con testo

**Scopo:** Controllare che un canto singolo si apra in revisione e si possa correggere.

**Preparazione:** Il PDF con il canto singolo è già letto (I-T1, passo 4 a 7).

**Passi:**
1. Tocca la riga del canto nell'elenco.
2. Conta le colonne che vedi: una, due o tre. Scrivi il numero.
3. Controlla che il testo originale mostri le righe con un "?" nel margine, se ci sono.
4. Scrivi il numero di righe con "?".
5. Nel pannello ChordPro, cambia una parola del testo.
6. Attendi qualche secondo.
7. Controlla che il pannello Anteprima mostri la modifica.

**Risultato atteso:** Il canto si apre in revisione. Le righe dubbie hanno il segno "?". L'anteprima segue la modifica.

**Cosa annotare:** Numero di colonne visibili. Numero di righe con "?". OK o KO per il passo 7.

### I-T3 — Libretto intero

**Scopo:** Controllare il riepilogo e l'elenco di un libretto.

**Preparazione:** Il PDF con il libretto è nella cartella "Download".

**Passi:**
1. Scegli il PDF del libretto con "Scegli un PDF".
2. Attendi la fine della lettura.
3. Leggi il riepilogo. Scrivi i quattro numeri: pagine con testo, pagine scansionate, canti trovati, canti nell'indice.
4. Tocca "Solo da controllare". Scrivi quanti canti restano nell'elenco.
5. Tocca "Nessuno". Controlla che il contatore dica 0 selezionati.
6. Controlla che "Salva" sia spento e che dica di selezionare almeno un canto.
7. Tocca "Tutti". Controlla che tutti i canti siano selezionati.

**Risultato atteso:** Il riepilogo mostra i quattro numeri. "Nessuno" toglie tutte le caselle e spegne il salvataggio. "Tutti" le rimette.

**Cosa annotare:** I quattro numeri. OK o KO per "Solo da controllare", "Nessuno" e "Tutti".

### I-T4 — Scansione

**Scopo:** Controllare la lettura di una scansione e l'annullamento.

ATTENZIONE: la prima lettura di una scansione può richiedere diversi minuti. Non chiudere l'app.

**Preparazione:** La scansione è nella cartella "Download".

**Passi:**
1. Scegli la scansione con "Scegli un PDF".
2. Controlla che "Pagina n di m" e la barra di avanzamento cambino.
3. Attendi la fine della lettura. Annota il tempo.
4. Controlla che la riga dei canti scansionati abbia l'etichetta "Scansione".
5. Scegli di nuovo la scansione con "Scegli un PDF".
6. Mentre la lettura è in corso, tocca "Annulla".
7. Controlla che compaia "Lettura annullata. Nessun canto importato."

**Risultato atteso:** La scansione si legge fino alla fine. L'annullamento a metà lettura non lascia canti.

**Cosa annotare:** Tempo di lettura in minuti. OK o KO per il passo 4 e per il passo 7.

### I-T5 — Dichiarazione e salvataggio

**Scopo:** Controllare che la dichiarazione non sia spuntata all'inizio e che Salva resti spento finché non si spunta.

**Preparazione:** Il canto singolo è letto (I-T2).

**Passi:**
1. Tocca la riga del canto. Il canto si apre in revisione.
2. Tocca "Salva questo canto".
3. Controlla che si apra il foglio della dichiarazione.
4. Controlla che la casella sia non spuntata.
5. Controlla che "Salva" sia spento e che dica di spuntare la dichiarazione.
6. Tocca la riga della dichiarazione, non solo la casella. Controlla che la casella si spunti.
7. Tocca di nuovo la riga. Controlla che la casella si tolga.
8. Tocca di nuovo la riga per spuntarla.
9. Tocca "Salva".
10. Controlla che compaia il banner "Salvato" in cima alla pagina.

**Risultato atteso:** La casella è non spuntata all'apertura. "Salva" si accende solo con la casella spuntata. Il canto è salvato una volta.

**Cosa annotare:** OK o KO. Per KO, scrivi il numero del passo.

### I-T6 — Apri il canto salvato

**Scopo:** Controllare che il canto salvato si apra con il testo corretto.

**Preparazione:** Il banner "Salvato" è visibile (I-T5, passo 10).

**Passi:**
1. Tocca "Apri" nel banner.
2. Controlla che il canto si apra con il testo corretto.
3. Torna indietro con il gesto Indietro di Android.
4. Controlla che la scheda "Importa" torni nello stesso stato, con la riga del canto salvata.

**Risultato atteso:** Il canto salvato si apre. Il ritorno lascia la scheda nello stato di prima.

**Cosa annotare:** OK o KO. Scrivi il titolo del canto salvato.

### I-T7 — Rotazione durante la revisione

**Scopo:** Controllare che la rotazione del tablet non perda il canto aperto e la modifica.

**Preparazione:** Un canto è aperto in revisione (I-T2).

ATTENZIONE: non salvare il canto prima della rotazione. La prova controlla che la modifica resti senza salvarla.

**Passi:**
1. Nel pannello ChordPro, cambia una parola.
2. Ruota il tablet di 90 gradi.
3. Controlla che il canto aperto resti aperto.
4. Controlla che la modifica resti.
5. Controlla che il pannello o il segmento scelto resti lo stesso.
6. Ruota il tablet di nuovo, nella posizione di partenza.
7. Controlla gli stessi punti dei passi 3, 4 e 5.

**Risultato atteso:** Nessuna delle due rotazioni perde il canto, la modifica o il pannello scelto.

**Cosa annotare:** OK o KO. Per KO, scrivi il numero del passo.

### I-T8 — Gesto Indietro

**Scopo:** Controllare che il gesto Indietro di Android faccia la cosa giusta in ogni stato.

**Preparazione:** Il PDF del libretto è letto (I-T3). Un canto è aperto in revisione.

ATTENZIONE: questa prova vale con il tablet in verticale, con una o due colonne. Con tre colonne il progetto non definisce il gesto Indietro. Se succede qualcosa di diverso, annotalo.

**Passi:**
1. Con un canto aperto in revisione, usa il gesto Indietro.
2. Controlla che il canto si chiuda e torni all'elenco. L'app non deve chiudersi.
3. Tocca "Salva questo canto" per aprire il foglio della dichiarazione.
4. Usa il gesto Indietro.
5. Controlla che il foglio si chiuda e che il canto resti aperto.
6. Avvia di nuovo la lettura con "Scegli un PDF" su una scansione.
7. Durante la lettura, usa il gesto Indietro.
8. Controlla che compaia una domanda di conferma prima di annullare.

**Risultato atteso:** Indietro chiude il canto, poi chiude il foglio, poi chiede conferma durante la lettura.

**Cosa annotare:** OK o KO per i passi 2, 5 e 8. Per KO, scrivi il numero del passo.

### I-T9 — Dimensione dei tasti principali

**Scopo:** Controllare che i pulsanti principali siano alti almeno 56 dp.

**Preparazione:** Righello in millimetri. 56 dp equivalgono a circa 8,9 mm. Per la misura usa una riga di un canto in revisione.

**Passi:**
1. Apri la scheda "Importa" senza file letti.
2. Misura in mm l'altezza del pulsante "Scegli un PDF".
3. Apri il libretto (I-T3).
4. Misura in mm l'altezza del pulsante "Salva n canti selezionati" in fondo all'elenco. Se è spento, misura lo stesso.
5. Apri un canto in revisione e tocca "Salva questo canto".
6. Misura in mm l'altezza del pulsante "Salva" nel foglio.

**Risultato atteso:** Ogni misura è di almeno 8,9 mm.

**Cosa annotare:** Le tre misure in mm. OK o KO. Per KO, scrivi quale pulsante.

### I-T10 — Tempo di correzione per il Gate 0 (traccia B)

**Scopo:** Misurare il tempo di correzione dei canti reali con il cronometro della schermata.

ATTENZIONE: il cronometro è spento per impostazione predefinita. Accendilo prima di aprire un canto.

**Preparazione:** Uno dei 10 libretti della prova B1. Timer pronto. Il foglio di registrazione in [CHIUSURA-F3.md](CHIUSURA-F3.md) è pronto.

**Passi:**
1. Nella schermata iniziale, accendi "Misura i tempi di correzione".
2. Scegli il PDF del libretto.
3. Attendi la fine della lettura.
4. Tocca il primo dei 5 canti scelti. Il tempo parte.
5. Correggi il canto fino a renderlo uguale al PDF originale.
6. Leggi il tempo in alto nella revisione. Scrivi i minuti.
7. Tocca "Succ. >" per passare al canto successivo. Il tempo del canto precedente resta nell'elenco.
8. Ripeti i passi 5 a 7 per gli altri canti scelti.
9. Torna all'elenco. Tocca "Copia i tempi".
10. Incolla il testo nel foglio di registrazione.

**Risultato atteso:** Per ogni canto un tempo in minuti. Il testo copiato contiene titolo, pagine, minuti, "sì" o "no" e piattaforma.

**Cosa annotare:** Numero di canti provati. Numero di canti con "sì". Piattaforma: Android.

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

**Preparazione:** La cartella estratta da `windows-debug-build` deve essere pronta.

**Passi:**
1. Apri la cartella estratta.
2. Controlla che esista la sottocartella `PdfWorker`.
3. Avvia `AppCanzoniere.MAUI.exe` dalla cartella estratta.
4. Controlla che l'app si apra.
5. Apri la scheda "Importa". Controlla che compaia il pulsante "Scegli un PDF".

**Risultato atteso:** La sottocartella `PdfWorker` esiste. L'app si apre. La scheda Importa mostra il pulsante.

**Cosa annotare:** OK o KO. Se la sottocartella ha un nome diverso, scrivi il nome.

### I-W1 — Scelta del PDF con Ctrl+O

**Scopo:** Controllare che Ctrl+O apra il selettore di file di Windows e che il PDF si legga.

**Preparazione:** I quattro file sono nella cartella "Documenti" del PC (passo 27). L'app è aperta.

**Passi:**
1. Apri la scheda "Importa" con un clic.
2. Premi Ctrl+O.
3. Controlla che si apra il selettore di file di Windows.
4. Scegli il PDF con il canto singolo.
5. Controlla che la lettura parta e che compaia l'elenco.
6. Scrivi quante righe ha l'elenco.
7. Torna a "Scegli un PDF" e scegli `danneggiato.pdf`.
8. Controlla che compaia un messaggio con un'azione. L'app non deve chiudersi.

**Risultato atteso:** Ctrl+O apre il selettore. Il PDF valido si legge. Il file non valido dà un messaggio.

**Cosa annotare:** OK o KO per ogni passo. Per KO, scrivi il numero del passo.

### I-W2 — Libretto intero

**Scopo:** Controllare il riepilogo, l'elenco e Tutti/Nessuno con un libretto su PC.

**Preparazione:** Il PDF del libretto è nella cartella "Documenti".

**Passi:**
1. Scegli il PDF del libretto con Ctrl+O.
2. Attendi la fine della lettura.
3. Leggi il riepilogo. Scrivi i quattro numeri: pagine con testo, pagine scansionate, canti trovati, canti nell'indice.
4. Fai clic su "Solo da controllare". Scrivi quanti canti restano.
5. Fai clic su "Nessuno". Controlla che "Salva" sia spento.
6. Fai clic su "Tutti". Controlla che tutti i canti siano selezionati.

**Risultato atteso:** Il riepilogo mostra i quattro numeri. Tutti e Nessuno cambiano le caselle.

**Cosa annotare:** I quattro numeri. OK o KO per i passi 4, 5 e 6.

### I-W3 — Scansione e tasto Esc

**Scopo:** Controllare la lettura di una scansione e l'annullamento con Esc.

**Preparazione:** La scansione è nella cartella "Documenti".

ATTENZIONE: la prima lettura di una scansione può richiedere diversi minuti. Non chiudere l'app.

**Passi:**
1. Scegli la scansione con Ctrl+O.
2. Controlla che "Pagina n di m" e la barra di avanzamento cambino.
3. Attendi la fine della lettura. Annota il tempo.
4. Controlla che la riga dei canti scansionati abbia l'etichetta "Scansione".
5. Scegli di nuovo la scansione con Ctrl+O.
6. Mentre la lettura è in corso, premi Esc.
7. Controlla che compaia "Lettura annullata. Nessun canto importato."

**Risultato atteso:** La scansione si legge fino alla fine. Esc annulla la lettura e non lascia canti.

**Cosa annotare:** Tempo di lettura in minuti. OK o KO per il passo 4 e per il passo 7.

### I-W4 — Tasti della revisione

**Scopo:** Controllare Alt+freccia, Ctrl+S e Esc nella revisione di un canto.

**Preparazione:** Un libretto è letto (I-W2). Un canto è aperto in revisione con un clic.

**Passi:**
1. Premi Alt+Freccia destra. Controlla che si apra il canto successivo.
2. Premi Alt+Freccia sinistra. Controlla che torni il canto precedente.
3. Premi Ctrl+S. Controlla che si apra il foglio della dichiarazione.
4. Controlla che la casella sia non spuntata e che "Salva" sia spento.
5. Premi Esc. Controlla che il foglio si chiuda e che il canto resti aperto.

**Risultato atteso:** Ogni tasto fa l'azione indicata. Con il foglio aperto, Esc chiude il foglio e non il canto.

**Cosa annotare:** OK o KO per ogni tasto. Per KO, scrivi il numero del passo.

### I-W5 — Ridimensionamento della finestra

**Scopo:** Controllare che la revisione passi da tre colonne a due, poi a una, senza perdere lo stato.

**Preparazione:** Scala di Windows al 100 %. Un canto è aperto in revisione. Modifica una parola nel pannello ChordPro, senza salvare.

**Passi:**
1. Trascina il bordo destro della finestra fino a circa 1300 px di larghezza.
2. Controlla che compaiano tre pannelli: Originale, ChordPro e Anteprima.
3. Trascina il bordo fino a circa 900 px.
4. Controlla che compaiano due colonne. A sinistra Originale e Anteprima con un selettore. A destra ChordPro.
5. Trascina il bordo fino a circa 600 px.
6. Controlla che compaia una colonna con tre selettori: Originale, ChordPro, Anteprima.
7. Controlla che la modifica e il canto aperto siano ancora lì.

**Risultato atteso:** Il layout cambia alla larghezza indicata. Il canto e la modifica restano.

**Cosa annotare:** La larghezza in cui il layout cambia, se diversa da 1200 px e da 700 px. OK o KO per il passo 7.

### I-W6 — Tempo di correzione e "Copia i tempi" (traccia B)

**Scopo:** Misurare il tempo di correzione con il cronometro e copiare la tabella dei tempi.

ATTENZIONE: il cronometro è spento per impostazione predefinita. Accendilo prima di aprire un canto.

**Preparazione:** Uno dei 10 libretti della prova B1. Timer pronto. Il foglio di registrazione in [CHIUSURA-F3.md](CHIUSURA-F3.md) è pronto.

**Passi:**
1. Nella schermata iniziale, accendi "Misura i tempi di correzione" con un clic.
2. Scegli il PDF del libretto con Ctrl+O.
3. Attendi la fine della lettura.
4. Fai clic sul primo dei 5 canti scelti. Il tempo parte.
5. Correggi il canto fino a renderlo uguale al PDF originale.
6. Leggi il tempo in alto nella revisione. Scrivi i minuti.
7. Premi Alt+Freccia destra per passare al canto successivo.
8. Ripeti i passi 5 a 7 per gli altri canti scelti.
9. Torna all'elenco. Fai clic su "Copia i tempi".
10. Apri il Blocco note e premi Ctrl+V. Controlla che la tabella contenga i canti e la piattaforma "Windows".
11. Incolla il testo nel foglio di registrazione.

**Risultato atteso:** Per ogni canto un tempo in minuti. "Copia i tempi" copia la tabella negli appunti.

**Cosa annotare:** Numero di canti provati. Numero di canti con "sì". OK o KO per il passo 10.

## Prova dell'import (traccia B, con il report)

### B1 — Tempo di correzione dei libretti (Gate 0), con il report

**Scopo:** Misurare quanto tempo serve per correggere un canto importato da un libretto, usando il report del banco di prova.

**Preparazione:** PC di sviluppo, con il report del banco di prova. Timer pronto. Due tabelle vuote: una per i PDF con testo, una per le scansioni. Dal round 9 la schermata Importa è nell'app. Per la traccia B usa I-T10 e I-W6. B1 con il report resta come controllo, da fare solo se il report è disponibile.

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

- Tema Palco scuro e temi Leggio e Palco definitivi: arrivano con la fase F4. Fino ad allora la schermata Importa usa la palette provvisoria di Leggio.

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
| I-T1 | | |
| I-T2 | | |
| I-T3 | | |
| I-T4 | | |
| I-T5 | | |
| I-T6 | | |
| I-T7 | | |
| I-T8 | | |
| I-T9 | | |
| I-T10 | | |
| W1 | | |
| W2 | | |
| W3 | | |
| W4 | | |
| W5 | | |
| W6 | | |
| W7 | | |
| I-W1 | | |
| I-W2 | | |
| I-W3 | | |
| I-W4 | | |
| I-W5 | | |
| I-W6 | | |
| B1 | | |

Copia questa riga nella richiesta successiva, e sostituisci i valori tra parentesi:

`Esito prove: T1 ok, T2 ko (nota: passo 3, la pagina non gira), T3 ok, T4 ok, T5 ok, T6 ok, T7 ok, I-T1 ok, I-T2 ok, I-T3 ok (4 numeri: 12, 1, 150, 152), I-T4 ok (tempo 3 min), I-T5 ok, I-T6 ok, I-T7 ok, I-T8 ok, I-T9 ok (9 mm, 9 mm, 9 mm), I-T10 (canti 10, sì 7, Android), W1 ok, W2 ok, W3 ok, W4 ok, W5 ok, W6 ok, W7 ok, I-W1 ok, I-W2 ok, I-W3 ok, I-W4 ok, I-W5 ok (cambio a 1200 e 700 px), I-W6 (canti 10, sì 8, Windows), B1 (PDF con testo: 62%; scansioni: 41%).`
