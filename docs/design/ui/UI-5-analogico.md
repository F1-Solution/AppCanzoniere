# UI 5 — Analogico

> Il fascino di un amplificatore vintage: crema e tolex, condensato tipografico, display ambra per la tonalità.

Mockup: [`UI-5-analogico.html`](UI-5-analogico.html) — quattro schermate telefono, canzone su tablet orizzontale, interruttore chiaro/scuro e tavola dei token.

## Carattere e ispirazione

Il mondo degli **amplificatori e dei registratori vintage**: crema e tolex nero, bordi spessi 2 px, pulsanti "a tasto" con il bordo inferiore più alto, etichette maiuscole come le serigrafie dei pannelli e la **tonalità su un display ambra**, come un accordatore. La tipografia è tutta della famiglia **Barlow** in larghezze diverse: il condensato non è solo stile, permette liriche a **21 px** con più caratteri per riga di tutte le altre proposte.

## Palette

Chiaro: crema `#EDE5D6`, inchiostro `#1D1A16` (usato anche per i bordi), accordi ruggine `#A8360A`, display `#1D1A16` con cifre `#FFB547`. Scuro: tolex `#191715`, testo crema `#F1E9DA`, accordi arancio `#FF8A3D`; il display resta nero con cifre ambra in entrambi i temi.

| Token | Uso | Chiaro | Scuro |
|---|---|---|---|
| `bg` | Sfondo app | `#EDE5D6` | `#191715` |
| `surface` | Superficie (card, barre) | `#F7F2E8` | `#23201D` |
| `surface2` | Superficie secondaria / campi | `#E2D8C5` | `#2D2925` |
| `lyricsBg` | Sfondo del testo canzone | `#F7F2E8` | `#191715` |
| `text` | Testo principale e liriche | `#1D1A16` | `#F1E9DA` |
| `text2` | Testo secondario | `#554D41` | `#BFB4A2` |
| `chord` | Accordi | `#A8360A` | `#FF8A3D` |
| `chordBg` | Fondo chip accordo (se usato) | `transparent` | `transparent` |
| `accent` | Azione primaria / selezione | `#1D1A16` | `#FF8A3D` |
| `onAccent` | Testo su accento | `#F7F2E8` | `#191715` |
| `accentSoft` | Fondo elemento selezionato | `#F1D5BF` | `#3E2716` |
| `onAccentSoft` | Testo su fondo selezionato | `#6E2406` | `#FFC49B` |
| `border` | Bordi e divisori | `#1D1A16` | `#5E554B` |
| `danger` | Errore / elimina | `#A1260F` | `#FF9580` |
| `focus` | Anello di focus | `#A8360A` | `#FFC49B` |
| `rule` | Specifico della direzione | `#BFB29B` | `#3A352F` |
| `lcdBg` | Specifico della direzione | `#1D1A16` | `#0B0A09` |
| `lcdFg` | Specifico della direzione | `#FFB547` | `#FFB547` |

## Tipografia

| Ruolo | Font | Dimensioni e note |
|---|---|---|
| Titoli ed etichette | Barlow Condensed Bold, maiuscolo | 30 px titoli, 13–15 px etichette spaziate |
| Liriche | Barlow Semi Condensed Medium | 21 px / 1,2: grande ma compatto |
| Accordi | Barlow Condensed Bold, ruggine/arancio | 19 px |
| Interfaccia | Barlow | 16 px |

Notazione accordi nei mockup: italiana (Do, Re, Mi…, minore con `-`), come nei canzonieri parrocchiali; la notazione anglosassone è un'impostazione.

## Componenti chiave

- Display tonalità nero con cifre ambra (anche nei numeri della scaletta e nei badge tonalità).
- Pulsanti a tasto con bordo inferiore di 5 px.
- Etichette di sezione come nastro adesivo colorato ("STROFA 1").
- Testata della raccolta con ombra netta spostata, come una scheda stampata.
- Navigazione con elemento attivo pieno.

## Contrasto misurato (WCAG 2.x)

Calcolato sui valori esadecimali dei token; soglia AA 4,5:1 per testo normale, AAA 7:1.

| Coppia | Chiaro | Scuro | Esito |
|---|---|---|---|
| Liriche su sfondo testo | #1D1A16 su #F7F2E8 = **15.53:1** | #F1E9DA su #191715 = **14.82:1** | AAA |
| Accordi su sfondo testo | #A8360A su #F7F2E8 = **5.89:1** | #FF8A3D su #191715 = **7.62:1** | AA |
| Testo principale su sfondo | #1D1A16 su #EDE5D6 = **13.85:1** | #F1E9DA su #191715 = **14.82:1** | AAA |
| Testo secondario su sfondo | #554D41 su #EDE5D6 = **6.65:1** | #BFB4A2 su #191715 = **8.74:1** | AA |
| Testo secondario su superficie | #554D41 su #F7F2E8 = **7.46:1** | #BFB4A2 su #23201D = **7.92:1** | AAA |
| Testo su pulsante primario | #F7F2E8 su #1D1A16 = **15.53:1** | #191715 su #FF8A3D = **7.62:1** | AAA |
| Testo su elemento selezionato | #6E2406 su #F1D5BF = **7.83:1** | #FFC49B su #3E2716 = **9.04:1** | AAA |
| Accento come testo su superficie | #1D1A16 su #F7F2E8 = **15.53:1** | #FF8A3D su #23201D = **6.91:1** | AA |

Tutte le coppie di testo superano AA in entrambi i temi.

## Leggibilità sul leggio

- Circa 37 caratteri per riga a 21 px: il miglior compromesso tra dimensione e righe spezzate.
- I condensati restano molto leggibili in grassetto medio; i titoli tutti maiuscoli sono invece più lenti da leggere, accettabile perché brevi.
- Il display ambra della tonalità si legge da lontano anche al buio.

## Fattibilità .NET MAUI

Specifico di questa direzione:

- Bordo inferiore più spesso: `Border` non ha spessori diversi per lato; si ottiene con due `Border` sovrapposti (fondo scuro spostato di 3 dp).
- Ombra interna del display: MAUI ha solo ombre esterne; si usa un gradiente lineare scuro o un pannello piatto.
- Ombra netta spostata: un secondo `Border` dietro, spostato di 5 dp (più leggero di `Shadow`).
- Bordo tratteggiato: `StrokeDashArray` è supportato.

Comune a tutte le proposte:

- Temi: due `ResourceDictionary` (Chiaro/Scuro) con le stesse chiavi dei token qui sotto, applicati con `DynamicResource` e scambiati a runtime; `AppThemeBinding` va bene solo per i pochi colori fissi.
- Riga accordi+testo: un controllo dedicato che misura le sillabe e disegna l'accordo sopra (consigliato `GraphicsView` con `Microsoft.Maui.Graphics`, una vista per riga); evitare una `Label` per ogni sillaba in un `FlexLayout`, che su Android con canti lunghi produce centinaia di viste.
- I pesi di un font vanno registrati come file e alias separati in `MauiProgram` (`AddFont("X-Bold.ttf", "XBold")`): `FontAttributes="Bold"` su Android non sceglie da solo il file giusto e i font variabili vengono resi solo nell'istanza predefinita.
- Icone: un solo set al tratto (Phosphor o Lucide) incluso come font icone e usato con `FontImageSource`, colorato dai token; niente emoji come icone (l'app attuale le usa).

## Font da includere nell'app

- Barlow: Regular, Medium, SemiBold — 3 file, OFL.
- Barlow Semi Condensed: Medium, SemiBold — 2 file, OFL.
- Barlow Condensed: SemiBold, Bold — 2 file, OFL.

Nel mockup i font arrivano da Google Fonts con ripiego sui font di sistema; nell'app vanno inclusi come file in `Resources/Fonts`.

## Punti di forza

- Più testo per riga a parità di dimensione: meno a capo sul telefono.
- Personalità forte e "musicale", fuori dai cliché delle app di chiesa.
- Il display della tonalità è un elemento memorabile e funzionale.

## Punti deboli

- Stile caratterizzato: può stancare o sembrare un gioco a chi cerca sobrietà liturgica.
- Più lavoro sui componenti (tasti a doppio strato, display).
- Il maiuscolo diffuso riduce un po' la leggibilità di titoli lunghi.
