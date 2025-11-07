using APIRestCOURS.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace APIRestCOURS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BankController : ControllerBase
{
    private readonly IBankService _bankService;

    public BankController(IBankService bankService)
    {
        _bankService = bankService;
    }

    [HttpGet("accounts/by-owner")]
    public async Task<IActionResult> GetAccountsByOwner()
    {
        var accounts = await _bankService.GetAccountsGroupedByOwnerAsync();
        return Ok(accounts);
    }

    [HttpGet("accounts/rich")]
    public async Task<IActionResult> GetRichAccounts()
    {
        var accounts = await _bankService.GetRichAccountsAsync();
        return Ok(accounts);
    }

    [HttpGet("transactions/recent")]
    public async Task<IActionResult> GetRecentTransactions()
    {
        var transactions = await _bankService.GetLast50TransactionsAsync();
        return Ok(transactions);
    }

    [HttpGet("owners/top-rich")]
    public async Task<IActionResult> GetTopRichOwners()
    {
        var topOwners = await _bankService.GetTop3RichestOwnersAsync();
        return Ok(topOwners.Select(x => new
        {
            ownerName = x.OwnerName,
            totalBalance = x.TotalBalance
        }));
    }

    [HttpGet("accounts/page/{pageNumber}")]
    public async Task<IActionResult> GetAccountsPage(int pageNumber, [FromQuery] int pageSize = 10)
    {
        var accounts = await _bankService.GetAccountsPageAsync(pageNumber, pageSize);
        var total = await _bankService.GetTotalAccountsCountAsync();

        return Ok(new
        {
            page = pageNumber,
            pageSize,
            total,
            totalPages = (int)Math.Ceiling(total / (double)pageSize),
            data = accounts
        });
    }
}
