using System.Security.Claims;
using Operations.Entities;
using Operations.Interfaces;

namespace ApplicationServices;

public interface IOperationsService
{
    public Task<List<Card>> GetAllUserCards(ClaimsPrincipal user);

    public Task<List<IOperation>> GetAllUserOperations(ClaimsPrincipal user);
    
    public Task<List<IOperation>> GetUserOperationsByCardIds(ClaimsPrincipal user, List<Guid> cardIds);
    
    
}