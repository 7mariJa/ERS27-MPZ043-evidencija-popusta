using Application.Interfaces.Repositories;
using Application.Services;
using Domain.Models;
using Moq;
using NUnit.Framework;
using Shared;

namespace Tests.Application;

// TE3 - pretraga F4, TE9 - komponenta za F3-F4 zavisi samo od interfejsa za citanje
[TestFixture]
public sealed class PregledServisTests
{
    private PregledServis NapraviServis(params Zapis[] zapisi)
    {
        var repo = new Mock<IZapisiCitanjeRepozitorijum>();
        repo.Setup(r => r.Svi()).Returns(Result<IReadOnlyList<Zapis>>.Success(zapisi.ToList()));
        return new PregledServis(repo.Object);
    }

    [Test]
    public void Pretraga_NeRazlikujeVelikaIMalaSlova()
    {
        var servis = NapraviServis(
            new ProcentualniPopust { Naziv = "Ljeto", SifraKupona = "LETO26" },
            new FiksniPopust { Naziv = "Zima", SifraKupona = "ZIMA26" });

        var rezultat = servis.PretraziPoSifri("leto26");

        Assert.That(rezultat.IsSuccess, Is.True);
        Assert.That(rezultat.Value!.Count, Is.EqualTo(1));
        Assert.That(rezultat.Value[0].Naziv, Is.EqualTo("Ljeto"));
    }

    [Test]
    public void Pretraga_NemaRezultata_VracaPraznuListu()
    {
        var servis = NapraviServis(new FiksniPopust { SifraKupona = "ZIMA26" });

        var rezultat = servis.PretraziPoSifri("PROLJECE");

        Assert.That(rezultat.IsSuccess, Is.True);
        Assert.That(rezultat.Value, Is.Empty);
    }

    [Test]
    public void Pretraga_PraznaSifra_VracaNeuspjeh()
    {
        var servis = NapraviServis();

        Assert.That(servis.PretraziPoSifri("  ").IsFailure, Is.True);
    }

    [Test]
    public void VazeciPopusti_IzbacujeIstekle()
    {
        var servis = NapraviServis(
            new FiksniPopust { Naziv = "Vazi", DatumVazenja = DateTime.Today.AddDays(1) },
            new FiksniPopust { Naziv = "Istekao", DatumVazenja = DateTime.Today.AddDays(-1) });

        var rezultat = servis.VazeciPopusti();

        Assert.That(rezultat.Value!.Count, Is.EqualTo(1));
        Assert.That(rezultat.Value[0].Naziv, Is.EqualTo("Vazi"));
    }

    [Test]
    public void PregledServis_ZavisiSamoOdInterfejsaZaCitanje()
    {
        Type[] parametri = typeof(PregledServis).GetConstructors()
            .SelectMany(c => c.GetParameters())
            .Select(p => p.ParameterType)
            .ToArray();

        Assert.That(parametri, Does.Contain(typeof(IZapisiCitanjeRepozitorijum)));
        Assert.That(parametri, Does.Not.Contain(typeof(IZapisiUpisRepozitorijum)));
        Assert.That(parametri, Does.Not.Contain(typeof(IZapisiRepozitorijum)));
    }
}
