using System.Security.Claims;
using Operations.Dtos;
using Operations.Entities;
using Operations.Interfaces;

namespace ApplicationServices.Operations;

public interface IOperationsService
{
    public Task<List<Card>> GetAllUserCards(ClaimsPrincipal user);

    public Task<List<IOperation>> GetAllUserOperations(ClaimsPrincipal user);
    
    public Task<List<IOperation>> GetUserOperationsByCardIds(ClaimsPrincipal user, List<Guid> cardIds);

    public Task CreatePayment(ClaimsPrincipal user, CreatePaymentDto dto);
    
    public Task CreateIncome(ClaimsPrincipal user, CreateIncomeDto dto);
    
    public Task UpdatePayment(ClaimsPrincipal user, UpdatePaymentDto dto);
    
    public Task UpdateIncome(ClaimsPrincipal user, UpdateIncomeDto dto);
    
    public Task DeletePayment(ClaimsPrincipal user, Guid paymentId);
    
    public Task DeleteIncome(ClaimsPrincipal user, Guid incomeId);

    public Task CreateCard(ClaimsPrincipal user, CreateCardDto dto);

    public Task UpdateCard(ClaimsPrincipal user, UpdateCardDto dto);

    public Task DeleteCard(ClaimsPrincipal user, Guid cardId);
}