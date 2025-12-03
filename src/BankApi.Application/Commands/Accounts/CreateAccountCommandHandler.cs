using BankApi.Application.DTOs;
using BankApi.Domain.Aggregates.Accounts;
using BankApi.Domain.Repositories;
using Mapster;
using MediatR;

namespace BankApi.Application.Commands.Accounts;

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, AccountDto>
{
    private readonly IAccountRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAccountCommandHandler(IAccountRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AccountDto> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = Account.Create(
            request.Request.AccountHolderId,
            request.Request.Currency,
            request.Request.Description);

        await _repository.AddAsync(account, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return account.Adapt<AccountDto>();
    }
}
