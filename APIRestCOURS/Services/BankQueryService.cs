using APIRestCOURS.DTOs;
using APIRestCOURS.Models;

namespace APIRestCOURS.Services;

public class BankQueryService
{
    private readonly Bank _bank;
    private readonly AccountService _accountService;

    public BankQueryService(Bank bank, AccountService accountService)
    {
        _bank = bank;
        _accountService = accountService;
    }

    public Dictionary<string, List<AccountResponse>> GetAccountsGroupedByOwner()
    {
        return _bank.Accounts
            .GroupBy(a => a.Owner!)
            .ToDictionary(
                g => $"{g.Key.Prenom} {g.Key.Nom}",
                g => g.Select(a => new AccountResponse
                {
                    Id = a.Id,
                    OwnerId = a.OwnerId,
                    OwnerName = $"{a.Owner!.Prenom} {a.Owner.Nom}",
                    Iban = a.Iban,
                    Balance = a.Balance
                }).ToList()
            );
    }

    public List<AccountResponse> GetRichAccounts()
    {
        return _bank.Accounts
            .Where(a => a.Balance > 1000)
            .Select(a => new AccountResponse
            {
                Id = a.Id,
                OwnerId = a.OwnerId,
                OwnerName = $"{a.Owner!.Prenom} {a.Owner.Nom}",
                Iban = a.Iban,
                Balance = a.Balance
            })
            .ToList();
    }

    public List<TransactionResponse> GetLast50Transactions()
    {
        return _bank.Transactions
            .OrderByDescending(t => t.At)
            .Take(50)
            .Select(t =>
            {
                var account = _accountService.GetAccount(t.AccountId);
                return new TransactionResponse
                {
                    Id = t.Id,
                    AccountId = t.AccountId,
                    At = t.At,
                    Kind = t.Kind,
                    Amount = t.Amount,
                    NewBalance = account!.Balance
                };
            })
            .ToList();
    }

    public List<(string OwnerName, decimal TotalBalance)> GetTop3RichestOwners()
    {
        return _bank.Accounts
            .GroupBy(a => a.Owner!)
            .Select(g => new
            {
                OwnerName = $"{g.Key.Prenom} {g.Key.Nom}",
                TotalBalance = g.Sum(a => a.Balance)
            })
            .OrderByDescending(x => x.TotalBalance)
            .Take(3)
            .Select(x => (x.OwnerName, x.TotalBalance))
            .ToList();
    }

    public List<AccountResponse> GetAccountsPage(int pageNumber, int pageSize = 10)
    {
        return _bank.Accounts
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new AccountResponse
            {
                Id = a.Id,
                OwnerId = a.OwnerId,
                OwnerName = $"{a.Owner!.Prenom} {a.Owner.Nom}",
                Iban = a.Iban,
                Balance = a.Balance
            })
            .ToList();
    }
}
