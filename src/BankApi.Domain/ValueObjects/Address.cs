using BankApi.Domain.Common;
using BankApi.Domain.Exceptions;

namespace BankApi.Domain.ValueObjects;

/// <summary>
/// Represents a physical address
/// </summary>
public sealed class Address : ValueObject
{
    public string Street { get; }
    public string City { get; }
    public string State { get; }
    public string PostalCode { get; }
    public string Country { get; }

    private Address(string street, string city, string state, string postalCode, string country)
    {
        Street = street;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
    }

    /// <summary>
    /// Creates an Address with validation
    /// </summary>
    public static Address Create(string street, string city, string state, string postalCode, string country)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ValidationException("Street cannot be empty");

        if (string.IsNullOrWhiteSpace(city))
            throw new ValidationException("City cannot be empty");

        if (string.IsNullOrWhiteSpace(state))
            throw new ValidationException("State cannot be empty");

        if (string.IsNullOrWhiteSpace(postalCode))
            throw new ValidationException("Postal code cannot be empty");

        if (string.IsNullOrWhiteSpace(country))
            throw new ValidationException("Country cannot be empty");

        return new Address(street, city, state, postalCode, country.ToUpperInvariant());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return PostalCode;
        yield return Country;
    }

    public override string ToString() => $"{Street}, {City}, {State} {PostalCode}, {Country}";
}
