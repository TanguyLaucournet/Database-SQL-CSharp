create database if not exists Mydb;
use Mydb;
DROP TABLE IF EXISTS  fournisseur, plat, produit,client,plat_produit,commande,ligne_commande;


CREATE TABLE fournisseur(
	id_fournisseur smallint NOT NULL AUTO_INCREMENT, 
	nom_fournisseur varchar(50) NOT NULL,
	tel_fournisseur varchar(15) NOT NULL,
	PRIMARY KEY(id_fournisseur)
);

CREATE TABLE plat (
	id_plat smallint NOT NULL AUTO_INCREMENT,	
    nom_plat varchar(50) NOT NULL,
	date_creation date NOT NULL,
	type varchar(50) NOT NULL, 
	description varchar(255) NOT NULL,
	prix_vente smallint NOT NULL,
	id_client smallint NOT NULL,	
    compteur smallint DEFAULT 0,
	PRIMARY KEY(id_plat)
);
CREATE TABLE produit (
	id_produit smallint NOT NULL AUTO_INCREMENT, 
	nom_produit varchar (50) NOT NULL,
	categorie varchar (50) NOT NULL,
	unite varchar (50) NOT NULL, 
	stock_actuel smallint NOT NULL,
	stock_minimum smallint NOT NULL,
	stock_maximum smallint NOT NULL,
    id_fournisseur smallint,
    
	FOREIGN KEY(id_fournisseur) REFERENCES fournisseur(id_fournisseur),
	PRIMARY KEY(id_produit)			
);
CREATE TABLE plat_produit(
	
	
    id_plat smallint,
	FOREIGN KEY (id_plat)REFERENCES plat(id_plat),
    id_produit smallint,
	FOREIGN KEY (id_produit) REFERENCES produit(id_produit),
	quantity_produit smallint NOT NULL,
	PRIMARY KEY(id_plat,id_produit)

);

CREATE TABLE client (
	id_client smallint NOT NULL AUTO_INCREMENT,
	name_client varchar(50) NOT NULL,
	tel_client varchar(15) NOT NULL,
    email varchar(50) NOT NULL,
    pwd varchar(255) NOT NULL,
    adresse varchar(50) NOT NULL,
	cdr boolean NOT NULL,
    cook smallint NOT NULL,

	PRIMARY KEY(id_client)
);
CREATE TABLE commande (
	id_commande smallint NOT NULL AUTO_INCREMENT,
	date_commande date NOT NULL,
    id_client smallint,
	FOREIGN KEY (id_client) REFERENCES client(id_client),
	PRIMARY KEY(id_commande)
);

CREATE TABLE ligne_commande (
	id_commande smallint NOT NULL,
	FOREIGN KEY (id_commande)REFERENCES commande(id_commande),
    id_plat smallint,
	FOREIGN KEY (id_plat) REFERENCES plat(id_plat),
    quantity_plat smallint NOT NULL,
	PRIMARY KEY(id_plat,id_commande)
);






INSERT INTO fournisseur(nom_fournisseur,tel_fournisseur) VALUES("Boucher","0123456789");

INSERT INTO fournisseur(nom_fournisseur,tel_fournisseur) VALUES("Poissonier","0223456789");
INSERT INTO fournisseur(nom_fournisseur,tel_fournisseur) VALUES("Fromager","0323456789");
INSERT INTO produit(nom_produit,categorie,unite,stock_actuel,stock_minimum,stock_maximum,id_fournisseur) VALUES("dorade","poisson","kg",5,2,10,2);
INSERT INTO produit(nom_produit,categorie,unite,stock_actuel,stock_minimum,stock_maximum,id_fournisseur) VALUES("entrecote","viande","kg",5,2,10,1);
INSERT INTO produit(nom_produit,categorie,unite,stock_actuel,stock_minimum,stock_maximum,id_fournisseur) VALUES("roquefort","fromage","kg",5,2,10,3);
INSERT INTO client(name_client, tel_client, email, pwd, adresse,cdr,cook) VALUES("admin", "admin", "admin", "admin", "admin",1,1000);

