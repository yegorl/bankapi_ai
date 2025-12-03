using BankApi.Application.DTOs;
using MediatR;

namespace BankApi.Application.Commands.Cards;

public record ValidateCVVCommand(ValidateCVVRequest Request) : IRequest<bool>;
