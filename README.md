# Blogic_Assignment

## Základní informace
Aplikace je psána stylem „code-first“ s EntityFramework Core. Schéma databáze je TPC (Table Per Concrete Type). V aplikaci působí tři základní druhy entit: poradci, klienti a smlouvy. Mezi poradci a smlouvou je vztah „many-to-many“, takže je v databázi dotvořena pomocná entita. Mezi klientem a smlouvou „one-to-many“, tj. jeden klient s mnoho smlouvami.

## Struktura
* Docs - uživatelská a technická dokumentace
* Blogic_Assignment_App
  * Models
    * User
    * Client
    * Consultant
    * Contract 
  * Views - CRUD
    * Shared
      * _LiveSearchJS
    * Clients
    * Consultants
    * Contracts
    * Home
  * ViewModels
    * Shared
      * PaginatedList
    * ContractDetails
    * ContractCreate
    * ConsultantDetails
    * ClientDetails
    * Home
  * Controllers
    * Consultants
    * Clients
    * Contracts
    * Home 
  * Services - zajišťují komunikaci mezi kontrolery a databází
    * Consultant + interface
    * Client + interface
    * Contract + interface
    * Home + interface
  * Migrations 

## Balíčky a knihovny
* EntityFrameworkCore 7.0.5
  * Design, SqlServer, Tools (7.0.5) 
* Bootstrap 5.1.0
  * Grid, Reboot, Utilities a soubor js (5.1.0)
* jQuery 3.5.1
  * jQuery Validation Plugin 1.17.0
