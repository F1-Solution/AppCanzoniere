# Specifica visiva — schermata Importa (round 9)

> Lavoro U2 di [PIANO-ROUND-9.md](../PIANO-ROUND-9.md). Input: [UX-IMPORT-ROUND-9.md](UX-IMPORT-ROUND-9.md) (stati S0–S7, classi di larghezza, errori). Valori e chiavi dalle tavole di [UI-1-leggio.md](ui/UI-1-leggio.md) (16 chiavi di colore di F4, DEC 8). Solo controlli MAUI standard più il controllo di disegno del canto. Nessun colore, misura o carattere scritto a mano nella pagina: tutto da `DynamicResource` sulle chiavi qui sotto.

## 1. Token provvisori

Un solo dizionario provvisorio `Token.*` (nome del file a scelta dello sviluppatore). F4 sostituisce i valori e non tocca la pagina. Le chiavi `Leggio.*` del controllo del canto restano com'è.

### Colori (le 16 chiavi di F4; valori provvisori = Leggio, come in UI-1)

| Chiave | Chiaro | Scuro | Uso nella pagina |
|---|---|---|---|
| `Token.bg` | #F6F1E7 | #16120E | Sfondo pagina, vano tra i pannelli |
| `Token.surface` | #FFFCF5 | #1F1A15 | Barre, riga elenco, foglio S4, pannello ChordPro |
| `Token.surface2` | #EFE8DA | #2A241D | Pannello Originale, Salva disattivo, riepilogo |
| `Token.lyricsBg` | #FFFCF5 | #16120E | Pannello Anteprima (vedi nota) |
| `Token.text` | #1E1A15 | #EFE6D6 | Testo principale, titoli |
| `Token.text2` | #5B5246 | #B9AD98 | Testo secondario, bordi dei controlli, etichetta Scansione |
| `Token.chord` | #9E2318 | #F29A86 | Non usato dalla pagina (solo dal controllo) |
| `Token.chordBg` | transparent | transparent | Non usato |
| `Token.accent` | #8C1E14 | #EC8B76 | Pulsante primario, barra di avanzamento, spunta, segmento attivo |
| `Token.onAccent` | #FFFFFF | #1A0E0A | Testo sul primario |
| `Token.accentSoft` | #F2DED6 | #3D211A | Badge avviso, riga attiva, righe dubbie, segmento attivo |
| `Token.onAccentSoft` | #6A160E | #F7C2B4 | Testo su `accentSoft` |
| `Token.border` | #D9CEBB | #3B3229 | Solo divisori decorativi (righe, barre) |
| `Token.danger` | #8C1E14 | #FF9C8A | Pallino "!" degli errori |
| `Token.focus` | #1E1A15 | #EFE6D6 | Anello di focus |
| `Token.rule` | #D9CEBB | #3B3229 | Non usato |

Note:
- Lo scuro provvisorio è il Leggio scuro: il Palco scuro (predefinito, DEC 8) arriva con F4. I contrasti vanno rimisurati allora.
- Il pannello Anteprima usa `Leggio.Background` (come il controllo) finché `lyricsBg` e `Leggio.Background` non coincidono in F4.
- Oggi il resto dell'app usa il viola `Primary` (#512BD4). Fino a F4 la pagina Importa avrà un aspetto diverso dalle altre: è voluto, e la decisione di non ricolorare le altre pagine è del proprietario.
- Nessun colore di "avviso" separato: l'avviso è `accentSoft`/`onAccentSoft` con "!" e testo; l'errore è `danger` con "!" e testo. Mai solo il colore.

### Misure, tipo, forma

| Chiave | Valore | Note |
|---|---|---|
| `Token.space.1/2/3/4/6/8` | 4 / 8 / 12 / 16 / 24 / 32 | Griglia a 8; il 4 solo dentro un componente |
| `Token.touch` | Android 56, Windows 48 | Altezza minima di ogni controllo toccabile; 8 dp tra due controlli |
| `Token.touch.primary` | 56 | Pulsante primario su entrambe |
| `Token.radius` | 4 | Angoli quasi vivi (Leggio). Pallino errore: 16 |
| `Token.stroke` | 1 (2 per focus e campo con focus) | Nessuna ombra. La separazione è bordo o sfondo |
| `Token.gutter` | 16 sotto 700 dp, 24 da 700 dp | Margine laterale della pagina |
| `Token.measure` | 560 | Larghezza massima del contenuto in S0, S1, S6, S7 e S4 |

| Ruolo tipo | Carattere (oggi) | Dimensione / riga | Uso |
|---|---|---|---|
| `Token.type.title` | OpenSansSemibold | 24 / 1,3 | Titolo pagina, titolo di S6 |
| `Token.type.heading` | OpenSansSemibold | 20 / 1,3 | Titolo di sezione, titolo foglio S4 |
| `Token.type.body` | OpenSansRegular | 16 / 1,4 | Testo, righe elenco, pulsanti (Semibold) |
| `Token.type.small` | OpenSansRegular | 14 / 1,4 | Secondario, pagine, badge (Semibold) |
| `Token.type.mono` | JetBrainsMono | 14 / 1,4 | ChordPro, Originale, numeri di riga, percentuale, cronometro |

Nessun testo sotto 14. I font di UI-1 (Literata, Source Sans 3) arrivano con F4: qui solo quelli già nel progetto. I testi vanno a capo, non si troncano (tranne il titolo in testata S3, con puntini). Maiuscolo non usato.

## 2. Componenti

| Componente | Costruzione MAUI | Aspetto |
|---|---|---|
| Pulsante primario | `Button` | `accent` pieno, testo `onAccent` body semibold, altezza 56, raggio 4, larghezza piena in stretta, min 240 altrove. Uno per stato. |
| Pulsante secondario | `Button` | Sfondo `surface`, bordo 1 `text2`, testo `text` body semibold, altezza `touch`. |
| Pulsante testo | `Button` | Senza bordo, testo `accent` body semibold, altezza `touch`. Per Apri, Dettagli, Altro PDF. |
| Disattivo (primario) | `Button IsEnabled=false` | Sfondo `surface2`, testo `text2`, senza bordo. Accanto o sotto, una riga small `text2` che dice perché (S2 e S4). |
| Segmento (selettore pannelli) | `Grid` di `RadioButton` o `Border`+`TapGestureRecognizer` | Altezza `touch`. Attivo: sfondo `accentSoft`, testo `onAccentSoft` semibold, barra inferiore 2 `accent`. Inattivo: `surface`, testo `text`. Stato attivo anche in `SemanticProperties`. |
| Badge avviso | `Border` + `Label` | Sfondo `accentSoft`, testo `onAccentSoft` small semibold, padding 8×4, raggio 4. Testo sempre "! n righe da controllare". |
| Etichetta Scansione | `Border` + `Label` | Solo contorno 1 `text2`, testo `text2` small, padding 8×4. Forma diversa dal badge (non pieno). |
| Etichetta Modificato | `Border` + `Label` | Come Scansione ma contorno e testo `accent`. |
| Banner (S5, riepilogo S2) | `Border` | Sfondo `accentSoft`, testo `onAccentSoft`, barra sinistra 4 `accent`, padding 16, pulsante testo **Chiudi** a destra. Senza glifo spunta: si legge "Salvato" in parole. |
| Campo con bordo (editor, dichiarazione) | `Border` che avvolge `Editor` | Bordo 1 `text2` (non `border`: serve ≥ 3:1), sfondo `surface`, raggio 4. Con focus: bordo 2 `focus`. |

Icone: nessuna. Glifi di testo ASCII: `!` avviso, `?` riga dubbia, `<` `>` con la parola ("< Prec.", "Succ. >", "< Elenco"). Niente emoji, niente `✓`/`ⓘ`/`⏱` (non nel carattere): ⓘ diventa il pulsante testo "Perché?", ⏱ diventa "Tempo 0:48" in mono.

## 3. Stati

Tutti i valori di spaziatura sono `Token.space.*`; la pagina è un `ScrollView` salvo S2/S3 (scorrono i loro pannelli).

| Stato | Struttura (dall'alto) | Dettagli visivi |
|---|---|---|
| S0 Avvio | Colonna centrata, max 560, margine `gutter`. Titolo (title) · riga di spiegazione (body, `text2` solo per la nota sui tempi) · 24 · **Scegli un PDF** (primario, larghezza piena) · 16 · nota sulle scansioni (small `text2`) · 32 · interruttore tempi | Stato vuoto = questo stato: nessuna illustrazione. Su Windows il pulsante ha una seconda riga small "Ctrl+O". L'interruttore è un `Switch` con etichetta body a sinistra, riga alta `touch`, spento. |
| S1 Lettura | Nome file (body semibold, a capo) · 16 · nome fase (body) · 8 · "Pagina n di m" (body) · 8 · `ProgressBar` alta 8, `ProgressColor` = `accent` · 8 · percentuale (mono, a destra) e tempo rimanente (small `text2`) · 24 · due righe small con i conteggi · 32 · **Annulla** (secondario, altezza 56, larghezza piena in stretta) | Barra sempre determinata. Se la fase cambia, cambia la parola, non il colore. Nessuna animazione oltre alla barra. Il resto della pagina non risponde: nessun altro controllo presente. |
| S2 Elenco | Riepilogo (`surface2`, padding 16, 4 righe max: nome file semibold, conteggi, scarto indice con pulsante testo "Perché?", pagine non lette come banner con "Dettagli") · barra filtri (Tutti, Nessuno: secondari; "Solo da controllare": `Switch` con etichetta) · contatore "151 selezionati" (small `text2`) · `CollectionView` righe · barra inferiore fissa | Barra inferiore: `surface`, bordo superiore 1 `border`, padding 8×16, **Salva n canti selezionati** primario 56 (disattivo a 0, con la riga "Seleziona almeno un canto"). L'elenco riserva sotto un margine pari alla barra. |
| S3 Revisione | Testata (titolo con puntini, `< Prec.`, "n di m", `Succ. >`, **< Elenco** fuori dalla classe larga; avvisi; "Tempo m:ss") · pannelli per classe (§5) · barra inferiore | Testata `surface`, bordo inferiore 1 `border`. Avvisi: lista di badge su più righe, un badge per avviso, testo da UX §8. Barra inferiore: **Ripristina** (secondario) a sinistra, **Salva questo canto** (primario) a destra; in stretta i due pulsanti in due colonne uguali. |
| S4 Dichiarazione | Scrim a tutta pagina nero al 50 %; sopra, foglio `surface` max 560, raggio 4, bordo 1 `text2`, padding 24: titolo (heading) · riga small `text2` · 16 · riga dichiarazione (§6) · 8 · riga di aiuto small `text2` · 24 · **Annulla** (secondario) e **Salva** (primario) | Stretta: foglio attaccato in basso a tutta larghezza. Altrimenti centrato. Realizzato come `Grid` sovrapposto alla pagina, non come pagina nuova. Il fondo non risponde ai tocchi. |
| S5 Salvato | Banner in cima alla pagina (§2), sotto il contenuto di S2/S3. Pulsanti **Apri** e **Prossimo >** (testo) dentro il banner, a capo se serve | Resta finché non si chiude. Non ruba il focus: annunciato con `SemanticScreenReader.Announce`. |
| S6 Errore | Colonna centrata, max 560: pallino 32 (`danger`, "!" in `onAccent` bold, raggio 16) a fianco del titolo (title) · 8 · cosa è successo (body) · 8 · cosa fare (body, `text2` escluso: va in `text`) · 24 · azioni | Primario: l'azione che porta avanti ("Importa tutto come un canto", "Riprova"; altrimenti "Scegli un altro PDF"). Secondario sotto: "Scegli un altro PDF". Il titolo riceve il focus. |
| S7 Annullato | Colonna centrata, max 560: riga body "Lettura annullata. Nessun canto importato." · 16 · **Scegli un PDF** primario | Nessun colore speciale. |

## 4. Riga dell'elenco (S2)

`Grid`, altezza minima 72 (= 8 + 56 + 8), padding 8×16, `surface`, divisore inferiore 1 `border`. Colonne: **56 | \* | auto**, spazio tra colonne 8.

| Colonna | Contenuto | Dettagli |
|---|---|---|
| 1 | `CheckBox` dentro un `Grid` 56×56 | Area di tocco 56×56 (Windows 48). `Color` = `accent`. Il bordo del riquadro non spuntato deve valere ≥ 3:1: verificare su dispositivo (§7); se la piattaforma lo disegna chiaro, avvolgerlo in un `Border` 2 `text2`. Riga salvata: spunta tolta e disattiva. |
| 2 | Titolo (body semibold, a capo) · seconda riga: pagine ("p. 4–5", small `text2`) · terza riga (può andare a capo, `FlexLayout Wrap`, spazio 8): badge avviso, etichetta Scansione, Modificato | Toccando la colonna si apre S3 (area intera, min 56). Nessun avviso: la terza riga manca (niente "Nessun avviso" in grigio: il vuoto basta; il lettore di schermo non lo dice). |
| 3 | Chevron come testo `>` (body, `text2`) in un'area 56 | Solo nelle classi stretta e media. Riga salvata: al suo posto il pulsante testo **Apri**. |

Stati:

| Stato riga | Sfondo | Altro |
|---|---|---|
| Normale | `surface` | |
| Attiva (classe larga, canto aperto a destra) | `accentSoft` | Barra sinistra 4 `accent`; il badge sulla riga attiva ha sfondo `surface` e bordo 1 `accent` (altrimenti sparisce) |
| Focus tastiera | come sopra | Anello di focus (§8) intorno a tutta la riga |
| Salvata | `surface` | Titolo `text2`; riga small "Salvato" |
| Con cronometro attivo | | Riga small "Salvato · 1:12" (mono per i numeri) |

Lettura per schermo: `SemanticProperties.Description` composta come in UX §7 ("Alleluia, passeranno i cieli, pagine 4 e 5, scansione, 2 righe da controllare, selezionato"); la spunta è raggiungibile a parte.

## 5. Revisione per classe di larghezza (S3)

Le soglie sono sulla larghezza della pagina (700 e 1200), non sul dispositivo. Pannelli in `Grid`, vano 8 `bg` tra i pannelli, ogni pannello `Border` 1 `border` (decorativo), raggio 4.

| Classe | Colonne | Pannelli | Altezza utile |
|---|---|---|---|
| Stretta < 700 | `*` | Segmento a 3: **Originale · ChordPro · Anteprima**, uno solo visibile; il segmento scelto resta dopo la rotazione e il cambio di canto | Pagina meno testata, segmento e barra inferiore |
| Media 700–1199 | `*, *` | Sinistra: segmento a 2 **Originale · Anteprima**; destra: ChordPro | Con la tastiera aperta i pannelli si accorciano ma restano visibili (min 160) |
| Larga ≥ 1200 | Elenco 360 fisso, poi `*, *, *` (min 280 ciascuno) | Originale · ChordPro · Anteprima, ognuno con intestazione 40 | A 1200 esatti restano (1200 − 360 − 16) / 3 ≈ 274 dp per colonna: il minimo è 272, non 280. Sotto 1200 vale la media. |

Pannelli:

| Pannello | Sfondo | Testo | Note |
|---|---|---|---|
| Originale | `surface2` | mono `text`, sola lettura (`Editor IsReadOnly` o `CollectionView` di righe) | Colonna sinistra 32 con numero di riga (mono small `text2`). Riga dubbia: sfondo `accentSoft`, "?" in `onAccentSoft` bold nel margine. Senza dati sulle righe dubbie (D-U3) si mostra solo il conteggio in testata. |
| ChordPro | `surface` | mono `text` | `Editor` multilinea, `IsSpellCheckEnabled=false`, `IsTextPredictionEnabled=false`. Bordo e focus come campo con bordo (§2). |
| Anteprima | `Leggio.Background` | Controllo di disegno del canto | Aggiornata dopo 400 ms. Se la conversione fallisce, resta l'ultima valida e sopra compare una striscia `accentSoft`/`onAccentSoft` "! Anteprima non aggiornata: controlla la riga n". |

Intestazione di pannello (classe larga): 40 alta, `surface`, testo small semibold `text`, bordo inferiore 1 `border`. In media e stretta fa le veci il segmento.

## 6. Blocco dichiarazione (S4)

| Elemento | Spec |
|---|---|
| Riga | `Border` con bordo 1 `text2`, raggio 4, padding 8, altezza minima 72; dentro `Grid` 56 \| \*. Tutta la riga è l'area di tocco (`TapGestureRecognizer`, più il tocco sulla `CheckBox`). |
| Casella | `CheckBox` `Color`=`accent`, **`IsChecked=false` ad ogni apertura**. Mai preselezionata, mai memorizzata (DEC 38). Nessun binding con una preferenza. |
| Testo | body `text`, a capo; è anche `SemanticProperties.Description` della casella. |
| Stato spuntato | Sfondo della riga `accentSoft`, testo `onAccentSoft` (la spunta resta il segno principale). |
| Salva | Disattivo finché non spuntata (§2: `surface2`, testo `text2`). Sotto la riga, finché disattivo: "Spunta la dichiarazione per salvare." (small `text2`), legato al pulsante con `SemanticProperties.Hint`. Attivo: primario pieno. |
| Annulla | Secondario, a sinistra di Salva. Su Windows `Esc` equivale. Stretta: due pulsanti in colonna, Salva sopra, 8 di spazio. |
| Dopo "Salva" | Pulsante e casella disattivi, testo "Salvataggio 37 di 150" (body) al posto della riga di aiuto, `ProgressBar` 8. Niente annulla. |

## 7. Contrasti

Calcolati sui valori della tavola §1 (formula WCAG 2.x). Soglie: testo ≥ 4,5:1, elementi di interfaccia e anelli di focus ≥ 3:1.

| Coppia | Uso | Chiaro | Scuro | Esito |
|---|---|---|---|---|
| `text` su `bg` | Pagina | 15,37 | 15,05 | Testo OK |
| `text` su `surface` | Righe, foglio | 16,88 | 13,94 | Testo OK |
| `text` su `surface2` | Originale | 14,19 | 12,39 | Testo OK |
| `text2` su `bg` | Secondario | 6,81 | 8,42 | Testo OK |
| `text2` su `surface` | Pagine, etichette, aiuto | 7,48 | 7,80 | Testo OK |
| `text2` su `surface2` | Riepilogo, Salva disattivo | 6,29 | 6,94 | Testo OK |
| `onAccent` su `accent` | Primario | 9,09 | 7,67 | Testo OK |
| `onAccentSoft` su `accentSoft` | Badge, banner, riga dubbia | 9,34 | 9,32 | Testo OK |
| `text` su `accentSoft` | Riga attiva | 13,34 | 11,85 | Testo OK |
| `text2` su `accentSoft` | Dettagli su riga attiva | 5,91 | 6,63 | Testo OK |
| `onAccentSoft` su `surface` | Badge su riga attiva | 11,82 | 10,96 | Testo OK |
| `accent` su `surface` | Pulsante testo, barra, spunta | 8,87 | 7,00 | Testo e UI OK |
| `accent` su `surface2` | Pulsante testo nel riepilogo | 7,45 | 6,23 | Testo OK |
| `accent` su `bg` | Barra di avanzamento sulla pagina | 8,07 | 7,56 | UI OK |
| `danger` su `surface` | Pallino errore su foglio | 8,87 | 8,53 | UI OK |
| `focus` su `bg` | Anello su pagina | 15,37 | 15,05 | UI OK |
| `focus` su `surface` | Anello su riga, foglio | 16,88 | 13,94 | UI OK |
| `focus` su `accentSoft` | Anello su riga attiva | 13,34 | 11,85 | UI OK |
| `text2` su `surface` | Bordo di campi e dichiarazione, Scansione | 7,48 | 7,80 | UI OK |
| `border` su `surface` | Divisori | 1,52 | 1,38 | **Sotto 3:1**: ammesso solo perché decorativo, mai come unico segno di un controllo |
| `surface2` su `surface` | Salva disattivo | 1,19 | 1,12 | Controllo disattivato: esente; si distingue per assenza di accento e per la riga di aiuto |

Da verificare sul dispositivo, non dalla tavola: il bordo del riquadro della `CheckBox` non spuntata (Android e Windows) deve dare ≥ 3:1 su `surface`; il binario della `ProgressBar` è del sistema (non si misura, la percentuale e "Pagina n di m" sono scritti). Il testo disattivo non è soggetto a soglia.

Quando F4 sostituirà i valori (Palco scuro, §1) la tabella va rifatta con il controllo in CI di F4; le soglie restano.

## 8. Focus da tastiera su Windows

| Elemento | Visuale del focus |
|---|---|
| `Button`, `CheckBox`, `Switch`, `Editor` | Il segno nativo di WinUI (anello a due toni) resta acceso: non lo si toglie. Su `Editor` nel `Border`, bordo 2 `focus` via `DataTrigger` su `IsFocused`. |
| Righe, segmenti, riga della dichiarazione (non nativi) | `Border` esterno con bordo 2 `focus` quando il figlio o la riga ha il focus, più 2 di spazio interno (`Padding` 2) tra anello e contenuto; l'anello sta fuori dal riempimento, quindi si legge contro lo sfondo adiacente anche sul pulsante primario (il `focus` sul solo `accent` darebbe 1,9:1). |
| Ordine di Tab | Come UX §6: riepilogo, Tutti/Nessuno, elenco, Originale, ChordPro, Anteprima, pulsanti. Il banner e il foglio S4 non coprono mai il controllo col focus (la barra inferiore riserva spazio all'elenco). |
| Alla comparsa del foglio S4 | Focus alla casella; alla chiusura torna al pulsante che l'ha aperto. |
| Spazio / Invio | Spazio spunta la riga con il focus; Invio la apre. |

Android con tastiera esterna usa il focus di sistema. Nessun effetto al passaggio del mouse oltre al cursore a mano sulle righe; lo stato premuto (Android: increspatura di sistema) non cambia le misure.

## 9. Per lo sviluppatore (promemoria)

- Misure e colori solo da `Token.*` con `DynamicResource`; i soli valori fissi sono lo scrim nero 50 % e le soglie 700/1200.
- `OnPlatform` per `Token.touch` (Android 56, Windows 48); nient'altro è per piattaforma.
- Una sola pagina con stati: `VisualStateManager` o `IsVisible` su contenitori; il foglio S4 è un `Grid` sovrapposto.
- Classi di larghezza dal `SizeChanged` della pagina, non da `DeviceIdiom`.
- Dopo la costruzione, confronto affiancato con questo documento su tablet verticale, tablet orizzontale e finestra Windows a tre larghezze, e conferma del proprietario prima di chiudere (regola del progetto sul lavoro visivo).
