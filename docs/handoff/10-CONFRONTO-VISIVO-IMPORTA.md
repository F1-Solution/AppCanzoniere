# Confronto visivo della schermata Importa

Scopo: confrontare la schermata Importa reale, su PC e su tablet, con la proposta visiva. Il confronto dice se il lavoro visivo è finito.

Stato: **da fare**.

Riferimenti (solo per leggere, non servono per il confronto):
- [design/UI-IMPORT-ROUND-9.md](../design/UI-IMPORT-ROUND-9.md): stati, misure e classi di larghezza.
- [design/UX-IMPORT-ROUND-9.md](../design/UX-IMPORT-ROUND-9.md): comportamento di ogni stato.

Regola del progetto: l'agente non passa al lavoro visivo successivo finché tu non confermi il confronto.

ATTENZIONE: fino alla fase F4 la schermata Importa usa i colori di Leggio (beige e rosso scuro). Il resto dell'app resta viola. La differenza di colore è attesa. Non segnalarla come errore. Segnala invece leggibilità, posizione, testi e pulsanti che non corrispondono.

---

## Cosa serve

- Il tablet Android con l'app del round 10.
- Il PC Windows con l'app del round 10, a scala 100 %.
- Il PDF con il canto singolo, il PDF del libretto, la scansione e il file `danneggiato.pdf` (vedi [PROVE-SU-DISPOSITIVO.md](../verifiche/PROVE-SU-DISPOSITIVO.md), passi 25 a 28).
- Uno strumento per fare immagini: su Android il pulsante screenshot del tablet, su Windows Win+Shift+S.

---

## Passi

1. Apri la schermata Importa sul tablet, in verticale: tocca Canti → + → Da PDF.
2. Porta ogni stato della tabella qui sotto. Confronta con la colonna "Cosa guardare".
3. Ripeti i passi 1 e 2 sul tablet in orizzontale.
4. Apri la schermata Importa sul PC, a finestra intera: clicca Canti → + → Da PDF.
5. Porta ogni stato della tabella qui sotto.
6. Apri un canto in revisione (S3). Trascina il bordo della finestra fino a circa 1300 px. Controlla la classe larga della sezione "Classi di larghezza".
7. Trascina il bordo fino a circa 900 px. Controlla la classe media.
8. Trascina il bordo fino a circa 600 px. Controlla la classe stretta.
9. Controlla il riquadro vuoto della casella della dichiarazione (stato S4). Si deve vedere bene, anche senza spunta.
10. Scrivi il rapporto. Usa il modello della sezione "Come scrivere il rapporto".
11. Allega le immagini al prompt, se puoi. Altrimenti descrivi in una riga ogni differenza.

---

## Stati da confrontare

| Stato | Come arrivarci | Cosa guardare |
|---|---|---|
| S0 Avvio | Apri Canti → + → Da PDF senza file letti. | Titolo, testo di spiegazione, pulsante "Scegli un PDF" pieno e largo. Sul PC, una seconda riga "Ctrl+O". Nota sulle scansioni. Interruttore "Misura i tempi di correzione" spento. |
| S1 Lettura | Scegli la scansione. | Nome del file. "Pagina n di m". Barra di avanzamento sottile. Percentuale a destra. Pulsante "Annulla" alto e largo. Nessun altro pulsante attivo. |
| S2 Elenco | Scegli un libretto e aspetta la fine. | Riepilogo in un riquadro. Tre pulsanti Tutti, Nessuno, "Solo da controllare". Contatore "N selezionati". Righe con casella. Badge "! n righe da controllare". Etichetta "Scansione" sulle righe scansionate. Barra in basso con "Salva n canti selezionati". |
| S2 con zero canti | Tocca "Nessuno". | Pulsante "Salva" spento. Riga con "Seleziona almeno un canto". |
| S3 Revisione | Tocca una riga dell'elenco. | Titolo con i puntini. "< Prec.", "Succ. >", "n di m". Pannelli: Originale, ChordPro, Anteprima. Righe dubbie con "?". Barra in basso con "Ripristina" e "Salva questo canto". |
| S4 Dichiarazione | Tocca "Salva questo canto". | Sfondo scuro dietro il foglio. Foglio con titolo, riga della dichiarazione con casella non spuntata, "Annulla" e "Salva". "Salva" spento con la riga "Spunta la dichiarazione per salvare.". |
| S5 Salvato | Spunta, tocca "Salva" nel foglio. | Banner in cima con la parola "Salvato", pulsanti "Apri" e "Prossimo >". |
| S6 Errore | Scegli `danneggiato.pdf`. | Cerchio con "!" in rosso scuro, titolo, due righe di testo, pulsante primario e pulsante secondario. |
| S7 Annullato | Scegli la scansione e tocca "Annulla" a metà lettura. | Riga "Lettura annullata. Nessun canto importato." e "Scegli un PDF". |

---

## Schermate del nuovo ingresso

L'import si apre da Canti → + → Da PDF. La scheda "Importa" non c'è più. Confronta anche queste schermate, su tablet e su PC.

| Schermata | Come arrivarci | Cosa guardare |
|---|---|---|
| C1 Elenco Canti | Apri la scheda Canti dalla barra in basso. | Titolo "Canti". Pulsante "+" in alto. Campo di ricerca e testi in italiano, nessun testo in inglese. Segnala ogni testo in inglese. |
| C2 Menu "Aggiungi canto" | Tocca "+" in Canti. | Menu "Aggiungi canto" con la voce "Da PDF". Tutte le voci in italiano. Due tocchi rapidi aprono il menu una sola volta. |
| C3 Import con freccia Indietro | Tocca "+", poi "Da PDF". | Freccia Indietro in alto a sinistra. Titolo della schermata. ATTENZIONE: Indietro durante una lettura la annulla. Tocca Indietro solo quando vuoi uscire. Dopo Indietro torni all'elenco Canti. |
| C4 Barra in basso senza Importa | Guarda la barra quando sei in Canti. | Nessuna scheda "Importa". La scheda dei canti si chiama "Canti". Se la scheda dice ancora "Home", segnalalo. |
| C5 Elenco dopo il salvataggio | Scegli un libretto, salva alcuni canti, poi torna a Canti con Indietro. | I canti salvati compaiono in Canti. L'elenco si aggiorna senza riavvio. Nessun canto compare due volte. |

## Classi di larghezza (S3)

| Classe | Larghezza della pagina | Cosa guardare |
|---|---|---|
| Stretta | Sotto 700 px | Una colonna. Tre selettori in alto: Originale, ChordPro, Anteprima. Solo un pannello visibile. Il pannello scelto resta dopo la rotazione. |
| Media | Da 700 a 1199 px | Due colonne. A sinistra selettore a due (Originale, Anteprima) e il pannello. A destra ChordPro. |
| Larga | 1200 px o più | Elenco stretto a sinistra, poi tre pannelli affiancati, ognuno con intestazione. |

Le soglie sono sulla larghezza della pagina, non sul modello del dispositivo.

---

## Come scrivere il rapporto

Scrivi una riga per ogni stato o classe. Usa questo modello:

`[stato o classe] [piattaforma]: corrisponde` oppure `[stato o classe] [piattaforma]: non corrisponde: [cosa non va]`

Esempi:
- `S2 PC larga: corrisponde`
- `S3 tablet verticale: non corrisponde: ChordPro e Anteprima sono una sotto l'altra, non affiancate`
- `S4 tablet: non corrisponde: la casella vuota non si vede bene sul fondo chiaro`

Se un punto non lo puoi verificare, scrivi "non verificato" e il motivo.

ATTENZIONE: non scrivere "corrisponde" per uno stato che non hai visto. Scrivi "non verificato".

---

## Cosa succede dopo

1. Mandi il rapporto nel prompt successivo, con le immagini se ci sono.
2. L'agente corregge le differenze che indichi, senza toccare ciò che corrisponde.
3. Ti chiede di ripetere il confronto solo sulle parti corrette.
4. Quando tutte le righe sono "corrisponde", confermi con: `Confronto visivo Importa: confermato`.
5. Se resta una differenza che non vuoi correggere, scrivi: `Confronto visivo Importa: confermato con eccezione: [la differenza]`.
