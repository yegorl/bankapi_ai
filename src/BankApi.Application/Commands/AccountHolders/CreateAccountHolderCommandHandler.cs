using BankApi.Application.DTOs;
using BankApi.Domain.Aggregates.AccountHolders;
using BankApi.Domain.Repositories;
using BankApi.Domain.ValueObjects;
using Mapster;
using MediatR;

namespace BankApi.Application.Commands.AccountHolders;

public class CreateAccountHolderCommandHandler : IRequestHandler<CreateAccountHolderCommand, AccountHolderDto>
{
    private readonly IAccountHolderRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAccountHolderCommandHandler(IAccountHolderRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AccountHolderDto> Handle(CreateAccountHolderCommand request, CancellationToken cancellationToken)
    {
        var email = EmailAddress.Create(request.Request.Email);
        var phone = PhoneNumber.Create(request.Request.Phone);
        
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

        var accountHolder = AccountHolder.Create(
            request.Request.FirstName,
            request.Request.LastName,
            email,
            phone,
            request.Request.DateOfBirth,
            address);

        await _repository.AddAsync(accountHolder, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return accountHolder.Adapt<AccountHolderDto>();
    }
}
