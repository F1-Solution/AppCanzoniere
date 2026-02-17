# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

AppCanzoniere is a .NET 10 / C# multi-platform songbook application using Clean Architecture. It has a .NET MAUI frontend (Android, iOS, Windows, macOS) and an ASP.NET Core REST API backend. Songs are stored in ChordPro format with transposition, OCR, audio recognition, and offline support.

## Build & Test Commands

```bash
# Restore and build
dotnet restore AppCanzoniere.slnx
dotnet build AppCanzoniere.slnx

# Run all unit tests (77 passing)
dotnet test

# Run a single test project
dotnet test tests/AppCanzoniere.Application.Tests
dotnet test tests/AppCanzoniere.Domain.Tests
dotnet test tests/AppCanzoniere.Infrastructure.Tests
dotnet test tests/AppCanzoniere.Backend.Tests

# Run a specific test by name
dotnet test --filter "FullyQualifiedName~TestMethodName"

# Run backend API
dotnet run --project src/AppCanzoniere.Backend

# MAUI platform-specific build
dotnet build src/AppCanzoniere.MAUI -f net10.0-android
dotnet build src/AppCanzoniere.MAUI -f net10.0-windows10.0.19041.0
```

**Note:** Integration tests (`AppCanzoniere.Backend.IntegrationTests`) are currently failing due to WebApplicationFactory setup issues — see `KNOWN_ISSUES.md`.

## Architecture

Clean Architecture with four layers (dependencies flow inward only):

- **Domain** (`src/AppCanzoniere.Domain`) — Entities (Song, Album, Tag, LiturgicalMoment, etc.), enums, repository interfaces. Zero external dependencies.
- **Application** (`src/AppCanzoniere.Application`) — Services, DTOs, service interfaces. Key services: `ChordProParserService`, `ChordTransposerService`. Depends only on Domain.
- **Infrastructure** (`src/AppCanzoniere.Infrastructure`) — EF Core `AppDbContext` with SQLite, repository implementations, migrations. Implements Application interfaces.
- **Presentation** — Two frontends:
  - **MAUI** (`src/AppCanzoniere.MAUI`) — Pages, ViewModels (MVVM via CommunityToolkit.Mvvm), platform services
  - **Backend** (`src/AppCanzoniere.Backend`) — ASP.NET Core controllers, JWT auth (ASP.NET Core Identity), Swagger

## Key Conventions

- **Language:** C# with nullable references enabled, implicit usings, async/await for all I/O
- **Naming:** `PascalCase` types/methods/properties, `camelCase` variables, `_camelCase` private fields, `I`-prefixed interfaces
- **DI:** All services registered in `MauiProgram.cs` (MAUI) and `Program.cs` (Backend). Repositories are scoped, parsers/transposers are singleton.
- **Testing:** xUnit + FluentAssertions + Moq. Arrange/Act/Assert pattern. In-memory EF Core for repository tests.
- **Database:** SQLite via EF Core 10. Automatic migrations on startup. Backend migrations in `src/AppCanzoniere.Backend/Migrations/`, MAUI migrations in `src/AppCanzoniere.Infrastructure/Migrations/`.
- **Anti-patterns to avoid:** Infrastructure dependencies in Domain/Application; fat controllers; anemic domain models; god services.

## External Service Integrations

All external services use interface abstractions with mock implementations as defaults:
- `IChordSuggestionService` — Perplexity AI (mock default)
- `IOcrService` — Azure Computer Vision / Tesseract (mock default)
- `IAudioRecognitionService` — ACRCloud (mock default)
- `ICloudSyncService` — LocalStorage implemented; OneDrive/GoogleDrive/Dropbox are placeholders

API keys are managed via User Secrets (`dotnet user-secrets`) — see `SETUP_INSTRUCTIONS.md` for details.

## Project Status

Phases 1–13 and 17 are complete. Phases 14–16 are partially done (security hardening, offline mode UI, advanced search UI). See `TEST_AND_COMPLETE.md` for the detailed checklist and `FUTURE_IMPLEMENTATIONS.md` for planned features.
