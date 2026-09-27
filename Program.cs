using System;
using System.Globalization;

namespace Biblio_DOTNET;

class Program
{
    static void Main(string[] args)
    {
        Bibliotheque bibliotheque = new Bibliotheque();
        string fichierSauvegarde = Environment.GetEnvironmentVariable("DATA_FILE_PATH") ?? "bibliotheque_data.txt";
        bool continuer = true;

        Console.WriteLine("==================================================");
        Console.WriteLine("  BIBLIO_DOTNET - Système de Gestion Documentaire");
        Console.WriteLine($"  Fichier de persistance : {fichierSauvegarde}");
        Console.WriteLine("==================================================");

        while (continuer)
        {
            Console.WriteLine("\n--- MENU BIBLIOTHEQUE ---");
            Console.WriteLine("1. Ajouter un document");
            Console.WriteLine("2. Afficher tous les documents");
            Console.WriteLine("3. Rechercher par mot-clé");
            Console.WriteLine("4. Supprimer un document");
            Console.WriteLine("5. Sauvegarder");
            Console.WriteLine("6. Charger");
            Console.WriteLine("7. Quitter");
            Console.Write("Votre choix : ");

            string? choix = Console.ReadLine();

            try
            {
                switch (choix)
                {
                    case "1":
                        AjouterDocumentMenu(bibliotheque);
                        break;
                    case "2":
                        bibliotheque.AfficherTous();
                        break;
                    case "3":
                        Console.Write("Entrez un mot-clé (titre ou auteur) : ");
                        string? motCle = Console.ReadLine() ?? string.Empty;
                        bibliotheque.Rechercher(motCle);
                        break;
                    case "4":
                        Console.Write("Entrez l'ID du document à supprimer : ");
                        string? idStr = Console.ReadLine();
                        if (Guid.TryParse(idStr, out Guid id))
                        {
                            bibliotheque.SupprimerDocument(id);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Format d'identifiant GUID invalide.");
                            Console.ResetColor();
                        }
                        break;
                    case "5":
                        bibliotheque.Sauvegarder(fichierSauvegarde);
                        break;
                    case "6":
                        bibliotheque.Charger(fichierSauvegarde);
                        break;
                    case "7":
                        continuer = false;
                        Console.WriteLine("Fermeture de l'application. À bientôt !");
                        break;
                    default:
                        Console.WriteLine("Choix invalide.");
                        break;
                }
            }
            catch (DocumentNonTrouveException ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Erreur logique : " + ex.Message);
                Console.ResetColor();
            }
            catch (FormatException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Erreur de format : Veuillez entrer des nombres valides.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Erreur inattendue : " + ex.Message);
                Console.ResetColor();
            }
        }
    }

    static void AjouterDocumentMenu(Bibliotheque biblio)
    {
        Console.WriteLine("Type de document ? (1: Livre, 2: Magazine, 3: PDF)");
        string? type = Console.ReadLine();

        Console.Write("Titre : ");
        string titre = Console.ReadLine() ?? "Sans titre";

        Console.Write("Auteur : ");
        string auteur = Console.ReadLine() ?? "Anonyme";

        Console.Write("Année : ");
        if (!int.TryParse(Console.ReadLine(), out int annee))
        {
            annee = DateTime.Now.Year;
        }

        if (type == "1")
        {
            Console.Write("Nombre de pages : ");
            if (int.TryParse(Console.ReadLine(), out int pages))
            {
                biblio.AjouterDocument(new Livre(titre, auteur, annee, pages));
            }
            else
            {
                Console.WriteLine("Nombre de pages invalide.");
            }
        }
        else if (type == "2")
        {
            Console.Write("Numéro du magazine : ");
            if (int.TryParse(Console.ReadLine(), out int num))
            {
                biblio.AjouterDocument(new Magazine(titre, auteur, annee, num));
            }
            else
            {
                Console.WriteLine("Numéro invalide.");
            }
        }
        else if (type == "3")
        {
            Console.Write("Taille en Mo : ");
            if (double.TryParse(Console.ReadLine(), NumberStyles.Any, CultureInfo.InvariantCulture, out double taille))
            {
                biblio.AjouterDocument(new DocumentPDF(titre, auteur, annee, taille));
            }
            else
            {
                Console.WriteLine("Taille invalide.");
            }
        }
        else
        {
            Console.WriteLine("Type inconnu.");
        }
    }
}
