namespace APIRestCOURS.DataAccess.Interfaces;

public interface IDbSeeder
{
    Task SeedAsync();
    Task ClearAsync();
}
