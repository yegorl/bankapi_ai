using BankApi.Application.Commands.Accounts;
using BankApi.Application.DTOs;
using BankApi.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AccountsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new account (Admin only)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAccountRequest request)
    {
        try
        {
            var command = new CreateAccountCommand(request);
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get account by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var query = new GetAccountByIdQuery(id);
            var result = await _mediator.Send(query);
            
            if (result is null)
                return NotFound();

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get accounts by holder ID
    /// </summary>
    [HttpGet("holder/{holderId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByHolderId(string holderId)
    {
        try
        {
            var query = new GetAccountsByHolderIdQuery(holderId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Update account description (Admin only)
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateDescription(Guid id, [FromBody] UpdateAccountRequest request)
    {
        try
        {
            // Note: This would need UpdateAccountCommand to be created
            return Ok(new { message = "Update account not yet implemented" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Delete account (Admin only)
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            // Note: This would need DeleteAccountCommand to be created
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
