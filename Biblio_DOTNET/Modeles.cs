using System;
using System.Collections.Generic;
using System.IO;

public abstract class Document
{
    public Guid Id { get; set; }
    public string Titre { get; set; }
    public string Auteur { get; set; }
    public int Annee { get; set; }

    public Document(string titre, string auteur, int annee)
    {
        Id = Guid.NewGuid(); 
        Titre = titre;
        Auteur = auteur;
        Annee = annee;
    }
    public abstract void AfficherDetails();
}

public class Livre : Document
{
    public int NombrePages { get; set; }

    public Livre(string titre, string auteur, int annee, int nombrePages)
        : base(titre, auteur, annee)
    {
        NombrePages = nombrePages;
    }

    public override void AfficherDetails()
    {
        Console.WriteLine($"[LIVRE] {Titre} - {Auteur} ({Annee}) - {NombrePages} pages (ID: {Id})");
    }
}

public class Magazine : Document
{
    public int Numero { get; set; }

    public Magazine(string titre, string auteur, int annee, int numero)
        : base(titre, auteur, annee)
    {
        Numero = numero;
    }

    public override void AfficherDetails()
    {
        Console.WriteLine($"[MAGAZINE] {Titre} - {Auteur} ({Annee}) - N°{Numero} (ID: {Id})");
    }
}

public class DocumentPDF : Document
{
    public double TailleEnMo { get; set; }

    public DocumentPDF(string titre, string auteur, int annee, double taille)
        : base(titre, auteur, annee)
    {
        TailleEnMo = taille;
    }

    public override void AfficherDetails()
    {
        Console.WriteLine($"[PDF] {Titre} - {Auteur} ({Annee}) - {TailleEnMo} Mo (ID: {Id})");
    }
}
public class DocumentNonTrouveException : Exception
{
    public DocumentNonTrouveException(string message) : base(message) { }
}