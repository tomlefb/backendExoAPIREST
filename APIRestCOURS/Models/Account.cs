namespace APIRestCOURS.Models;

public class Account
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public string Iban { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public User? Owner { get; set; }
}
