using Domain.Models;
using Presentation.Interfaces;

namespace Presentation.Formateri;

// Sazeti prikaz: tip, ID, naziv i status
public sealed class SazetiFormater : IPopustFormater
{
    public string Formatiraj(Zapis zapis)
    {
        string tip = zapis is ProcentualniPopust ? "Procentualni" : "Fiksni";
        return tip + " | " + zapis.Id + " | " + zapis.Naziv + " | " + zapis.Status;
    }
}
