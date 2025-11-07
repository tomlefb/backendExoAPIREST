namespace APIRestCOURS.DataAccess.Models;

public class User
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public DateTime DateNaissance { get; set; }

    // Navigation property
    public ICollection<Account> Accounts { get; set; } = new List<Account>();
}
