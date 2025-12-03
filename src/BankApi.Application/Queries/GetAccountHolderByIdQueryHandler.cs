using BankApi.Application.DTOs;
using BankApi.Domain.Repositories;
using Mapster;
using MediatR;

namespace BankApi.Application.Queries;

public class GetAccountHolderByIdQueryHandler : IRequestHandler<GetAccountHolderByIdQuery, AccountHolderDto?>
{
    private readonly IAccountHolderRepository _repository;

    public GetAccountHolderByIdQueryHandler(IAccountHolderRepository repository)
    {
        _repository = repository;
    }

    public async Task<AccountHolderDto?> Handle(GetAccountHolderByIdQuery request, CancellationToken cancellationToken)
    {
        var accountHolder = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return accountHolder?.Adapt<AccountHolderDto>();
    }
}
