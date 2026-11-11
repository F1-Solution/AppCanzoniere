# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

AppCanzoniere is a .NET 10 / C# multi-platform songbook application using Clean Architecture. It has a .NET MAUI frontend (Android, Windows); the ASP.NET Core backend is frozen outside the main solution. Songs are stored in ChordPro format with transposition, OCR, audio recognition, and offline support.

## Build & Test Commands

```bash
# Libraries and tests (no MAUI workload needed)
dotnet test AppCanzoniere.Core.slnx

# Single test project / single test
dotnet test tests/AppCanzoniere.Application.Tests
dotnet test --filter "FullyQualifiedName~TestMethodName"

# MAUI (Android and Windows only; iOS/Mac Catalyst are not targets)
dotnet build src/AppCanzoniere.MAUI -f net10.0-android
dotnet build src/AppCanzoniere.MAUI -f net10.0-windows10.0.19041.0 -p:MauiPlatforms=windows
```

`AppCanzoniere.slnx` is the main solution (core + MAUI, needs the Android and Windows workloads); `AppCanzoniere.Core.slnx` has no MAUI. Package versions are central in `Directory.Packages.props`, the SDK is pinned in `global.json`. CI is `.github/workflows/ci.yml` (jobs core, android, windows). Core tests: 60 (Domain 5, Application 45, Infrastructure 10).

**Backend:** the ASP.NET Core API is frozen and outside the main solution and CI; open it with `AppCanzoniere.Backend.slnx`. It does not work (see `docs/VALUTAZIONE.md`) and its integration tests fail.

## Architecture

Clean Architecture with four layers (dependencies flow inward only):

- **Domain** (`src/AppCanzoniere.Domain`) — Entities (Song, Album, Tag, LiturgicalMoment, etc.), enums, repository interfaces. Zero external dependencies.
- **Application** (`src/AppCanzoniere.Application`) — Services, DTOs, service interfaces. Key services: `ChordProParserService`, `ChordTransposerService`. Depends only on Domain.
- **Infrastructure** (`src/AppCanzoniere.Infrastructure`) — EF Core `AppDbContext` with SQLite, repository implementations, migrations. Implements Application interfaces.
- **Presentation** — Two frontends:
  - **MAUI** (`src/AppCanzoniere.MAUI`) — Pages, ViewModels (MVVM via CommunityToolkit.Mvvm), platform services
  - **Backend** (`src/AppCanzoniere.Backend`) — frozen, not in the main solution or CI

## Key Conventions

- **Language:** C# with nullable references enabled, implicit usings, async/await for all I/O
- **Naming:** `PascalCase` types/methods/properties, `camelCase` variables, `_camelCase` private fields, `I`-prefixed interfaces
- **DI:** All services registered in `MauiProgram.cs` (MAUI) and `Program.cs` (Backend). Repositories are scoped, parsers/transposers are singleton.
- **Testing:** xUnit + FluentAssertions + Moq. Arrange/Act/Assert pattern. In-memory EF Core for repository tests.
- **Database:** SQLite via EF Core 10. The MAUI app creates the schema with `EnsureCreated` (no migrations yet; persistence is redone in a later phase).
- **Anti-patterns to avoid:** Infrastructure dependencies in Domain/Application; fat controllers; anemic domain models; god services.

## External Service Integrations

All external services use interface abstractions with mock implementations as defaults:
- `IChordSuggestionService` — Perplexity AI (mock default)
- `IOcrService` — Azure Computer Vision / Tesseract (mock default)
- `IAudioRecognitionService` — ACRCloud (mock default)
- `ICloudSyncService` — LocalStorage implemented; OneDrive/GoogleDrive/Dropbox are placeholders

API keys are managed via User Secrets (`dotnet user-secrets`) — see `SETUP_INSTRUCTIONS.md` for details.

## Project Status

Restart in progress: the old "87% complete" claims are unreliable. See `ROADMAP.md` for the phase table, `PROGRESS.md` for completed work, `docs/VALUTAZIONE.md` for the real state and `docs/FASI.md` for the plan.
