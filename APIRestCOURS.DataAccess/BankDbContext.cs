using System.Security.Cryptography;
using System.Text;
using APIRestCOURS.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace APIRestCOURS.DataAccess;

public class BankDbContext : DbContext
{
    public BankDbContext(DbContextOptions<BankDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nom).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Prenom).IsRequired().HasMaxLength(100);
            entity.Property(e => e.DateNaissance).IsRequired();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PasswordHash).IsRequired();

            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Account configuration
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Iban).IsRequired().HasMaxLength(34);
            entity.Property(e => e.Balance).HasColumnType("decimal(18,2)");

            entity.HasOne(e => e.Owner)
                .WithMany(u => u.Accounts)
                .HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Transaction configuration
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.At).IsRequired();
            entity.Property(e => e.Kind).IsRequired();

            entity.HasOne(e => e.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // RefreshToken configuration
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Token).IsRequired();
            entity.Property(e => e.ExpiresAt).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsRevoked).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.Token).IsUnique();
        });

        // Seed Data
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Helper method to hash passwords
        string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        // Seed Users
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Email = "jean.dupont@email.com",
                PasswordHash = HashPassword("Password123!"),
                Nom = "Dupont",
                Prenom = "Jean",
                DateNaissance = new DateTime(1990, 1, 15)
            },
            new User
            {
                Id = 2,
                Email = "marie.martin@email.com",
                PasswordHash = HashPassword("Password123!"),
                Nom = "Martin",
                Prenom = "Marie",
                DateNaissance = new DateTime(1985, 5, 20)
            },
            new User
            {
                Id = 3,
                Email = "pierre.durand@email.com",
                PasswordHash = HashPassword("Password123!"),
                Nom = "Durand",
                Prenom = "Pierre",
                DateNaissance = new DateTime(1995, 8, 10)
            },
            new User
            {
                Id = 4,
                Email = "sophie.bernard@email.com",
                PasswordHash = HashPassword("Password123!"),
                Nom = "Bernard",
                Prenom = "Sophie",
                DateNaissance = new DateTime(1988, 3, 25)
            }
        );

        // Seed Accounts
        modelBuilder.Entity<Account>().HasData(
            new Account
            {
                Id = 1,
                OwnerId = 1,
                Iban = "FR7612345678901234567890123",
                Balance = 50000.00m
            },
            new Account
            {
                Id = 2,
                OwnerId = 1,
                Iban = "FR7612345678901234567890124",
                Balance = 25000.00m
            },
            new Account
            {
                Id = 3,
                OwnerId = 2,
                Iban = "FR7698765432109876543210987",
                Balance = 120000.00m
            },
            new Account
            {
                Id = 4,
                OwnerId = 3,
                Iban = "FR7611111111111111111111111",
                Balance = 85000.00m
            },
            new Account
            {
                Id = 5,
                OwnerId = 4,
                Iban = "FR7622222222222222222222222",
                Balance = 200000.00m
            }
        );

        // Seed Transactions
        modelBuilder.Entity<Transaction>().HasData(
            // Transactions pour le compte 1 (Jean Dupont)
            new Transaction
            {
                Id = 1,
                AccountId = 1,
                Amount = 10000.00m,
                Kind = TransactionKind.Deposit,
                At = new DateTime(2024, 10, 11, 10, 0, 0, DateTimeKind.Utc)
            },
            new Transaction
            {
                Id = 2,
                AccountId = 1,
                Amount = 2500.00m,
                Kind = TransactionKind.Withdrawal,
                At = new DateTime(2024, 10, 16, 14, 30, 0, DateTimeKind.Utc)
            },
            new Transaction
            {
                Id = 3,
                AccountId = 1,
                Amount = 5000.00m,
                Kind = TransactionKind.Deposit,
                At = new DateTime(2024, 10, 21, 9, 15, 0, DateTimeKind.Utc)
            },
            // Transactions pour le compte 2 (Jean Dupont - 2e compte)
            new Transaction
            {
                Id = 4,
                AccountId = 2,
                Amount = 15000.00m,
                Kind = TransactionKind.Deposit,
                At = new DateTime(2024, 10, 26, 11, 0, 0, DateTimeKind.Utc)
            },
            new Transaction
            {
                Id = 5,
                AccountId = 2,
                Amount = 3000.00m,
                Kind = TransactionKind.Withdrawal,
                At = new DateTime(2024, 10, 31, 16, 45, 0, DateTimeKind.Utc)
            },
            // Transactions pour le compte 3 (Marie Martin)
            new Transaction
            {
                Id = 6,
                AccountId = 3,
                Amount = 50000.00m,
                Kind = TransactionKind.Deposit,
                At = new DateTime(2024, 10, 13, 10, 30, 0, DateTimeKind.Utc)
            },
            new Transaction
            {
                Id = 7,
                AccountId = 3,
                Amount = 8000.00m,
                Kind = TransactionKind.Withdrawal,
                At = new DateTime(2024, 10, 23, 13, 20, 0, DateTimeKind.Utc)
            },
            new Transaction
            {
                Id = 8,
                AccountId = 3,
                Amount = 12000.00m,
                Kind = TransactionKind.Withdrawal,
                At = new DateTime(2024, 11, 5, 15, 0, 0, DateTimeKind.Utc)
            },
            // Transactions pour le compte 4 (Pierre Durand)
            new Transaction
            {
                Id = 9,
                AccountId = 4,
                Amount = 30000.00m,
                Kind = TransactionKind.Deposit,
                At = new DateTime(2024, 10, 19, 12, 0, 0, DateTimeKind.Utc)
            },
            new Transaction
            {
                Id = 10,
                AccountId = 4,
                Amount = 5000.00m,
                Kind = TransactionKind.Withdrawal,
                At = new DateTime(2024, 10, 29, 17, 30, 0, DateTimeKind.Utc)
            },
            // Transactions pour le compte 5 (Sophie Bernard)
            new Transaction
            {
                Id = 11,
                AccountId = 5,
                Amount = 100000.00m,
                Kind = TransactionKind.Deposit,
                At = new DateTime(2024, 10, 1, 9, 0, 0, DateTimeKind.Utc)
            },
            new Transaction
            {
                Id = 12,
                AccountId = 5,
                Amount = 25000.00m,
                Kind = TransactionKind.Withdrawal,
                At = new DateTime(2024, 11, 2, 14, 0, 0, DateTimeKind.Utc)
            },
            new Transaction
            {
                Id = 13,
                AccountId = 5,
                Amount = 15000.00m,
                Kind = TransactionKind.Deposit,
                At = new DateTime(2024, 11, 7, 11, 30, 0, DateTimeKind.Utc)
            }
        );
    }
}
