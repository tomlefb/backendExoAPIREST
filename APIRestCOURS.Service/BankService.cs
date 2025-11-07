using APIRestCOURS.DataAccess.Interfaces;
using APIRestCOURS.DataAccess.Models;
using APIRestCOURS.Service.DTOs;
using APIRestCOURS.Service.Interfaces;

namespace APIRestCOURS.Service;

public class BankService : IBankService
{
    private readonly IBankDataAccess _dataAccess;

    public BankService(IBankDataAccess dataAccess)
    {
        _dataAccess = dataAccess;
    }

    // User operations
    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _dataAccess.GetUserByIdAsync(id);
        return user == null ? null : MapToUserDto(user);
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _dataAccess.GetAllUsersAsync();
        return users.Select(MapToUserDto).ToList();
    }

    public async Task<UserDto> CreateUserAsync(CreateUserRequest request)
    {
        var user = new User
        {
            Nom = request.Nom,
            Prenom = request.Prenom,
            DateNaissance = request.DateNaissance
        };

        var createdUser = await _dataAccess.CreateUserAsync(user);
        return MapToUserDto(createdUser);
    }

    // Account operations
    public async Task<AccountDto?> GetAccountByIdAsync(int id)
    {
        var account = await _dataAccess.GetAccountByIdAsync(id);
        return account == null ? null : MapToAccountDto(account);
    }

    public async Task<List<AccountDto>> GetAllAccountsAsync()
    {
        var accounts = await _dataAccess.GetAllAccountsAsync();
        return accounts.Select(MapToAccountDto).ToList();
    }

    public async Task<AccountDto> CreateAccountAsync(CreateAccountRequest request)
    {
        var account = new Account
        {
            OwnerId = request.OwnerId,
            Iban = request.Iban,
            Balance = request.InitialBalance
        };

        var createdAccount = await _dataAccess.CreateAccountAsync(account);

        // Reload to get owner info
        var accountWithOwner = await _dataAccess.GetAccountByIdAsync(createdAccount.Id);
        return MapToAccountDto(accountWithOwner!);
    }

    public async Task<int> GetTotalAccountsCountAsync()
    {
        return await _dataAccess.GetTotalAccountsCountAsync();
    }

    // Transaction operations
    public async Task<TransactionDto> CreateTransactionAsync(TransactionRequest request)
    {
        var account = await _dataAccess.GetAccountByIdAsync(request.AccountId);
        if (account == null)
        {
            throw new InvalidOperationException("Account not found");
        }

        var kind = request.Amount > 0 ? DataAccess.Models.TransactionKind.Deposit : DataAccess.Models.TransactionKind.Withdrawal;
        var transaction = new Transaction
        {
            AccountId = request.AccountId,
            At = DateTime.UtcNow,
            Kind = kind,
            Amount = Math.Abs(request.Amount)
        };

        // Update account balance
        account.Balance += request.Amount;
        await _dataAccess.UpdateAccountAsync(account);

        var createdTransaction = await _dataAccess.CreateTransactionAsync(transaction);

        return new TransactionDto
        {
            Id = createdTransaction.Id,
            AccountId = createdTransaction.AccountId,
            At = createdTransaction.At,
            Kind = (DTOs.TransactionKind)createdTransaction.Kind,
            Amount = createdTransaction.Amount,
            NewBalance = account.Balance
        };
    }

    public async Task<List<TransactionDto>> GetTransactionsByAccountIdAsync(int accountId)
    {
        var transactions = await _dataAccess.GetTransactionsByAccountIdAsync(accountId);
        return transactions.Select(t => new TransactionDto
        {
            Id = t.Id,
            AccountId = t.AccountId,
            At = t.At,
            Kind = (DTOs.TransactionKind)t.Kind,
            Amount = t.Amount,
            NewBalance = 0 // Can't calculate without full history
        }).ToList();
    }

    public async Task<List<TransactionDto>> GetLast50TransactionsAsync()
    {
        var transactions = await _dataAccess.GetLast50TransactionsAsync();
        return transactions.Select(t => new TransactionDto
        {
            Id = t.Id,
            AccountId = t.AccountId,
            At = t.At,
            Kind = (DTOs.TransactionKind)t.Kind,
            Amount = t.Amount,
            NewBalance = 0
        }).ToList();
    }

    // Complex queries
    public async Task<Dictionary<string, List<AccountDto>>> GetAccountsGroupedByOwnerAsync()
    {
        var grouped = await _dataAccess.GetAccountsGroupedByOwnerAsync();
        return grouped.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Select(MapToAccountDto).ToList()
        );
    }

    public async Task<List<AccountDto>> GetRichAccountsAsync()
    {
        var accounts = await _dataAccess.GetRichAccountsAsync();
        return accounts.Select(MapToAccountDto).ToList();
    }

    public async Task<List<AccountDto>> GetAccountsPageAsync(int pageNumber, int pageSize)
    {
        var accounts = await _dataAccess.GetAccountsPageAsync(pageNumber, pageSize);
        return accounts.Select(MapToAccountDto).ToList();
    }

    public async Task<List<TopOwnerDto>> GetTop3RichestOwnersAsync()
    {
        var topOwners = await _dataAccess.GetTop3RichestOwnersAsync();
        return topOwners.Select(o => new TopOwnerDto
        {
            OwnerName = o.OwnerName,
            TotalBalance = o.TotalBalance
        }).ToList();
    }

    // Mapping helpers
    private static UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Nom = user.Nom,
            Prenom = user.Prenom,
            DateNaissance = user.DateNaissance
        };
    }

    private static AccountDto MapToAccountDto(Account account)
    {
        return new AccountDto
        {
            Id = account.Id,
            OwnerId = account.OwnerId,
            OwnerName = account.Owner != null ? $"{account.Owner.Prenom} {account.Owner.Nom}" : "",
            Iban = account.Iban,
            Balance = account.Balance
        };
    }
}
