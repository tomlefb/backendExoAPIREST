using APIRestCOURS.DTOs;
using APIRestCOURS.Models;

namespace APIRestCOURS.Services;

public class TransactionService
{
    private readonly Bank _bank;
    private readonly AccountService _accountService;

    public TransactionService(Bank bank, AccountService accountService)
    {
        _bank = bank;
        _accountService = accountService;
    }

    public TransactionResponse? Deposit(TransactionRequest request)
    {
        var account = _accountService.GetAccount(request.AccountId);
        if (account == null) return null;

        account.Balance += request.Amount;

        var transaction = new Transaction
        {
            Id = _bank.GenerateTransactionId(),
            AccountId = account.Id,
            At = DateTime.UtcNow,
            Kind = TransactionKind.Deposit,
            Amount = request.Amount
        };

        _bank.Transactions.Add(transaction);

        return new TransactionResponse
        {
            Id = transaction.Id,
            AccountId = transaction.AccountId,
            At = transaction.At,
            Kind = transaction.Kind,
            Amount = transaction.Amount,
            NewBalance = account.Balance
        };
    }

    public TransactionResponse? Withdraw(TransactionRequest request)
    {
        var account = _accountService.GetAccount(request.AccountId);
        if (account == null) return null;
        if (account.Balance < request.Amount)
            throw new InvalidOperationException("Fonds insuffisants");

        account.Balance -= request.Amount;

        var transaction = new Transaction
        {
            Id = _bank.GenerateTransactionId(),
            AccountId = account.Id,
            At = DateTime.UtcNow,
            Kind = TransactionKind.Withdrawal,
            Amount = request.Amount
        };

        _bank.Transactions.Add(transaction);

        return new TransactionResponse
        {
            Id = transaction.Id,
            AccountId = transaction.AccountId,
            At = transaction.At,
            Kind = transaction.Kind,
            Amount = transaction.Amount,
            NewBalance = account.Balance
        };
    }

    public List<Transaction> GetAllTransactions()
    {
        return _bank.Transactions.ToList();
    }

    public List<Transaction> GetTransactionsByAccount(int accountId)
    {
        return _bank.Transactions
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.At)
            .ToList();
    }
}
