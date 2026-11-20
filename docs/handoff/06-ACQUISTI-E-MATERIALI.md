# Acquisti e materiali

Scopo: elencare il materiale fisico e i contenuti che devi procurare. Per ogni voce vedi la fase che la richiede e quando serve.

Fonti: `docs/FASI.md`, righe "Dal proprietario" delle fasi F1, F2, F3, F6, F7, F8, F9 e F10. Le altre voci vengono da `docs/DECISIONI-ROUND-8.md`, da `docs/verifiche/PROVE-SU-DISPOSITIVO.md` e dalle sezioni 5 e 6 di `docs/FASI.md`.

Dove restano i PDF dei canti: solo nella cartella locale `corpus-locale/`. Vedi la regola nella sezione "Regola sui PDF" in fondo.

Costi: dove il costo non è scritto nei documenti, la voce dice "[DA VERIFICARE]".

---

## 1. Hardware per le prove

| Cosa | Quanto costa | Fase che lo richiede | Quando serve | Note |
|---|---|---|---|---|
| Tablet Android economico | 100-150 € (piano F1) | F3 (prove T1-T7), F7 | Ordinalo adesso. Serve prima della traccia A di F3 (entro 24/01/2027). | Deve avere Android 6.0 o superiore (DEC 34). Consigliato: tasti del volume fisici, usati dalla prova T4. [IPOTESI] |
| Pedale Bluetooth | 30-60 € (piano F1) | F3 (prove), F7 | Ordinalo adesso, dopo la verifica sotto. | Deve inviare Pagina giù e Pagina su, oppure le frecce. Vedi "Come verificare il pedale". |
| PC Windows 10 o 11 | [DA VERIFICARE se già disponibile] | F1, F3 (prove W1-W7), F7 | Adesso. | Il pacchetto usa il target Windows 10 versione 2004 o successiva (10.0.19041). [IPOTESI: ricavato dal target del progetto.] |
| Telefono per i video | [DA VERIFICARE se già disponibile] | Prove T6 e W6 | Prima delle prove. | Serve un supporto stabile per il telefono. |
| Timer o cronometro | [DA VERIFICARE se già disponibile] | Prove T3, T5, B1 | Prima delle prove. | Il telefono può fare da timer. |
| Supporto per il tablet (leggio) | [DA VERIFICARE] | Prova T7 (facoltativa, chiesta dal Gate 0) | Prima della prova in chiesa. | Il tablet deve stare a distanza di lettura. |
| Secondo dispositivo Android | [DA VERIFICARE] | F8 (due dispositivi, condivisione della scaletta) | Prima della fase F8 (aprile 2027). | Può essere il telefono di un familiare. |

### Come verificare il pedale prima di comprarlo

Scopo: sapere se il pedale invia i tasti giusti. L'app lavora con Pagina giù, Pagina su e le frecce.

Cosa serve: un PC Windows, un blocco note e la scheda tecnica del pedale. Se il pedale è già in casa, serve anche quello.

Passi:

1. Apri la scheda tecnica o la pagina del prodotto.
2. Cerca le parole "tastiera", "keyboard" o "HID". Il pedale deve avere una modalità tastiera.
3. ATTENZIONE: scarta i pedali che dichiarano solo "mouse" o solo "MIDI". Non inviano i tasti giusti. [IPOTESI]
4. Scegli un venditore che accetta la restituzione. [DA VERIFICARE la condizione del venditore.]
5. Accoppia il pedale al PC. Vai su Impostazioni > Bluetooth e dispositivi.
6. Apri Blocco note.
7. Scrivi 60 righe con numeri, da 1 a 60. Premi Invio dopo ogni numero.
8. Premi il tasto avanti del pedale una sola volta.
9. Osserva il cursore:
   - Se il cursore salta in basso di una pagina, il pedale invia Pagina giù.
   - Se il cursore scende di una riga, il pedale invia Freccia giù.
   - Se il cursore non si muove, il pedale non è in modalità tastiera. Consulta il manuale.
10. Premi il tasto indietro del pedale una sola volta. Osserva come sopra, verso l'alto.
11. Annota il tasto che hai visto per ogni direzione.

**Risultato atteso:** il pedale invia Pagina giù e Pagina su, oppure le frecce. Lo dice la prova W1.

---

## 2. Contenuti: canti e calendari

| Cosa | Quanto | Fase che lo richiede | Quando serve | Note |
|---|---|---|---|---|
| Libretti PDF reali | 10 libretti | F3 (prova B1), F7 | Prima della prova B1. Gate 0: entro il 17/01/2027. | I PDF restano nella cartella locale `corpus-locale/`. Non vanno su GitHub. |
| Canti reali con esito atteso | 10-20 canti. Per almeno 5 scrivi a mano il risultato atteso. | F2 (casi di test) | Prima dei casi di test di F2. | [DA VERIFICARE: nel ROADMAP F2 risulta già completata. Il piano in FASI.md la dà ancora da iniziare.] |
| Canti propri | 20-30 canti | F6 (prova di aggiornamento su tablet e PC) | Prima di F6. | Usa canti che possiedi con i diritti. |
| Scaletta reale | Una scaletta della tua parrocchia | F8 | Prima di F8. | Serve per la prova di condivisione. |
| Calendari liturgici ufficiali | Calendario CEI annuale o Ordo, per 2026, 2027 e 2028 | F9 (test del calendario) | Febbraio 2027. | Serve anche un controllo a campione su 30 giorni. |
| Uso per due domeniche | Due domeniche nella tua parrocchia | F10 | Prima di F10. | Serve anche un elenco dei momenti della Messa più usati. |
| Canti di dominio pubblico | 20-40 canti | F14 (nucleo per la verifica legale) | Prima dell'anteprima (2 maggio 2027). | Ogni canto deve avere la fonte. |
| Cori per l'anteprima | Almeno 3 cori | F14 | Prima dell'anteprima (2 maggio 2027). | Vedi `08`. |
| Tester del test chiuso | Almeno 12 indirizzi Google | F13 (parte 2), F14 | Aprile 2027. | Vedi `07` e `08`. |

---

## 3. Account, servizi e consulenze

| Cosa | Quanto costa | Fase che lo richiede | Quando serve | Note |
|---|---|---|---|---|
| Account Google Play | 25 $, una tantum | F13 (parte 1) | Marzo 2027. | Serve la verifica dell'identità. Vedi `07`. |
| Account sviluppatore Microsoft (Partner Center) | [DA VERIFICARE]. Il piano V-04 lo dà a circa 19 $ una tantum, da controllare. | F12, F13 | Prima di F12 (2027). | Il costo attuale va verificato. |
| Certificato di firma del pacchetto Windows | [DA VERIFICARE] | F13 (pacchetto pilota) | Prima del pilota (settembre 2027). | Vedi V-08 in `07`. |
| Account AdMob | [DA VERIFICARE] | F11 (pubblicità) | Prima di F11 (2027). | Va collegato al profilo di pagamento. |
| Account Apple Developer | 99 $ all'anno | F16 (iPad e iOS), dopo il lancio | Solo se il censimento lo giustifica. | Fase condizionata. |
| Dominio e servizio di pagamento per le donazioni | [DA VERIFICARE] | F14, F15 | Prima del lancio. | Scelta tua (DEC 12). |
| Commercialista | Circa 600 € all'anno (piano commerciale, sezione 8.1) | Gate 0 (attività 12), F15 | Prima del lancio. Il Gate 0 scade il 31/01/2027. | Il piano lo rende obbligatorio per la v1. Vedi `07`. |
| Parere legale | Circa 800 € (stima del piano, fase F17) [DA VERIFICARE] | F14 (decisione D11) | Prima del pilota completo (12/09/2027). | Vedi `07`. |

---

## Regola sui PDF

ATTENZIONE: i PDF dei canti non devono mai finire su GitHub. Non caricarli in una PR, in un commit o in un'immagine.

- I PDF restano nella cartella `corpus-locale/`.
- La cartella è esclusa da Git nel file `.gitignore`, alla riga 100.
- Il motivo è il diritto d'autore. Il piano commerciale lo richiede.

Se un PDF compare in `git status`, non fare commit. Chiedi all'agente di toglierlo dal repository.
