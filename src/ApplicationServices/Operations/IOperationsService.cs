using System.Security.Claims;
using Operations.Dtos;
using Operations.Entities;
using Operations.Interfaces;

namespace ApplicationServices.Operations;

public interface IOperationsService
{
    public Task<List<Card>> GetAllUserCards(ClaimsPrincipal user);

    public Task<List<IOperationRecord>> GetAllUserOperations(ClaimsPrincipal user);
    
    public Task<List<IOperationRecord>> GetUserOperationsByCardIds(ClaimsPrincipal user, List<Guid> cardIds);

    public Task CreatePayment(ClaimsPrincipal user, CreatePaymentRecordDto recordDto);
    
    public Task CreateIncome(ClaimsPrincipal user, CreateIncomeRecordDto recordDto);
    
    public Task UpdatePayment(ClaimsPrincipal user, UpdatePaymentRecordDto recordDto);
    
    public Task UpdateIncome(ClaimsPrincipal user, UpdateIncomeRecordDto recordDto);
    
    public Task DeletePayment(ClaimsPrincipal user, Guid paymentId);
    
    public Task DeleteIncome(ClaimsPrincipal user, Guid incomeId);

    public Task CreateCard(ClaimsPrincipal user, CreateCardDto dto);

    public Task UpdateCard(ClaimsPrincipal user, UpdateCardDto dto);

    public Task DeleteCard(ClaimsPrincipal user, Guid cardId);
}