using Domain.Models;
using Presentation.Interfaces;

namespace Presentation.Formateri;

// Detaljni prikaz: sve o popustu, ukljucujuci procenat ili iznos
public sealed class DetaljniFormater : IPopustFormater
{
    public string Formatiraj(Zapis zapis)
    {
        string tekst = "ID: " + zapis.Id + "\n"
            + "Naziv: " + zapis.Naziv + "\n"
            + "Opis: " + zapis.Opis + "\n"
            + "Sifra kupona: " + zapis.SifraKupona + "\n"
            + "Vazi do: " + zapis.DatumVazenja.ToString("dd.MM.yyyy") + "\n"
            + "Status: " + zapis.Status + "\n";

        if (zapis is ProcentualniPopust p)
            tekst += "Tip: Procentualni\nProcenat popusta: " + p.Procenat + "%\n";
        else if (zapis is FiksniPopust f)
            tekst += "Tip: Fiksni\nIznos popusta: " + f.Iznos + "\n";

        return tekst;
    }
}
