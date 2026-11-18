using Domain.Enums;
using Domain.Models;
using NUnit.Framework;
using Presentation.Formateri;
using Presentation.Interfaces;

namespace Tests.Presentation;

// TE7 - oba formata prikaza F3 i dodavanje treceg formatera bez izmjene servisa (OCP)
[TestFixture]
public sealed class FormateriTests
{
    [Test]
    public void SazetiFormater_PrikazujeTipIdNazivIStatus()
    {
        var popust = new ProcentualniPopust { Naziv = "Ljeto", Status = StatusPopusta.Aktivan, Procenat = 20 };

        string tekst = new SazetiFormater().Formatiraj(popust);

        Assert.That(tekst, Does.Contain("Procentualni"));
        Assert.That(tekst, Does.Contain(popust.Id.ToString()));
        Assert.That(tekst, Does.Contain("Ljeto"));
        Assert.That(tekst, Does.Contain("Aktivan"));
    }

    [Test]
    public void DetaljniFormater_PrikazujeProcenat()
    {
        var popust = new ProcentualniPopust { Naziv = "Ljeto", SifraKupona = "LETO26", Procenat = 20 };

        string tekst = new DetaljniFormater().Formatiraj(popust);

        Assert.That(tekst, Does.Contain("Procenat popusta: 20"));
        Assert.That(tekst, Does.Contain("LETO26"));
    }

    [Test]
    public void DetaljniFormater_PrikazujeIznos()
    {
        var popust = new FiksniPopust { Naziv = "Zima", Iznos = 500 };

        string tekst = new DetaljniFormater().Formatiraj(popust);

        Assert.That(tekst, Does.Contain("Iznos popusta: 500"));
    }

    // Treci format napravljen samo u testu - nijedna klasa u Domain/Application nije mijenjana
    private sealed class SamoNazivFormater : IPopustFormater
    {
        public string Formatiraj(Zapis zapis) => zapis.Naziv;
    }

    [Test]
    public void TreciFormater_RadiBezIzmjeneServisa()
    {
        IPopustFormater[] formateri = { new SazetiFormater(), new DetaljniFormater(), new SamoNazivFormater() };
        var popust = new FiksniPopust { Naziv = "Zima", Iznos = 100 };

        foreach (IPopustFormater formater in formateri)
            Assert.That(formater.Formatiraj(popust), Does.Contain("Zima"));
    }
}
