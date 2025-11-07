namespace APIRestCOURS.DTOs;

public class CreateAccountRequest
{
    public int OwnerId { get; set; }
    public string Iban { get; set; } = string.Empty;
    public decimal InitialBalance { get; set; } = 0;
}
