# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

AppCanzoniere is a .NET 10 / C# multi-platform songbook application using Clean Architecture. It has a .NET MAUI frontend (Android, Windows); the ASP.NET Core backend is frozen outside the main solution. Songs are stored in ChordPro format with transposition. OCR, audio recognition and chord suggestions exist only as mock implementations by default; there is no offline mode.

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

`tools/AppCanzoniere.CorpusReport` is a console tool that converts the local, git-ignored song corpus (`corpus-locale/`) and writes an HTML comparison report: `dotnet run --project tools/AppCanzoniere.CorpusReport -c Release -- corpus-locale`.

`tools/AppCanzoniere.PdfBench` measures PDF extraction on the local corpus and writes `corpus-locale/report-pdf/index.md`: `dotnet run --project tools/AppCanzoniere.PdfBench -c Release -- corpus-locale`. It reads through the separate worker by default (`--in-process` to skip it).

`tools/AppCanzoniere.OcrBench` (Windows only, outside both solutions) runs on-device OCR on scanned corpus PDFs and writes `corpus-locale/report-ocr/index.md`: `dotnet run --project tools/AppCanzoniere.OcrBench -c Release -- corpus-locale`.

`AppCanzoniere.slnx` is the main solution (core + MAUI, needs the Android and Windows workloads); `AppCanzoniere.Core.slnx` has no MAUI. Package versions are central in `Directory.Packages.props`, the SDK is pinned in `global.json`. CI is `.github/workflows/ci.yml` (jobs core, android, windows). Core tests: 3467 (Domain 5, Application 2878, Infrastructure 584).

**Backend:** the ASP.NET Core API is frozen and outside the main solution and CI; open it with `AppCanzoniere.Backend.slnx`. It does not work (see `docs/VALUTAZIONE.md`) and its integration tests fail.

## Architecture

Clean Architecture with four layers (dependencies flow inward only):

- **Domain** (`src/AppCanzoniere.Domain`) — Entities (Song, Album, Tag, LiturgicalMoment, etc.), enums, repository interfaces. Zero external dependencies.
- **Application** (`src/AppCanzoniere.Application`) — Services, DTOs, service interfaces. Key services: `ChordProParser`, `ChordTransposer`. Chord notation, accidental spelling and capo logic live in the `Music` namespace, the UI-independent song layout in `Layout`, and the conversion of chords-above-lyrics text to ChordPro in `Import`. What the song screen shows (notation, accidentals, transposition, capo from the user settings) is prepared in `Display`, and PDF text extraction with column splitting lives in `PdfImport` (the PdfPig adapter and the limits against crafted files are in Infrastructure `PdfImport`). Depends only on Domain.
- **Infrastructure** (`src/AppCanzoniere.Infrastructure`) — EF Core `AppDbContext` with SQLite, repository implementations, migrations. Implements Application interfaces.
- **Presentation** — Two frontends:
  - **MAUI** (`src/AppCanzoniere.MAUI`) — Pages, ViewModels (MVVM via CommunityToolkit.Mvvm), platform services
  - **Backend** (`src/AppCanzoniere.Backend`) — frozen, not in the main solution or CI

PDFs are read by `src/AppCanzoniere.PdfWorker`, a separate process supervised from Infrastructure `PdfImport`; songbook splitting into songs and the OCR contract live in Application `PdfImport`, and the song screen is drawn by a GraphicsView control in MAUI `Controls`.

## Key Conventions

- **Language:** C# with nullable references enabled, implicit usings, async/await for all I/O
- **Naming:** `PascalCase` types/methods/properties, `camelCase` variables, `_camelCase` private fields, `I`-prefixed interfaces
- **DI:** All services registered in `MauiProgram.cs` (MAUI) and `Program.cs` (Backend). Repositories are scoped, parsers/transposers are singleton.
- **Testing:** xUnit + FluentAssertions + Moq. Arrange/Act/Assert pattern. In-memory EF Core for repository tests.
- **Database:** SQLite via EF Core 10. The MAUI app creates the schema with `EnsureCreated` in `MauiProgram.cs`; `App.xaml.cs` then calls `MigrateAsync` and seeds data, but the Infrastructure project has no migrations (persistence is redone in a later phase). The Backend has its own migrations.
- **Anti-patterns to avoid:** Infrastructure dependencies in Domain/Application; fat controllers; anemic domain models; god services.

## External Service Integrations

All external services use interface abstractions with mock implementations as defaults:
- `IChordSuggestionService` — Perplexity AI (mock default)
- `IOcrService` — Azure Computer Vision (mock default; there is no Tesseract implementation)
- `IAudioRecognitionService` — ACRCloud (mock default)
- `ICloudSyncService` — LocalStorage implemented; OneDrive/GoogleDrive/Dropbox providers do not exist (only commented-out registrations)

The real services are commented out in `MauiProgram.cs`; the Perplexity service reads `PERPLEXITY_API_KEY` from the environment. The old setup guide is archived in `docs/archivio/` and unverified.

## Project Status

Restart in progress: the old "87% complete" claims are unreliable. See `ROADMAP.md` for the phase table, `PROGRESS.md` for completed work, `docs/VALUTAZIONE.md` for the real state and `docs/FASI.md` for the plan.
