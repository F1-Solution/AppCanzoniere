# Piano del round 9 (20 novembre 2026)

Ramo `f3/round-9`, una pull request, test prima del codice, review finale Opus xhigh. DEC 32-35 confermate dal proprietario; PR #9 unita; il repository resta pubblico.

| Onda | Lavoro | Agenti |
|---|---|---|
| R9-0 | Ramo, piano, decisioni, tabella delle fasi | Orchestratore |
| R9-1 | U1: proposta UX della schermata di import. B1: conteggio dell'indice del libretto 14. C1: servizio di import comune, OCR di sistema e disegno delle pagine su Windows, lettura dei PDF su Android | UX Architect (Opus), 2 Sonnet |
| R9-2 | U2: proposta UI. D1: schermata di import nell'app su Android e Windows. E1: prove automatiche dell'import su emulatore e su Windows | UI Designer, 2 Sonnet |
| R9-3 | Review del ramo e correzioni | Opus xhigh, Sonnet |
| R9-4 | Documenti, prove su dispositivo dell'import, scheda di chiusura di F3, PR, CI | Haiku |

F3 si chiude solo quando il proprietario riporta gli esiti delle prove su dispositivo; questo round prepara la scheda di chiusura.

## Scelte dell'orchestratore, da confermare

- **DEC 36** — Su Android i PDF si leggono nel processo dell'app con i limiti già usati contro i file costruiti ad arte; il processo separato resta in F7 (DEC 26).
- **DEC 37** — La schermata di import sostituisce la vecchia pagina OCR simulata e salva i canti nella libreria attuale; la persistenza si rifà in F6 e potrebbe non conservarli.
- **DEC 38** — Prima di salvare, l'utente spunta una dichiarazione non preselezionata sui diritti del canto (anticipo di F7, piano commerciale §3.2).
