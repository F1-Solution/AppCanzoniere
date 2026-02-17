# Known Issues - AppCanzoniere

**Version:** 0.17.0-alpha  
**Last Updated:** 2026-02-01

This document tracks known issues, limitations, and bugs in the current version of AppCanzoniere.

---

## 🔴 Critical Issues

### 1. Integration Tests Failures (Phase 14)
**Status:** ⚠️ Not Fixed  
**Affected Component:** Backend Integration Tests  
**Description:**  
12 integration tests in `AppCanzoniere.Backend.IntegrationTests` are failing. Tests were created but not debugged.

**Impact:**  
- Cannot verify backend API correctness automatically
- Manual testing required for backend changes

**Workaround:**  
Manual testing of backend endpoints via Swagger UI

**Files:**
- `tests/AppCanzoniere.Backend.IntegrationTests/AccountIntegrationTests.cs` (5 tests)
- `tests/AppCanzoniere.Backend.IntegrationTests/SongsIntegrationTests.cs` (8 tests)

**See:** `TEST_AND_COMPLETE.md` Section: Fase 14.1

---

### 2. Offline Mode Database Migration Not Applied (Phase 15)
**Status:** ⚠️ Not Applied  
**Affected Component:** Offline Mode  
**Description:**  
The `OfflineSongs` table migration has been generated but not applied to the database.

**Impact:**  
- Offline mode download functionality cannot work
- App will crash if attempting to download songs offline

**Workaround:**  
None - feature is non-functional until migration is applied

**Fix Required:**
```bash
cd src/AppCanzoniere.Infrastructure
dotnet ef migrations add AddOfflineSongs --startup-project ../AppCanzoniere.MAUI
dotnet ef database update --startup-project ../AppCanzoniere.MAUI
```

**See:** `TEST_AND_COMPLETE.md` Section: Fase 15.1

---

### 3. FileService Not Implemented (Phase 15)
**Status:** ⚠️ Missing Implementation  
**Affected Component:** Offline Mode  
**Description:**  
`IFileService` interface exists but MAUI implementation is missing. Required for saving offline songs to disk.

**Impact:**  
- Cannot save downloaded songs to local storage
- Offline mode incomplete

**Workaround:**  
None

**Fix Required:**  
Implement `FileService.cs` in `AppCanzoniere.MAUI/Services/` with platform-specific paths

**See:** `TEST_AND_COMPLETE.md` Section: Fase 15.2

---

## 🟡 Major Issues

### 4. Backend URL Hardcoded (Phase 14)
**Status:** ⚠️ Not Fixed  
**Affected Component:** MAUI Client  
**Description:**  
Backend API URL is hardcoded as `http://localhost:5091` in `MauiProgram.cs`. Should be configurable.

**Impact:**  
- Cannot easily switch between dev/staging/production environments
- Requires recompilation to change backend URL

**Workaround:**  
Manually edit `MauiProgram.cs` before building

**Fix Required:**  
Add configuration support (appsettings.json or environment variables)

**See:** `TEST_AND_COMPLETE.md` Section: Fase 14.2

---

### 5. No Token Refresh Mechanism (Phase 14)
**Status:** ⚠️ Not Implemented  
**Affected Component:** Authentication  
**Description:**  
JWT tokens expire after a set time but there's no automatic refresh. Users must re-login when token expires.

**Impact:**  
- Poor UX: users logged out unexpectedly
- Active users interrupted

**Workaround:**  
Users must manually re-login

**Fix Required:**  
Implement automatic token refresh when near expiration

**See:** `TEST_AND_COMPLETE.md` Section: Fase 14.3

---

### 6. Offline Mode UI Missing (Phase 15)
**Status:** ⚠️ Not Implemented  
**Affected Component:** User Interface  
**Description:**  
Download buttons, indicators, progress bars, and offline song management UI are missing.

**Impact:**  
- Users cannot interact with offline mode features
- Infrastructure exists but no user-facing functionality

**Workaround:**  
None

**Fix Required:**  
Create UI components: AdvancedSearchPage, OfflineSongsPage, download buttons

**See:** `TEST_AND_COMPLETE.md` Section: Fase 15.4-15.7

---

## 🟢 Minor Issues

### 7. Advanced Search UI Missing (Phase 16)
**Status:** ⚠️ Not Implemented  
**Affected Component:** Search Feature  
**Description:**  
Advanced search infrastructure complete but UI page not created. Feature accessible only via code.

**Impact:**  
- Users cannot use advanced search from UI
- Feature exists but not user-facing

**Workaround:**  
Use basic search in existing UI

**Fix Required:**  
Create `AdvancedSearchPage.xaml` and `AdvancedSearchViewModel`

---

### 8. Dark Mode Not Implemented (Phase 16)
**Status:** ⚠️ Not Implemented  
**Affected Component:** UI Themes  
**Description:**  
No dark mode support. App uses light theme only.

**Impact:**  
- Poor UX in low-light conditions
- Accessibility issue for some users

**Workaround:**  
Use device accessibility features (invert colors)

**Fix Required:**  
Create dark theme ResourceDictionary and theme switcher

---

### 9. No Code Coverage Reporting (Phase 14)
**Status:** ⚠️ Not Configured  
**Affected Component:** Testing  
**Description:**  
No code coverage tool configured. Cannot measure test coverage percentage.

**Impact:**  
- Unknown test coverage
- Cannot identify untested code paths

**Workaround:**  
Manual code review

**Fix Required:**  
Setup coverlet and generate reports

**See:** `TEST_AND_COMPLETE.md` Section: Fase 14.6

---

### 10. Fuzzy Search Not Optimized (Phase 16)
**Status:** ⚠️ Naive Implementation  
**Affected Component:** Advanced Search  
**Description:**  
Fuzzy search uses simple `Contains()` with `ToLower()`. Not true Levenshtein distance.

**Impact:**  
- Less accurate fuzzy matching
- Performance not optimal for large datasets

**Workaround:**  
Use exact search

**Fix Required:**  
Implement proper fuzzy matching algorithm (e.g., FuzzySharp library)

---

## 📋 Limitations

### Database
- **SQLite only**: No support for SQL Server, PostgreSQL, etc.
- **No encryption**: Local database not encrypted (except via OS-level encryption)
- **No multi-user**: Single-user app, no concurrent access handling

### Offline Mode
- **No conflict resolution**: If song modified both locally and on server, no merge UI
- **No media download**: PDF/MP3 attachments not cached offline yet
- **Storage limit**: Hardcoded 500MB limit (not user-configurable from UI)

### Backend API
- **No rate limiting**: API not protected against abuse
- **No pagination**: All songs returned at once (could be slow with many songs)
- **No API versioning**: Breaking changes would affect all clients

### Synchronization
- **No real-time sync**: Changes not pushed automatically
- **Manual sync only**: User must trigger sync explicitly
- **No conflict detection**: Last-write-wins strategy

### External Services
- **API keys in config**: Not obfuscated (stored in user secrets/env vars)
- **No retry logic**: Failed API calls not automatically retried (except in DownloadService)
- **No caching**: OCR/AI results not cached

### Multilanguage (Prompt Iniziale Request)
- **UI only in English**: No Italian/multi-language support yet
- **No localization**: .resx files not configured
- **Song language**: Tracked but not used for UI filtering

### Copyright & Compliance (Prompt Iniziale Request)
- **No Terms & Conditions UI**: User not prompted to accept terms
- **No upload disclaimer**: No explicit "I own the rights" checkbox
- **CopyrightStatus tracked**: But no enforcement or validation
- **No Privacy Policy**: GDPR compliance not implemented

### AI Collections (Prompt Iniziale Request)
- **No automatic playlists**: AI-based collections not implemented
- **Manual collections only**: User must create albums manually
- **No clustering algorithm**: ML.NET integration not done

### Scraping (Prompt Iniziale Request - Intentionally Limited)
- **No web scraping**: Intentionally not implemented due to legal concerns
- **Manual import only**: Users must copy-paste or use OCR
- **No browser extension**: Chrome/Firefox extension not available

---

## 🔧 Platform-Specific Issues

### Android
- **Min SDK**: Requires Android 7.0+ (API 24)
- **Permissions**: Microphone permission required for audio recognition (must be granted manually)

### iOS
- **Signing**: Requires Apple Developer account for device testing
- **Keychain**: SecureStorage requires iCloud enabled in capabilities

### Windows
- **DPAPI**: SecureStorage uses Windows DPAPI (user-profile specific)
- **MSIX**: Packaging requires Windows SDK 19041+

### macOS
- **Catalyst**: Mac Catalyst has some UI limitations vs native AppKit
- **Notarization**: Requires Apple Developer account for distribution

---

## 📊 Test Status

- **Unit Tests:** 77/77 passing ✅
- **Integration Tests:** 0/12 passing ⚠️ (needs debugging)
- **E2E Tests:** None created
- **UI Tests:** None created
- **Performance Tests:** None created

---

## 🔍 Security Considerations

### Current Implementation
- ✅ JWT tokens stored in SecureStorage (platform-specific)
- ✅ HTTPS enforced for backend communication
- ✅ User secrets not committed to Git
- ⚠️ API keys not obfuscated in compiled app
- ⚠️ No certificate pinning
- ⚠️ No SQL injection protection audit

### Recommendations
1. Implement API key obfuscation (ProGuard for Android)
2. Add certificate pinning for backend API
3. Audit all user input validation
4. Implement rate limiting on backend
5. Add penetration testing before production

---

## 📝 Reported Bugs

**None reported yet** - app in alpha testing

To report a bug:
1. Check this document first
2. Create GitHub issue with template
3. Include: version, platform, steps to reproduce, expected vs actual behavior

---

## ℹ️ Notes

- This is an **alpha release** (v0.17.0-alpha)
- Many features are infrastructure-ready but UI-incomplete
- See `TEST_AND_COMPLETE.md` for full checklist of pending work
- See `FUTURE_IMPLEMENTATIONS.md` for planned features

---

_Last updated: 2026-02-01_
