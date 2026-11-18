using Application.Interfaces.Repositories;
using Database.Repositories;
using Domain.Enums;
using Domain.Models;
using NUnit.Framework;

namespace Tests.Database;

// TE1, TE8 - isti testovi ugovora se izvrsavaju nad oba repozitorijuma (LSP)
public abstract class RepozitorijumUgovorTests
{
    protected abstract IZapisiRepozitorijum NapraviRepozitorijum();

    [Test]
    public void Sacuvaj_IPronadji_ProcentualniPopust()
    {
        IZapisiRepozitorijum repo = NapraviRepozitorijum();
        var popust = new ProcentualniPopust
        {
            Naziv = "Ljeto", Opis = "Ljetna akcija", SifraKupona = "LETO26",
            DatumVazenja = new DateTime(2026, 12, 31), Procenat = 20
        };

        repo.Sacuvaj(popust);
        var rezultat = repo.Pronadji(popust.Id);

        Assert.That(rezultat.IsSuccess, Is.True);
        var procitan = rezultat.Value as ProcentualniPopust;
        Assert.That(procitan, Is.Not.Null);
        Assert.That(procitan!.Naziv, Is.EqualTo("Ljeto"));
        Assert.That(procitan.Opis, Is.EqualTo("Ljetna akcija"));
        Assert.That(procitan.SifraKupona, Is.EqualTo("LETO26"));
        Assert.That(procitan.DatumVazenja, Is.EqualTo(new DateTime(2026, 12, 31)));
        Assert.That(procitan.Procenat, Is.EqualTo(20));
        Assert.That(procitan.Status, Is.EqualTo(StatusPopusta.Neaktivan));
    }

    [Test]
    public void Sacuvaj_IPronadji_FiksniPopust()
    {
        IZapisiRepozitorijum repo = NapraviRepozitorijum();
        var popust = new FiksniPopust
        {
            Naziv = "Zima", Opis = "Zimska akcija", SifraKupona = "ZIMA26",
            DatumVazenja = new DateTime(2026, 12, 31), Iznos = 500
        };

        repo.Sacuvaj(popust);
        var rezultat = repo.Pronadji(popust.Id);

        Assert.That(rezultat.IsSuccess, Is.True);
        var procitan = rezultat.Value as FiksniPopust;
        Assert.That(procitan, Is.Not.Null);
        Assert.That(procitan!.Naziv, Is.EqualTo("Zima"));
        Assert.That(procitan.SifraKupona, Is.EqualTo("ZIMA26"));
        Assert.That(procitan.Iznos, Is.EqualTo(500));
    }

    [Test]
    public void Svi_VracaObeVrstePopusta()
    {
        IZapisiRepozitorijum repo = NapraviRepozitorijum();
        repo.Sacuvaj(new ProcentualniPopust { Naziv = "A", Opis = "A", Procenat = 10 });
        repo.Sacuvaj(new FiksniPopust { Naziv = "B", Opis = "B", Iznos = 100 });

        var svi = repo.Svi().Value!;

        Assert.That(svi.Count, Is.EqualTo(2));
        Assert.That(svi.OfType<ProcentualniPopust>().Count(), Is.EqualTo(1));
        Assert.That(svi.OfType<FiksniPopust>().Count(), Is.EqualTo(1));
    }

    [Test]
    public void Pronadji_NepostojeciId_VracaNeuspjeh()
    {
        IZapisiRepozitorijum repo = NapraviRepozitorijum();

        var rezultat = repo.Pronadji(Guid.NewGuid());

        Assert.That(rezultat.IsFailure, Is.True);
    }

    [Test]
    public void Azuriraj_NepostojeciZapis_VracaNeuspjeh()
    {
        IZapisiRepozitorijum repo = NapraviRepozitorijum();

        var rezultat = repo.Azuriraj(new FiksniPopust { Naziv = "X", Opis = "X", Iznos = 1 });

        Assert.That(rezultat.IsFailure, Is.True);
    }

    [Test]
    public void Azuriraj_CuvaNoviStatus()
    {
        IZapisiRepozitorijum repo = NapraviRepozitorijum();
        var popust = new FiksniPopust { Naziv = "X", Opis = "X", Iznos = 1 };
        repo.Sacuvaj(popust);

        popust.Status = StatusPopusta.Aktivan;
        repo.Azuriraj(popust);

        Assert.That(repo.Pronadji(popust.Id).Value!.Status, Is.EqualTo(StatusPopusta.Aktivan));
    }
}

[TestFixture]
public sealed class InMemoryRepozitorijumTests : RepozitorijumUgovorTests
{
    protected override IZapisiRepozitorijum NapraviRepozitorijum() => new InMemoryZapisiRepozitorijum();
}

[TestFixture]
public sealed class JsonRepozitorijumTests : RepozitorijumUgovorTests
{
    private string _putanja = string.Empty;

    [SetUp]
    public void SetUp()
    {
        _putanja = Path.Combine(Path.GetTempPath(), "popusti_test_" + Guid.NewGuid() + ".json");
    }

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(_putanja))
            File.Delete(_putanja);
    }

    protected override IZapisiRepozitorijum NapraviRepozitorijum() => new JsonZapisiRepozitorijum(_putanja);

    // TE5 - podaci se citaju i kad se repozitorijum ponovo otvori (kao poslije restarta aplikacije)
    [Test]
    public void PonovnoOtvaranje_PodaciSuSacuvani()
    {
        var prvi = new JsonZapisiRepozitorijum(_putanja);
        var procentualni = new ProcentualniPopust { Naziv = "A", Opis = "A", SifraKupona = "K1", Procenat = 15 };
        var fiksni = new FiksniPopust { Naziv = "B", Opis = "B", SifraKupona = "K2", Iznos = 250, Status = StatusPopusta.Aktivan };
        prvi.Sacuvaj(procentualni);
        prvi.Sacuvaj(fiksni);

        var drugi = new JsonZapisiRepozitorijum(_putanja);

        var p = drugi.Pronadji(procentualni.Id).Value as ProcentualniPopust;
        var f = drugi.Pronadji(fiksni.Id).Value as FiksniPopust;
        Assert.That(p, Is.Not.Null);
        Assert.That(p!.Procenat, Is.EqualTo(15));
        Assert.That(f, Is.Not.Null);
        Assert.That(f!.Iznos, Is.EqualTo(250));
        Assert.That(f.Status, Is.EqualTo(StatusPopusta.Aktivan));
    }
}
