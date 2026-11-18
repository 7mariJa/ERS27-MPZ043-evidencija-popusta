using Domain.Enums;
using Domain.Models;
using Domain.Pravila;
using NUnit.Framework;

namespace Tests.Domain;

// TE2 - pravilo P3, TE4 - dozvoljene i nedozvoljene promjene stanja
[TestFixture]
public sealed class PravilaPopustaTests
{
    [TestCase(1)]
    [TestCase(50)]
    [TestCase(90)]
    public void Procentualni_IspravanProcenat_Prolazi(decimal procenat)
    {
        var popust = new ProcentualniPopust { Naziv = "Ljeto", Opis = "Akcija", Procenat = procenat };
        Assert.That(ValidacijaPopusta.Provjeri(popust), Is.Null);
    }

    [TestCase(0)]
    [TestCase(0.5)]
    [TestCase(91)]
    [TestCase(-10)]
    public void Procentualni_ProcenatVanOpsega_Odbija(decimal procenat)
    {
        var popust = new ProcentualniPopust { Naziv = "Ljeto", Opis = "Akcija", Procenat = procenat };
        Assert.That(ValidacijaPopusta.Provjeri(popust), Is.Not.Null);
    }

    [TestCase(0)]
    [TestCase(-100)]
    public void Fiksni_IznosNijePozitivan_Odbija(decimal iznos)
    {
        var popust = new FiksniPopust { Naziv = "Zima", Opis = "Akcija", Iznos = iznos };
        Assert.That(ValidacijaPopusta.Provjeri(popust), Is.Not.Null);
    }

    [Test]
    public void Fiksni_PozitivanIznos_Prolazi()
    {
        var popust = new FiksniPopust { Naziv = "Zima", Opis = "Akcija", Iznos = 500 };
        Assert.That(ValidacijaPopusta.Provjeri(popust), Is.Null);
    }

    [Test]
    public void PrazanNaziv_Odbija()
    {
        var popust = new ProcentualniPopust { Naziv = "  ", Opis = "Akcija", Procenat = 10 };
        Assert.That(ValidacijaPopusta.Provjeri(popust), Is.Not.Null);
    }

    [Test]
    public void Aktiviraj_DatumNijeProsao_PostajeAktivan()
    {
        var popust = new FiksniPopust { DatumVazenja = DateTime.Today.AddDays(5) };

        string? greska = popust.Aktiviraj();

        Assert.That(greska, Is.Null);
        Assert.That(popust.Status, Is.EqualTo(StatusPopusta.Aktivan));
    }

    [Test]
    public void Aktiviraj_DatumProsao_OdbijaIStatusOstaje()
    {
        var popust = new FiksniPopust { DatumVazenja = DateTime.Today.AddDays(-1) };

        string? greska = popust.Aktiviraj();

        Assert.That(greska, Is.Not.Null);
        Assert.That(popust.Status, Is.EqualTo(StatusPopusta.Neaktivan));
    }

    [Test]
    public void Deaktiviraj_BioAktivan_PostajeNeaktivan()
    {
        var popust = new FiksniPopust { Status = StatusPopusta.Aktivan };

        string? greska = popust.Deaktiviraj();

        Assert.That(greska, Is.Null);
        Assert.That(popust.Status, Is.EqualTo(StatusPopusta.Neaktivan));
    }

    [Test]
    public void Deaktiviraj_NijeBioAktivan_OdbijaIStatusOstaje()
    {
        var popust = new FiksniPopust { Status = StatusPopusta.Neaktivan };

        string? greska = popust.Deaktiviraj();

        Assert.That(greska, Is.Not.Null);
        Assert.That(popust.Status, Is.EqualTo(StatusPopusta.Neaktivan));
    }
}
