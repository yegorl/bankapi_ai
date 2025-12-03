using BankApi.Domain.Repositories;
using MediatR;

namespace BankApi.Application.Commands.AccountHolders;

public class DeleteAccountHolderCommandHandler : IRequestHandler<DeleteAccountHolderCommand, bool>
{
    private readonly IAccountHolderRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAccountHolderCommandHandler(IAccountHolderRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteAccountHolderCommand request, CancellationToken cancellationToken)
    {
        var accountHolder = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (accountHolder is null)
            return false;

        accountHolder.MarkAsDeleted();
        await _repository.UpdateAsync(accountHolder, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
