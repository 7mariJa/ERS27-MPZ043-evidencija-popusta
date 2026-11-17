using Presentation.Composition;
using Presentation.Interfaces;

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
            _io.WriteLine("0 - Izlaz");
            string izbor = _io.ReadLine();

            if( izbor == "1" ) DodajProcentualni();
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
}
