using DigitalBankLite.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DigitalBankLite.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult GetAccounts()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var accounts = _accountService.GetAccounts(userId);
            return Ok(accounts);
        }

        [HttpGet("{accountId}/transactions")]
        public IActionResult GetTransactions(int accountId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var transactions = _accountService.GetTransactions(accountId, userId);
            return Ok(transactions);
        }
        
        [HttpGet("transactions")]
        public IActionResult GetAllTransactions()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var transactions = _accountService.GetAllTransactions(userId);
            return Ok(transactions);
        }

        [HttpGet("mini-statement")]
        public IActionResult GetMiniStatement()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var transactions = _accountService.GetMiniStatement(userId);
            return Ok(transactions);
        }

        [HttpGet("download-statement")]
        public IActionResult DownloadStatement()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var csvData = _accountService.GetDownloadStatement(userId);
            var fileName = $"Statement_{DateTime.Now:yyyyMMdd}.csv";
            return File(csvData, "text/csv", fileName);
        }

        [HttpPost("{accountId}/deposit")]
        public IActionResult Deposit(int accountId, [FromBody] decimal amount)
        {
             var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
             var result = _accountService.Deposit(accountId, amount, userId);
             
             if (!result.Success)
             {
                 return BadRequest(new { message = result.Message });
             }
             return Ok(new { message = result.Message, newBalance = result.Transaction?.BalanceAfterTxn });
        }
    }
}
