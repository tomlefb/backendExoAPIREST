namespace APIRestCOURS.Models;

public class Bank
{
    private int _nextUserId = 1;
    private int _nextAccountId = 1;
    private int _nextTransactionId = 1;

    public List<User> Users { get; set; } = new();
    public List<Account> Accounts { get; set; } = new();
    public List<Transaction> Transactions { get; set; } = new();

    public int GenerateUserId() => _nextUserId++;
    public int GenerateAccountId() => _nextAccountId++;
    public int GenerateTransactionId() => _nextTransactionId++;
}
