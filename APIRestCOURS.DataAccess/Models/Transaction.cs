namespace APIRestCOURS.DataAccess.Models;

public enum TransactionKind
{
    Deposit,
    Withdrawal
}

public class Transaction
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public DateTime At { get; set; }
    public TransactionKind Kind { get; set; }
    public decimal Amount { get; set; }

    // Navigation property
    public Account? Account { get; set; }
}
