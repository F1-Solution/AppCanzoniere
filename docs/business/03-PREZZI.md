# 03 — Prezzi: posizionamento, confine gratis/Pro, Piano Coro e proiezioni

Data: 10 novembre 2026. Autore: Pricing Analyst (agente).
Documenti di base: `docs/business/01-RICERCA-MERCATO.md` (citato **[R §x]**, fonti **[Sn]**), `docs/business/02-STRATEGIA.md` (citato **[STR §x]**, fonti **[Tn]**), `docs/PROMPT-INIZIALE.md` (elenco funzioni). Fonti nuove di questo documento: **[Pn]**, elenco in fondo.

**Come leggere il documento**
- **[VERIFICATO]**: controllato oggi su una fonte primaria (sito del produttore, pagina ufficiale dello store).
- **[FONTE SECONDARIA]**: trovato su aggregatori, stampa o siti di monitoraggio prezzi; ordine di grandezza affidabile, cifra esatta da ricontrollare.
- **[IPOTESI]**: mio calcolo o stima, con le premesse scritte.
- **[DA VERIFICARE]**: punto che deve controllare un commercialista, un legale o la Play Console al momento del lancio.

Nessuna cifra di questo documento è una consulenza fiscale o legale.

---

## 0. Raccomandazione in una pagina

| Decisione | Raccomandazione | Fascia |
|---|---|---|
| **Versione gratuita** | Tutto ciò che serve a **suonare** è gratuito e senza scadenza: lettura, trasposizione, Do-Re-Mi, Modalità Palco, scalette della Messa, condivisione con QR. Limite: **30 canti propri** (creati o importati); i canti ricevuti dal coro e quelli di dominio pubblico non contano. Nessuna pubblicità | Quota da testare tra 20 e 50 |
| **Sblocco Pro** (una tantum, per piattaforma) | **9,99 €**. Sblocca canti illimitati, import dell'intero libretto (PDF/OCR in blocco), pedali Bluetooth, modelli di scaletta personalizzati, stampa ed esportazione in PDF, backup automatico | **7,99–12,99 €** |
| **Prezzo di lancio Pro** | **6,99 €** per 90 giorni dal lancio su ogni piattaforma, con data di fine scritta | — |
| **Piano Coro** (annuale) | **49 €/anno per coro** (1 gruppo, fino a 25 membri, Pro incluso per 2 responsabili). **Piano Parrocchia 99 €/anno** (fino a 4 gruppi e 100 membri) | Coro 35–69 €; Parrocchia 79–129 € |
| **Prezzo fondatore Coro** | 35 €/anno per i primi 30 cori, bloccato per 2 rinnovi | — |
| **Diocesi** | Sconto del 20% alle parrocchie di una diocesi convenzionata; licenza diocesana in blocco a 25 €/coro/anno (minimo 20 cori) fatturata alla diocesi | — |
| **Canali di pagamento** | Pro: solo tramite store. Piano Coro: abbonamento in app **e** fattura alla parrocchia fuori dall'app (vincoli in §4.4) | — |

**Ricavi netti stimati** (dopo IVA e commissioni, prima delle imposte sul reddito):

| Scenario | 12 mesi | 24 mesi | 36 mesi | Cumulato 36 mesi |
|---|---|---|---|---|
| Prudente | 0,1 K€ | 0,45 K€ | 0,7 K€ | **≈ 1,3 K€** |
| Base | 0,5 K€ | 2,0 K€ | 3,7 K€ | **≈ 6,2 K€** |
| Favorevole | 1,5 K€ | 5,6 K€ | 11,1 K€ | **≈ 18,3 K€** |

**Dove non sono d'accordo con la strategia** (dettagli in §8):
1. La versione gratuita non può essere illimitata in tutto: serve una quota sui canti propri, altrimenti la conversione resta sotto il 2%. Suonare resta sempre libero.
2. I pedali Bluetooth vanno in Pro: segmentano bene chi è disposto a pagare e non mettono a rischio la domenica.
3. Il backup manuale va lasciato gratuito; in Pro va solo quello automatico.
4. Il Gate 2 per il Piano Coro va alzato da "≥10 cori lo chiedono" a **≥25 cori con preordine**: sotto questa soglia il Piano Coro non ripaga il legale e l'hosting.
5. La disponibilità a pagare dei cori va misurata già al Gate 0, non dopo. Nello scenario base il Piano Coro vale circa il 74% dei ricavi del terzo anno: è lì il modello di business, mentre il Pro serve da ingresso e da cassa del primo anno.

Il modello in sé (una tantum per il singolo, annuale per il gruppo) **regge ai numeri**: l'abbonamento personale rende meno nei 36 mesi (§3.5).

---

## 1. Confronto prezzi con i concorrenti

### 1.1 Leggio digitale e app per musicisti

| Prodotto | Modello e prezzo | Stato della verifica | Piattaforme |
|---|---|---|---|
| **SongbookPro** | Solo: una tantum **circa 7 $**, versione gratuita limitata a 12 canti; licenza separata per iOS, Android e Windows. Groups: "da 4 $ per utente all'anno", piani Band (6 utenti), Ensemble (12), Orchestra (20) con prezzo non mostrato; 40 $/anno ogni 10 utenti in più; prova di 21 giorni; **Groups venduto solo dal sito, non in app** | [VERIFICATO] [P6]; prezzi dei singoli piani Groups non visibili | Android, iOS, Windows |
| **MobileSheets** (Zubersoft) | Una tantum per piattaforma. Google Play **15,99 $** [FONTE SECONDARIA, P8]; Windows 12,99 $ secondo una recensione non recente [P9]. Prova Android: 8 canti, niente sync. Una licenza per piattaforma; sconti a volume per gruppi dal negozio Zubersoft | Condizioni [VERIFICATO] [P7]; importi solo da fonti secondarie | Android, Windows, iOS, e-ink |
| **SongBook** (LinkeSOFT) | Una tantum. iOS **14,99 $** [S9]; Windows Tablets (Microsoft Store) 9,99 $ [FONTE SECONDARIA, P10]; Android **non trovato**. Licenze di sito e sconti a volume su richiesta | Parziale | Android, iOS, Windows, Mac |
| **forScore** | Una tantum **24,99 $**; forScore Pro **14,99 $/anno**; pass di 30 giorni 2,99 $ | [VERIFICATO] sulla pagina di supporto ufficiale (aggiornata a febbraio 2025) e sulla scheda App Store [P11]; varia per paese | Solo Apple |
| **OnSong** | Essentials 29,99 $/anno; Premium 59,99 $/anno | [S1] | iOS, Android, Web |
| **BandHelper** | Solo da 16 $/anno; gruppi 2-5 utenti 4-8 $/mese | [S19] | iOS, Android, Web |
| **Planning Center Music Stand** | Componente aggiuntivo **5 $/mese** (60 $/anno), sopra un piano Services; piano Services gratuito fino a 5 membri del team; prezzi dei piani più grandi non mostrati | [VERIFICATO] [P13]; una fonte secondaria indica 10 $/mese per la fascia da 150 membri | Web, iOS, Android |
| **Chordify** | Premium circa **8–9 $/mese** o **42–54 $/anno** secondo il paese | [FONTE SECONDARIA, P12]: nessuna pagina prezzi ufficiale trovata | Web, iOS, Android |
| **Ultimate Guitar** | Pro 9,99 $/mese o 59,99 $/anno | [S13] | Web, iOS, Android |

### 1.2 App italiane e mondo liturgico

| Prodotto | Prezzo | Nota per il prezzo |
|---|---|---|
| **Canti** (Editrice Nuovo Mondo) | 1,79 € Play, 1,99 € iOS [S27, S28] | Unica app liturgica italiana a pagamento con valutazioni significative (3,49★) |
| **Canzoniere Italiano Pro** | A pagamento, importo non reperito [S38] | Non liturgico |
| **IL Canzoniere Scout** | Freemium; licenza "Gruppo Scout" annunciata come "in arrivo", **prezzo non pubblicato**; la versione gratuita limita la condivisione a 25 utenti [P15] | È l'unico modello di gruppo italiano, ma senza prezzo non fa da ancora |
| **Risuscitò, Canti Cristiani, Canta e Cammina, Canti Liturgici…** | Gratuite [S30–S37] | Fissano l'aspettativa "app di canti = gratis" |
| **NuoviConvertiti** (web) | Gratuito con donazioni [S41] | Concorrente gratuito con trasposizione e "modalità live" |
| **Canto App** (mondo ispanico) | 30 $/anno [S40] | Prova che una nicchia cattolica accetta un abbonamento |
| **Convenzione CEI-Soundreef** | 152,50 €/anno per parrocchia (tariffa non verificata come attuale) [S64] | Ordine di grandezza di una spesa "musicale" annuale già approvata dalle parrocchie |

### 1.3 Commissioni degli store per un piccolo sviluppatore

| Store | Commissione oggi | Fonte e stato |
|---|---|---|
| **Google Play, utenti SEE, dal 30/06/2026** | **Commissione di servizio 10%** sui primi 1 M$ di guadagni annui (acquisti una tantum, abbonamenti, link esterni e fatturazione alternativa) **+ commissione di fatturazione 5%** se si usa Google Play Billing. Totale con Play Billing: **15%**. Oltre 1 M$: 20% (nuove installazioni) o 25% (installazioni esistenti) + 5% sugli acquisti non ricorrenti; 10% + 5% sugli abbonamenti | [VERIFICATO] [P1, P2, P3]. **[DA VERIFICARE]** in Play Console se il livello "primi 1 M$" richiede un'iscrizione (com'era per il vecchio livello 15%) |
| Google Play, resto del mondo (fino all'estensione del nuovo schema) | 15% sul primo 1 M$; 15% sugli abbonamenti | [VERIFICATO] [P1] |
| Google Play, programma "Apps Experience" | Tariffe ridotte (15%/20%) **oltre** il primo milione; irrilevante per noi | [VERIFICATO] [P2, P3] |
| **Microsoft Store**, app non-gioco | **15%** se si usa il sistema di pagamento Microsoft; **0%** se l'app usa un proprio sistema di pagamento | [FONTE SECONDARIA] [P14]: annunci 2021 e Build 2025, ripresi ad agosto 2026; **nessun documento Microsoft 2026 trovato**. Iscrizione gratuita per i singoli [T3] |
| Apple App Store (UE, dal 1/10/2026) | 15% per il Small Business Program [T5] | Non verificato da me; rilevante solo se si fa iOS |

**Conseguenza sul netto** [IPOTESI, da confermare con il commercialista]: lo store versa l'IVA sulle vendite ai consumatori UE e trattiene la commissione sul prezzo senza IVA. Netto per lo sviluppatore ≈ **P ÷ 1,22 × 0,85 ≈ 0,70 × P**. È lo stesso fattore usato nella strategia [STR §3.1], ora confermato con le tariffe 2026.

**Fatturazione alternativa su Play (SEE)**: evita il 5% ma non il 10%, e richiede un proprio fornitore di pagamenti (es. Stripe, circa 1,5% + 0,25 € in UE [IPOTESI]) e la gestione dell'IVA UE (OSS). Su un acquisto da 9,99 € il risparmio netto è di pochi centesimi: **non conviene** per il Pro.

### 1.4 Mappa prezzo/valore

```
 Prezzo per il singolo (una tantum equivalente)
  25 $ │                                   forScore (solo Apple)
       │
  15 $ │  MobileSheets   LinkeSOFT iOS
       │
  10 $ │                          ┌───────────────────────────┐
       │                          │ AppCanzoniere Pro 9,99 €  │
   7 $ │  SongbookPro             └───────────────────────────┘
       │
   2 € │                                         Canti (Nuovo Mondo)
   0   │  siti di accordi, PDF diocesani, Risuscitò, Canta e Cammina, Canzoniere Scout (base)
       └────────────────────────────────────────────────────────────────
         generalista                                      liturgia italiana
```

**Lettura** [IPOTESI]: il Pro si colloca poco sopra SongbookPro (che è generalista, in inglese e senza modello per la Messa) e sotto MobileSheets e LinkeSOFT. Per l'utente italiano il riferimento psicologico non è SongbookPro, che spesso non conosce, ma il "gratis" delle app di canti e dei PDF. Per questo il prezzo deve restare **sotto i 10 €** e la versione gratuita deve essere utile davvero.

---

## 2. Confine tra gratis e Pro

### 2.1 Regola di separazione

- **Convince a installare** (gratis): suonare la domenica senza fogli, trasposizione, scaletta della Messa, ricevere i canti dal proprio animatore.
- **Convince a pagare** (Pro): **volume** (tutto il repertorio della parrocchia), **tempo risparmiato** (libretto intero importato in una volta, stampa delle scalette), **attrezzatura** (pedale).
- **Convince a pagare in modo ricorrente** (Piano Coro): **coordinare un gruppo** (scalette e repertorio sincronizzati per tutti i membri).
- **Mai a pagamento**: nulla che possa bloccare un canto durante la Messa. Una scadenza o un limite che scatta la domenica mattina distrugge la fiducia e le recensioni.
- **Mai pubblicità**: contesto liturgico, presenza di minori nei cori, e i ricavi pubblicitari di un'app da poche migliaia di utenti sono trascurabili [IPOTESI].

### 2.2 Tabella funzione per funzione

| Funzione | Gratis | Pro (9,99 €) | Piano Coro | Motivo |
|---|---|---|---|---|
| Lettura con accordi sopra il testo, schermo intero | ✔ illimitata | | | È il cuore del "leggio"; i siti gratuiti lo offrono già |
| Trasposizione, capotasto, Do-Re-Mi / C-D-E, diesis/bemolle | ✔ | | | Funzione di base presso tutti i concorrenti gratuiti (NuoviConvertiti) |
| Diagrammi chitarra, note degli accordi per pianoforte | ✔ | | | Serve a suonare |
| Modalità Palco: anteprima del canto successivo, scorrimento automatico, ricerca d'emergenza, tasti del volume | ✔ | | | Serve a suonare; nessun rischio la domenica |
| **Pedali Bluetooth/MIDI** (AirTurn e simili, emulazione tastiera) | | ✔ | | Chi compra un pedale è l'utente più motivato e paga senza difficoltà; senza Pro può comunque usare tocco e tasti del volume. **Diverge dalla strategia** (§8) |
| Temi, dimensione del carattere, modalità notte | ✔ | | | Accessibilità: non va mai fatta pagare |
| **Canti propri** (creati a mano o importati) | ✔ **fino a 30** | ✔ illimitati | | Leva principale di conversione nel segmento (SongbookPro: 12; MobileSheets: 8). 30 canti coprono un tempo liturgico (6-8 canti a Messa, 4-6 settimane), non l'intero anno (repertorio parrocchiale di 80-200 canti [IPOTESI]) |
| Canti **ricevuti** dal coro (QR/file) e nucleo di dominio pubblico | ✔ illimitati, non contano nella quota | | | Il membro del coro non deve mai pagare per leggere ciò che gli manda l'animatore: è il motore del passaparola |
| Import da PDF testuale e OCR sul dispositivo | ✔ un canto o una pagina alla volta, entro la quota | ✔ **import in blocco del libretto intero** (rilevamento automatico dei canti, coda di revisione) | | Il gratuito fa vivere il momento "dal PDF al leggio"; il Pro fa risparmiare ore |
| Scalette con modello "Messa" e momenti liturgici, scalette libere | ✔ illimitate | | | È il motivo per cui si sceglie questa app; limitarle annulla il posizionamento |
| Modelli di scaletta personalizzati (feste, matrimoni, funerali, cresime), duplicazione rapida, storico "cosa abbiamo cantato" con rotazione | | ✔ | | Risparmio di tempo per chi prepara ogni settimana |
| **Stampa ed export PDF** di scalette, album, canzoniere impaginato | | ✔ | | Valore concreto: oggi l'animatore fa fotocopie per il coro e il celebrante |
| Album e raccolte manuali, ricerca per titolo, autore, momento, tonalità, riferimento biblico | ✔ | | | Organizzazione di base |
| Condivisione di canti e scalette tramite QR o file (invio e ricezione) | ✔ | | | Diffusione gratuita dentro il coro; la responsabilità sui diritti resta all'utente |
| Export ChordPro di un singolo canto, **backup manuale completo** (file) | ✔ | | | I dati sono dell'utente: non vanno tenuti in ostaggio. **Diverge dalla strategia**, che metteva il backup in Pro (§8) |
| Backup **automatico** pianificato in una cartella scelta (anche una cartella sincronizzata con Drive o OneDrive), ripristino selettivo | | ✔ | | Comodità, non necessità |
| Link a YouTube e MP3 | ✔ | | | Metadati |
| File allegati (PDF, MP3 locali) e lettore per le prove | | ✔ | | Utenti avanzati |
| Suggerimento di canti per momento/tempo liturgico basato su regole locali (tag) | ✔ | | | Mostra il valore del "leggio della Messa", costo zero |
| Indice liturgico pubblico senza testi (quando esisterà) | ✔ | | | Strumento di acquisizione [STR §4.4] |
| Repertorio e scalette **sincronizzati** nel gruppo, ruoli (responsabile/membro), notifica "scaletta pronta", accesso da più piattaforme con account | | | ✔ | Richiede un server: costo ricorrente → prezzo ricorrente |
| Pulizia dell'import con AI in cloud, raccolte automatiche con AI | | | ✔ con limite mensile (§5) | Costo variabile: solo dove c'è un ricavo ricorrente |
| Pro sui dispositivi dei responsabili | | | ✔ (2 nel Coro, 6 nella Parrocchia) | Risolve il problema delle licenze per piattaforma per chi prepara le scalette |
| Riconoscimento audio, accordi suggeriti dall'AI per brani protetti, catalogo pubblico | ✘ | ✘ | ✘ | Esclusi dalla strategia [STR §0] |

**Avviso nella versione gratuita**: al 25° canto proprio compare un messaggio informativo; al 31° l'app chiede lo sblocco **solo al momento di aggiungere** un canto, mai all'apertura o durante la Modalità Palco. I canti già presenti restano sempre leggibili, anche dopo un ripristino da backup.

### 2.3 Windows e licenze per piattaforma

Senza backend non si può collegare un acquisto Google a una licenza Microsoft. Proposta [IPOTESI]:
- **v1**: Pro Windows venduto separatamente sul Microsoft Store a **9,99 €**, come fanno SongbookPro e MobileSheets [P6, P7]. Va detto chiaramente nella scheda dello store per evitare recensioni negative.
- **Con il Piano Coro** (account): i responsabili hanno il Pro su tutte le piattaforme.
- **Da valutare dopo**: una licenza Pro "tutte le piattaforme" venduta dal proprio sito con account. Su Windows la commissione sarebbe 0% (sistema di pagamento proprio) [P14], ma serve un server di licenze.

---

## 3. Prezzo dello sblocco Pro

### 3.1 Raccomandazione

| | Valore |
|---|---|
| **Prezzo di listino** | **9,99 €** (IVA inclusa), una tantum, per piattaforma |
| **Fascia accettabile** | **7,99 € – 12,99 €** |
| **Prezzo di lancio** | **6,99 €** per 90 giorni da ogni lancio (Android, poi Windows), con data di fine visibile |
| **Cori pilota** | Codici promozionali Play gratuiti per gli animatori dei 5 cori pilota (le quote di codici disponibili per trimestre sono [DA VERIFICARE] in Play Console) |
| **Netto per vendita** | 9,99 ÷ 1,22 × 0,85 ≈ **6,96 €** (a 6,99 €: 4,87 €) |

### 3.2 Ragionamento

**Costo** (pilastro 1). Costo variabile diretto quasi nullo: OCR sul dispositivo, nessun server. Il vero costo unitario è il **supporto**: con 6,96 € netti, ogni acquirente che richiede più di 14 minuti di assistenza (a 30 €/ora) azzera il margine sul tempo [IPOTESI]. Servono FAQ, video brevi e procedure automatiche. Il prezzo non deve scendere sotto gli 8 € se il supporto medio supera i 5 minuti per acquirente (§3.3).

**Concorrenti** (pilastro 2). Ancore: SongbookPro ~7 $, MobileSheets 15,99 $, LinkeSOFT 9,99–14,99 $, forScore 24,99 $. 9,99 € sta nella fascia bassa-media del leggio specializzato. Sotto c'è solo l'ancora italiana dei 1,79–1,99 € di "Canti", che però è un archivio di testi, non uno strumento.

**Valore** (pilastro 3). Il lavoro da svolgere è "suonare 6-8 canti ogni domenica senza fogli" [STR §1.1]. Valore economico stimato per l'animatore [IPOTESI]: 1-2 ore risparmiate nell'importare un libretto, fotocopie evitate (vedi §4.2), meno stress quando un canto cambia all'ultimo momento. Con un mercato competitivo (quota di valore catturata 10-25%) e un pubblico di volontari, un prezzo di 8-12 € è coerente. L'utente non ragiona in ore risparmiate: il riferimento percepito è "meno di una pizza, per uno strumento che uso ogni settimana".

**Sensibilità al prezzo** (pilastro 4). Non ci sono dati storici. Il pubblico è sensibile al prezzo (volontari, alternative gratuite ovunque [R §3.5]); la soglia psicologica dei 10 € conta. 9,99 € lascia margine per un lancio a 6,99 € (-30%) senza scendere sotto SongbookPro.

### 3.3 Analisi di sensibilità (±20%)

Base: 1.000 installazioni, conversione del 4% a 9,99 €. Due ipotesi di elasticità della conversione al prezzo: **-0,8** (pubblico che sceglie per valore) e **-1,5** (pubblico molto sensibile). Supporto stimato in 2 € per acquirente (4 minuti a 30 €/ora) [IPOTESI].

| Prezzo | Netto unitario | Conversione (e = -0,8 / -1,5) | Ricavo netto per 1.000 installazioni | Margine dopo il supporto | Note |
|---|---|---|---|---|---|
| 7,99 € (-20%) | 5,57 € | 4,64% / 5,20% | 258 € / 289 € | 166 € / 185 € | Più utenti, margine minimo per ora di supporto |
| 8,99 € (-10%) | 6,26 € | 4,32% / 4,60% | 271 € / 288 € | 184 € / 196 € | |
| **9,99 € (racc.)** | **6,96 €** | **4,00%** | **278 €** | **198 €** | **Il risultato peggiore tra le due ipotesi è il più alto** |
| 10,99 € (+10%) | 7,66 € | 3,68% / 3,40% | 282 € / 260 € | 208 € / 192 € | |
| 11,99 € (+20%) | 8,35 € | 3,36% / 2,80% | 281 € / 234 € | 213 € / 178 € | Sopra la soglia dei 10 €: rischio reale con elasticità alta |

**Lettura**: la curva è **piatta** tra 8,99 e 10,99 € (±5%). Il prezzo del Pro non è la leva principale; contano il volume di installazioni e la quota gratuita. 9,99 € è la scelta più robusta: massimizza il risultato peggiore tra le due ipotesi di elasticità.

### 3.4 Prezzi per paese

| Mercato | Prezzo | Motivo |
|---|---|---|
| Italia, San Marino, Città del Vaticano, resto dell'area euro | 9,99 € | App in italiano: il mercato è quasi solo italiano |
| Svizzera (Ticino) | 9,90 CHF | Arrotondamento locale |
| USA, Regno Unito (solo dopo la versione inglese) | 9,99 $ / 8,99 £ | Vicino a SongbookPro; da rivedere quando ci sarà il posizionamento anglofono |
| Altri paesi | Conversione automatica di Play, con arrotondamento | Volume trascurabile; nessuno sconto per potere d'acquisto finché l'app non è localizzata |

### 3.5 Il modello una tantum regge? Confronto con l'abbonamento personale

Scenario base, 36 mesi, solo singoli (stesse installazioni del §6):

| Modello | Ipotesi | Ricavo netto cumulato 36 mesi |
|---|---|---|
| **Pro una tantum 9,99 €** | Conversione 4% | **≈ 2,3 K€** |
| Abbonamento Pro 4,99 €/anno | Conversione 3% (resistenza agli abbonamenti [S3, S58]), rinnovo 60% | ≈ 1,3 K€ |
| Abbonamento Pro 9,99 €/anno | Conversione 3%, rinnovo 60% | ≈ 2,6 K€, con rischio alto di recensioni negative e conversione reale più bassa |

**Conclusione**: per il singolo l'abbonamento non rende di più in modo significativo entro 36 mesi e porta un rischio di reputazione documentato. Confermo l'una tantum.

---

## 4. Piano Coro

### 4.1 Unità di prezzo

| Unità | Pro | Contro | Giudizio |
|---|---|---|---|
| Per membro (es. 2 €/membro/anno) | Cresce con il valore | Far pagare ogni bambino o ragazzo che si aggiunge è un incentivo sbagliato; i volontari cambiano spesso; il budget della parrocchia diventa imprevedibile | **Scartata** |
| **Per coro, a forfait con tetto di membri** | Semplice da capire e da approvare; chi decide è l'animatore del coro | Un coro molto grande paga come uno piccolo | **Raccomandata** |
| **Per parrocchia/unità pastorale** (più gruppi) | Coincide con chi paga (parroco, consiglio affari economici); copre coro adulti, giovani e bambini | Vendita più lenta | **Raccomandata come secondo livello** |
| Per diocesi | Grande volume con una sola trattativa | Decisioni lente, poche diocesi comprano licenze [STR §2] | **Solo su richiesta** (§4.5) |

### 4.2 Prezzo raccomandato

| Piano | Contenuto | Prezzo | Fascia |
|---|---|---|---|
| **Coro** | 1 gruppo, fino a 25 membri, Pro incluso per 2 responsabili, repertorio e scalette sincronizzati, AI con limite mensile | **49 €/anno** | 35–69 € |
| **Parrocchia** | Fino a 4 gruppi e 100 membri, Pro per 6 responsabili | **99 €/anno** | 79–129 € |
| **Prezzo fondatore** | Primi 30 cori: Coro a 35 €/anno, bloccato per 2 rinnovi | 35 € | — |
| Prova | 30 giorni gratuiti, senza carta se acquistato con fattura | — | — |

Membri del coro: **sempre gratis**, con l'app gratuita.

**Ancoraggio**:
- SongbookPro Groups: 4 $/utente/anno → 25 utenti ≈ 100 $/anno [P6].
- Planning Center Music Stand: 60 $/anno più l'abbonamento Services [P13].
- BandHelper gruppi: 48–96 $/anno [S19].
- Canto App: 30 $/anno **per utente** [S40].

49 € per 25 membri sono **circa 2 € a membro all'anno**, meno della metà di SongbookPro Groups: è coerente con un pubblico di volontari italiani.

**Valore** [IPOTESI da validare nelle interviste]:
- Fotocopie delle scalette: 20 membri × 2 pagine × 45 domeniche ≈ 1.800 copie ≈ 90 € a 5 centesimi.
- Tempo dell'animatore: 1 ora a settimana in meno per preparare e distribuire.
- Valore stimato per il coro: 100–200 €/anno. Con una quota catturata del 25–40% (prodotto differenziato, nessuna alternativa italiana) si arriva a **40–60 €**.

**Rapporto Pro/Coro**: 49 € ≈ 5 sblocchi Pro. L'animatore vede che per il gruppo spende come per 5 colleghi, e in più ottiene la sincronizzazione.

### 4.3 Analisi di sensibilità (±20%)

Base: 70 cori a 49 € (scenario base, anno 3). Netto medio per coro **39 €** (mix dei canali, §4.4). Costo variabile per coro: 15 € di supporto (30 minuti l'anno) + 3 € di AI (§5) = **18 €** [IPOTESI]. Elasticità: **-0,6** (spesa piccola per una parrocchia) e **-1,2**.

| Prezzo | Netto per coro | Cori (e = -0,6 / -1,2) | Ricavo netto | Margine di contribuzione |
|---|---|---|---|---|
| 39 € (-20%) | 31 € | 78 / 87 | 2.430 € / 2.690 € | 1.020 € / 1.130 € |
| 44 € (-10%) | 35 € | 74 / 78 | 2.600 € / 2.740 € | 1.260 € / 1.330 € |
| **49 € (racc.)** | **39 €** | **70** | **2.730 €** | **1.470 €** |
| 54 € (+10%) | 43 € | 66 / 62 | 2.830 € / 2.650 € | 1.650 € / 1.540 € |
| 59 € (+20%) | 47 € | 62 / 53 | 2.900 € / 2.500 € | 1.790 € / 1.540 € |

**Lettura**: a differenza del Pro, qui il **margine cresce con il prezzo** in entrambe le ipotesi, perché il supporto pesa per coro. Il modello tollera 54–59 €. Raccomando comunque **49 €** perché:
1. resta sotto la soglia psicologica dei 50 €;
2. più cori significano più membri che conoscono l'app (canale verso il Pro);
3. i dati sono solo ipotesi.

Nelle interviste va testato anche 59 € (§7.1).

### 4.4 Chi paga e come

| Canale | Per chi | Netto su 49 € | Vincoli |
|---|---|---|---|
| **Abbonamento annuale in app** (Google Play Billing; Microsoft Store) | L'animatore che paga di tasca propria, eventualmente con rimborso dalla parrocchia | 49 ÷ 1,22 × 0,85 ≈ **34,1 €** | Lo store gestisce IVA, rinnovo e rimborsi. La parrocchia non riceve una fattura intestata, ma solo la ricevuta dello store |
| **Fattura alla parrocchia/ente**, pagamento con bonifico o link di pagamento sul sito | Parrocchia, unità pastorale, associazione, diocesi | ≈ **48–49 €** in regime forfettario (bonifico o circa 1 € di commissione di pagamento) | Vedi vincoli sotto |

**Vincoli delle regole degli store** [DA VERIFICARE sulla policy "Payments" di Google Play al momento del lancio]:
- Google ammette che un'app consenta l'accesso a contenuti pagati altrove: l'utente fa l'accesso e trova il servizio attivo (esempio "consumption-only" nelle FAQ ufficiali) [P4].
- Senza aderire al programma di scelta della fatturazione SEE, l'app **non deve** contenere link, prezzi o inviti ad acquistare fuori dall'app. Si possono linkare solo pagine amministrative (account, privacy, assistenza) che non portano a un pagamento [P4].
- Aderendo al programma SEE di link esterni o fatturazione alternativa, Google applica comunque la **commissione di servizio del 10%** sulle transazioni [P3]. Conviene quindi vendere la fattura alla parrocchia **dal sito, senza link dall'app**, e attivare il piano con un codice o un account.
- Da far confermare: se lo stesso Piano Coro possa essere venduto **sia** in app (Play Billing) **sia** fuori (fattura), con l'app che riconosce entrambi. È una prassi diffusa nelle app con account, ma la formulazione della policy va riletta. SongbookPro, per esempio, vende Groups **solo** dal sito [P6].
- Microsoft Store: con un proprio sistema di pagamento la commissione sulle app non-gioco è dello 0% [P14], da verificare nell'accordo per sviluppatori in vigore.
- Apple (solo se si fa iOS): verificare la regola sui "servizi multipiattaforma" prima di qualsiasi lancio.

**Vincoli fiscali da far verificare a un commercialista** [DA VERIFICARE, tutti]:
1. **Forma di attività**: le vendite continuative tramite store fanno pensare a un'attività abituale, quindi partita IVA. Regime forfettario ipotizzato: ricavi sotto 85.000 €; imposta sostitutiva al 5% per i primi 5 anni (se ci sono i requisiti), poi 15%; coefficiente di redditività 67% per i codici ATECO del software; contributi INPS gestione separata (aliquota diversa se si è già iscritti a un'altra cassa). Esclusione se il reddito da lavoro dipendente supera la soglia di legge (35.000 € nel 2025, da riconfermare).
2. **Fatture alle parrocchie**: fattura elettronica tramite SdI (obbligatoria anche per i forfettari dal 2024); parrocchia spesso senza partita IVA, quindi solo codice fiscale e codice destinatario generico. In forfettario niente IVA, ma **marca da bollo di 2 €** sopra i 77,47 € (riguarda il piano Parrocchia a 99 €, non il Coro a 49 €).
3. **Incassi da Google e Microsoft** (società irlandesi): trattamento IVA dei pagamenti ricevuti (operazione B2B UE), iscrizione al VIES, eventuali obblighi sulle commissioni trattenute (inversione contabile) ed elenchi Intrastat.
4. **Regime IVA ordinario** (se il forfettario non è possibile): la fattura alla parrocchia sconta il 22% di IVA. Il netto del canale fattura scende a circa 40 € e il netto medio per coro a circa 34 € invece di 39 €.
5. **Costo del commercialista**: ipotesi 400–800 €/anno. Pesa molto su ricavi di questa dimensione (§6.4).

**Netto medio per coro usato nelle proiezioni** [IPOTESI]: 50% in app (34,1 €) + 50% con fattura (48,3 €) = 41,2 €; il 20% dei cori ha lo sconto diocesano (-20%) → **≈ 39 €**.

### 4.5 Diocesi

| Formula | Condizioni | Motivo |
|---|---|---|
| **Convenzione diocesana** (senza costo per la diocesi) | Sconto del 20% (Coro a 39 €, Parrocchia a 79 €) alle parrocchie di una diocesi che presenta l'app nei corsi per animatori; codice valido un anno, rinnovabile | Paga il canale, non il prezzo: la diocesi dà accesso a decine di cori [STR §5.3] |
| **Licenza diocesana in blocco** | 25 €/coro/anno, minimo 20 cori (≥ 500 €/anno), fattura unica alla diocesi, rinnovo annuale | Per le diocesi che vogliono offrire lo strumento ai propri cori. Netto più basso per coro, ma nessun costo di acquisizione |
| Versione con repertorio diocesano | Su preventivo (opzione C della strategia) | Fuori dal listino |

**Regole sugli sconti**: ogni sconto ha una scadenza e una motivazione scritta. Non si fanno sconti caso per caso: al loro posto, una sessione di formazione online gratuita per il coro o un mese di prova in più.

---

## 5. Funzioni con costo variabile (OCR, AI)

**Principio**: un ricavo una tantum non può finanziare un costo che si ripete. Le funzioni con costo per uso stanno sul dispositivo oppure dentro il Piano Coro, con un limite.

| Funzione | Soluzione raccomandata | Costo per noi | Alternative scartate o rinviate |
|---|---|---|---|
| **OCR** di scansioni e foto | **Sul dispositivo**: motore di riconoscimento testo di Android (libreria on-device di Google) e OCR integrato di Windows | 0 € | OCR in cloud (Azure Read ≈ 1,5 $ ogni 1.000 pagine [T8]): solo se la qualità sul dispositivo non basta, e solo nel Piano Coro |
| **Riconoscimento di accordi e struttura nell'import** | **Regole deterministiche** (parser ChordPro, riconoscimento delle righe di accordi, Do-Re-Mi): gratis e Pro | 0 € | — |
| **Pulizia dell'import con AI in cloud** | **Solo nel Piano Coro**, tramite un nostro proxy, con un limite di **150 richieste al mese per coro** | ≈ 0,002–0,005 $ a richiesta [T9] → al massimo ≈ 0,75 $/mese, ≈ 9 $/anno per coro al limite; uso medio stimato ≈ 3 €/anno [IPOTESI] | Inclusa senza limiti nel Pro: costo ricorrente senza ricavo ricorrente, ed esposta agli abusi |
| **AI sul dispositivo** (modelli generativi integrati in alcuni Android recenti) | Da valutare per il Pro: costo zero, ma funziona solo su una parte dei dispositivi | 0 € | [DA VERIFICARE] disponibilità sui dispositivi tipici dei volontari |
| **Ricarica di crediti AI** (acquisto consumabile in app, es. 2,99 € per 300 pulizie) | Opzione per chi ha il Pro e non il Piano Coro, **solo** se nelle interviste emerge la domanda | Ogni pacchetto paga i propri costi (margine > 90%) | — |
| **Chiave API dell'utente** | Opzione nascosta nelle impostazioni avanzate, per utenti tecnici | 0 € | Non adatta come soluzione principale: i volontari non creano chiavi API, e gestire chiavi altrui crea problemi di sicurezza e supporto |
| **Raccolte automatiche AI** | Regole locali gratuite (per momento, tempo, tag); versione AI nel Piano Coro con lo stesso limite mensile | Come sopra | — |
| **Riconoscimento audio** | Escluso [STR §0] | — | AudD 2–5 $ ogni 1.000 richieste [T7]: è un costo senza un valore distintivo |

**Protezioni operative**:
- Limite per coro e per giorno.
- Limite di spesa mensile sul conto del fornitore AI.
- Proxy con autenticazione dell'account.
- Nessuna chiave nell'app.
- Messaggio chiaro quando il limite è raggiunto: l'import resta possibile senza AI.

---

## 6. Proiezione dei ricavi netti a 12, 24 e 36 mesi

### 6.1 Ipotesi esplicite

| Ipotesi | Prudente | Base | Favorevole | Motivo |
|---|---|---|---|---|
| Mese 0 | Lancio Android dopo il Gate 0; Windows dal mese 6 | idem | idem | [STR §5.2] |
| Nuove installazioni (Android + Windows) anno 1 / 2 / 3 | 800 / 1.000 / 1.000 | 2.000 / 3.000 / 3.500 | 4.000 / 6.000 / 8.000 | Strategia: 1K / 3K / 8K l'anno [STR §3.3]; nessuna app liturgica italiana supera circa 15K installazioni totali [R §1.2] |
| Conversione a Pro (sulle nuove installazioni dell'anno) | 2% | 4% | 6% | Mediana RevenueCat 2% [S54]; la quota gratuita alza la conversione [R §2.2 H5] |
| Prezzo medio Pro | Anno 1: 9,00 € (25% delle vendite al prezzo di lancio); poi 9,99 € | idem | idem | §3.1 |
| Netto per Pro | 6,27 € (anno 1); 6,96 € | idem | idem | IVA 22% + 15% di commissioni store (§1.3) |
| Piano Coro | Avviato al mese 13; cori attivi a fine anno 2 / 3: **8 / 15** | **30 / 70** | **80 / 200** | 0,03%–0,8% delle 25.600 parrocchie [S43]; strategia 15 / 60 / 250 [STR §3.3] |
| Netto per coro | 39 €/anno | idem | idem | §4.4 (mix dei canali, sconto diocesano; regime forfettario) |
| Ricavo Coro dell'anno | Cori attivi a fine anno × netto (semplificazione: pagamento annuale anticipato) | idem | idem | — |
| Costi operativi | Anno 1: ≈ 75 € (Play 25 $, dominio e sito). Dal lancio del Piano Coro: hosting 225 €/anno + dominio 50 € + **parere legale 2.000 € una tantum** all'inizio dell'anno 2 | idem | idem | Valori medi delle fasce della strategia [STR §3.1, §3.3] |
| Non inclusi | Imposte sul reddito, contributi, commercialista, valore del tempo dello sviluppatore, ricavi B2B (opzione C), iOS, mercato anglofono | — | — | Trattati in §6.4 |

### 6.2 Risultati per anno

**Prudente**

| | Anno 1 (12 m) | Anno 2 (24 m) | Anno 3 (36 m) |
|---|---|---|---|
| Pro venduti | 16 | 20 | 20 |
| Ricavo netto Pro | 100 € | 139 € | 139 € |
| Cori attivi | — | 8 | 15 |
| Ricavo netto Coro | — | 312 € | 585 € |
| **Ricavo netto totale** | **100 €** | **451 €** | **724 €** |
| Costi operativi | 75 € | 2.275 € | 275 € |
| Margine dell'anno | +25 € | −1.824 € | +449 € |
| **Cumulato** | +25 € | −1.799 € | **−1.350 €** |
| *Variante: solo Pro, Piano Coro non avviato* | *+25 €* | *+89 €* | *+89 € → cumulato +203 €* |

**Base**

| | Anno 1 | Anno 2 | Anno 3 |
|---|---|---|---|
| Pro venduti | 80 | 120 | 140 |
| Ricavo netto Pro | 502 € | 835 € | 974 € |
| Cori attivi | — | 30 | 70 |
| Ricavo netto Coro | — | 1.170 € | 2.730 € |
| **Ricavo netto totale** | **502 €** | **2.005 €** | **3.704 €** |
| Costi operativi | 75 € | 2.275 € | 275 € |
| Margine dell'anno | +427 € | −270 € | +3.429 € |
| **Cumulato** | +427 € | +157 € | **+3.586 €** |

**Favorevole**

| | Anno 1 | Anno 2 | Anno 3 |
|---|---|---|---|
| Pro venduti | 240 | 360 | 480 |
| Ricavo netto Pro | 1.505 € | 2.506 € | 3.341 € |
| Cori attivi | — | 80 | 200 |
| Ricavo netto Coro | — | 3.120 € | 7.800 € |
| **Ricavo netto totale** | **1.505 €** | **5.626 €** | **11.141 €** |
| Costi operativi | 75 € | 2.275 € | 275 € |
| Margine dell'anno | +1.430 € | +3.351 € | +10.866 € |
| **Cumulato** | +1.430 € | +4.781 € | **+15.647 €** |

**Peso del Piano Coro sul ricavo dell'anno 3**: prudente 81%, base 74%, favorevole 70%.

### 6.3 Punto di pareggio rispetto ai costi operativi della strategia

| Elemento | Soglia | Raggiunta? |
|---|---|---|
| **Solo Pro** (≈ 75 € l'anno 1, poi ≈ 50 €/anno) | **≈ 11 vendite Pro l'anno** | Sì, in tutti e tre gli scenari |
| **Piano Coro, costi incrementali** (hosting e dominio 275 €/anno + legale 2.000 € ammortizzato su 3 anni = ≈ 940 €/anno) | **≈ 26 cori** paganti (39 € netti − 3 € di AI) | Prudente: **no**; base: dall'anno 2; favorevole: sì |
| Piano Coro, valorizzando anche il supporto (18 €/coro) | ≈ 45 cori | Base: dall'anno 3 |
| **Cumulato, scenario base** | La spesa legale all'inizio dell'anno 2 porta il cumulato a circa −1,6 K€, recuperati entro la fine dell'anno 2 | **Pareggio cumulato verso il mese 22–24** |
| Cumulato, scenario favorevole | Spesa legale recuperata in circa 5 mesi | Sempre positivo a fine anno |
| Cumulato, scenario prudente | Con il Piano Coro avviato: **non raggiunto in 36 mesi** (−1,35 K€). Senza: +0,2 K€ | Il Piano Coro **non va avviato** se i preordini non arrivano a circa 25 cori |

### 6.4 Oltre i costi operativi: fisco e tempo [IPOTESI da confermare]

| Voce | Effetto |
|---|---|
| Imposte e contributi in forfettario (67% × [5% + ~26%], meno la deduzione dei contributi) | ≈ **20–22% del ricavo netto** |
| Commercialista | 400–800 €/anno (uso 600 €) |
| **Cumulato 36 mesi dopo fisco e commercialista** | Prudente (solo Pro): **≈ −1,7 K€**. Base: **≈ +0,5 K€**. Favorevole: **≈ +10 K€** |
| Pareggio "solo Pro" con commercialista | ≈ 97 vendite Pro l'anno: raggiunto solo nello scenario base dall'anno 2 e nel favorevole |
| Valore del tempo (10 ore/mese a 30 €/ora ≈ 3,6 K€/anno [STR §3.3]) | Coperto solo nello scenario favorevole all'anno 3 |

**Conseguenza pratica**: il costo fisso fiscale (partita IVA e commercialista) è il primo ostacolo al pareggio, prima di hosting e legale. Va chiarito con il commercialista **prima** della prima vendita, insieme al momento giusto per aprire la partita IVA.

---

## 7. Come verificare il prezzo prima di investire

### 7.1 Domande per le interviste (Gate 0)

**Comportamento attuale** (prima di parlare di prezzo):
1. "Com'è andata l'ultima domenica: da dove leggevi gli accordi? Chi ha scelto i canti e come li ha fatti avere a voi?"
2. "Quanto tempo passi a settimana a preparare i canti? Cosa ti fa perdere più tempo?"
3. "Quanto spende il coro o la parrocchia in un anno per fotocopie, libretti, raccoglitori? Chi paga?"
4. "Hai mai pagato un'app o un programma per la musica? Quale, quanto, perché?"
5. "Usi un pedale o un tablet al leggio?" (pedale = forte segnale di disponibilità per il Pro)

**Van Westendorp** sullo sblocco Pro, dopo una demo di 5 minuti (dal PDF del libretto alla scaletta in Modalità Palco):
6. "A che prezzo questo sblocco ti sembrerebbe così economico da dubitare della qualità?"
7. "…un affare?"
8. "…caro, ma lo prenderesti comunque?"
9. "…troppo caro per prenderlo?"

Servono almeno 30 risposte per una curva leggibile; con 15–20 interviste è solo un'indicazione.

**Gabor-Granger** sul Piano Coro (solo con animatori e parroci):
10. "Il coro, o la parrocchia, pagherebbe 69 € l'anno per avere scalette e repertorio sincronizzati per tutti i membri?" Se no: 59 €, 49 €, 39 €, 29 €.
11. "Chi approverebbe la spesa? Con quale procedura (rimborso, fattura, carta personale)? Entro quale cifra decidi da solo?"
12. "Se la diocesi la consigliasse nei corsi per animatori, cambierebbe qualcosa?"

**Impegno reale** (vale più delle risposte):
13. Lista d'attesa con **preordine rimborsabile**: Pro a 6,99 € e Piano Coro fondatore a 35 € tramite un link di pagamento sul sito, con rimborso garantito se l'app non esce entro una data. Il pagamento prima del lancio è [DA VERIFICARE] con il commercialista (acconti, fattura).
   Obiettivo Gate 0: **≥ 20 preordini Pro** e **≥ 5 cori fondatori**.

### 7.2 Test su Google Play

| Strumento | Uso | Limite |
|---|---|---|
| **Esperimenti sui prezzi** in Play Console (Monetizza > Prodotti > Esperimenti sui prezzi) [P5] | Test A/B del prezzo del prodotto in app per paese, con intervallo di confidenza e metriche su ricavo e quota di acquirenti | **Volume insufficiente nei primi anni**: per distinguere una conversione del 4% da una del 3% servono circa **5.300 installazioni per variante** (α = 0,05, potenza 80%). Utilizzabile dall'anno 2–3 dello scenario base. Dopo un esperimento bisogna attendere 30 giorni prima di ripeterlo sugli stessi prodotti e paesi |
| **Test in sequenza** | 8 settimane a 9,99 €, poi 8 settimane a 11,99 € (o 8,99 €), confrontando la quota di acquirenti sulle nuove installazioni | Confuso dalla stagionalità liturgica (ripresa a settembre, Avvento, Quaresima): confrontare periodi simili |
| **Quota gratuita** (20 / 30 / 50 canti) | Va decisa con i cori pilota; il test A/B richiede configurazione remota, che oggi non c'è | Senza backend si cambia solo con un aggiornamento dell'app |
| **Esperimenti sulla scheda dello store** | Testare il messaggio ("il leggio della Messa", "dal PDF al tablet") più che il prezzo | — |

### 7.3 Segnali da monitorare

| Segnale | Prezzo **troppo alto** | Prezzo **troppo basso** |
|---|---|---|
| Conversione da installazione a Pro | < 1,5% con uso domenicale alto | > 8–10% |
| Utenti che raggiungono la quota di 30 canti | Molti la raggiungono e non comprano (o disinstallano) | Quasi tutti comprano subito al limite |
| Interviste e Van Westendorp | > 30% indica "troppo caro" al prezzo di listino | Nessuna obiezione; "lo avrei pagato di più"; il prezzo "affare" mediano supera il listino |
| Recensioni | Il prezzo è citato nelle recensioni negative | Nessuna menzione del prezzo, valutazione ≥ 4,5 |
| Rimborsi Play | > 5% delle vendite | — |
| Piano Coro | Rinnovo < 70%; trattative su ogni fattura | Le parrocchie pagano senza discutere; richiesta di più membri o gruppi |
| Supporto | — | Tempo di supporto per acquirente > 14 minuti (il margine sul tempo si annulla) |

**Revisione dei prezzi**:
- Indicatori ogni mese.
- Revisione del prezzo Pro ogni trimestre per il primo anno.
- Revisione del Piano Coro ogni anno a giugno, prima dei rinnovi di settembre (ripresa dell'anno pastorale).
- Ai clienti esistenti del Piano Coro un aumento va comunicato con almeno 60 giorni di preavviso; i prezzi fondatore restano bloccati come promesso.

---

## 8. Differenze rispetto alla strategia (02-STRATEGIA)

| Tema | Strategia | Proposta | Motivo con i numeri |
|---|---|---|---|
| Limiti della versione gratuita | "Tutto ciò che serve a suonare resta gratuito e senza limiti di tempo"; si paga ciò che fa risparmiare tempo | Suonare resta libero, ma **quota di 30 canti propri** | Senza una quota la leva di conversione è debole: si resta alla mediana del 2% o sotto. Con il 2% invece del 4%, lo scenario base perde circa 1,1 K€ di Pro in 36 mesi. I concorrenti che funzionano usano tutti una quota (12 e 8 canti) [P6, P7] |
| Pedali | Implicitamente gratuiti ("suonare") | **Pro** | Segmenta chi è disposto a pagare senza bloccare la domenica (tocco e tasti del volume restano gratuiti). Da riverificare con i pilota |
| Backup | In Pro | **Manuale gratuito, automatico in Pro** | Fiducia e dati dell'utente; la differenza di ricavo è trascurabile |
| Gate 2 (avvio Piano Coro) | ≥ 10 cori chiedono la condivisione | **≥ 25 cori con preordine** (o licenza diocesana) | Sotto circa 26 cori i costi incrementali (legale + hosting ≈ 940 €/anno) non si ripagano; lo scenario prudente con il Piano Coro chiude a −1,35 K€ |
| Quando misurare la disponibilità a pagare dei cori | Dopo, quando i pilota lo chiedono | **Già al Gate 0** (domande 10–13) | Il Piano Coro vale il 70–81% dei ricavi dell'anno 3 in tutti gli scenari: è l'ipotesi da cui dipende il progetto |
| Costi | Hosting, legale, store | Aggiungere **commercialista e fisco** come costo fisso | Nello scenario prudente trasformano un piccolo attivo in una perdita (§6.4) |

**Nessun disaccordo** su: una tantum per il singolo, annuale per il gruppo, OCR sul dispositivo, esclusione del riconoscimento audio, opzione B2B da cogliere se si presenta. Un accordo con un editore o una diocesi (opzione C) resta l'unica leva che sposta lo scenario prudente in modo significativo.

---

## 9. Cosa non è stato possibile verificare

1. **Prezzi esatti dei piani SongbookPro Groups** (Band, Ensemble, Orchestra): non mostrati sulla pagina; visibile solo "da 4 $ per utente all'anno" e 40 $/anno ogni 10 utenti in più [P6].
2. **MobileSheets**: la pagina ufficiale non mostra importi; 15,99 $ su Play è una fonte secondaria [P8]; il prezzo Windows (12,99 $) viene da una recensione non recente [P9].
3. **LinkeSOFT SongBook**: prezzo Android non trovato; Windows 9,99 $ solo da fonte secondaria [P10].
4. **Chordify**: nessuna pagina prezzi ufficiale; 8–9 $/mese e 42–54 $/anno vengono da siti di monitoraggio [P12].
5. **Planning Center Services**: verificati il piano gratuito (5 membri del team) e Music Stand a 5 $/mese; i prezzi dei piani Services più grandi non sono mostrati [P13].
6. **Microsoft Store 2026**: 15% con il sistema di pagamento Microsoft e 0% con un sistema proprio vengono da annunci 2021–2025 ripresi dalla stampa; nessun documento Microsoft del 2026 [P14].
7. **Google Play**: tariffe verificate su fonte primaria, ma da confermare in Play Console: se il livello "primi 1 M$" al 10% richiede un'iscrizione; le quote dei codici promozionali; la possibilità di vendere lo stesso Piano Coro sia in app sia con fattura esterna.
8. **IL Canzoniere Scout**: la licenza di gruppo è annunciata ma senza prezzo [P15].
9. **Tariffa CEI-Soundreef** (152,50 €/anno) non verificata come attuale [S64].
10. **Tutto il §4.4 sul fisco** (forfettario, coefficienti, INPS, bollo, VIES, trattamento degli incassi dagli store): ipotesi da confermare con un commercialista.
11. **Elasticità al prezzo e conversione**: nessun dato storico per questo pubblico; tutte le curve di sensibilità sono ipotesi da sostituire con le interviste e i primi 6 mesi di vendite.
12. **Normativa sugli annunci di prezzo scontato** (es. il "prezzo di lancio" e l'obbligo di indicare il prezzo più basso dei 30 giorni precedenti): [DA VERIFICARE] come si applica a un prodotto nuovo venduto tramite store.

---

## Fonti nuove di questo documento

Consultate il 10/11/2026. Per [S1–S81] vedi `01-RICERCA-MERCATO.md`; per [T1–T10] vedi `02-STRATEGIA.md`.

- [P1] Google Play Console Help — Service fees: https://support.google.com/googleplay/android-developer/answer/112622
- [P2] Google Play Console Help — Lower service fees (tabelle nuove/esistenti installazioni, livello primi 1 M$, programmi): https://support.google.com/googleplay/android-developer/answer/16954621
- [P3] Android Developers Blog — "Expanded billing choice and lower fees on Google Play" (24/06/2026): https://developer.android.com/blog/posts/expanded-billing-choice-and-lower-fees-on-google-play ; programma di scelta della fatturazione: https://support.google.com/googleplay/android-developer/answer/17161464
- [P4] Google Play — Payments policy e FAQ (consumption-only, comunicazioni fuori dall'app): https://support.google.com/googleplay/android-developer/answer/10281818 ; https://support.google.com/googleplay/android-developer/answer/9858738
- [P5] Google Play Console — Price experiments: https://play.google.com/console/about/price-experiments ; Help: https://support.google.com/googleplay/android-developer/answer/13343030 ; annuncio 2023: https://android-developers.googleblog.com/2023/05/price-experiments-in-play-console.html
- [P6] SongbookPro — Pricing: https://songbook-pro.com/pricing
- [P7] Zubersoft — MobileSheets Buy: https://www.zubersoft.com/mobilesheets/buy
- [P8] App Pricing Lab — MobileSheets (Google Play): https://apppricinglab.com/app/google_play/com.zubersoft.mobilesheetspro
- [P9] Windows Central — MobileSheets for Windows 10 review: https://www.windowscentral.com/mobilesheets-review-making-it-easy-read-and-annotate-music
- [P10] LinkeSOFT — Order: https://linkesoft.com/order.html ; Windows Report — SongBook apps for Windows: https://windowsreport.com/songbook-apps-pc/
- [P11] forScore — How to purchase: https://forscore.co/kb/how-to-buy/ ; App Store: https://apps.apple.com/app/forscore/id363738376 ; App Pricing Lab: https://apppricinglab.com/iap/apple/363738376
- [P12] App Pricing Lab — Chordify: https://apppricinglab.com/iap/apple/1073624757 ; AppsHunter: https://appshunter.io/ios/app/chordify-chords-songs-tuner/id1073624757
- [P13] Planning Center — Pricing: https://www.planningcenter.com/pricing ; Music Stand: https://planningcenter.com/musicstand
- [P14] TweakTown — Build 2025, Microsoft Store (15% app, 12% giochi): https://www.tweaktown.com/news/105364/heres-big-play-at-build-2025-its-now-free-for-devs-to-get-apps-or-games-on-microsoft-store/index.html ; PC Gamer — 100% dei ricavi con commerce proprio (2021): https://www.pcgamer.com/windows-store-devs-can-keep-100-of-their-revenue-unless-theyre-making-games ; WN Hub (2026): https://wnhub.io/news/marketing/item-3655
- [P15] App Store — IL Canzoniere Scout: https://apps.apple.com/it/app/il-canzoniere-scout/id6446152727
