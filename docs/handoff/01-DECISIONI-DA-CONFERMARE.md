# Decisioni da confermare

Scopo: elencare le decisioni che servono dal proprietario. Per ogni decisione vedi cosa cambia se confermi e se cambi.

Fonti: `docs/DECISIONI-ROUND-8.md` e `docs/FASI.md`, sezione 0.

Come rispondere: usa la riga modello in [00-INDICE.md](00-INDICE.md), sezione "Come riportare gli esiti".

---

## Da decidere adesso (DEC 32-35)

Le quattro decisioni sono scritte dall'agente come "da confermare". Solo tu le confermi o le cambi.

### DEC 32 — OCR su Android con modello incluso nell'app

- **Cosa dice:** su Android l'OCR usa il modello ML Kit incluso nell'app. Funziona senza rete. Riconosce solo l'alfabeto latino.
- **Perché:** l'app non deve dipendere dalla rete. Il piano commerciale lo chiede (R-04, R-05).
- **Se confermi:** l'APK di debug cresce di circa 15,6 MB (da 94,9 a 110,5 MB). L'OCR funziona senza rete.
- **Se cambi:** l'OCR Android va cambiato. Un altro motore richiede una nuova prova (fase F3).
- **Cosa scrivere nel prompt:** `DEC 32: confermata` oppure `DEC 32: cambiata, [la tua scelta]`.

### DEC 33 — Prove automatiche al posto delle prove a mano

- **Cosa dice:** le prove che non richiedono mani, pedale o occhi girano in automatico. Sono l'emulatore Android in CI, i runner Windows e il PC di sviluppo. Restano a te solo le prove su hardware reale.
- **Perché:** riduce il lavoro manuale. Il piano stima 4-6 ore per round.
- **Se confermi:** esegui solo le prove in [PROVE-SU-DISPOSITIVO.md](../verifiche/PROVE-SU-DISPOSITIVO.md). Le prove T1-T7, W1-W7 e B1.
- **Se cambi:** devi eseguire a mano anche le prove già automatiche. La tabella "Cosa è già provato in automatico" va rivista.
- **Cosa scrivere nel prompt:** `DEC 33: confermata` oppure `DEC 33: cambiata, [la tua scelta]`.

### DEC 34 — Android minimo 6.0

- **Cosa dice:** la versione minima di Android è 6.0. Prima era 5.0. ML Kit richiede la 6.0.
- **Se confermi:** i dispositivi con Android 5.x non installano più l'app. Controlla la versione del tablet prima dell'acquisto.
- **Se cambi:** con Android 5.0 l'OCR non può funzionare su Android 5.x. Serve un motore diverso oppure un controllo che spegne l'OCR su quei dispositivi. [DA VERIFICARE con l'agente.]
- **Cosa scrivere nel prompt:** `DEC 34: confermata` oppure `DEC 34: cambiata, [la tua scelta]`.

### DEC 35 — Titoli di canto da riga centrata

- **Cosa dice:** una riga centrata conta come titolo di canto solo se è scritta più grande del testo. Inoltre non si ripete nella pagina.
- **Perché:** i ritornelli rientrati dividevano i canti per errore.
- **Effetto noto:** nel libretto 14 si perdono 2 titoli veri scritti solo un po' più grandi. Il conteggio scende da 151 a 149 canti.
- **Se confermi:** i ritornelli rientrati non dividono più i canti. I 2 titoli persi non sono più canti separati.
- **Se cambi:** in alcuni libretti i ritornelli rientrati tornano a dividere i canti.
- **Cosa scrivere nel prompt:** `DEC 35: confermata` oppure `DEC 35: cambiata, [la tua scelta]`.

---

## Da decidere più avanti (D10-D13)

Queste decisioni non servono adesso. Ogni riga dice quando serve la decisione e quale fase è bloccata.

| Codice | Domanda | Scadenza | Fase bloccata | Cosa serve per decidere | Cosa scrivere nel prompt |
|---|---|---|---|---|---|
| **D10** | Nome dell'app, `ApplicationId` (identificativo permanente), tipo di account Google Play, account Partner Center (Microsoft). | Entro il 28/02/2027. | F13 (e quindi F11 e F12, che dipendono dall'identificativo). | Scelta del nome. Verifica dei nomi nei due store. | `D10: nome [...], ApplicationId [...], account Play [sì/no], Partner Center [sì/no]` |
| **D11** | Parere legale. Il perimetro è ora più ampio: pubblicità, consenso, acquisti, minori. | Nessuna data scritta nel piano. Il pilota completo parte il 12/09/2027. | F14 (Gate Anteprima e Gate Pilota). | Incarico a un legale. Domande in [07-VERIFICHE-ESTERNE-E-PROFESSIONISTI.md](07-VERIFICHE-ESTERNE-E-PROFESSIONISTI.md). | `D11: incarico dato il [data] / parere ricevuto` |
| **D12** | Codice aperto o chiuso. La scelta decide anche dove si pubblicano le release Windows del pilota. Un repository privato non rende le release scaricabili dai cori. | Entro marzo 2027 (F13, parte 1). | F13 (sede delle release) e F15 (lancio). | Vedi nota sotto: il repository risulta già pubblico. | `D12: [aperto / chiuso], release Windows su [sede]` |
| **D13** | Forma fiscale (partita IVA o altra). Ora va decisa prima del lancio. | Prima del lancio (entro il 21/11/2027). | F15 (lancio) e F17 (piano coro). | Parere del commercialista (Gate 0, attività 12). Vedi `07`. | `D13: [forma scelta], commercialista [nome o da scegliere]` |

**Nota su D12.** Il repository `F1-Solution/AppCanzoniere` risulta pubblico (verificato il 20/11/2026). I documenti di piano lo chiamano privato. Una parte della decisione è quindi già presa di fatto. Conferma lo stato e la scelta. Vedi anche [05-GITHUB-E-MACCHINA-LOCALE.md](05-GITHUB-E-MACCHINA-LOCALE.md).

**Nota su D10.** D10 non deve cambiare dopo la pubblicazione. L'`ApplicationId` è permanente.

---

## Punti tecnici aperti (non serve decidere)

Sono problemi noti di qualità. Non serve una decisione. Li trovi anche in `docs/DECISIONI-ROUND-8.md`.

- Libretto 14: ha 20 canti in meno rispetto al suo indice.
- Libretti 16 e 17: trovano più canti di quanti ne conti l'indice.
- OCR su Windows: a volte perde accordi in grassetto.
- Scansione 19: a 70 dpi è illeggibile.
- Pagine ruotate di 90 gradi senza testo: non vengono lette.
- Lettura dei PDF su Android in un processo separato: rinviata alla fase F7.
- Schermata di import: non è ancora nell'app, né su Android né su Windows.
- Primo tocco su Android: può spostare lo scorrimento di pochi pixel. Da verificare sul tablet (prova T2).
