using Domain.Enums;

namespace Domain.Models;

// Zajednički osnovni tip iz koga se mogu izvesti modeli konkretnog zadatka.
// Polja, nazive i nasledjivanje prilagoditi dodeljenoj specifikaciji.
public abstract class Zapis
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Naziv { get; init; } = string.Empty;
    public string Opis { get; init; } = string.Empty;
    public DateTimeOffset KreiranoUtc { get; init; } = DateTimeOffset.UtcNow;

    public StatusPopusta Status { get; set; } = StatusPopusta.Neaktivan;
    public string SifraKupona { get; set; } = string.Empty;
    public DateTime DatumVazenja { get; set; }

    // F5 - pravilo: popust se moze aktivirati samo ako datum vazenja nije prosao
    public string? Aktiviraj()
    {
        if (DatumVazenja.Date < DateTime.Today)
            return "Popust se ne moze aktivirati jer je datum vazenja prosao.";

        Status = StatusPopusta.Aktivan;
        return null;
    }
}
