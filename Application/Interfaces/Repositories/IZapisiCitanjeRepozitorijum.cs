using Domain.Models;
using Shared;

namespace Application.Interfaces.Repositories;

// Samo citanje - koristi ga pregled i pretraga (F3, F4)
public interface IZapisiCitanjeRepozitorijum
{
    Result<Zapis> Pronadji(Guid id);
    Result<IReadOnlyList<Zapis>> Svi();
}
