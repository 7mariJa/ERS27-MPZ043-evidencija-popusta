using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Models;
using Shared;

namespace Database.Repositories;

// TODO: implementirati skladištenje u memoriji.
public sealed class InMemoryZapisiRepozitorijum : IZapisiRepozitorijum
{
    private readonly List<Zapis> _zapisi = new();   //prazna lista, u njoj ce se cuvati svi popusti

    public Result Sacuvaj(Zapis zapis)          //dodavanje popusta u listu
    {
        _zapisi.Add(zapis);
        return Result.Success();                //vraca obican result bez <Guid>, jer ovdje nema sta drugo da se vrati osim uspjeha ili neuspjeha
    }

    public Result<Zapis> Pronadji(Guid id)      //trazi popust po id-u
    {
        Zapis? z = _zapisi.FirstOrDefault(x => x.Id == id); //lambda izraz (za svako x provjeri da li je njegov Id = trazenom Id)
        if (z == null)
            return Result<Zapis>.Failure("nema", "Zapis nije pronadjen.");
        return Result<Zapis>.Success(z);
    }

    public Result<IReadOnlyList<Zapis>> Svi()
    {
        return Result<IReadOnlyList<Zapis>>.Success(_zapisi);
    }


    public Result Obrisi(Guid id)
    {
        _zapisi.RemoveAll(x => x.Id == id);
        return Result.Success();
    }
}
