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
    /// Execute a money transfer between cards
    /// </summary>
    [HttpPost("card-transfer")]
    public async Task<IActionResult> ExecuteCardTransfer([FromBody] ExecuteCardTransferRequest request)
    {
        try
        {
            var command = new ExecuteCardTransferCommand(request);
            var result = await _mediator.Send(command);
            return Created($"/api/moneytransfers/{result.Id}", result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get money transfer by ID
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
