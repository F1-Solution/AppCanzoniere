# Decisioni del proprietario — round 6 (16 novembre 2026)

Seguono le decisioni DEC 19-25 di [DECISIONI-ROUND-5.md](DECISIONI-ROUND-5.md).

| # | Decisione | Effetto | Chi |
|---|---|---|---|
| DEC 22-25 | Confermate dal proprietario il 16/11/2026 (erano "Orchestratore, da confermare" nel round 5). | — | Proprietario |
| DEC 26 | I PDF si leggono in un processo separato; un file costruito ad arte può chiudere o bloccare solo quel processo. | Su Windows un programma a parte con limiti di tempo (2 minuti) e memoria (1 GB); su Android un servizio in un processo separato, da costruire in F7. | Proprietario |
| DEC 27 | Con il capotasto e senza `{key}` la tonalità indovinata può mescolare diesis e bemolle. | `Am Dm G C Ab` con capotasto 2 dà `Gm Cm F Bb F#`. | Proprietario |

## Punti rimasti aperti

- **OCR su Windows:** sul PC c'è solo la lingua inglese; la scheda 19 è a 70 dpi e illeggibile; la scheda 20 dà circa 25 righe di testo su 30 ma nessuna riga di accordi riconosciuta (testo tutto maiuscolo).
- **OCR su Android:** il riconoscimento con ML Kit non è ancora costruito; rischio V-07.
- **Pagine ruotate di 90 gradi:** risultano senza testo.
- **Divisione in canti:** debole sulle schede di un solo canto senza riga di titolo (04, 06) e sul libretto 15 (352 canti trovati contro 221 voci d'indice).
- **Piè di pagina:** i link che restano nel testo del canto.
- **Pacchetto Windows:** il programma di lettura dei PDF va incluso nel pacchetto.
- **Leggio:** non ancora provato su tablet e PC reali.
