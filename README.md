# ERS27 - Prakticni zadatak 043: Evidencija popusta

Konzolna aplikacija za evidentiranje i obradu popusta. Podrzane su dvije vrste zapisa: **procentualni popust** (procenat od 1 do 90) i **fiksni popust** (pozitivan iznos). Podaci se cuvaju u memoriji ili u JSON datoteci.

## Studentski podaci

- Broj i naziv zadatka: **043 - Evidencija popusta**
- Javni GitHub repozitorijum: https://github.com/7mariJa/ERS27-MPZ043-evidencija-popusta
- Pull Request 1: https://github.com/7mariJa/ERS27-MPZ043-evidencija-popusta/pull/1
- Pull Request 2: https://github.com/7mariJa/ERS27-MPZ043-evidencija-popusta/pull/2

Svi spojeni Pull Request-ovi:

| PR | Grana | Sadrzaj |
|---|---|---|
| [#1](https://github.com/7mariJa/ERS27-MPZ043-evidencija-popusta/pull/1) | `feature/domain-i-servisi` | Korisnicke price US-01 do US-06 |
| [#2](https://github.com/7mariJa/ERS27-MPZ043-evidencija-popusta/pull/2) | `feature/domain-modeli` | F1 - dodavanje procentualnog popusta |
| [#3](https://github.com/7mariJa/ERS27-MPZ043-evidencija-popusta/pull/3) | `feature/fiksni-popust` | F2 - dodavanje fiksnog popusta |
| [#4](https://github.com/7mariJa/ERS27-MPZ043-evidencija-popusta/pull/4) | `feature/pregled-popusta` | F3 - pregled popusta i formateri prikaza |
| [#5](https://github.com/7mariJa/ERS27-MPZ043-evidencija-popusta/pull/5) | `feature/pretraga-po-sifri` | F4 - pretraga po sifri kupona |
| [#6](https://github.com/7mariJa/ERS27-MPZ043-evidencija-popusta/pull/6) | `feature/aktiviranje-popusta` | F5 - aktiviranje popusta |
| [#7](https://github.com/7mariJa/ERS27-MPZ043-evidencija-popusta/pull/7) | `feature/deaktiviranje-popusta` | F6 - deaktiviranje popusta |
| [#8](https://github.com/7mariJa/ERS27-MPZ043-evidencija-popusta/pull/8) | `feature/json-repozitorijum` | JSON repozitorijum |
| [#9](https://github.com/7mariJa/ERS27-MPZ043-evidencija-popusta/pull/9) | `feature/konzolni-logger` | Konzolni logger i izbor loggera |
| [#10](https://github.com/7mariJa/ERS27-MPZ043-evidencija-popusta/pull/10) | `feature/arhitektura-dokumentacija` | docs/architecture.md |
| [#11](https://github.com/7mariJa/ERS27-MPZ043-evidencija-popusta/pull/11) | `feature/testovi` | Testovi TE1-TE9 |

## Pokretanje

Potreban je **.NET 8 SDK**. Komande se pokrecu iz korijenskog foldera repozitorijuma.

```bash
dotnet restore
dotnet build
dotnet run --project Presentation -- in-memory
```

### Izbor rezima cuvanja podataka

Prvi argument bira repozitorijum:

| Argument | Repozitorijum | Opis |
|---|---|---|
| `in-memory` (podrazumijevano) | `InMemoryZapisiRepozitorijum` | podaci se cuvaju u memoriji i brisu se kad se aplikacija ugasi |
| `json` | `JsonZapisiRepozitorijum` | podaci se cuvaju u datoteci `podaci.json` i ostaju i poslije ponovnog pokretanja |

```bash
dotnet run --project Presentation -- in-memory
dotnet run --project Presentation -- json
```

### Izbor loggera

Drugi argument bira logger:

| Argument | Logger | Opis |
|---|---|---|
| `datoteka` (podrazumijevano) | `FileLoggerServis` | poruke se upisuju u `log.txt` |
| `konzola` | `ConsoleLoggerServis` | poruke se ispisuju u konzolu |

```bash
dotnet run --project Presentation -- json konzola
dotnet run --project Presentation -- in-memory datoteka
```

Izbor se vrsi samo u `Presentation/Composition/AppContainer`, bez izmjena u projektima Domain i Application.

U Visual Studio-u argumenti se upisuju u **Presentation -> Properties -> Debug -> Open debug launch profiles UI -> Command line arguments**.

### Meni

```
1 - Dodaj procentualni popust      (F1)
2 - Dodaj fiksni popust            (F2)
3 - Prikazi popuste                (F3 - svi ili vazeci, sazeti ili detaljni prikaz)
4 - Pretrazi po sifri kupona       (F4)
5 - Aktiviraj popust               (F5)
6 - Deaktiviraj popust             (F6)
0 - Izlaz
```

Datum vazenja se unosi u obliku `yyyy-MM-dd` (npr. `2026-12-31`). ID popusta potreban za opcije 5 i 6 vidi se u opciji 3.

## Testiranje

```bash
dotnet test
```

Testovi su u projektu `Tests` (NUnit i Moq):

| Folder | Sta se provjerava |
|---|---|
| `Tests/Domain` | pravilo P3 i dozvoljene/nedozvoljene promjene stanja (TE2, TE4) |
| `Tests/Database` | isti testovi ugovora nad memorijskim i JSON repozitorijumom, citanje JSON datoteke nakon ponovnog otvaranja (TE1, TE5, TE8) |
| `Tests/Application` | servisi uz mock repozitorijum i logger, pretraga po sifri, zavisnost samo od interfejsa za citanje (TE3, TE4, TE6, TE9) |
| `Tests/Presentation` | oba formatera prikaza i dodavanje treceg bez izmjene servisa (TE7) |
| `Tests/Architecture` | smjer zavisnosti izmedju projekata |

## Organizacija projekta

- `Domain` - modeli (`Zapis`, `ProcentualniPopust`, `FiksniPopust`), pravilo P3 (`ValidacijaPopusta`) i pravila promjene stanja
- `Application` - servisi `ZapisServis` i `PregledServis`, interfejsi repozitorijuma i loggera, `Result`
- `Database` - memorijski i JSON repozitorijum
- `Infrastructure` - `FileLoggerServis` i `ConsoleLoggerServis`
- `Presentation` - konzolni meni, formateri prikaza i `AppContainer`
- `Tests` - NUnit testovi

## Dokumentacija

- [docs/user-stories.md](docs/user-stories.md) - korisnicke price US-01 do US-06
- [docs/architecture.md](docs/architecture.md) - primjena SOLID principa i smjer zavisnosti izmedju projekata
