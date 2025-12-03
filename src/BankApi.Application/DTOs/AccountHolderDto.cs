namespace BankApi.Application.DTOs;

public record AccountHolderDto(
    string Id,
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    DateTime DateOfBirth,
    AddressDto? Address,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record AddressDto(
    string Street,
    string City,
    string State,
    string PostalCode,
    string Country);

public record CreateAccountHolderRequest(
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    DateTime DateOfBirth,
    AddressDto? Address);

public record UpdateAccountHolderRequest(
    string? Email,
    string? Phone,
    AddressDto? Address);
