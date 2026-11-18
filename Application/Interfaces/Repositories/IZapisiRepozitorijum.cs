namespace Application.Interfaces.Repositories;

// Spaja interfejs za citanje i interfejs za upis (jedna klasa moze implementirati oba)
public interface IZapisiRepozitorijum : IZapisiCitanjeRepozitorijum, IZapisiUpisRepozitorijum
{
}
