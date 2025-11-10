using Microsoft.AspNetCore.Identity;

namespace APIRestCOURS.DataAccess.Models;

public class User : IdentityUser<int>
{
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public DateTime DateNaissance { get; set; }

    // Navigation properties
    public ICollection<Account> Accounts { get; set; } = new List<Account>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
