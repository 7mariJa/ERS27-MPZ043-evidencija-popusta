using Domain.Models;

namespace Presentation.Interfaces;

// Zajednicki ugovor za prikaz popusta (novi format = nova klasa)
public interface IPopustFormater
{
    string Formatiraj(Zapis zapis);
}
