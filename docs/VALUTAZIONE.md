# Valutazione tecnica indipendente — AppCanzoniere

Data: 2026-11-10 · Commit valutato: `c5ce11c` (unico commit, 2026-02-17) · SDK: .NET 10.0.401 su Windows 11

Legenda evidenze: **[ESEGUITO]** = verificato lanciando comandi o il codice; **[LETTO]** = dedotto leggendo il sorgente; **[NON VERIFICATO]** = non è stato possibile controllarlo.
I file README.md, TEST_AND_COMPLETE.md, KNOWN_ISSUES.md, FUTURE_IMPLEMENTATIONS.md e docs/plan.md sono stati trattati come affermazioni, non come fatti.

---

## 1. Sintesi (10 righe)

1. Il codice non-MAUI compila senza errori e 77 test unitari passano; 3 di questi però sono test vuoti (`Test1`) e altri 6 controllano solo i valori predefiniti di POCO. **[ESEGUITO]**
2. I test di integrazione del backend falliscono: 10 su 12. La causa non è un problema di setup dei test, come dice KNOWN_ISSUES. Il backend è rotto: Identity è collegato a un `DbContext` che non contiene le tabelle utenti. **[ESEGUITO]**
3. Il backend avviato davvero risponde 500 sia a `GET /api/songs` (`no such table: Songs`, le migrazioni non vengono mai applicate) sia a `POST /api/account/register`. Oggi login e DB pubblico non funzionano. **[ESEGUITO]**
4. Il progetto MAUI non si compila con il comando della soluzione su questa macchina (workload incompleto: manca il pack `Microsoft.NET.Runtime.MonoTargets.Sdk.net10`). Su una copia limitata al solo target Windows compila con 0 errori e 171 warning. **[ESEGUITO]**
5. Il valore reale sta nel nucleo musicale: parser ChordPro e trasposizione funzionano e sono testati (46 test), insieme al modello dati di dominio (Song, Album, Tag, momenti liturgici, riferimenti biblici).
6. Tutti i servizi esterni usati dall'app sono mock: AI accordi, OCR e riconoscimento audio. Tesseract è uno stub, la registrazione dal microfono non c'è e l'unico provider cloud è una cartella locale. **[LETTO]**
7. Mancano del tutto: multilingua (nessun file resx, testi UI in inglese fissi nel codice), notazione DO/RE/MI, accordi per pianoforte, tablature, Terms & Conditions, consenso all'upload, album automatici AI ed export di una canzone o di un album.
8. Nell'app MAUI ci sono bug bloccanti probabili, non verificati a runtime. Aprire una canzone esistente mostra un modulo vuoto (QueryProperty sul ViewModel) e salvare una canzone nuova va in errore (`Update` su un Id inesistente). L'inizializzazione del DB contraddice se stessa (`EnsureCreated` e poi `MigrateAsync` su migrazioni che EF non riesce a trovare).
9. La CI non è mai stata verde: l'unica esecuzione su GitHub (2026-08-02) fallisce in "Build solution". Il workflow ha anche errori di progetto che lo farebbero fallire di nuovo (restore della soluzione con MAUI su Ubuntu, test di integrazione rossi, percorsi degli artefatti sbagliati). **[ESEGUITO via `gh`]**
10. Giudizio: vanno tenuti Domain, parser/trasposizione e le interfacce dei servizi. Vanno rifatti persistenza e migrazioni, autenticazione backend, navigazione MAUI e CI. Il "~87% completato" dichiarato nel README non corrisponde alla realtà: stimo che un 25-30% dei requisiti funzioni davvero end-to-end.

---

## 2. Esito reale di build e test

### 2.1 Restore e build della soluzione

| Comando | Esito | Dettaglio |
|---|---|---|
| `dotnet restore AppCanzoniere.slnx` | **FALLITO** | `NETSDK1147: To build this project, the following workloads must be installed: macos [...AppCanzoniere.MAUI.csproj::TargetFramework=net10.0-android]` |
| `dotnet build AppCanzoniere.slnx` | **FALLITO** | stesso errore NETSDK1147, ripetuto 2 volte |

Diagnosi **[ESEGUITO]**: `dotnet msbuild -getItem:MissingWorkloadPack` restituisce, per `net10.0-android`, il pack mancante `Microsoft.NET.Runtime.MonoTargets.Sdk.net10` 10.0.14. Per `net10.0-windows10.0.19041.0` non manca nessun pack. Il messaggio "macos" dell'SDK è fuorviante. `dotnet workload --info` dice "No workload sets are installed": i manifest sono di versioni diverse (maui 10.0.20, android 36.1.69, ios/maccatalyst 26.5) e non c'è un workload set coerente. È un problema dell'ambiente, non del codice. Probabile rimedio: `dotnet workload restore` oppure `dotnet workload update` da prompt amministratore. **Non l'ho eseguito per non modificare l'SDK di sistema.** Un `bin/Debug/net10.0-android` già presente indica che in passato il target Android compilava.

### 2.2 Build per progetto (repo reale)

| Progetto | Esito | Warning |
|---|---|---|
| Domain | OK | 0 |
| Application | OK | 0 |
| Infrastructure | OK | 6 (NU1903 SQLitePCLRaw, CS8602 in `AlbumRepository.cs:43`) |
| Backend | OK | 12 (NU1903 Microsoft.OpenApi 2.0.0 e SQLitePCLRaw 2.1.11) |
| Domain.Tests / Application.Tests | OK | 0 |
| Infrastructure.Tests | OK | 8 |
| Backend.Tests | OK | 44 (CS8625 null passati ai costruttori mock) |
| Backend.IntegrationTests | OK | 20 |

### 2.3 Build MAUI per target

| Target | Esito | Note |
|---|---|---|
| `-f net10.0-windows10.0.19041.0` sul repo reale | **FALLITO** | NETSDK1147, perché il restore valuta tutti i TFM |
| idem con `-p:TargetFrameworks=...` | **FALLITO** | NETSDK1005: la proprietà globale passa anche ai progetti referenziati e rompe gli assets |
| `-f net10.0-android` | **FALLITO** | NETSDK1147 / pack MonoTargets mancante (ambiente) |
| **Copia in scratchpad con csproj ridotto al solo Windows** | **OK** | `AppCanzoniere.MAUI.dll` prodotto, **0 errori, 171 warning unici**: 94 MVVMTK0045 (`[ObservableProperty]` su campi, non compatibile AOT/WinRT), 72 CS0618 (`DisplayAlert` obsoleto, sostituirlo con `DisplayAlertAsync`), 4 NU1903, 1 CS8602 |
| iOS / MacCatalyst | **NON VERIFICATO** | richiedono un Mac |

Il codice C#/XAML della MAUI quindi compila (verificato su Windows). Android non l'ho potuto verificare su questa macchina.

### 2.4 Test (`dotnet test --no-build`, progetto per progetto) **[ESEGUITO]**

| Progetto | Passati | Falliti | Totale | Note di qualità |
|---|---|---|---|---|
| Domain.Tests | 6 | 0 | 6 | 1 è `Test1` vuoto; il resto controlla solo valori predefiniti e setter |
| Application.Tests | 46 | 0 | 46 | 1 è `Test1` vuoto; 45 test veri su parser/trasposizione, **utili** |
| Infrastructure.Tests | 11 | 0 | 11 | 1 è `Test1` vuoto; usano EF InMemory, quindi non vedono vincoli SQLite né migrazioni |
| Backend.Tests | 14 | 0 | 14 | `UserManager` è un mock: per questo non vedono il bug di Identity |
| Backend.IntegrationTests | **2** | **10** | 12 | 9× `Cannot create a DbSet for 'IdentityUser' because this type is not included in the model`; 1× 400 al posto di 200 |
| **Totale** | **79** | **10** | **89** | il "77/77" del README esclude gli integration test; 3 dei 77 sono vuoti |

I due integration test che passano sono `CreateSong_WithoutAuth_ReturnsUnauthorized` e `Register_PasswordMismatch_ReturnsBadRequest`: nessuno dei due tocca il DB. Nessun test copre i ViewModel MAUI, i servizi esterni reali, CloudSync, DownloadService o le migrazioni.

### 2.5 Avvio reale del backend **[ESEGUITO]** (`dotnet run --launch-profile http`, Jwt da user-secrets)

| Chiamata | Risultato |
|---|---|
| `GET /api/songs` | **500** `SqliteException: no such table: Songs` (nessuna `Migrate()` in `Program.cs`) |
| `POST /api/account/register` | **500** `InvalidOperationException: Cannot create a DbSet for 'IdentityUser'` |
| `GET /openapi/v1.json` | 200 |

`dotnet ef migrations list` sul Backend restituisce `20260201094323_InitialCreate (Pending)`. `has-pending-model-changes` restituisce "Changes have been made to the model since the last migration": manca `OfflineSongs`. Il file `.db` vuoto creato dal test l'ho poi cancellato.

---

## 3. Matrice di tracciabilità requisito → stato → evidenza

Stati: ✅ Completo · 🟡 Parziale · 🎭 Solo mock/placeholder · ❌ Assente. Se non indicato altrimenti, le evidenze sono **[LETTO]**.

### 3.1 Obiettivi generali (1–10)

| # | Requisito | Stato | Evidenza |
|---|---|---|---|
| 1a | Testi + accordi | 🟡 | Solo ChordPro: `Song.ChordProContent` (`Domain/Entities/Song.cs`), parser `Application/Services/ChordProParser.cs`. La resa a schermo è il testo ChordPro grezzo con accordi in linea (`ChordProParser.cs:115-194` → `SongDetailPage.xaml:62`), non accordi sopra il testo |
| 1b | Guitar Tab | ❌ | Nessun parsing `{start_of_tab}` e nessuna entità Tab. `PdfType.Tablature` è solo un tipo di allegato |
| 1c | Accordi pianoforte | ❌ | Nessun codice che calcoli le note dell'accordo. Il pulsante `🎸/🎹` non ha Command (`SongDetailPage.xaml:120-123`) |
| 2a | Transpose su/giù | ✅ (logica) / 🟡 (UI) | `ChordTransposer.cs`, testato (`ChordTransposerTests.cs`). UI: `SongDetailViewModel.cs:62-102`. `Render` crea `new ChordTransposer()` invece di usare DI (`ChordProParser.cs:118`) e ignora la preferenza bemolle |
| 2b | Scelta chitarra/pianoforte | ❌ | `UserSettings.PreferredInstrument` viene salvato (`SettingsPage.xaml:29`) ma nessun codice lo legge |
| 3a | DB privato locale | 🟡 | SQLite + EF (`MauiProgram.cs:33-35`). L'inizializzazione è rotta, vedi §4.3 |
| 3b | DB pubblico/condiviso | 🟡 rotto | Backend `SongsController.cs` esiste ma risponde 500 **[ESEGUITO]**. Lettura dalla MAUI in `PublicSongsViewModel.cs:32,68`. Dall'app non si pubblica nulla (`IBackendSongService.CreateSongAsync` non è mai usato nella MAUI) |
| 3c | Sync DB privato su cloud utente | 🎭 | Unico provider: `LocalStorageProvider` (copia in `LocalApplicationData`, `LocalStorageProvider.cs:15-16`). OneDrive/Drive/Dropbox commentati (`MauiProgram.cs:50-53`); merge con TODO (`CloudSyncService.cs:165,310`) |
| 4 | Metadati ricchi | 🟡 | Entità complete (Song, YouTubeLink, Mp3Attachment, PdfAttachment, BibleReference, LiturgicalMoment). **UI**: Title/Artist/Key/ChordPro (`SongDetailPage.xaml:68-90`); tag, momenti liturgici e riferimenti biblici (`SongMetadataPage.xaml`). **Nessuna UI** per lingua, tipo, YouTube, MP3, PDF, copyright, pubblico/privato |
| 5a | Riconoscimento audio | 🎭 | DI su `MockAudioRecognitionService` (`MauiProgram.cs:71`), che restituisce un risultato casuale. Microfono: "Not Implemented" (`AudioRecognitionViewModel.cs:162-168`). ACRCloud scritto ma non collegato e non testato |
| 5b | Accordi via AI + disclaimer | 🎭 (+ disclaimer ✅) | DI su `MockChordSuggestionService` (`MauiProgram.cs:59`). Perplexity non collegato e con difetti (§4.2). Disclaimer presente (`AiChordSuggestionPage.xaml:133-150`) |
| 6a | OCR PDF testuali/scansioni | 🎭 | DI su `MockOcrService` (`MauiProgram.cs:64`). Tesseract è uno stub che restituisce errore (`TesseractOcrService.cs:45-58,119`). Azure senza PDF (`AzureComputerVisionService.cs:192`). Nessun parsing diretto dei PDF testuali |
| 6b | Scraping siti accordi (conforme ToS) | ❌ | Nessuna interfaccia o codice |
| 7a | Album manuali | ✅ | `AlbumListViewModel`, `AlbumDetailViewModel.cs:78-186` (aggiunta, rimozione, ordinamento), `AlbumRepository` |
| 7b | Album automatici da AI | ❌ | Esiste solo il valore `AlbumType.AIGenerated` (`Domain/Enums/AlbumType.cs`) |
| 8 | Offline (tutto / album / canzone) | 🟡 / ❌ | Il DB locale è offline per natura. La copia locale dal DB pubblico è codice morto: `DownloadService` e `OfflineSongRepository` non sono registrati in DI né usati; media con TODO (`DownloadService.cs:129`) |
| 9 | Copyright, T&C, consenso upload, privacy | ❌ (modello 🟡) | Esiste `CopyrightStatus` nel dominio ma nessuna UI lo imposta. Nessuna pagina T&C o checkbox "Sono responsabile". `CreatedBy` non viene mai valorizzato (`SongsController.cs:79-95`) |
| 10 | Multilingua IT/EN | ❌ | Nessun `.resx` né `AppResources`. Testi XAML in inglese fissi. `Song.Language` esiste ma non è modificabile. I nomi dei momenti liturgici sono in italiano fisso (`DatabaseSeeder.cs:30+`) |

### 3.2 Funzionalità chiave (1–7)

| Gruppo | Requisito | Stato | Evidenza |
|---|---|---|---|
| 1 Gestione canzoni | CRUD | 🟡 probabilmente rotto | Apertura: `[QueryProperty]` sul ViewModel (`SongDetailViewModel.cs:9`), mentre Shell lo applica solo alla Page, e `OnAppearing` crea una nuova Song vuota (`SongDetailPage.xaml.cs`). Salvataggio di una nuova canzone: `Song.Id = Guid.NewGuid()` quindi `Id == Guid.Empty` è sempre falso (`SongDetailViewModel.cs:111`) e chiama `UpdateAsync` su una riga inesistente (`SongRepository.cs:55-61`), con probabile `DbUpdateConcurrencyException`. **[LETTO, da verificare a runtime]** |
| 1 | Allegati YouTube/MP3/PDF | ❌ UI | Entità sì, UI no. YouTube viene aggiunto solo dal flusso audio (mock) (`AudioRecognitionViewModel.cs:268-273`) |
| 1 | Momenti messa, riferimenti biblici, tag | ✅ (UI + DB) | `SongMetadataViewModel.cs`, `SongMetadataPage.xaml`. Seed di 11 momenti (`DatabaseSeeder.cs`) |
| 2 Visualizzazione | Testo+accordi leggibile | 🟡 | ChordPro grezzo, vedi 1a |
| 2 | Tab chitarra / note accordo pianoforte | ❌ | — |
| 2 | Transpose tab | ❌ | — |
| 2 | Notazione DO/RE/MI vs C/D/E, diesis/bemolle | ❌ (solo impostazione) | `UserSettings.cs:8-9` e `SettingsPage.xaml:34-42` salvano il valore, ma `ChordTransposer` conosce solo C–B (`ChordTransposer.cs:9-12`) e nessuno legge le impostazioni |
| 3 Album | Manuali + ordinamento | ✅ | vedi 7a |
| 3 | Agente AI | ❌ | vedi 7b |
| 4 Ricerca | Titolo/autore | ✅ | `SongRepository.SearchAsync`, `HomeViewModel.cs:45-64`, `SearchViewModel.cs:44-87` |
| 4 | Tag | ✅ | `GetSongsByTagsAsync` (`SearchViewModel.cs:59-61`) |
| 4 | Tonalità, tipo, lingua, momenti messa, rif. biblici | ❌ UI / 🟡 repo | `AdvancedSearchAsync` (`SongRepository.cs:73`) non è usato dalla UI. Converte gli Id tag da `int` a `Guid` in modo errato (`SongRepository.cs:124`). Nessun filtro per momento liturgico (commento in-code) |
| 5 Import/export | Import PDF testuale / OCR / siti | 🎭 / ❌ | vedi 6a, 6b |
| 5 | Export canzone (JSON/ChordPro), album | ❌ | — |
| 5 | Backup/restore intero DB | 🟡 | Copia del file `.db` (`CloudSyncService.cs:121-148`, import 150-200, merge non implementato). La DbContext singleton chiude la connessione (rischio file bloccati, §4.3) |
| 6 Online/offline | Offline per canzoni scaricate | ❌ | vedi 8 |
| 6 | Sync cloud backup/restore | 🎭 | vedi 3c |
| 6 | Accesso DB pubblico, distinzione privato/pubblico | 🟡 rotto | Backend 500 **[ESEGUITO]**. Il flag `IsPublic` non è modificabile nella MAUI. Le canzoni private sono visibili **solo agli Admin**, non al proprietario (`SongsController.cs:45-56`) |
| 7 Copyright | Avvisi, accettazione upload, stato legale, tracciamento uploader | ❌ (solo modello) | vedi 9 |
| — | Autenticazione | 🟡 rotto | JWT + Identity nel backend: non funziona (§2.5). Il ruolo "User" non viene mai creato (`AccountController.cs:50`). Il client MAUI punta a `https://localhost:5001/api/` (`MauiProgram.cs:81,87`), che non corrisponde alle porte reali (7052/5043) e su Android non raggiunge l'host. Token in SecureStorage ✅ (`Services/SecureTokenStorage.cs`). Refresh token con TODO (`AuthService.cs:98`) |

---

## 4. Valutazione qualità per layer

### 4.1 Domain — **buono, da tenere**
- Entità POCO pulite e senza dipendenze esterne (verificato nel csproj). Il modello copre bene i requisiti: momenti liturgici, riferimenti biblici, allegati, versioning (`Version`, `IsSynced`, `LastSyncedAt`).
- Il modello è anemico (solo setter pubblici, nessuna invariante); per l'uso attuale è accettabile.
- Due modelli paralleli per lo stesso concetto: `OfflineSong` (Id `int`, `SyncStatus`) e `Song` (`IsDownloadedForOffline`, `IsSynced`). Va scelto uno solo.
- `UserSettings` usa stringhe magiche ("Guitar", "Anglo") invece di enum.
- `AdvancedSearchCriteria`/`AdvancedSearchResult` stanno in `Domain/Interfaces` ma sono DTO.

### 4.2 Application — **misto: nucleo buono, servizi esterni da rifare**
- Buoni e testati: `ChordProParser` e `ChordTransposer`. Da migliorare: rendering per la UI (righe accordi sopra il testo), notazione italiana, bemolle da preferenza, tonalità minori con alterazioni.
- **Violazione della Clean Architecture**: client HTTP verso servizi esterni (Perplexity, ACRCloud, Azure), I/O su file (`LocalStorageProvider`) e client backend (`AuthService`, `BackendSongService`) stanno in Application. Il posto giusto è Infrastructure.
- `PerplexityChordSuggestionService`: modello `llama-3.1-sonar-small-128k-online` dismesso da Perplexity (`:53`). Aggiunge l'header Authorization a ogni chiamata su `DefaultRequestHeaders` (`:64`), con header duplicati alla seconda chiamata. Legge la chiave da una variabile d'ambiente (`:19`), che su mobile non esiste. Il costruttore con `string? apiKey` non è risolvibile dalla DI come registrato nel commento.
- Le interfacce (`IChordSuggestionService`, `IOcrService`, `IAudioRecognitionService`, `ICloudStorageProvider`, `ICloudSyncService`) sono ragionevoli e vanno tenute come contratti.

### 4.3 Infrastructure / persistenza / migrazioni — **da rifare**
- `AppDbContext : DbContext` (`Data/AppDbContext.cs:6`) è usato sia dalla MAUI sia dal backend, ma il backend ci aggancia Identity (`Program.cs:28-38`) che richiede `IdentityDbContext`. **È la causa dei 9 integration test rossi** **[ESEGUITO]**.
- Due serie di migrazioni divergenti:
  - `Infrastructure/Migrations/*`: scritte a mano, **senza file Designer e senza attributi `[Migration]`/`[DbContext]`**, quindi EF non le trova. Lo snapshot non contiene `OfflineSongs`.
  - `Backend/Migrations/20260201094323_InitialCreate`: mai applicata (`Pending`) e già superata dal modello **[ESEGUITO]**.
- MAUI: `EnsureCreated()` in `MauiProgram.cs:127`, poi `MigrateAsync()` + seed lanciati senza attendere il risultato in `App.xaml.cs:14,23`. Con EF 9+ e modifiche al modello non migrate, `Migrate` lancia un'eccezione, che qui viene ingoiata, quindi **il seed probabilmente non parte mai** **[LETTO]**. CLAUDE.md dice "Automatic migrations on startup": non è vero per il backend.
- `CloudSyncService` sta nel progetto Infrastructure ma ha namespace `AppCanzoniere.Application.Services` (`CloudSyncService.cs:6`). È registrato Singleton (`MauiProgram.cs:56`) e cattura una `AppDbContext` Scoped (captive dependency): DbContext condivisa tra thread e mai rinnovata.
- `SongsController.GetPublicSongs/Search` caricano **tutte** le canzoni in memoria e filtrano lato server (`SongsController.cs:26,64`).
- Test su EF InMemory: non rilevano differenze con SQLite (case-sensitivity, vincoli, migrazioni).

### 4.4 MAUI (Pages + ViewModels) — **struttura recuperabile, collegamenti da rifare**
- 13 pagine e 13 ViewModel con CommunityToolkit.Mvvm. Sono collegati a **repository reali** (SQLite) per Home, Search, Album, Metadata e Settings. Sono collegati a **mock** per AI, OCR e Audio, e a **storage locale** per Cloud Sync. Non ci sono dati finti scritti nei ViewModel: i dati demo arrivano dai servizi mock e dal seeder.
- Bug probabili (da confermare a runtime):
  - `[QueryProperty]` su 4 ViewModel (`SongDetail`, `SongMetadata`, `AiChordSuggestion`, `AlbumDetail`). Shell applica `QueryProperty` alla Page o a un `IQueryAttributable`, quindi i parametri di navigazione probabilmente non arrivano.
  - Salvataggio di una canzone nuova rotto (vedi 3.2).
  - La route `publicsongs` è sia una `ShellContent` (`AppShell.xaml:22`) sia registrata con `Routing.RegisterRoute` (`AppShell.xaml.cs:18`): rischio di eccezione "duplicated route".
  - Icone `home.png`, `cloud.png` ecc. referenziate in `AppShell.xaml` ma assenti in `Resources/Images` (c'è solo `dotnet_bot.png`).
- Debito: 72 chiamate a `DisplayAlert` obsolete, 94 warning MVVMTK0045, `ApplicationId` ancora `com.companyname.appcanzoniere.maui`, `MainPage.xaml` del template inutilizzata, `StringToBoolConverter.ConvertBack` lancia `NotImplementedException`.
- Nessun test sui ViewModel.

### 4.5 Backend e sicurezza
| Aspetto | Stato | Evidenza |
|---|---|---|
| Segreti nel repo | ✅ nessuno | `appsettings*.json` è in `.gitignore`. Jwt è in user-secrets (presente su questa macchina, solo i nomi delle chiavi controllati) |
| Chiave JWT di fallback nel codice | ❌ | `Program.cs:41` `"default-secret-key-must-be-32-chars"`: se manca la configurazione il backend parte comunque con una chiave nota |
| `appsettings.json` di base | ❌ assente | in Production mancano Issuer, Audience e chiave, quindi la validazione JWT fallisce o usa il fallback |
| CORS | ❌ | `AllowAnyOrigin/Method/Header` (`Program.cs:64-72`) |
| Autorizzazione | 🟡 | Update/Delete solo per Admin; il proprietario non può modificare le proprie canzoni; nessun ruolo viene mai creato; `CreatedBy` non viene valorizzato |
| Password policy | 🟡 | lunghezza minima 6 (`Program.cs:34`) |
| Rate limiting / lockout | ❌ | `CheckPasswordSignInAsync(..., false)` |
| Input validation | ❌ | nessuna DataAnnotation o validazione sui DTO |
| HTTPS | 🟡 | `UseHttpsRedirection` c'è; il profilo `http` non ha una porta HTTPS ("Failed to determine the https port") |

### 4.6 Dipendenze **[ESEGUITO]**
- **Vulnerabili (High)**: `Microsoft.OpenApi` 2.0.0 (transitiva da `Microsoft.AspNetCore.OpenApi` 10.0.2, GHSA-v5pm-xwqc-g5wc) e `SQLitePCLRaw.lib.e_sqlite3` 2.1.11 (transitiva da EF Sqlite 10.0.2, GHSA-2m69-gcr7-jv3q).
- **Obsolete**: tutto lo stack ASP.NET/EF è a 10.0.2, l'ultima è 10.0.14. Scalar 2.0.36 → 2.19.0; Microsoft.NET.Test.Sdk 17.14.1 → 18.11.0; xunit.runner.visualstudio 3.1.4 → 4.0.1; coverlet 6.0.4 → 10.1.0; Moq 4.20.72 → 4.21.0.
- **Attenzione**: FluentAssertions 7.0.0 → 8.x **cambia licenza (commerciale)**. Restare su 7.x o passare ad AwesomeAssertions/Shouldly.
- `Microsoft.AspNetCore.Mvc.Testing` è a 10.0.0 in un progetto e a 10.0.2 nell'altro (versioni disallineate). Non c'è un `Directory.Packages.props` per la gestione centralizzata.
- Pacchetti MAUI: `dotnet list package` non è eseguibile perché il restore MAUI fallisce in questo ambiente **[NON VERIFICATO]**.

---

## 5. Stato CI (`.github/workflows/build-and-test.yml`)

**Esito reale [ESEGUITO via `gh run list/view`]**: una sola esecuzione (run 30764736376, 2026-08-02), **fallita** nello step "Build solution" con `The runtime pack for Microsoft.NETCore.App.Runtime.win-x64 was not downloaded`. Tutti i job a valle sono stati saltati. Gli artefatti `.trx` e `.coverage` non sono stati trovati.

Cosa manca o è sbagliato perché test e pubblicazione passino interamente via CI:
1. **Build della soluzione intera con MAUI in Release + `--no-restore`**: il restore non include il RID `win-x64` richiesto dalla MAUI Windows. Separare la build "core" (librerie, backend, test, su Ubuntu) dalle build MAUI per piattaforma.
2. **Test**: `dotnet test` sulla soluzione include gli integration test, che oggi sono rossi. Il job resterebbe rosso anche dopo aver sistemato la build. Servono un filtro o un job separato finché il backend non è corretto.
3. **Coverage**: con `XPlat Code Coverage` il file prodotto è `coverage.cobertura.xml`, non `*.coverage`, quindi l'upload è vuoto. Non c'è nessuna soglia né report.
4. **`build-backend` su ubuntu-latest** esegue `dotnet restore AppCanzoniere.slnx`, che include il progetto MAUI con TFM android/ios/maccatalyst senza workload: fallirebbe con NETSDK1147 **[LETTO]**. Va fatto il restore del solo progetto Backend.
5. **Android**: nessuna firma (keystore nei Secrets), nessun `AndroidPackageFormat` esplicito (in Release il default è AAB, ma il pattern di upload cerca `*.apk`), nessun upload verso Play Console.
6. **Windows**: `WindowsPackageType=None` non produce `.msix`, quindi l'upload `**/*.msix` è vuoto. Nessuna firma del codice.
7. **iOS/macOS**: solo `dotnet build`, nessuna firma e nessun provisioning; gli upload puntano a `./artifacts/ios` e `./artifacts/macos` che non vengono mai creati. iOS non è prioritario per i requisiti, quindi conviene toglierli o lasciarli manuali.
8. **Nessun job di pubblicazione**: nessuna release GitHub o tag, nessun versioning automatico (`ApplicationVersion` fisso a 1), nessun deploy del backend (container o App Service), nessuna gestione degli ambienti o dei segreti di produzione.
9. **Qualità**: niente `dotnet format --verify-no-changes`, niente `dotnet list package --vulnerable` come gate, niente caching NuGet o workload, niente versione SDK fissata (`global.json` assente), niente workload set fissato (`dotnet workload restore` / `--version`). Le action `@v4` girano su Node 20 deprecato (annotazione del runner).

---

## 6. Tenere / Rifare / Buttare

| Decisione | Cosa |
|---|---|
| **TENERE** | `Domain` (entità ed enum; rivedere OfflineSong/UserSettings). `ChordProParser` + `ChordTransposer` e i loro 45 test. Le interfacce dei servizi esterni e i DTO. I Mock come implementazioni di sviluppo. `AlbumRepository`/`SongRepository` (CRUD base). `SongMetadataViewModel` e la gestione di tag, momenti liturgici e riferimenti biblici. `DatabaseSeeder` (momenti liturgici). La struttura MVVM/DI della MAUI. `SecureTokenStorage`. |
| **RIFARE** | Persistenza: un solo set di migrazioni generate da EF, `Migrate()` atteso all'avvio, niente `EnsureCreated`, eventualmente DbContext separati MAUI/Backend (`IdentityDbContext` per il backend). Autenticazione backend: Identity, ruoli, nessun fallback della chiave, CORS ristretto, `CreatedBy`, ownership. Navigazione e passaggio parametri MAUI (`IQueryAttributable`). Salvataggio canzone nuova. Rendering accordi sopra il testo. Notazione IT/EN e preferenza bemolle. Configurazione URL backend per piattaforma. Servizi esterni reali spostati in Infrastructure, con configurazione e chiavi gestite. CI (da zero, piccola). Test di integrazione su SQLite in-memory reale. |
| **BUTTARE** | `Infrastructure/Migrations/*` scritte a mano. `Backend/Migrations` attuale (va rigenerata). `TesseractOcrService` stub (o lo si implementa o lo si toglie). `AdvancedSearchAsync` nella forma attuale (conversione int→Guid errata). `DownloadService` + `OfflineSongRepository` + `OfflineSong` finché non c'è un design offline. I tre `UnitTest1.cs` vuoti. `MainPage.xaml` del template. Gran parte di README, TEST_AND_COMPLETE, KNOWN_ISSUES e FUTURE_IMPLEMENTATIONS (~1.950 righe) e di `docs/plan.md` (2.549 righe): dichiarazioni inesatte che fuorviano chi riprende il lavoro. Tenere solo il prompt originale e questa valutazione come base. Rivedere CLAUDE.md, che dichiara cose false: "77 passing" e "Automatic migrations on startup". |

---

## 7. Rischi principali per la ripresa

1. **Falsa percezione di completezza**: i documenti dichiarano "Phase 17 Complete – Production Ready" e "87%". Chi pianifica partendo da lì sottostima il lavoro di 2-3 volte.
2. **Persistenza incoerente**: due catene di migrazioni divergenti, `EnsureCreated` più `Migrate`, eccezioni ingoiate all'avvio. Con dati reali degli utenti diventa un rischio di perdita o corruzione del DB locale ai prossimi cambi di schema.
3. **Backend non funzionante e insicuro di default** (Identity rotto, chiave JWT di fallback nota, CORS aperto, nessuna ownership). Pubblicarlo oggi esporrebbe un DB "pubblico" senza un modello di permessi.
4. **Bug di navigazione e salvataggio nella MAUI**, non coperti da test né verificati a runtime: l'app potrebbe non permettere nemmeno il flusso base "apri o crea una canzone".
5. **Ambiente di build fragile**: il workload set non è coerente (Android non compila qui), non c'è `global.json` né un workload fissato. Ogni macchina e ogni runner CI possono comportarsi diversamente.
6. **Dipendenze da servizi esterni mai provate**: Perplexity (modello dismesso), ACRCloud e Azure CV non sono mai stati eseguiti. Costi, termini d'uso e copyright (scraping, AI) non sono indirizzati nel codice: mancano feature flag per disattivarli per contesto legale.
7. **Requisiti legali assenti**: T&C, consenso all'upload, stato del copyright modificabile e tracciamento dell'uploader sono obbligatori prima di aprire il DB pubblico.
8. **Vulnerabilità note (High)** nelle dipendenze transitive (OpenApi, SQLite nativo) e stack fermo a 10.0.2: va aggiornato prima di qualsiasi rilascio.
9. **Multilingua da introdurre a posteriori**: tutte le stringhe sono nel XAML e nei ViewModel. Più si aspetta, più costa estrarle in resx.
10. **Copertura di test illusoria**: i test che "passano" coprono soprattutto parser e POCO. Le aree a rischio (DB reale, auth, ViewModel, sync) sono scoperte; gli unici test che le toccano sono rossi.

---

## 8. Limiti di questa valutazione
- La MAUI non è stata **avviata** (né Windows né Android): i bug di navigazione e salvataggio sono dedotti dal codice.
- Android, iOS e MacCatalyst non sono stati compilati (workload locale incompleto / Mac non disponibile).
- I servizi esterni reali (Perplexity, ACRCloud, Azure) non sono stati chiamati (nessuna chiave, e farlo avrebbe costi).
- `dotnet list package --outdated/--vulnerable` non è stato eseguito sul progetto MAUI (restore fallito).
- La build Windows riuscita è stata fatta su una **copia** del repo in scratchpad con il csproj ridotto al solo target Windows. Il repo originale non è stato modificato.
