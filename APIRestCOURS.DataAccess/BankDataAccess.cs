using APIRestCOURS.DataAccess.Interfaces;
using APIRestCOURS.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace APIRestCOURS.DataAccess;

public class BankDataAccess : IBankDataAccess
{
    private readonly BankDbContext _context;

    public BankDataAccess(BankDbContext context)
    {
        _context = context;
    }

    // User operations
    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User> CreateUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    // Account operations
    public async Task<Account?> GetAccountByIdAsync(int id)
    {
        return await _context.Accounts
            .Include(a => a.Owner)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<Account>> GetAllAccountsAsync()
    {
        return await _context.Accounts
            .Include(a => a.Owner)
            .ToListAsync();
    }

    public async Task<Account> CreateAccountAsync(Account account)
    {
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();
        return account;
    }

    public async Task UpdateAccountAsync(Account account)
    {
        _context.Accounts.Update(account);
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetTotalAccountsCountAsync()
    {
        return await _context.Accounts.CountAsync();
    }

    // Transaction operations
    public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }

    public async Task<List<Transaction>> GetTransactionsByAccountIdAsync(int accountId)
    {
        return await _context.Transactions
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.At)
            .ToListAsync();
    }

    public async Task<List<Transaction>> GetLast50TransactionsAsync()
    {
        return await _context.Transactions
            .OrderByDescending(t => t.At)
            .Take(50)
            .ToListAsync();
    }

    // Complex queries
    public async Task<Dictionary<string, List<Account>>> GetAccountsGroupedByOwnerAsync()
    {
        var accounts = await _context.Accounts
            .Include(a => a.Owner)
            .ToListAsync();

        return accounts
            .GroupBy(a => $"{a.Owner?.Prenom} {a.Owner?.Nom}")
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    public async Task<List<Account>> GetRichAccountsAsync()
    {
        return await _context.Accounts
            .Include(a => a.Owner)
            .Where(a => a.Balance > 100000)
            .ToListAsync();
    }

    public async Task<List<Account>> GetAccountsPageAsync(int pageNumber, int pageSize)
    {
        return await _context.Accounts
            .Include(a => a.Owner)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<(string OwnerName, decimal TotalBalance)>> GetTop3RichestOwnersAsync()
    {
        var grouped = await _context.Accounts
            .Include(a => a.Owner)
            .GroupBy(a => new { a.OwnerId, a.Owner!.Prenom, a.Owner.Nom })
            .Select(g => new
            {
                OwnerName = g.Key.Prenom + " " + g.Key.Nom,
                TotalBalance = g.Sum(a => a.Balance)
            })
            .ToListAsync();

        return grouped
            .OrderByDescending(x => x.TotalBalance)
            .Take(3)
            .Select(x => (x.OwnerName, x.TotalBalance))
            .ToList();
    }
}
