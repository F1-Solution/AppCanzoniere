# Decisioni del proprietario — round 8 (19 novembre 2026)

Seguono le decisioni DEC 28-31 di [DECISIONI-ROUND-7.md](DECISIONI-ROUND-7.md).

| # | Decisione | Effetto | Chi |
|---|---|---|---|
| DEC 28-31 | Confermate dal proprietario il 19/11/2026. | — | Proprietario |
| DEC 32 | Su Android l'OCR usa il modello ML Kit incluso nell'app, che funziona senza rete e riconosce solo l'alfabeto latino. | L'APK debug cresce di circa 15,6 MB (da 94,9 a 110,5 MB). | Orchestratore, da confermare |
| DEC 33 | Le prove che non richiedono mani, pedale o occhi girano in automatico: emulatore Android in CI, runner Windows e PC di sviluppo. Al proprietario restano solo le prove su hardware reale. | Le prove su dispositivo sono in [verifiche/PROVE-SU-DISPOSITIVO.md](verifiche/PROVE-SU-DISPOSITIVO.md). | Orchestratore, da confermare |
| DEC 34 | La versione minima di Android è 6.0 (prima 5.0), perché ML Kit la richiede. | I dispositivi con Android 5.x non possono più installare l'app. | Orchestratore, da confermare |
| DEC 35 | Una riga centrata conta come titolo di canto solo se è scritta più grande del testo e non si ripete nella pagina. | I ritornelli rientrati non dividono più i canti, ma il libretto 14 perde 2 titoli veri scritti solo un po' più grandi (151 → 149 canti). | Orchestratore, da confermare |

## Punti rimasti aperti

- **Libretto 14:** ha 20 canti in meno rispetto al suo indice. La differenza è dovuta soprattutto a righe dell'indice che non sono canti (intestazioni di sezione, titoli tradotti), più i 2 titoli tolti con DEC 35.
- **Libretti 16 e 17:** trovano più canti di quanti ne conti l'indice. Il contatore dell'indice sottostima.
- **OCR su Windows:** a volte perde interi accordi in grassetto.
- **Scansione 19:** a 70 dpi è illeggibile.
- **Pagine ruotate:** le pagine ruotate di 90 gradi, senza testo, non vengono lette.
- **Android, lettura dei PDF:** la lettura in un processo separato (DEC 26) è rinviata alla fase F7.
- **Import nell'app:** la schermata di import non è ancora nell'app, né su Android né su Windows.
- **Android, primo tocco:** il primo tocco sul canto può spostare lo scorrimento di pochi pixel. Da verificare sul tablet.
- **Prove su dispositivo:** restano da eseguire, secondo [verifiche/PROVE-SU-DISPOSITIVO.md](verifiche/PROVE-SU-DISPOSITIVO.md).
