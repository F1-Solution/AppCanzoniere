# AppCanzoniere - Setup Instructions

This guide covers all configuration and secrets management for the AppCanzoniere project.

## Table of Contents
1. [Security Best Practices](#security-best-practices)
2. [Development Environment Setup](#development-environment-setup)
3. [AI Services Configuration](#ai-services-configuration)
4. [OCR Services Configuration](#ocr-services-configuration)
5. [Audio Recognition Configuration](#audio-recognition-configuration)
6. [Cloud Storage Configuration](#cloud-storage-configuration)
7. [Backend API Configuration](#backend-api-configuration)
8. [Database Configuration](#database-configuration)

---

## Security Best Practices

### ⚠️ NEVER commit secrets to Git!

All sensitive information must be stored using one of these methods:

1. **User Secrets** (recommended for development)
2. **Environment Variables** (for production/CI)
3. **Azure Key Vault** (for production)
4. **Local `.env` files** (excluded from Git)

### Files Already in .gitignore

The following patterns are automatically excluded:
- `appsettings.*.json` (except appsettings.json template)
- `*.env`
- `.env.*`
- `secrets.json`
- `*.secret.*`
- `bin/` and `obj/`

---

## Development Environment Setup

### Prerequisites

- .NET 10 SDK (10.0.102 or later)
- Visual Studio 2025 or VS Code with .NET MAUI workload
- Git
- (Optional) Perplexity AI API key
- (Optional) Azure Computer Vision API key

### Initial Setup

```bash
# Clone repository
git clone <repository-url>
cd AppCanzoniere

# Restore packages
dotnet restore

# Run migrations
cd src/AppCanzoniere.MAUI
dotnet ef database update --project ../AppCanzoniere.Infrastructure

# Build solution
dotnet build
```

---

## AI Services Configuration

### Perplexity AI (Chord Suggestions)

**Default:** Uses `MockChordSuggestionService` (no API key required)

**To use real API:**

#### Option 1: Environment Variable (Recommended)

```bash
# Windows PowerShell
$env:PERPLEXITY_API_KEY = "pplx-your-api-key-here"

# Windows CMD
set PERPLEXITY_API_KEY=pplx-your-api-key-here

# Linux/macOS
export PERPLEXITY_API_KEY=pplx-your-api-key-here
```

#### Option 2: User Secrets (MAUI Project)

```bash
cd src/AppCanzoniere.MAUI
dotnet user-secrets init
dotnet user-secrets set "AI:PerplexityApiKey" "pplx-your-api-key-here"
```

#### Enable Real Service

Edit `src/AppCanzoniere.MAUI/MauiProgram.cs`:

```csharp
// Comment this line:
// builder.Services.AddSingleton<IChordSuggestionService, MockChordSuggestionService>();

// Uncomment this line:
builder.Services.AddSingleton<IChordSuggestionService, PerplexityChordSuggestionService>();
```

**Get API Key:** https://www.perplexity.ai/settings/api

**Pricing:** ~$0.20 per 1M tokens (model: llama-3.1-sonar-small-128k-online)

---

## OCR Services Configuration

### Tesseract OCR (Local, Free)

**Windows:**
```bash
# Install via Chocolatey
choco install tesseract

# Or download from: https://github.com/UB-Mannheim/tesseract/wiki
# Set environment variable
$env:TESSDATA_PREFIX = "C:\Program Files\Tesseract-OCR\tessdata"
```

**Linux:**
```bash
sudo apt-get install tesseract-ocr
sudo apt-get install tesseract-ocr-ita  # Italian language pack
```

**Configuration:**
```bash
cd src/AppCanzoniere.MAUI
dotnet user-secrets set "OCR:TesseractPath" "C:\Program Files\Tesseract-OCR\tesseract.exe"
dotnet user-secrets set "OCR:TessdataPath" "C:\Program Files\Tesseract-OCR\tessdata"
```

### Azure Computer Vision (Cloud, Paid)

**Setup:**
1. Create Azure Computer Vision resource: https://portal.azure.com
2. Get API key and endpoint from "Keys and Endpoint" section

**Environment Variables:**
```bash
$env:AZURE_COMPUTER_VISION_KEY = "your-azure-key-here"
$env:AZURE_COMPUTER_VISION_ENDPOINT = "https://your-resource.cognitiveservices.azure.com/"
```

**User Secrets:**
```bash
cd src/AppCanzoniere.MAUI
dotnet user-secrets set "OCR:AzureComputerVision:Key" "your-azure-key"
dotnet user-secrets set "OCR:AzureComputerVision:Endpoint" "https://your-resource.cognitiveservices.azure.com/"
```

**Pricing:** Free tier: 5,000 transactions/month

---

## Audio Recognition Configuration

### ACRCloud (Audio Fingerprinting)

**Default:** Uses `MockAudioRecognitionService` (no API key required)

**To use real API:**

#### Setup ACRCloud Account

1. Register at: https://www.acrcloud.com
2. Create a new project (Music Recognition)
3. Get credentials from Console → Projects → Your Project

#### Option 1: Environment Variables (Recommended)

```bash
# Windows PowerShell
$env:ACRCLOUD_ACCESS_KEY = "your-access-key-here"
$env:ACRCLOUD_ACCESS_SECRET = "your-access-secret-here"
$env:ACRCLOUD_HOST = "identify-eu-west-1.acrcloud.com"

# Linux/macOS
export ACRCLOUD_ACCESS_KEY=your-access-key-here
export ACRCLOUD_ACCESS_SECRET=your-access-secret-here
export ACRCLOUD_HOST=identify-eu-west-1.acrcloud.com
```

#### Option 2: User Secrets (MAUI Project)

```bash
cd src/AppCanzoniere.MAUI
dotnet user-secrets set "Audio:ACRCloud:AccessKey" "your-access-key"
dotnet user-secrets set "Audio:ACRCloud:AccessSecret" "your-access-secret"
dotnet user-secrets set "Audio:ACRCloud:Host" "identify-eu-west-1.acrcloud.com"
```

#### Enable Real Service

Edit `src/AppCanzoniere.MAUI/MauiProgram.cs`:

```csharp
// Comment this line:
// builder.Services.AddSingleton<IAudioRecognitionService, MockAudioRecognitionService>();

// Uncomment this line:
builder.Services.AddSingleton<IAudioRecognitionService, ACRCloudAudioRecognitionService>();
```

**Pricing:**
- Free tier: 2,000 recognitions/month
- Music database: 80+ million tracks
- Response time: < 3 seconds

**Supported Formats:** WAV, MP3, AAC, FLAC (16-bit PCM recommended for best results)

---

## Cloud Storage Configuration

### OneDrive

**OAuth 2.0 Configuration:**

1. Register app at: https://portal.azure.com/#blade/Microsoft_AAD_RegisteredApps
2. Get Client ID and Client Secret
3. Set redirect URI: `msal{ClientId}://auth`

```bash
cd src/AppCanzoniere.MAUI
dotnet user-secrets set "CloudStorage:OneDrive:ClientId" "your-client-id"
dotnet user-secrets set "CloudStorage:OneDrive:ClientSecret" "your-client-secret"
dotnet user-secrets set "CloudStorage:OneDrive:RedirectUri" "msalyour-client-id://auth"
```

### Google Drive

**OAuth 2.0 Configuration:**

1. Create project at: https://console.cloud.google.com
2. Enable Google Drive API
3. Create OAuth 2.0 credentials
4. Download `credentials.json`

```bash
cd src/AppCanzoniere.MAUI
dotnet user-secrets set "CloudStorage:GoogleDrive:ClientId" "your-client-id"
dotnet user-secrets set "CloudStorage:GoogleDrive:ClientSecret" "your-client-secret"
dotnet user-secrets set "CloudStorage:GoogleDrive:RedirectUri" "com.googleusercontent.apps.your-client-id:/oauth2redirect"
```

### Dropbox

**OAuth 2.0 Configuration:**

1. Create app at: https://www.dropbox.com/developers/apps
2. Get App Key and App Secret

```bash
cd src/AppCanzoniere.MAUI
dotnet user-secrets set "CloudStorage:Dropbox:AppKey" "your-app-key"
dotnet user-secrets set "CloudStorage:Dropbox:AppSecret" "your-app-secret"
dotnet user-secrets set "CloudStorage:Dropbox:RedirectUri" "com.appcanzoniere://auth"
```

---

## Backend API Configuration (Phase 11 ✅)

### Connection Strings

The backend uses SQLite by default. Configuration already set in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=appcanzoniere_backend.db"
  }
}
```

**For Production SQL Server:**
```bash
cd src/AppCanzoniere.Backend
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=your-server.database.windows.net;Database=AppCanzoniere;User Id=your-username;Password=your-password;Encrypt=True;"
```

### JWT Configuration (REQUIRED FOR PRODUCTION)

**Generate a strong secret key (minimum 32 characters):**
```powershell
# PowerShell
[Convert]::ToBase64String((1..64 | ForEach-Object { Get-Random -Minimum 0 -Maximum 256 }))
```

**Set with user secrets:**
```bash
cd src/AppCanzoniere.Backend
dotnet user-secrets init
dotnet user-secrets set "Jwt:SecretKey" "your-generated-secret-key-min-32-chars"
dotnet user-secrets set "Jwt:Issuer" "https://appcanzoniere.com"
dotnet user-secrets set "Jwt:Audience" "https://appcanzoniere.com"
dotnet user-secrets set "Jwt:ExpirationMinutes" "60"
```

**Default appsettings.json template:**
```json
{
  "Jwt": {
    "SecretKey": "REPLACE-WITH-USER-SECRETS-32-CHARS-MINIMUM",
    "Issuer": "https://appcanzoniere.com",
    "Audience": "https://appcanzoniere.com",
    "ExpirationMinutes": 60
  }
}
```

### Default Admin User

On first run, the backend seeds:
- **Email:** admin@appcanzoniere.com
- **Password:** Admin123!
- **Role:** Admin

**⚠️ CHANGE THIS PASSWORD IN PRODUCTION!**

### Run Backend API

```bash
cd src/AppCanzoniere.Backend
dotnet run
```

Access Swagger UI at: https://localhost:5001/swagger

### Test Backend API

```bash
# Register new user
curl -X POST https://localhost:5001/api/account/register \
  -H "Content-Type: application/json" \
  -d '{"email":"test@test.com","password":"Test123!","confirmPassword":"Test123!"}'

# Login
curl -X POST https://localhost:5001/api/account/login \
  -H "Content-Type: application/json" \
  -d '{"email":"test@test.com","password":"Test123!"}'

# Get public songs (no auth required)
curl https://localhost:5001/api/songs

# Create song (requires auth - use token from login)
curl -X POST https://localhost:5001/api/songs \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d '{"title":"Amazing Grace","artist":"John Newton","isPublic":true}'
```

---

## 12. Testing Configuration (Phase 12) ✅

### Backend Unit Tests

Create test project:
```bash
cd tests
dotnet new xunit -n AppCanzoniere.Backend.Tests
dotnet sln ../../AppCanzoniere.slnx add AppCanzoniere.Backend.Tests/AppCanzoniere.Backend.Tests.csproj
```

Required NuGet packages:
```bash
cd AppCanzoniere.Backend.Tests
dotnet add package Microsoft.AspNetCore.Mvc.Testing
dotnet add package Microsoft.EntityFrameworkCore.InMemory
dotnet add package Moq
dotnet add package FluentAssertions
```

Run tests:
```bash
dotnet test
```

---

## Database Configuration
}
```

---

## Database Configuration

### MAUI App (SQLite)

**Default location:** `%LOCALAPPDATA%/AppCanzoniere/appcanzoniere.db`

**No configuration needed** - database is created automatically on first run.

**Reset database:**
```bash
# Delete database file
rm "$env:LOCALAPPDATA\AppCanzoniere\appcanzoniere.db"
```

### 14.2 Backend API (SQLite)
rm "$env:LOCALAPPDATA/AppCanzoniere/appcanzoniere.db"

# Rebuild migrations
cd src/AppCanzoniere.Infrastructure
dotnet ef migrations remove --startup-project ../AppCanzoniere.MAUI
dotnet ef migrations add InitialCreate --startup-project ../AppCanzoniere.MAUI
dotnet ef database update --startup-project ../AppCanzoniere.MAUI
```

### Backend API (SQL Server/SQLite)

**Run Migrations:**
```bash
cd src/AppCanzoniere.Backend
dotnet ef database update --project ../AppCanzoniere.Infrastructure
```

---

## Environment-Specific Configuration

### Development (.env file)

Create `src/AppCanzoniere.MAUI/.env.development` (excluded from Git):

```env
PERPLEXITY_API_KEY=pplx-dev-key-here
AZURE_COMPUTER_VISION_KEY=azure-dev-key-here
AZURE_COMPUTER_VISION_ENDPOINT=https://dev-resource.cognitiveservices.azure.com/
TESSDATA_PREFIX=C:\Program Files\Tesseract-OCR\tessdata
```

### Production (Azure App Service)

**Set in Azure Portal:**
1. Go to App Service → Configuration → Application Settings
2. Add each secret as a separate setting
3. Enable "Deployment slot setting" for environment-specific values

**Example:**
```
Name: PERPLEXITY_API_KEY
Value: pplx-prod-key-here
Deployment slot setting: ✓
```

---

## 14. SecureStorage Configuration (Phase 14)

### Overview
Phase 14 introduced secure token storage using platform-specific secure storage APIs instead of in-memory variables.

### Implementation
- **MAUI SecureStorage API**: `SecureStorage.Default`
- **iOS**: Keychain
- **Android**: KeyStore
- **Windows**: DPAPI (Data Protection API)
- **macOS**: Keychain

### No Configuration Required
SecureStorage works out-of-the-box on all platforms. Tokens are automatically persisted securely and survive app restarts.

### Usage in Code
```csharp
// Storing tokens
await SecureStorage.Default.SetAsync("jwt_token", token);
await SecureStorage.Default.SetAsync("jwt_expiration", expiration.ToString("O"));

// Retrieving tokens
var token = await SecureStorage.Default.GetAsync("jwt_token");
var expirationStr = await SecureStorage.Default.GetAsync("jwt_expiration");
```

### Clearing Tokens (Logout)
```csharp
SecureStorage.Default.RemoveAll();
```

---

## 15. Offline Mode Configuration (Phase 15)

### Overview
Phase 15 implements offline mode with local song caching and sync status management.

### Database Migration

**Generate migration:**
```bash
cd src/AppCanzoniere.Infrastructure
dotnet ef migrations add AddOfflineSongs --startup-project ../AppCanzoniere.MAUI
```

**Apply migration:**
```bash
dotnet ef database update --startup-project ../AppCanzoniere.MAUI
```

### Storage Configuration

**Default Storage Limit:** 500MB  
Location: `DownloadService.MaxStorageBytes` constant

**Modify storage limit:**
```csharp
// In DownloadService.cs
private const long MaxStorageBytes = 500 * 1024 * 1024; // Change as needed
```

### File Paths (Platform-Specific)

Downloaded songs are stored in:
- **Android**: `/data/data/[package]/files/offline_songs/`
- **iOS**: `[AppData]/Documents/offline_songs/`
- **Windows**: `%LOCALAPPDATA%/AppCanzoniere/offline_songs/`
- **macOS**: `~/Library/Application Support/AppCanzoniere/offline_songs/`

### Sync Status States

1. **OnlineOnly**: Not downloaded, requires internet
2. **Downloading**: Download in progress
3. **Downloaded**: Available offline
4. **LocallyModified**: User made local changes
5. **DownloadFailed**: Download error occurred
6. **Conflict**: Server and local versions differ

### Features Implemented (Phase 15 - 70%)

✅ **Completed:**
- OfflineSong entity with BackendSongId mapping
- SyncStatus enum for tracking download state
- OfflineSongRepository with storage calculations
- IDownloadService interface
- DownloadService with progress tracking
- Storage limit management (500MB default)

⚠️ **Pending:**
- FileService MAUI implementation
- UI download indicators
- Database migration generation
- Media caching (PDF/MP3)
- Conflict resolution UI

### Testing Offline Mode

**Check storage usage:**
```csharp
var usedBytes = await _offlineSongRepository.GetTotalStorageUsageAsync();
var downloadedCount = await _offlineSongRepository.GetDownloadedCountAsync();
```

**Download a song:**
```csharp
var progress = new Progress<double>(percent => {
    Console.WriteLine($"Download progress: {percent:F1}%");
});
await _downloadService.DownloadSongAsync("SongTitle", progress);
```

---

## Verification Checklist

### Before First Run:

- [ ] .NET 10 SDK installed
- [ ] MAUI workload installed
- [ ] All packages restored (`dotnet restore`)
- [ ] Database migrations applied
- [ ] .gitignore includes all secret patterns

### For AI Features:

- [ ] Perplexity API key configured (or using Mock)
- [ ] Service registered in `MauiProgram.cs`

### For OCR Features:

- [ ] Tesseract installed and path configured
- [ ] OR Azure Computer Vision key configured
- [ ] Language packs installed (for Tesseract)

### For Audio Recognition:

- [ ] ACRCloud account registered (or using Mock)
- [ ] Access key and secret configured
- [ ] Host endpoint configured
- [ ] Microphone permissions granted on mobile

### For Cloud Sync:

- [ ] OAuth apps registered
- [ ] Client IDs and secrets configured
- [ ] Redirect URIs match platform requirements

### For Backend API:

- [ ] Connection string configured
- [ ] JWT secret key generated (min 32 chars)
- [ ] CORS origins configured
- [ ] Database migrations applied

### For SecureStorage (Phase 14):

- [ ] ITokenStorage registered in MauiProgram.cs
- [ ] AuthService using ITokenStorage dependency
- [ ] Tokens persist across app restarts ✅

### For Offline Mode (Phase 15):

- [ ] OfflineSong migration generated
- [ ] OfflineSong migration applied
- [ ] FileService implemented (MAUI layer)
- [ ] Storage paths accessible on target platform
- [ ] Download UI integrated (PublicSongsPage)

---

## Troubleshooting

### "API key not found"
- Check environment variable is set in current session
- Verify user secrets: `dotnet user-secrets list`
- Restart IDE/terminal after setting environment variables

### "Database not found"
- Run: `dotnet ef database update`
- Check database path in AppDbContext
- Verify SQLite permissions

### "Tesseract not found"
- Verify installation: `tesseract --version`
- Check TESSDATA_PREFIX environment variable
- Ensure tesseract.exe is in PATH

### "OAuth redirect failed"
- Verify redirect URI matches registered value
- Check platform-specific URI schemes (Android/iOS)
- Ensure app is registered with correct bundle ID

---

## Security Reminders

✅ **DO:**
- Use User Secrets for local development
- Use Environment Variables for CI/CD
- Use Azure Key Vault for production
- Rotate API keys regularly
- Use separate keys for dev/staging/production

❌ **DON'T:**
- Commit appsettings.Development.json
- Commit .env files
- Share API keys in chat/email
- Use production keys in development
- Store secrets in source code

---

## Support

For issues or questions:
- Check **KNOWN_ISSUES.md** first for documented problems
- Check GitHub Issues: [repository-url]/issues
- Documentation: `docs/`, `README.md`, `FUTURE_IMPLEMENTATIONS.md`
- Contact: [your-email]

---

## 16. CI/CD Configuration (Phase 17) ✅

### GitHub Actions Workflow

The project includes an automated CI/CD pipeline at `.github/workflows/build-and-test.yml`.

**Features:**
- ✅ Automatic build on push/pull request
- ✅ Multi-platform builds (Android, iOS, Windows, macOS, Backend)
- ✅ Unit test execution
- ✅ Integration test execution
- ✅ Artifact uploads for distribution
- ✅ Code coverage reporting (optional)

**Trigger Events:**
- Push to `main` or `develop` branches
- Pull requests to `main` or `develop`
- Manual workflow dispatch

**Artifacts Generated:**
- Android APK (`android-apk`)
- Windows MSIX (`windows-msix`)
- iOS build (`ios-build`)
- macOS build (`macos-build`)
- Backend API (`backend-api`)

### Local Build Commands

```bash
# Build all platforms locally
dotnet build AppCanzoniere.slnx --configuration Release

# Build specific platform
dotnet build src/AppCanzoniere.MAUI/AppCanzoniere.MAUI.csproj -f net10.0-android -c Release
dotnet build src/AppCanzoniere.MAUI/AppCanzoniere.MAUI.csproj -f net10.0-windows10.0.19041.0 -c Release

# Run tests
dotnet test --configuration Release
```

### Signing Configuration (Production)

**Android:**
```bash
# Generate keystore
keytool -genkey -v -keystore appcanzoniere.keystore -alias appcanzoniere -keyalg RSA -keysize 2048 -validity 10000

# Add to GitHub Secrets:
# ANDROID_KEYSTORE_FILE (base64 encoded)
# ANDROID_KEYSTORE_PASSWORD
# ANDROID_KEY_ALIAS
# ANDROID_KEY_PASSWORD
```

**iOS:**
- Requires Apple Developer account ($99/year)
- Setup in Apple Developer portal
- Add signing certificate to GitHub Secrets

**Windows:**
- Generate code signing certificate
- Store in GitHub Secrets for automated signing

---

Last updated: 2026-02-01 (Phase 17 - v0.17.0-alpha)
