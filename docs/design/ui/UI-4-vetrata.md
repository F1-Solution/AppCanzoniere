# UI 4 — Vetrata

> I colori dei tempi liturgici come una vetrata: blocchi pieni, forme morbide, accordi in pastiglie.

Mockup: [`UI-4-vetrata.html`](UI-4-vetrata.html) — quattro schermate telefono, canzone su tablet orizzontale, interruttore chiaro/scuro e tavola dei token.

## Carattere e ispirazione

Ispirata alle **vetrate** e ai **colori liturgici**: blocchi di colore pieno, forme morbide (raggi 16–26, pillole), card con ombra leggera, testata della raccolta con un motivo a spicchi come un rosone. L'idea chiave è che l'**accento segua il tempo liturgico**: verde nel Tempo Ordinario, viola in Avvento e Quaresima, oro a Natale e Pasqua, rosso a Pentecoste. Gli accordi sono **pastiglie** colorate sopra la sillaba, il ritornello sta in un riquadro tinto: la struttura del canto si vede da lontano come una mappa.

## Palette

Verde liturgico `#0E6B4D` su fondo verde-grigio chiarissimo `#F3F6F2`; accordi verde scuro su pastiglia `#D6EFE3`. Nel tema scuro fondo `#0E1512`, accento `#5FD3A3`, pastiglie `#1C3A2E` con testo `#9BE7C6`. Le varianti per tempo liturgico sono nella tavola dei token.

| Token | Uso | Chiaro | Scuro |
|---|---|---|---|
| `bg` | Sfondo app | `#F3F6F2` | `#0E1512` |
| `surface` | Superficie (card, barre) | `#FFFFFF` | `#16211C` |
| `surface2` | Superficie secondaria / campi | `#E7EEE8` | `#1F2C26` |
| `lyricsBg` | Sfondo del testo canzone | `#FFFFFF` | `#0E1512` |
| `text` | Testo principale e liriche | `#14211B` | `#E8F1EC` |
| `text2` | Testo secondario | `#4A5A52` | `#A5B8AE` |
| `chord` | Accordi | `#0A4A36` | `#9BE7C6` |
| `chordBg` | Fondo chip accordo (se usato) | `#D6EFE3` | `#1C3A2E` |
| `accent` | Azione primaria / selezione | `#0E6B4D` | `#5FD3A3` |
| `onAccent` | Testo su accento | `#FFFFFF` | `#03140C` |
| `accentSoft` | Fondo elemento selezionato | `#D6EFE3` | `#1C3A2E` |
| `onAccentSoft` | Testo su fondo selezionato | `#0A4A36` | `#9BE7C6` |
| `border` | Bordi e divisori | `#D5E0D9` | `#2A3A33` |
| `danger` | Errore / elimina | `#B3261E` | `#FF9A8F` |
| `focus` | Anello di focus | `#0E6B4D` | `#9BE7C6` |
| `rule` | Specifico della direzione | `#E1E8E3` | `#24302B` |
| `s-gold` | Specifico della direzione | `#B98500` | `#F2C14E` |

## Tipografia

| Ruolo | Font | Dimensioni e note |
|---|---|---|
| Titoli | Bricolage Grotesque ExtraBold | 30 px, grottesco espressivo con un tocco artigianale |
| Liriche e interfaccia | Figtree Medium | liriche 19 px / 1,3, aperto e amichevole |
| Accordi | Figtree ExtraBold in pastiglia | 15 px su fondo tinto |

Notazione accordi nei mockup: italiana (Do, Re, Mi…, minore con `-`), come nei canzonieri parrocchiali; la notazione anglosassone è un'impostazione.

## Componenti chiave

- Card arrotondate per ogni canto con banda colorata a sinistra (oro per i canti di Natale).
- Testata della raccolta a tutto colore con motivo a rosone.
- Barra controlli flottante a pillola.
- Momenti della Messa come pillole piene quando selezionati.
- Indicatore del tempo liturgico nella striscia dei metadati della canzone.

## Contrasto misurato (WCAG 2.x)

Calcolato sui valori esadecimali dei token; soglia AA 4,5:1 per testo normale, AAA 7:1.

| Coppia | Chiaro | Scuro | Esito |
|---|---|---|---|
| Liriche su sfondo testo | #14211B su #FFFFFF = **16.63:1** | #E8F1EC su #0E1512 = **16.06:1** | AAA |
| Accordi su sfondo testo | #0A4A36 su #D6EFE3 = **8.46:1** | #9BE7C6 su #1C3A2E = **8.64:1** | AAA |
| Testo principale su sfondo | #14211B su #F3F6F2 = **15.26:1** | #E8F1EC su #0E1512 = **16.06:1** | AAA |
| Testo secondario su sfondo | #4A5A52 su #F3F6F2 = **6.70:1** | #A5B8AE su #0E1512 = **8.87:1** | AA |
| Testo secondario su superficie | #4A5A52 su #FFFFFF = **7.30:1** | #A5B8AE su #16211C = **7.94:1** | AAA |
| Testo su pulsante primario | #FFFFFF su #0E6B4D = **6.50:1** | #03140C su #5FD3A3 = **10.22:1** | AA |
| Testo su elemento selezionato | #0A4A36 su #D6EFE3 = **8.46:1** | #9BE7C6 su #1C3A2E = **8.64:1** | AAA |
| Accento come testo su superficie | #0E6B4D su #FFFFFF = **6.50:1** | #5FD3A3 su #16211C = **8.93:1** | AA |

Tutte le coppie di testo superano AA in entrambi i temi.

## Leggibilità sul leggio

- Circa 34 caratteri per riga.
- Le pastiglie fanno spiccare gli accordi anche con la luce che si riflette sullo schermo, ma occupano qualche pixel in più e possono sovrapporsi con accordi ravvicinati ("Sol Re" su sillabe brevi): il controllo deve garantire uno spazio minimo.
- Il ritornello tinto si ritrova subito alzando lo sguardo.

## Fattibilità .NET MAUI

Specifico di questa direzione:

- Pastiglie accordi: vanno disegnate nel `GraphicsView` (rettangolo arrotondato + testo); con un `Border` per accordo il numero di viste esplode.
- Ombre su ogni card: `Shadow` dentro `CollectionView` su Android costa durante lo scorrimento; meglio ombre solo su testata e barre, card piatte con fondo bianco.
- Motivo a rosone: i gradienti conici non esistono in MAUI (solo lineari e radiali): va fornito come immagine vettoriale ricolorabile (`TintColor`) per ogni tempo liturgico.
- Accento dinamico per tempo liturgico: fattibile scambiando le risorse a runtime, ma richiede un calendario liturgico nel dominio.

Comune a tutte le proposte:

- Temi: due `ResourceDictionary` (Chiaro/Scuro) con le stesse chiavi dei token qui sotto, applicati con `DynamicResource` e scambiati a runtime; `AppThemeBinding` va bene solo per i pochi colori fissi.
- Riga accordi+testo: un controllo dedicato che misura le sillabe e disegna l'accordo sopra (consigliato `GraphicsView` con `Microsoft.Maui.Graphics`, una vista per riga); evitare una `Label` per ogni sillaba in un `FlexLayout`, che su Android con canti lunghi produce centinaia di viste.
- I pesi di un font vanno registrati come file e alias separati in `MauiProgram` (`AddFont("X-Bold.ttf", "XBold")`): `FontAttributes="Bold"` su Android non sceglie da solo il file giusto e i font variabili vengono resi solo nell'istanza predefinita.
- Icone: un solo set al tratto (Phosphor o Lucide) incluso come font icone e usato con `FontImageSource`, colorato dai token; niente emoji come icone (l'app attuale le usa).

## Font da includere nell'app

- Bricolage Grotesque: statici SemiBold ed ExtraBold (taglio ottico 36–48 pt) — 2 file, OFL.
- Figtree: Regular, Medium, SemiBold, ExtraBold — 4 file, OFL.

Nel mockup i font arrivano da Google Fonts con ripiego sui font di sistema; nell'app vanno inclusi come file in `Resources/Fonts`.

## Punti di forza

- La più calorosa e "viva": adatta a cori e gruppi giovani.
- Il colore liturgico dà un significato all'accento, non è decorazione.
- Struttura del canto (pastiglie e ritornello tinto) chiarissima.

## Punti deboli

- La più costosa da realizzare bene in MAUI (pastiglie, ombre, motivi).
- Il colore che cambia con il calendario può disorientare chi non lo conosce: serve un'opzione per bloccarlo.
- Con molte card e ombre l'aspetto può diventare pesante su desktop.
