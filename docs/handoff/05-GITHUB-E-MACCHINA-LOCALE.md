# GitHub e macchina locale — compiti della fase F1

Scopo: chiudere i tre compiti tuoi ancora aperti per la fase F1. Il primo protegge `main`. Il secondo controlla i minuti. Il terzo ripara la build Android in locale.

Fonti: `ROADMAP.md`, riga F1. `docs/FASI.md`, sezione F1 "Dal proprietario" e verifica V-09 (sezione 6).

Stato verificato il 20/11/2026 con GitHub:
- Il branch `main` non ha protezione. Non ci sono regole (Rulesets).
- Il repository risulta pubblico. I documenti di piano dicono privato.
- Il repository è di un'organizzazione, `F1-Solution`, non di un singolo utente.

Cosa serve in tutto:
- Un browser con accesso a GitHub come proprietario o amministratore di `F1-Solution`.
- Un PowerShell come amministratore sul PC di sviluppo.

Tempo stimato: a) 15 minuti. b) 10 minuti. c) 30-60 minuti, per il download dei workload. [IPOTESI]

---

## a) Proteggere il branch `main`

**Scopo:** impedire l'unione di una PR quando un controllo è rosso.

**Cosa serve:** accesso di amministratore al repository.

**Passi:**

1. Apri la pagina https://github.com/F1-Solution/AppCanzoniere/settings/branches.
2. Se la pagina non si apre, apri le impostazioni del repository. Poi scegli "Branches".
3. Clicca il pulsante per aggiungere una regola per un branch.
4. Nel campo del nome scrivi `main`.
5. Attiva "Require a pull request before merging".
6. Lascia "Required approvals" a 0.
   ATTENZIONE: con un solo proprietario, una approvazione obbligatoria blocca le tue PR.
7. Attiva "Require status checks to pass before merging".
8. Cerca e seleziona questi quattro controlli:
   - `core (Linux)`
   - `MAUI Android`
   - `MAUI Windows`
   - `Android emulator tests`
   ATTENZIONE: i nomi compaiono solo dopo una esecuzione della CI. Se un nome manca, apri una PR, aspetta la CI e riprova.
9. Attiva "Require branches to be up to date before merging". Passo consigliato.
10. Decidi se applicare la regola anche agli amministratori. Se la attivi, nemmeno tu puoi unire con un controllo rosso. Questa scelta è consigliata.
11. Clicca per salvare la regola.

**Risultato atteso:** la pagina Branches mostra una regola per `main`. La regola elenca i quattro controlli obbligatori.

**Nota sul piano:** per un repository pubblico, la protezione dei branch è di norma disponibile anche nel piano gratuito. [DA VERIFICARE nella pagina. Se la pagina chiede un piano a pagamento, annotalo e riportalo.]

**Cosa fare se non funziona:**
- Non vedi la sezione "Branches": non sei amministratore. Chiedi l'accesso.
- Vedi un messaggio che chiede un aggiornamento del piano: copia il messaggio intero e riportalo.
- Puoi usare anche "Rules" > "Rulesets" > "New ruleset" > "Branch". Il risultato è lo stesso.

---

## b) Controllare i minuti di GitHub Actions

**Scopo:** sapere quanti minuti di CI sono usati e quanti restano nel piano.

**Cosa serve:** accesso alla fatturazione dell'organizzazione `F1-Solution`.

**Passi:**

1. Apri le impostazioni dell'organizzazione `F1-Solution`.
2. Apri la sezione di fatturazione ("Billing and licensing"). [DA VERIFICARE: il nome della voce cambia spesso.]
3. Cerca la parte "Actions" o "Usage". Può essere chiamata anche "Plans and usage".
4. Annota questi dati:
   - Il piano attuale.
   - I minuti inclusi nel piano.
   - I minuti usati nel mese.
   - I minuti usati per sistema: Linux, Windows e macOS.
   - La data di rinnovo.
5. Apri la scheda "Actions" del repository. Apri l'ultima esecuzione di "CI".
6. Annota la durata di ogni controllo. Il controllo "Android emulator tests" è il più lungo.

**Nota:** il repository è pubblico. Su GitHub i runner standard per repository pubblici sono di norma gratuiti. [DA VERIFICARE: se la fatturazione mostra costi, annotali.] La verifica V-09 del piano va quindi confermata.

**Risultato atteso:** hai quattro numeri: piano, minuti usati, minuti totali, minuti Windows.

**Cosa fare se non funziona:** la voce non compare. Non hai il ruolo di proprietario dell'organizzazione. Chiedi i dati a chi lo ha. Se la pagina non esiste più, cerca "Billing" nel menu dell'organizzazione.

---

## c) Riparare i workload .NET MAUI in locale

**Scopo:** far riuscire la build Android sul PC di sviluppo. Oggi la build fallisce con l'errore NETSDK1147.

**Cosa serve:**
- PowerShell come amministratore.
- Connessione a Internet. Il download può durare a lungo.

**Versione SDK:** il file `global.json` fissa la versione `10.0.401`, con `rollForward` a `latestPatch`. La versione `10.0.401` è installata sul PC. Sono installati anche `10.0.115`, `10.0.303` e `8.0.428`.

**Passi:**

1. Clicca Start. Scrivi `PowerShell`.
2. Clicca con il tasto destro su "Windows PowerShell". Scegli "Esegui come amministratore". Conferma con "Sì".
3. Esegui: `cd C:\Progetti\AppCanzoniere`
4. Esegui: `dotnet --version`
5. Controlla che la versione sia `10.0.401`.
   ATTENZIONE: se la versione è diversa, fermati. Riporta l'output all'agente.
6. Esegui: `dotnet workload repair`
   ATTENZIONE: il comando può durare molti minuti. Non chiudere la finestra.
7. Esegui: `dotnet workload list`
8. Controlla che siano presenti il workload Android e il workload Windows MAUI. Sulla macchina vedi `android` e `maui-windows`.
9. Se il workload Android manca, esegui: `dotnet workload install maui-android maui-windows`. [DA VERIFICARE: il nome del workload Android. Nella lista compare `android`.]
10. Esegui: `dotnet build src/AppCanzoniere.MAUI -f net10.0-android`

**Risultato atteso:** la build finisce con `Build succeeded` e `0 Error(s)`. Gli avvisi sono ammessi.

**Cosa fare se non funziona:**
- L'errore NETSDK1147 resta. Copia il messaggio intero. Riportalo all'agente insieme all'output di `dotnet workload list`.
- Il comando `dotnet workload repair` fallisce con un errore di rete. Riprova più tardi. Controlla il proxy aziendale.
- Compare un altro errore di build. Copia il messaggio intero e riportalo.
- Se il problema continua, apri "Visual Studio Installer". Controlla che sia installato il carico di lavoro per lo sviluppo mobile con .NET. [DA VERIFICARE.]

---

## Cosa riportare

Scrivi nel prompt una riga come questa:

```
GitHub: protezione di main (sì/no), controlli obbligatori (N); Actions: piano (nome), minuti usati (X su Y), Windows (Z); build Android locale: OK / KO (messaggio).
```
