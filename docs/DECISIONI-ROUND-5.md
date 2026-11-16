# Decisioni del proprietario — round 5 (14 novembre 2026)

Seguono le decisioni DEC 16-18 di [DECISIONI-ROUND-4.md](DECISIONI-ROUND-4.md). Chiudono la fase F2 e aprono la traccia B della fase F3 (import da PDF).

| # | Decisione | Effetto | Chi |
|---|---|---|---|
| DEC 19 | Le righe di accordi in minuscolo con spazi singoli, senza altri segni da accordo, restano testo. | "do re mi fa" in una strofa non diventa una riga di accordi. | Proprietario |
| DEC 20 | Nel testo incollato quadre e graffe diventano tonde, con un avviso. | Il ChordPro prodotto resta valido. | Proprietario |
| DEC 21 | Con il capotasto le forme seguono le alterazioni della loro tonalità (conferma di DEC 16 per "incolla"). | — | Proprietario |
| DEC 22 | Con trasposizione zero e senza capotasto la tonalità si mostra come è scritta. | La preferenza diesis/bemolle vale solo per una tonalità che suona ed è ambigua. | Orchestratore, da confermare |
| DEC 23 | Senza una lingua scelta l'app segue la lingua del dispositivo. | La notazione predefinita (DEC 9) segue la stessa lingua. | Orchestratore, da confermare |
| DEC 24 | Un PDF oltre i limiti viene rifiutato come "troppo grande": 1 MB di contenuto e 100.000 lettere per pagina; 500 pagine, 700.000 lettere, 32 MB di contenuto e 64 MB decompressi in tutto per documento; 16 MB per ogni flusso. | I libretti reali stanno almeno due volte sotto ogni limite; il peggior file accettato costa circa 9 secondi e 350 MB su PC. | Orchestratore, da confermare |
| DEC 25 | Il banco di prova dei PDF è uno strumento separato, fuori dall'app e dalla CI. | Si esegue sul corpus locale. | Orchestratore, da confermare |

## Punti rimasti aperti

- **Condizione per portare l'import dai PDF nell'app (F7):** alcuni PDF costruiti ad arte fanno chiudere o girare senza fine la libreria PdfPig mentre apre il documento o legge i caratteri, prima di ogni nostro controllo. Serve una decisione: correggere PdfPig (copia nostra o correzioni proposte agli autori) oppure leggere i PDF in un processo separato.
- Regola per indovinare la tonalità quando c'è il capotasto ma manca `{key}`: oggi `Am Dm G C Ab` con capotasto 2 dà `Gm Cm F Bb F#`, con diesis e bemolle mescolati.
- Le pagine ruotate di 90 gradi risultano senza testo.
- Divisione dei libretti in canti, glifi speciali per diesis e bemolle (caratteri TeX), OCR dei PDF scansionati.
- La schermata del canto non è ancora stata verificata a video su Android e Windows.
- Le impostazioni salvate in un database di sviluppo già esistente restano in inglese.
