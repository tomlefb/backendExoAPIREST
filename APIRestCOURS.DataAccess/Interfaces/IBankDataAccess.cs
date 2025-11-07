using APIRestCOURS.DataAccess.Models;

namespace APIRestCOURS.DataAccess.Interfaces;

public interface IBankDataAccess
{
    // User operations
    Task<User?> GetUserByIdAsync(int id);
    Task<List<User>> GetAllUsersAsync();
    Task<User> CreateUserAsync(User user);

    // Account operations
    Task<Account?> GetAccountByIdAsync(int id);
    Task<List<Account>> GetAllAccountsAsync();
    Task<Account> CreateAccountAsync(Account account);
    Task UpdateAccountAsync(Account account);
    Task<int> GetTotalAccountsCountAsync();

    // Transaction operations
    Task<Transaction> CreateTransactionAsync(Transaction transaction);
    Task<List<Transaction>> GetTransactionsByAccountIdAsync(int accountId);
    Task<List<Transaction>> GetLast50TransactionsAsync();

    // Complex queries
    Task<Dictionary<string, List<Account>>> GetAccountsGroupedByOwnerAsync();
    Task<List<Account>> GetRichAccountsAsync();
    Task<List<Account>> GetAccountsPageAsync(int pageNumber, int pageSize);
    Task<List<(string OwnerName, decimal TotalBalance)>> GetTop3RichestOwnersAsync();
}
