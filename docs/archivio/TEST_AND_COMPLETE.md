# TEST_AND_COMPLETE.md

**Last Updated:** 2026-02-01 (Phase 17 Complete)  
**Purpose:** Checklist completa di tutti gli step mancanti per le Fasi 14-16

**Status:** Fasi 1-13 + 17 Complete ✅ | Fasi 14-16 Partial ⚠️

---

## ⚠️ OVERVIEW

Questo file documenta tutti gli step NON completati delle Fasi 14, 15, 16.  
La **Fase 17 (Production Deployment - Infrastructure)** è stata completata.

**Progress Complessivo:**
- ✅ Fasi 1-13: Complete (100%)
- ⚠️ Fase 14: 40% Complete (SecureStorage ✅, Integration tests ⚠️)
- ⚠️ Fase 15: 70% Complete (Infrastructure ✅, UI ⚠️)
- ⚠️ Fase 16: 25% Complete (Advanced Search ✅, UI ⚠️)
- ✅ Fase 17: Infrastructure Complete (CI/CD ✅, Docs ✅)

**Documenti Correlati:**
- **KNOWN_ISSUES.md:** 10 problemi noti documentati
- **FUTURE_IMPLEMENTATIONS.md:** 47 future features pianificate
- **README.md:** Panoramica progetto
- **SETUP_INSTRUCTIONS.md:** Configuration guide

---

## 📋 FASE 14: Testing & Refinement (40% Complete)

### ✅ COMPLETATO

- [x] SecureStorage Implementation
  - [x] ITokenStorage interface creata
  - [x] SecureTokenStorage implementato (iOS Keychain, Android KeyStore, Windows DPAPI)
  - [x] AuthService migrato da in-memory a ITokenStorage
  - [x] Token persistono tra riavvii dell'app
  - [x] Registrazione in MauiProgram.cs
  
- [x] Integration Tests Setup
  - [x] Progetto AppCanzoniere.Backend.IntegrationTests creato
  - [x] CustomWebApplicationFactory<Program> configurato
  - [x] Program.cs modificato (Testing environment, public partial class)
  - [x] AccountIntegrationTests.cs (5 tests)
  - [x] SongsIntegrationTests.cs (8 tests)
  - [x] TestDtos.cs creato

### ⚠️ DA COMPLETARE

#### 1. Integration Tests Debugging
- [ ] **CRITICO:** Fix integration tests failures (12 tests failing)
  - [ ] Investigate WebApplicationFactory database setup
  - [ ] Check test isolation and cleanup
  - [ ] Verify Swagger conditional loading works
  - [ ] Check JWT token generation in tests
  - [ ] Debug AccountIntegrationTests failures
  - [ ] Debug SongsIntegrationTests failures
- [ ] Run: `dotnet test tests/AppCanzoniere.Backend.IntegrationTests/`
- [ ] Verify all 12 tests pass ✅

#### 2. Configuration Management
- [ ] Create appsettings.json for MAUI project
- [ ] Move Backend URL from hardcoded to configuration
  - Current: `http://localhost:5091` hardcoded in MauiProgram.cs
  - Target: Load from appsettings.json or environment variable
- [ ] Add IConfiguration dependency to MAUI services
- [ ] Document configuration in SETUP_INSTRUCTIONS.md

#### 3. Token Refresh Mechanism
- [ ] Implement automatic token refresh when near expiration
- [ ] Add RefreshTokenAsync method in ITokenStorage/AuthService
- [ ] Add background service to check token expiration
- [ ] Test token refresh flow

#### 4. End-to-End Tests (MAUI ↔ Backend)
- [ ] Setup E2E test project (optional)
- [ ] Test complete authentication flow
- [ ] Test public song browsing
- [ ] Test song upload/download
- [ ] Test offline sync

#### 5. Performance Testing
- [ ] Measure app startup time
- [ ] Profile database query performance
- [ ] Test with large song datasets (1000+ songs)
- [ ] Measure API response times
- [ ] Memory leak detection

#### 6. Code Coverage Analysis
- [ ] Setup code coverage tool (coverlet)
- [ ] Generate coverage report
- [ ] Target: 80%+ coverage
- [ ] Identify untested code paths

---

## 📋 FASE 15: Modalità Offline (70% Complete)

### ✅ COMPLETATO

- [x] Domain Layer
  - [x] SyncStatus enum (6 stati)
  - [x] OfflineSong entity con BackendSongId (string), sync tracking
  - [x] AppDbContext configuration (DbSet + unique index)
  
- [x] Repository Layer
  - [x] IOfflineSongRepository interface
  - [x] OfflineSongRepository implementation
  - [x] Storage usage calculation (GetTotalStorageUsageAsync)
  - [x] Sync status filtering (GetBySyncStatusAsync)
  - [x] IsDownloadedAsync method
  
- [x] Service Layer
  - [x] IDownloadService interface
  - [x] IFileService interface (abstract)
  - [x] DownloadService implementation
  - [x] Progress tracking con IProgress<double>
  - [x] Storage limit management (500MB)
  - [x] Error handling e retry logic

- [x] Build & Documentation
  - [x] OfflineSongRepository signatures fixed (string BackendSongId)
  - [x] Build succeeded ✅
  - [x] README.md updated
  - [x] plan.md updated
  - [x] SETUP_INSTRUCTIONS.md Section 15 added

### ⚠️ DA COMPLETARE

#### 1. Database Migration
- [ ] **CRITICO:** Generate EF Core migration
  ```bash
  cd src/AppCanzoniere.Infrastructure
  dotnet ef migrations add AddOfflineSongs --startup-project ../AppCanzoniere.MAUI
  ```
- [ ] Apply migration to local database
  ```bash
  dotnet ef database update --startup-project ../AppCanzoniere.MAUI
  ```
- [ ] Verify OfflineSongs table created
- [ ] Test migration rollback

#### 2. FileService Implementation (MAUI Layer)
- [ ] Create `IFileService` implementation: `FileService.cs` in MAUI/Services
- [ ] Implement platform-specific file paths:
  - [ ] Android: `/data/data/[package]/files/offline_songs/`
  - [ ] iOS: `[AppData]/Documents/offline_songs/`
  - [ ] Windows: `%LOCALAPPDATA%/AppCanzoniere/offline_songs/`
  - [ ] macOS: `~/Library/Application Support/AppCanzoniere/offline_songs/`
- [ ] Methods to implement:
  - [ ] `Task<string> GetOfflineSongsFolderAsync()`
  - [ ] `Task<string> SaveSongContentAsync(int songId, string content)`
  - [ ] `Task<string?> ReadSongContentAsync(int songId)`
  - [ ] `Task DeleteSongFilesAsync(int songId)`
  - [ ] `Task<long> GetFolderSizeAsync(string path)`
- [ ] Register in MauiProgram.cs: `services.AddSingleton<IFileService, FileService>()`
- [ ] Test file operations on all platforms

#### 3. Service Registration & DI
- [ ] Register IOfflineSongRepository in MauiProgram.cs
  ```csharp
  services.AddScoped<IOfflineSongRepository, OfflineSongRepository>();
  ```
- [ ] Register IDownloadService in MauiProgram.cs
  ```csharp
  services.AddScoped<IDownloadService, DownloadService>();
  ```
- [ ] Register IFileService (after implementation)
- [ ] Verify dependency injection chain

#### 4. UI Integration - PublicSongsPage
- [ ] Add download button to each song item
- [ ] Add download indicator icon (✓ when downloaded)
- [ ] Show SyncStatus (Downloading, Downloaded, Failed)
- [ ] Implement download progress bar
- [ ] Handle download errors with user feedback
- [ ] Disable download button if storage limit reached
- [ ] Test UI responsiveness during download

#### 5. UI Integration - PublicSongsViewModel
- [ ] Add `ObservableCollection<DownloadStatus>` for tracking
- [ ] Add `DownloadSongCommand` (RelayCommand)
- [ ] Add `CancelDownloadCommand` (RelayCommand)
- [ ] Add `DeleteOfflineSongCommand` (RelayCommand)
- [ ] Implement progress tracking
- [ ] Update UI when download completes
- [ ] Check IsDownloaded status on load

#### 6. UI Integration - SettingsPage
- [ ] Add "Offline Mode" section
- [ ] Display total storage used (MB/GB)
- [ ] Display downloaded songs count
- [ ] Add "Clear all offline songs" button
- [ ] Add storage limit configuration (dropdown: 100MB, 250MB, 500MB, 1GB)
- [ ] Show offline songs list with sizes
- [ ] Add "Manage Offline Songs" navigation

#### 7. Offline Song Display
- [ ] Create OfflineSongsPage.xaml (list of downloaded songs)
- [ ] Create OfflineSongsViewModel
- [ ] Show downloaded songs with sync status
- [ ] Allow viewing offline songs (read from local storage)
- [ ] Handle locally modified songs (indicate changes)
- [ ] Implement "Sync changes" button

#### 8. Conflict Resolution
- [ ] Detect conflicts (LocallyModified + backend updated)
- [ ] Show conflict resolution dialog
- [ ] Options: Keep Local / Use Server / Merge
- [ ] Implement merge strategy (if feasible)
- [ ] Update SyncStatus after resolution

#### 9. Media Caching (PDF/MP3) - OPTIONAL
- [ ] Download PDF attachments
- [ ] Download MP3 audio files
- [ ] Cache images (song covers)
- [ ] Include in storage calculations
- [ ] Display cached media in song view

#### 10. Testing - Offline Mode
- [ ] Unit tests for DownloadService
  - [ ] Test storage limit enforcement
  - [ ] Test progress tracking
  - [ ] Test error handling
- [ ] Unit tests for OfflineSongRepository
  - [ ] Test CRUD operations
  - [ ] Test storage calculations
  - [ ] Test sync status filtering
- [ ] Integration tests
  - [ ] Test download workflow
  - [ ] Test offline song retrieval
  - [ ] Test sync status updates
- [ ] UI tests (manual or automated)
  - [ ] Test download button
  - [ ] Test progress indicator
  - [ ] Test offline song display

---

## 🎯 PRIORITÀ

### 🔴 CRITICI (Must Fix)
1. Integration tests debugging (Fase 14)
2. Database migration per OfflineSong (Fase 15)
3. FileService implementation (Fase 15)

### 🟡 IMPORTANTI (Should Have)
4. UI integration per download (Fase 15)
5. Configuration management (Fase 14)
6. Testing offline mode (Fase 15)

### 🟢 OPZIONALI (Nice to Have)
7. Token refresh mechanism (Fase 14)
8. Performance testing (Fase 14)
9. Media caching (Fase 15)
10. Conflict resolution UI (Fase 15)

---

## 📊 METRICHE DI SUCCESSO

### Fase 14 - Testing & Refinement
- [ ] ✅ 12 integration tests passing
- [ ] ✅ Backend URL configurable (not hardcoded)
- [ ] ✅ Token refresh implemented
- [ ] ✅ Code coverage > 80%
- [ ] ✅ No performance regressions

### Fase 15 - Modalità Offline
- [ ] ✅ Database migration applied
- [ ] ✅ FileService implemented for all platforms
- [ ] ✅ Download workflow functional
- [ ] ✅ UI shows download status
- [ ] ✅ Offline songs accessible without internet
- [ ] ✅ Storage limit enforced
- [ ] ✅ Unit tests for download/repository (80%+ coverage)

---

## 📝 NOTE TECNICHE

### Fase 14
- Integration tests usano in-memory database (Testing environment)
- Swagger disabled in Testing environment to avoid TypeLoadException
- SecureStorage funziona out-of-the-box, no extra config needed

### Fase 15
- BackendSongId stored as string (handles Guid from backend)
- SyncStatus lifecycle: OnlineOnly → Downloading → Downloaded → LocallyModified
- Storage calculations include estimated text size (content × 2 bytes)
- DownloadService uses IProgress<double> for 0-100% tracking
- OfflineSong.Lyrics maps to PublicSongDto.ChordProContent

---

## 🚀 COMANDI UTILI

### Test Integration Tests
```bash
cd C:\Progetti\AppCanzoniere
dotnet test tests/AppCanzoniere.Backend.IntegrationTests/ --verbosity normal
```

### Generate Migration
```bash
cd src/AppCanzoniere.Infrastructure
dotnet ef migrations add AddOfflineSongs --startup-project ../AppCanzoniere.MAUI
```

### Apply Migration
```bash
cd src/AppCanzoniere.Infrastructure
dotnet ef database update --startup-project ../AppCanzoniere.MAUI
```

### Run Backend
```bash
cd src/AppCanzoniere.Backend
dotnet run
```

### Run MAUI App (Android)
```bash
cd src/AppCanzoniere.MAUI
dotnet build -t:Run -f net10.0-android
```

### Code Coverage (if using coverlet)
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

---

## ✅ QUANDO COMPLETARE

**Avvisare l'utente quando:**
- [ ] Tutti i 12 integration tests passano
- [ ] Database migration applicata con successo
- [ ] FileService implementato e testato
- [ ] Download workflow funzionante end-to-end
- [ ] UI completa per offline mode

**Solo allora procedere con Fase 16.**

---

_File creato per tracciare gli step mancanti delle Fasi 14-15.  
Non eliminare fino al completamento di tutti i task._
