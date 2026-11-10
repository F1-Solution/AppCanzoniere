# Confronto delle cinque proposte visive

Le cinque proposte mostrano le stesse quattro schermate telefono (libreria, canzone, raccolta, metadati) e la canzone su tablet orizzontale, con la stessa struttura: cambiano solo carattere, tipografia, colore, densità e forma. L'architettura dell'informazione è oggetto delle proposte UX in `docs/design/ux/`.

| # | Nome | In una riga | Font liriche / accordi |
|---|---|---|---|
| 1 | [Leggio](UI-1-leggio.md) | Messale: carta calda, serif, accordi rosso rubrica | Literata / Source Sans 3 Bold |
| 2 | [Palco](UI-2-palco.md) | Dark-first, nero OLED, iperleggibile, accordi ambra | Atkinson Hyperlegible Next / Mono |
| 3 | [Spartito](UI-3-spartito.md) | Strumento svizzero, monospazio, denso, blu segnale | IBM Plex Sans / Plex Mono |
| 4 | [Vetrata](UI-4-vetrata.md) | Colori liturgici, forme morbide, accordi in pastiglie | Figtree / Figtree ExtraBold |
| 5 | [Analogico](UI-5-analogico.md) | Amplificatore vintage, condensato, display ambra | Barlow Semi Condensed / Barlow Condensed |

## Valutazione (1 = debole, 5 = ottimo)

| Criterio (peso) | Leggio | Palco | Spartito | Vetrata | Analogico |
|---|:-:|:-:|:-:|:-:|:-:|
| Leggibilità dal vivo (×3) | 3 | **5** | 4 | 4 | 4 |
| Carattere e identità (×1) | **5** | 3 | 3 | 4 | 4 |
| Accessibilità (×2) | 4 | **5** | 4 | 4 | 4 |
| Fattibilità MAUI (×2) | 4 | 4 | **5** | 3 | 3 |
| Resa su tablet e desktop (×1) | 4 | 4 | **5** | 3 | 4 |
| Affinità con il coro parrocchiale (×1) | **5** | 3 | 2 | 4 | 2 |
| **Totale pesato (su 50)** | 39 | **43** | 40 | 37 | 36 |

Note sui criteri:

- **Leggibilità dal vivo**: dimensione delle liriche, caratteri per riga a 360 dp (Leggio ~33, Palco ~27, Spartito ~35, Vetrata ~34, Analogico ~37), distinguibilità delle lettere, resa del tema scuro.
- **Accessibilità**: tutte superano WCAG AA nelle coppie di testo in entrambi i temi (tabelle nei singoli documenti); Palco arriva ad AAA quasi ovunque e usa un font progettato per l'ipovisione.
- **Fattibilità MAUI**: Spartito è tutto rettangoli e filetti; Vetrata richiede pastiglie disegnate, motivi vettoriali e ombre da dosare; Analogico richiede componenti a doppio strato.

## Raccomandazione

**Proposta 2 — Palco.** La schermata che conta è la canzone letta a un braccio di distanza con poca luce, e Palco è l'unica progettata a partire da questo: liriche a 22 px in un font nato per l'ipovisione, accordi ambra in monospazio, contrasto AAA, nero OLED che non illumina il coro, controlli da 56 dp usabili senza guardare. È anche realizzabile in MAUI senza effetti critici. Il suo limite è un carattere meno riconoscibile e un tema chiaro austero; si può compensare prendendo da **Leggio** la carta calda e il rosso rubrica per il tema chiaro (studio a casa), mantenendo Palco per il tema scuro (uso dal vivo), se il proprietario accetta un ibrido.

Seconda scelta: **Spartito**, se la priorità diventa la preparazione su desktop e tablet; **Leggio**, se conta di più l'identità liturgica.

## Decisioni per il proprietario

1. Direzione principale, ed eventuale ibrido (es. tema chiaro di Leggio + tema scuro di Palco).
2. Notazione accordi predefinita: italiana (Do, Re, Si-) come nei mockup oppure anglosassone; minore scritto `Si-` o `Sim`.
3. Ritornello: corsivo (Leggio), grassetto (Palco) o riquadro tinto (Vetrata) — incide sulla lettura dal vivo.
4. Solo per Vetrata: accetta che il colore dell'app cambi con il tempo liturgico (richiede un calendario liturgico nei dati)?
5. Barra di navigazione: inferiore personalizzata (necessaria per gli indicatori a pillola di Palco e Vetrata) o quella standard di Shell.
