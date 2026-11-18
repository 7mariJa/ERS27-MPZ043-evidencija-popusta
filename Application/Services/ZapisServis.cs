using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Models;
using Domain.Pravila;
using Shared;

namespace Application.Services;

// Polazna struktura servisa; poslovne operacije nisu implementirane.
public sealed class ZapisServis
{
    /*()*/
    private readonly IZapisiRepozitorijum _repozitorijum;
    private readonly ILoggerServis _logger;

    public ZapisServis(IZapisiRepozitorijum repozitorijum, ILoggerServis logger)
    {
        _repozitorijum = repozitorijum ?? throw new ArgumentNullException(nameof(repozitorijum));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    /*()*/
    public Result<Guid> DodajProcentualni(string naziv, string opis, string sifra, decimal procenat, DateTime datum) //<Guid> u slucaju uspjeha vraca tip id - a
    {
        // TODO: definisati poslovna pravila i pozvati repozitorijum.
        // Primer pokazuje gde pripada use case, ne daje rešenje zadatka.

        var popust = new ProcentualniPopust     //novi objekat - jedan popust //var - C# sam odlucuje tip
        {
            Naziv = naziv,
            Opis = opis,
            SifraKupona = sifra,
            Procenat = procenat,
            DatumVazenja = datum
            //Id, Status se ne pisu jer se kreiraju sami (Id dobije novi Guid.NewGuid())
        };

        string? greska = ValidacijaPopusta.Provjeri(popust); //Provjera pravila T3
        if(greska != null)
        {
            _logger.Loguj("Neuspjesno dodavanje: " + greska);       //upisuje neuspijeh u log.txt to trazi T4
            return Result<Guid>.Failure("VALIDATION_ERROR", greska);//vraca neuspijeh i odmah izlazi iz metode      kod se ne izvrsava, pa se popust ne cuva T4
        }   

        _repozitorijum.Sacuvaj(popust);             //uspjeh se cuva
        _logger.Loguj("Uspjesno dodavanje procentualnog popusta: " + popust.Id);//upisuje u log
        return Result<Guid>.Success(popust.Id); //vraca uspjeh i id novog popusta F1
    }

    public Result<Guid> DodajFiksni(string naziv, string opis, string sifra, decimal iznos, DateTime datum)
    {
        var popust = new FiksniPopust
        {
            Naziv = naziv,
            Opis = opis,
            SifraKupona = sifra,
            Iznos = iznos,
            DatumVazenja = datum
        };

        string? greska = ValidacijaPopusta.Provjeri(popust);
        if(greska != null)
        {
            _logger.Loguj("Neuspjesno dodavanje: " + greska);
            return Result<Guid>.Failure("VALIDATION_ERROR", greska);
        }

        _repozitorijum.Sacuvaj(popust);
        _logger.Loguj("Uspjesno dodavanje fiksnog popusta: " + popust.Id);
        return Result<Guid>.Success(popust.Id);
    }

    // F5 - aktiviranje popusta
    public Result Aktiviraj(Guid id)
    {
        var pronadjen = _repozitorijum.Pronadji(id);
        if (pronadjen.IsFailure)
        {
            _logger.Loguj("Neuspjesno aktiviranje, ne postoji popust: " + id);
            return Result.Failure("nema", "Popust sa tim ID-jem ne postoji.");
        }

        Zapis popust = pronadjen.Value!;
        string? greska = popust.Aktiviraj();
        if (greska != null)
        {
            _logger.Loguj("Neuspjesno aktiviranje: " + greska);
            return Result.Failure("nedozvoljeno", greska);
        }

        _repozitorijum.Azuriraj(popust);
        _logger.Loguj("Uspjesno aktiviran popust: " + id);
        return Result.Success();
    }
    
}
