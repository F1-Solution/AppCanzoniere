# Decisioni del proprietario — round 4 (13 novembre 2026)

Seguono le decisioni DEC 1-15 di [DECISIONI-ROUND-1.md](DECISIONI-ROUND-1.md). Riguardano il nucleo musicale (fase F2).

| # | Decisione | Effetto |
|---|---|---|
| DEC 16 | La preferenza diesis o bemolle decide solo il nome della tonalità che suona. Con il capotasto le forme si scrivono con le alterazioni naturali della loro tonalità. | In Mi maggiore le forme sono `Do#m` e `Sol#m`, mai `Rebm` e `Labm`. |
| DEC 17 | `{key}` è la tonalità degli accordi scritti nel file, come nello standard ChordPro; il capotasto alza la tonalità che suona. | Tonalità e accordi si spostano sempre insieme. I file di OnSong e SongbookPro, che usano convenzioni diverse, vanno convertiti all'import (F3, F7). |
| DEC 18 | Una tonalità scritta a parole o nell'altra notazione viene riconosciuta e riscritta nello stesso stile. | "La minore" trasposta di +2 diventa "Si minore". |

## Verifica su cui si basa DEC 17

| Fonte | Significato di `{key}` | Effetto del capotasto | Affidabilità |
|---|---|---|---|
| Standard ChordPro | Tonalità degli accordi come sono scritti | Alza solo la tonalità che suona | Verificato |
| OnSong | Il file contiene gli accordi alla tonalità che suona | L'app mostra le forme e lascia invariata la tonalità | Verificato |
| SongbookPro | Non documentato per un file importato con capotasto | L'app adatta gli accordi alla tonalità scelta | Parziale |

## Punti rimasti aperti

- Conversione delle convenzioni di OnSong e SongbookPro all'import.
- Le modifiche strutturali a un canto già letto (inserire o togliere righe) non sono supportate: vanno progettate insieme all'editor (F6, F7).
- La notazione scelta dall'utente non è ancora collegata alla schermata del canto: l'app legge ancora tutto in notazione inglese.
- La separazione delle pagine a due colonne dei PDF spetta all'import (F3).
