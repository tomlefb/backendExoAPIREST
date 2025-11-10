using APIRestCOURS.DataAccess.Interfaces;
using APIRestCOURS.DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace APIRestCOURS.DataAccess;

public class DbSeeder : IDbSeeder
{
    private readonly BankDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<DbSeeder> _logger;

    public DbSeeder(BankDbContext context, UserManager<User> userManager, ILogger<DbSeeder> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            _logger.LogInformation("Starting database seeding...");

            // Vérifier si des données existent déjà
            if (await _context.Users.AnyAsync())
            {
                _logger.LogInformation("Database already contains data. Skipping seeding.");
                return;
            }

            // 1. Créer les utilisateurs avec UserManager
            var users = await CreateUsersAsync();
            _logger.LogInformation($"Created {users.Count} users");

            // 2. Créer les comptes
            var accounts = CreateAccounts(users);
            await _context.Accounts.AddRangeAsync(accounts);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Created {accounts.Count} accounts");

            // 3. Créer les transactions
            var transactions = CreateTransactions(accounts);
            await _context.Transactions.AddRangeAsync(transactions);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Created {transactions.Count} transactions");

            _logger.LogInformation("Database seeding completed successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    public async Task ClearAsync()
    {
        _logger.LogWarning("Clearing all data from database...");

        _context.Transactions.RemoveRange(_context.Transactions);
        _context.Accounts.RemoveRange(_context.Accounts);
        _context.RefreshTokens.RemoveRange(_context.RefreshTokens);
        _context.Users.RemoveRange(_context.Users);

        await _context.SaveChangesAsync();

        _logger.LogInformation("Database cleared successfully");
    }

    private async Task<List<User>> CreateUsersAsync()
    {
        var users = new List<User>();
        var password = "Password123!";

        var usersData = new[]
        {
            new { Nom = "Dupont", Prenom = "Jean", Email = "jean.dupont@email.com", DateNaissance = new DateTime(1990, 1, 15) },
            new { Nom = "Martin", Prenom = "Marie", Email = "marie.martin@email.com", DateNaissance = new DateTime(1985, 5, 20) },
            new { Nom = "Durand", Prenom = "Pierre", Email = "pierre.durand@email.com", DateNaissance = new DateTime(1995, 8, 10) },
            new { Nom = "Bernard", Prenom = "Sophie", Email = "sophie.bernard@email.com", DateNaissance = new DateTime(1988, 3, 25) },
            new { Nom = "Petit", Prenom = "Lucas", Email = "lucas.petit@email.com", DateNaissance = new DateTime(1992, 11, 5) },
            new { Nom = "Robert", Prenom = "Emma", Email = "emma.robert@email.com", DateNaissance = new DateTime(1993, 7, 18) }
        };

        foreach (var userData in usersData)
        {
            var user = new User
            {
                UserName = userData.Email,
                Email = userData.Email,
                Nom = userData.Nom,
                Prenom = userData.Prenom,
                DateNaissance = userData.DateNaissance,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                users.Add(user);
            }
            else
            {
                _logger.LogError($"Failed to create user {userData.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }

        return users;
    }

    private List<Account> CreateAccounts(List<User> users)
    {
        return new List<Account>
        {
            // Jean a 2 comptes
            new Account
            {
                OwnerId = users[0].Id,
                Iban = "FR7612345678901234567890123",
                Balance = 50000.00m
            },
            new Account
            {
                OwnerId = users[0].Id,
                Iban = "FR7612345678901234567890124",
                Balance = 25000.00m
            },
            // Marie a 1 compte riche
            new Account
            {
                OwnerId = users[1].Id,
                Iban = "FR7698765432109876543210987",
                Balance = 120000.00m
            },
            // Pierre a 1 compte
            new Account
            {
                OwnerId = users[2].Id,
                Iban = "FR7611111111111111111111111",
                Balance = 85000.00m
            },
            // Sophie a 1 compte très riche
            new Account
            {
                OwnerId = users[3].Id,
                Iban = "FR7622222222222222222222222",
                Balance = 200000.00m
            },
            // Lucas a 1 compte pauvre
            new Account
            {
                OwnerId = users[4].Id,
                Iban = "FR7633333333333333333333333",
                Balance = 500.00m
            }
        };
    }

    private List<Transaction> CreateTransactions(List<Account> accounts)
    {
        var baseDate = DateTime.UtcNow.AddDays(-30); // Transactions sur les 30 derniers jours

        return new List<Transaction>
        {
            // Compte 1 (Jean - premier compte)
            new Transaction
            {
                AccountId = accounts[0].Id,
                At = baseDate.AddDays(1),
                Kind = TransactionKind.Deposit,
                Amount = 10000.00m
            },
            new Transaction
            {
                AccountId = accounts[0].Id,
                At = baseDate.AddDays(6),
                Kind = TransactionKind.Withdrawal,
                Amount = 2500.00m
            },
            new Transaction
            {
                AccountId = accounts[0].Id,
                At = baseDate.AddDays(11),
                Kind = TransactionKind.Deposit,
                Amount = 5000.00m
            },

            // Compte 2 (Jean - deuxième compte)
            new Transaction
            {
                AccountId = accounts[1].Id,
                At = baseDate.AddDays(16),
                Kind = TransactionKind.Deposit,
                Amount = 15000.00m
            },
            new Transaction
            {
                AccountId = accounts[1].Id,
                At = baseDate.AddDays(21),
                Kind = TransactionKind.Withdrawal,
                Amount = 3000.00m
            },

            // Compte 3 (Marie)
            new Transaction
            {
                AccountId = accounts[2].Id,
                At = baseDate.AddDays(3),
                Kind = TransactionKind.Deposit,
                Amount = 50000.00m
            },
            new Transaction
            {
                AccountId = accounts[2].Id,
                At = baseDate.AddDays(13),
                Kind = TransactionKind.Withdrawal,
                Amount = 8000.00m
            },
            new Transaction
            {
                AccountId = accounts[2].Id,
                At = baseDate.AddDays(20),
                Kind = TransactionKind.Deposit,
                Amount = 25000.00m
            },

            // Compte 4 (Pierre)
            new Transaction
            {
                AccountId = accounts[3].Id,
                At = baseDate.AddDays(8),
                Kind = TransactionKind.Deposit,
                Amount = 60000.00m
            },
            new Transaction
            {
                AccountId = accounts[3].Id,
                At = baseDate.AddDays(18),
                Kind = TransactionKind.Withdrawal,
                Amount = 15000.00m
            },

            // Compte 5 (Sophie)
            new Transaction
            {
                AccountId = accounts[4].Id,
                At = baseDate.AddDays(2),
                Kind = TransactionKind.Deposit,
                Amount = 100000.00m
            },
            new Transaction
            {
                AccountId = accounts[4].Id,
                At = baseDate.AddDays(12),
                Kind = TransactionKind.Deposit,
                Amount = 75000.00m
            },
            new Transaction
            {
                AccountId = accounts[4].Id,
                At = baseDate.AddDays(22),
                Kind = TransactionKind.Withdrawal,
                Amount = 10000.00m
            },

            // Compte 6 (Lucas - compte pauvre)
            new Transaction
            {
                AccountId = accounts[5].Id,
                At = baseDate.AddDays(5),
                Kind = TransactionKind.Deposit,
                Amount = 500.00m
            }
        };
    }
}
