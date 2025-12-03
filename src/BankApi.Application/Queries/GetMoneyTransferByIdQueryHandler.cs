using BankApi.Application.DTOs;
using BankApi.Domain.Repositories;
using Mapster;
using MediatR;

namespace BankApi.Application.Queries;

public class GetMoneyTransferByIdQueryHandler : IRequestHandler<GetMoneyTransferByIdQuery, MoneyTransferDto?>
{
    private readonly IMoneyTransferRepository _moneyTransferRepository;

    public GetMoneyTransferByIdQueryHandler(IMoneyTransferRepository moneyTransferRepository)
    {
        _moneyTransferRepository = moneyTransferRepository;
    }

    public async Task<MoneyTransferDto?> Handle(GetMoneyTransferByIdQuery request, CancellationToken cancellationToken)
    {
        var moneyTransfer = await _moneyTransferRepository.GetByIdAsync(request.Id, cancellationToken);
        return moneyTransfer?.Adapt<MoneyTransferDto>();
    }
}
