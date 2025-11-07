using APIRestCOURS.Models;

namespace APIRestCOURS.DTOs;

public class TransactionResponse
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public DateTime At { get; set; }
    public TransactionKind Kind { get; set; }
    public decimal Amount { get; set; }
    public decimal NewBalance { get; set; }
}
