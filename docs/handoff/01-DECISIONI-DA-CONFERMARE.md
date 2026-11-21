# Decisioni da confermare

Scopo: elencare le decisioni che servono dal proprietario. Dal round 10 non ci sono più decisioni di interfaccia in sospeso.

Fonti: `docs/DECISIONI-ROUND-8.md`, `docs/DECISIONI-ROUND-9.md`, `docs/DECISIONI-ROUND-10.md`, `docs/FASI.md` (sezione 0) e `docs/design/UX-IMPORT-ROUND-9.md` (sezione 12).

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

## Confermate il 21/11/2026 (round 10)

Nessuna azione richiesta. Dettagli in [DECISIONI-ROUND-10.md](../DECISIONI-ROUND-10.md).

| Voce | Decisione |
|---|---|
| DEC 36 | Lettura dei PDF nel processo dell'app, con i limiti. Lo spostamento nel processo separato è in F7. |
| DEC 37 | Canti importati salvati nella libreria attuale. Possono andare persi quando F6 rifà la persistenza. |
| DEC 38 | Dichiarazione da spuntare prima di ogni salvataggio, non preselezionata e non ricordata. |
| U-1 | Import del libretto intero in v1, per tutti, in modo definitivo. |
| U-2 | Una dichiarazione per salvataggio, con il numero dei canti nel titolo. |
| U-3 | Testo della dichiarazione proposto, finché il legale non risponde (D11). |
| U-4 | Canti selezionati all'inizio: tutti. |
| U-5 | Annullare la lettura scarta tutto. |
| U-6 | Cronometro per il Gate 0, spento per predefinito, da togliere dopo il Gate 0. |
| U-8 | Canti importati con stato dei diritti "Sconosciuto", fino a F7. |
| Palette | La pagina Importa usa i colori di Leggio fino a F4. |

---

## Non confermata (round 10)

**U-7 — Scheda "Importa" nella barra fino a F6.** Il proprietario non ha confermato la proposta. L'ingresso dell'import è già in Canti → + → Da PDF, e la scheda "Importa" è uscita dalla barra. F6 non deve spostarlo. Dettagli in [DECISIONI-ROUND-10.md](../DECISIONI-ROUND-10.md).

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

Sono problemi noti di qualità. Non serve una decisione. Li trovi anche in `docs/DECISIONI-ROUND-9.md` e `docs/DECISIONI-ROUND-10.md`.

- Libretto 14: il piè di pagina di sezione non entra più nei canti (risolto nel round 10). Restano 152 canti contro 152 voci. Una sezione di 1-2 pagine conserva ancora il suo piè di pagina.
- Libretti 15, 16, 17 e 18: trovano più canti di quanti ne conti l'indice.
- OCR su Windows: a volte perde accordi in grassetto. Non trattato nei round 9 e 10.
- Scansione 19: a 70 dpi è illeggibile. Non trattato nei round 9 e 10.
- Pagine ruotate di 90 gradi senza testo: non vengono lette. Non trattato nei round 9 e 10.
- Android, lavoro scaduto: il round 10 ha corretto di nuovo il caso (R9-09). Resta da provare sul tablet con un PDF molto grande.
- Copia in memoria: quando il selettore di Android non dà la lunghezza del file resta una copia intera del PDF (R10-06, parziale).
- ML Kit e rete: risolto nel round 10. Il traffico di metriche è eliminato e la CI lo controlla ([V-07](../verifiche/V-07-mlkit.md)).
- Revisione dei round 9 e 10: lo stato di ogni rilievo va confermato nella review successiva ([REVIEW-ROUND-9.md](../verifiche/REVIEW-ROUND-9.md), [REVIEW-ROUND-10.md](../verifiche/REVIEW-ROUND-10.md)).
- Primo tocco su Android: può spostare lo scorrimento di pochi pixel. Da verificare con la prova T2.
