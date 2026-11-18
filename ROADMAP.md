# ROADMAP

Tabella delle fasi tratta da [docs/FASI.md](docs/FASI.md), che resta la fonte del perimetro. Lo stato dei lavori eseguiti è in [PROGRESS.md](PROGRESS.md). Le decisioni del proprietario sono in [docs/DECISIONI-ROUND-1.md](docs/DECISIONI-ROUND-1.md), [docs/DECISIONI-ROUND-4.md](docs/DECISIONI-ROUND-4.md) e [docs/DECISIONI-ROUND-5.md](docs/DECISIONI-ROUND-5.md) e [docs/DECISIONI-ROUND-6.md](docs/DECISIONI-ROUND-6.md).

Stati: **completata** · **in corso** · **da iniziare** · **condizionata** (si apre solo se scatta un gate).

Windows fa parte della prima versione insieme ad Android (non è una fase condizionata); iOS e Mac Catalyst sono fuori dai target e tornano con F16. Il backend è fuori dalla soluzione principale e dalla CI. Si lavora su ramo con pull request e si unisce solo con la CI verde.

| ID | Titolo | Stato | Dipende da | Bloccata da |
|---|---|---|---|---|
| F0 | Ricognizione: valutazione del codice, proposte UX e UI, piano commerciale, collegamento a GitHub | **completata** | — | — |
| G0 | Validazione commerciale e d'uso (Gate 0, lavoro del proprietario, in parallelo) | **in corso** (avviato il 18/11/2026) | F0 | Avvio entro il 16/11/2026; copione delle interviste aggiornato a tre modalità, pubblicità e Pro |
| F1 | Risanamento del repository e CI verde (build Android e Windows) | **in corso** (prima PR unita l'11/11/2026) | F0 | Protezione del branch e minuti Actions sul piano GitHub (V-09); workload della macchina locale (proprietario) |
| F2 | Nucleo musicale: ChordPro, trasposizione, notazione legata alla lingua, "incolla" | **completata** (round 5: notazione collegata alla schermata del canto, rendering corretto nell'app, punti aperti di "incolla") | F1 (job core) | Nulla |
| F3 | Prove tecniche del Gate 0: leggio su tablet Android e su Windows, import da PDF | **in corso** (round 6: divisione dei libretti in canti, conversione canto per canto in ChordPro, glifi TeX per diesis e bemolle, lettura dei PDF in un processo separato, prima prova OCR su Windows, leggio che va a capo sulla larghezza reale; restano pedale e schermo acceso, OCR su Android, prove su dispositivo) | F1 (job Android e Windows), F2 | Tablet economico, pedale Bluetooth, PC Windows di prova; 10 libretti PDF reali |
| F4 | Sistema di token, cinque temi e componenti | da iniziare | F1 | Installazione della taste-skill (proprietario) |
| F5 | Prove visive automatiche (confronto di schermate) in CI | da iniziare | F1, F4 | Cattura su emulatore Android e su Windows nei runner (V-09) |
| F6 | Fondamenta: persistenza, impostazioni sul dispositivo, navigazione a tre modalità, libreria | da iniziare | F1, F2; F4 per la UI | Persistenza: nulla. UI: i due temi predefiniti di F4 |
| F7 | Leggio e import su Android e Windows (traguardo T1) | da iniziare | F3, F6 | Esito della prova import (G0) |
| F8 | Scalette, repertori, Palco in sequenza, condivisione, backup ed export | da iniziare | F6, F7 | Nulla oltre le dipendenze |
| F9 | Calendario liturgico: motore e riferimenti delle letture | da iniziare | F1, F2 (formato dei riferimenti biblici) | Calendari ufficiali di riferimento per i test (proprietario); V-11 |
| F10 | Modalità Liturgia: celebrazioni, slot, suggerimenti locali, storico | da iniziare | F8, F9 | Nulla oltre le dipendenze |
| F11 | Pubblicità e consenso | da iniziare | F6, F13 (identificativo dell'app) | Account AdMob; V-01, V-02, V-06, V-07 |
| F12 | Acquisti in app: Pro e donazioni | da iniziare | F6, F13 | Profilo pagamenti Play, account Partner Center; V-03, V-04, V-05, V-07 |
| F13 | Canali di rilascio: Google Play; Windows su GitHub, poi Microsoft Store | da iniziare | F1 (in parallelo a F7-F10) | D10, account Play e Partner Center, segreti; V-04, V-08, V-10 |
| F14 | Anteprima ai cori e pilota (Gate Anteprima e Gate Pilota) | da iniziare | Anteprima: F7, F8, F13. Pilota: anche F10, F11, F12 | Cori e ≥ 12 tester (G0); D11 parere legale; V-05; dominio ed email |
| F15 | Lancio: Google Play, Microsoft Store, sito con pagina di donazione | da iniziare | F14 | Gate Pilota; D12; posizione fiscale definita; V-03 |
| F16 | iPad / iOS | condizionata | F15 | Gate 1 e censimento (iPad > 40%); account Apple e runner macOS |
| F17 | Piano Coro e licenza diocesana (server della sola scaletta) | condizionata | F15 | Gate 2, D13, D14, secondo parere legale |
| F18 | Nuove funzioni a pagamento | condizionata | F17 | D15 e richieste dei cori |
| F19 | Estensioni di contenuto (rito ambrosiano, note per pianoforte, allegati, pianificatore a settimane, indice pubblico senza testi) | condizionata | F15 | Gate 1 e richieste esplicite |
