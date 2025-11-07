# Bank API - Projet B3 Dev

API REST pour la gestion d'une banque avec utilisateurs, comptes bancaires et transactions.

## Démarrage

```bash
cd APIRestCOURS
dotnet run
```

L'API sera disponible sur `https://localhost:5001` (ou `http://localhost:5000`)

## Swagger UI

Une fois lancé, accède à Swagger pour tester l'API : **https://localhost:5001/swagger**

## Endpoints disponibles

### Users (Utilisateurs)

- **POST /api/users** - Créer un utilisateur
  ```json
  {
    "nom": "Dupont",
    "prenom": "Jean",
    "dateNaissance": "1990-05-15"
  }
  ```

- **GET /api/users/{id}** - Récupérer un utilisateur

### Accounts (Comptes)

- **POST /api/accounts** - Créer un compte
  ```json
  {
    "ownerId": 1,
    "iban": "FR7612345678901234567890123",
    "initialBalance": 1000
  }
  ```

- **GET /api/accounts/{id}** - Récupérer un compte

- **POST /api/accounts/{id}/deposit** - Déposer de l'argent
  ```json
  500.50
  ```

- **POST /api/accounts/{id}/withdraw** - Retirer de l'argent
  ```json
  200.00
  ```

- **GET /api/accounts?page=2&pageSize=10** - Pagination des comptes

### Bank (Opérations LINQ)

- **GET /api/bank/accounts/by-owner** - Comptes groupés par propriétaire (dictionnaire)
- **GET /api/bank/accounts/rich** - Comptes avec balance > 1000€
- **GET /api/bank/transactions/recent** - 50 dernières transactions
- **GET /api/bank/owners/top-rich** - Top 3 des propriétaires les plus riches

## Middleware de traçabilité

Chaque requête est automatiquement tracée avec :
- ID de corrélation (renvoyé dans le header `X-Correlation-Id`)
- Timestamp UTC
- Chemin et méthode HTTP
- Client ID (via header `X-API-Key`, sinon "anonymous")
- Code de statut
- Durée d'exécution

Exemple de log :
```
[AUDIT] CorrelationId=abc-123 | Timestamp=2025-11-05 14:30:00.123 UTC |
Path=/api/accounts/1/withdraw | Method=POST | ClientId=client-123 |
StatusCode=200 | Result=Success | Duration=45ms
```

## Architecture

```
APIRestCOURS/
├── Models/          # Modèles de données
│   ├── User.cs
│   ├── Account.cs
│   ├── Transaction.cs
│   └── Bank.cs
├── DTOs/            # Data Transfer Objects
│   ├── CreateUserRequest.cs
│   ├── CreateAccountRequest.cs
│   ├── TransactionRequest.cs
│   ├── AccountResponse.cs
│   └── TransactionResponse.cs
├── Services/        # Logique métier
│   └── BankService.cs
├── Controllers/     # API REST endpoints
│   ├── UsersController.cs
│   ├── AccountsController.cs
│   └── BankController.cs
├── Middleware/      # Middleware de traçabilité
│   └── AuditMiddleware.cs
└── Program.cs       # Configuration de l'app
```

## Concepts utilisés

- **API REST** avec routing conventionnel
- **Builder pattern** (WebApplicationBuilder)
- **Dependency Injection** (BankService en Singleton)
- **DTOs** pour séparer les modèles internes de l'API publique
- **LINQ** pour toutes les opérations de requête
- **Middleware** personnalisé pour l'audit
- **Swagger/OpenAPI** pour la documentation auto-générée

## Exemple de scénario complet

1. Créer un utilisateur
2. Créer un compte pour cet utilisateur
3. Déposer de l'argent
4. Retirer de l'argent
5. Consulter les transactions récentes
6. Voir le top des plus riches

Tout est tracé automatiquement dans les logs !
