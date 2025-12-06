class Program
{
    static void Main(string[] args)
    {
        Bibliotheque bibliotheque = new Bibliotheque();
        string fichierSauvegarde = "bibliotheque_data.txt";
        bool continuer = true;

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

            string choix = Console.ReadLine();

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
                        bibliotheque.Rechercher(Console.ReadLine());
                        break;
                    case "4":
                        Console.Write("Entrez l'ID du document à supprimer : ");
                        string idStr = Console.ReadLine();
                        bibliotheque.SupprimerDocument(Guid.Parse(idStr));
                        break;
                    case "5":
                        bibliotheque.Sauvegarder(fichierSauvegarde);
                        break;
                    case "6":
                        bibliotheque.Charger(fichierSauvegarde);
                        break;
                    case "7":
                        continuer = false;
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
        string type = Console.ReadLine();

        Console.Write("Titre : ");
        string titre = Console.ReadLine();
        Console.Write("Auteur : ");
        string auteur = Console.ReadLine();
        Console.Write("Année : ");
        int annee = int.Parse(Console.ReadLine());

        if (type == "1")
        {
            Console.Write("Nombre de pages : ");
            int pages = int.Parse(Console.ReadLine());
            biblio.AjouterDocument(new Livre(titre, auteur, annee, pages));
        }
        else if (type == "2")
        {
            Console.Write("Numéro du magazine : ");
            int num = int.Parse(Console.ReadLine());
            biblio.AjouterDocument(new Magazine(titre, auteur, annee, num));
        }
        else if (type == "3")
        {
            Console.Write("Taille en Mo : ");
            double taille = double.Parse(Console.ReadLine());
            biblio.AjouterDocument(new DocumentPDF(titre, auteur, annee, taille));
        }
        else
        {
            Console.WriteLine("Type inconnu.");
        }
    }
}
