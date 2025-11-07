using APIRestCOURS.DTOs;
using APIRestCOURS.Models;

namespace APIRestCOURS.Services;

public class UserService
{
    private readonly Bank _bank;

    public UserService(Bank bank)
    {
        _bank = bank;
    }

    public User CreateUser(CreateUserRequest request)
    {
        var user = new User
        {
            Id = _bank.GenerateUserId(),
            Nom = request.Nom,
            Prenom = request.Prenom,
            DateNaissance = request.DateNaissance
        };

        _bank.Users.Add(user);
        return user;
    }

    public User? GetUser(int userId)
    {
        return _bank.Users.FirstOrDefault(u => u.Id == userId);
    }

    public List<User> GetAllUsers()
    {
        return _bank.Users.ToList();
    }
}
