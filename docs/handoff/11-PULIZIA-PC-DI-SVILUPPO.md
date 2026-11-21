# Pulizia della libreria di prova sul PC di sviluppo

Scopo: svuotare la libreria dell'app di prova installata sul PC di sviluppo. Nella libreria ci sono circa 150 canti di prova salvati durante lo sviluppo.

Stato: **da fare**. Fallo quando vuoi una libreria pulita, per esempio prima della misura del Gate 0.

ATTENZIONE: cancellare il file del database elimina **tutti** i canti dell'app di prova sul PC. Non ci sono canti da recuperare dopo. Se vuoi tenere alcuni canti, copia il file prima (passo 6).

ATTENZIONE: non cancellare nulla dentro la cartella del progetto `C:\Progetti\AppCanzoniere`. Il database non è lì.

Il database è un file chiamato `appcanzoniere.db`. Si trova nella cartella dei dati locali dell'app. Per l'app di prova installata da cartella (non dallo store) questa cartella è di solito dentro `%LOCALAPPDATA%`.

---

## Passi

### Trovare il file

1. Chiudi l'app di prova. Se è aperta, chiudila dalla finestra. Non lasciarla in secondo piano.
2. Apri Gestione attività (Ctrl+Maiusc+Esc).
3. Nella scheda "Processi", cerca `AppCanzoniere.MAUI`.
4. ATTENZIONE: se il processo è ancora attivo, selezionalo e scegli "Termina attività". Il file del database non si può cancellare con l'app aperta.
5. Apri Esplora file.
6. Nella barra dell'indirizzo, scrivi `%LOCALAPPDATA%` e premi Invio.
7. Nella casella di ricerca in alto a destra, scrivi `appcanzoniere.db` e premi Invio.
8. Aspetta che la ricerca finisca. Annota il percorso completo di ogni file trovato.
9. Controlla la dimensione e la data di modifica di ogni file. Il file della libreria di prova è di solito quello più grande e più recente.

### Salvare una copia, se vuoi tenere i canti

10. Se vuoi tenere i canti, copia `appcanzoniere.db` in una cartella fuori dal progetto, per esempio "Documenti\backup-canti".
11. Se nella stessa cartella ci sono `appcanzoniere.db-wal` o `appcanzoniere.db-shm`, copiali anche tu.

### Cancellare il database

12. Controlla di nuovo che l'app di prova sia chiusa (passi 1 a 4).
13. Seleziona `appcanzoniere.db`.
14. Premi Canc. Conferma la cancellazione.
15. Se ci sono `appcanzoniere.db-wal` o `appcanzoniere.db-shm` nella stessa cartella, cancellali anche tu.

### Verificare

16. Apri l'app di prova dalla cartella di installazione.
17. Apri la libreria.
18. Controlla che non ci siano canti importati o salvati durante lo sviluppo.
19. Se compaiono ancora canti, annota i titoli e scrivili nel prompt successivo.

---

## Cosa scrivere nel prompt

`Pulizia PC: database trovato in [percorso completo], copia salvata [sì/no], libreria vuota [sì/no], canti rimasti [numero o nessuno].`

Non scrivere nel prompt i titoli dei canti salvati.
