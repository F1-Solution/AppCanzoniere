# Compiti del proprietario, fase per fase

Questo documento dice cosa fai tu prima e dopo ogni fase del progetto, da F4 a F15. Dice anche cosa succede per le fasi condizionate, da F16 a F19.

Stato al 20/11/2026.

Fonti:
- `ROADMAP.md` (tabella delle fasi, colonna "Bloccata da").
- `docs/FASI.md`, sezione 4 (riga "Dal proprietario" di ogni fase), sezione 5 (fasi condizionate), sezione 10 (calendario).
- `docs/verifiche/PROVE-SU-DISPOSITIVO.md` (prove su tablet e PC).
- `docs/handoff/04-GATE-0.md` e `docs/handoff/07-VERIFICHE-ESTERNE-E-PROFESSIONISTI.md`.

ATTENZIONE: la sezione 11 di `docs/FASI.md` ("Ordine dei prossimi tre round") è vecchia. Inizia da F2, che è già completata (vedi sezione 5). Per decidere il prossimo round, usa la colonna "Dipende da" di `ROADMAP.md`.

---

## 1. Tabella riassuntiva

| Fase | Prima del round | Dopo il round | Blocca |
|---|---|---|---|
| F4 Token, cinque temi, componenti | Scegli Phosphor o Lucide. Conferma lo stile del ritornello per tema | Prova il catalogo dei componenti su tablet e PC, nei due temi predefiniti | Scelta delle icone (tua) |
| F5 Prove visive in CI | Verifica i minuti di emulatore e Windows sul piano GitHub | Apri una PR di prova che cambia un colore. Controlla che la CI fallisca | V-09 (minuti e runner) |
| F6 Persistenza, impostazioni, tre modalità, libreria | Raccogli 20-30 canti propri. Tieni pronti tablet e PC | Prova l'aggiornamento con migrazione su tablet e PC | Nulla per la persistenza. I due temi di F4 per la UI |
| F7 Leggio e import (T1, 21/03/2027) | Prova in chiesa con poca luce. Cronometra 5 canti per piattaforma. Conferma il pedale | Video del T1 su tablet e PC. Prova import e rete spenta | Esito della prova import (Gate 0, attività 5) |
| F8 Scalette, condivisione, backup | Due dispositivi Android e il PC. Una scaletta reale | Prova in parrocchia: QR, file, backup da Android a Windows e viceversa | Nulla oltre le dipendenze |
| F9 Calendario liturgico | Procurati un calendario ufficiale per 2026-2028. Chiedi al legale (V-11) | Controllo a campione di 30 giorni contro il calendario ufficiale | Calendari di riferimento (tuoi); V-11 |
| F10 Modalità Liturgia | Elenco dei momenti della Messa senza canto da inserire | Due domeniche nella tua parrocchia. Annota i problemi | Nulla oltre le dipendenze |
| F11 Pubblicità e consenso | Account AdMob. Unità pubblicitarie. Categorie bloccate. Esito di V-01, V-02, V-06 | Prova su tablet: consenso prima degli annunci; nessun annuncio su canto e Palco | Account AdMob; V-01, V-02, V-06, V-07 |
| F12 Acquisti in app (Pro, donazioni) | Profilo dei pagamenti su Google e Microsoft. Prodotti nelle console. Tester delle licenze | Acquisto di prova su Google Play. Ripristino. Donazione di prova | Profilo pagamenti; V-03, V-04, V-05 |
| F13 Canali di rilascio | D10 (entro 28/02/2027). Account Play. Segreti. Tester (12 o più). Decisione D12 | Installa la build di Play dal tablet. Installa il pacchetto Windows senza aiuto | D10; account Play e Partner Center; segreti; V-04, V-08, V-10 |
| F14 Anteprima e pilota | Almeno 3 cori. Ambasciatore. Parere legale (D11). Dominio ed email. Canti di dominio pubblico | Note settimanali dei cori. Verifica dei gate il 20/06/2027 e il 24/10/2027 | Cori e 12 tester (Gate 0); D11; V-05; dominio ed email |
| F15 Lancio | Testi delle schede e video. Dominio e servizio per le donazioni. Dichiarazioni fiscali. Corsi diocesani | Controllo dei rilasci graduali e delle schede dello store | Gate Pilota; D12; posizione fiscale; V-03 |
| F16 iPad e iOS (condizionata) | Account Apple (99 $ all'anno). Runner macOS | [DA VERIFICARE: definito quando la fase si apre] | Gate 1; censimento con iPad oltre 40% |
| F17 Piano Coro e licenza diocesana (condizionata) | Hosting. Secondo parere legale (≈ 800 €). Contratti | [DA VERIFICARE: definito quando la fase si apre] | Gate 2; D13; D14 |
| F18 Funzioni a pagamento (condizionata) | Decidi il prezzo (`FASI.md` §0.1). Richieste scritte di almeno 3 cori | [DA VERIFICARE] | D15; richieste dei cori |
| F19 Estensioni di contenuto (condizionata) | Fonti per il calendario ambrosiano, se richiesto | [DA VERIFICARE] | Gate 1; richieste esplicite |

---

## 2. Come usare questo documento

1. Leggi la colonna "Quando arriva" della fase. La fase parte solo quando le sue dipendenze sono chiuse.
2. Fai i compiti della sezione "Prima del round" prima di dare il prompt all'agente.
3. Dopo il round, fai le prove della sezione "Dopo il round". Scrivi gli esiti come nel modello della sezione 4 di questo documento.
4. Usa il prompt della fase. Sostituisci i valori tra parentesi quadre.

**Modello di prompt per ogni round:**

```
Round [N]: fase F[x] — [titolo]. Leggi docs/FASI.md, sezione 4, F[x], e i documenti indicati nella fase.
Ramo f[x]/[argomento]. Una pull request. Test prima del codice. Review Opus xhigh.
Rispetta i criteri di accettazione della fase. Aggiorna ROADMAP.md e PROGRESS.md.
Prima di chiudere la fase, esegui la verifica dei criteri. Non chiudere con CRITICAL o HIGH aperti.
```

**Regole che valgono per ogni round:**
- Non unire una PR senza CI verde.
- Non fare commit diretti su `main`.
- Tieni le chiavi e i segreti fuori dai file del repository.
- Le prove su tablet e PC restano a te. L'agente non le può fare.

---

## 3. Compiti per fase

### Note su F1 e F3 (in corso)

**F1 (risanamento e CI).** Il tuo compito aperto è questo:
- Ripara i problemi dei workload MAUI in locale, come amministratore (`VALUTAZIONE.md` §2.1).
- Verifica i minuti Actions e la protezione del branch sul piano GitHub (V-09).
- Ordina un tablet (100-150 €) e un pedale (30-60 €).
- Tieni a disposizione un PC Windows 10 o 11.

**F3 (prove tecniche).** Le prove su dispositivo restano da fare. Sono nella guida `docs/verifiche/PROVE-SU-DISPOSITIVO.md`: T1-T7 sul tablet, W1-W7 sul PC, B1 sui libretti. Segui la guida nell'ordine scritto. Scrivi l'esito nella tabella della guida. Poi riporta all'agente con la riga "Esito prove: ...".

---

### F4 — Token, cinque temi, componenti

**Quando arriva:** Dopo l'unione della PR di F1 su `main`, con CI verde. La prima parte si fa prima del Gate 0 (3 round). Il resto a febbraio 2027 (2 round).

**Blocca:** Scelta del set di icone (tua).

**Prima del round:**

1. Nessuna azione per la taste-skill: è già installata.
2. Scegli il set di icone: Phosphor oppure Lucide. Vedi la procedura sotto.
3. Conferma lo stile del ritornello per ogni tema. Le proposte sono: corsivo per Leggio, grassetto per Palco, riquadro tinto per Vetrata (`docs/design/ui/CONFRONTO-UI.md`, decisione 3). Scrivi "confermo" oppure la tua scelta.

**taste-skill:** già installata. Verificata il 20/11/2026: la skill `taste-skill:design-taste-frontend` è attiva nella sessione dell'agente. Nessuna azione.

**Procedura: scegliere il set di icone**

**Scopo:** Fissare un solo set di icone al tratto per tutta l'app.

**Passi:**

1. Chiedi all'agente: "Confronta Phosphor e Lucide per licenza, numero di icone e uso come font, per le schermate di `docs/design/ui/`."
2. Leggi la risposta.
3. Scrivi nella richiesta del round: "Icone: Phosphor" oppure "Icone: Lucide".

**Dopo il round:**

1. Apri il catalogo dei componenti. Il catalogo è disponibile solo nella build di debug.
2. Installa la build di debug su tablet e apri il catalogo.
3. Nelle impostazioni, scegli "Leggio chiaro". Controlla pulsanti, chip, elenco, campo di ricerca, barra dei controlli e riga degli accordi. Controlla che il testo sia leggibile a una distanza di lettura.
4. Scegli "Palco scuro". Ripeti il controllo.
5. Ripeti i passi 2-4 su PC Windows.
6. Leggi nella PR il peso dell'APK. Il criterio è [IPOTESI] 8 MB in più al massimo.

**Risultato atteso:** Catalogo visibile e leggibile su tablet e PC, nei due temi predefiniti. Peso dell'APK scritto nella PR.

**Cosa riportare:** "F4 prova: tablet Leggio chiaro OK/KO; tablet Palco scuro OK/KO; PC Leggio chiaro OK/KO; PC Palco scuro OK/KO; peso APK: X MB."

**Prompt:**

```
Round F4 (token, temi, componenti). Leggi docs/FASI.md, sezione 4, F4, e docs/design/ui/.
Usa la taste-skill per la specifica dei token. Non rifare le proposte UI.
Icone: [Phosphor o Lucide]. Ritornelli: [Leggio corsivo, Palco grassetto, Vetrata riquadro tinto, oppure la tua scelta].
Ramo f4/token-temi. Una PR. Test prima del codice. Review Opus xhigh.
```

---

### F5 — Prove visive automatiche in CI

**Quando arriva:** Dopo F4 (catalogo dei componenti). Febbraio 2027 (3 round).

**Blocca:** Minuti e runner di CI (V-09).

**Prima del round:**

1. Apri il piano GitHub. Leggi i minuti Actions per un repository privato, e il moltiplicatore dei runner Windows (V-09).
2. Scrivi i numeri nella richiesta: "V-09: minuti X; moltiplicatore Y".

**Dopo il round:**

**Scopo:** Controllare che la CI segnali un cambio di colore non voluto.

**Passi:**

1. Chiedi all'agente di aprire una PR di prova. La PR cambia un colore di un token, di proposito.
2. Apri la PR. Controlla che la CI fallisca.
3. Scarica gli artefatti "prima", "dopo" e "differenza". Controlla che ci siano.
4. Chiudi la PR di prova senza unirla.

**Risultato atteso:** La CI fallisce. Le tre immagini sono negli artefatti.

**Cosa riportare:** "F5 prova: CI fallisce OK/KO; artefatti presenti OK/KO."

**Prompt:**

```
Round F5 (prove visive in CI). Leggi docs/FASI.md, sezione 4, F5.
Livello 1 su ogni PR, livello 2 sui due temi predefiniti. Ramo f5/prove-visive.
Una PR. Test prima del codice. Review Opus xhigh.
```

---

### F6 — Fondamenta: persistenza, impostazioni, tre modalità, libreria

**Quando arriva:** Dopo F1 e F2 (F2 è già completata). Per la UI serve F4. La persistenza può partire prima del Gate 0. Febbraio-aprile 2027 (6 round).

**Blocca:** Nulla per la persistenza. Per la UI, i due temi predefiniti di F4.

**Prima del round:**

1. Raccogli 20-30 canti tuoi. Servono per le prove.
2. Tieni pronti il tablet e il PC.
3. Conserva la build di F3 o di F5, per usarla come versione precedente nella prova di aggiornamento.

**Dopo il round:**

**Scopo:** Controllare che i dati sopravvivano a un aggiornamento con migrazione.

**Passi:**

1. Installa la build precedente su tablet.
2. Crea 3 canti e una raccolta. Scegli la modalità Scaletta.
3. Chiudi e riapri l'app. Controlla che i dati ci siano.
4. Installa la build nuova sopra la precedente. Non disinstallare.
5. Controlla che i 3 canti e la raccolta ci siano.
6. Controlla che esista il backup fatto prima della migrazione. [DA VERIFICARE: percorso del backup].
7. Cambia modalità (Libreria) e tema. Chiudi e riapri. Controlla che la scelta resti.
8. Ripeti i passi 1-7 su PC Windows.

ATTENZIONE: se la migrazione fallisce, non disinstallare. Riporta all'agente il messaggio esatto e il nome del file di backup.

**Risultato atteso:** Dati e impostazioni presenti dopo l'aggiornamento. Backup presente.

**Cosa riportare:** "F6 prova: aggiornamento tablet OK/KO; aggiornamento PC OK/KO; backup presente sì/no."

**Prompt:**

```
Round F6 (fondamenta). Leggi docs/FASI.md, sezione 4, F6, e docs/design/ux/.
Persistenza con migrazioni e backup prima di ogni migrazione. Tre modalità con un solo modello di dati.
Ramo f6/fondamenta. Una PR. Test prima del codice, su SQLite reale. Review Opus xhigh.
```

---

### F7 — Leggio e import (traguardo T1, 21/03/2027)

**Quando arriva:** Dopo F3 (prove tecniche) e F6. Dipende anche dall'esito del Gate 0, attività 5. Marzo 2027 (6 round).

**Blocca:** Esito della prova import (Gate 0, attività 5).

**Prima del round:**

1. Prova in una chiesa poco illuminata, o in un luogo con la stessa luce. Passi:
   1. Appoggia il tablet sul leggio, a distanza di lettura.
   2. Spegni le luci principali.
   3. Apri un canto. Leggi il testo. Controlla gli accordi.
   4. Annota la luminosità e il tipo di luce.
2. Scegli 5 canti importati da libretti. Cronometra la correzione di ciascuno, su tablet e su PC.
3. Conferma il pedale di riferimento. Controlla che sia in modalità tastiera e che gli accoppiamenti funzionino.

**Dopo il round:**

**Scopo:** Verificare il traguardo T1 su entrambe le piattaforme.

**Passi sul tablet:**

1. Prendi un canto dal libretto PDF della parrocchia.
2. Importalo. Nella revisione affiancata, correggi gli errori.
3. Controlla che la casella di dichiarazione non sia già spuntata. Spuntala tu.
4. Trasponi in Do-Re-Mi con il capotasto.
5. Apri la modalità Palco. Gira pagina con il pedale.
6. Registra un video dello schermo.

**Passi sul PC:** Ripeti i passi 1-6.

**Prova di rete:** Metti il tablet in modalità aereo. Disattiva la rete sul PC. Ripeti l'import e la lettura. Controlla che funzionino.

**Prova di import:** Per ciascuna delle 5 correzioni cronometrate, scrivi i minuti. Scrivi "sì" se sono 2 minuti o meno.

**Risultato atteso:** T1 su tablet e PC, con video. Import funzionante senza rete.

**Ripiego:** Se T1 slitta oltre il 18/04/2027, il T1 si fa senza OCR (`FASI.md` F7).

**Cosa riportare:** "F7 T1: tablet OK/KO; PC OK/KO; video: nome file. Import: tablet X minuti medi, sì Y su 5; PC X minuti medi, sì Y su 5. Rete spenta: OK/KO."

**Prompt:**

```
Round F7 (leggio e import, T1 entro il 21/03/2027). Leggi docs/FASI.md, sezione 4, F7.
Esito del Gate 0, attività 5: [OCR nella v1 / senza OCR / import fuori dalla v1].
Ramo f7/leggio-import. Una PR. Test prima del codice, con fixture PDF in tests/. Review Opus xhigh.
Nessuna richiesta di rete durante import e lettura.
```

---

### F8 — Scalette, repertori, condivisione, backup

**Quando arriva:** Dopo F6 e F7. Aprile 2027 (6 round).

**Blocca:** Nulla oltre le dipendenze.

**Prima del round:**

1. Prepara due dispositivi Android e il PC.
2. Prepara una scaletta reale, per la prossima domenica nella tua parrocchia.

**Dopo il round:**

**Scopo:** Verificare la scaletta, la condivisione e il backup.

**Passi:**

1. Crea la scaletta di domenica con il modello Messa.
2. Mostra il QR della scaletta. Leggilo con il primo tablet.
3. Controlla che il tablet veda i canti mancanti.
4. Invia i canti mancanti come file. Controlla che arrivino.
5. Sul PC, apri la scaletta. Controlla che sia identica.
6. In Palco, passa da un canto al successivo con il pedale. Controlla l'anteprima del successivo.
7. Crea un backup su Android. Ripristinalo su Windows.
8. Crea un backup su Windows. Ripristinalo su Android.

**Risultato atteso:** La scaletta arriva ai dispositivi. Il backup si ripristina tra piattaforme.

**Cosa riportare:** "F8 prova: QR OK/KO; file OK/KO; pedale in Palco OK/KO; backup da Android a Windows OK/KO; backup da Windows ad Android OK/KO."

**Prompt:**

```
Round F8 (scalette, condivisione, backup, export). Leggi docs/FASI.md, sezione 4, F8.
Modello Messa per la scaletta. QR con la sola scaletta. Backup in un archivio uguale su Android e Windows.
Ramo f8/scalette-condivisione. Una PR. Test prima del codice, incluso il ripristino tra piattaforme. Review Opus xhigh.
```

---

### F9 — Calendario liturgico

**Quando arriva:** Dopo F1 e F2 (F2 è già completata). Può andare in parallelo a F6-F8. Il motore è a febbraio 2027; la tabella dei riferimenti a marzo 2027 (6 round).

**Blocca:** Calendari di riferimento per i test (tuoi). V-11 (parere del legale).

**Prima del round:**

1. Procurati il calendario liturgico ufficiale per gli anni 2026-2028. Il piano chiede il calendario CEI annuale oppure un Ordo. [DA VERIFICARE: la fonte disponibile].
2. Porta al legale la domanda A11 (`07-VERIFICHE-ESTERNE-E-PROFESSIONISTI.md`). Chiedi se una tabella di soli riferimenti (non i testi) è consentita.

**Dopo il round:**

**Scopo:** Controllare che il calendario dell'app sia uguale a quello ufficiale.

**Passi:**

1. Scegli 30 giorni a caso, tra il 2026 e il 2028.
2. Per ogni giorno, confronta con il calendario ufficiale: tempo liturgico, settimana, ciclo (A, B o C), settimana feriale (I o II), celebrazione, colore, letture.
3. Controlla anche questo esempio: la domenica 15/11/2026 è la XXXIII del Tempo Ordinario, anno A, con Vangelo Mt 25,14-30.
4. Scrivi le differenze in una lista.

**Risultato atteso:** Zero differenze sui 30 giorni.

**Cosa riportare:** "F9 controllo: 30 giorni; differenze: N. Elenco: data, campo, valore app, valore ufficiale."

**Prompt:**

```
Round F9 (calendario liturgico, motore e tabella dei riferimenti). Leggi docs/FASI.md, sezione 4, F9.
Rito romano con calendario proprio d'Italia. Solo riferimenti delle letture, mai i testi.
Test sugli anni ufficiali. Fonte per ogni voce. Ramo f9/calendario. Una PR. Review Opus xhigh mirata alle precedenze.
```

---

### F10 — Modalità Liturgia

**Quando arriva:** Dopo F8 e F9. Aprile 2027 (4 round).

**Blocca:** Nulla oltre le dipendenze.

**Prima del round:**

1. Scrivi un elenco dei momenti della Messa senza canto che vuoi nella scaletta (per esempio letture e preghiere).

**Dopo il round:**

**Scopo:** Usare la modalità in una Messa vera, prima del lancio.

**Passi:**

1. Usa la modalità Liturgia per due domeniche nella tua parrocchia.
2. Ogni domenica, annota: la schermata della prossima celebrazione, i suggerimenti per la Comunione, i canti che hai scelto.
3. Controlla che il Gloria non compaia in Avvento e in Quaresima, salvo le solennità.
4. Annota ogni problema con la data e il momento della Messa.

**Risultato atteso:** Due domeniche con la modalità usata. Lista dei problemi.

**Cosa riportare:** "F10 prova: domenica X, problemi: elenco. Domenica Y, problemi: elenco."

**Prompt:**

```
Round F10 (modalità Liturgia). Leggi docs/FASI.md, sezione 4, F10, e docs/design/ux/UX-C-liturgia.md.
Celebrazioni con slot per momento. Suggerimenti locali, senza AI e senza rete. Ramo f10/liturgia. Una PR. Review Opus xhigh.
```

---

### F11 — Pubblicità e consenso

**Quando arriva:** Dopo F6 e F13 (identificativo dell'app, da marzo 2027). Maggio-giugno 2027 (3 round). Va completata prima del pilota (12/09/2027).

**Blocca:** Account AdMob; V-01, V-02, V-06, V-07.

**Prima del round:**

1. Crea l'account AdMob. Collegalo al profilo dei pagamenti (`07-VERIFICHE-ESTERNE-E-PROFESSIONISTI.md`, sezione 5).
2. Crea le unità pubblicitarie. Usa le unità di prova per lo sviluppo.
3. Blocca le categorie sensibili (V-01, punti 1-2).
4. Chiudi V-01 (punti 1-7), V-02 e V-06. Riporta l'esito all'agente.

**Dopo il round:**

**Scopo:** Controllare che la pubblicità compaia solo dove è ammessa.

**Passi sul tablet Android:**

1. Installa la build di prova. Avvia l'app per la prima volta. Controlla che compaia la richiesta di consenso prima di ogni annuncio.
2. Vai nelle schermate di elenco e di gestione. Controlla che ci sia un banner di prova.
3. Chiudi e riapri l'app a freddo. Controlla che l'annuncio di apertura compaia.
4. Apri una scaletta. Chiudi l'app. Riaprila. Controlla che l'annuncio di apertura non compaia.
5. Apri un canto. Apri il Palco. Apri una scaletta aperta. Controlla che non ci siano annunci.
6. Attiva il Pro, quando la fase F12 è pronta. Controlla che nessun annuncio compaia.
7. Metti il tablet in modalità aereo. Avvia l'app. Controlla che i tempi siano uguali a prima.

**Passi sul PC:** Controlla che non ci siano annunci.

ATTENZIONE: per il controllo "nessun annuncio prima del consenso" serve un registro di rete. [DA VERIFICARE: strumento per il registro di rete].

**Risultato atteso:** Consenso prima degli annunci. Nessun annuncio su canto, Palco e scaletta aperta.

**Cosa riportare:** "F11 prova: consenso OK/KO; banner OK/KO; apertura a freddo OK/KO; scaletta in corso OK/KO; canto e Palco OK/KO; Pro OK/KO; modalità aereo OK/KO."

**Prompt:**

```
Round F11 (pubblicità e consenso). Leggi docs/FASI.md, sezione 4, F11, e docs/verifiche/V-01-V-02-pubblicita.md.
Annunci solo nelle schermate ammesse. Consenso prima di ogni richiesta. Nessun SDK pubblicitario con il Pro attivo.
Ramo f11/pubblicita. Una PR. Test prima del codice. Review Opus xhigh.
```

---

### F12 — Acquisti in app (Pro e donazioni)

**Quando arriva:** Dopo F6 e F13. Aprile-maggio 2027 (4 round).

**Blocca:** Profilo dei pagamenti su Google e Microsoft; V-03, V-04, V-05.

**Prima del round:**

1. Crea il profilo dei pagamenti su Google Play. Vedi `07-VERIFICHE-ESTERNE-E-PROFESSIONISTI.md`, sezione 5.
2. Crea il profilo fiscale su Microsoft Partner Center.
3. Crea in Play Console il prodotto Pro, non consumabile, a 4,99 €.
4. Crea in Play Console tre prodotti consumabili per le donazioni. [IPOTESI: 1,99 €, 4,99 €, 9,99 €. Decidi tu gli importi].
5. Aggiungi i tester delle licenze in Play Console.
6. Porta a termine V-03, V-04 e V-05. Riporta l'esito all'agente.

**Dopo il round:**

**Scopo:** Controllare il flusso di acquisto su Google Play.

**Passi:**

1. Sul tablet, con l'account tester, acquista il Pro. Controlla che la pubblicità sparisca.
2. Chiudi l'app. Metti il tablet in modalità aereo. Riapri l'app. Controlla che il Pro resti.
3. Disinstalla l'app. Reinstallala. Ripristina l'acquisto. Controlla che il Pro torni.
4. Fai una donazione di prova. Controlla che compaia il ringraziamento.
5. Controlla che non ci sia nessun pulsante verso la pagina esterna di donazione.

**Su Windows:** Gli acquisti Windows si possono provare solo con l'app pubblicata nello Store, anche in pubblico privato (V-04). Prima di quel momento, non c'è una prova Windows.

**Risultato atteso:** Pro attivo e persistente. Ripristino funzionante. Nessun collegamento esterno.

**Cosa riportare:** "F12 prova: acquisto OK/KO; modalità aereo OK/KO; ripristino OK/KO; donazione OK/KO; nessun collegamento esterno OK/KO."

**Prompt:**

```
Round F12 (acquisti in app: Pro e donazioni). Leggi docs/FASI.md, sezione 4, F12, e docs/verifiche/V-03-V-04-V-05-store-e-fisco.md.
Pro non consumabile a 4,99 €. Donazioni come consumabili, senza vantaggi. Nessun prezzo nel codice.
Ramo f12/acquisti. Una PR. Test prima del codice. Review Opus xhigh.
```

---

### F13 — Canali di rilascio

**Quando arriva:** Dopo F1. Si sviluppa in parallelo a F7-F10. Parte 1 a marzo 2027; parte 2 da aprile a luglio 2027.

**Blocca:** D10; account Play e Partner Center; segreti; V-04, V-08, V-10.

**Prima del round (parte 1, entro marzo 2027):**

1. Prendi la decisione D10 (nome, `ApplicationId`, tipo di account). Scadenza: 28/02/2027.
2. Apri l'account Google Play Console, come account personale. Costo: 25 $. Verifica dell'identità.
3. Raccogli gli indirizzi Google di almeno 12 tester.
4. Crea la chiave di caricamento. Conserva il file e le password in un luogo sicuro, fuori dal repository.
5. Inserisci questi segreti nel repository, solo nella sezione dei segreti: `ANDROID_KEYSTORE_BASE64`, `ANDROID_KEYSTORE_PASSWORD`, `ANDROID_KEY_ALIAS`, `ANDROID_KEY_PASSWORD`, `PLAY_SERVICE_ACCOUNT_JSON`.
6. Esegui il primo caricamento manuale, se Play lo chiede (V-10).
7. Decidi D12: dove pubblicare le release Windows del pilota (vedi `07-VERIFICHE-ESTERNE-E-PROFESSIONISTI.md`, sezione 6, V-08).
8. Se scegli un certificato per il pacchetto pilota, procuralo (V-08).

**Prima del round (parte 2, da aprile):**

1. Apri l'account Microsoft Partner Center, dopo la risposta del commercialista (domanda 6). Vedi `07-VERIFICHE-ESTERNE-E-PROFESSIONISTI.md`, sezione 5.
2. Prenota l'app in Partner Center.

**Dopo il round (parte 1):**

**Scopo:** Controllare il rilascio su Play e il pacchetto Windows.

**Passi:**

1. Chiedi all'agente di avviare una prova con un tag di prova, per esempio `v0.1.0-prova`.
2. Controlla che la build di Play compaia nella traccia interna.
3. Installa l'app da Play sul tablet. Controlla che si apra e che un canto si apra.
4. Controlla che il pacchetto Windows sia nella release di GitHub.
5. Fai installare il pacchetto Windows a una persona di un coro, senza assistenza. Scrivi cosa fa.

**Dopo il round (parte 2):**

1. Chiedi di provare il passaggio dal pacchetto pilota allo Store su un PC con dati veri.
2. Crea un backup. Installa la versione dello Store. Ripristina il backup.
3. Controlla che i canti e le scalette siano presenti.

**Risultato atteso:** Build su Play installabile dal tablet. Pacchetto Windows installabile senza assistenza. Passaggio allo Store provato.

**Cosa riportare:** "F13 prova: traccia interna OK/KO; installazione da Play OK/KO; pacchetto Windows dalla release OK/KO; installazione senza aiuto: sì/no, annotazioni; passaggio allo Store OK/KO."

**Prompt:**

```
Round F13 (canali di rilascio, parte 1). Leggi docs/FASI.md, sezione 4, F13.
Nome e ApplicationId: [decisione D10]. Sede delle release Windows: [decisione D12].
Build di Play da tag, con AAB firmato. Pacchetto Windows in una release GitHub. Segreti solo nei segreti del repository.
Ramo f13/rilascio. Una PR. Test prima del codice. Review Opus xhigh.
```

---

### F14 — Anteprima ai cori e pilota

**Quando arriva:**
- Anteprima: dopo F7, F8 e F13 parte 1. Inizia il 02/05/2027. Gate Anteprima il 20/06/2027.
- Pilota completo: dopo F10, F11, F12 e F13 parte 2. Va dal 12/09/2027. Gate Pilota il 24/10/2027.

**Blocca:** Cori e 12 tester (Gate 0, attività 11); parere legale (D11); V-05; dominio ed email.

**Prima dell'anteprima:**

1. Conferma almeno 3 cori. Ogni coro deve avere un tablet Android. Ogni coro deve avere un referente.
2. Conferma l'ambasciatore (Gate 0, attività 13).
3. Conferma almeno 12 tester per il test chiuso su Google Play. Il test dura 14 giorni.
4. Affida al legale il gruppo A delle domande (`07-VERIFICHE-ESTERNE-E-PROFESSIONISTI.md`, sezione 4). Budget: circa 400 €.
5. Registra il dominio. Crea un sito minimo con informativa e modulo di segnalazione. Crea un'email dedicata.
6. Seleziona 20-40 canti di dominio pubblico. Documenta la verifica di ogni canto.

**Durante l'anteprima:**

1. Ogni settimana, chiedi a ogni coro una nota breve. Riporta le note all'agente.
2. Non chiedere aggiornamenti da venerdì a domenica.

**Gate Anteprima (20/06/2027). Controllo:**

- Almeno 3 cori usano l'app per almeno 3 domeniche su 4, per 6 settimane.
- Zero perdite di dati.
- Almeno il 99,5% delle sessioni senza crash su Play.
- Almeno 12 tester per almeno 14 giorni. L'accesso alla produzione è richiesto.
- Ogni coro ha lasciato una nota sulle tre modalità.

**Gate Pilota (24/10/2027). Controllo:** Le stesse soglie, con l'app completa (pubblicità, consenso, acquisti in prova). Più:
- Nessun avviso di policy aperto su Play, AdMob o Partner Center.
- Acquisti provati su entrambi gli store.
- Parere legale ricevuto.
- Posizione fiscale definita.

**Risultato atteso:** Ogni gate ha il suo esito scritto: "superato" oppure "non superato".

**Cosa riportare:** "Gate Anteprima: cori X su 3; domeniche Y; perdite Z; crash W%; tester N per M giorni; note: sì/no. Esito: ..."

**Prompt:**

```
Round F14 (anteprima ai cori e pilota). Leggi docs/FASI.md, sezione 4, F14, e docs/business/05-PIANO-COMMERCIALE.md, sezione 6.1.
Informativa privacy con pubblicità, consenso e acquisti. Schermate legali. Nucleo di canti di dominio pubblico documentato.
Ramo f14/anteprima-pilota. Una PR. Test prima del codice. Review Opus xhigh.
```

---

### F15 — Lancio

**Quando arriva:** Dopo F14 (Gate Pilota superato). Novembre 2027 (3 round). Lancio entro il 21/11/2027.

**Blocca:** Gate Pilota; D12; posizione fiscale definita; V-03.

**Prima del round:**

1. Scrivi i testi delle schede degli store in italiano. Usa solo canti tuoi o di dominio pubblico negli screenshot.
2. Scrivi la frase "suonare sarà sempre gratis" nelle schede. Vedi `docs/business/05-PIANO-COMMERCIALE.md`, §7.3.
3. Prepara tre video brevi.
4. Registra il dominio per la pagina di donazione. Scegli il servizio di pagamento esterno (DEC 12).
5. Decidi la dichiarazione fiscale e quella di "trader" (V-05, con il commercialista).
6. Prendi presenza in 1-2 corsi diocesani.

**Dopo il round:**

1. Controlla che la scheda su Play dica "contiene annunci" e "acquisti in app".
2. Controlla che il rilascio su Play sia graduale: 10%, poi 50%, poi 100%. Non rilasciare da venerdì a domenica.
3. Controlla che l'app sia pubblicata sul Microsoft Store.
4. Controlla il sito: informativa, termini, segnalazioni e pagina di donazione.
5. Controlla che l'app non contenga un collegamento alla pagina di donazione. Il collegamento resta spento finché V-03 non è positivo.

**Risultato atteso:** App in produzione su Play e pubblicata sul Microsoft Store entro il 21/11/2027. Sito pubblicato.

**Cosa riportare:** "F15: Play in produzione: sì/no, percentuale; Microsoft Store: sì/no; sito: OK/KO; collegamento esterno assente: sì/no."

**Prompt:**

```
Round F15 (lancio). Leggi docs/FASI.md, sezione 4, F15, e la sezione 1.2 per le date.
Schede in italiano con soli canti propri o di dominio pubblico. Rilascio graduale su Play, mai da venerdì a domenica.
Ramo f15/lancio. Una PR. Test prima del codice, incluso il test del blocco del fine settimana. Review Opus xhigh.
```

---

### F16-F19 — Fasi condizionate

Queste fasi si aprono solo se scatta il gate indicato. Non prepararle ora.

**F16 — iPad e iOS.**
- Quando si apre: Gate 1 (30/04/2028), e il censimento mostra più del 40% di iPad.
- Prima del round: account Apple Developer (99 $ all'anno). Certificati nei segreti. Runner macOS.
- Dopo il round: [DA VERIFICARE: la prova si definisce quando la fase si apre].
- Blocca: Gate 1; account Apple; runner macOS.

**F17 — Piano Coro e licenza diocesana.**
- Quando si apre: Gate 2 (30/11/2028), D13 e D14.
- Prima del round: hosting in UE. Secondo parere legale (≈ 800 €). Contratti. Partita IVA aperta.
- Dopo il round: [DA VERIFICARE: la prova si definisce quando la fase si apre].
- Blocca: Gate 2; D13; D14; secondo parere legale.

**F18 — Funzioni a pagamento.**
- Quando si apre: D15, Gate 2, e richieste scritte di almeno 3 cori.
- Prima del round: decidi il prezzo e il rapporto con il Pro (`FASI.md` §0.1).
- Dopo il round: [DA VERIFICARE].
- Blocca: D15; richieste dei cori.

**F19 — Estensioni di contenuto.**
- Quando si apre: Gate 1 e richieste esplicite.
- Prima del round: fonti per il calendario ambrosiano, se richiesto (DEC 7). Scegli le estensioni che vuoi (per esempio note per pianoforte, allegati, pianificatore a settimane).
- Dopo il round: [DA VERIFICARE].
- Blocca: Gate 1; richieste esplicite.

---

## 4. Modello per riportare gli esiti

Usa una riga per ogni prova:

`Esito [fase] [prova]: [OK o KO per ogni punto]. Note: [testo breve]. Per KO: numero del passo e cosa è successo.`

---

## 5. Punti da chiarire nei documenti

Questi punti non coincidono nelle fonti. Decidi tu, poi riporta la decisione all'agente.

1. **F2 è completata?** `ROADMAP.md` dice "completata (round 5)". `docs/FASI.md` §1 dice "da iniziare".
2. **Ordine dei round.** `docs/FASI.md` §11 parte da F2 e F3. È vecchio. Il prossimo round si sceglie dalle dipendenze della sezione 1.
3. **Numero del round.** Il round 8 è l'ultimo nel repository (`docs/PIANO-ROUND-8.md`). Non è scritto il numero del prossimo round.
4. **taste-skill.** Il 11/11/2026 non risultava attiva. Il 20/11/2026 è attiva: punto chiuso.
5. **Data di D12.** Decidi entro marzo 2027 (`FASI.md` §10). `05-PIANO-COMMERCIALE.md` §11 dice 30/06/2027. Vedi `07-VERIFICHE-ESTERNE-E-PROFESSIONISTI.md`, sezione 7.
