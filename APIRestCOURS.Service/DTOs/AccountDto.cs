namespace APIRestCOURS.Service.DTOs;

public class AccountDto
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public string Iban { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}
