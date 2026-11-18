using Application.Interfaces.Repositories;
using Domain.Models;
using Shared;

namespace Application.Services;

// F3 - pregled popusta, zavisi samo od interfejsa za citanje
public sealed class PregledServis
{
    private readonly IZapisiCitanjeRepozitorijum _repozitorijum;

    public PregledServis(IZapisiCitanjeRepozitorijum repozitorijum)
    {
        _repozitorijum = repozitorijum;
    }

    public Result<IReadOnlyList<Zapis>> SviPopusti()
    {
        return _repozitorijum.Svi();
    }

    public Result<IReadOnlyList<Zapis>> VazeciPopusti()
    {
        var svi = _repozitorijum.Svi();
        if (svi.IsFailure)
            return svi;

        List<Zapis> vazeci = svi.Value!.Where(z => z.DatumVazenja.Date >= DateTime.Today).ToList();
        return Result<IReadOnlyList<Zapis>>.Success(vazeci);
    }
}
