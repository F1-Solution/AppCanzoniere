# UI 3 — Spartito

> Uno strumento di precisione: griglia svizzera, monospazio per accordi e tonalità, densità alta.

Mockup: [`UI-3-spartito.html`](UI-3-spartito.html) — quattro schermate telefono, canzone su tablet orizzontale, interruttore chiaro/scuro e tavola dei token.

## Carattere e ispirazione

Uno **strumento di precisione** in stile svizzero: griglia rigorosa, filetti da 1 px, angoli vivi, un solo colore segnale (blu inchiostro). Ogni dato "musicale" è in monospazio IBM Plex Mono: accordi, tonalità, numeri di posizione, codici delle sezioni (`V1`, `RIT`). Le righe del testo sono numerate nel margine come in un editor, utile in prova ("riprendiamo dalla riga 7"). È la direzione più densa e più adatta al desktop, dove l'utente prepara e modifica i canti.

## Palette

Grigi freddi (`#EEF0F3` sfondo, `#FFFFFF` superfici) con un unico accento blu `#0B57D0`, usato anche per gli accordi. Il tema scuro è un blu notte quasi nero (`#0B0F14`) con accento `#7CB2FF`.

| Token | Uso | Chiaro | Scuro |
|---|---|---|---|
| `bg` | Sfondo app | `#EEF0F3` | `#0B0F14` |
| `surface` | Superficie (card, barre) | `#FFFFFF` | `#131920` |
| `surface2` | Superficie secondaria / campi | `#F5F6F8` | `#1A222B` |
| `lyricsBg` | Sfondo del testo canzone | `#FFFFFF` | `#0F141A` |
| `text` | Testo principale e liriche | `#101418` | `#E6EAF0` |
| `text2` | Testo secondario | `#4A525D` | `#9AA5B3` |
| `chord` | Accordi | `#0B57D0` | `#7CB2FF` |
| `chordBg` | Fondo chip accordo (se usato) | `transparent` | `transparent` |
| `accent` | Azione primaria / selezione | `#0B57D0` | `#7CB2FF` |
| `onAccent` | Testo su accento | `#FFFFFF` | `#07111F` |
| `accentSoft` | Fondo elemento selezionato | `#DCE7FC` | `#16294A` |
| `onAccentSoft` | Testo su fondo selezionato | `#0A3A8A` | `#B7D3FF` |
| `border` | Bordi e divisori | `#C5CBD3` | `#2A333E` |
| `danger` | Errore / elimina | `#B42318` | `#FF8F84` |
| `focus` | Anello di focus | `#0B57D0` | `#7CB2FF` |
| `rule` | Specifico della direzione | `#D5DAE1` | `#242C36` |

## Tipografia

| Ruolo | Font | Dimensioni e note |
|---|---|---|
| Liriche e interfaccia | IBM Plex Sans | liriche 18 px / 1,3; corpo 15 px |
| Accordi, tonalità, numeri | IBM Plex Mono SemiBold, blu | 15 px; cifre e simboli allineati |
| Etichette | IBM Plex Mono / Sans maiuscolo | 11,5 px con spaziatura 0,06 em |

Notazione accordi nei mockup: italiana (Do, Re, Mi…, minore con `-`), come nei canzonieri parrocchiali; la notazione anglosassone è un'impostazione.

## Componenti chiave

- Elenco a tabella: indice `001`, titolo, tonalità in riquadro monospazio allineata a destra.
- Tag come piccoli riquadri monospazio maiuscoli.
- Testo con numeri di riga e filetto verticale nel margine.
- Navigazione con barra superiore di 2 px sull'elemento attivo.
- Pulsanti e campi rettangolari, nessuna ombra.

## Contrasto misurato (WCAG 2.x)

Calcolato sui valori esadecimali dei token; soglia AA 4,5:1 per testo normale, AAA 7:1.

| Coppia | Chiaro | Scuro | Esito |
|---|---|---|---|
| Liriche su sfondo testo | #101418 su #FFFFFF = **18.50:1** | #E6EAF0 su #0F141A = **15.32:1** | AAA |
| Accordi su sfondo testo | #0B57D0 su #FFFFFF = **6.39:1** | #7CB2FF su #0F141A = **8.52:1** | AA |
| Testo principale su sfondo | #101418 su #EEF0F3 = **16.20:1** | #E6EAF0 su #0B0F14 = **15.92:1** | AAA |
| Testo secondario su sfondo | #4A525D su #EEF0F3 = **6.92:1** | #9AA5B3 su #0B0F14 = **7.69:1** | AA |
| Testo secondario su superficie | #4A525D su #FFFFFF = **7.91:1** | #9AA5B3 su #131920 = **7.08:1** | AAA |
| Testo su pulsante primario | #FFFFFF su #0B57D0 = **6.39:1** | #07111F su #7CB2FF = **8.72:1** | AA |
| Testo su elemento selezionato | #0A3A8A su #DCE7FC = **8.50:1** | #B7D3FF su #16294A = **9.50:1** | AAA |
| Accento come testo su superficie | #0B57D0 su #FFFFFF = **6.39:1** | #7CB2FF su #131920 = **8.14:1** | AA |

Tutte le coppie di testo superano AA in entrambi i temi.

## Leggibilità sul leggio

- Circa 35 caratteri per riga: poche righe spezzate.
- Leggibile e ordinato, ma liriche a 18 px sono il minimo per la distanza del leggio: lo slider testo dovrebbe partire da 20 px sul telefono.
- Accordi e testo distinti per famiglia (mono contro sans) oltre che per colore.

## Fattibilità .NET MAUI

Specifico di questa direzione:

- La più semplice da realizzare: rettangoli, filetti, nessun effetto.
- Numeri di riga e filetto verticale: disegnati dallo stesso controllo della riga accordi.
- Cifre tabulari non disponibili (niente funzioni OpenType): il monospazio risolve.
- Due colonne su tablet: non esiste un layout a colonne, il controllo deve dividere le sezioni tra due `VerticalStackLayout`.

Comune a tutte le proposte:

- Temi: due `ResourceDictionary` (Chiaro/Scuro) con le stesse chiavi dei token qui sotto, applicati con `DynamicResource` e scambiati a runtime; `AppThemeBinding` va bene solo per i pochi colori fissi.
- Riga accordi+testo: un controllo dedicato che misura le sillabe e disegna l'accordo sopra (consigliato `GraphicsView` con `Microsoft.Maui.Graphics`, una vista per riga); evitare una `Label` per ogni sillaba in un `FlexLayout`, che su Android con canti lunghi produce centinaia di viste.
- I pesi di un font vanno registrati come file e alias separati in `MauiProgram` (`AddFont("X-Bold.ttf", "XBold")`): `FontAttributes="Bold"` su Android non sceglie da solo il file giusto e i font variabili vengono resi solo nell'istanza predefinita.
- Icone: un solo set al tratto (Phosphor o Lucide) incluso come font icone e usato con `FontImageSource`, colorato dai token; niente emoji come icone (l'app attuale le usa).

## Font da includere nell'app

- IBM Plex Sans: Regular, Medium, SemiBold, Bold — 4 file, OFL.
- IBM Plex Mono: Medium, SemiBold — 2 file, OFL.

Nel mockup i font arrivano da Google Fonts con ripiego sui font di sistema; nell'app vanno inclusi come file in `Resources/Fonts`.

## Punti di forza

- Massima densità e chiarezza su tablet e desktop, ottima per la preparazione.
- Implementazione rapida e prestazioni ottime su Android.
- Numeri di riga utili nelle prove.

## Punti deboli

- Fredda: poco calore per un coro parrocchiale.
- Testo più piccolo delle altre a parità di schermo, da compensare con le impostazioni.
- Il blu come unico accento si confonde con lo stile di molti strumenti di produttività.
