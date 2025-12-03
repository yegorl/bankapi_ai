using BankApi.Application.Commands.AccountHolders;
using BankApi.Application.DTOs;
using BankApi.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AccountHoldersController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountHoldersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new account holder (Admin only)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAccountHolderRequest request)
    {
        try
        {
            var command = new CreateAccountHolderCommand(request);
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get account holder by ID
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(string id)
    {
        try
        {
            var query = new GetAccountHolderByIdQuery(id);
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
    /// Update account holder (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateAccountHolderRequest request)
    {
        try
        {
            var command = new UpdateAccountHolderCommand(id, request);
            var result = await _mediator.Send(command);
            
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
    /// Delete account holder (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            var command = new DeleteAccountHolderCommand(id);
            var result = await _mediator.Send(command);
            
            if (!result)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
