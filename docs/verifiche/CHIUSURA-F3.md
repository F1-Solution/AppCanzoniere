# Chiusura della fase F3 — scheda

Questa scheda dice cosa serve per chiudere la fase F3 del piano. Stato di F3 al 21/11/2026: **in corso**. F3 si chiude solo quando il proprietario riporta gli esiti delle prove su dispositivo. Il codice del round 9 non chiude F3 da solo.

Fonti: [docs/FASI.md](../FASI.md), fase F3 (criteri di accettazione). Prove: [PROVE-SU-DISPOSITIVO.md](PROVE-SU-DISPOSITIVO.md). Istruzioni passo passo: [docs/handoff/09-PROVE-IMPORT-E-CHIUSURA-F3.md](../handoff/09-PROVE-IMPORT-E-CHIUSURA-F3.md).

## Come usare questa scheda

1. Leggi la sezione 1. Sono i criteri che il piano chiede.
2. Esegui le prove della sezione 2, con la guida delle prove.
3. Compila le tabelle della sezione 3. Usa codici, non nomi di libretti, cori o persone.
4. Decidi i punti della sezione 4.
5. Incolla la riga della sezione 5 nel prompt successivo.

ATTENZIONE: non scrivere nel repository i nomi dei libretti, dei cori o delle persone. Tieni la scheda compilata fuori dal repository.

## 1. Criteri di accettazione di F3

| Traccia | Scadenza | Criterio (da FASI.md) | Chi verifica |
|---|---|---|---|
| A | 24/01/2027 | Un canto di due pagine nel tema Palco scuro scorre fluido e gira pagina con il pedale Bluetooth sul tablet Android, anche con i tasti del volume, e su un PC Windows, a schermo sempre acceso. | Proprietario, con le prove |
| A | 24/01/2027 | Nessun fotogramma perso visibile, con video su entrambe le piattaforme. Il pedale funziona senza configurazioni dopo l'accoppiamento. | Proprietario, con i video |
| A | 24/01/2027 | Test unitari e test di disegno senza errori in CI. | CI (agente lo controlla) |
| B | 17/01/2027 | Percentuale di canti importati da 10 libretti PDF reali con al massimo 2 minuti di correzione, separata tra PDF con testo e scansioni. | Proprietario, con i tempi |
| B | 17/01/2027 | Soglie: almeno 70% con "sì" apre l'OCR nella prima versione; dal 50% al 70% solo PDF con testo e "incolla"; sotto il 50% import fuori dalla prima versione. Valutate per piattaforma, se l'OCR di Windows dà risultati diversi. | Agente e proprietario, insieme |
| Tutte | — | APK e pacchetto Windows di prova prodotti dalla CI. | CI (agente lo controlla) |

Nota: il criterio A cita il tema Palco scuro. Il tema arriva con F4. Vedi la sezione 4, punto 1.

## 2. Prove richieste

### Traccia A — tablet Android e PC Windows

| Prova | Obbligatoria per F3 |
|---|---|
| T1, T2, T3, T4 (pedale e tasti del volume sul tablet) | Sì |
| T5 (schermo acceso sul tablet) | Sì |
| T6 (video dello scorrimento sul tablet) | Sì |
| T7 (prova in chiesa poco illuminata) | No. È richiesta dal Gate 0, non da F3. |
| W1, W2, W3 (pedale e tasti sul PC) | Sì |
| W4 (campo di testo) | Sì |
| W5 (schermo acceso sul PC) | Sì |
| W6 (video dello scorrimento sul PC) | Sì |
| W7 (avvio dal pacchetto) | Sì |

### Traccia B — import da 10 libretti

| Prova | Obbligatoria per F3 |
|---|---|
| I-T1, I-T3, I-T4 (scelta, libretto, scansione sul tablet) | Sì, prima della misura |
| I-T10 (tempo sul tablet) | Sì, per la misura su Android |
| I-W1, I-W2, I-W3 (scelta, libretto, scansione su PC) | Sì, prima della misura |
| I-W6 (tempo su PC) | Sì, per la misura su Windows |
| B1 (con il report del banco di prova) | No. È un controllo facoltativo. |

Regola per la misura: 10 libretti reali, 5 canti per libretto. Conta ogni canto, con il tempo in minuti e il "sì" o il "no".

## 3. Tabelle da compilare

### 3.1 Traccia A

| Prova | Esito (OK / KO) | Note |
|---|---|---|
| Pedale e tasti del volume sul tablet (T1-T4) | | |
| Schermo acceso sul tablet (T5) | | |
| Video dello scorrimento sul tablet (T6) | | |
| Pedale e tasti sul PC (W1-W4) | | |
| Schermo acceso sul PC (W5) | | |
| Video dello scorrimento sul PC (W6) | | |
| Avvio dal pacchetto (W7) | | |

### 3.2 Traccia B — libretti

Una riga per ogni libretto. Scrivi il codice del libretto (per esempio L01), non il nome.

| Codice | Tipo (testo / scansione) | Piattaforma (Android / Windows) | Canti provati | Canti con "sì" | Percentuale |
|---|---|---|---|---|---|
| L01 | | | | | |
| L02 | | | | | |
| L03 | | | | | |
| L04 | | | | | |
| L05 | | | | | |
| L06 | | | | | |
| L07 | | | | | |
| L08 | | | | | |
| L09 | | | | | |
| L10 | | | | | |

Percentuale = canti con "sì" diviso per canti provati, per 100.

### 3.3 Traccia B — riepilogo

Compila una tabella per piattaforma. Se la piattaforma è una sola, compila una tabella sola.

| Piattaforma | Tipo | Canti provati | Canti con "sì" | Percentuale | Soglia raggiunta (≥ 70 / 50-70 / < 50) |
|---|---|---|---|---|---|
| | PDF con testo | | | | |
| | Scansioni | | | | |
| | Totale | | | | |

## 4. Punti da decidere prima della chiusura

Ogni punto ha una riga. Scrivi "confermato" oppure "cambiato, [la tua scelta]".

| Punto | Decisione | Dove è spiegato |
|---|---|---|
| 1. Tema del criterio A: chiudere F3 con la palette provvisoria di Leggio, oppure aspettare F4 (tema Palco scuro) | | [DECISIONI-ROUND-9.md](../DECISIONI-ROUND-9.md), riga "Tema" |
| 2. DEC 36: lettura dei PDF nel processo dell'app | | [DECISIONI-ROUND-9.md](../DECISIONI-ROUND-9.md) |
| 3. DEC 37: canti importati salvati nella libreria attuale | | [DECISIONI-ROUND-9.md](../DECISIONI-ROUND-9.md) |
| 4. DEC 38: dichiarazione da spuntare prima di ogni salvataggio | | [DECISIONI-ROUND-9.md](../DECISIONI-ROUND-9.md) |
| 5. U-1 a U-8 (decisioni di interfaccia) | | [DECISIONI-ROUND-9.md](../DECISIONI-ROUND-9.md) |
| 6. Esito della misura B: OCR in v1, solo PDF con testo e "incolla", oppure import fuori dalla v1 | | Sezione 3.3 di questa scheda |

## 5. Riga da incollare per chiudere F3

Compila la riga, poi incollala nel prompt successivo. Sostituisci i valori tra parentesi.

`Chiudi F3: traccia A (T1-T6, W1-W7 OK; tema: [Leggio provvisorio / Palco scuro]); import (I-T1, I-T3, I-T4, I-T10, I-W1, I-W2, I-W3, I-W6 OK); traccia B: PDF con testo [__]%, scansioni [__]%, piattaforma [Android / Windows / entrambe]; DEC 36-38 e U-1..U-8: [confermate / cambiate, la scelta]; tema: [confermato / cambiato].`

Se una prova non è stata fatta, scrivi "non fatto" al suo posto.
