# Architecture de l'API REST Bank avec EF Core

## Structure du Projet

Le projet est maintenant organisé en 3 couches distinctes :

### 1. **APIRestCOURS.DataAccess** (Couche d'accès aux données)
- **Rôle** : Gestion de la persistance des données avec Entity Framework Core et SQLite
- **Contenu** :
  - `Models/` : Entités de base de données (User, Account, Transaction)
  - `BankDbContext.cs` : Contexte Entity Framework
  - `Interfaces/IBankDataAccess.cs` : Interface d'accès aux données
  - `BankDataAccess.cs` : Implémentation de l'accès aux données
  - `Migrations/` : Migrations Entity Framework

### 2. **APIRestCOURS.Service** (Couche métier)
- **Rôle** : Logique métier et transformation des données
- **Contenu** :
  - `DTOs/` : Data Transfer Objects pour la communication avec l'API
  - `Interfaces/IBankService.cs` : Interface du service métier
  - `BankService.cs` : Implémentation de la logique métier

### 3. **APIRestCOURS** (Couche présentation/API)
- **Rôle** : Exposition des endpoints REST et gestion des requêtes HTTP
- **Contenu** :
  - `Controllers/` : Contrôleurs API (UsersController, AccountsController, BankController)
  - `Middleware/` : Middleware personnalisé (AuditMiddleware)
  - `Program.cs` : Configuration de l'application

## Configuration

### Base de données
- **Type** : SQLite
- **Fichier** : `bank.db` (créé automatiquement dans le répertoire du projet API)
- **Chaîne de connexion** : Configurée dans `appsettings.json`

### Packages NuGet installés
- **DataAccess** :
  - Microsoft.EntityFrameworkCore (9.0.10)
  - Microsoft.EntityFrameworkCore.Sqlite (9.0.10)
  - Microsoft.Data.Sqlite.Core (9.0.10)

- **API** :
  - Microsoft.EntityFrameworkCore.Design (9.0.10)
  - Microsoft.Data.Sqlite.Core (9.0.10)

## Endpoints disponibles

### Users
- `POST /api/users` - Créer un utilisateur
- `GET /api/users` - Liste tous les utilisateurs
- `GET /api/users/{id}` - Obtenir un utilisateur par ID

### Accounts
- `POST /api/accounts` - Créer un compte
- `GET /api/accounts` - Liste tous les comptes
- `GET /api/accounts/{id}` - Obtenir un compte par ID
- `POST /api/accounts/{id}/deposit` - Effectuer un dépôt
- `POST /api/accounts/{id}/withdraw` - Effectuer un retrait
- `GET /api/accounts/{id}/transactions` - Liste des transactions d'un compte

### Bank (Requêtes complexes)
- `GET /api/bank/accounts/by-owner` - Comptes groupés par propriétaire
- `GET /api/bank/accounts/rich` - Comptes avec solde > 100 000
- `GET /api/bank/transactions/recent` - 50 dernières transactions
- `GET /api/bank/owners/top-rich` - Top 3 des propriétaires les plus riches
- `GET /api/bank/accounts/page/{pageNumber}?pageSize={size}` - Pagination des comptes

## Commandes utiles

### Créer une nouvelle migration
```bash
dotnet dotnet-ef migrations add MigrationName --project APIRestCOURS.DataAccess/APIRestCOURS.DataAccess.csproj --startup-project APIRestCOURS/APIRestCOURS.csproj
```

### Appliquer les migrations
```bash
dotnet dotnet-ef database update --project APIRestCOURS.DataAccess/APIRestCOURS.DataAccess.csproj --startup-project APIRestCOURS/APIRestCOURS.csproj
```

### Lancer l'application
```bash
dotnet run --project APIRestCOURS/APIRestCOURS.csproj
```

### Builder la solution
```bash
dotnet build APIRestCOURS.sln
```

## Principe de séparation des responsabilités

### DataAccess Layer
- Responsable uniquement de l'accès aux données
- Contient les requêtes LINQ to Entities
- Retourne des entités de base de données

### Service Layer
- Contient la logique métier
- Transforme les entités en DTOs
- Gère les validations et les règles métier
- Orchestration des opérations complexes

### API Layer
- Gestion des requêtes HTTP
- Validation des entrées
- Sérialisation/désérialisation JSON
- Gestion des codes de statut HTTP

## Tests

L'application a été testée avec succès avec les opérations suivantes :
- ✅ Création d'utilisateurs
- ✅ Création de comptes
- ✅ Dépôts et retraits
- ✅ Récupération des transactions
- ✅ Requêtes complexes (comptes riches, top propriétaires, etc.)
- ✅ Pagination

## Base de données SQLite

La base de données contient 3 tables principales :
- **Users** : Utilisateurs (Id, Nom, Prenom, DateNaissance)
- **Accounts** : Comptes bancaires (Id, OwnerId, Iban, Balance)
- **Transactions** : Transactions (Id, AccountId, At, Kind, Amount)

Relations :
- Un User peut avoir plusieurs Accounts (1-N)
- Un Account peut avoir plusieurs Transactions (1-N)
