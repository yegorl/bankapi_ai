using BankApi.Application.DTOs;
using BankApi.Domain.Repositories;
using BankApi.Domain.ValueObjects;
using Mapster;
using MediatR;

namespace BankApi.Application.Commands.AccountHolders;

public class UpdateAccountHolderCommandHandler : IRequestHandler<UpdateAccountHolderCommand, AccountHolderDto?>
{
    private readonly IAccountHolderRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAccountHolderCommandHandler(IAccountHolderRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AccountHolderDto?> Handle(UpdateAccountHolderCommand request, CancellationToken cancellationToken)
    {
        var accountHolder = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (accountHolder is null)
            return null;

        EmailAddress? email = request.Request.Email is not null ? EmailAddress.Create(request.Request.Email) : null;
        PhoneNumber? phone = request.Request.Phone is not null ? PhoneNumber.Create(request.Request.Phone) : null;
        
        Address? address = null;
        if (request.Request.Address is not null)
        {
            address = Address.Create(
                request.Request.Address.Street,
                request.Request.Address.City,
                request.Request.Address.State,
                request.Request.Address.PostalCode,
                request.Request.Address.Country);
        }

        accountHolder.UpdateContactInfo(email, phone, address);
        await _repository.UpdateAsync(accountHolder, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return accountHolder.Adapt<AccountHolderDto>();
    }
}
