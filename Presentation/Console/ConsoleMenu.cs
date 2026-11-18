using Domain.Models;
using Presentation.Composition;
using Presentation.Formateri;
using Presentation.Interfaces;
using Shared;

namespace Presentation.Console;

// Meni treba dopuniti operacijama F1-F6 iz dodeljenog zadatka.
public sealed class ConsoleMenu
{
    private readonly IConsoleIO _io;
    private readonly AppContainer _container;

    public ConsoleMenu(IConsoleIO io, AppContainer container)
    {
        _io = io;
        _container = container;
    }

    public void Run()
    {
        while (true)
        {
            _io.WriteLine("1 - Dodaj procentualni popust");
            _io.WriteLine("2 - Dodaj fiksni popust");
            _io.WriteLine("3 - Prikazi popuste");
            _io.WriteLine("0 - Izlaz");
            string izbor = _io.ReadLine();

            if( izbor == "1" ) DodajProcentualni();
            else if( izbor == "2" ) DodajFiksni();
            else if( izbor == "3" ) PrikaziPopuste();
            else if( izbor == "0" ) break;
            else _io.WriteLine("Nepoznata opcija.");
        }
    }

    public void DodajProcentualni()
    {
        _io.Write("Unesite naziv popusta: ");
        string naziv = _io.ReadLine();

        _io.Write("Unesite opis popusta: ");
        string opis = _io.ReadLine();

        _io.Write("Unesite šifru kupona: ");
        string sifra = _io.ReadLine();

        _io.WriteLine("Unesite procenat popusta (decimalni broj):");
        if(!decimal.TryParse(_io.ReadLine(), out decimal procenat))
        {
            _io.WriteLine("Neispravan unos procenata. Operacija prekinuta.");
            return;
        }

        _io.WriteLine("Unesite datum važenja (yyyy-MM-dd):");
        if(!DateTime.TryParse(_io.ReadLine(), out DateTime datum))
        {
            _io.WriteLine("Neispravan unos datuma. Operacija prekinuta.");
            return;
        }

        var rezultat = _container.Servis.DodajProcentualni(naziv, opis, sifra, procenat, datum);
        if (rezultat.IsSuccess)
            _io.WriteLine("Uspješno dodan popust sa ID: " + rezultat.Value);
        else
            _io.WriteLine("Greška: " + rezultat.Error!.Message);
    }

    public void DodajFiksni()
    {
        _io.Write("Unesite naziv popusta: ");
        string naziv = _io.ReadLine();

        _io.Write("Unesite opis popusta: ");
        string opis = _io.ReadLine();

        _io.Write("Unesite šifru kupona: ");
        string sifra = _io.ReadLine();

        _io.WriteLine("Unesite iznos popusta (decimalni broj):");
        if(!decimal.TryParse(_io.ReadLine(), out decimal iznos))
        {
            _io.WriteLine("Neispravan unos iznosa. Operacija prekinuta.");
            return;
        }

        _io.WriteLine("Unesite datum važenja (yyyy-MM-dd):");
        if(!DateTime.TryParse(_io.ReadLine(), out DateTime datum))
        {
            _io.WriteLine("Neispravan unos datuma. Operacija prekinuta.");
            return;
        }

        var rezultat = _container.Servis.DodajFiksni(naziv, opis, sifra, iznos, datum);
        if (rezultat.IsSuccess)
            _io.WriteLine("Uspješno dodan popust sa ID: " + rezultat.Value);
        else
            _io.WriteLine("Greška: " + rezultat.Error!.Message);
    }

    public void PrikaziPopuste()
    {
        _io.WriteLine("1 - Svi popusti");
        _io.WriteLine("2 - Samo vazeci kuponi");
        string koji = _io.ReadLine();

        Result<IReadOnlyList<Zapis>> rezultat;
        if (koji == "1")
            rezultat = _container.Pregled.SviPopusti();
        else if (koji == "2")
            rezultat = _container.Pregled.VazeciPopusti();
        else
        {
            _io.WriteLine("Nepoznata opcija.");
            return;
        }

        _io.WriteLine("Format: 1 - sazeti, 2 - detaljni");
        string format = _io.ReadLine();

        IPopustFormater formater;
        if (format == "1")
            formater = new SazetiFormater();
        else if (format == "2")
            formater = new DetaljniFormater();
        else
        {
            _io.WriteLine("Nepoznat format.");
            return;
        }

        if (rezultat.IsFailure)
        {
            _io.WriteLine("Greška: " + rezultat.Error!.Message);
            return;
        }

        if (rezultat.Value!.Count == 0)
        {
            _io.WriteLine("Nema zapisa za prikaz.");
            return;
        }

        foreach (Zapis zapis in rezultat.Value)
            _io.WriteLine(formater.Formatiraj(zapis));
    }
}
