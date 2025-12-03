using BankApi.Application.DTOs;
using BankApi.Domain.Repositories;
using Mapster;
using MediatR;

namespace BankApi.Application.Queries;

public class GetAccountsByHolderIdQueryHandler : IRequestHandler<GetAccountsByHolderIdQuery, IEnumerable<AccountDto>>
{
    private readonly IAccountRepository _repository;

    public GetAccountsByHolderIdQueryHandler(IAccountRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<AccountDto>> Handle(GetAccountsByHolderIdQuery request, CancellationToken cancellationToken)
    {
        var accounts = await _repository.GetByHolderIdAsync(request.HolderId, cancellationToken);
        return accounts.Adapt<IEnumerable<AccountDto>>();
    }
}
