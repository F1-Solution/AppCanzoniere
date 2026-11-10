Agisci come un senior software architect e sviluppatore .NET esperto in:
- .NET MAUI (Android, desktop, tablet, smartphone),
- app musicali con gestione di accordi e tablature,
- sincronizzazione dati tra DB locale e cloud,
- integrazione con servizi esterni (API AI, OCR, audio recognition),
- progettazione di app multi-tenant, multilingua, con attenzione a copyright e privacy.

========================================
OBIETTIVO GENERALE
========================================
Voglio progettare e sviluppare un’app **.NET MAUI** (Android + desktop + tablet + smartphone) che:

1. Raccolga **canzoni (soprattutto di chiesa, ma non solo)** con:
   - **tabulati/formato Guitar Tab**,
   - accordi e testi,
   - opzionalmente anche **accordi per pianoforte**.

2. Permetta di:
   - **alzare/abbassare la tonalità** (transpose) a piacimento,
   - scegliere nelle impostazioni se visualizzare/gestire la canzone per **chitarra o pianoforte** (o entrambe).

3. Gestisca un **database privato** (dell’utente) e un **database pubblico/condiviso**:
   - canzoni private (solo utente),
   - canzoni pubbliche (condivise con altri),
   - possibilità di sincronizzare il DB privato su uno storage cloud dell’utente (es. Dropbox, Google Drive, OneDrive, ecc.).

4. Supporti **metadati ricchi** per ogni canzone:
   - titolo, autori, lingua,
   - tonalità originale,
   - tipo di canzone (es. canto di chiesa, cantautore, musical, ecc.),
   - link a **uno o più video YouTube**,
   - **MP3** (file o URL),
   - **PDF** associati (testo, spartito, tab),
   - per canti di chiesa:
     - tag per identificare per quali **momenti della Messa** va bene (es. ingresso, offertorio, comunione, finale, ecc.),
     - uno o più riferimenti a **brani del Vangelo o della Bibbia** (es. “Mt 5,1-12” ecc.).

5. Offra funzionalità di:
   - **riconoscimento canzone da audio** tramite servizio tipo **Shazam** (o API equivalenti di audio recognition),
   - **ricerca accordi tramite API di AI** (es. Perplexity API o servizi simili):
     - dato titolo + autore (o altro input), chiedere un suggerimento di accordi/tab,
     - indicare bene che si tratta di suggerimenti automatici, non garantiti e soggetti a copyright.

6. Importazione automatica dei contenuti:
   - **OCR**:
     - caricare **PDF testuali** e **PDF di scansioni** (immagini),
     - estrarre testo, accordi e tabulati per inserirli nel DB canzoni.
   - **Scraping privato**:
     - funzionalità (solo per uso personale) per importare accordi da **siti di accordi**,
     - nel design, specifica che:
       - l’uso deve rispettare i Termini d’Uso e il copyright dei siti,
       - l’app deve essere progettata tenendo conto delle **limitazioni legali** (non incoraggiare scraping illegale).

7. Organizzazione in album / raccolte:
   - le canzoni possono essere raccolte in **album** o **playlist**:
     - create **manualmente** dall’utente,
     - create **automaticamente da un agente AI** (es. in base a tags, stile, uso liturgico, autore, ecc.).
   - esempi: “Canzoni di chiesa”, “Canzoni di cantautori”, “Canzoni di musical”, raccolte definite dall’utente.

8. Funzionamento **offline-friendly**:
   - possibilità di scaricare:
     - tutto il db,
     - oppure per singolo album,
     - oppure per singola canzone.
   - L’app deve poter funzionare **senza connessione** una volta scaricati i dati.

9. Aspetti di **copyright, responsabilità e privacy**:
   - il materiale caricato dagli utenti deve essere:
     - preferibilmente non coperto da copyright, **oppure** caricato solo se l’utente ha diritti/permessi,
   - la responsabilità legale deve essere chiaramente indicata come:
     - **in capo all’utente che carica i contenuti**,
   - il design deve includere:
     - schermata di **Terms & Conditions**,
     - schermata di **upload** con esplicito “Sono responsabile del contenuto e dei relativi diritti”,
     - attenzione alla gestione di dati personali (es. account, note personali).

10. L’app deve essere **multilingua** (es. almeno IT/EN all’inizio):
    - interfaccia pronta per localizzazione,
    - metadati delle canzoni con lingua indicata.

========================================
CONTESTO TECNICO
========================================
- Piattaforma: **.NET MAUI** (versione recente .NET, es. .NET 8).
- Target: Android, Windows desktop (e se proponi anche iOS/macOS ok, ma non è prioritario per ora).
- DB locale: preferibilmente **SQLite** integrato nell’app, con:
  - schema progettato per:
    - canzoni,
    - accordi/tab,
    - album/raccolte,
    - metadati (YouTube, MP3, PDF, tags liturgici e biblici),
    - impostazioni utente (strumento preferito, lingua, ecc.).
- Sincronizzazione cloud:
  - design di un layer di **sync astratto** verso provider tipo Dropbox, Google Drive, OneDrive (non serve implementare tutti i provider per ora, ma progettare interfaccia e flusso).
  - possibilità di esportare/importare il DB o un subset (album/selezione canzoni) per backup/migrazione.

- Servizi esterni:
  - **Audio recognition**:
    - puoi assumere un generico servizio REST stile Shazam-like (invio di un sample audio, ricezione di un ID canzone o titolo+autore).
    - progetta una interfaccia tipo `IAudioRecognitionService` con metodi e DTO.
  - **AI per ricerca accordi**:
    - puoi assumere un servizio tipo Perplexity / LLM via API REST,
    - l’input potrebbe essere: titolo, artista, lingua, testo parziale,
    - l’output: una bozza di accordi/tab testuali da validare.
  - **OCR**:
    - proponi un servizio OCR (cloud o libreria locale tipo Tesseract) per:
      - PDF testuali → parsing diretto,
      - PDF immagine → conversione a immagini + OCR.

- Attenzione al rispetto:
  - **copyright** (non fornire codice che aggiri DRM o restrizioni),
  - **Termini di servizio** dei provider (Shazam, YouTube, Perplexity, siti di accordi, ecc.).

========================================
COSA DEVE FARE L’APPLICAZIONE (FUNZIONALITÀ CHIAVE)
========================================

1. **Gestione canzoni**
   - Creare, modificare, cancellare canzoni.
   - Associare:
     - testo + accordi/tab per chitarra,
     - opzionali accordi per pianoforte,
     - link YouTube,
     - link/file MP3,
     - PDF (spartito/tab/testo).
   - Gestire campi specifici per:
     - canti di chiesa (momento della messa, riferimenti biblici),
     - tag generici (es. “adorazione”, “Natale”, “Pasqua”, “matrimonio”, “funerale”).

2. **Visualizzazione e transpose**
   - Visualizzare:
     - testo + accordi in modo leggibile,
     - tab chitarra,
     - accordi pianoforte (es. note dell’accordo).
   - Aumentare/diminuire la tonalità:
     - per chitarra: shift degli accordi (e se possibile adattamento della tab),
     - per pianoforte: shift degli accordi.
   - Impostazioni: preferenze per notazione (es. DO/RE/MI vs C/D/E, diesis/bemolle).

3. **Album e raccolte**
   - Creazione manuale di album:
     - aggiunta/rimozione canzoni,
     - ordinamento.
   - Creazione automatica tramite “agente AI”:
     - dato il catalogo canzoni + tag, proporre raccolte automatiche (es. tutte le canzoni di un certo stile, momento liturgico, autore, etc.).

4. **Ricerca e filtri**
   - Ricerca per:
     - titolo,
     - autore,
     - tag,
     - tonalità,
     - tipo (canto di chiesa, cantautore, ecc.),
     - lingua.
   - Filtri per momenti della messa, riferimenti biblici, ecc.

5. **Import/export**
   - Import:
     - da PDF testuali (parsing del testo e riconoscimento accordi/tab),
     - da PDF scansioni tramite OCR,
     - da siti di accordi (scraping **limitato e conforme a copyright/TOS**, meglio progettare interfacce e lasciare open agli adattamenti).
   - Export:
     - singola canzone (es. come file JSON/ChordPro o altro formato),
     - album intero,
     - backup dell’intero DB o di una selezione.

6. **Online/offline e condivisione**
   - Modalità offline:
     - tutti i dati necessari per le canzoni scaricate devono essere disponibili senza rete.
   - Modalità online:
     - sincronizzazione con DB privato su cloud (backup/restore),
     - accesso al DB pubblico (canzoni condivise da altri, marcate come tali).
   - Distinzione chiara tra:
     - canzoni private,
     - canzoni pubbliche.

7. **Gestione responsabilità e copyright**
   - UI/flow per:
     - mostrare all’utente avvisi su copyright,
     - far accettare termini e responsabilità al momento dell’upload,
     - segnare se una canzone è originale, di dominio pubblico, coperta da licenza, ecc.
   - Nel codice, prevedere un modello dati che tenga traccia:
     - di chi ha caricato cosa (almeno a livello di identificatore locale, se si prevede login in futuro),
     - dello stato legale dichiarato dall’utente (a puro scopo informativo).

========================================
COSA MI ASPETTO DALLA TUA RISPOSTA
========================================
Voglio che la tua risposta sia strutturata in più sezioni, in questo ordine:

1. **Domande di chiarimento**
   - Fammi solo le domande essenziali (es. se voglio supportare iOS/macOS, quale provider cloud preferire per un primo prototipo, se preferisco un certo servizio OCR, ecc.).

2. **Architettura ad alto livello**
   - Descrivi:
     - i principali layer/moduli dell’app (UI, domain, data access, servizi esterni, sync, AI),
     - pattern suggeriti (es. MVVM per MAUI, uso di dependency injection, ecc.),
     - come gestire differenze tra piattaforme mantenendo il più possibile codice condiviso.

3. **Modello dati e schema del database**
   - Proponi un design delle entità principali:
     - `Song`, `ChordSheet`, `Tab`, `Album`, `Tag`, `MediaAttachment`, `LiturgicalTag`, `BibleReference`, `UserSettings`, ecc.
   - Spiega quali tabelle potrebbero stare nel DB locale SQLite.
   - Descrivi come modellare:
     - canzoni private vs pubbliche,
     - sincronizzazione (es. versioning, timestamp, flag “dirty” per sync, ecc.).

4. **Flussi utente principali (UX)**
   - Descrivi i flow per:
     - cercare e aprire una canzone,
     - cambiare tonalità,
     - scegliere chitarra/piano,
     - importare una canzone da PDF (testo o scansione),
     - invocare OCR e AI per generare accordi,
     - creare un album manuale,
     - usare l’agente AI per creare una raccolta automatica,
     - scaricare dati per l’uso offline.

5. **Integrazione con servizi esterni (AI, OCR, audio recognition, cloud)**
   - Definisci interfacce in stile C# (pseudo-code) per:
     - `IAudioRecognitionService`,
     - `IChordSuggestionService` (per Perplexity/LLM),
     - `IOcrService`,
     - `ICloudSyncService`.
   - Spiega le responsabilità di ciascuna e come iniettarle nei ViewModel/servizi applicativi.

6. **Gestione multilanguage (UI e contenuti)**
   - Spiega come impostare la localizzazione in MAUI (resx, ecc.).
   - Descrivi come gestire la lingua di UI e quella delle canzoni.

7. **Esempi di codice C# (MAUI)**
   - Fornisci esempi di:
     - modello `Song` con metadati,
     - una pagina MAUI (XAML + code-behind/ViewModel) che:
       - mostra una canzone con accordi,
       - permette il transpose di tonalità per chitarra,
     - un servizio semplice per eseguire il transpose degli accordi,
     - un esempio di chiamata a un servizio esterno (es. `IAudioRecognitionService`) usando HttpClient (anche simulato).

8. **Considerazioni su sicurezza, copyright e limiti**
   - Evidenzia:
     - quali parti sono delicate a livello legale (scraping, AI chords, condivisione pubblica),
     - come strutturare il codice e la configurazione per poter abilitare/disabilitare facilmente certe funzionalità in base al contesto legale,
     - quali sono le limitazioni dell’approccio proposto.

Segui un approccio iterativo: prima proponimi l’architettura e il modello dati, poi possiamo scendere insieme nel dettaglio del codice e dei servizi esterni.
Alla fine crea un file plan.md nella stessa cartella di questeo file