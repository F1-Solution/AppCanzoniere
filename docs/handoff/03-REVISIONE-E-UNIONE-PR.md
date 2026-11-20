# Revisione e unione delle pull request

Scopo: rivedere una pull request (PR) su GitHub e unirla a `main`. Unisci solo quando i controlli sono verdi.

Stato: la PR #8 è già unita (stato "MERGED", verificato il 20/11/2026). Questa procedura serve per le PR dei round futuri.

Cosa serve:
- Un browser con accesso a GitHub, con permesso di scrittura sul repository `F1-Solution/AppCanzoniere`.
- Un PowerShell aperto nella cartella `C:\Progetti\AppCanzoniere`, per l'ultima parte.
- Tempo stimato: 20-30 minuti per PR. [IPOTESI]

Regola del progetto: si unisce solo con la CI verde. Il piano lo prevede in `docs/FASI.md`, sezione 2.1.

ATTENZIONE: il branch `main` non ha ancora protezione. GitHub permette quindi l'unione anche con controlli rossi. Controlla tu i controlli. Il passo 5 è obbligatorio.

---

## Parte 1 — Revisione e unione su GitHub

### Passi

1. Apri la pagina https://github.com/F1-Solution/AppCanzoniere/pulls.
2. Clicca la PR del round. Il titolo di solito indica il round.
3. Leggi la descrizione in alto. Deve dire cosa cambia e quali prove sono state fatte.
4. Se la descrizione manca, chiedi all'agente di completarla prima dell'unione.
5. Apri la scheda "Checks" in alto.
6. Controlla che ci siano questi quattro controlli, tutti verdi:
   - `core (Linux)`
   - `MAUI Android`
   - `MAUI Windows`
   - `Android emulator tests`
7. ATTENZIONE: se un controllo è rosso, non unire. Vai alla sezione "Cosa fare se non funziona".
8. ATTENZIONE: se un controllo è giallo, aspetta che finisca. Non unire.
9. Torna alla scheda "Conversation".
10. Apri la scheda "Files changed".
11. Controlla i nomi dei file. Cerca questi casi:
    - PDF, immagini o testi di canti. Non devono essere nel repository.
    - File con chiavi, password o `.env`.
    - File molto grandi che non servono.
12. ATTENZIONE: se trovi PDF di canti, non unire. Chiedi all'agente di toglierli.
13. Torna alla scheda "Conversation".
14. Clicca "Merge pull request".
15. Se GitHub chiede il tipo di unione, scegli "Create a merge commit". La storia di `main` usa questo tipo.
16. Clicca "Confirm merge".
17. Se appare "Delete branch", clicca per cancellare il ramo. Il passo è facoltativo.

### Risultato atteso

La PR mostra la scritta "Merged". Il ramo è su `main`.

---

## Parte 2 — Aggiornare la copia locale

Questa parte serve per avere `main` aggiornato sul tuo PC.

### Passi

18. Apri PowerShell nella cartella `C:\Progetti\AppCanzoniere`.
19. Esegui `git status`.
20. ATTENZIONE: se Git mostra modifiche non salvate, fermati. Non continuare. Chiedi all'agente.
21. Esegui `git checkout main`.
22. Esegui `git pull`.
23. Esegui `git log --oneline -3`.

### Risultato atteso

Il commit più recente della lista è il commit di unione della PR. `git status` dice che il ramo `main` è aggiornato.

---

## Alternativa: lascia fare all'agente

Scrivi questo nel prompt successivo:

```
unisci la PR #N
```

Sostituisci `N` con il numero della PR. L'agente fa prima questi controlli:
- Verifica che i quattro controlli siano verdi.
- Legge i file cambiati.

Se un controllo è rosso, l'agente non unisce. Ti dice quale controllo è rosso.

---

## Cosa fare se non funziona

- **Il pulsante "Merge pull request" è grigio.** Un controllo non è verde oppure è in corso. Apri la scheda "Checks" e leggi quale.
- **Un controllo è rosso.** Non unire. Copia il nome del controllo. Scrivi nel prompt: `La PR N ha il controllo [nome] rosso`.
- **Il pulsante di unione non c'è.** Non hai il permesso di scrittura. Chiedi all'amministratore del repository.
- **`git checkout main` dice che ci sono modifiche locali.** Non forzare. Non cancellare file. Chiedi all'agente.
- **La CI non parte.** Chiudi e riapri la PR. Se non parte ancora, scrivi nel prompt il numero della PR.

---

## Per le prossime PR

Controlla questi punti prima di unire una PR di round:

1. Il ramo ha il nome `f<fase>/<argomento>`, per esempio `f4/token`. [IPOTESI: nome del piano, DEC 15.]
2. Ogni round ha una sola PR.
3. I quattro controlli sono verdi.
4. Le prove a mano hanno un esito scritto nella descrizione della PR. Vale per le prove su tablet e PC.
5. Nella PR non ci sono PDF, chiavi o password.
6. La descrizione è breve: al massimo quattro frasi.
