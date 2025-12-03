using BankApi.Application.Commands.Transactions;
using BankApi.Application.DTOs;
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
    /// Create a transaction
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTransactionRequest request)
    {
        try
        {
            // Note: This would need CreateTransactionCommand to be created
            return Ok(new { message = "Create transaction functionality to be implemented" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
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
            return Ok(result);
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
            // Note: This would need GetTransactionByIdQuery to be created
            return Ok(new { message = "Get transaction functionality to be implemented" });
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
            // Note: This would need GetAccountStatementQuery to be created
            return Ok(new { message = "Get statement functionality to be implemented" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
