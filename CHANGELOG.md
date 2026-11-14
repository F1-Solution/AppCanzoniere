# Changelog

## Non rilasciato

### Aggiunto
- Notazione degli accordi in Do-Re-Mi e C-D-E, con il valore predefinito scelto dalla lingua e il minore scritto "Lam" o "La-".
- Trasposizione con diesis, bemolle o scelta automatica in base alla tonalità.
- Capotasto: forme suonate e tonalità che suona.
- Blocchi tab di ChordPro mantenuti identici e mai trasposti.
- Un file ChordPro letto e riscritto senza modifiche torna identico, comprese le direttive non riconosciute.
- Conversione in ChordPro di un testo con gli accordi scritti sopra le parole, con una diagnosi per ogni riga.
- Impaginazione del canto indipendente dallo schermo: accordi sopra le sillabe, a capo, pagine e modalità solo testo.
- La tonalità scritta a parole, come "La minore", viene riconosciuta e trasposta nello stesso stile.

### Modificato
- `{key}` indica la tonalità degli accordi scritti e il capotasto alza la tonalità che suona, come nello standard ChordPro.
- Con il capotasto le forme usano le alterazioni naturali della loro tonalità; la preferenza diesis o bemolle vale per la tonalità che suona.

### Corretto
- Gli accordi di un canto ChordPro vengono riscritti nella posizione in cui erano.
- Leggere e riscrivere un canto ChordPro non aggiunge più righe vuote.
- Le righe di testo che contengono parentesi graffe non vengono più scartate.
