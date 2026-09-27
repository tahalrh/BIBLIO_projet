using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Biblio_DOTNET;

public class Bibliotheque
{
    private readonly List<Document> documents = new List<Document>();

    public IReadOnlyList<Document> Documents => documents.AsReadOnly();

    public void AjouterDocument(Document d)
    {
        if (d == null) throw new ArgumentNullException(nameof(d));
        documents.Add(d);
        Console.WriteLine("Document ajouté avec succès.");
    }

    public void SupprimerDocument(Guid id)
    {
        Document? doc = documents.Find(d => d.Id == id);
        if (doc == null)
        {
            throw new DocumentNonTrouveException("Impossible de supprimer : ID introuvable.");
        }
        documents.Remove(doc);
        Console.WriteLine("Document supprimé.");
    }

    public List<Document> Rechercher(string motCle)
    {
        if (string.IsNullOrWhiteSpace(motCle))
        {
            return new List<Document>(documents);
        }

        var resultats = documents.FindAll(d =>
            d.Titre.Contains(motCle, StringComparison.OrdinalIgnoreCase) ||
            d.Auteur.Contains(motCle, StringComparison.OrdinalIgnoreCase));

        if (resultats.Count == 0)
        {
            Console.WriteLine("Aucun document trouvé.");
        }
        else
        {
            foreach (var doc in resultats)
                doc.AfficherDetails();
        }

        return resultats;
    }

    public void AfficherTous()
    {
        if (documents.Count == 0)
        {
            Console.WriteLine("La bibliothèque est vide.");
        }
        else
        {
            foreach (var doc in documents)
                doc.AfficherDetails();
        }
    }

    public void Sauvegarder(string cheminFichier)
    {
        try
        {
            using FileStream fs = new FileStream(cheminFichier, FileMode.Create, FileAccess.Write);
            using StreamWriter sw = new StreamWriter(fs);

            foreach (var doc in documents)
            {
                string ligne = "";
                if (doc is Livre l)
                    ligne = $"LIVRE;{l.Id};{l.Titre};{l.Auteur};{l.Annee};{l.NombrePages}";
                else if (doc is Magazine m)
                    ligne = $"MAGAZINE;{m.Id};{m.Titre};{m.Auteur};{m.Annee};{m.Numero}";
                else if (doc is DocumentPDF p)
                    ligne = $"PDF;{p.Id};{p.Titre};{p.Auteur};{p.Annee};{p.TailleEnMo.ToString(CultureInfo.InvariantCulture)}";

                sw.WriteLine(ligne);
            }
            Console.WriteLine("Sauvegarde réussie !");
        }
        catch (IOException ex)
        {
            Console.WriteLine("Erreur d'accès au fichier : " + ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erreur inattendue lors de la sauvegarde : " + ex.Message);
            throw;
        }
    }

    public void Charger(string cheminFichier)
    {
        if (!File.Exists(cheminFichier))
        {
            Console.WriteLine("Fichier de sauvegarde introuvable.");
            return;
        }

        try
        {
            using FileStream fs = new FileStream(cheminFichier, FileMode.Open, FileAccess.Read);
            using StreamReader sr = new StreamReader(fs);

            documents.Clear();
            string? ligne;
            while ((ligne = sr.ReadLine()) != null)
            {
                string[] parts = ligne.Split(';');
                if (parts.Length < 6) continue;

                string type = parts[0];
                string id = parts[1];
                string titre = parts[2];
                string auteur = parts[3];
                int annee = int.Parse(parts[4]);
                string spec = parts[5];

                Document? doc = null;

                if (type == "LIVRE")
                    doc = new Livre(titre, auteur, annee, int.Parse(spec));
                else if (type == "MAGAZINE")
                    doc = new Magazine(titre, auteur, annee, int.Parse(spec));
                else if (type == "PDF")
                    doc = new DocumentPDF(titre, auteur, annee, double.Parse(spec, CultureInfo.InvariantCulture));

                if (doc != null)
                {
                    doc.Id = Guid.Parse(id);
                    documents.Add(doc);
                }
            }
            Console.WriteLine("Chargement terminé !");
        }
        catch (FormatException)
        {
            Console.WriteLine("Erreur : Le format du fichier est incorrect (problème de conversion de nombre).");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erreur lors du chargement : " + ex.Message);
            throw;
        }
    }
}
