using BankApi.Application.DTOs;
using MediatR;

namespace BankApi.Application.Queries;

public record GetCardByIdQuery(Guid CardId) : IRequest<CardDto?>;
