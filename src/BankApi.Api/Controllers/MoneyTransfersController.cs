using BankApi.Application.Commands.MoneyTransfers;
using BankApi.Application.DTOs;
using BankApi.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MoneyTransfersController : ControllerBase
{
    private readonly IMediator _mediator;

    public MoneyTransfersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Execute a money transfer from your card to another card
    /// </summary>
    /// <param name="sourceCardNumber">The card number to transfer from (query parameter)</param>
    /// <param name="request">The transfer request containing amount and target card number</param>
    [HttpPost("card-transfer")]
    public async Task<IActionResult> ExecuteCardTransfer(
        [FromQuery] string sourceCardNumber,
        [FromBody] ExecuteCardTransferRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(sourceCardNumber))
                return BadRequest(new { error = "Source card number is required" });

            var command = new ExecuteCardTransferCommand(request, sourceCardNumber);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get money transfer by ID (returns full details including status)
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var query = new GetMoneyTransferByIdQuery(id);
            var result = await _mediator.Send(query);
            
            if (result == null)
                return NotFound(new { error = $"Money transfer with ID {id} not found" });
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
