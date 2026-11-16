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
- La schermata del canto mostra accordi e testo impaginati, nella notazione, nella preferenza diesis/bemolle e con la trasposizione scelte nelle impostazioni.
- Nelle impostazioni si scelgono la notazione degli accordi, diesis o bemolle e la lingua, con la voce "Automatica (lingua del dispositivo)".
- Il canto usa un carattere a spaziatura fissa incluso nell'app (JetBrains Mono, licenza OFL), così gli accordi restano sopra la sillaba giusta su Android e Windows.
- Estrazione del testo dai PDF con il testo già selezionabile, comprese le pagine a due colonne, che vengono separate.
- I PDF troppo grandi, danneggiati o costruiti per consumare memoria vengono rifiutati con un messaggio; l'import dai PDF non è ancora disponibile nell'app.

### Modificato
- `{key}` indica la tonalità degli accordi scritti e il capotasto alza la tonalità che suona, come nello standard ChordPro.
- Con il capotasto le forme usano le alterazioni naturali della loro tonalità; la preferenza diesis o bemolle vale per la tonalità che suona.
- Nel testo incollato le parentesi quadre e graffe diventano tonde, con un avviso.
- Le righe in minuscolo con spazi singoli e senza altri segni da accordo restano testo anche se le parole sembrano accordi.

### Corretto
- Gli accordi di un canto ChordPro vengono riscritti nella posizione in cui erano.
- Leggere e riscrivere un canto ChordPro non aggiunge più righe vuote.
- Le righe di testo che contengono parentesi graffe non vengono più scartate.
- Su Android la schermata del canto mostrava solo la prima riga.
- Con trasposizione zero la preferenza diesis/bemolle non riscrive più gli accordi del canto.
- "Ripristina" nelle impostazioni non impedisce più i salvataggi successivi.
- Le note scritte in minuscolo sotto una riga di accordi vengono lette come testo.
