using APIRestCOURS.DTOs;
using APIRestCOURS.Models;

namespace APIRestCOURS.Services;

public class AccountService
{
    private readonly Bank _bank;
    private readonly UserService _userService;

    public AccountService(Bank bank, UserService userService)
    {
        _bank = bank;
        _userService = userService;
    }

    public Account? CreateAccount(CreateAccountRequest request)
    {
        var owner = _userService.GetUser(request.OwnerId);
        if (owner == null) return null;

        var account = new Account
        {
            Id = _bank.GenerateAccountId(),
            OwnerId = request.OwnerId,
            Iban = request.Iban,
            Balance = request.InitialBalance,
            Owner = owner
        };

        _bank.Accounts.Add(account);

        if (request.InitialBalance > 0)
        {
            var transaction = new Transaction
            {
                Id = _bank.GenerateTransactionId(),
                AccountId = account.Id,
                At = DateTime.UtcNow,
                Kind = TransactionKind.Deposit,
                Amount = request.InitialBalance
            };
            _bank.Transactions.Add(transaction);
        }

        return account;
    }

    public Account? GetAccount(int accountId)
    {
        return _bank.Accounts.FirstOrDefault(a => a.Id == accountId);
    }

    public List<Account> GetAllAccounts()
    {
        return _bank.Accounts.ToList();
    }

    public AccountResponse? GetAccountResponse(int accountId)
    {
        var account = GetAccount(accountId);
        if (account == null) return null;

        return new AccountResponse
        {
            Id = account.Id,
            OwnerId = account.OwnerId,
            OwnerName = $"{account.Owner!.Prenom} {account.Owner.Nom}",
            Iban = account.Iban,
            Balance = account.Balance
        };
    }

    public int GetTotalAccountsCount()
    {
        return _bank.Accounts.Count;
    }
}
