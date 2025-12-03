using BankApi.Application.Commands.Cards;
using BankApi.Application.DTOs;
using BankApi.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class CardsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CardsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Request a new card (Admin only)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> RequestCard([FromBody] RequestCardRequest request)
    {
        try
        {
            var command = new RequestCardCommand(request);
            var result = await _mediator.Send(command);
            return Created($"/api/cards/{result.Id}", result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get card by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var query = new GetCardByIdQuery(id);
            var result = await _mediator.Send(query);
            
            if (result == null)
                return NotFound(new { error = $"Card with ID {id} not found" });
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Block a card permanently (Admin only)
    /// </summary>
    [HttpPut("{id:guid}/block")]
    public async Task<IActionResult> BlockCard(Guid id)
    {
        try
        {
            var command = new BlockCardCommand(id);
            await _mediator.Send(command);
            return Ok(new { message = "Card blocked successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Temporarily block a card (Admin only)
    /// </summary>
    [HttpPut("{id:guid}/block-temporary")]
    public async Task<IActionResult> TemporarilyBlockCard(Guid id)
    {
        try
        {
            var command = new TemporarilyBlockCardCommand(id);
            await _mediator.Send(command);
            return Ok(new { message = "Card temporarily blocked successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Unblock a card (Admin only)
    /// </summary>
    [HttpPut("{id:guid}/unblock")]
    public async Task<IActionResult> UnblockCard(Guid id)
    {
        try
        {
            var command = new UnblockCardCommand(id);
            await _mediator.Send(command);
            return Ok(new { message = "Card unblocked successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Validate CVV
    /// </summary>
    [HttpPost("validate-cvv")]
    [AllowAnonymous]
    public async Task<IActionResult> ValidateCVV([FromBody] ValidateCVVRequest request)
    {
        try
        {
            var command = new ValidateCVVCommand(request);
            var isValid = await _mediator.Send(command);
            return Ok(new { valid = isValid });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
