using BankApi.Application.DTOs;
using BankApi.Domain.Aggregates.Transactions;
using BankApi.Domain.Repositories;
using BankApi.Domain.ValueObjects;
using Mapster;
using MediatR;

namespace BankApi.Application.Commands.Transactions;

public class ExecuteTransferCommandHandler : IRequestHandler<ExecuteTransferCommand, TransactionDto>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ExecuteTransferCommandHandler(
        IAccountRepository accountRepository,
        ITransactionRepository transactionRepository,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TransactionDto> Handle(ExecuteTransferCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        
        try
        {
            var sourceAccount = await _accountRepository.GetByIdAsync(request.Request.SourceAccountId, cancellationToken);
            var targetAccount = await _accountRepository.GetByIdAsync(request.Request.TargetAccountId, cancellationToken);

            if (sourceAccount is null || targetAccount is null)
                throw new InvalidOperationException("One or both accounts not found");

            var amount = Money.Create(request.Request.Amount, sourceAccount.Balance.Currency);

            var transaction = Transaction.Create(
                request.Request.SourceAccountId,
                request.Request.TargetAccountId,
                amount,
                TransactionType.Transfer,
                request.Request.Description);

            sourceAccount.Debit(amount);
            targetAccount.Credit(amount);

            await _accountRepository.UpdateAsync(sourceAccount, cancellationToken);
            await _accountRepository.UpdateAsync(targetAccount, cancellationToken);
            await _transactionRepository.AddAsync(transaction, cancellationToken);

            transaction.Execute();
            await _transactionRepository.UpdateAsync(transaction, cancellationToken);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return transaction.Adapt<TransactionDto>();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
