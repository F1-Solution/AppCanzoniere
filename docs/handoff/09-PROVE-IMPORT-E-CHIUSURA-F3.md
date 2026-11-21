# Prove dell'import e chiusura di F3

Scopo: eseguire le prove della schermata Importa su tablet e PC, misurare il tempo di correzione dei libretti e riportare gli esiti. Con questi esiti la fase F3 si chiude.

Stato: **da fare**. F3 è in corso e si chiude solo con i tuoi esiti.

Scadenze:
- Traccia B (misura dei libretti): 17/01/2027.
- Traccia A (scorrimento e pedale): 24/01/2027.

Documenti collegati:
- [PROVE-SU-DISPOSITIVO.md](../verifiche/PROVE-SU-DISPOSITIVO.md): tutte le prove, con i passi.
- [CHIUSURA-F3.md](../verifiche/CHIUSURA-F3.md): i criteri, le tabelle da compilare e la riga finale.
- [10-CONFRONTO-VISIVO-IMPORTA.md](10-CONFRONTO-VISIVO-IMPORTA.md): confronto della schermata con la proposta visiva.

---

## Cosa serve

- Il tablet Android con l'app installata (round 9).
- Il PC Windows con la cartella `windows-debug-build` estratta.
- Il pedale Bluetooth, accoppiato al tablet e al PC.
- Quattro file PDF, come descritto nel passo 3 di questa guida.
- Un timer.
- Un telefono per i video.
- Un righello in millimetri.
- Dieci libretti PDF reali, per la misura. Di ogni libretto devi avere il diritto di usare una copia.

---

## Passi

### Parte 1 — Preparazione

1. Esegui i passi 1 a 28 della sezione "Preparazione" in [PROVE-SU-DISPOSITIVO.md](../verifiche/PROVE-SU-DISPOSITIVO.md).
2. Apri la sezione "Preparazione" della guida. Il passo 25 dice quali quattro file preparare.
ATTENZIONE: non copiare i PDF nella cartella del progetto. Non caricarli su GitHub.

3. Copia i quattro file sul tablet nella cartella "Download".
4. Copia i quattro file sul PC nella cartella "Documenti".

### Parte 2 — Prove sul tablet

6. Esegui le prove I-T1, I-T2, I-T3, I-T4 nell'ordine.
7. Esegui le prove I-T5, I-T6, I-T7, I-T8 nell'ordine.
8. Esegui la prova I-T9 con il righello.
9. Non eseguire ancora I-T10. Si esegue nella Parte 4, con i libretti.
10. Annota gli esiti nella tabella di "Come riportare gli esiti" di PROVE-SU-DISPOSITIVO.md.

### Parte 3 — Prove sul PC

11. Esegui le prove W1 e W2, poi W3 a W7.
12. Esegui le prove I-W1, I-W2, I-W3, I-W4, I-W5.
13. Annota gli esiti nella tabella. Non eseguire ancora I-W6.

### Parte 4 — Misura dei libretti (traccia B)

14. Scegli dieci libretti reali. Dai a ciascuno un codice: L01, L02 e così via.
15. Per ogni libretto, segna se il PDF ha testo oppure è una scansione.
16. Scegli il dispositivo per la misura. Puoi usare il tablet (I-T10), il PC (I-W6), o entrambi.
17. Per ogni libretto, scegli cinque canti. Scrivi i numeri dei canti scelti.
18. ATTENZIONE: misura il tempo di un canto alla volta. Non aprire due canti insieme.
19. Esegui I-T10 o I-W6 per ogni canto scelto. Il tempo si legge in alto nella revisione.
20. Scrivi per ogni canto i minuti. Scrivi "sì" se sono 2 minuti o meno. Scrivi "no" se sono più di 2.
21. Premi "Copia i tempi" in elenco. Incolla il testo nel foglio di registrazione.
22. Ripeti i passi 17 a 21 per tutti i libretti.

### Parte 5 — Chiusura

23. Apri [CHIUSURA-F3.md](../verifiche/CHIUSURA-F3.md).
24. Compila la sezione 3: prove, libretti e riepilogo.
25. Decidi i punti della sezione 4. Scrivi "confermato" o "cambiato" per ciascuno.
26. Compila la riga della sezione 5.
27. Incolla la riga nel prompt successivo.

---

## Cosa fare se qualcosa non funziona

- Se una prova è KO, scrivi il numero del passo e cosa è successo. Continua con la prova successiva.
- ATTENZIONE: se l'app si chiude durante una lettura, non riprovare subito. Annota il file, la piattaforma e il momento. Poi riavvia l'app.
- ATTENZIONE: su Android una lettura che supera 2 minuti mostra un errore. La lettura però continua in background. Aspetta qualche minuto prima di un nuovo import.
- Se il riconoscimento di una scansione è lento, è normale alla prima lettura. Annota il tempo.

---

## Cosa scrivere nel prompt

Usa la riga della sezione 5 di [CHIUSURA-F3.md](../verifiche/CHIUSURA-F3.md). Se una prova non è stata fatta, scrivi "non fatto" al suo posto.

Non scrivere nel prompt i nomi dei libretti, dei cori o delle persone. Usa solo i codici.
