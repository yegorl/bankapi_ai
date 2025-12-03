using BankApi.Application.DTOs;
using BankApi.Domain.Aggregates.MoneyTransfers;
using BankApi.Domain.Aggregates.Transactions;
using BankApi.Domain.Repositories;
using BankApi.Domain.ValueObjects;
using Mapster;
using MediatR;

namespace BankApi.Application.Commands.MoneyTransfers;

public class ExecuteCardTransferCommandHandler : IRequestHandler<ExecuteCardTransferCommand, MoneyTransferResponse>
{
    private readonly ICardRepository _cardRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IMoneyTransferRepository _moneyTransferRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ExecuteCardTransferCommandHandler(
        ICardRepository cardRepository,
        IAccountRepository accountRepository,
        IMoneyTransferRepository moneyTransferRepository,
        ITransactionRepository transactionRepository,
        IUnitOfWork unitOfWork)
    {
        _cardRepository = cardRepository;
        _accountRepository = accountRepository;
        _moneyTransferRepository = moneyTransferRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<MoneyTransferResponse> Handle(ExecuteCardTransferCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        
        try
        {
            // Validate amount
            if (request.Request.Amount <= 0)
                throw new InvalidOperationException("Transfer amount must be positive");

            // Validate source card via repository
            var sourceCard = await _cardRepository.GetByCardNumberAsync(request.SourceCardNumber, cancellationToken);
            if (sourceCard is null)
                throw new InvalidOperationException($"Source card not found");

            if (!sourceCard.IsUsable())
                throw new InvalidOperationException($"Source card is not usable (blocked, deleted, or expired)");

            // Validate target card via repository
            var targetCard = await _cardRepository.GetByCardNumberAsync(request.Request.CardNumber, cancellationToken);
            if (targetCard is null)
                throw new InvalidOperationException($"Target card not found");

            if (!targetCard.IsUsable())
                throw new InvalidOperationException($"Target card is not usable (blocked, deleted, or expired)");

            // Ensure cards are different
            if (sourceCard.CardNumber.Value == targetCard.CardNumber.Value)
                throw new InvalidOperationException("Source and target cards cannot be the same");

            // Get accounts via repository
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

            // Create money transfer (adds to table immediately)
            var moneyTransfer = MoneyTransfer.Create(
                sourceCard.CardNumber.Value,
                targetCard.CardNumber.Value,
                sourceAccount.Id,
                targetAccount.Id,
                amount,
                $"Card-to-card transfer");

            // Add money transfer to database
            await _moneyTransferRepository.AddAsync(moneyTransfer, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Create corresponding Transaction
            var transaction = Transaction.Create(
                sourceAccount.Id,
                targetAccount.Id,
                amount,
                TransactionType.Transfer,
                $"Card transfer from {sourceCard.CardNumber.Value} to {targetCard.CardNumber.Value}");

            await _transactionRepository.AddAsync(transaction, cancellationToken);

            // Perform the transfer
            sourceAccount.Debit(amount);
            targetAccount.Credit(amount);

            // Mark both as executed
            moneyTransfer.Execute();
            transaction.Execute();

            // Save all updates
            await _accountRepository.UpdateAsync(sourceAccount, cancellationToken);
            await _accountRepository.UpdateAsync(targetAccount, cancellationToken);
            await _moneyTransferRepository.UpdateAsync(moneyTransfer, cancellationToken);
            await _transactionRepository.UpdateAsync(transaction, cancellationToken);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return new MoneyTransferResponse(moneyTransfer.Id, moneyTransfer.Status.ToString());
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
