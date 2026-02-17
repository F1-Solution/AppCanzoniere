# AppCanzoniere - .NET MAUI

**Versione:** 0.17.0-alpha ✨ **ULTIMA FASE COMPLETATA**  
**Framework:** .NET 10.0  
**Piattaforme:** Android, iOS, Windows, macOS  
**Status:** Phase 17 Complete - Production Ready (Infrastructure) 🎉

## 📋 Descrizione

AppCanzoniere è un'applicazione multi-piattaforma per la gestione di canzoni con accordi, tablature e spartiti. Supporta formati ChordPro, MusicXML e JSON custom, con funzionalità di trasposizione tonalità, sincronizzazione cloud multi-provider, OCR per import da PDF, AI chord suggestions, riconoscimento audio e **backend API RESTful con autenticazione JWT**.

## ⭐ Features Principali

- 🎵 **Gestione Canzoni:** CRUD completo, parser ChordPro, trasposizione accordi real-time
- 📚 **Organizzazione:** Album/playlist con ordinamento, tag liturgici e generici, riferimenti biblici
- 🤖 **AI Services:** Suggerimento accordi via Perplexity AI, con disclaimer copyright
- 📷 **OCR Import:** Estrazione testo da immagini/PDF (Tesseract locale o Azure Computer Vision)
- 🎤 **Audio Recognition:** Identificazione brani da file audio (ACRCloud - 80M+ tracks)
- ☁️ **Cloud Sync:** Multi-provider (OneDrive, Google Drive, Dropbox) - in sviluppo
- 🔐 **Backend API:** ASP.NET Core REST API con JWT auth, Swagger docs, Songs/Account endpoints
- 📱 **Multi-platform:** Android, iOS, Windows Desktop, macOS con .NET MAUI

## 🏗️ Architettura

Il progetto segue i principi della **Clean Architecture** con separazione in layer:

```
AppCanzoniere/
├── src/
│   ├── AppCanzoniere.Domain/          # Entità, interfacce, enums
│   ├── AppCanzoniere.Application/     # Servizi applicativi, DTOs
│   ├── AppCanzoniere.Infrastructure/  # EF Core, repositories, servizi esterni
│   ├── AppCanzoniere.MAUI/            # UI multi-platform, ViewModels, Pages
│   └── AppCanzoniere.Backend/         # ASP.NET Core Web API (JWT, Swagger)
├── tests/
│   ├── AppCanzoniere.Domain.Tests/
│   ├── AppCanzoniere.Application.Tests/
│   └── AppCanzoniere.Infrastructure.Tests/
└── docs/                               # Documentazione e piani
│   ├── AppCanzoniere.MAUI/            # UI multipiattaforma (MVVM)
│   └── AppCanzoniere.Backend/         # API REST per DB pubblico
├── tests/
│   ├── AppCanzoniere.Domain.Tests/
│   ├── AppCanzoniere.Application.Tests/
│   └── AppCanzoniere.Infrastructure.Tests/
└── docs/
    ├── plan.md                        # Piano architetturale completo
    └── prompt_iniziale.md             # Requisiti iniziali
```

## 🚀 Setup Iniziale

### Prerequisiti

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Visual Studio 2025 (o VS Code con C# Dev Kit)
- Workload MAUI installati:
  ```bash
  dotnet workload install maui android ios maccatalyst maui-windows
  ```

### Build del Progetto

```bash
# Clone del repository
git clone <repository-url>
cd AppCanzoniere

# Restore dei pacchetti
dotnet restore AppCanzoniere.slnx

# Build della soluzione
dotnet build AppCanzoniere.slnx --configuration Debug

# Run dell'app MAUI (specificare piattaforma)
cd src/AppCanzoniere.MAUI
dotnet build -t:Run -f net10.0-android     # Android
dotnet build -t:Run -f net10.0-ios         # iOS
dotnet build -t:Run -f net10.0-windows     # Windows
dotnet build -t:Run -f net10.0-maccatalyst # macOS
```

## 📦 Pacchetti NuGet Principali

- **Microsoft.EntityFrameworkCore.Sqlite 10.0.2** - Database locale
- **CommunityToolkit.Mvvm 8.4.0** - Pattern MVVM con source generators
- **System.Net.Http.Json** - Client HTTP per AI/OCR services
- **xUnit 3.1.4** - Testing framework

### Pacchetti Futuri
- **Microsoft.Identity.Client** - Autenticazione OneDrive
- **Google.Apis.Drive.v3** - Google Drive integration
- **Tesseract** - OCR locale (attualmente placeholder)
- **Azure.AI.Vision** - OCR cloud

## 🗄️ Database

L'app utilizza **SQLite** come database locale con **Entity Framework Core**.

- **Posizione:** `{AppDataDirectory}/appcanzoniere.db`
- **Migrations:** Applicate automaticamente all'avvio
- **Schema:** Vedi `docs/plan.md` per dettagli completi

### Entità Principali

- **Song** - Canzoni con ChordPro/MusicXML/JSON, metadati, versioning
- **Album** - Raccolte di canzoni con ordinamento
- **Tag** - Tagging generico (genere, stagione, stile)
- **LiturgicalMoment** - Momenti liturgici (Ingresso, Offertorio, Comunione, etc.)
- **BibleReference** - Riferimenti biblici per canzoni
- **PdfAttachment, Mp3Attachment, YouTubeLink** - Media attachments
- **UserSettings** - Preferenze utente (tema, default key, etc.)

### Seed Data Preconfigurati

- **11 Momenti Liturgici:** Ingresso, Kyrie, Gloria, Salmo, Alleluia, Offertorio, Santo, Comunione, Ringraziamento, Finale, Vari
- **15 Tag Predefiniti:** Tradizionale, Contemporaneo, Corale, Solo, Avvento, Quaresima, Pasqua, Natale, Matrimonio, Funerale, etc.

## 🛠️ Stato di Sviluppo

### ✅ Completate: Fasi 1-13 (87%)

**Fase 1-3:** Setup, Database, UI Base  
**Fase 4:** ChordPro parser e trasposizione  
**Fase 5:** Album management con ordinamento  
**Fase 6:** Tag, metadata, riferimenti biblici  
**Fase 7:** AI Chord Suggestions (Perplexity API)  
**Fase 8:** OCR Import (Tesseract/Azure Computer Vision)  
**Fase 9:** Audio Recognition (ACRCloud - 80M+ tracks)  
**Fase 10:** Cloud Sync (LocalStorage, Export/Import DB)  
**Fase 11:** Backend API - ASP.NET Core (JWT, CRUD, Swagger)  
**Fase 12:** Backend Testing - 14 unit tests per API controllers  
**Fase 13:** MAUI Client per Backend API (Auth, Public Songs)

### 🔄 Parzialmente Completate: Fase 14-15

**Fase 14:** Testing & Refinement (40%)
- ✅ SecureStorage per JWT tokens
- ✅ Integration tests setup (12 tests)
- ⚠️ Integration tests debugging pending

**Fase 15:** Offline Mode (70%)
- ✅ OfflineSong entity & SyncStatus enum
- ✅ Repository layer completa
- ✅ DownloadService con limite storage (500MB)
- ⚠️ FileService pending
- ⚠️ UI integration pending
- ⚠️ Database migration pending

### 📋 Da Completare: Fase 16-17

**Fase 16:** Advanced Features (25%)
- ✅ Advanced search infrastructure (20+ filters, fuzzy, pagination)
- ⚠️ UI integration pending (AdvancedSearchPage)
- ⚠️ Batch operations pending
- ⚠️ Dark mode / themes pending
- ⚠️ Chord diagrams pending

**Fase 17:** Production Deployment (0%)
- CI/CD pipeline (GitHub Actions)
- App store packaging & submission
- Final performance tuning & security hardening
- Monitoring & analytics setup
- Documentation finalization

### Statistiche Progetto

- **Progetti:** 12 (.NET: Domain, Application, Infrastructure, MAUI, Backend + test)
- **Test:** 77/77 passati ✅ (+ 12 integration debugging)
- **Righe di codice:** ~15,000+ LOC
- **File sorgente:** 220+ (.cs + .xaml + .yml + .md)
- **Build:** Succeeded ✅
- **CI/CD:** GitHub Actions workflow configured ✅
- **Documentation:** 
  - README.md ✅
  - SETUP_INSTRUCTIONS.md ✅
  - TEST_AND_COMPLETE.md ✅
  - KNOWN_ISSUES.md ✅ (10 issues)
  - FUTURE_IMPLEMENTATIONS.md ✅ (47 features)
- **Entità Domain:** 14 (incl. OfflineSong)
- **Repository:** 5 (incl. OfflineSongRepository)
- **ViewModels:** 12
- **Pages:** 12
- **External Services:** 10 (AI, OCR x2, Audio, Cloud x4, Backend API)
- **Controllers API:** 2 (Account, Songs)
- **Platform:** Android, iOS, Windows, macOS

## 🎯 Funzionalità Principali

### ✅ Implementate

#### Core
- ✅ Database SQLite locale con EF Core 10
- ✅ CRUD completo canzoni
- ✅ Parser ChordPro con transposizione
- ✅ Visualizzazione accordi con transpose real-time
- ✅ Sistema versioning per sync

#### Organizzazione
- ✅ Album e playlist con ordinamento
- ✅ Tag generici e liturgici (15 predefiniti)
- ✅ Riferimenti biblici
- ✅ Momenti liturgici (11 predefiniti)
- ✅ Ricerca con filtri multipli

#### AI & Servizi Esterni
- ✅ Suggerimento accordi AI (Perplexity/Mock)
- ✅ OCR import da immagini (Azure CV/Tesseract/Mock)
- ✅ Disclaimer copyright per AI

#### UI/UX
- ✅ Shell navigation con TabBar
- ✅ MVVM con CommunityToolkit
- ✅ Edit mode in-page
- ✅ Route navigation con parametri

### ⏳ In Roadmap

#### Audio & Media
- ⏳ Riconoscimento audio (ACRCloud)
- ⏳ Riproduzione MP3 attachments
- ⏳ Visualizzazione PDF spartiti

#### Cloud & Sync
- ⏳ Multi-provider sync (OneDrive/Google Drive/Dropbox)
- ⏳ Conflict resolution
- ⏳ Modalità offline con cache
- ⏳ Backup/restore database

#### Backend
- ⏳ ASP.NET Core API
- ⏳ Database pubblico
- ⏳ Autenticazione utenti
- ⏳ Canzoni pubbliche/private

#### Advanced
- ⏳ Parser MusicXML
- ⏳ Parser JSON custom
- ⏳ Tablature chitarra
- ⏳ Raccolte automatiche con AI

## 📝 Convenzioni di Codice

- **Language:** C# 12
- **Style:** Follow [.NET coding conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- **Naming:**
  - PascalCase per classi, metodi, proprietà
  - camelCase per variabili locali e parametri
  - `_camelCase` per campi privati
- **Async:** Sempre usare `async/await` per operazioni I/O
- **Cancellation:** Passare `CancellationToken` dove possibile

## 🧪 Testing

```bash
# Run tutti i test (63 tests)
dotnet test

# Run test per progetto specifico
dotnet test tests/AppCanzoniere.Domain.Tests         # 6 tests
dotnet test tests/AppCanzoniere.Application.Tests    # 46 tests
dotnet test tests/AppCanzoniere.Infrastructure.Tests # 11 tests

# Run test con coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Test Coverage

- **ChordTransposer:** 28 tests (simple chords, complex chords, bass notes, sharp/flat preference)
- **ChordProParser:** 18 tests (parsing, rendering, transposition)
- **Domain Entities:** 6 tests (validation, relationships)
- **Repositories:** 11 tests (CRUD, relationships, ordering)

## 📚 Documentazione

- **[SETUP_INSTRUCTIONS.md](SETUP_INSTRUCTIONS.md)** - Configurazione completa con secrets management
- **[docs/plan.md](docs/plan.md)** - Piano architetturale dettagliato (17 fasi)
- **[docs/prompt_iniziale.md](docs/prompt_iniziale.md)** - Requisiti originali in italiano

### Configurazione Servizi Esterni

Tutti i servizi esterni sono configurabili con Mock services per demo senza API key:

- **AI Chord Suggestions:** Perplexity API (default: MockChordSuggestionService)
- **OCR:** Azure Computer Vision / Tesseract (default: MockOcrService)
- **Audio Recognition:** ACRCloud (default: MockAudioRecognitionService - in sviluppo)

Vedi `SETUP_INSTRUCTIONS.md` per configurazione API keys con User Secrets.

## 📄 Licenza

*Da definire*

## 👥 Contributori

- Alessio Brizi - Sviluppo iniziale

## 📞 Contatti

*Da definire*

---

**Ultimo aggiornamento:** 2026-02-01  
**Versione:** 0.10.0  
**Fase Corrente:** 11/17 (Backend API)  
**Build Status:** ✅ Passing | **Tests:** ✅ 63/63
