namespace APIRestCOURS.DataAccess.Models;

public class Account
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public string Iban { get; set; } = string.Empty;
    public decimal Balance { get; set; }

    // Navigation property
    public User? Owner { get; set; }
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
