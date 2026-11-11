# ROADMAP

Tabella delle fasi tratta da [docs/FASI.md](docs/FASI.md), che resta la fonte del perimetro. Lo stato dei lavori eseguiti è in [PROGRESS.md](PROGRESS.md).

Stati: **completata** · **in corso** · **da iniziare** · **condizionata** (si apre solo se scatta un gate).

Decisioni del proprietario successive a FASI.md, che prevalgono su di esso:
- la prima versione esce su Android e Windows insieme: la CI compila il progetto MAUI per entrambi; iOS e Mac Catalyst sono fuori dai target (F10 li riaggiunge se serve);
- la prima versione non ha backend, account né catalogo pubblico: il backend è fuori dalla soluzione principale e dalla CI ma il suo codice non è cancellato;
- si lavora su ramo con pull request e si unisce solo con la CI verde.

| ID | Titolo | Stato | Dipende da | Bloccata da |
|---|---|---|---|---|
| F0 | Ricognizione: valutazione del codice, proposte UX e UI, piano commerciale, collegamento del repo a GitHub | **completata** | — | — |
| G0 | Validazione commerciale (Gate 0, lavoro del proprietario, in parallelo) | da iniziare | F0 | D1 provvisoria, D3 (avvio), D4 (ore dichiarate) |
| F1 | Risanamento del repository e CI verde | **in corso** (PR aperta, in attesa di CI verde e merge) | F0 | Nulla: può partire subito. Per la build Android **in locale** serve riparare i workload della macchina (azione del proprietario, da amministratore) |
| F2 | Nucleo musicale: ChordPro, trasposizione, notazione, "incolla" | da iniziare | F1 (job "core" della CI) | Nulla. D8 fissa solo i valori predefiniti (Do-Re-Mi, "Lam"/"La-"), modificabili dopo |
| F3 | Prove tecniche del Gate 0: leggio "Palco" su tablet e import da PDF | da iniziare | F1 (job Android), F2 (modello di impaginazione) | Tablet Android economico e pedale Bluetooth; 10 libretti PDF reali (proprietario) |
| F4 | Fondamenta dell'app: persistenza, navigazione, libreria | da iniziare | F1, F2 | Persistenza: nulla (anticipabile prima del Gate 0). Navigazione e UI: esito G0 "procedo", D2, D5, D8 |
| F5 | Leggio e import: primo flusso completo (traguardo T1) | da iniziare | F3, F4 | Esito G0 (prova import, D2), D5, D8 |
| F6 | Scalette della Messa, condivisione, backup ed export | da iniziare | F4, F5 | D5, D7, decisioni UX 3-5 |
| F7 | Canale di rilascio Android: firma, versioni, Play test interno e chiuso | da iniziare | F1 (si sviluppa in parallelo a F5-F6) | D10 (nome, `ApplicationId`, account Play), segreti, account Play verificato |
| F8 | Pilota e prontezza della v1 (Gate Pilota) | da iniziare | F5, F6, F7 | Cori pilota e ≥ 12 tester (G0), D11 (parere legale), dominio ed email per privacy e segnalazioni |
| F9 | Lancio in produzione su Google Play | da iniziare | F8 | Gate Pilota (30/06/2027), D6, D12 |
| F10 | Seconda piattaforma (iPad o Windows) | condizionata | F9 | Gate 1 + D9 (iPad > 40%) oppure ≥ 3 cori che chiedono Windows; per iPad account Apple e runner macOS |
| F11 | Piano Coro e licenza diocesana (server della sola scaletta) | condizionata | F9 | Gate 2, D13, D14, secondo parere legale |
| F12 | Strumenti Pro | condizionata | F11 | D15 e condizioni di [PC §4.2] |
| F13 | Estensioni di contenuto (inglese, accordi per pianoforte, filtri estesi, indice liturgico, calendario) | condizionata | F9 | Gate 1; richieste esplicite dei cori |
