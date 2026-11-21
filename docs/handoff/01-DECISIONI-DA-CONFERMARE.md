# Decisioni da confermare

Scopo: elencare le decisioni che servono dal proprietario. Per ogni decisione vedi cosa cambia se confermi e se cambi.

Fonti: `docs/DECISIONI-ROUND-8.md`, `docs/DECISIONI-ROUND-9.md`, `docs/FASI.md` (sezione 0) e `docs/design/UX-IMPORT-ROUND-9.md` (sezione 12).

Come rispondere: usa la riga modello in [00-INDICE.md](00-INDICE.md), sezione "Come riportare gli esiti".

---

## Già confermate (DEC 32-35)

Confermate il 20/11/2026. Non serve altro.

| DEC | Decisione |
|---|---|
| 32 | OCR su Android con il modello ML Kit incluso nell'app. Funziona senza rete. |
| 33 | Prove automatiche al posto delle prove a mano, per tutto ciò che non richiede hardware. |
| 34 | Android minimo 6.0. |
| 35 | Titoli di canto da riga centrata: conta solo se più grande del testo, con una riga vuota sopra e non ripetuta nella pagina. |

Nota su DEC 35. Il round 8 indicava 2 titoli persi nel libretto 14. Erano 6. Il round 9 li ha trovati. Vedi [DECISIONI-ROUND-9.md](../DECISIONI-ROUND-9.md).

---

## Da decidere adesso (round 9)

Queste decisioni nascono nel round 9. Leggi ogni voce. Poi scrivi la riga della colonna "Cosa scrivere nel prompt".

### DEC 36 — Lettura dei PDF nel processo dell'app

- **Cosa dice:** su Android i PDF si leggono nel processo dell'app. Su Windows il testo si legge ancora nel lettore separato. Su Android e su Windows le pagine scansionate si disegnano nel processo dell'app. Il lettore separato per Android arriva in F7.
- **Perché:** il lettore separato su Android era già rinviato a F7 nel round 8.
- **Se confermi:** un PDF costruito ad arte può chiudere l'app. Su Windows una pagina solo immagine passa dal processo dell'app. Su Android un lavoro che scade dopo 2 minuti continua in background finché non finisce. Un nuovo import attende.
- **Se cambi:** il lettore separato per Android va fatto prima della chiusura di F3. Non è nel piano attuale. Serve un nuovo round.
- **Cosa scrivere nel prompt:** `DEC 36: confermata` oppure `DEC 36: cambiata, [la tua scelta]`.

### DEC 37 — Canti importati salvati nella libreria attuale

- **Cosa dice:** la schermata Importa sostituisce la vecchia pagina OCR simulata. I canti salvati entrano nella libreria attuale.
- **Perché:** la misura del Gate 0 usa canti salvati veri.
- **Se confermi:** i canti salvati restano nella libreria fino a F6. Quando F6 rifà la persistenza, possono andare persi.
- **Se cambi:** va scelto dove salvare i canti importati. Serve un nuovo round.
- **Cosa scrivere nel prompt:** `DEC 37: confermata` oppure `DEC 37: cambiata, [la tua scelta]`.

### DEC 38 — Dichiarazione sui diritti prima di ogni salvataggio

- **Cosa dice:** prima di salvare, spunti una dichiarazione sui diritti del canto. La casella parte sempre non spuntata e non viene ricordata. Salva resta spento finché non è spuntata. Ogni salvataggio ha la sua dichiarazione. I canti da salvare sono fissi quando il foglio si apre.
- **Perché:** anticipa un requisito di F7 sui diritti.
- **Se confermi:** ogni salvataggio richiede un passo in più. Il testo va rivisto con il legale (D11, U-3).
- **Se cambi:** scrivi cosa cambia: la dichiarazione si toglie, si sposta in F7, oppure il testo è diverso.
- **Cosa scrivere nel prompt:** `DEC 38: confermata` oppure `DEC 38: cambiata, [la tua scelta]`.

---

## Decisioni di interfaccia (round 9, da confermare)

Fonte: `docs/design/UX-IMPORT-ROUND-9.md`, sezione 12. Ogni voce dice la domanda e la proposta del documento.

### U-1 — Import del libretto intero in questa versione

- **Cosa dice:** dal round 9 l'import del libretto intero è disponibile. Il piano lo metteva in una versione futura, come funzione Pro.
- **Se confermi:** la misura del Gate 0 (traccia B) usa i libretti. Decidi in F7 se il libretto resta per tutti.
- **Se cambi:** il libretto intero torna in una versione futura. La misura del Gate 0 resta sul report del banco di prova.
- **Cosa scrivere nel prompt:** `U-1: confermata` oppure `U-1: cambiata, [la tua scelta]`.

### U-2 — Una dichiarazione per salvataggio o una per canto

- **Cosa dice:** la proposta è una dichiarazione per ogni salvataggio, con il numero dei canti nel titolo.
- **Se confermi:** salvare 150 canti richiede una sola spunta. Il legale deve vedere il testo (D11).
- **Se cambi:** una dichiarazione per canto. Un libretto da 150 canti richiede 150 spunte.
- **Cosa scrivere nel prompt:** `U-2: confermata` oppure `U-2: cambiata, [la tua scelta]`.

### U-3 — Testo della dichiarazione

- **Cosa dice:** il testo è quello proposto nella sezione 6 del documento UX.
- **Se confermi:** il testo si rivede quando il legale risponde (D11).
- **Se cambi:** scrivi il testo nuovo nel prompt.
- **Cosa scrivere nel prompt:** `U-3: testo proposto` oppure `U-3: testo nuovo: [testo]`.

### U-4 — Canti selezionati all'inizio

- **Cosa dice:** la proposta è selezionare tutti i canti all'apertura dell'elenco.
- **Se confermi:** chi vuole il libretto intero non tocca 150 caselle. La dichiarazione resta il controllo.
- **Se cambi:** nessun canto selezionato. L'utente sceglie uno per uno, oppure con "Tutti".
- **Cosa scrivere nel prompt:** `U-4: confermata` oppure `U-4: cambiata, [la tua scelta]`.

### U-5 — Annullare la lettura

- **Cosa dice:** la proposta è che annullare la lettura scarti tutto.
- **Se confermi:** niente elenchi parziali.
- **Se cambi:** i canti già letti restano nell'elenco. Serve lavoro in più nella schermata.
- **Cosa scrivere nel prompt:** `U-5: confermata` oppure `U-5: cambiata, [la tua scelta]`.

### U-6 — Cronometro per il Gate 0

- **Cosa dice:** la proposta è un cronometro nella schermata, spento per predefinito.
- **Se confermi:** il cronometro serve per la traccia B. Va tolto dopo il Gate 0, con un round.
- **Se cambi:** la misura si fa con il timer del telefono.
- **Cosa scrivere nel prompt:** `U-6: confermata` oppure `U-6: cambiata, [la tua scelta]`.

### U-7 — Scheda "Importa" nella barra fino a F6

- **Cosa dice:** la proposta è tenere la scheda "Importa" nella barra attuale fino a F6.
- **Se confermi:** la navigazione si rifà in F6.
- **Se cambi:** serve un'altra posizione nella barra. Decidila ora.
- **Cosa scrivere nel prompt:** `U-7: confermata` oppure `U-7: cambiata, [la tua scelta]`.

### U-8 — Stato dei diritti dei canti importati

- **Cosa dice:** la proposta è salvare i canti importati con stato dei diritti "Sconosciuto", fino a F7.
- **Se confermi:** lo stato resta "Sconosciuto" fino a F7. Con DEC 37 i canti potrebbero perdersi in F6.
- **Se cambi:** serve un altro stato iniziale. Serve un round di lavoro.
- **Cosa scrivere nel prompt:** `U-8: confermata` oppure `U-8: cambiata, [la tua scelta]`.

### Palette della schermata Importa

- **Cosa dice:** fino a F4 la pagina Importa usa i colori di Leggio, beige e rosso scuro. Il resto dell'app resta viola.
- **Se confermi:** la differenza resta fino a F4. Decidi tu se ricolorare le altre pagine.
- **Se cambi:** la pagina Importa va in viola. I contrasti vanno rifatti.
- **Cosa scrivere nel prompt:** `Palette: accettata fino a F4` oppure `Palette: cambiata, [la tua scelta]`.

Il confronto visivo della schermata è in [10-CONFRONTO-VISIVO-IMPORTA.md](10-CONFRONTO-VISIVO-IMPORTA.md).

---

## Da decidere più avanti (D10-D13)

Queste decisioni non servono adesso. Ogni riga dice quando serve la decisione e quale fase è bloccata.

| Codice | Domanda | Scadenza | Fase bloccata | Cosa serve per decidere | Cosa scrivere nel prompt |
|---|---|---|---|---|---|
| **D10** | Nome dell'app, `ApplicationId` (identificativo permanente), tipo di account Google Play, account Partner Center (Microsoft). | Entro il 28/02/2027. | F13 (e quindi F11 e F12, che dipendono dall'identificativo). | Scelta del nome. Verifica dei nomi nei due store. | `D10: nome [...], ApplicationId [...], account Play [sì/no], Partner Center [sì/no]` |
| **D11** | Parere legale. Il perimetro è ora più ampio: pubblicità, consenso, acquisti, minori, dichiarazione sui diritti. | Nessuna data scritta nel piano. Il pilota completo parte il 12/09/2027. | F14 (Gate Anteprima e Gate Pilota). | Incarico a un legale. Domande in [07-VERIFICHE-ESTERNE-E-PROFESSIONISTI.md](07-VERIFICHE-ESTERNE-E-PROFESSIONISTI.md). | `D11: incarico dato il [data] / parere ricevuto` |
| **D12** | Codice aperto o chiuso. La scelta decide anche dove si pubblicano le release Windows del pilota. | Entro marzo 2027 (F13, parte 1). | F13 (sede delle release) e F15 (lancio). | Vedi nota sotto. | `D12: release Windows su [sede]` |
| **D13** | Forma fiscale (partita IVA o altra). Ora va decisa prima del lancio. | Prima del lancio (entro il 21/11/2027). | F15 (lancio) e F17 (piano coro). | Parere del commercialista (Gate 0, attività 12). Vedi `07`. | `D13: [forma scelta], commercialista [nome o da scegliere]` |

**Nota su D12.** Il repository `F1-Solution/AppCanzoniere` resta pubblico. Lo hai confermato il 20/11/2026. Resta da decidere solo la sede delle release Windows, in F13. Le release scaricabili dai cori dipendono da questa scelta. Vedi anche [05-GITHUB-E-MACCHINA-LOCALE.md](05-GITHUB-E-MACCHINA-LOCALE.md).

**Nota su D10.** D10 non deve cambiare dopo la pubblicazione. L'`ApplicationId` è permanente.

---

## Punti tecnici aperti (non serve decidere)

Sono problemi noti di qualità. Non serve una decisione. Li trovi anche in `docs/DECISIONI-ROUND-9.md`.

- Libretto 14: trovati 151 canti, nell'indice 152 voci. I piè di pagina che cambiano da una sezione all'altra entrano ancora nel testo di alcuni canti.
- Libretti 15, 16, 17 e 18: trovano più canti di quanti ne conti l'indice.
- OCR su Windows: a volte perde accordi in grassetto. Non trattato nel round 9.
- Scansione 19: a 70 dpi è illeggibile. Non trattato nel round 9.
- Pagine ruotate di 90 gradi senza testo: non vengono lette. Non trattato nel round 9.
- Android, lavoro scaduto: dopo 2 minuti l'errore compare, ma la lettura continua in background. Il nuovo import attende. Da verificare sul tablet.
- ML Kit e rete: la documentazione di Google dice che ML Kit può inviare metriche d'uso. Da verificare con una cattura del traffico. È un compito dell'agente.
- Revisione del round 9: lo stato di ogni rilievo (R9-01 a R9-24) va confermato nel round successivo.
- Primo tocco su Android: può spostare lo scorrimento di pochi pixel. Da verificare con la prova T2.
