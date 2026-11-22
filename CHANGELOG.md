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
- I PDF troppo grandi, danneggiati o costruiti per consumare memoria vengono rifiutati con un messaggio.
- I libretti in PDF si dividono in canti, e ogni canto viene convertito in ChordPro con il suo titolo.
- I PDF si leggono in un processo separato con limiti di tempo e memoria; un file che non si può leggere viene segnalato come illeggibile.
- Le pagine scansionate si possono leggere con il riconoscimento del testo del dispositivo.
- Su Android le pagine scansionate si leggono con il riconoscimento del testo incluso nell'app, anche senza rete.
- Il pacchetto Windows include il lettore dei PDF, anche nella versione pubblicata.
- Nelle versioni di prova l'elenco della Home mostra un canto lungo, il "Canto di prova".
- Il pedale Bluetooth e la tastiera girano pagina nella schermata del canto: avanti con Pagina giù, Freccia giù, Freccia destra o Spazio, indietro con Pagina su, Freccia su o Freccia sinistra.
- Su Android i tasti del volume girano pagina nella schermata del canto (su indietro, giù avanti); altrove regolano il volume.
- Lo schermo resta acceso solo mentre è aperta la schermata di un canto, su Android e Windows.
- Da Canti, con il pulsante + e la voce "Da PDF", su Android e Windows si sceglie un PDF, si vede l'avanzamento della lettura e si può annullarla.
- Dall'import da PDF si leggono anche i libretti interi, con il riepilogo dei canti trovati e il numero di canti previsto dall'indice.
- Nell'elenco dell'import da PDF si scelgono i canti con Tutti, Nessuno o Solo da controllare, e ogni canto si corregge nella revisione affiancata.
- Nella revisione le righe dubbie del PDF hanno un segno "?" accanto al testo originale.
- Prima di salvare un canto importato si spunta una dichiarazione sui diritti del canto: la casella parte sempre non spuntata, e Salva resta spento finché non è spuntata.
- Su Windows l'import da PDF risponde a Ctrl+O per scegliere un PDF, Ctrl+S per aprire la dichiarazione, Esc per annullare o chiudere il foglio, e Alt con le frecce per cambiare canto.
- Nella revisione di un canto importato un cronometro misura il tempo di correzione, spento per predefinito, e "Copia i tempi" copia la tabella dei tempi.

### Modificato
- `{key}` indica la tonalità degli accordi scritti e il capotasto alza la tonalità che suona, come nello standard ChordPro.
- Con il capotasto le forme usano le alterazioni naturali della loro tonalità; la preferenza diesis o bemolle vale per la tonalità che suona.
- Nel testo incollato le parentesi quadre e graffe diventano tonde, con un avviso.
- Le righe in minuscolo con spazi singoli e senza altri segni da accordo restano testo anche se le parole sembrano accordi.
- La schermata del canto va a capo sulla larghezza reale dello schermo e si ricalcola alla rotazione o al ridimensionamento della finestra, con gli accordi sopra le sillabe.
- Girare pagina scorre di un'altezza dello schermo meno una riga, così l'ultima riga letta resta visibile.
- Su Windows lo schermo si spegne quando l'app è ridotta a icona durante la lettura di un canto.
- Tenendo premuto un tasto di pagina si gira una sola pagina, e i tasti di pagina non fanno nulla mentre si scrive in un campo di testo.
- Il minimo di Android è ora la versione 6.0: i dispositivi con Android 5.x non possono più installare l'app.
- La vecchia pagina di prova dell'OCR è sostituita dall'import da PDF, su Android e Windows.
- La scheda Importa esce dalla barra: l'import si apre da Canti → + → Da PDF.
- La scheda dei canti si chiama "Canti" e i suoi testi sono in italiano.
- Su Android l'app non invia più a Google le metriche d'uso di ML Kit durante il riconoscimento del testo delle scansioni.
- Scegliendo un altro PDF, l'app avvisa che i canti letti e non salvati andranno persi.
- Indietro durante una lettura la annulla senza chiedere conferma.

### Corretto
- Gli accordi di un canto ChordPro vengono riscritti nella posizione in cui erano.
- Leggere e riscrivere un canto ChordPro non aggiunge più righe vuote.
- Le righe di testo che contengono parentesi graffe non vengono più scartate.
- Su Android la schermata del canto mostrava solo la prima riga.
- Con trasposizione zero la preferenza diesis/bemolle non riscrive più gli accordi del canto.
- "Ripristina" nelle impostazioni non impedisce più i salvataggi successivi.
- Le note scritte in minuscolo sotto una riga di accordi vengono lette come testo.
- Diesis, bemolle e bequadro disegnati con i caratteri matematici di TeX vengono letti correttamente.
- Il lettore dei PDF su Windows si chiude insieme all'app, anche se l'app si chiude all'improvviso.
- Un PDF di un solo canto senza riga di titolo diventa un canto, intitolato dal titolo del PDF, dalla prima riga di testo o dal nome del file.
- Negli indici dei libretti le voci con i numeri di pagina tra trattini vengono contate per intero.
- Le righe di indicazione come Tastiera, Chitarra, capotasto o trasposizione non diventano titoli e non restano nel testo del canto.
- Una seconda versione di un canto, in altra tonalità o marcata "BIS", resta nello stesso canto.
- I piè di pagina con link e le righe correnti ripetute su più pagine non entrano nei canti, mentre titoli, ritornelli e righe di accordi vicino al margine restano.
- I piè di pagina che cambiano da una sezione all'altra del libretto non entrano più nel testo dei canti.
- Una scansione il cui unico testo è un piè di pagina viene segnalata come senza testo.
- Nelle schede scansionate tutte maiuscole le righe di accordi vengono riconosciute e scritte in notazione normale (Sol, Lam), mentre le righe di testo maiuscolo restano testo.
- Nelle scansioni gli accordi cadono sulla sillaba giusta, anche con "SI m" staccato, e le righe con barre o "BB" in maiuscolo vengono riconosciute.
- I libretti si dividono in canti con più precisione: intestazioni in fondo a una colonna, righe con numeri romani, titoli centrati, pagine ripetute in un'altra tonalità, e i ritornelli rientrati non iniziano più un nuovo canto.
- Su Android il primo tasto del pedale dopo un tocco sul canto gira pagina.
- Negli indici dei libretti le intestazioni di sezione e i piè di pagina non contano più come canti, e i titoli centrati più grandi del testo vengono riconosciuti.
- Un ritornello rientrato a metà canto non divide più il canto, a meno che non sia scritto come un titolo e non si ripeta nella pagina.
- Le lettere con la parte inferiore, come g, p e q, e le righe di accordi non vengono più tagliate in basso, nel canto e nell'anteprima.
- Uscendo dall'import durante una lettura, la lettura si annulla e nessun canto viene salvato.
- Uscendo dall'import con canti letti e non salvati, l'app chiede conferma prima di perderli.
- Un nuovo import attende la fine di una lettura ancora in corso.
- Le intestazioni di sezione elencate nell'indice di un libretto non diventano più titoli né testo dei canti.
- Un canto senza titolo sotto un'intestazione di sezione resta un canto a sé, intitolato dalla sua prima riga.
- I titoli numerati ripetuti in cima alle pagine restano nei canti anche nei libretti corti.
- Una riga uguale a un'intestazione dell'indice non divide né accorcia più un canto.