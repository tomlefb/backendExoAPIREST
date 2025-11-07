using APIRestCOURS.Service.DTOs;
using APIRestCOURS.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace APIRestCOURS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IBankService _bankService;

    public UsersController(IBankService bankService)
    {
        _bankService = bankService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var user = await _bankService.CreateUserAsync(request);
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _bankService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound(new { message = "Utilisateur non trouvé" });

        return Ok(user);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _bankService.GetAllUsersAsync();
        return Ok(users);
    }
}
