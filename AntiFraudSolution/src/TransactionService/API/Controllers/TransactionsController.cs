using Microsoft.AspNetCore.Mvc;

using TransactionService.Application.DTOs;
using TransactionService.Application.Interfaces;

namespace TransactionService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionRequest request)
    {
        var createdTransaction = await _transactionService.CreateTransactionAsync(request);
        return CreatedAtAction(nameof(GetTransactionById), new { id = createdTransaction.Id }, createdTransaction);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTransactionById(Guid id)
    {
        var transaction = await _transactionService.GetTransactionByIdAsync(id);
        if (transaction == null)
            return NotFound();

        return Ok(transaction);
    }
}
