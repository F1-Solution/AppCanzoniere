# UI 2 — Palco

> Pensato per il buio: nero OLED, caratteri iperleggibili enormi, accordi ambra come le spie di un mixer.

Mockup: [`UI-2-palco.html`](UI-2-palco.html) — quattro schermate telefono, canzone su tablet orizzontale, interruttore chiaro/scuro e tavola dei token.

## Carattere e ispirazione

Una direzione **dark-first** pensata per il palco e per la chiesa in penombra: nero OLED, testo quasi bianco, accordi **ambra** come le spie di un mixer o i LED di un pedale. Tutto è grande e tondo: barra dei controlli a pillola alta 56 dp con angoli superiori arrotondati, chip alti 44 dp, ricerca a pillola. La tipografia è **Atkinson Hyperlegible**, disegnata dal Braille Institute per l'ipovisione: forme di lettere molto differenziate (I/l/1, O/0, rn/m). Il ritornello è in grassetto con una barra ambra a sinistra, così si ritrova in un attimo alzando lo sguardo.

## Palette

Scuro: nero `#000000`, testo `#F7F7F2`, accordi e azione primaria `#FFB224`. Il chiaro è volutamente essenziale: bianco, nero e accordi in ambra scuro `#8A4600`; l'azione primaria diventa nera per non avere un'ambra troppo debole su bianco.

| Token | Uso | Chiaro | Scuro |
|---|---|---|---|
| `bg` | Sfondo app | `#FFFFFF` | `#000000` |
| `surface` | Superficie (card, barre) | `#F3F3F1` | `#121212` |
| `surface2` | Superficie secondaria / campi | `#E8E8E5` | `#1E1E1E` |
| `lyricsBg` | Sfondo del testo canzone | `#FFFFFF` | `#000000` |
| `text` | Testo principale e liriche | `#0A0A0A` | `#F7F7F2` |
| `text2` | Testo secondario | `#45453F` | `#B9B9B0` |
| `chord` | Accordi | `#8A4600` | `#FFB224` |
| `chordBg` | Fondo chip accordo (se usato) | `transparent` | `transparent` |
| `accent` | Azione primaria / selezione | `#0A0A0A` | `#FFB224` |
| `onAccent` | Testo su accento | `#FFFFFF` | `#000000` |
| `accentSoft` | Fondo elemento selezionato | `#FFE3B0` | `#3A2700` |
| `onAccentSoft` | Testo su fondo selezionato | `#4A2600` | `#FFD68A` |
| `border` | Bordi e divisori | `#D0D0CB` | `#2E2E2E` |
| `danger` | Errore / elimina | `#B3261E` | `#FF8A80` |
| `focus` | Anello di focus | `#8A4600` | `#FFD68A` |
| `rule` | Specifico della direzione | `#E1E1DC` | `#262626` |

## Tipografia

| Ruolo | Font | Dimensioni e note |
|---|---|---|
| Liriche e interfaccia | Atkinson Hyperlegible Next | liriche 22 px / 1,25; titoli ExtraBold 28 px |
| Accordi | Atkinson Hyperlegible Mono Bold, ambra | 19 px; il monospazio rende uniformi Sol, Si-, Fa#7 |
| Tonalità | Atkinson Hyperlegible Mono Bold | 30 px nella barra dei controlli |

Notazione accordi nei mockup: italiana (Do, Re, Mi…, minore con `-`), come nei canzonieri parrocchiali; la notazione anglosassone è un'impostazione.

## Componenti chiave

- Barra controlli a pillola con −/+ circolari da 56 dp e tonalità in ambra al centro.
- Selettore chitarra/piano a pillola, selezionato pieno ambra.
- Navigazione inferiore con indicatore a pillola sull'icona attiva.
- Tonalità nell'elenco come anello ambra, leggibile anche scorrendo veloce.

## Contrasto misurato (WCAG 2.x)

Calcolato sui valori esadecimali dei token; soglia AA 4,5:1 per testo normale, AAA 7:1.

| Coppia | Chiaro | Scuro | Esito |
|---|---|---|---|
| Liriche su sfondo testo | #0A0A0A su #FFFFFF = **19.80:1** | #F7F7F2 su #000000 = **19.54:1** | AAA |
| Accordi su sfondo testo | #8A4600 su #FFFFFF = **7.10:1** | #FFB224 su #000000 = **11.65:1** | AAA |
| Testo principale su sfondo | #0A0A0A su #FFFFFF = **19.80:1** | #F7F7F2 su #000000 = **19.54:1** | AAA |
| Testo secondario su sfondo | #45453F su #FFFFFF = **9.65:1** | #B9B9B0 su #000000 = **10.63:1** | AAA |
| Testo secondario su superficie | #45453F su #F3F3F1 = **8.69:1** | #B9B9B0 su #121212 = **9.48:1** | AAA |
| Testo su pulsante primario | #FFFFFF su #0A0A0A = **19.80:1** | #000000 su #FFB224 = **11.65:1** | AAA |
| Testo su elemento selezionato | #4A2600 su #FFE3B0 = **10.75:1** | #FFD68A su #3A2700 = **10.36:1** | AAA |
| Accento come testo su superficie | #0A0A0A su #F3F3F1 = **17.82:1** | #FFB224 su #121212 = **10.39:1** | AAA |

Tutte le coppie di testo superano AA in entrambi i temi.

## Leggibilità sul leggio

- Circa 27 caratteri per riga a 360 dp: più a capo delle altre proposte, compensato dalla dimensione.
- La scelta più leggibile a un braccio di distanza: caratteri grandi, alto contrasto (fino a 19,5:1), lettere difficili da confondere.
- Il nero puro su OLED non emette luce: non disturba il coro né i fedeli e consuma meno batteria.

## Fattibilità .NET MAUI

Specifico di questa direzione:

- Barra con angoli solo superiori: `Border` con `StrokeShape="RoundRectangle 26,26,0,0"`; l'ombra (`Shadow`) su Android ha un costo, va usata solo su questa barra.
- Indicatore a pillola nella navigazione: la `TabBar` di Shell non lo consente, serve una barra inferiore personalizzata (o accettare quella standard).
- Barre di sistema nere: `StatusBarBehavior` del CommunityToolkit e colore della barra di navigazione Android.
- Nessun effetto critico.

Comune a tutte le proposte:

- Temi: due `ResourceDictionary` (Chiaro/Scuro) con le stesse chiavi dei token qui sotto, applicati con `DynamicResource` e scambiati a runtime; `AppThemeBinding` va bene solo per i pochi colori fissi.
- Riga accordi+testo: un controllo dedicato che misura le sillabe e disegna l'accordo sopra (consigliato `GraphicsView` con `Microsoft.Maui.Graphics`, una vista per riga); evitare una `Label` per ogni sillaba in un `FlexLayout`, che su Android con canti lunghi produce centinaia di viste.
- I pesi di un font vanno registrati come file e alias separati in `MauiProgram` (`AddFont("X-Bold.ttf", "XBold")`): `FontAttributes="Bold"` su Android non sceglie da solo il file giusto e i font variabili vengono resi solo nell'istanza predefinita.
- Icone: un solo set al tratto (Phosphor o Lucide) incluso come font icone e usato con `FontImageSource`, colorato dai token; niente emoji come icone (l'app attuale le usa).

## Font da includere nell'app

- Atkinson Hyperlegible Next: Regular, SemiBold, Bold, ExtraBold — 4 file, OFL.
- Atkinson Hyperlegible Mono: Medium, Bold — 2 file, OFL.

Nel mockup i font arrivano da Google Fonts con ripiego sui font di sistema; nell'app vanno inclusi come file in `Resources/Fonts`.

## Punti di forza

- Leggibilità dal vivo migliore delle cinque, a tutti i livelli di luce.
- Accessibilità oltre AA quasi ovunque (AAA nelle coppie principali).
- Componenti grandi e semplici: errori di tocco ridotti con le mani impegnate.

## Punti deboli

- Carattere meno distintivo: rischia di sembrare un'app "tecnica" generica.
- Densità bassa: in libreria si vedono meno canti per schermata.
- Il tema chiaro è austero; per lo studio a casa potrebbe servire un tono più caldo.
