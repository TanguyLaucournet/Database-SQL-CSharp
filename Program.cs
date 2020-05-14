using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace PROJETBDD
{
    class Program
    {
        static void Main(string[] args)
        {
            string cs = @"server=localhost;userid=root;password=password;database=Mydb";

            var con = new MySqlConnection(cs);
            con.Open();

            Console.WriteLine("Souhaitez vous: ");
            Console.WriteLine(" 1 - Créer un compte");
            Console.WriteLine(" 2 - Se connecter");
            Console.WriteLine(" 3 - Acces Admin ");
            Console.WriteLine(" 4 - Demo ");


            int choix = Convert.ToInt32(Console.ReadLine());
            if (choix == 1)
            {
                bool new_email = true;

                do
                {


                    Console.WriteLine("Adresse email: ");
                    string email = Console.ReadLine();
                    Console.WriteLine("Nom :");
                    string nom = Console.ReadLine();
                    Console.WriteLine("Mot de passe: ");
                    string mdp = Console.ReadLine();
                    Console.WriteLine("Téléphone: ");
                    string tel = Console.ReadLine();
                    Console.WriteLine("Adresse: ");
                    string adresse = Console.ReadLine();

                    Console.WriteLine("Createur de recette: 1 si oui 0 si non ");
                    int creadr = Convert.ToInt32(Console.ReadLine());
                    bool cdr = false;
                    if (creadr == 1) { cdr = true; }

                    string sql = "SELECT email FROM client";
                    MySqlCommand cmd = new MySqlCommand(sql, con);
                    MySqlDataReader info = cmd.ExecuteReader();
                    while (info.Read())
                    {
                        if (info.GetString(0) == email)
                        {
                            new_email = false;
                            Console.WriteLine("Adresse email déjà utilisée veuillez recommencer l'inscription: ");
                        }
                    }
                    info.Close();

                    if (new_email)
                    {

                        string new_client = "INSERT INTO client(name_client, tel_client, email, pwd, adresse,cdr) VALUES(@nom, @tel, @email, @mdp, @adresse,@cdr)";

                        MySqlCommand insert = new MySqlCommand(new_client, con);
                        insert.Parameters.AddWithValue("@nom", nom);
                        insert.Parameters.AddWithValue("@tel", tel);
                        insert.Parameters.AddWithValue("@email", email); // Cree un nouveau client
                        insert.Parameters.AddWithValue("@mdp", mdp);
                        insert.Parameters.AddWithValue("@adresse", adresse);
                        insert.Parameters.AddWithValue("@cdr", cdr);

                        insert.ExecuteNonQuery();
                    }


                } while (!new_email);

            }
            if (choix == 2)
            {
                int id_client = 0;
                bool connection_success = true;
                bool good_email = false;
                bool cdr = false;
                do  // Vérifie l'email et le mot de passe de la personne souhaitant se connecter
                {
                    string email;
                    connection_success = true;
                    good_email = false;
                    Console.WriteLine("Adresse email: ");
                    email = Console.ReadLine();

                    Console.WriteLine("Mot de passe: ");
                    string mdp = Console.ReadLine();
                    string sql = "SELECT email FROM client";
                    MySqlCommand cmd = new MySqlCommand(sql, con);
                    MySqlDataReader info = cmd.ExecuteReader();
                    while (info.Read())
                    {

                        if (info.GetString(0) == email)

                        { good_email = true; }
                    }
                    info.Close();

                    string sql2 = "SELECT * FROM client WHERE email LIKE @email";
                    MySqlCommand cmd2 = new MySqlCommand(sql2, con);
                    cmd2.Parameters.AddWithValue("@email", email);
                    MySqlDataReader info2 = cmd2.ExecuteReader();

                    while (info2.Read())
                    {
                        cdr = info2.GetBoolean(6);
                        id_client = info2.GetInt32(0);
                        if (info2.GetString(4) != mdp)
                        {
                            connection_success = false;
                            Console.WriteLine("Mauvaise combinaison email/mot de passe veuillez recommencer");
                        }


                    }
                    info2.Close();
                    if (!good_email) { Console.WriteLine("Mauvaise adresse email veuillez recommencer "); }
                } while (!connection_success || !good_email);

                Console.WriteLine("Connexion Réussie");
                Console.ReadKey();
                if (cdr)
                {
                    int choix2;
                    do
                    {
                        Console.WriteLine("Souhaitez vous: 1 - Créer une recette, 2 - Commander, 3 - Consulter votre solde cook, 4 - Consulter vos recettes");
                        choix2 = Convert.ToInt32(Console.ReadLine());

                    } while (choix2 != 1 && choix2 != 2 && choix2 != 3 && choix2 != 4);
                    if (choix2 == 1)
                    {
                        Console.WriteLine("Nom de la recette: ");
                        string nom = Console.ReadLine();
                        Console.WriteLine("Type de  recette: ");
                        string typ = Console.ReadLine();
                        Console.WriteLine("Descritpif de la recette: ");
                        string desc = Console.ReadLine();
                        Console.WriteLine("Prix de la recette: ");
                        string prix = Console.ReadLine();
                        bool end = false;
                        List<int> product = new List<int>();
                        List<int> quantity = new List<int>();
                        do
                        {
                            string sql = "SELECT id_produit,nom_produit,unite FROM produit";
                            MySqlCommand cmd = new MySqlCommand(sql, con);
                            MySqlDataReader info = cmd.ExecuteReader();
                            Console.WriteLine("0 - Finnaliser la recette ");
                            while (info.Read())
                            {
                                Console.Write(info.GetString(0) + " - "); // Permet de visualiser les produits disponibles
                                Console.Write(info.GetString(1) + " - ");
                                Console.WriteLine(info.GetString(2));
                            }
                            Console.WriteLine("Sélectionnez l'id du produit que vous souhaitez ajouter à votre recette :");
                            int id = Convert.ToInt32(Console.ReadLine());
                            int qty = 0;
                            if (id != 0)
                            {
                                Console.WriteLine("Quelle quantité de ce produit est nécéssaire pour votre recette votre recette :");
                                qty = Convert.ToInt32(Console.ReadLine());
                            }
                            if (id == 0) { end = true; }
                            info.Close();
                            product.Add(id);
                            quantity.Add(qty);
                        } while (!end);
                        DateTime aDate = DateTime.Now;



                        string new_plat = "INSERT INTO plat(nom_plat,date_creation,type,description,prix_vente,id_client) VALUES(@nom,@date, @typ, @desc, @prix, @id_client)";

                        MySqlCommand insert = new MySqlCommand(new_plat, con);
                        insert.Parameters.AddWithValue("@nom", nom);
                        insert.Parameters.AddWithValue("@date", aDate);
                        insert.Parameters.AddWithValue("@typ", typ);
                        insert.Parameters.AddWithValue("@desc", desc);      //Creer une nouvelle recette dans la table plat
                        insert.Parameters.AddWithValue("@prix", prix);
                        insert.Parameters.AddWithValue("@id_client", id_client);
                        insert.ExecuteNonQuery();

                        string lastid = "SELECT MAX(id_plat) FROM plat";
                        MySqlCommand cmdid = new MySqlCommand(lastid, con);
                        MySqlDataReader infoid = cmdid.ExecuteReader();
                        int id_plat = 0;
                        while (infoid.Read())
                        {
                            id_plat = infoid.GetInt16(0);
                        }
                        infoid.Close();

                        string new_plat_produit = "INSERT INTO plat_produit(id_plat,id_produit,quantity_produit) VALUES(@id_plat, @id_produit,@qty)";
                        for (int j = 0; j < (product.Count) - 1; j++)
                        {
                            MySqlCommand insert_product = new MySqlCommand(new_plat_produit, con);
                            insert_product.Parameters.AddWithValue("@id_plat", id_plat);
                            insert_product.Parameters.AddWithValue("@id_produit", product[j]); // Ajoute les détails de la recette dans la table plat_produit
                            insert_product.Parameters.AddWithValue("@qty", quantity[j]);
                            insert_product.ExecuteNonQuery();

                            string query = " UPDATE produit SET stock_minimum = (stock_minimum/2)+3*@qty WHERE id_produit=@id_produit";
                            MySqlCommand update_stockmin = new MySqlCommand(query, con);
                            update_stockmin.Parameters.AddWithValue("@id_produit", product[j]); // Modifie stock_minimum
                            update_stockmin.Parameters.AddWithValue("@qty", quantity[j]);
                            update_stockmin.ExecuteNonQuery();
                            query = " UPDATE produit SET stock_maximum = stock_maximum+2*@qty WHERE id_produit=@id_produit";
                            MySqlCommand update_stockmax = new MySqlCommand(query, con);
                            update_stockmax.Parameters.AddWithValue("@id_produit", product[j]); // Modifie stock_minimal
                            update_stockmax.Parameters.AddWithValue("@qty", quantity[j]);
                            update_stockmax.ExecuteNonQuery();

                        }


                        Console.WriteLine("Recette bien créée!! ");
                        Console.ReadKey();
                    }
                    if (choix2 == 2)
                    {
                        bool end = false;
                        List<int> plat = new List<int>();
                        List<int> quantity = new List<int>();
                        int prix_tot = 0;
                        do
                        {
                            string sql = "SELECT id_plat,nom_plat,description,prix_vente FROM plat";
                            MySqlCommand cmd = new MySqlCommand(sql, con);
                            MySqlDataReader info = cmd.ExecuteReader();
                            Console.WriteLine("0 - Finnaliser et payer la commande ");
                            while (info.Read())
                            {
                                Console.Write(info.GetString(0) + " - ");
                                Console.Write(info.GetString(1) + " - ");       //Affiche les plats disponibles
                                Console.Write(info.GetString(2) + " - ");
                                Console.WriteLine(info.GetString(3) + " cook");
                            }
                            Console.WriteLine("Sélectionnez l'id du plat que vous souhaitez ajouter à votre commande :");
                            int id = Convert.ToInt32(Console.ReadLine());
                            int qty = 0;
                            if (id != 0)
                            {
                                Console.WriteLine("Quelle quantité de ce plat souhaitez vous ajouter a votre commande :");
                                qty = Convert.ToInt32(Console.ReadLine());
                            }
                            info.Close();

                            string requette_prix = "SELECT prix_vente FROM plat WHERE id_plat=@id_plat";
                            MySqlCommand cmd_prix = new MySqlCommand(requette_prix, con);

                            cmd_prix.Parameters.AddWithValue("@id_plat", id);

                            MySqlDataReader info_prix = cmd_prix.ExecuteReader();
                            while (info_prix.Read())
                            {

                                prix_tot += info_prix.GetInt16(0) * qty;  // Fait le prix total de tout les plats commandés
                            }

                            Console.WriteLine();
                            Console.WriteLine("Prix total de la commande: " + prix_tot + " cook");
                            Console.WriteLine();

                            if (id == 0) { end = true; }
                            info_prix.Close();
                            plat.Add(id);
                            quantity.Add(qty);
                        } while (!end);

                        DateTime aDate = DateTime.Now;
                        string date = aDate.ToString("MM/dd/yyyy");


                        string new_commande = "INSERT INTO commande(date_commande,id_client) VALUES(@date, @id_client)";

                        MySqlCommand insert = new MySqlCommand(new_commande, con);
                        insert.Parameters.AddWithValue("@date", aDate);                    // On ajoute la commande à la table commande
                        insert.Parameters.AddWithValue("@id_client", id_client);
                        insert.ExecuteNonQuery();

                        string lastid = "SELECT MAX(id_commande) FROM commande";
                        MySqlCommand cmdid = new MySqlCommand(lastid, con);
                        MySqlDataReader infoid = cmdid.ExecuteReader();
                        int id_commande = 0;
                        while (infoid.Read())
                        {
                            id_commande = infoid.GetInt16(0);
                        }
                        infoid.Close();

                        string new_plat_produit = "INSERT INTO ligne_commande(id_commande,id_plat,quantity_plat) VALUES(@id_commande, @id_plat,@qty)";
                        for (int j = 0; j < (plat.Count) - 1; j++)
                        {
                            MySqlCommand insert_product = new MySqlCommand(new_plat_produit, con);
                            insert_product.Parameters.AddWithValue("@id_commande", id_commande);
                            insert_product.Parameters.AddWithValue("@id_plat", plat[j]);        // On ajoute les details de la commande à la table ligne_commande
                            insert_product.Parameters.AddWithValue("@qty", quantity[j]);
                            insert_product.ExecuteNonQuery();

                            string mysql_query = " SELECT compteur FROM plat WHERE id_plat = @id_plat";
                            MySqlCommand cmd = new MySqlCommand(mysql_query, con);
                            cmd.Parameters.AddWithValue("@id_plat", plat[j]);
                            MySqlDataReader info = cmd.ExecuteReader();

                            int compteur0 = 0;
                            while (info.Read())
                            {
                                compteur0 = info.GetInt16(0);
                            }
                            info.Close();

                            mysql_query = " UPDATE plat SET compteur = compteur + @qty WHERE id_plat = @id_plat";
                            MySqlCommand add_compteur = new MySqlCommand(mysql_query, con);
                            add_compteur.Parameters.AddWithValue("@id_plat", plat[j]);  // On augmente les compteur correspondant au plats commandés
                            add_compteur.Parameters.AddWithValue("@qty", quantity[j]);
                            add_compteur.ExecuteNonQuery();

                            mysql_query = " SELECT compteur,id_client FROM plat WHERE id_plat = @id_plat";
                            cmd = new MySqlCommand(mysql_query, con);
                            cmd.Parameters.AddWithValue("@id_plat", plat[j]);
                            info = cmd.ExecuteReader();
                            int cdr_paie = 0;
                            int compteur1 = 0;
                            while (info.Read())
                            {
                                compteur1 = info.GetInt16(0);
                                cdr_paie = info.GetInt16(1);
                            }
                            info.Close();
                            if (compteur0 < 10 && compteur1 >= 10)
                            {
                                mysql_query = " UPDATE plat SET prix_vente = prix_vente + 2 WHERE id_plat = @id_plat";
                                MySqlCommand change_price = new MySqlCommand(mysql_query, con);
                                change_price.Parameters.AddWithValue("@id_plat", plat[j]); //augmente le prix du plat si il passe au dessus de 10 commandes
                                change_price.ExecuteNonQuery();
                            }
                            if (compteur0 < 50 && compteur1 >= 50)
                            {
                                mysql_query = " UPDATE plat SET prix_vente = prix_vente + 5 WHERE id_plat = @id_plat";
                                MySqlCommand change_price = new MySqlCommand(mysql_query, con);//augmente le prix du plat si il passe au dessus de 50 commandes
                                change_price.Parameters.AddWithValue("@id_plat", plat[j]);
                                change_price.ExecuteNonQuery();
                            }
                            if (compteur1 > 50)
                            {
                                mysql_query = " UPDATE client SET cook = cook + 4*@qty WHERE id_client = @id_client";
                                MySqlCommand pay_cdr = new MySqlCommand(mysql_query, con);
                                pay_cdr.Parameters.AddWithValue("@id_client", cdr_paie); // Paye le CDR 4 cook dans le cas ou son plat a déja plus de 50 commandes
                                pay_cdr.Parameters.AddWithValue("@qty", quantity[j]);
                                pay_cdr.ExecuteNonQuery();
                            }
                            else
                            {
                                mysql_query = " UPDATE client SET cook = cook + 2*@qty WHERE id_client = @id_client";
                                MySqlCommand pay_cdr = new MySqlCommand(mysql_query, con);
                                pay_cdr.Parameters.AddWithValue("@id_client", cdr_paie); // Paye le CDR 2 cook habituellement
                                pay_cdr.Parameters.AddWithValue("@qty", quantity[j]);
                                pay_cdr.ExecuteNonQuery();
                            }

                            mysql_query = " SELECT quantity_produit,plat_produit.id_produit FROM plat_produit JOIN produit ON produit.id_produit = plat_produit.id_produit WHERE id_plat=@id_plat";
                            MySqlCommand compteur_stock = new MySqlCommand(mysql_query, con);
                            compteur_stock.Parameters.AddWithValue("@id_plat", plat[j]);

                            MySqlDataReader info_stock = compteur_stock.ExecuteReader();
                            List<int> produit = new List<int>();
                            List<int> quantite = new List<int>();
                            while (info_stock.Read())
                            {
                                produit.Add(info_stock.GetInt16(1));
                                quantite.Add(info_stock.GetInt16(0));     // Permet de récupérer les quaantité nécéssaire de chaque produit pour la conception du plat
                            }
                            info_stock.Close();
                            for (int i = 0; i < produit.Count; i++)
                            {
                                mysql_query = " UPDATE produit SET stock_actuel = stock_actuel - @qty*@qte WHERE id_produit = @id_produit";
                                MySqlCommand update_stock = new MySqlCommand(mysql_query, con);
                                update_stock.Parameters.AddWithValue("@qte", quantite[i]);
                                update_stock.Parameters.AddWithValue("@qty", quantity[j]);          // Modifie les stock actuel des produits composant le plat
                                update_stock.Parameters.AddWithValue("@id_produit", produit[i]);
                                update_stock.ExecuteNonQuery();
                            }



                        }

                        Console.WriteLine("Commande efféctuée.  ");
                        Console.ReadKey();



                    }
                    if (choix2 == 3)
                    {
                        string point_cook = "SELECT cook FROM client WHERE id_client=@id";
                        MySqlCommand cmd_cook = new MySqlCommand(point_cook, con);
                        cmd_cook.Parameters.AddWithValue("@id", id_client);
                        MySqlDataReader info_cook = cmd_cook.ExecuteReader();

                        while (info_cook.Read())
                        {
                            Console.WriteLine("Nombre de points cook: " + info_cook.GetInt16(0));
                            Console.ReadKey();
                        }
                        info_cook.Close();


                    }
                    if (choix2 == 4)
                    {
                        string mes_recettes = "SELECT nom_plat,compteur FROM plat WHERE id_client=@id";
                        MySqlCommand cmd_recette = new MySqlCommand(mes_recettes, con);
                        cmd_recette.Parameters.AddWithValue("@id", id_client);
                        MySqlDataReader info_recette = cmd_recette.ExecuteReader();

                        while (info_recette.Read())
                        {
                            Console.Write("Recette: " + info_recette.GetString(0));
                            Console.WriteLine(" - Nombre de commande: " + info_recette.GetInt16(1));

                        }
                        info_recette.Close();
                        Console.ReadKey();
                    }


                    ///





                    ///



                }
                else
                {
                    bool end = false;
                    List<int> plat = new List<int>();
                    List<int> quantity = new List<int>();
                    int prix_tot = 0;
                    Console.WriteLine("Veuillez effectuer votre commande ");
                    Console.WriteLine();
                    do
                    {
                        string sql = "SELECT id_plat,nom_plat,description,prix_vente FROM plat";
                        MySqlCommand cmd = new MySqlCommand(sql, con);
                        MySqlDataReader info = cmd.ExecuteReader();
                        Console.WriteLine("0 - Finnaliser et payer la commande ");
                        while (info.Read())
                        {
                            Console.Write(info.GetString(0) + " - ");
                            Console.Write(info.GetString(1) + " - ");       //Affiche les plats disponibles
                            Console.Write(info.GetString(2) + " - ");
                            Console.WriteLine(info.GetString(3) + " cook");
                        }
                        Console.WriteLine("Sélectionnez l'id du plat que vous souhaitez ajouter à votre commande :");
                        int id = Convert.ToInt32(Console.ReadLine());
                        int qty = 0;
                        if (id != 0)
                        {
                            Console.WriteLine("Quelle quantité de ce plat souhaitez vous ajouter a votre commande :");
                            qty = Convert.ToInt32(Console.ReadLine());
                        }
                        info.Close();

                        string requette_prix = "SELECT prix_vente FROM plat WHERE id_plat=@id_plat";
                        MySqlCommand cmd_prix = new MySqlCommand(requette_prix, con);

                        cmd_prix.Parameters.AddWithValue("@id_plat", id);

                        MySqlDataReader info_prix = cmd_prix.ExecuteReader();
                        while (info_prix.Read())
                        {
                            prix_tot += info_prix.GetInt16(0) * qty;  // Fait le prix total de tout les plats commandés
                        }

                        Console.WriteLine();
                        Console.WriteLine("Prix total de la commande: " + prix_tot + " cook");
                        Console.WriteLine();

                        if (id == 0) { end = true; }
                        info_prix.Close();
                        plat.Add(id);
                        quantity.Add(qty);
                    } while (!end);

                    DateTime aDate = DateTime.Now;


                    string new_commande = "INSERT INTO commande(date_commande,id_client) VALUES(@date, @id_client)";

                    MySqlCommand insert = new MySqlCommand(new_commande, con);
                    insert.Parameters.AddWithValue("@date", aDate);                    // On ajoute la commande à la table commande
                    insert.Parameters.AddWithValue("@id_client", id_client);
                    insert.ExecuteNonQuery();

                    string lastid = "SELECT MAX(id_commande) FROM commande";
                    MySqlCommand cmdid = new MySqlCommand(lastid, con);
                    MySqlDataReader infoid = cmdid.ExecuteReader();
                    int id_commande = 0;
                    while (infoid.Read())
                    {
                        id_commande = infoid.GetInt16(0);
                    }
                    infoid.Close();

                    string new_plat_produit = "INSERT INTO ligne_commande(id_commande,id_plat,quantity_plat) VALUES(@id_commande, @id_plat,@qty)";
                    for (int j = 0; j < (plat.Count) - 1; j++)
                    {
                        MySqlCommand insert_product = new MySqlCommand(new_plat_produit, con);
                        insert_product.Parameters.AddWithValue("@id_commande", id_commande);
                        insert_product.Parameters.AddWithValue("@id_plat", plat[j]);        // On ajoute les details de la commande à la table ligne_commande
                        insert_product.Parameters.AddWithValue("@qty", quantity[j]);
                        insert_product.ExecuteNonQuery();

                        string mysql_query = " SELECT compteur FROM plat WHERE id_plat = @id_plat";
                        MySqlCommand cmd = new MySqlCommand(mysql_query, con);
                        cmd.Parameters.AddWithValue("@id_plat", plat[j]);
                        MySqlDataReader info = cmd.ExecuteReader();

                        int compteur0 = 0;
                        while (info.Read())
                        {
                            compteur0 = info.GetInt16(0);
                        }
                        info.Close();

                        mysql_query = " UPDATE plat SET compteur = compteur + @qty WHERE id_plat = @id_plat";
                        MySqlCommand add_compteur = new MySqlCommand(mysql_query, con);
                        add_compteur.Parameters.AddWithValue("@id_plat", plat[j]);  // On augmente les compteur correspondant au plats commandés
                        add_compteur.Parameters.AddWithValue("@qty", quantity[j]);
                        add_compteur.ExecuteNonQuery();

                        mysql_query = " SELECT compteur,id_client FROM plat WHERE id_plat = @id_plat";
                        cmd = new MySqlCommand(mysql_query, con);
                        cmd.Parameters.AddWithValue("@id_plat", plat[j]);
                        info = cmd.ExecuteReader();
                        int cdr_paie = 0;
                        int compteur1 = 0;
                        while (info.Read())
                        {
                            compteur1 = info.GetInt16(0);
                            cdr_paie = info.GetInt16(1);
                        }
                        info.Close();
                        if (compteur0 < 10 && compteur1 >= 10)
                        {
                            mysql_query = " UPDATE plat SET prix_vente = prix_vente + 2 WHERE id_plat = @id_plat";
                            MySqlCommand change_price = new MySqlCommand(mysql_query, con);
                            change_price.Parameters.AddWithValue("@id_plat", plat[j]); //augmente le prix du plat si il passe au dessus de 10 commandes
                            change_price.ExecuteNonQuery();
                        }
                        if (compteur0 < 50 && compteur1 >= 50)
                        {
                            mysql_query = " UPDATE plat SET prix_vente = prix_vente + 5 WHERE id_plat = @id_plat";
                            MySqlCommand change_price = new MySqlCommand(mysql_query, con);//augmente le prix du plat si il passe au dessus de 50 commandes
                            change_price.Parameters.AddWithValue("@id_plat", plat[j]);
                            change_price.ExecuteNonQuery();
                        }
                        if (compteur1 > 50)
                        {
                            mysql_query = " UPDATE client SET cook = cook + 4*@qty WHERE id_client = @id_client";
                            MySqlCommand pay_cdr = new MySqlCommand(mysql_query, con);
                            pay_cdr.Parameters.AddWithValue("@id_client", cdr_paie); // Paye le CDR 4 cook dans le cas ou son plat a déja plus de 50 commandes
                            pay_cdr.Parameters.AddWithValue("@qty", quantity[j]);
                            pay_cdr.ExecuteNonQuery();
                        }
                        else
                        {
                            mysql_query = " UPDATE client SET cook = cook + 2*@qty WHERE id_client = @id_client";
                            MySqlCommand pay_cdr = new MySqlCommand(mysql_query, con);
                            pay_cdr.Parameters.AddWithValue("@id_client", cdr_paie); // Paye le CDR 2 cook habituellement
                            pay_cdr.Parameters.AddWithValue("@qty", quantity[j]);
                            pay_cdr.ExecuteNonQuery();
                        }

                        mysql_query = " SELECT quantity_produit,plat_produit.id_produit FROM plat_produit JOIN produit ON produit.id_produit = plat_produit.id_produit WHERE id_plat=@id_plat";
                        MySqlCommand compteur_stock = new MySqlCommand(mysql_query, con);
                        compteur_stock.Parameters.AddWithValue("@id_plat", plat[j]);

                        MySqlDataReader info_stock = compteur_stock.ExecuteReader();
                        List<int> produit = new List<int>();
                        List<int> quantite = new List<int>();
                        while (info_stock.Read())
                        {
                            produit.Add(info_stock.GetInt16(1));
                            quantite.Add(info_stock.GetInt16(0));     // Permet de récupérer les quaantité nécéssaire de chaque produit pour la conception du plat
                        }
                        info_stock.Close();
                        for (int i = 0; i < produit.Count; i++)
                        {
                            mysql_query = " UPDATE produit SET stock_actuel = stock_actuel - @qty*@qte WHERE id_produit = @id_produit";
                            MySqlCommand update_stock = new MySqlCommand(mysql_query, con);
                            update_stock.Parameters.AddWithValue("@qte", quantite[i]);
                            update_stock.Parameters.AddWithValue("@qty", quantity[j]);          // Modifie les stock actuel des produits composant le plat
                            update_stock.Parameters.AddWithValue("@id_produit", produit[i]);
                            update_stock.ExecuteNonQuery();
                        }



                    }

                    Console.WriteLine("Commande efféctuée.  ");
                    Console.ReadKey();
                }
            }
            if (choix == 3)
            {

                Console.WriteLine("Que souhaitez vous consulter: ");
                Console.WriteLine(" 1 - CdR de la semaine ");
                Console.WriteLine(" 2 - Top 5 recettes ");
                Console.WriteLine(" 3 - CdR d'OR ");


                int choix3 = Convert.ToInt32(Console.ReadLine());

                if (choix3 == 1)
                {
                    DateTime date = DateTime.Now;
                    string formatForMySql = date.ToString("yyyy-MM-dd");

                    DateTime new_date = date.AddDays(-7);
                    string formatForMySql2 = new_date.ToString("yyyy-MM-dd");


                    string mysql_query = "  SELECT quantity_plat,ligne_commande.id_plat FROM ligne_commande JOIN commande ON commande.id_commande = ligne_commande.id_commande WHERE commande.date_commande BETWEEN @date1 AND @date2";
                    MySqlCommand week_order = new MySqlCommand(mysql_query, con);
                    week_order.Parameters.AddWithValue("@date1", formatForMySql2);
                    week_order.Parameters.AddWithValue("@date2", formatForMySql);

                    MySqlDataReader info_order = week_order.ExecuteReader();
                    List<int> plat = new List<int>();
                    List<int> quantite = new List<int>();
                    while (info_order.Read())
                    {
                        plat.Add(info_order.GetInt16(1));       //récupère les plats commandés ainsi que leur quantité
                        quantite.Add(info_order.GetInt16(0));

                    }

                    info_order.Close();
                    int[] compteur_plat = new int[plat.Count()];
                    for (int i = 0; i < plat.Count(); i++)
                    {
                        compteur_plat[plat[i]] += quantite[i]; //additionne tout les mêmes plats entre eux
                    }
                    int[] compteur_cdr = new int[compteur_plat.Length - 1];
                    for (int i = 1; i < compteur_plat.Length - 1; i++)
                    {
                        mysql_query = " SELECT id_client FROM plat WHERE id_plat = @id_plat";
                        MySqlCommand cmd = new MySqlCommand(mysql_query, con);

                        cmd.Parameters.AddWithValue("@id_plat", i);
                        MySqlDataReader info = cmd.ExecuteReader();

                        while (info.Read())
                        {
                            compteur_cdr[info.GetInt16(0)] += compteur_plat[i];


                        }
                        info.Close();

                    }

                    int best_score = 0;
                    int best_cdr = 0;
                    for (int i = 0; i < compteur_cdr.Length - 1; i++)
                    {
                        if (compteur_cdr[i] > best_score)
                        {
                            Console.WriteLine(compteur_cdr[i]);
                            best_score = compteur_cdr[i];
                            best_cdr = i;
                        }
                    }
                    mysql_query = " SELECT name_client, email FROM client WHERE id_client = @id_client";
                    MySqlCommand cmd_cdr = new MySqlCommand(mysql_query, con);

                    cmd_cdr.Parameters.AddWithValue("@id_client", best_cdr);
                    MySqlDataReader info_cdr = cmd_cdr.ExecuteReader();
                    string nom_best_cdr = "";
                    string mail_best_cdr = "";
                    while (info_cdr.Read())
                    {
                        nom_best_cdr = info_cdr.GetString(0);
                        mail_best_cdr = info_cdr.GetString(1);

                    }
                    info_cdr.Close();
                    Console.WriteLine("Le meilleur CdR de cette semaine est " + nom_best_cdr + "; email: " + mail_best_cdr + "  avec un total de " + compteur_cdr[best_cdr] + " plats vendus");
                    Console.ReadKey();
                }
                if (choix3 == 2)
                {
                    string mysql_query = "  SELECT quantity_plat,ligne_commande.id_plat FROM ligne_commande JOIN commande ON commande.id_commande = ligne_commande.id_commande";
                    MySqlCommand all_order = new MySqlCommand(mysql_query, con);


                    MySqlDataReader info_order = all_order.ExecuteReader();
                    List<int> plat = new List<int>();
                    List<int> quantite = new List<int>();
                    while (info_order.Read())
                    {
                        plat.Add(info_order.GetInt16(1));
                        quantite.Add(info_order.GetInt16(0));

                    }

                    info_order.Close();
                    int[] compteur_plat = new int[plat.Count()];
                    for (int i = 0; i < plat.Count(); i++)
                    {
                        compteur_plat[plat[i]] += quantite[i];
                    }
                    int top1 = 0;
                    int top2 = 0;
                    int top3 = 0;
                    int top4 = 0;
                    int top5 = 0;
                    compteur_plat[0] = 0;
                    for (int i = 0; i < compteur_plat.Length - 1; i++)
                    {
                        if (compteur_plat[i] > compteur_plat[top1])
                        {
                            top5 = top4; top4 = top3; top3 = top2; top2 = top1;
                            top1 = i;
                        }
                        else if (compteur_plat[i] > compteur_plat[top2])
                        {
                            top5 = top4; top4 = top3; top3 = top2;
                            top2 = i;
                        }
                        else if (compteur_plat[i] > compteur_plat[top3])
                        {
                            top5 = top4; top4 = top3;
                            top3 = i;
                        }
                        else if (compteur_plat[i] > compteur_plat[top4])
                        {
                            top5 = top4;
                            top4 = i;
                        }
                        else if (compteur_plat[i] > compteur_plat[top5])
                        {
                            top5 = i;
                        }
                    }
                    int[] top = new int[] { top1, top2, top3, top4, top5 };

                    for (int i = 0; i < 5; i++)
                    {
                        mysql_query = " SELECT nom_plat, type,compteur,id_client FROM plat WHERE id_plat = @id_plat";
                        MySqlCommand cmd_top = new MySqlCommand(mysql_query, con);
                        cmd_top.Parameters.AddWithValue("@id_plat", top[i]);
                        MySqlDataReader info_top = cmd_top.ExecuteReader();
                        string nom_plat = "";
                        string type = "";
                        int compteur = 0;
                        int client_id = 0;
                        while (info_top.Read())
                        {
                            nom_plat = info_top.GetString(0);
                            type = info_top.GetString(1);
                            compteur = info_top.GetInt16(2);
                            client_id = info_top.GetInt16(3);

                        }
                        info_top.Close();

                        mysql_query = " SELECT name_client, email FROM client WHERE id_client = @id_client";
                        cmd_top = new MySqlCommand(mysql_query, con);
                        cmd_top.Parameters.AddWithValue("@id_client", client_id);
                        MySqlDataReader info = cmd_top.ExecuteReader();
                        string nom_best = "";
                        string mail_best = "";
                        while (info.Read())
                        {
                            nom_best = info.GetString(0);
                            mail_best = info.GetString(1);

                        }
                        info.Close();


                        Console.WriteLine("Top " + (i + 1) + " : " + nom_plat + " - " + type + " vendu " + compteur + " fois, créer par " + nom_best + " : " + mail_best);

                    }
                    Console.ReadKey();
                }
                if (choix3 == 3)
                {
                    string mysql_query = "  SELECT quantity_plat,ligne_commande.id_plat FROM ligne_commande JOIN commande ON commande.id_commande = ligne_commande.id_commande";
                    MySqlCommand week_order = new MySqlCommand(mysql_query, con);


                    MySqlDataReader info_order = week_order.ExecuteReader();
                    List<int> plat = new List<int>();
                    List<int> quantite = new List<int>();
                    while (info_order.Read())
                    {
                        plat.Add(info_order.GetInt16(1));
                        quantite.Add(info_order.GetInt16(0));

                    }

                    info_order.Close();


                    int[] compteur_plat = new int[plat.Count()];
                    for (int i = 0; i < plat.Count(); i++)
                    {
                        compteur_plat[plat[i]] += quantite[i];
                    }
                    int[] compteur_cdr = new int[compteur_plat.Length - 1];
                    for (int i = 1; i < compteur_plat.Length - 1; i++)
                    {
                        mysql_query = " SELECT id_client FROM plat WHERE id_plat = @id_plat";
                        MySqlCommand cmd = new MySqlCommand(mysql_query, con);

                        cmd.Parameters.AddWithValue("@id_plat", i);
                        MySqlDataReader info = cmd.ExecuteReader();

                        while (info.Read())
                        {
                            compteur_cdr[info.GetInt16(0)] += compteur_plat[i];

                        }
                        info.Close();

                    }

                    int best_score = 0;
                    int best_cdr = 0;
                    for (int i = 0; i < compteur_cdr.Length - 1; i++)
                    {
                        if (compteur_cdr[i] > best_score)
                        {
                            best_score = compteur_cdr[i];
                            best_cdr = i;
                        }
                    }
                    mysql_query = " SELECT name_client, email FROM client WHERE id_client = @id_client";
                    MySqlCommand cmd_cdr = new MySqlCommand(mysql_query, con);

                    cmd_cdr.Parameters.AddWithValue("@id_client", best_cdr);
                    MySqlDataReader info_cdr = cmd_cdr.ExecuteReader();
                    string nom_best_cdr = "";
                    string mail_best_cdr = "";
                    while (info_cdr.Read())
                    {
                        nom_best_cdr = info_cdr.GetString(0);
                        mail_best_cdr = info_cdr.GetString(1);

                    }
                    info_cdr.Close();
                    Console.WriteLine("Le CdR d'OR est " + nom_best_cdr + "; email: " + mail_best_cdr + "  avec un total de " + compteur_cdr[best_cdr] + " plats vendus");
                    Console.ReadKey();


                    mysql_query = " SELECT id_plat,compteur FROM plat WHERE id_client = @id_client";
                    cmd_cdr = new MySqlCommand(mysql_query, con);

                    cmd_cdr.Parameters.AddWithValue("@id_client", best_cdr);
                    info_cdr = cmd_cdr.ExecuteReader();
                    List<int> plat2 = new List<int>();
                    List<int> quantite2 = new List<int>();
                    while (info_cdr.Read())
                    {

                        plat2.Add(info_cdr.GetInt16(0));
                        quantite2.Add(info_cdr.GetInt16(1));
                    }
                    info_cdr.Close();
                    int top1 = 0;
                    int top2 = 0;
                    int top3 = 0;
                    int top4 = 0;
                    int top5 = 0;

                    for (int i = 1; i < plat2.Count - 1; i++)
                    {
                        if (quantite2[i] > quantite2[top1])
                        {
                            top5 = top4; top4 = top3; top3 = top2; top2 = top1;
                            top1 = plat2[i];
                        }
                        else if (quantite2[i] > quantite2[top2])
                        {
                            top5 = top4; top4 = top3; top3 = top2;
                            top2 = plat2[i];
                        }
                        else if (quantite2[i] > quantite2[top3])
                        {
                            top5 = top4; top4 = top3;
                            top3 = plat2[i];
                        }
                        else if (quantite2[i] > quantite2[top4])
                        {
                            top5 = top4;
                            top4 = plat2[i];
                        }
                        else if (quantite2[i] > quantite2[top5])
                        {

                            top5 = plat2[i];
                        }
                    }
                    Console.WriteLine(top1);
                    Console.WriteLine(top2);
                    Console.WriteLine(top3);
                    Console.WriteLine(top4);
                    Console.WriteLine(top5);

                    Console.ReadKey();
                }
            }
            if (choix == 4)
            {
                string lastid = "SELECT MAX(id_client) FROM client";
                MySqlCommand cmdid = new MySqlCommand(lastid, con);
                MySqlDataReader infoid = cmdid.ExecuteReader();
                int nb_client = 0;
                while (infoid.Read())
                {
                    nb_client = infoid.GetInt16(0);
                }
                infoid.Close();
                Console.WriteLine("Nombres total de client: " + nb_client);
                Console.ReadKey();

                string get_cdr = "SELECT name_client,id_client FROM client WHERE cdr=true";
                cmdid = new MySqlCommand(get_cdr, con);
                infoid = cmdid.ExecuteReader();
                List<string> nom_cdr = new List<string>();
                List<int> id_cdr = new List<int>();
                while (infoid.Read())
                {
                    nom_cdr.Add(infoid.GetString(0));
                    id_cdr.Add(infoid.GetInt16(1));
                }
                infoid.Close();
                Console.WriteLine("Nombre total de CdR: " + id_cdr.Count);
                for (int i = 0; i < id_cdr.Count; i++)
                {
                    string get_plat = "SELECT compteur FROM plat WHERE id_client=@id_client";
                    cmdid = new MySqlCommand(get_plat, con);
                    cmdid.Parameters.AddWithValue("@id_client", id_cdr[i]);
                    MySqlDataReader infoid2 = cmdid.ExecuteReader();
                    int nb_plats = 0;
                    while (infoid2.Read())
                    {
                        nb_plats += infoid2.GetInt16(0);
                    }
                    Console.WriteLine(nom_cdr[i] + " - nombre total de recette commandées:  " + nb_plats);
                    infoid2.Close();
                }
                Console.ReadKey();


                string stock = "SELECT nom_produit FROM produit WHERE stock_actuel<2*stock_minimum";
                cmdid = new MySqlCommand(stock, con);
                infoid = cmdid.ExecuteReader();
                Console.WriteLine("Produit ayant une quantité en stock <= 2 * leur quantité minimale");
                while (infoid.Read())
                {
                    Console.WriteLine(infoid.GetString(0));
                }
                infoid.Close();

                Console.ReadKey();
                string sql = "SELECT id_produit,nom_produit,unite FROM produit";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                MySqlDataReader info = cmd.ExecuteReader();

                while (info.Read())
                {
                    Console.Write(info.GetString(0) + " - "); // Permet de visualiser les produits disponibles
                    Console.Write(info.GetString(1) + " - ");
                    Console.WriteLine(info.GetString(2));
                }
                info.Close();
                Console.WriteLine("Veuillez sélectionner le nom d'un produit: ");
                string n_produit = Console.ReadLine();


                sql = "SELECT id_plat,quantity_produit,nom_produit FROM plat_produit JOIN produit ON plat_produit.id_produit=produit.id_produit";
                cmd = new MySqlCommand(sql, con);
                info = cmd.ExecuteReader();
                List<int> id = new List<int>();
                List<string> nom =new List<string>();
                List<int> q = new List<int>();

                while (info.Read())
                {
                   
                    if (info.GetString(2) == n_produit)
                    {
                        id.Add(info.GetInt16(0));
                        nom.Add(info.GetString(2));
                        q.Add(info.GetInt16(1));

                        

                    }
                    
                    
                }
                info.Close();
                for (int i = 0; i < q.Count; i++)
                {


                    string new_query = "SELECT nom_plat FROM plat WHERE id_plat=@id";
                    MySqlCommand cmd_query = new MySqlCommand(new_query, con);
                    cmd_query.Parameters.AddWithValue("@id", id[i]);
                    MySqlDataReader res = cmd_query.ExecuteReader();
                    while (res.Read())
                    {
                        Console.Write(res.GetString(0));
                    }
                    Console.WriteLine(" utilise " + q[i] + " " + n_produit);
                    res.Close();
                    Console.ReadKey();
                }
            }
        }
    }
}
