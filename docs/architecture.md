# Архитектура решења

За сваку ставку навести конкретне класе/интерфејсе из сопственог решења и доказ о примени (метод, тест или могућност замене имплементације). Наводи без везе са кодом се не прихватају.

## SRP — једна одговорност

Svaka klasa radi jednu stvar:

| Odgovornost | Klasa |
|---|---|
| Provjera pravila P3 (naziv i opis nisu prazni, procenat 1-90, iznos > 0) | `Domain/Pravila/ValidacijaPopusta` |
| Pravila promjene stanja (F5, F6) | `Domain/Models/Zapis` - metode `Aktiviraj()` i `Deaktiviraj()` |
| Izvrsavanje operacija F1, F2, F5, F6 | `Application/Services/ZapisServis` |
| Izvrsavanje operacija F3, F4 | `Application/Services/PregledServis` |
| Cuvanje podataka | `Database/Repositories/InMemoryZapisiRepozitorijum`, `JsonZapisiRepozitorijum` |
| Logovanje | `Infrastructure/Services/FileLoggerServis`, `ConsoleLoggerServis` |
| Formiranje teksta za prikaz | `Presentation/Formateri/SazetiFormater`, `DetaljniFormater` |
| Citanje sa konzole i meni | `Presentation/Console/ConsoleMenu` |

**Kako se provjerava:** `ZapisServis` i `PregledServis` nemaju nijedan poziv `Console` ni `File` i ne prave tekst za prikaz - samo vracaju `Result`. Unos sa tastature i parsiranje broja/datuma je u `ConsoleMenu`, a tekst za ispis prave formateri. Pravilo P3 se mijenja samo u `ValidacijaPopusta`, bez diranja servisa.

## OCP — отвореност за проширење

Za prikaz F3 definisan je ugovor `Presentation/Interfaces/IPopustFormater` sa metodom `string Formatiraj(Zapis zapis)` i dvije implementacije:

- `SazetiFormater` - tip, ID, naziv i status
- `DetaljniFormater` - svi podaci, ukljucujuci procenat popusta ili iznos popusta

**Kako se provjerava:** `PregledServis` vraca listu zapisa i ne zna nista o formatima. Za novi format (npr. CSV) dovoljno je napraviti novu klasu `CsvFormater : IPopustFormater` i dodati je kao izbor u meniju - `PregledServis` i ostale klase u Application i Domain se ne mijenjaju.

## LSP — заменљивост имплементација

`InMemoryZapisiRepozitorijum` i `JsonZapisiRepozitorijum` implementiraju isti interfejs `IZapisiRepozitorijum` i ponasaju se isto:

- `Sacuvaj` / `Azuriraj` - upis i izmjena zapisa
- `Pronadji` - za nepostojeci ID oba vracaju `Result.Failure` sa porukom "Zapis nije pronadjen."
- `Svi` - vracaju sve zapise, za praznu evidenciju praznu listu
- oba cuvaju obe vrste zapisa (`ProcentualniPopust` i `FiksniPopust`); JSON repozitorijum u datoteku upisuje i tip zapisa da bi pri citanju napravio pravi objekat

Pravila P3 nisu u repozitorijumima, nego u `ValidacijaPopusta` (Domain) i servisima (Application), pa ne zavise od nacina cuvanja.

**Kako se provjerava:** u `Tests/Database/RepozitorijumUgovorTests` je apstraktna klasa sa testovima ugovora (cuvanje i citanje obe vrste popusta, nepostojeci ID, azuriranje). Iste testove nasljedjuju `InMemoryRepozitorijumTests` i `JsonRepozitorijumTests`, pa se izvrsavaju nad oba repozitorijuma i oba moraju proci. `JsonRepozitorijumTests.PonovnoOtvaranje_PodaciSuSacuvani` provjerava da podaci ostaju poslije ponovnog otvaranja datoteke. Aplikacija takodje radi isto sa `dotnet run --project Presentation -- in-memory` i `dotnet run --project Presentation -- json`.

## ISP — раздвајање интерфејса

Pocetni `IZapisiRepozitorijum` je podijeljen na dva uska interfejsa:

- `IZapisiCitanjeRepozitorijum` - `Pronadji`, `Svi`
- `IZapisiUpisRepozitorijum` - `Sacuvaj`, `Azuriraj`, `Obrisi`

`IZapisiRepozitorijum` samo spaja ta dva, a oba repozitorijuma ga implementiraju (jedna klasa implementira vise uskih interfejsa).

**Kako se provjerava:** komponenta za pregled i pretragu (F3, F4) - `PregledServis` - u konstruktoru prima samo `IZapisiCitanjeRepozitorijum`, pa ne moze pozvati `Sacuvaj`, `Azuriraj` ni `Obrisi` (kompajler to ne bi dozvolio). Upis i promjenu stanja radi samo `ZapisServis`.

## DIP — инверзија зависности

Servisi zavise od interfejsa, a ne od konkretnih klasa, i dobijaju ih kroz konstruktor:

- `ZapisServis(IZapisiRepozitorijum repozitorijum, ILoggerServis logger)`
- `PregledServis(IZapisiCitanjeRepozitorijum repozitorijum)`

Konkretne klase bira samo `Presentation/Composition/AppContainer` u metodi `Create(rezim, vrstaLoggera)`:

- repozitorijum: `InMemoryZapisiRepozitorijum` ili `JsonZapisiRepozitorijum`
- logger: `FileLoggerServis` ili `ConsoleLoggerServis`

**Kako se provjerava:** izbor se mijenja samo argumentima pri pokretanju, bez ikakve izmjene koda u Domain i Application:

```bash
dotnet run --project Presentation -- in-memory datoteka
dotnet run --project Presentation -- json konzola
```

Servisi nigdje ne koriste `new` za repozitorijum ni logger.

## Clean Architecture — смер зависности

```
            Presentation
                 |
                 v
Database ---> Application <--- Infrastructure
                 |
                 v
               Domain
```

Reference izmedju projekata (iz `.csproj` fajlova):

| Projekat | Zavisi od |
|---|---|
| Domain | nicega |
| Application | Domain |
| Database | Application, Domain |
| Infrastructure | Application |
| Presentation | Application, Domain, Database, Infrastructure (samo zbog `AppContainer`) |

Interfejsi `IZapisiRepozitorijum` i `ILoggerServis` su u Application, a implementiraju ih Database i Infrastructure - zato strelice idu prema Application, a ne obrnuto. Domain ne pristupa konzoli ni datotekama.

**Kako se provjerava:** testovi u `Tests/Architecture/LayeringTests`:

- `Domain_NemaZavisnostiOdSpoljasnjihProjekata` - Domain ne referencira Application, Database, Infrastructure ni Presentation
- `Application_NeZavisiOdAdaptera` - Application ne referencira Database, Infrastructure ni Presentation
- `Slojevi_SuUOdvojenimProjektima` - slojevi su u odvojenim projektima

Pokrecu se sa `dotnet test`.
