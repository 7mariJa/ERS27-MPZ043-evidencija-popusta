using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services;
using Domain.Enums;
using Domain.Models;
using Moq;
using NUnit.Framework;
using Shared;

namespace Tests.Application;

// TE6, TE9 - servis se testira uz lazni (mock) repozitorijum i logger, bez datoteke
[TestFixture]
public sealed class ZapisServisTests
{
    private Mock<IZapisiRepozitorijum> _repo = null!;
    private Mock<ILoggerServis> _logger = null!;
    private ZapisServis _servis = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IZapisiRepozitorijum>();
        _repo.Setup(r => r.Sacuvaj(It.IsAny<Zapis>())).Returns(Result.Success());
        _repo.Setup(r => r.Azuriraj(It.IsAny<Zapis>())).Returns(Result.Success());
        _logger = new Mock<ILoggerServis>();
        _logger.Setup(l => l.Loguj(It.IsAny<string>())).Returns(true);
        _servis = new ZapisServis(_repo.Object, _logger.Object);
    }

    [Test]
    public void DodajProcentualni_IspravanUnos_CuvaIVracaId()
    {
        var rezultat = _servis.DodajProcentualni("Ljeto", "Akcija", "LETO", 20, DateTime.Today.AddDays(10));

        Assert.That(rezultat.IsSuccess, Is.True);
        Assert.That(rezultat.Value, Is.Not.EqualTo(Guid.Empty));
        _repo.Verify(r => r.Sacuvaj(It.Is<ProcentualniPopust>(p => p.Procenat == 20)), Times.Once);
    }

    // TE2 - neispravan unos se ne cuva
    [Test]
    public void DodajProcentualni_Procenat91_NeCuva()
    {
        var rezultat = _servis.DodajProcentualni("Ljeto", "Akcija", "LETO", 91, DateTime.Today.AddDays(10));

        Assert.That(rezultat.IsFailure, Is.True);
        _repo.Verify(r => r.Sacuvaj(It.IsAny<Zapis>()), Times.Never);
    }

    [Test]
    public void DodajFiksni_NegativanIznos_NeCuva()
    {
        var rezultat = _servis.DodajFiksni("Zima", "Akcija", "ZIMA", -5, DateTime.Today.AddDays(10));

        Assert.That(rezultat.IsFailure, Is.True);
        _repo.Verify(r => r.Sacuvaj(It.IsAny<Zapis>()), Times.Never);
    }

    [Test]
    public void DodajFiksni_IspravanUnos_CuvaIVracaId()
    {
        var rezultat = _servis.DodajFiksni("Zima", "Akcija", "ZIMA", 500, DateTime.Today.AddDays(10));

        Assert.That(rezultat.IsSuccess, Is.True);
        _repo.Verify(r => r.Sacuvaj(It.Is<FiksniPopust>(p => p.Iznos == 500)), Times.Once);
    }

    // TE4 - F5
    [Test]
    public void Aktiviraj_NepostojeciId_VracaNeuspjeh()
    {
        _repo.Setup(r => r.Pronadji(It.IsAny<Guid>()))
            .Returns(Result<Zapis>.Failure("nema", "Zapis nije pronadjen."));

        var rezultat = _servis.Aktiviraj(Guid.NewGuid());

        Assert.That(rezultat.IsFailure, Is.True);
        _repo.Verify(r => r.Azuriraj(It.IsAny<Zapis>()), Times.Never);
    }

    [Test]
    public void Aktiviraj_VazeciPopust_AktiviraIAzurira()
    {
        var popust = new FiksniPopust { DatumVazenja = DateTime.Today.AddDays(3) };
        _repo.Setup(r => r.Pronadji(popust.Id)).Returns(Result<Zapis>.Success(popust));

        var rezultat = _servis.Aktiviraj(popust.Id);

        Assert.That(rezultat.IsSuccess, Is.True);
        Assert.That(popust.Status, Is.EqualTo(StatusPopusta.Aktivan));
        _repo.Verify(r => r.Azuriraj(popust), Times.Once);
    }

    [Test]
    public void Aktiviraj_IstekaoPopust_OdbijaBezPromjene()
    {
        var popust = new FiksniPopust { DatumVazenja = DateTime.Today.AddDays(-3) };
        _repo.Setup(r => r.Pronadji(popust.Id)).Returns(Result<Zapis>.Success(popust));

        var rezultat = _servis.Aktiviraj(popust.Id);

        Assert.That(rezultat.IsFailure, Is.True);
        Assert.That(popust.Status, Is.EqualTo(StatusPopusta.Neaktivan));
        _repo.Verify(r => r.Azuriraj(It.IsAny<Zapis>()), Times.Never);
    }

    // TE4 - F6
    [Test]
    public void Deaktiviraj_AktivanPopust_Deaktivira()
    {
        var popust = new FiksniPopust { Status = StatusPopusta.Aktivan };
        _repo.Setup(r => r.Pronadji(popust.Id)).Returns(Result<Zapis>.Success(popust));

        var rezultat = _servis.Deaktiviraj(popust.Id);

        Assert.That(rezultat.IsSuccess, Is.True);
        Assert.That(popust.Status, Is.EqualTo(StatusPopusta.Neaktivan));
    }

    [Test]
    public void Deaktiviraj_NeaktivanPopust_OdbijaBezPromjene()
    {
        var popust = new FiksniPopust { Status = StatusPopusta.Neaktivan };
        _repo.Setup(r => r.Pronadji(popust.Id)).Returns(Result<Zapis>.Success(popust));

        var rezultat = _servis.Deaktiviraj(popust.Id);

        Assert.That(rezultat.IsFailure, Is.True);
        _repo.Verify(r => r.Azuriraj(It.IsAny<Zapis>()), Times.Never);
    }

    [Test]
    public void NeuspjesnoDodavanje_SeLoguje()
    {
        _servis.DodajProcentualni("", "", "X", 10, DateTime.Today);

        _logger.Verify(l => l.Loguj(It.IsAny<string>()), Times.Once);
    }
}
