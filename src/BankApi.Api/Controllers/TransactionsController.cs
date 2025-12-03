using BankApi.Application.Commands.Transactions;
using BankApi.Application.DTOs;
using BankApi.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransactionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Execute a transfer between accounts
    /// </summary>
    [HttpPost("transfer")]
    public async Task<IActionResult> ExecuteTransfer([FromBody] ExecuteTransferRequest request)
    {
        try
        {
            var command = new ExecuteTransferCommand(request);
            var result = await _mediator.Send(command);
            return Created($"/api/transactions/{result.Id}", result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get transaction by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var query = new GetTransactionByIdQuery(id);
            var result = await _mediator.Send(query);
            
            if (result == null)
                return NotFound(new { error = $"Transaction with ID {id} not found" });
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get account statement
    /// </summary>
    [HttpGet("statement")]
    public async Task<IActionResult> GetStatement([FromQuery] Guid accountId, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        try
        {
            var query = new GetAccountStatementQuery(accountId, from, to);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
