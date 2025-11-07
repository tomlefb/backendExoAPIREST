namespace APIRestCOURS.Service.DTOs;

public enum TransactionKind
{
    Deposit,
    Withdrawal
}

public class TransactionDto
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public DateTime At { get; set; }
    public TransactionKind Kind { get; set; }
    public decimal Amount { get; set; }
    public decimal NewBalance { get; set; }
}
