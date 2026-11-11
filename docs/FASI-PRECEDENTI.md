# AppCanzoniere - Sintesi delle fasi del piano precedente

**Scopo:** fotografia di come il progetto era pianificato e di cosa i documenti dichiaravano come completato, senza verifiche sul codice e senza valutazioni.
**Data di redazione:** 2026-11-10.
**Ultimi aggiornamenti dei documenti:** plan.md 2026-01-31; README, SETUP_INSTRUCTIONS, TEST_AND_COMPLETE, KNOWN_ISSUES, FUTURE_IMPLEMENTATIONS 2026-02-01.

**Legenda sigle delle fonti:**
- [PLAN] = `docs/archivio/plan.md`
- [TEST] = `docs/archivio/TEST_AND_COMPLETE.md`
- [FUT] = `docs/archivio/FUTURE_IMPLEMENTATIONS.md`
- [KNOWN] = `docs/archivio/KNOWN_ISSUES.md`
- [MEM] = `docs/archivio/copilot_memory.md`
- [README] = `README.md`
- [SETUP] = `docs/archivio/SETUP_INSTRUCTIONS.md`

---

## 1. Fasi del piano

| N. | Titolo | Obiettivo | Stato dichiarato (fonte) | Principali cose consegnate |
|----|--------|-----------|--------------------------|----------------------------|
| 1 | Setup e architettura base | Soluzione MAUI .NET 10, struttura progetti, SQLite con EF Core, MVVM, DI | Completa ([PLAN] 5/5 voci spuntate; [README] "Fasi 1-13") | Soluzione MAUI, progetti Domain/Application/Infrastructure/MAUI/Backend, SQLite + EF Core, CommunityToolkit.Mvvm, Dependency Injection |
| 2 | Modello dati e database | Entità di dominio, schema SQLite, repository, versioning per sync, seed | Completa ([PLAN] 5/5) | Entità Song, Album, Tag, ecc.; repository pattern; campo di versioning; seed data (11 momenti liturgici, 15 tag) [README] |
| 3 | UI base e navigazione | Shell navigation e pagine base | Completa ([PLAN] 5/5) | Shell con TabBar; HomePage, SongDetailPage, SearchPage, SettingsPage |
| 4 | Funzionalità core canzoni | CRUD, parser ChordPro, trasposizione, altri formati, tablature, metadati | Parziale ([PLAN]: 4 voci aperte - parser MusicXML, parser/serializer JSON custom, tablature chitarra, metadati YouTube/MP3/PDF). [README] la elenca come completata solo per ChordPro e trasposizione | CRUD canzoni; parser ChordPro; trasposizione tonalità |
| 5 | Album e raccolte | CRUD album, relazione molti-a-molti, ordinamento, UI | Completa ([PLAN] 4/4) | CRUD album, associazione canzoni-album con ordine, UI gestione album |
| 6 | Tag e metadati specializzati | Tag generici, tag liturgici, riferimenti biblici, filtri | Completa ([PLAN] 4/4) | Tagging generico, tag liturgici, riferimenti biblici, filtri avanzati per tag |
| 7 | Servizi esterni - AI | Interfaccia IChordSuggestionService, client Perplexity, mock, UI, disclaimer | Completa ([PLAN] 5/5) | Client Perplexity, mock per demo, UI richiesta accordi, disclaimer copyright |
| 8 | Servizi esterni - OCR | Interfaccia IOcrService, Tesseract, Azure Computer Vision, mock, import immagini/PDF | Completa ([PLAN] 5/5; Tesseract descritto come "placeholder locale") | Import da immagini e PDF con file picker; implementazione Azure e Tesseract |
| 9 | Servizi esterni - audio recognition | Interfaccia IAudioRecognitionService, client ACRCloud, mock, firma HMAC-SHA1, parsing, UI, controllo duplicati | Completa per [PLAN] (9/9). Incongruente: [README] la mette in "In Roadmap" (vedi sez. 5) | Client ACRCloud, firma HMAC-SHA1, parsing risultati, UI con file picker, check duplicati. Registrazione da microfono solo "placeholder" [PLAN] |
| 10 | Sincronizzazione cloud multi-provider | Interfaccia provider astratta, LocalStorage, orchestratore, conflict resolution, export/import, UI | Parziale ([PLAN]: OneDrive, Google Drive, Dropbox ancora "placeholder" con voci non spuntate). [README] la conta come completata | LocalStorageProvider, CloudSyncService, conflict resolution KeepLocal/KeepRemote/KeepBoth, export/import DB, UI selezione provider e indicatori sync |
| 11 | Database pubblico e backend | ASP.NET Core API, EF migrations, JWT + Identity, ruoli, controller Account/Songs, Swagger, CORS | Completa ([PLAN] ✅, [README] ✅, [TEST] "Fasi 1-13: Complete") | API REST con JWT e Identity, ruoli User/Admin, endpoint CRUD canzoni, flag IsPublic, Swagger, CORS per il client MAUI |
| 12 | Testing backend e refactoring | Progetto xUnit backend, test di controller, pulizia | Completa ([PLAN] "COMPLETATA") | 14 test backend (6 AccountController, 8 SongsController); 77 test totali passati (Domain 6, Application 46, Infrastructure 11, Backend 14) |
| 13 | Client MAUI per backend API | HttpClient, DTO, gestione JWT, login e canzoni pubbliche | Completa ([PLAN] "COMPLETED"; [README] ✅; [TEST] ✅) | AuthService, LoginPage/ViewModel, PublicSongsPage/ViewModel, tab "Public Songs", route di navigazione |
| 14 | Testing e refinement | SecureStorage, integration test, configurazione, E2E, performance, coverage | Parziale - 40% ([TEST], [README]). Incongruente: [PLAN] intestazione "IN PROGRESS" ma stato "COMPLETATA (con limitazioni note)" (vedi sez. 5) | ITokenStorage e SecureTokenStorage (Keychain, KeyStore, DPAPI), AuthService migrato, progetto integration test con 13 test scritti (5 Account + 8 Songs) non funzionanti |
| 15 | Modalità offline | Download e gestione offline delle canzoni pubbliche | Parziale - 70% ([PLAN], [TEST], [README], [SETUP]) | SyncStatus (6 stati), entità OfflineSong, IOfflineSongRepository, IDownloadService, DownloadService con progress e limite storage 500 MB. Pendenti: migrazione EF, FileService, UI, media caching, UI conflitti |
| 16 | Funzionalità avanzate | Ricerca avanzata, batch, UI/UX e dark mode, funzioni accordi, raccolte AI | Parziale - 25% ([PLAN], [TEST], [README]) | Infrastruttura di ricerca avanzata: AdvancedSearchCriteria con 20+ filtri, ricerca "fuzzy", ordinamento, paginazione, tempo di esecuzione. UI e resto pendenti |
| 17 | Production deployment | CI/CD, packaging per store, performance, sicurezza, monitoraggio, documentazione | Completa solo per l'infrastruttura ([PLAN] "COMPLETATA (Infrastructure Ready)" con voci manuali aperte; [TEST] "Infrastructure Complete"). Incongruente: [README] riporta "Fase 17 (0%)" in una sezione (vedi sez. 5) | Workflow GitHub Actions `.github/workflows/build-and-test.yml` (build multi-piattaforma, test, artifact); KNOWN_ISSUES.md, FUTURE_IMPLEMENTATIONS.md, aggiornamento documentazione. Aperti: packaging store, performance, hardening sicurezza, monitoraggio |

**Riepilogo stati dichiarati:** complete 1, 2, 3, 5, 6, 7, 8, 9, 11, 12, 13; parziali 4, 10, 14, 15, 16, 17 (per la sola infrastruttura); non iniziate: nessuna fase risulta "non iniziata" in nessun documento.

---

## 2. Decisioni tecniche prese

| Area | Decisione | Fonte |
|------|-----------|-------|
| Piattaforme client | Android, iOS, Windows Desktop, macOS (via Mac Catalyst) | [PLAN], [README] |
| Framework | .NET MAUI su .NET 10; EF Core 10.0.2 (SQLite); CommunityToolkit.Mvvm 8.4.0; System.Net.Http.Json | [README], [PLAN] |
| Target minimi dichiarati | Android API 21+, iOS 11+, Windows 10 1809+, macOS 10.15+ | [PLAN] (sez. "Piattaforme target") |
| Architettura | Clean Architecture a 4 livelli (Domain, Application, Infrastructure, Presentation); dipendenze solo verso l'interno | [PLAN], [MEM] |
| Pattern UI | MVVM con CommunityToolkit.Mvvm | [PLAN], [README] |
| Pattern dati e servizi | Repository pattern; Strategy pattern per servizi esterni intercambiabili (OCR, cloud, AI); DI con `MauiProgram.cs` e `Program.cs` | [PLAN] |
| Database client | SQLite con EF Core; file `appcanzoniere.db` in AppDataDirectory ([PLAN], [README]) oppure `%LOCALAPPDATA%/AppCanzoniere` ([SETUP]) | [PLAN], [README], [SETUP] |
| Migrazioni client | [README] dichiara migrazioni applicate automaticamente all'avvio; [SETUP] indica `dotnet ef database update` manuale | [README], [SETUP] |
| Database backend | SQLite di default (`appcanzoniere_backend.db`); SQL Server in produzione come opzione | [SETUP] |
| Auth backend | JWT + ASP.NET Core Identity; autorizzazione basata su ruoli; utente admin di default seedato (credenziali nel documento sorgente, sezione "Default Admin User") | [PLAN] Fase 11, [SETUP] |
| Storage token client | SecureStorage (iOS e macOS Keychain, Android KeyStore, Windows DPAPI); `ITokenStorage` | [PLAN] Fase 14, [SETUP] sez. 14 |
| Backend API | REST, Swagger/OpenAPI (disabilitato in ambiente Testing), CORS per il client MAUI, ProblemDetails previsti per gli errori | [PLAN], [MEM] |
| Testing | xUnit (3.1.4), FluentAssertions, Moq, EF Core in-memory, WebApplicationFactory per integration test | [README], [SETUP] sez. 12, [MEM] |
| Formato canzoni | ChordPro come formato primario; MusicXML e JSON custom previsti come formati aggiuntivi (MusicXML aggiunto nel changelog v1.1 del piano). Accordi in ChordPro tra parentesi quadre; trasposizione su scala cromatica con alias bemolle (es. Db -> C#) | [PLAN] (schema, changelog, TransposeService) |
| Servizio AI accordi | Perplexity API (chat/completions), mock di default, disclaimer copyright obbligatorio | [PLAN] Fase 7, [SETUP] |
| Modello Perplexity | Nel codice di esempio: `llama-3.1-sonar-large-128k-online`; nella sezione prezzi di [SETUP]: `llama-3.1-sonar-small-128k-online` | [PLAN], [SETUP] |
| OCR | Tesseract locale (descritto come placeholder) e Azure Computer Vision Read API v3.2 (cloud, free tier 5.000 transazioni/mese); mock di default; selezione tramite feature flag `Features:UseAzureOcr` | [PLAN] Fase 8 e DI, [SETUP] |
| Riconoscimento audio | ACRCloud (firma HMAC-SHA1, host `identify-eu-west-1.acrcloud.com`, free tier 2.000 riconoscimenti/mese, mock di default). Il codice di esempio nel piano usa il nome `ShazamAudioRecognitionService` | [PLAN] Fase 9 e architettura, [SETUP], [README] |
| Cloud sync | Interfaccia `ICloudStorageProvider`; LocalStorage implementato; OneDrive via MSAL e Microsoft Graph; Google Drive via Google.Apis.Drive.v3 con OAuth2 PKCE; Dropbox via Dropbox.Api o HTTP diretto (questi tre come placeholder) | [PLAN] Fase 10 e sez. integrazioni, [README] |
| Conflitti di sync | Strategia dichiarata in Fase 10: KeepLocal / KeepRemote / KeepBoth. [KNOWN] dichiara invece "Last-write-wins" senza rilevamento conflitti | [PLAN], [KNOWN] |
| Offline | Entità OfflineSong con BackendSongId come stringa; limite storage 500 MB (costante in DownloadService); percorsi per piattaforma in [SETUP] | [PLAN] Fase 15, [SETUP] sez. 15 |
| Localizzazione | Pianificata con .resx (AppResources.resx default EN e AppResources.it.resx); lingua UI separata dalla lingua delle canzoni (`Song.Language`) | [PLAN] sez. multilanguage, [FUT] #22 |
| Ricerca avanzata | Criteri con 20+ filtri, ricerca full-text, fuzzy via Contains/ToLower (non Levenshtein), ordinamento, skip/take | [PLAN] Fase 16, [KNOWN] #10 |
| Configurazione e segreti | User Secrets in sviluppo; variabili d'ambiente in CI; Azure Key Vault in produzione; file `.env` esclusi da Git | [SETUP] |
| CI/CD | GitHub Actions: build e test su push/PR verso main e develop; artifact Android APK, Windows MSIX, iOS, macOS, backend | [SETUP] sez. 16, [PLAN] Fase 17 |
| Scraping siti di accordi | Feature flag `EnableWebScraping` disattivato di default; non implementato per motivi legali | [PLAN] sicurezza, [KNOWN], [FUT] #37 |
| Copyright | Enum `CopyrightStatus` nel dominio; disclaimer AI; disclaimer di responsabilità upload previsto (non implementato) | [PLAN], [FUT] #28 |
| Licenza e contatti | "Da definire" | [README] |

**Risposte alle domande di chiarimento del proprietario:** nei sette documenti letti non risulta documentata nessuna domanda di chiarimento con relativa risposta. [MEM] prescrive all'agente di porre domande mirate quando mancano decisioni (DB, cache, ecc.), ma non riporta esiti.

---

## 3. Funzionalità rimandate

Priorità come dichiarata in [FUT] (legenda: Critica, Importante, Nice-to-have, Futura). "Richiesta dal prompt" secondo le annotazioni di [FUT]; il prompt iniziale non è stato aperto per questa sintesi.

| N. | Funzionalità | Priorità dichiarata | Richiesta dal prompt iniziale |
|----|--------------|---------------------|-------------------------------|
| 1 | Completamento offline mode (Fase 15) | Critica | No |
| 2 | Correzione integration test (Fase 14) | Critica | No |
| 3 | Gestione configurazione (Fase 14) | Critica | No |
| 4 | UI ricerca avanzata (Fase 16) | Importante | No |
| 5 | Token refresh | Importante | No |
| 6 | Dark mode | Importante | No |
| 7 | UI operazioni batch | Importante | No |
| 8 | Diagrammi accordi e visualizzazioni | Importante | No |
| 9 | Collaborazione in tempo reale | Nice-to-have | No |
| 10 | Ottimizzazioni tablet e desktop | Importante | No |
| 11 | Controlli gestuali | Nice-to-have | No |
| 12 | Temi personalizzati e font | Nice-to-have | No |
| 13 | Raccolte intelligenti con AI | Nice-to-have | **Sì** |
| 14 | Motore di raccomandazione brani | Nice-to-have | No |
| 15 | Generazione testi con AI | Nice-to-have | No |
| 16 | Riproduzione audio con metronomo | Importante | No |
| 17 | Registrazione e modalità pratica | Nice-to-have | No |
| 18 | Supporto MIDI | Nice-to-have | No |
| 19 | Backend API potenziato | Importante | No |
| 20 | Sincronizzazione in tempo reale | Nice-to-have | No |
| 21 | Backup e ripristino cloud | Importante | No |
| 22 | Multilingua IT/EN | Importante | **Sì** |
| 23 | Rilevamento lingua del brano | Nice-to-have | No |
| 24 | Analytics di utilizzo | Importante | No |
| 25 | Crash reporting | Critica | No |
| 26 | Monitoraggio performance | Importante | No |
| 27 | Sicurezza potenziata | Critica | No |
| 28 | Conformità GDPR (Terms, disclaimer upload, dichiarazione diritti) | Critica | **Sì** (nel documento compare "Copyright e responsabilità") |
| 29 | Autenticazione a due fattori | Nice-to-have | No |
| 30 | Marketplace canzoni pubbliche | Nice-to-have | No |
| 31 | Funzioni social | Nice-to-have | No |
| 32 | Playlist collaborative | Nice-to-have | No |
| 33 | Analisi delle progressioni | Nice-to-have | No |
| 34 | Trasposizione alla tonalità ideale per la voce | Nice-to-have | No |
| 35 | Companion Apple Watch | Nice-to-have | No |
| 36 | Companion Android Wear | Nice-to-have | No |
| 37 | Estensione browser (chord scraper) | Nice-to-have | **Parziale** (scraping con limiti legali) |
| 38 | Ottimizzazione database | Importante | No |
| 39 | Compressione immagini e media | Importante | No |
| 40 | Ottimizzazione tempo di avvio | Importante | No |
| 41 | Manuale utente (IT/EN) | Importante | No |
| 42 | Documentazione per sviluppatori | Importante | No |
| 43 | Rilascio Google Play | Critica | No |
| 44 | Rilascio App Store | Critica | No |
| 45 | Rilascio Microsoft Store | Importante | No |
| 46 | Versione Premium / Pro | Futura | No |
| 47 | Licenze per chiese e organizzazioni | Futura | No |

**Note dichiarate in [FUT]:**
- La sezione "Features dal Prompt Iniziale" dichiara 10 su 13 implementate, 1 parziale (offline mode) e 2 "da implementare", ma elenca 4 voci da implementare (#13, #22, #28, #37).
- La leggenda dichiara che le funzioni richieste dal prompt sono Critiche; #13 e #22 sono invece Nice-to-have e Importante.
- Tra le funzioni "implementate" [FUT] include il riconoscimento audio (vedi sez. 5).

---

## 4. Problemi noti aperti

Tutti e 10 i problemi di [KNOWN] risultano aperti alla data del documento. "Reported bugs" in [KNOWN] riporta: nessuno.

| N. | Problema | Fase | Stato dichiarato in [KNOWN] |
|----|----------|------|-----------------------------|
| 1 | 12 integration test falliti in `AppCanzoniere.Backend.IntegrationTests` (0/12 passano) | 14 | Non risolto |
| 2 | Migrazione tabella `OfflineSongs` "generata ma non applicata" (vedi sez. 5 sulla discordanza) | 15 | Non applicata |
| 3 | `IFileService` senza implementazione MAUI | 15 | Implementazione mancante |
| 4 | URL backend hardcoded (`http://localhost:5091` in `MauiProgram.cs`) | 14 | Non risolto |
| 5 | Nessun refresh automatico dei token JWT | 14 | Non implementato |
| 6 | UI offline mancante (pulsanti download, indicatori, gestione) | 15 | Non implementata |
| 7 | UI ricerca avanzata mancante (`AdvancedSearchPage`) | 16 | Non implementata |
| 8 | Nessun dark mode | 16 | Non implementato |
| 9 | Nessun tool di code coverage configurato | 14 | Non configurato |
| 10 | Fuzzy search basata su `Contains()` e non su distanza di Levenshtein | 16 | Implementazione naïve |

**Limitazioni dichiarate in [KNOWN]:**
- Database: solo SQLite; nessuna cifratura del DB locale; utente singolo, senza accessi concorrenti.
- Offline: nessuna risoluzione dei conflitti; nessun download di PDF/MP3; limite 500 MB fisso.
- Backend: nessun rate limiting; nessuna paginazione (tutti i brani in una risposta); nessun versioning API.
- Sincronizzazione: nessuna sync in tempo reale; sync solo manuale; nessun rilevamento conflitti (last-write-wins).
- Servizi esterni: chiavi API non offuscate; nessun retry tranne che in DownloadService; nessuna cache di risultati OCR/AI.
- Multilingua: UI solo in inglese; nessun file .resx configurato; lingua delle canzoni non usata per il filtro UI.
- Copyright e compliance: nessuna UI di Terms & Conditions; nessun disclaimer di upload; `CopyrightStatus` tracciato ma non validato; nessuna privacy policy.
- Raccolte AI: non implementate; nessun algoritmo di clustering (ML.NET non integrato).
- Scraping: intenzionalmente non implementato; nessuna estensione browser.

**Problemi di piattaforma dichiarati in [KNOWN]:**
- Android: minimo API 24 (Android 7.0); permesso microfono da concedere manualmente.
- iOS: firma che richiede account Apple Developer; SecureStorage richiede iCloud abilitato nelle capability.
- Windows: DPAPI legato al profilo utente; MSIX richiede Windows SDK 19041+.
- macOS: limitazioni di Mac Catalyst rispetto ad AppKit; notarizzazione richiede account Apple Developer.

**Sicurezza dichiarata in [KNOWN]:** nessun certificate pinning; chiavi API non offuscate nell'app compilata; nessun audit di SQL injection; nessuna pentest. Raccomandazioni elencate: obfuscation, pinning, audit validazione input, rate limiting, pentest prima della produzione.

**Test dichiarati in [KNOWN]:** unit 77/77 passano; integration 0/12; nessun test E2E, UI o performance.

---

## 5. Incongruenze tra i documenti

1. **Fase 17:** [PLAN] "COMPLETATA (Infrastructure Ready)" e [TEST] "Infrastructure Complete"; [README] intestazione "Phase 17 Complete" ma nella sezione stato "Fase 17: Production Deployment (0%)".
2. **Fase 14:** [PLAN] intestazione "IN PROGRESS" e stesso blocco con "Stato: COMPLETATA (con limitazioni note)"; [TEST] e [README] indicano 40%.
3. **Fasi 1-13:** [README] "Completate: Fasi 1-13 (87%)"; [TEST] "Fasi 1-13: Complete (100%)".
4. **Fase 4:** [README] la conta come completata e nel roadmap elenca MusicXML, JSON custom e tablature; [PLAN] ha le stesse voci non spuntate, senza stato dichiarato nell'intestazione.
5. **Fase 9 (audio):** [PLAN] e [FUT] la considerano implementata; [README] la mette in "In Roadmap" (⏳) e indica il mock come "in sviluppo".
6. **Fase 10:** [README] la conta come completata; [PLAN] lascia tre provider come placeholder non spuntati; [KNOWN] dichiara "nessun rilevamento conflitti" e last-write-wins, mentre [PLAN] Fase 10 dichiara conflict resolution KeepLocal/KeepRemote/KeepBoth implementata.
7. **Migrazione OfflineSongs:** [KNOWN] #2 dice "generata ma non applicata"; [TEST], [SETUP] e [README] la trattano come ancora da generare.
8. **Integration test:** [PLAN] dichiara 13 test scritti (5 Account + 8 Songs); [TEST], [KNOWN] e [README] parlano di 12.
9. **Conteggio test e versione in README:** [PLAN] 77 test (incluso backend, 14); [README] "63 tests" e footer "Tests 63/63"; footer [README] "Versione 0.10.0" e "Fase corrente 11/17", mentre l'intestazione dice 0.17.0-alpha.
10. **Android minimo:** [PLAN] API 21+; [KNOWN] Android 7.0 (API 24).
11. **Versione .NET:** [PLAN] indica ".NET 8 SDK" nella sezione "Tecnologie chiave da configurare", mentre lo stesso documento e [README]/[SETUP] indicano .NET 10.
12. **Database backend:** [PLAN] "SQL Server/PostgreSQL"; [SETUP] SQLite di default e SQL Server in produzione; [KNOWN] "SQLite only, no SQL Server, PostgreSQL".
13. **Ruoli backend:** [PLAN] Fase 11 "User/Admin"; [PLAN] architettura "roles: user, moderator".
14. **Migrazioni client:** [README] "applicate automaticamente all'avvio"; [SETUP] comando manuale `dotnet ef database update`.
15. **Provider audio:** [PLAN] Fase 9, [README] e [SETUP] usano ACRCloud (con classe `ACRCloudAudioRecognitionService` in [SETUP]); [PLAN] architettura e DI usano `ShazamAudioRecognitionService`.
16. **Chiavi di configurazione non allineate:** modello Perplexity diverso tra [PLAN] (large) e [SETUP] (small); chiave Perplexity `Perplexity:ApiKey` in [PLAN] contro `AI:PerplexityApiKey` in [SETUP]; Azure `Azure:ComputerVision:ApiKey` in [PLAN] contro `OCR:AzureComputerVision:Key` in [SETUP]; redirect OneDrive `msauth://com.yourcompany.appcanzoniere` in [PLAN] contro `msal{ClientId}://auth` in [SETUP].
17. **Limite offline:** [PLAN] sezione mitigazioni `MaxOfflineDownloadMB = 200`; [PLAN] Fase 15, [SETUP] e [KNOWN] indicano 500 MB.
18. **OCR Tesseract:** [PLAN] lo descrive come "placeholder locale"; [README] elenca "Tesseract locale o Azure Computer Vision" come disponibili.
19. **Fuzzy search:** [PLAN] Fase 16 la dichiara "supportata" (voce spuntata); [KNOWN] #10 la descrive come implementazione naïve basata su Contains.
20. **Priorità di [FUT]:** conteggi "Implementate (10/13)" e "Da implementare (2/13)" con 4 voci elencate; la legenda assegna la priorità Critica alle funzioni richieste dal prompt, ma #13 e #22 non sono Critiche.
21. **Nome del file del prompt iniziale:** [README] e [PLAN] citano `docs/prompt_iniziale.md`; lo stato git del repository mostra il file rinominato in `docs/PROMPT-INIZIALE.md`.
22. **Date e stato del piano:** [PLAN] è marcato "Draft - In attesa di approvazione" con data 2026-01-31, mai aggiornato. [FUT] fissa target di release v1.0 nel Q2 2026, v1.1 nel Q3 e v1.2 nel Q4 2026; nessun documento risulta aggiornato dopo il 2026-02-01.
