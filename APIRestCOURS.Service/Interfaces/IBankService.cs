using APIRestCOURS.Service.DTOs;

namespace APIRestCOURS.Service.Interfaces;

public interface IBankService
{
    // User operations
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto> CreateUserAsync(CreateUserRequest request);

    // Account operations
    Task<AccountDto?> GetAccountByIdAsync(int id);
    Task<List<AccountDto>> GetAllAccountsAsync();
    Task<AccountDto> CreateAccountAsync(CreateAccountRequest request);
    Task<int> GetTotalAccountsCountAsync();

    // Transaction operations
    Task<TransactionDto> CreateTransactionAsync(TransactionRequest request);
    Task<List<TransactionDto>> GetTransactionsByAccountIdAsync(int accountId);
    Task<List<TransactionDto>> GetLast50TransactionsAsync();

    // Complex queries
    Task<Dictionary<string, List<AccountDto>>> GetAccountsGroupedByOwnerAsync();
    Task<List<AccountDto>> GetRichAccountsAsync();
    Task<List<AccountDto>> GetAccountsPageAsync(int pageNumber, int pageSize);
    Task<List<TopOwnerDto>> GetTop3RichestOwnersAsync();
}
