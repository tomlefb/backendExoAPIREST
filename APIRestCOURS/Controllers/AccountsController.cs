using APIRestCOURS.Service.DTOs;
using APIRestCOURS.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace APIRestCOURS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IBankService _bankService;

    public AccountsController(IBankService bankService)
    {
        _bankService = bankService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
    {
        try
        {
            var account = await _bankService.CreateAccountAsync(request);
            return CreatedAtAction(nameof(GetAccount), new { id = account.Id }, account);
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Utilisateur non trouvé" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAccount(int id)
    {
        var account = await _bankService.GetAccountByIdAsync(id);
        if (account == null)
            return NotFound(new { message = "Compte non trouvé" });

        return Ok(account);
    }

    [HttpPost("{id}/deposit")]
    public async Task<IActionResult> Deposit(int id, [FromBody] decimal amount)
    {
        try
        {
            var request = new TransactionRequest { AccountId = id, Amount = amount };
            var result = await _bankService.CreateTransactionAsync(request);
            return Ok(result);
        }
        catch (Exception)
        {
            return NotFound(new { message = "Compte non trouvé" });
        }
    }

    [HttpPost("{id}/withdraw")]
    public async Task<IActionResult> Withdraw(int id, [FromBody] decimal amount)
    {
        try
        {
            var request = new TransactionRequest { AccountId = id, Amount = -amount };
            var result = await _bankService.CreateTransactionAsync(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}/transactions")]
    public async Task<IActionResult> GetAccountTransactions(int id)
    {
        var transactions = await _bankService.GetTransactionsByAccountIdAsync(id);
        return Ok(transactions);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAccounts()
    {
        var accounts = await _bankService.GetAllAccountsAsync();
        return Ok(accounts);
    }
}
