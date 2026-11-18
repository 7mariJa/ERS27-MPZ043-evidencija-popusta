using System.Text.Json;
using Application.Interfaces.Repositories;
using Domain.Enums;
using Domain.Models;
using Shared;

namespace Database.Repositories;

// Trajno cuvanje u JSON datoteci - podaci ostaju i poslije ponovnog pokretanja
public sealed class JsonZapisiRepozitorijum : IZapisiRepozitorijum
{
    private readonly string _putanja;

    public JsonZapisiRepozitorijum(string putanja)
    {
        _putanja = putanja;
    }

    public Result Sacuvaj(Zapis zapis)
    {
        List<Zapis> zapisi = Ucitaj();
        zapisi.Add(zapis);
        return Snimi(zapisi);
    }

    public Result Azuriraj(Zapis zapis)
    {
        List<Zapis> zapisi = Ucitaj();
        int index = zapisi.FindIndex(x => x.Id == zapis.Id);
        if (index == -1)
            return Result.Failure("nema", "Zapis nije pronadjen.");

        zapisi[index] = zapis;
        return Snimi(zapisi);
    }

    public Result<Zapis> Pronadji(Guid id)
    {
        Zapis? z = Ucitaj().FirstOrDefault(x => x.Id == id);
        if (z == null)
            return Result<Zapis>.Failure("nema", "Zapis nije pronadjen.");
        return Result<Zapis>.Success(z);
    }

    public Result<IReadOnlyList<Zapis>> Svi()
    {
        return Result<IReadOnlyList<Zapis>>.Success(Ucitaj());
    }

    public Result Obrisi(Guid id)
    {
        List<Zapis> zapisi = Ucitaj();
        zapisi.RemoveAll(x => x.Id == id);
        return Snimi(zapisi);
    }

    // Cita sve zapise iz datoteke (ako datoteka ne postoji - prazna lista)
    private List<Zapis> Ucitaj()
    {
        if (!File.Exists(_putanja))
            return new List<Zapis>();

        string json = File.ReadAllText(_putanja);
        List<ZapisJson>? podaci = JsonSerializer.Deserialize<List<ZapisJson>>(json);
        if (podaci == null)
            return new List<Zapis>();

        return podaci.Select(UZapis).ToList();
    }

    // Upisuje sve zapise u datoteku
    private Result Snimi(List<Zapis> zapisi)
    {
        try
        {
            List<ZapisJson> podaci = zapisi.Select(UJson).ToList();
            string json = JsonSerializer.Serialize(podaci, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_putanja, json);
            return Result.Success();
        }
        catch (IOException)
        {
            return Result.Failure("json.greska", "Greska pri upisu u JSON datoteku.");
        }
    }

    // Postoje dva tipa popusta, pa se u datoteku upisuje i "Tip"
    // da bi se pri citanju znalo koji objekat treba napraviti
    private static ZapisJson UJson(Zapis z)
    {
        var podatak = new ZapisJson
        {
            Id = z.Id,
            Naziv = z.Naziv,
            Opis = z.Opis,
            KreiranoUtc = z.KreiranoUtc,
            Status = z.Status,
            SifraKupona = z.SifraKupona,
            DatumVazenja = z.DatumVazenja
        };

        if (z is ProcentualniPopust p)
        {
            podatak.Tip = "Procentualni";
            podatak.Procenat = p.Procenat;
        }
        else if (z is FiksniPopust f)
        {
            podatak.Tip = "Fiksni";
            podatak.Iznos = f.Iznos;
        }

        return podatak;
    }

    private static Zapis UZapis(ZapisJson j)
    {
        if (j.Tip == "Procentualni")
        {
            return new ProcentualniPopust
            {
                Id = j.Id,
                Naziv = j.Naziv,
                Opis = j.Opis,
                KreiranoUtc = j.KreiranoUtc,
                Status = j.Status,
                SifraKupona = j.SifraKupona,
                DatumVazenja = j.DatumVazenja,
                Procenat = j.Procenat
            };
        }

        return new FiksniPopust
        {
            Id = j.Id,
            Naziv = j.Naziv,
            Opis = j.Opis,
            KreiranoUtc = j.KreiranoUtc,
            Status = j.Status,
            SifraKupona = j.SifraKupona,
            DatumVazenja = j.DatumVazenja,
            Iznos = j.Iznos
        };
    }

    // Oblik jednog zapisa u JSON datoteci
    private sealed class ZapisJson
    {
        public string Tip { get; set; } = string.Empty;
        public Guid Id { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public string Opis { get; set; } = string.Empty;
        public DateTimeOffset KreiranoUtc { get; set; }
        public StatusPopusta Status { get; set; }
        public string SifraKupona { get; set; } = string.Empty;
        public DateTime DatumVazenja { get; set; }
        public decimal Procenat { get; set; }
        public decimal Iznos { get; set; }
    }
}
