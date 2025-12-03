using BankApi.Application.DTOs;
using BankApi.Domain.Repositories;
using Mapster;
using MediatR;

namespace BankApi.Application.Queries;

public class GetAccountByIdQueryHandler : IRequestHandler<GetAccountByIdQuery, AccountDto?>
{
    private readonly IAccountRepository _repository;

    public GetAccountByIdQueryHandler(IAccountRepository repository)
    {
        _repository = repository;
    }

    public async Task<AccountDto?> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
    {
        var account = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return account?.Adapt<AccountDto>();
    }
}
