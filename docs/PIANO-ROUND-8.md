# Piano del round 8 (19 novembre 2026)

Ramo `f3/round-8`, una pull request, test prima del codice, review finale Opus xhigh. DEC 28-31 confermate dal proprietario; PR #7 unita.

| Onda | Lavoro | Agenti |
|---|---|---|
| R8-0 | Ramo, piano, tabella delle fasi | Orchestratore |
| R8-1 | B1: libretti 13 e 14 contro l'indice, titoli della 07, scheda 02. D1: OCR su Android con ML Kit (verifica V-07). C1: lettore PDF nel pacchetto Windows e prove automatiche dell'app su Windows | 3 Sonnet |
| R8-2 | B2: accordi OCR allineati alle parole. E1: prove automatiche su emulatore Android in CI | 2 Sonnet |
| R8-3 | Review del ramo e correzioni | Opus xhigh, Sonnet |
| R8-4 | Documenti, guida passo passo alle prove su dispositivo, PR, CI | Haiku |

## Scelte dell'orchestratore, da confermare

- **DEC 32** — Su Android l'OCR usa il modello ML Kit incluso nell'app (funziona senza rete e senza scaricare nulla), solo alfabeto latino.
- **DEC 33** — Le prove che non richiedono mani, pedale o occhi si fanno in automatico: in CI su emulatore Android e sul runner Windows, e sul PC di sviluppo. Al proprietario restano solo le prove su hardware reale.
