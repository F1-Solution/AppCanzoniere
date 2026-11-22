# Decisioni del proprietario — round 10 (21 novembre 2026)

Seguono le decisioni del [round 9](DECISIONI-ROUND-9.md). Sono le risposte del proprietario al piano del round, [PIANO-ROUND-10.md](PIANO-ROUND-10.md).

## Decisioni del proprietario

| # | Decisione | Effetto | Chi |
|---|---|---|---|
| DEC 36 | Confermata. I PDF si leggono nel processo dell'app, con i limiti contro i file costruiti ad arte. Le pagine scansionate sono disegnate nel processo dell'app. Lo spostamento nel processo separato è in F7. | Un PDF malevolo può chiudere l'app fino a F7. Su Android un lavoro scaduto continua in background finché non finisce; un nuovo import attende la sua fine. | Proprietario, 21/11/2026 |
| DEC 37 | Confermata. La schermata di import sostituisce la pagina OCR simulata e salva i canti nella libreria attuale. | I canti importati possono andare persi quando F6 rifà la persistenza. | Proprietario, 21/11/2026 |
| DEC 38 | Confermata. Prima di ogni salvataggio, una dichiarazione sui diritti da spuntare; la casella parte non spuntata e non si ricorda. I canti da salvare sono fissi quando il foglio si apre. | Anticipa un requisito di F7. | Proprietario, 21/11/2026 |

## Decisioni di interfaccia

Fonte delle domande: [design/UX-IMPORT-ROUND-9.md](design/UX-IMPORT-ROUND-9.md), sezione 12.

| # | Domanda | Risposta | Effetto |
|---|---|---|---|
| U-1 | Import del libretto intero in v1? | Sì, per tutti, in modo definitivo. | Il libretto intero resta in v1. F7 non lo decide più. |
| U-2 | Una dichiarazione per salvataggio o per canto? | Confermata: una per salvataggio, con il numero dei canti nel titolo. Da far vedere al legale (D11). | Il legale deve vedere il testo. |
| U-3 | Testo della dichiarazione | Confermato il testo proposto, finché il legale non risponde. | Il testo va rivisto dopo il parere di D11. |
| U-4 | Canti selezionati all'inizio | Confermata: tutti. | La dichiarazione resta il controllo. |
| U-5 | Annullare la lettura scarta tutto? | Confermata: sì. | Nessun elenco parziale. |
| U-6 | Cronometro per il Gate 0 | Confermata: spento per predefinito, da togliere dopo il Gate 0. | Il cronometro va tolto con un round dopo il Gate 0. |
| U-7 | Scheda "Importa" nella barra fino a F6 | **Non confermata.** L'ingresso è spostato ora in Canti → + → Da PDF; la scheda "Importa" esce dalla barra. | F6 non deve più spostare l'ingresso. Le guide di prova sono aggiornate. |
| U-8 | Canti importati con stato dei diritti "Sconosciuto" | Confermata, fino a F7. | Con DEC 37 i canti possono perdersi in F6. |
| Tema | Pagina Importa con la palette Leggio fino a F4 | Accettata fino a F4. | Il criterio A di F3 cita il tema Palco scuro: vedi [CHIUSURA-F3.md](verifiche/CHIUSURA-F3.md). |

## Effetti nel round 10

- **Ingresso dell'import:** da Canti → + → Da PDF. La scheda "Importa" non c'è più. La scheda dei canti si chiama "Canti" e i suoi testi sono in italiano.
- **Uscita dalla schermata di import:** uscendo durante una lettura, la lettura si annulla e lo schermo si rilascia. Uscendo con canti letti e non salvati, l'app chiede conferma. Un nuovo import attende una lettura ancora in corso.
- **ML Kit su Android (R9-21):** l'app non invia più le metriche d'uso a Google. Il controllo di CI conferma che il traffico non c'è e che l'OCR funziona con il modello incluso. Vedi [verifiche/V-07-mlkit.md](verifiche/V-07-mlkit.md).
- **Libretto 14:** i piè di pagina di sezione non entrano più nei canti (112 righe passate a 0). Il libretto dà 152 canti contro 152 voci dell'indice.
- **Altri libretti:** invariati. 13: 73 / 73; 15: 353 / 344; 16: 139 / 137; 17: 66 / 61; 18: 83 / 77.
- **Intestazioni di sezione:** le intestazioni elencate nell'indice non diventano più titoli né testo dei canti. Un canto senza titolo sotto un'intestazione di sezione resta un canto, intitolato dalla prima riga.

## Punti rimasti aperti

- **F3:** gli esiti delle prove su dispositivo non sono ancora disponibili. F3 resta "in corso" fino a quando il proprietario li riporta.
- **Libretto 14, piè di pagina:** una sezione di 1-2 pagine conserva ancora il suo piè di pagina. Nella lista di revisione non c'è un segno per i titoli dubbi (F7).
- **Copia in memoria (R10-06, parziale):** resta una copia intera del PDF in memoria quando il selettore di Android non dà la lunghezza del file.
- **Lavoro scaduto su Android (R9-10, R9-09):** le pagine scansionate restano nel processo dell'app fino a F7. Da verificare sul tablet con un PDF molto grande.
- **Palette:** la pagina Importa resta con i colori di Leggio fino a F4. La decisione di ricolorare le altre pagine resta del proprietario.
- **Rilievi della review del round 10:** lo stato di ogni rilievo (R10-01 a R10-12) va confermato nella review successiva.
- **Primo tocco su Android:** può spostare lo scorrimento di pochi pixel. Da verificare con la prova T2.
