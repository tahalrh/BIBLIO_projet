using System;
using System.IO;
using System.Linq;
using Biblio_DOTNET;
using Xunit;

namespace Biblio_DOTNET.Tests;

public class BibliothequeTests : IDisposable
{
    private readonly Bibliotheque _biblio;
    private readonly string _tempFile;

    public BibliothequeTests()
    {
        _biblio = new Bibliotheque();
        _tempFile = Path.Combine(Path.GetTempPath(), $"test_biblio_{Guid.NewGuid():N}.txt");
    }

    public void Dispose()
    {
        if (File.Exists(_tempFile))
        {
            File.Delete(_tempFile);
        }
    }

    [Fact]
    public void AjouterDocument_DevraitAjouterDocumentCorrectement()
    {
        // Arrange
        var livre = new Livre("Clean Code", "Robert C. Martin", 2008, 464);

        // Act
        _biblio.AjouterDocument(livre);

        // Assert
        Assert.Single(_biblio.Documents);
        Assert.Equal("Clean Code", _biblio.Documents[0].Titre);
        Assert.Equal("Robert C. Martin", _biblio.Documents[0].Auteur);
        Assert.Equal(2008, _biblio.Documents[0].Annee);
    }

    [Fact]
    public void AjouterDocument_Null_DevraitLeverArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _biblio.AjouterDocument(null!));
    }

    [Fact]
    public void SupprimerDocument_IdExistant_DevraitRetirerDocument()
    {
        // Arrange
        var livre = new Livre("The Pragmatic Programmer", "Andy Hunt", 1999, 352);
        _biblio.AjouterDocument(livre);
        var id = livre.Id;

        // Act
        _biblio.SupprimerDocument(id);

        // Assert
        Assert.Empty(_biblio.Documents);
    }

    [Fact]
    public void SupprimerDocument_IdInexistant_DevraitLeverDocumentNonTrouveException()
    {
        // Arrange
        var fauxId = Guid.NewGuid();

        // Act & Assert
        var ex = Assert.Throws<DocumentNonTrouveException>(() => _biblio.SupprimerDocument(fauxId));
        Assert.Contains("ID introuvable", ex.Message);
    }

    [Fact]
    public void Rechercher_ParTitreOuAuteur_DevraitRetournerResultats()
    {
        // Arrange
        _biblio.AjouterDocument(new Livre("Design Patterns", "Gang of Four", 1994, 395));
        _biblio.AjouterDocument(new Magazine("Programmez!", "Collectif", 2024, 260));
        _biblio.AjouterDocument(new DocumentPDF("Guide Architecture C#", "Microsoft", 2023, 14.5));

        // Act
        var resultatsTitre = _biblio.Rechercher("Patterns");
        var resultatsAuteur = _biblio.Rechercher("Microsoft");
        var resultatsInexistant = _biblio.Rechercher("Introuvable");

        // Assert
        Assert.Single(resultatsTitre);
        Assert.Equal("Design Patterns", resultatsTitre[0].Titre);

        Assert.Single(resultatsAuteur);
        Assert.Equal("Microsoft", resultatsAuteur[0].Auteur);

        Assert.Empty(resultatsInexistant);
    }

    [Fact]
    public void SauvegarderEtCharger_DevraitPreserverTousLesTypesDeDocuments()
    {
        // Arrange
        var livre = new Livre("Refactoring", "Martin Fowler", 1999, 431);
        var magazine = new Magazine(".NET Magazine", "Tech Press", 2023, 42);
        var pdf = new DocumentPDF("Spécification C# 12", "Microsoft Team", 2023, 8.75);

        _biblio.AjouterDocument(livre);
        _biblio.AjouterDocument(magazine);
        _biblio.AjouterDocument(pdf);

        // Act
        _biblio.Sauvegarder(_tempFile);

        var nouvelleBiblio = new Bibliotheque();
        nouvelleBiblio.Charger(_tempFile);

        // Assert
        Assert.Equal(3, nouvelleBiblio.Documents.Count);

        var livreCharge = nouvelleBiblio.Documents.OfType<Livre>().FirstOrDefault();
        Assert.NotNull(livreCharge);
        Assert.Equal(livre.Id, livreCharge.Id);
        Assert.Equal("Refactoring", livreCharge.Titre);
        Assert.Equal(431, livreCharge.NombrePages);

        var magCharge = nouvelleBiblio.Documents.OfType<Magazine>().FirstOrDefault();
        Assert.NotNull(magCharge);
        Assert.Equal(magazine.Id, magCharge.Id);
        Assert.Equal(42, magCharge.Numero);

        var pdfCharge = nouvelleBiblio.Documents.OfType<DocumentPDF>().FirstOrDefault();
        Assert.NotNull(pdfCharge);
        Assert.Equal(pdf.Id, pdfCharge.Id);
        Assert.Equal(8.75, pdfCharge.TailleEnMo);
    }
}
