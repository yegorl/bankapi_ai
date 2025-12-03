using BankApi.Application.DTOs;
using MediatR;

namespace BankApi.Application.Commands.Cards;

public record RequestCardCommand(RequestCardRequest Request) : IRequest<CardDto>;
