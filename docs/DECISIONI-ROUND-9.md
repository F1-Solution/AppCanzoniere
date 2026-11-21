# Decisioni del proprietario — round 9 (20 novembre 2026)

Seguono le decisioni DEC 32-35 di [DECISIONI-ROUND-8.md](DECISIONI-ROUND-8.md). Aggiornato il 21/11/2026 con la revisione del ramo e con le decisioni di interfaccia U-1..U-8.

| # | Decisione | Effetto | Chi |
|---|---|---|---|
| DEC 32-35 | Confermate dal proprietario il 20/11/2026. | — | Proprietario |
| D12 (parte) | Il repository `F1-Solution/AppCanzoniere` resta pubblico. | La sede delle release Windows si sceglie in F13. | Proprietario |
| DEC 36 | Su Android i PDF si leggono nel processo dell'app, con i limiti contro i file costruiti ad arte; il processo separato resta in F7. La revisione estende la decisione: le pagine scansionate sono disegnate dal renderer di sistema nel processo dell'app, su Android e su Windows, senza i limiti del lettore separato. Lo spostamento nel processo separato è previsto in F7. | Un PDF malevolo può chiudere l'app fino a F7. Su Windows una pagina solo immagine viene elaborata nel processo dell'app. Su Android un lavoro che scade dopo 2 minuti continua in background finché non finisce; un nuovo import attende. | Confermata il 21/11/2026 ([round 10](DECISIONI-ROUND-10.md)) |
| DEC 37 | La schermata di import sostituisce la vecchia pagina OCR simulata e salva i canti nella libreria attuale. | I canti importati potrebbero andare persi quando F6 rifà la persistenza. | Confermata il 21/11/2026 ([round 10](DECISIONI-ROUND-10.md)) |
| DEC 38 | Prima di salvare un canto importato, l'utente spunta una dichiarazione non preselezionata sui diritti del canto. La casella parte sempre non spuntata e non viene ricordata. Ogni salvataggio ha la sua dichiarazione. I canti da salvare sono fissi quando il foglio si apre. | Anticipa un requisito di F7. | Confermata il 21/11/2026 ([round 10](DECISIONI-ROUND-10.md)) |

## Decisioni di interfaccia (da confermare)

Fonte: [design/UX-IMPORT-ROUND-9.md](design/UX-IMPORT-ROUND-9.md), sezione 12. La colonna "Proposta" è la raccomandazione del documento.

| # | Domanda | Proposta | Stato |
|---|---|---|---|
| U-1 | Import del libretto intero in v1? | Sì in questo round, perché serve alla traccia B del Gate 0. Decidere in F7 se resta per tutti. | Confermata il 21/11/2026, in modo definitivo ([round 10](DECISIONI-ROUND-10.md)) |
| U-2 | Una dichiarazione per ogni salvataggio, anche di 150 canti insieme, oppure una per canto? | Una per salvataggio, con il numero dei canti nel titolo. Da far vedere al legale (D11). | Confermata il 21/11/2026 ([round 10](DECISIONI-ROUND-10.md)) |
| U-3 | Testo della dichiarazione. | Usare il testo proposto finché il legale non risponde. | Confermata il 21/11/2026 ([round 10](DECISIONI-ROUND-10.md)) |
| U-4 | Nell'elenco, canti selezionati all'inizio: tutti o nessuno? | Tutti, così chi vuole tutto il libretto non tocca 150 caselle. La dichiarazione resta il punto di controllo. | Confermata il 21/11/2026 ([round 10](DECISIONI-ROUND-10.md)) |
| U-5 | Annullare la lettura scarta tutto? | Sì: niente elenchi parziali. | Confermata il 21/11/2026 ([round 10](DECISIONI-ROUND-10.md)) |
| U-6 | Cronometro per il Gate 0 nella schermata? | Sì, spento per impostazione predefinita, da togliere dopo il Gate 0. | Confermata il 21/11/2026 ([round 10](DECISIONI-ROUND-10.md)) |
| U-7 | La scheda "Importa" resta nella barra attuale fino a F6? | Sì: è temporanea e la navigazione si rifà in F6. | Non confermata il 21/11/2026: l'ingresso è in Canti → + → Da PDF e la scheda "Importa" esce dalla barra ([round 10](DECISIONI-ROUND-10.md)) |
| U-8 | Canti importati salvati con stato dei diritti "Sconosciuto"? | Sì, fino a F7. Con DEC 37 potrebbero perdersi in F6. | Confermata il 21/11/2026 ([round 10](DECISIONI-ROUND-10.md)) |
| Tema | Fino a F4 la pagina Importa usa la palette Leggio, mentre il resto dell'app resta viola. | Accettare la differenza fino a F4. La decisione di ricolorare le altre pagine è del proprietario. | Accettata il 21/11/2026 ([round 10](DECISIONI-ROUND-10.md)) |

## Correzione del round 8

Il round 8 indicava 2 titoli persi nel libretto 14, per effetto di DEC 35. Erano 6. Il round 9 li ha trovati. Le regole di DEC 35 ora li recuperano quando sono nell'indice, centrati, un po' più grandi, con una riga vuota sopra e non ripetuti nella pagina.

---

## Punti rimasti aperti

- **Libretto 14, conteggio:** trovati 151 canti, nell'indice 152 voci. Prima del round 9 erano 149 contro 169. L'indice ora non conta più intestazioni di sezione e piè di pagina. Round 10: 152 canti contro 152 voci.
- **Libretto 14, piè di pagina:** risolto nel round 10. I piè di pagina di sezione non entrano più nel testo dei canti ([round 10](DECISIONI-ROUND-10.md)).
- **Altri libretti (canti trovati / voci dell'indice):** 13: 73 / 73; 15: 353 / 344; 16: 139 / 137; 17: 66 / 61; 18: 83 / 77. Il conteggio dell'indice non prova che la divisione sia giusta.
- **ML Kit e rete (R9-21):** risolto nel round 10. Il traffico di metriche è stato eliminato e la CI lo verifica ([V-07](verifiche/V-07-mlkit.md)).
- **Android, lavoro scaduto (R9-09):** una lettura scaduta continua in background finché non finisce. Un nuovo import attende. Da verificare sul tablet con un PDF molto grande.
- **Rilievi della revisione (R9-01..R9-24):** le correzioni sono nei commit `98ed658`, `aa5e947`, `37f9fa7`, `ba51250`, `ce765a4` e `c18ab09`. Il file [verifiche/REVIEW-ROUND-9.md](verifiche/REVIEW-ROUND-9.md) non dice, per ogni rilievo, se è chiuso. Lo stato di ogni rilievo va confermato nel round successivo.
- **Palco e Leggio:** i temi definitivi arrivano con F4. Fino ad allora la pagina Importa usa la palette provvisoria.
- **Cronometro (U-6):** serve solo per il Gate 0. Va tolto dopo il Gate 0.
- **Riportati dal round 8, non trattati nel round 9:** accordi in grassetto persi dall'OCR su Windows; scansione 19 illeggibile a 70 dpi; pagine ruotate di 90 gradi senza testo non lette.
- **Primo tocco su Android:** può spostare lo scorrimento di pochi pixel. Da verificare con la prova T2.
- **Test del nucleo:** 3815 dopo le correzioni della review (Domain 5, Application 3193, Infrastructure 617); il file di revisione riporta il conteggio precedente alle correzioni.
