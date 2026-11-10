# 02 — Strategia: fattibilità commerciale e opzioni per AppCanzoniere

Data: 10 novembre 2026. Autore: Business Strategist (agente).
Documenti di base: `docs/PROMPT-INIZIALE.md` (requisiti), `docs/design/ux/CONFRONTO-UX.md` (direzione "Scaletta + modello Messa", non ancora approvata), `docs/FASI-PRECEDENTI.md`, `docs/VALUTAZIONE.md` (stato reale del codice), `docs/business/01-RICERCA-MERCATO.md` (dati di mercato, citati qui come **[R §x]** con le sue fonti **[Sn]**).
Il prezzo non è trattato: lo definirà il Pricing Analyst. Dove servono cifre per stimare l'ordine di grandezza uso un prezzo simbolico **P** (singolo) e **G** (gruppo), con intervalli presi da [R §2.2, H4]; non sono raccomandazioni di prezzo.

**Come leggere il documento**
- **[FATTO]**: verificato in una fonte citata (fonti nuove **[Tn]** in fondo; le **[Sn]** sono in 01-RICERCA-MERCATO).
- **[IPOTESI]**: mio ragionamento o stima, con le premesse scritte.
- **[NON VERIFICATO]**: cercato senza riuscire a confermarlo, oppure da far verificare (legale, commercialista, policy degli store).

---

## 0. Raccomandazione in una pagina

**Verdetto: PROCEDERE A CONDIZIONI**, come micro-prodotto di nicchia che si ripaga i costi vivi e può produrre un reddito accessorio. **Non** come attività a tempo pieno: con i dati disponibili il mercato italiano vale da qualche centinaio a qualche decina di migliaia di euro l'anno [R §2.2].

| Domanda | Risposta |
|---|---|
| **Dove competere** | Animazione musicale liturgica italiana: chitarristi e tastieristi di cori parrocchiali, gruppi giovani, scout e oratori, su Android (poi Windows). |
| **Come vincere** | "Il leggio della Messa": scaletta per momenti liturgici, notazione Do-Re-Mi, offline per default, dal PDF della propria parrocchia al tablet sul leggio in pochi minuti. È quello che né SongbookPro (generalista, inglese) né le app italiane (archivi senza esecuzione dal vivo) fanno bene insieme. |
| **Cosa NON fare** | Nessun catalogo pubblico di testi e accordi; niente riconoscimento audio; niente "accordi suggeriti dall'AI" per brani protetti; niente iOS, sync cloud multi-provider e backend nella prima versione; nessun marketing al pubblico non liturgico. |
| **Modello di business** | Ibrido in due tempi: **(1)** app gratuita + sblocco **Pro una tantum** per il singolo, senza backend; **(2)** **Piano Coro** annuale (paga l'animatore o la parrocchia) solo quando i cori pilota lo chiedono. Alternativa: accordo B2B con un editore, un movimento o una diocesi. |
| **Catalogo pubblico** | È il **rischio principale, non il valore principale**. Sostituirlo con: libreria privata, condivisione chiusa dentro il coro, un piccolo nucleo di canti di dominio pubblico e un **indice liturgico pubblico senza testi** (titolo, autori, momenti, riferimenti biblici, dove trovarlo). |
| **Prima di scrivere altro codice** | 15-20 interviste ad animatori e 5 cori pilota disposti a provarlo (Gate 0, §5.5). |

---

## 1. Analisi competitiva strategica

### 1.1 Definizione del mercato

- **Cliente**: chi suona da accordi (chitarra, tastiera) durante la Messa e altre celebrazioni, di solito volontario, spesso anche chi sceglie i canti (animatore musicale). Non "tutti i musicisti".
- **Lavoro da svolgere (JTBD)**: "Domenica alle 10 devo suonare 6-8 canti scelti in settimana, in tonalità adatte al coro, senza sfogliare fogli, anche se il celebrante cambia un canto all'ultimo momento".
- **Alternative reali**, in ordine di diffusione [IPOTESI basata su R §1.2]: (1) libretto parrocchiale stampato o PDF; (2) fotocopie e raccoglitori; (3) siti gratuiti di accordi (NuoviConvertiti, Animatamente…); (4) app generaliste (SongbookPro, OnSong, Ultimate Guitar); (5) app italiane di movimento o di parrocchia. **Il concorrente da battere è il PDF/libretto, non un'altra app.**

### 1.2 Dove sono forti i concorrenti

| Gruppo | Forza reale | Debolezza sfruttabile | Minaccia |
|---|---|---|---|
| **SongbookPro** | Android + Windows + iOS, ChordPro, scalette, pedali; ~7 $ una tantum; 692K installazioni, 4,63★ [S6, S20] | Generalista, nessun modello liturgico, nessuna notazione Do-Re-Mi nativa [IPOTESI: non verificata nell'app], interfaccia in inglese, nessun import pensato per i libretti italiani | **Alta**: è l'alternativa che un chitarrista esperto sceglie già oggi. Se aggiungesse l'italiano e il Do-Re-Mi, il vantaggio si ridurrebbe a liturgia + import |
| **OnSong, BandHelper, Planning Center** | Molto completi; integrati con CCLI/SongSelect [S1, S5, S19] | Abbonamenti cari, pensati per chiese USA con staff; resistenza documentata agli abbonamenti [S3, S58] | Bassa in Italia |
| **Ultimate Guitar, Chordify, Moises** | Catalogo pop licenziato, AI per accordi, decine di milioni di installazioni [S14, S15, S22, S23] | Repertorio liturgico italiano scarso; non organizzano una Messa | Media: rendono **non distintivi** gli "accordi da AI" |
| **App italiane liturgiche** (Canti, Risuscitò, Canzoniere Scout, Canta e Cammina…) | Repertorio del movimento con diritti; distribuzione interna; Canta e Cammina ha scalette con QR [S29-S36] | ≤15K installazioni, pochi voti, spesso senza accordi o scalette; le più vicine sono solo iOS o legate a un movimento [R §1.2] | Media-bassa: piccole, ma gratuite |
| **Siti gratuiti** (NuoviConvertiti) | Gratis, trasposizione e "modalità live", canti caricati dagli utenti [S41] | Web, dipendente dalla rete; repertorio non organizzato per celebrazione; stesso rischio copyright | Media: fissa l'aspettativa "accordi gratis" |
| **PDF e libretti diocesani** | Gratuiti, ufficiali, già nelle mani di tutti [S42] | Non trasponibili, illeggibili sul telefono, nessuna scaletta | **È lo status quo**: l'app deve importarli, non sostituirli |

### 1.3 Mappa di posizionamento

Assi scelti perché guidano la scelta dell'utente: **specificità per la liturgia italiana** (orizzontale) e **qualità come strumento per suonare dal vivo** (verticale).

```
 Strumento dal vivo
      alto │ OnSong  SongbookPro              ┌──────────────────────┐
           │ BandHelper  MobileSheets         │  SPAZIO LIBERO       │
           │                                  │  (Android + Windows) │
           │                    Canta e Cammina (solo iOS)  AppCanzoniere
           │                                  └──────────────────────┘
     medio │ Ultimate Guitar       NuoviConvertiti (web)
           │
     basso │ Chordify                Canti, Risuscitò, Canti Liturgici,
           │                         libretti PDF diocesani
           └───────────────────────────────────────────────────────────
             generalista                         liturgia italiana
```

[IPOTESI] Lo spazio in alto a destra è libero su Android e Windows [R §1.2, "NON REPERITO" su un'app che unisca tutto]. Lo spazio esiste perché è piccolo: questo va detto con onestà.

### 1.4 Cinque forze (sintesi)

| Forza | Livello | Motivo |
|---|---|---|
| Nuovi entranti | **Alto** | Barriere tecniche basse; un altro sviluppatore con l'AI può fare lo stesso in pochi mesi |
| Potere dei fornitori | **Alto** sui contenuti, basso sulla tecnologia | Gli editori (Paoline, San Paolo, Elledici, RnS) controllano il repertorio protetto [S67]; store e API sono sostituibili |
| Potere dei clienti | **Alto** | Volontari, alternative gratuite ovunque, costo di cambio quasi nullo |
| Sostituti | **Alto** | PDF, carta, siti gratuiti |
| Rivalità | Media | Pochi attori dedicati e piccoli; il grande generalista (SongbookPro) non presidia la liturgia |
| **Attrattività complessiva** | **Bassa come business, discreta come nicchia di servizio** | Si vince solo con specializzazione profonda e costi quasi nulli |

### 1.5 Quale posizionamento è difendibile per uno sviluppatore singolo

| Opzione | Difendibilità | Giudizio |
|---|---|---|
| **A. Strumento generalista per suonare dal vivo** | Nulla: SongbookPro, OnSong e MobileSheets sono più maturi, multipiattaforma ed economici | **Scartare** come posizionamento (resta un uso possibile, non un messaggio) |
| **B. Catalogo/community di canti** | Negativa: valore che dipende da contenuti che non si possono pubblicare legalmente (§4) e da una comunità che non c'è | **Scartare** |
| **C. Nicchia liturgica italiana + esecuzione dal vivo** | Bassa ma reale. Il vantaggio non è tecnologico: è **conoscenza del dominio** (momenti della Messa, Do-Re-Mi, libretti diocesani), **relazioni** (uffici liturgici, corsi animatori) e un **indice liturgico** che cresce con l'uso | **Raccomandato** |

**Il "fossato", detto onestamente** [IPOTESI]: è stretto. Dura finché (a) i grandi non trovano conveniente localizzare per un mercato da poche decine di migliaia di persone, (b) lo sviluppatore costruisce relazioni con diocesi e cori più velocemente di un imitatore. Si distrugge se SongbookPro aggiunge italiano + Do-Re-Mi + modelli di scaletta, o se un editore lancia un'app con il proprio repertorio licenziato. La difesa più solida a medio termine è un **accordo di contenuto con un editore o un movimento** (§3, opzione C).

---

## 2. Segmenti di clientela e chi paga

| Segmento | Dimensione indicativa | Lavoro da svolgere | Chi decide e chi paga | Disponibilità a pagare attesa |
|---|---|---|---|---|
| **Musicista singolo** (chitarrista o tastierista volontario) | Italia: 44K-133K musicisti liturgici, centrale ~75K [R §2.2, STIMA] | Suonare senza fogli, trasporre al volo, avere i canti offline | Lui stesso, con i propri soldi | **Bassa** [IPOTESI]: abituato al gratuito; accetta una piccola cifra una tantum per uno strumento che usa ogni settimana (modello SongbookPro [S6]); rifiuta l'abbonamento personale [S3, S58] |
| **Animatore / responsabile del coro** | 1 per coro; cori parrocchiali in Italia [NON REPERITO]; ordine di grandezza: decine di migliaia [IPOTESI, da 25K parrocchie S43] | Preparare la scaletta della domenica, distribuirla al gruppo, gestire tonalità e cambi | Lui decide; paga lui o si fa rimborsare dalla parrocchia | **Media** [IPOTESI]: il dolore è maggiore (coordinamento) e la spesa è condivisibile. Segnali: Canzoniere Scout prevede una licenza di gruppo [S31]; SongbookPro Groups; Canto App 30 $/anno nel mondo ispanico [S40] |
| **Parrocchia / unità pastorale** | ~25.600 parrocchie [S43] | Uniformare repertorio e libretti, ridurre fotocopie, rispettare i diritti | Parroco o consiglio per gli affari economici | **Media, lenta** [IPOTESI]: le parrocchie pagano già convenzioni SIAE/SCF [S64]; budget piccoli; decisioni lente. Pagano solo se il coro lo chiede |
| **Diocesi / ufficio liturgico** | 226 diocesi [S44] | Diffondere un repertorio diocesano, formare animatori | Direttore dell'ufficio liturgico, economo | **Bassa per comprare licenze, utile come canale** [IPOTESI]: più probabile che promuova uno strumento gratuito o che commissioni una versione con il proprio repertorio |
| **Editori di musica liturgica** (Paoline, San Paolo, Elledici, RnS) | Pochi attori [S67] | Vendere e diffondere il proprio repertorio in digitale, proteggere i diritti | Direzione editoriale/commerciale | **Ambivalente**: sono titolari dei diritti (fornitori potenti, §1.4) e possibili clienti B2B o partner. Possono pagare per uno strumento, ma più probabilmente chiederanno di essere pagati |
| **Movimenti e associazioni** (scout, RnS, gruppi giovanili) | Decine di migliaia di membri; app proprie già esistenti [S29-S31] | Canzoniere comune del movimento | Responsabili nazionali/regionali | **Media per un accordo B2B** [IPOTESI], ma alcuni hanno già un'app |
| *Musicisti non liturgici* (band, matrimoni) | Grande ma presidiato | Leggio generico | Loro stessi | Pagano già SongbookPro & co.; **non è un segmento da inseguire** |

**Conclusione** [IPOTESI]: **usa** il singolo musicista, **decide** l'animatore, **può pagare in modo ricorrente** il coro o la parrocchia. Il prodotto va progettato per conquistare il singolo gratis e far pagare l'animatore per il coordinamento del gruppo.

---

## 3. Opzioni di modello di business

### 3.1 Costi operativi di riferimento

| Voce | Costo | Stato |
|---|---|---|
| Account Google Play | 25 $ una tantum [T4] | [FATTO, fonte secondaria] |
| Commissione Google Play | Modello storico: 15% sul primo milione l'anno, 15% sugli abbonamenti [T1]. Nuovo modello SEE da metà 2026: 10% di servizio + 5% se si usa Play Billing, totale 15% [T2] | [FATTO, date da ricontrollare in Play Console] |
| Account Microsoft Store | Gratuito per i singoli (dal 2025) e per le aziende (da maggio 2026) [T3] | [FATTO] |
| Commissione Microsoft Store | 15% per le app non-gioco secondo fonti del 2025; possibilità di usare un proprio sistema di pagamento per le app | [NON VERIFICATO per il 2026] |
| Apple Developer Program | 99 $/anno [T4] + un Mac per compilare | [FATTO, fonte secondaria] |
| Commissione Apple (UE, dal 1/10/2026) | 15% per Small Business Program e abbonamenti dopo il primo anno [T5] | [FATTO, fonti secondarie discordanti su alcune aliquote] |
| IVA | Sulle vendite via store ai consumatori UE l'IVA la versa lo store; il prezzo pubblico la include. Netto stimato ≈ P / 1,22 × 0,85 ≈ **0,70 × P** | [IPOTESI, da confermare con un commercialista] |
| Hosting backend (solo se serve) | VPS europeo piccolo ≈ 4-10 €/mese [T10, fonte C]; con backup, dominio ed email ≈ **150-300 €/anno** | [FATTO per l'ordine di grandezza] |
| OCR | Sul dispositivo (motori OCR di Android e Windows): 0 €. In cloud: Azure Read ≈ 1,5 $ ogni 1.000 pagine, 500 pagine/mese gratis [T8] | [FATTO per Azure, fonte secondaria + pagina Microsoft] |
| AI (pulizia di import, aiuto alla scaletta) | Modello piccolo, es. GPT-5 mini: 0,25 $ / 2 $ per milione di token in input/output [T9] → circa 0,002-0,005 $ a richiesta: 1.000 richieste/mese ≈ 2-5 $ | [FATTO per il listino; IPOTESI per i token a richiesta]. Richiede un proxy sul server: le chiavi non possono stare nell'app |
| Riconoscimento audio | ACRCloud: listino dietro login [T6]; AudD 2-5 $ ogni 1.000 richieste, 300 gratis [T7] | [NON VERIFICATO per ACRCloud] |
| Parere legale (copyright, DSA, T&C, privacy) | 1.000-3.000 € una tantum | [IPOTESI] |
| Avvio attività (partita IVA o prestazione occasionale) | Da definire con un commercialista | [NON VERIFICATO] |

Due scelte tagliano quasi tutti i costi variabili: **OCR sul dispositivo** ed **eliminazione del riconoscimento audio** (costoso, poco utile per un repertorio liturgico raramente presente nei database di registrazioni commerciali [IPOTESI], e già offerto gratis da Shazam).

### 3.2 Le tre opzioni

**Opzione A — Freemium + sblocco Pro una tantum (singolo musicista), senza backend**
- *Come funziona*: app gratuita utile davvero (libreria locale, leggio, trasposizione, scalette); acquisto unico in app per le funzioni che fanno risparmiare tempo (import PDF/OCR, backup ed esportazioni, più dispositivi, modelli di scaletta avanzati). Condivisione delle scalette tramite file o QR, senza server.
- *Perché*: è il modello che funziona nel segmento leggio su Android/Windows (SongbookPro, MobileSheets, LinkeSOFT [R §3.1]) e non richiede di aggiustare il backend rotto [VALUTAZIONE §2.5].
- *Limite*: ricavi non ricorrenti e piccoli; il tetto del mercato italiano si raggiunge presto.

**Opzione B — Piano Coro annuale (gruppo/parrocchia) + app gratuita per i membri**
- *Come funziona*: l'animatore attiva un "coro" (uno spazio chiuso, a invito, con un numero massimo di membri); repertorio e scalette condivisi e sincronizzati; i membri usano l'app gratis. Paga l'animatore o la parrocchia.
- *Perché*: sposta il pagamento su chi ha il problema maggiore e un budget, e crea ricavi ricorrenti (modelli SongbookPro Groups, Canzoniere Scout, Canto App).
- *Limite*: richiede backend, account, sicurezza, GDPR (anche minori nei cori giovani) e un parere legale sulla condivisione nel gruppo (§4). Da avviare solo dopo la validazione.
- *Da verificare*: se la licenza di gruppo può essere venduta fuori dall'app (fattura alla parrocchia) senza violare le regole di pagamento di Google e Apple **[NON VERIFICATO]**.

**Opzione C — B2B con editore, movimento o diocesi**
- *Come funziona*: (C1) versione personalizzata o "canzoniere ufficiale" di un movimento o di una diocesi, a canone; oppure (C2) accordo con un editore per distribuire il suo repertorio nell'app (licenza a pagamento per l'utente, ricavi condivisi).
- *Perché*: è l'unico modo per avere **contenuti protetti legalmente** e un fossato vero.
- *Limite*: vendite lunghe, pochi interlocutori, contratti, richieste di personalizzazione e supporto. Rischio di trasformarsi in un lavoro su commessa. Richiede una forma d'impresa.

### 3.3 Confronto quantitativo (ordine di grandezza, a regime: anno 2-3)

Prezzi simbolici solo per il calcolo: P tra 5 e 15 € una tantum; G tra 20 e 80 €/anno per coro [intervalli da R §2.2 H4 e dai concorrenti; li fisserà il Pricing Analyst]. Netto ≈ 70% del prezzo (§3.1).

| | A · Pro una tantum | B · Piano Coro | C · B2B |
|---|---|---|---|
| **Ipotesi di volume** | Nuove installazioni/anno: 1K (basso), 3K (base), 8K (alto); conversione 2% / 4% / 6% [S54, R §2.2 H5] | Cori paganti: 15 / 60 / 250 (0,06%-1% delle parrocchie) | 0 / 1 / 2 contratti l'anno |
| **Ricavi annui netti** | basso ≈ 0,1-0,2 K€; base ≈ 0,5-1,3 K€; alto ≈ 2-5 K€ | basso ≈ 0,2-0,8 K€; base ≈ 0,8-3,4 K€; alto ≈ 3,5-14 K€ | 0 / 2-15 K€ / 5-30 K€ (stima molto incerta) |
| **Ricorrenti?** | No (solo nuovi utenti) | Sì | Parzialmente (canone) |
| **Costi fissi annui** | ≈ 30-80 € (dominio, sito; Play 25 $ una tantum) | ≈ 150-300 € di hosting + 1-3 K€ di legale una tantum (≈ 0,3-1 K€/anno su 3 anni) | Legale e contratti, forma d'impresa, tempo di vendita |
| **Costi variabili** | Commissioni store (incluse nel netto) | Commissioni + AI/OCR cloud trascurabili (< 10 €/mese fino a migliaia di utenti) | Personalizzazioni, supporto |
| **Pareggio di cassa** | ≈ 10-25 vendite l'anno | ≈ 10-25 cori (solo hosting); ≈ 20-60 cori con il legale ammortizzato | 1 contratto |
| **Tempo dello sviluppatore** | Il più basso | Medio-alto (backend, account, supporto) | Alto e imprevedibile |
| **Rischio legale** | Basso (contenuti dell'utente, in locale) | Medio (condivisione nel gruppo, dati personali) | Basso sui contenuti (licenziati), medio sui contratti |
| **Allineamento con lo stato del codice** | Alto: usa ciò che funziona (dominio, parser, trasposizione) | Richiede di rifare backend e autenticazione [VALUTAZIONE §6] | Dipende dal cliente |

**Nota sul pareggio "vero"** [IPOTESI]: il pareggio di cassa è facile. Se invece si vuole remunerare anche il tempo (es. 10 ore al mese a 30 €/ora ≈ 3,6 K€/anno), solo lo scenario alto di B o un contratto C ci arrivano.

### 3.4 Scenari a 3 anni (somma delle opzioni raccomandate A → B)

| Scenario | Probabilità [IPOTESI] | Cosa succede | Ricavi netti annui al 3° anno |
|---|---|---|---|
| **Pessimistico** | 45% | Interesse tiepido, pochi cori convertono, nessun accordo con editori | < 1 K€: progetto personale che si autofinanzia |
| **Base** | 40% | Passaparola in 2-4 diocesi, Piano Coro adottato da qualche decina di gruppi | 2-5 K€ |
| **Ottimistico** | 15% | Promozione da parte di un ufficio liturgico o di un movimento, oppure accordo con un editore | 10-30 K€ |

Indicatori precoci: rapporto tra installazioni e utenti attivi la domenica; numero di cori creati nei primi tre mesi del Piano Coro; risposta degli uffici liturgici alla prima presentazione.

### 3.5 Raccomandazione

**Modello raccomandato: A subito, poi B quando i dati lo giustificano** ("una tantum per il singolo, abbonamento per il gruppo"). Regola di separazione del valore: tutto ciò che serve a **suonare** resta gratuito e senza limiti di tempo (il leggio deve essere affidabile la domenica); si paga ciò che fa **risparmiare tempo** (import da PDF/OCR, pulizia con AI, backup) e ciò che serve a **coordinare un gruppo**.
**Alternativa: C**, da cogliere se un editore, un movimento o una diocesi mostra interesse concreto; può sostituire B se arriva prima.
**Cosa si sacrifica**: ricavi ricorrenti nel primo anno; il mercato non liturgico; iOS nella prima fase.

---

## 4. Il nodo del copyright come questione strategica

### 4.1 Fatti rilevanti
- I testi delle canzoni sono protetti fino a 70 anni dalla morte dell'autore (L. 633/1941) e gran parte del repertorio liturgico corrente è post-conciliare, con autori spesso viventi [R §4.1].
- Le convenzioni CEI-SIAE/SCF riguardano l'**esecuzione**, non la **riproduzione dei testi** in app [S64, S65].
- Non risulta in Italia una licenza collettiva per riprodurre testi liturgici in app (tipo CCLI/OneLicense) [R §4.1, NON REPERITO].
- Chi ospita contenuti degli utenti deve gestire le segnalazioni (DSA art. 16, anche le microimprese); un servizio che organizza e promuove grandi quantità di opere caricate dagli utenti a scopo di lucro può ricadere nell'art. 17 della Direttiva DSM (D.Lgs. 177/2021) [R §4.2].
- Precedenti: i siti di tab chiusi su pressione degli editori (MXtabs, 2005); Ultimate Guitar sopravvive grazie a licenze dirette [S14, S76, S77].

### 4.2 Valore o rischio?
**È il rischio principale.** [IPOTESI motivata] Per l'utente il valore di un catalogo pubblico è reale ma sostituibile (ha già il libretto PDF della sua parrocchia e i siti gratuiti); per lo sviluppatore il rischio è asimmetrico: una sola diffida di un editore può imporre di rimuovere il catalogo, e se il prodotto è costruito attorno al catalogo si ferma tutto. Inoltre gli editori sono i partner più promettenti per l'opzione C: pubblicare i loro canti senza licenza chiude quella porta.

### 4.3 Confronto delle alternative

| Alternativa | Valore per l'utente | Rischio legale | Costo operativo | Giudizio |
|---|---|---|---|---|
| **1. Catalogo pubblico aperto caricato dagli utenti** (requisito originale) | Alto all'inizio, se la comunità c'è | **Alto**: art. 17 DSM probabile, notifica e azione DSA, moderazione | Alto (moderazione, segnalazioni, server) | **Scartare** |
| **2. Nessun catalogo pubblico** (solo libreria privata) | Medio: l'utente importa i propri canti | Basso | Nullo | Base minima, sempre presente |
| **3. Solo dominio pubblico o licenze aperte** | Basso: gregoriano, inni antichi, canti tradizionali; poco del repertorio usato oggi | Basso (va verificato canto per canto) | Basso (curatela) | **Sì, come nucleo iniziale** per una "prima esperienza" non vuota |
| **4. Accordo con editori o società di licenze** | **Il più alto**: repertorio ufficiale legale | Basso se il contratto è chiaro | Alto (trattativa, royalty, rendicontazione) | **Obiettivo di medio termine** (opzione C) |
| **5. Condivisione privata tra membri di un gruppo** | Alto per il coro: è il caso d'uso reale (la scaletta di domenica) | Medio-basso [IPOTESI]: non è "pubblico", ma resta una riproduzione; serve un parere | Medio (backend, account) | **Sì, con limiti**: gruppi chiusi, a invito, numero massimo di membri, nessuna ricerca tra gruppi |
| **6. (Proposta aggiuntiva) Indice liturgico pubblico senza testi** | Medio-alto: "cosa cantare all'offertorio della IV di Avvento", con titolo, autori, momenti, riferimenti biblici, tonalità consigliata, raccolta in cui si trova, link a registrazioni ufficiali | Basso [IPOTESI]: dati e metadati, non l'opera | Basso | **Sì: è il "catalogo" difendibile** e un asset che cresce con l'uso |

### 4.4 Posizione raccomandata
**Combinare 2 + 3 + 5 + 6 ora, perseguire 4 dopo.** Mai l'opzione 1 senza licenze.
In pratica: libreria privata con import dal PDF dell'utente; condivisione solo dentro un coro chiuso; un piccolo repertorio di dominio pubblico verificato; un indice liturgico pubblico senza testi; e, appena ci sono utenti da mostrare, una proposta agli editori. Anche gli "accordi suggeriti dall'AI" per brani protetti vanno tolti: generano proprio la riproduzione che si vuole evitare e sono già offerti dai grandi [S15]. L'AI resta per la pulizia dell'import del materiale dell'utente.

### 4.5 Cosa richiede il parere di un legale (prima del Piano Coro e di qualsiasi pubblicazione)
1. La condivisione di testi protetti tra i membri di un gruppo chiuso (es. fino a 30-50 persone) tramite i nostri server è lecita? Con quali limiti?
2. Conservare sul server, per backup e sincronizzazione, testi protetti caricati dall'utente per uso personale: quali obblighi DSA si applicano (notifica e azione, punto di contatto, motivazione)?
3. Un indice pubblico con titoli, autori, momenti liturgici, tonalità e incipit (prime parole) è libero? E le sole sequenze di accordi senza testo?
4. L'art. 17 DSM e il D.Lgs. 177/2021 si applicano a un servizio così configurato? Il regime ridotto (meno di 3 anni, sotto 10 M€) basta?
5. Testi di T&C, informativa privacy, consenso dei minori (cori giovani: in Italia l'età del consenso digitale è 14 anni [NON VERIFICATO in questa sessione]), responsabilità dell'utente che carica.
6. Riferimenti biblici e liturgici: confermare che citare "Mt 5,1-12" e i nomi dei tempi liturgici non richiede licenza, e che i testi CEI non vanno mostrati [S69].
7. Schema di contratto con un editore (licenza di distribuzione, royalty, rendicontazione).

---

## 5. Strategia di ingresso

### 5.1 Prodotto minimo vendibile (MVS)
Costruito sulle parti che funzionano (dominio, parser ChordPro, trasposizione [VALUTAZIONE §1, §6]) e sulla direzione UX "Scaletta + modello Messa" [CONFRONTO-UX].

| Dentro | Fuori (per ora) |
|---|---|
| Libreria locale ChordPro con **accordi sopra il testo** | Backend, account, catalogo pubblico |
| Trasposizione con tonalità originale visibile; **Do-Re-Mi** e C-D-E; diesis/bemolle | Riconoscimento audio |
| **Scaletta "Messa"** con i momenti come sezioni; scalette libere per altri eventi | Accordi suggeriti dall'AI |
| **Modalità Palco**: schermo intero, anteprima del canto successivo, pedale/tasti volume, ricerca d'emergenza | Sync con OneDrive, Google Drive, Dropbox |
| Offline totale per definizione (tutto è locale) | iOS e macOS |
| **Import da PDF testuale e OCR sul dispositivo**, con revisione affiancata | Calendario liturgico completo e lezionario |
| Condivisione di canti e scalette tramite **file o QR** (da dispositivo a dispositivo) | Raccolte automatiche con AI |
| Esportazione/backup locale; interfaccia in italiano (EN dopo) | Tablature, accordi per pianoforte con le note |
| Sblocco Pro una tantum tramite store | |
| Un nucleo di canti di dominio pubblico verificati | |

**Criterio di qualità unico**: un animatore deve poter passare da "PDF del libretto parrocchiale" a "scaletta di domenica pronta sul tablet" in meno di 15 minuti la prima volta, e suonare senza toccare lo schermo se ha un pedale.
Stima di tempo [IPOTESI molto incerta]: 3-5 mesi part-time con agenti AI, perché si evita tutto il lavoro su backend e sync.

### 5.2 Ordine delle piattaforme
1. **Android (telefono e tablet)**: dove sono i volontari [IPOTESI: quota Android alta in Italia, da verificare con le interviste], dove mancano le alternative liturgiche [R §6], costo d'ingresso 25 $.
2. **Windows**: "tavolo di preparazione" (import dei PDF, preparazione delle scalette); compila già [VALUTAZIONE §2.3]; registrazione gratuita [T3].
3. **iOS/iPadOS**: solo quando almeno il 20-30% dei cori pilota lo chiede o il Piano Coro è attivo. Costa 99 $/anno più un Mac, e su iOS esiste già Canta e Cammina [S36].

### 5.3 Canali a costo zero
| Canale | Azione concreta |
|---|---|
| Rete personale | La propria parrocchia e le vicine: primi 5 cori pilota, usati come caso di studio |
| Uffici liturgici diocesani (226) | Presentazione di 10 minuti nei corsi per animatori musicali; offrire gratis l'**indice liturgico** del repertorio diocesano [S63, S78] |
| Eventi | Incontri diocesani dei cori, Settimana Liturgica Nazionale, corsi CEI MLO/COPERLIM [S63, S78] |
| Store | Ottimizzazione per "canti liturgici accordi", "canti messa", "scaletta coro"; schede in italiano con screenshot della scaletta della Messa |
| Siti e creator cristiani | Proposta a NuoviConvertiti, Animatamente, SpazioAccordi: link reciproci o "apri nell'app" [S41, S42] |
| Movimenti | Scout, gruppi giovani, RnS: presentare il Piano Coro e l'eventuale versione B2B |
| Produttori di pedali | Richiesta di inserimento nella lista di app compatibili AirTurn [S60] |
| Contenuti propri | Brevi video "dal PDF al leggio in 5 minuti", "come usare il pedale alla Messa" |

### 5.4 I primi 100 utenti paganti
1. **Mesi -2/0 (prima del lancio)**: 15-20 interviste, 5 cori pilota con versione di prova; raccogliere 20-30 impegni di acquisto (lista d'attesa con "pagherei").
2. **Mesi 1-3**: lancio su Android in 1-2 diocesi; un incontro di formazione con l'ufficio liturgico; obiettivo 500 installazioni e 20-30 paganti (pilota + passaparola).
3. **Mesi 4-6**: Windows; 3-5 incontri diocesani; obiettivo 1.500 installazioni cumulative e 60 paganti.
4. **Mesi 7-12**: casi di studio dei cori pilota, contatti con movimenti; obiettivo **100 paganti** (con conversione 3-5% servono circa 2.000-3.500 installazioni [IPOTESI]).

### 5.5 Criteri per proseguire o fermarsi
| Gate | Quando | Proseguire se… | Altrimenti |
|---|---|---|---|
| **Gate 0 — Problema** | Prima di riprendere lo sviluppo (4-6 settimane) | ≥15 interviste fatte; ≥60% usa già telefono o tablet al leggio; ≥5 cori accettano il pilota; ≥3 animatori dichiarano che pagherebbero (meglio: preordine) | Fermarsi come prodotto commerciale; continuare solo come progetto personale o open source |
| **Gate 1 — Uso** | 3 mesi dopo il lancio Android | ≥500 installazioni; ≥30% degli installati usa una scaletta almeno 3 domeniche su 4 nel secondo mese; ≥20 paganti; voto ≥4,3 | Correggere una volta (prodotto o canali); se fallisce di nuovo, fermare gli investimenti |
| **Gate 2 — Gruppo** | 6-9 mesi | ≥10 cori chiedono la condivisione sincronizzata **e** parere legale favorevole | Restare all'opzione A senza backend |
| **Gate 3 — Scala** | 12 mesi | ≥100 paganti oppure ≥20 cori sul Piano Coro oppure un accordo C in trattativa | Mettere il prodotto in manutenzione (gratuito o una tantum, nessuna nuova funzione) |

---

## 6. Verdetto di fattibilità

### 6.1 Verdetto: **PROCEDERE A CONDIZIONI**
Come **micro-prodotto di nicchia** (liturgia italiana, Android poi Windows, senza catalogo pubblico, senza backend nella prima versione) che si ripaga i costi e può dare un reddito accessorio. **Non** è sostenibile come attività principale con i dati attuali: il ricavo atteso nello scenario base al terzo anno è di qualche migliaio di euro [§3.4].

### 6.2 Tre condizioni che cambierebbero il verdetto
1. **Le parrocchie o i cori pagano in modo ricorrente** (Gate 0 e Gate 2 superati largamente, es. ≥50 cori paganti entro un anno) → il verdetto sale a **procedere**, con investimento nel Piano Coro e in iOS.
2. **Un accordo di contenuto** con un editore, un movimento o una diocesi (licenza del repertorio o versione ufficiale) → **procedere** con un posizionamento più forte e un fossato vero.
3. **Il Gate 0 fallisce, oppure il parere legale chiude la condivisione nel gruppo, oppure SongbookPro aggiunge italiano + Do-Re-Mi + scalette liturgiche** → **non procedere come prodotto commerciale**: tenerlo come strumento personale o open source.

### 6.3 Ipotesi chiave messe alla prova
| Ipotesi | Perché potrebbe essere falsa | Come verificarla |
|---|---|---|
| Gli animatori usano già un dispositivo al leggio | Molti usano ancora carta e libretto stampato | Gate 0, domanda diretta + osservazione di una Messa |
| Paga chi coordina il gruppo, non il singolo | Le parrocchie potrebbero non avere budget o processi per spese software | Chiedere ai pilota "chi pagherebbe e con quale procedura" |
| Il vantaggio liturgico conta più della maturità di SongbookPro | Per un chitarrista esperto bastano scalette e trasposizione | Far provare entrambe le app a 5 musicisti e chiedere quale terrebbero |

### 6.4 Rischi principali e mitigazioni
| Rischio | Probabilità / Impatto | Mitigazione |
|---|---|---|
| **Copyright** (diffida, rimozione, art. 17) | Media / Alto | Nessun catalogo pubblico di testi; gruppi chiusi; parere legale prima del Piano Coro; procedura di segnalazione e rimozione già pronta; trattare gli editori da partner |
| **Bassa disponibilità a pagare** | Alta / Alto | Far pagare il gruppo, non il singolo; Gate 0 con preordini; costi fissi vicini a zero |
| **Tempo e motivazione dello sviluppatore singolo** | Alta / Alto | Ambito minimo; niente backend in v1; gate con date; regola "una funzione nuova solo se chiesta da almeno 3 cori" |
| **Debito tecnico** (backend rotto, migrazioni incoerenti, bug MAUI) [VALUTAZIONE] | Certo / Medio | Il MVS non usa il backend; rifare la persistenza locale con migrazioni EF corrette prima di avere utenti con dati reali |
| **Perdita di dati la domenica** (DB locale corrotto, crash) | Media / **Molto alto per la reputazione** | Backup automatico locale, test su dispositivi reali, nessun aggiornamento il sabato sera |
| **Concorrente che entra nella nicchia** (SongbookPro, un editore) | Bassa-media / Alto | Velocità sulle relazioni diocesane; indice liturgico come asset; proporre per primi l'accordo agli editori |
| **Piattaforma** (MAUI su Android, pedali Bluetooth) | Media / Medio | Prototipare subito la modalità Palco su tablet e pedale reali [CONFRONTO-UX] |
| **Costi delle API fuori controllo** | Bassa / Medio | OCR sul dispositivo; AI solo tramite proxy, con limiti per utente e solo per i paganti |
| **Obblighi fiscali e privacy** | Media / Medio | Commercialista prima della prima vendita; informativa privacy e nessun dato personale sul server nella prima versione |

---

## 7. Decisioni che spettano al proprietario

1. **Obiettivo del progetto**: reddito accessorio, servizio alla comunità che si autofinanzia, oppure hobby? Cambia i gate e quanto tempo investire.
2. **Tempo disponibile** a settimana per i prossimi 12 mesi (serve a fissare le date dei gate).
3. **Confermare il posizionamento** "leggio per la liturgia italiana" e la rinuncia al pubblico generalista come messaggio.
4. **Confermare la rinuncia al catalogo pubblico** di testi e accordi (e agli accordi suggeriti dall'AI per brani protetti) a favore di libreria privata, cori chiusi e indice liturgico senza testi.
5. **Togliere dal perimetro** il riconoscimento audio e la sync con OneDrive/Drive/Dropbox.
6. **Ordine delle piattaforme**: Android → Windows → iOS solo su richiesta.
7. **Modello**: A subito, B dopo il Gate 2, C se si presenta l'occasione. Il prezzo va al Pricing Analyst.
8. **Budget una tantum** per un parere legale (stima 1-3 K€) e per il commercialista, e in quale momento spenderlo (consigliato: prima del Piano Coro; prima della prima vendita per il fisco).
9. **Forma giuridica** per incassare (prestazione occasionale, partita IVA forfettaria, altro): da decidere con un commercialista.
10. **Fare il Gate 0** (interviste e cori pilota) prima di riprendere lo sviluppo, e chi contattare per primi (propria parrocchia, ufficio liturgico diocesano).
11. **Approvare la direzione UX** "Scaletta + modello Messa" (decisione aperta in CONFRONTO-UX), coerente con questa strategia.

---

## Fonti nuove di questo documento

Consultate il 10/11/2026. Per le fonti [S1-S81] vedi `docs/business/01-RICERCA-MERCATO.md`.

- [T1] Google Play Console Help — Service fees: https://support.google.com/googleplay/android-developer/answer/112622
- [T2] Adapty — What Google Play's new billing rules mean for subscriptions: https://adapty.io/blog/google-play-billing-changes-subscriptions-fees/ ; Engadget: https://www.engadget.com/apps/google-ends-is-30-percent-app-store-fee-and-welcomes-third-party-app-stores-185248673.html
- [T3] Windows Developer Blog — Free developer registration for individual developers (10/09/2025): https://blogs.windows.com/windowsdeveloper/2025/09/10/free-developer-registration-for-individual-developers-on-microsoft-store/ ; per le aziende (07/05/2026): https://blogs.windows.com/windowsdeveloper/2026/05/07/publish-to-microsoft-store-as-a-company-now-with-free-registration-and-faster-onboarding/
- [T4] Windows Central — Microsoft Store drops fees; Apple still charges $99 per year (cita anche i 25 $ di Google): https://www.windowscentral.com/microsoft/windows-11/microsoft-store-drops-fees-for-individual-developers-apple-still-charges-usd99-per-year
- [T5] Apple Newsroom — Changes for apps in the European Union (agosto 2026): https://www.apple.com/mt/newsroom/2026/08/apple-announces-changes-for-apps-in-the-european-union/ ; Game World Observer: https://gameworldobserver.com/2026/08/19/apple-will-reduce-the-app-store-commission-in-the-eu-to-resolve-disagreements-with-the-european-commission
- [T6] ACRCloud Docs — Recognize Music (prezzi dietro login): https://docs.acrcloud.com/tutorials/recognize-music
- [T7] AudD — Music Recognition API: https://audd.io/
- [T8] Azure — Document Intelligence pricing: https://azure.microsoft.com/en-gb/pricing/details/form-recognizer/ ; Activepieces (2026): https://www.activepieces.com/blog/azure-document-intelligence-pricing-per-model-in-2026
- [T9] OpenAI — GPT-5 mini: https://developers.openai.com/api/docs/models/gpt-5-mini
- [T10] Hetzner pricing 2026 (aggregatore, affidabilità C): https://agentdeals.dev/hetzner-pricing-2026

### Cosa non sono riuscito a verificare
- Commissione 2026 del Microsoft Store per le app non-gioco e possibilità di pagamenti propri.
- Listino attuale di ACRCloud.
- Se una licenza di gruppo può essere venduta fuori dagli store senza violare le loro regole di pagamento.
- Quota di Android tra gli animatori italiani e numero di cori parrocchiali (nessun censimento pubblico).
- Disponibilità a pagare di parrocchie e cori: nessun dato, solo segnali da concorrenti esteri e da Canzoniere Scout.
- Età del consenso digitale e regole per i minori nei cori: da confermare con il legale.
