using BankApi.Application.DTOs;
using BankApi.Domain.Aggregates.MoneyTransfers;
using BankApi.Domain.Repositories;
using BankApi.Domain.ValueObjects;
using Mapster;
using MediatR;

namespace BankApi.Application.Commands.MoneyTransfers;

public class ExecuteCardTransferCommandHandler : IRequestHandler<ExecuteCardTransferCommand, MoneyTransferDto>
{
    private readonly ICardRepository _cardRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IMoneyTransferRepository _moneyTransferRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ExecuteCardTransferCommandHandler(
        ICardRepository cardRepository,
        IAccountRepository accountRepository,
        IMoneyTransferRepository moneyTransferRepository,
        IUnitOfWork unitOfWork)
    {
        _cardRepository = cardRepository;
        _accountRepository = accountRepository;
        _moneyTransferRepository = moneyTransferRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<MoneyTransferDto> Handle(ExecuteCardTransferCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        
        try
        {
            // Validate source card
            var sourceCard = await _cardRepository.GetByCardNumberAsync(request.Request.SourceCardNumber, cancellationToken);
            if (sourceCard is null)
                throw new InvalidOperationException($"Source card {request.Request.SourceCardNumber} not found");

            if (!sourceCard.IsUsable())
                throw new InvalidOperationException($"Source card is not usable (blocked, deleted, or expired)");

            // Validate target card
            var targetCard = await _cardRepository.GetByCardNumberAsync(request.Request.TargetCardNumber, cancellationToken);
            if (targetCard is null)
                throw new InvalidOperationException($"Target card {request.Request.TargetCardNumber} not found");

            if (!targetCard.IsUsable())
                throw new InvalidOperationException($"Target card is not usable (blocked, deleted, or expired)");

            // Ensure cards are different
            if (sourceCard.CardNumber.Value == targetCard.CardNumber.Value)
                throw new InvalidOperationException("Source and target cards cannot be the same");

            // Get accounts
            var sourceAccount = await _accountRepository.GetByIdAsync(sourceCard.AccountId, cancellationToken);
            var targetAccount = await _accountRepository.GetByIdAsync(targetCard.AccountId, cancellationToken);

            if (sourceAccount is null || targetAccount is null)
                throw new InvalidOperationException("One or both accounts not found");

            // Ensure accounts are different
            if (sourceAccount.Id == targetAccount.Id)
                throw new InvalidOperationException("Source and target accounts cannot be the same");

            // Ensure same currency
            if (sourceAccount.Balance.Currency != targetAccount.Balance.Currency)
                throw new InvalidOperationException(
                    $"Currency mismatch: source account is {sourceAccount.Balance.Currency}, " +
                    $"target account is {targetAccount.Balance.Currency}");

            // Create money amount
            var amount = Money.Create(request.Request.Amount, sourceAccount.Balance.Currency);

            // Create money transfer
            var moneyTransfer = MoneyTransfer.Create(
                sourceCard.CardNumber.Value,
                targetCard.CardNumber.Value,
                sourceAccount.Id,
                targetAccount.Id,
                amount,
                request.Request.Description);

            // Perform the transfer
            sourceAccount.Debit(amount);
            targetAccount.Credit(amount);

            // Save updates
            await _accountRepository.UpdateAsync(sourceAccount, cancellationToken);
            await _accountRepository.UpdateAsync(targetAccount, cancellationToken);
            await _moneyTransferRepository.AddAsync(moneyTransfer, cancellationToken);

            // Mark transfer as executed
            moneyTransfer.Execute();
            await _moneyTransferRepository.UpdateAsync(moneyTransfer, cancellationToken);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return moneyTransfer.Adapt<MoneyTransferDto>();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
