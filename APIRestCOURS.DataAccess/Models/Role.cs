using Microsoft.AspNetCore.Identity;

namespace APIRestCOURS.DataAccess.Models;

public class Role : IdentityRole<int>
{
    public Role() : base()
    {
    }

    public Role(string roleName) : base(roleName)
    {
    }
}
