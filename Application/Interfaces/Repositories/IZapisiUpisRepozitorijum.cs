using Domain.Models;
using Shared;

namespace Application.Interfaces.Repositories;

// Upis i izmjena
public interface IZapisiUpisRepozitorijum
{
    Result Sacuvaj(Zapis zapis);
    Result Azuriraj(Zapis zapis);
    Result Obrisi(Guid id);
}
