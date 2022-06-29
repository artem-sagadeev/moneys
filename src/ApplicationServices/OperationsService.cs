using System.Security.Claims;
using Common.Exceptions;
using Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Operations.Entities;
using Operations.Interfaces;
using Operations.Logic.Cards;
using Operations.Logic.Incomes;
using Operations.Logic.Payments;

namespace ApplicationServices;

public class OperationsService : IOperationsService
{
    private readonly IPaymentService _paymentService;
    private readonly IIncomeService _incomeService;
    private readonly ICardService _cardService;
    private readonly UserManager<User> _userManager;

    public OperationsService(IPaymentService paymentService, 
        IIncomeService incomeService, ICardService cardService, UserManager<User> userManager)
    {
        _paymentService = paymentService;
        _incomeService = incomeService;
        _cardService = cardService;
        _userManager = userManager;
    }


    public async Task<List<Card>> GetAllUserCards(ClaimsPrincipal user)
    {
        var currentUser = await _userManager.GetUserAsync(user);
        var userCards = await _cardService.GetByUserId(currentUser.Id);

        return userCards;
    }

    public async Task<List<IOperation>> GetAllUserOperations(ClaimsPrincipal user)
    {
        var userCards = await GetAllUserCards(user);
        var userCardIds = userCards.Select(card => card.Id).ToList();
        
        var payments = await _paymentService.GetByCardIds(userCardIds);
        var incomes = await _incomeService.GetByCardIds(userCardIds);

        var operations = payments
            .Select(payment => (IOperation)payment)
            .Concat(incomes)
            .ToList();

        return operations;
    }

    public async Task<List<IOperation>> GetUserOperationsByCardIds(ClaimsPrincipal user, List<Guid> cardIds)
    {
        var userCards = await GetAllUserCards(user);
        var userCardIds = userCards.Select(card => card.Id).ToList();
        if (cardIds.Any(cardId => !userCardIds.Contains(cardId)))
            throw new NoAccessException();
        
        var payments = await _paymentService.GetByCardIds(cardIds);
        var incomes = await _incomeService.GetByCardIds(cardIds);

        var operations = payments
            .Select(payment => (IOperation)payment)
            .Concat(incomes)
            .ToList();

        return operations;
    }
}