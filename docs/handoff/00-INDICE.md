# Indice della cartella handoff

Scopo: trovare il file giusto e sapere cosa fare adesso.

Data di verifica: 20/11/2026. Stato verificato con GitHub:
- La pull request #8 risulta già unita (stato "MERGED").
- Il repository `F1-Solution/AppCanzoniere` risulta **pubblico**. I documenti di piano dicono "privato": il dato va confermato (vedi `05`).
- Il branch `main` **non ha protezione**. GitHub permette quindi di unire anche con controlli rossi.
- La build Android in locale non è stata provata in questa verifica.

## Tabella dei file

| N. | File | Cosa fai | Quando | Cosa blocca | Stato |
|---|---|---|---|---|---|
| 01 | [01-DECISIONI-DA-CONFERMARE.md](01-DECISIONI-DA-CONFERMARE.md) | Confermi o cambi DEC 32-35. Prendi D10-D13 più avanti. | Adesso (DEC 32-35). D10 entro il 28/02/2027. | Lavoro dei round 9 e seguenti. D10-D13 bloccano F11-F15. | da fare |
| 02 | [PROVE-SU-DISPOSITIVO.md](../verifiche/PROVE-SU-DISPOSITIVO.md) | Esegui le prove su tablet (T1-T7), PC (W1-W7) e import (B1). | Adesso. | Chiusura della fase F3 (traccia A entro il 24/01/2027). | da fare |
| 03 | [03-REVISIONE-E-UNIONE-PR.md](03-REVISIONE-E-UNIONE-PR.md) | Rivedi e unisci una pull request su GitHub. | Ogni volta che una PR di round è pronta. | Unione di ogni PR. Regola: solo con CI verde. | fatto per la PR #8; ripeti per le PR future |
| 04 | [04-GATE-0.md](04-GATE-0.md) | Fai le interviste e la validazione commerciale. | Adesso. Il Gate 0 scade il 31/01/2027. | Decisione "procedo" o "mi fermo" (Gate 0). | da fare |
| 05 | [05-GITHUB-E-MACCHINA-LOCALE.md](05-GITHUB-E-MACCHINA-LOCALE.md) | Proteggi `main`, controlla i minuti Actions, ripara i workload MAUI. | Adesso. | Chiusura di F1. Costi CI per F5 e F13. | da fare |
| 06 | [06-ACQUISTI-E-MATERIALI.md](06-ACQUISTI-E-MATERIALI.md) | Compra tablet e pedale. Procura libretti, canti e calendari. | Tablet e pedale adesso. Il resto secondo la fase. | Prove di F3, F6, F8, F9. | da fare |
| 07 | [07-VERIFICHE-ESTERNE-E-PROFESSIONISTI.md](07-VERIFICHE-ESTERNE-E-PROFESSIONISTI.md) | Verifiche V-01 a V-11, commercialista, legale, account degli store. | Prima delle fasi future (F11-F15). | F11, F12, F13, F14, F15. | da fare |
| 08 | [08-COMPITI-DELLE-FASI-FUTURE.md](08-COMPITI-DELLE-FASI-FUTURE.md) | Compiti tuoi per ogni fase da F4 a F15. | Prima di ogni fase. | La fase indicata nel file. | da fare |

Nota: i file 04, 07 e 08 sono scritti da un altro agente con questi nomi. Il file 02 non esiste in questa cartella: la riga 02 porta alla prova già scritta in `verifiche`.

## Ordine consigliato

1. **Adesso, in quest'ordine:** `01`, `03`, `02`, `05`.
2. **Adesso, in parallelo:** `04` e `06`.
3. **Prima delle fasi future:** `07` e `08`.

Il file `03` è già stato eseguito per la PR #8. Serve di nuovo per le PR dei round futuri.

## Come riportare gli esiti

Copia la riga modello nel prompt successivo. Sostituisci i valori tra parentesi. Se una voce non esiste, scrivi "non fatto".

```
Esito prove: T1 (OK), T2 (KO, nota: passo 3, la pagina non gira), T3 (OK), T4 (OK), T5 (OK), T6 (OK), T7 (non fatto), W1 (OK), W2 (OK), W3 (OK), W4 (OK), W5 (OK), W6 (OK), W7 (OK), B1 (PDF con testo: 62%; scansioni: 41%); DEC 32-35: (confermate oppure cambia la voce); D10: (dati oppure non deciso); GitHub: protezione di main (sì o no), controlli obbligatori (numero); Actions: piano (nome), minuti usati (numero su totale); build Android locale: (OK oppure messaggio d'errore).
```

Per una prova KO, scrivi il numero del passo e cosa è successo. Per B1, scrivi le due percentuali.

## Parole usate in questa cartella

| Parola | Significato |
|---|---|
| PR | Pull request. Richiesta di unire modifiche di un ramo a `main` su GitHub. |
| CI | Controlli automatici su GitHub. Si avviano a ogni PR. |
| Controllo | Un job della CI. Può essere verde (riuscito), rosso (fallito) o giallo (in corso). |
| Round | Un lavoro con un obiettivo, un ramo e una PR. |
| Fase | Una parte del piano, da F0 a F19. Vedi `docs/FASI.md`. |
| Gate | Verifica con data che decide se il progetto continua. |
| DEC | Decisione del proprietario già presa. Vedi `docs/DECISIONI-ROUND-*.md`. |
| D10-D13 | Decisioni del proprietario ancora aperte. |
| V-01 a V-11 | Verifiche esterne da fare fuori dal codice. Vedi `docs/FASI.md`, sezione 6. |
| Prova | Verifica su un dispositivo reale, eseguita a mano. |
| Runner | Computer di GitHub che esegue la CI. |
| Canto di prova | Canto della versione di prova dell'app. Compare solo in quella versione. |
