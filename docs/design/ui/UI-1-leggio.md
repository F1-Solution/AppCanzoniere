# UI 1 — Leggio

> Il canzoniere come un messale: carta calda, serif da lettura, accordi in rosso rubrica.

Mockup: [`UI-1-leggio.html`](UI-1-leggio.html) — quattro schermate telefono, canzone su tablet orizzontale, interruttore chiaro/scuro e tavola dei token.

## Carattere e ispirazione

Il riferimento è il **messale** e il libretto dei canti stampato: carta avorio, inchiostro nero caldo e il **rosso delle rubriche** riservato a ciò che si "esegue", cioè gli accordi, le etichette di strofa e i momenti liturgici. Il ritornello è in corsivo come le risposte dell'assemblea. Pochi ornamenti tipografici fanno da firma: filetti doppi sotto la barra del titolo e sopra i controlli, tag in corsivo separati da punto mediano, raccolta impaginata al centro come un frontespizio. Nessuna ombra, angoli quasi vivi: è l'interfaccia più "libro" delle cinque e la più vicina al mondo del coro parrocchiale.

## Palette

Due soli colori portanti: inchiostro `#1E1A15` e rosso rubrica `#9E2318` su carta `#F6F1E7`. Nel tema scuro la carta diventa un carbone caldo (`#16120E`, non nero puro, per non abbagliare per contrasto con un testo color crema) e il rosso schiarisce verso il salmone `#F29A86`, così resta caldo senza vibrare sul fondo scuro.

| Token | Uso | Chiaro | Scuro |
|---|---|---|---|
| `bg` | Sfondo app | `#F6F1E7` | `#16120E` |
| `surface` | Superficie (card, barre) | `#FFFCF5` | `#1F1A15` |
| `surface2` | Superficie secondaria / campi | `#EFE8DA` | `#2A241D` |
| `lyricsBg` | Sfondo del testo canzone | `#FFFCF5` | `#16120E` |
| `text` | Testo principale e liriche | `#1E1A15` | `#EFE6D6` |
| `text2` | Testo secondario | `#5B5246` | `#B9AD98` |
| `chord` | Accordi | `#9E2318` | `#F29A86` |
| `chordBg` | Fondo chip accordo (se usato) | `transparent` | `transparent` |
| `accent` | Azione primaria / selezione | `#8C1E14` | `#EC8B76` |
| `onAccent` | Testo su accento | `#FFFFFF` | `#1A0E0A` |
| `accentSoft` | Fondo elemento selezionato | `#F2DED6` | `#3D211A` |
| `onAccentSoft` | Testo su fondo selezionato | `#6A160E` | `#F7C2B4` |
| `border` | Bordi e divisori | `#D9CEBB` | `#3B3229` |
| `danger` | Errore / elimina | `#8C1E14` | `#FF9C8A` |
| `focus` | Anello di focus | `#1E1A15` | `#EFE6D6` |
| `rule` | Specifico della direzione | `#D9CEBB` | `#3B3229` |

## Tipografia

| Ruolo | Font | Dimensioni e note |
|---|---|---|
| Liriche e titoli | Literata (serif nato per la lettura su schermo, occhio medio ampio) | 19 px / 1,3, Regular; ritornello in corsivo |
| Accordi | Source Sans 3 Bold, rosso rubrica | 16 px; il bastone dritto stacca dal serif delle liriche e i segni # e b restano nitidi |
| Interfaccia | Source Sans 3 | 16 px corpo, 13,5 px secondario, etichette 12 px maiuscolo spaziato |

Notazione accordi nei mockup: italiana (Do, Re, Mi…, minore con `-`), come nei canzonieri parrocchiali; la notazione anglosassone è un'impostazione.

## Componenti chiave

- Ricerca "a riga": solo filetto inferiore, nessun riquadro.
- Chip filtro rettangolari (raggio 3) con bordo; selezionato = pieno rosso.
- Tonalità in Literata corsivo rosso; pulsanti − / + circolari a filo.
- Raccolta impaginata al centro con titolo corsivo, letture del giorno sotto.
- Note per il coro come citazione con filetto rosso a sinistra.

## Contrasto misurato (WCAG 2.x)

Calcolato sui valori esadecimali dei token; soglia AA 4,5:1 per testo normale, AAA 7:1.

| Coppia | Chiaro | Scuro | Esito |
|---|---|---|---|
| Liriche su sfondo testo | #1E1A15 su #FFFCF5 = **16.88:1** | #EFE6D6 su #16120E = **15.05:1** | AAA |
| Accordi su sfondo testo | #9E2318 su #FFFCF5 = **7.57:1** | #F29A86 su #16120E = **8.64:1** | AAA |
| Testo principale su sfondo | #1E1A15 su #F6F1E7 = **15.37:1** | #EFE6D6 su #16120E = **15.05:1** | AAA |
| Testo secondario su sfondo | #5B5246 su #F6F1E7 = **6.81:1** | #B9AD98 su #16120E = **8.42:1** | AA |
| Testo secondario su superficie | #5B5246 su #FFFCF5 = **7.48:1** | #B9AD98 su #1F1A15 = **7.80:1** | AAA |
| Testo su pulsante primario | #FFFFFF su #8C1E14 = **9.09:1** | #1A0E0A su #EC8B76 = **7.67:1** | AAA |
| Testo su elemento selezionato | #6A160E su #F2DED6 = **9.34:1** | #F7C2B4 su #3D211A = **9.32:1** | AAA |
| Accento come testo su superficie | #8C1E14 su #FFFCF5 = **8.87:1** | #EC8B76 su #1F1A15 = **7.00:1** | AAA |

Tutte le coppie di testo superano AA in entrambi i temi.

## Leggibilità sul leggio

- Circa 33 caratteri per riga a 360 dp: le righe più lunghe vanno a capo una volta.
- Il serif aiuta la lettura distesa a casa; a un braccio di distanza e con poca luce è leggermente meno immediato di un sans iperleggibile, soprattutto il corsivo del ritornello: va previsto un'opzione "ritornello in tondo".
- Rosso su carta e salmone su carbone superano 7:1 (AAA): gli accordi si distinguono per colore **e** per famiglia di carattere, quindi funzionano anche per chi non distingue il rosso.

## Fattibilità .NET MAUI

Specifico di questa direzione:

- Filetto doppio: due `BoxView` alti 1 dp distanziati di 1 dp.
- Campo di ricerca sottolineato: serve un handler per uniformare `Entry` tra Android (sottolineatura nativa) e Windows (riquadro).
- Maiuscoletto non disponibile (MAUI non espone le funzioni OpenType): si usa maiuscolo con `CharacterSpacing`.
- Nessun effetto critico: niente ombre, sfocature o gradienti.

Comune a tutte le proposte:

- Temi: due `ResourceDictionary` (Chiaro/Scuro) con le stesse chiavi dei token qui sotto, applicati con `DynamicResource` e scambiati a runtime; `AppThemeBinding` va bene solo per i pochi colori fissi.
- Riga accordi+testo: un controllo dedicato che misura le sillabe e disegna l'accordo sopra (consigliato `GraphicsView` con `Microsoft.Maui.Graphics`, una vista per riga); evitare una `Label` per ogni sillaba in un `FlexLayout`, che su Android con canti lunghi produce centinaia di viste.
- I pesi di un font vanno registrati come file e alias separati in `MauiProgram` (`AddFont("X-Bold.ttf", "XBold")`): `FontAttributes="Bold"` su Android non sceglie da solo il file giusto e i font variabili vengono resi solo nell'istanza predefinita.
- Icone: un solo set al tratto (Phosphor o Lucide) incluso come font icone e usato con `FontImageSource`, colorato dai token; niente emoji come icone (l'app attuale le usa).

## Font da includere nell'app

- Literata: statici Regular, Italic, SemiBold, SemiBold Italic (taglio 18 pt) — circa 4 file, licenza OFL.
- Source Sans 3: Regular, SemiBold, Bold — 3 file, OFL.

Nel mockup i font arrivano da Google Fonts con ripiego sui font di sistema; nell'app vanno inclusi come file in `Resources/Fonts`.

## Punti di forza

- Identità forte e coerente con il mondo liturgico, riconoscibile a colpo d'occhio.
- Accordi distinti per colore e per famiglia di carattere.
- Implementazione semplice: tutto piatto.

## Punti deboli

- Il serif e il corsivo rendono meno del sans al buio e a distanza.
- Rischio di apparire "solo per la chiesa" a chi la usa per cantautori o musical.
- Su desktop l'impaginazione centrata della raccolta spreca spazio.
