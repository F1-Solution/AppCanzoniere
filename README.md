# AppCanzoniere

Canzoniere per cori e animatori liturgici, in C# / .NET 10 con Clean Architecture. L'app è in .NET MAUI per Android e Windows; i testi sono in formato ChordPro, con parser e trasposizione degli accordi.

## Stato

Il progetto è in ripartenza: il codice precedente è stato rivalutato e in parte rimosso. La MAUI compila per Android e Windows, ma l'app non è un prodotto finito: OCR, riconoscimento audio e suggerimenti di accordi usano implementazioni finte di default, la sincronizzazione cloud ha solo l'archiviazione locale e il backend ASP.NET Core è congelato e non funziona.
Lo stato reale è in [docs/VALUTAZIONE.md](docs/VALUTAZIONE.md), le fasi in [ROADMAP.md](ROADMAP.md) e [docs/FASI.md](docs/FASI.md).

## Compilare e provare

Servono l'SDK indicato in `global.json` e, per la MAUI, i workload `maui-android` e/o `maui-windows`.

```bash
# Librerie e test, senza workload MAUI
dotnet test AppCanzoniere.Core.slnx

# Soluzione principale (core + MAUI)
dotnet build AppCanzoniere.slnx

# Backend (congelato, non funzionante)
dotnet build AppCanzoniere.Backend.slnx

# Solo una piattaforma MAUI
dotnet build src/AppCanzoniere.MAUI -f net10.0-android -p:MauiPlatforms=android
dotnet build src/AppCanzoniere.MAUI -f net10.0-windows10.0.19041.0 -p:MauiPlatforms=windows
```

## Struttura

- `src/AppCanzoniere.Domain`: entità e interfacce dei repository.
- `src/AppCanzoniere.Application`: servizi (parser ChordPro, trasposizione, servizi esterni finti o opzionali).
- `src/AppCanzoniere.Infrastructure`: EF Core con SQLite e repository.
- `src/AppCanzoniere.MAUI`: app Android e Windows.
- `src/AppCanzoniere.Backend`: API ASP.NET Core, congelata.
- `tests/`: test xUnit.
- `docs/`: documentazione; `docs/archivio/` contiene i documenti del vecchio piano, non affidabili.

## Documenti

- [docs/VALUTAZIONE.md](docs/VALUTAZIONE.md): valutazione del codice esistente.
- [docs/FASI.md](docs/FASI.md) e [ROADMAP.md](ROADMAP.md): piano per fasi.
- [docs/DECISIONI-ROUND-1.md](docs/DECISIONI-ROUND-1.md): decisioni del proprietario.
- [docs/business/](docs/business/) e [docs/design/](docs/design/): piano commerciale e proposte UX/UI.
- [PROGRESS.md](PROGRESS.md): lavori eseguiti.
