using System.Security.Claims;
using Common.Exceptions;
using Common.Extensions;
using Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Operations.Dtos;
using Operations.Entities;
using Operations.Interfaces;
using Operations.Logic.Cards;
using Operations.Logic.Incomes;
using Operations.Logic.Payments;

namespace ApplicationServices.Operations;

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
        var userCardIds = userCards.Ids();
        
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
        if (cardIds.Any(cardId => !userCards.Ids().Contains(cardId)))
            throw new NoAccessException();
        
        var payments = await _paymentService.GetByCardIds(cardIds);
        var incomes = await _incomeService.GetByCardIds(cardIds);

        var operations = payments
            .Select(payment => (IOperation)payment)
            .Concat(incomes)
            .ToList();

        return operations;
    }

    public async Task CreatePayment(ClaimsPrincipal user, CreatePaymentDto dto)
    {
        if (!await HasUserAccessToCard(user, dto.CardId))
            throw new NoAccessException();
        
        await _paymentService.Create(dto);
    }
    
    public async Task CreateIncome(ClaimsPrincipal user, CreateIncomeDto dto)
    {
        if (!await HasUserAccessToCard(user, dto.CardId))
            throw new NoAccessException();
        
        await _incomeService.Create(dto);
    }

    public async Task UpdatePayment(ClaimsPrincipal user, UpdatePaymentDto dto)
    {
        if (!await HasUserAccessToPayment(user, dto.Id) || !await HasUserAccessToCard(user, dto.CardId))
            throw new NoAccessException();
        
        await _paymentService.Update(dto);
    }

    public async Task UpdateIncome(ClaimsPrincipal user, UpdateIncomeDto dto)
    {
        if (!await HasUserAccessToIncome(user, dto.Id) || !await HasUserAccessToCard(user, dto.CardId))
            throw new NoAccessException();
        
        await _incomeService.Update(dto);
    }

    public async Task DeletePayment(ClaimsPrincipal user, Guid paymentId)
    {
        if (!await HasUserAccessToPayment(user, paymentId))
            throw new NoAccessException();

        await _paymentService.Delete(paymentId);
    }

    public async Task DeleteIncome(ClaimsPrincipal user, Guid incomeId)
    {
        if (!await HasUserAccessToIncome(user, incomeId))
            throw new NoAccessException();

        await _incomeService.Delete(incomeId);
    }

    public async Task Transfer(ClaimsPrincipal user, TransferDto dto)
    {
        if (dto.FromCardId == dto.ToCardId)
            throw new ArgumentException("Transfer to same card is not impossible");
        
        if (!await HasUserAccessToCard(user, dto.FromCardId))
            throw new NoAccessException();
        
        if (!await HasUserAccessToCard(user, dto.ToCardId))
            throw new NoAccessException();

        await _paymentService.Create(new CreatePaymentDto
        {
            Name = dto.Name, 
            Amount = dto.Amount, 
            CardId = dto.FromCardId
        });
        await _incomeService.Create(new CreateIncomeDto
        {
            Name = dto.Name, 
            Amount = dto.Amount, 
            CardId = dto.ToCardId
        });
    }

    public async Task CreateCard(ClaimsPrincipal user, CreateCardDto dto)
    {
        var currentUser = await _userManager.GetUserAsync(user);
        dto.UserId = currentUser.Id;

        await _cardService.Create(dto);
    }

    public async Task UpdateCard(ClaimsPrincipal user, UpdateCardDto dto)
    {
        if (!await HasUserAccessToCard(user, dto.Id))
            throw new NoAccessException();

        await _cardService.Update(dto);
    }

    public async Task DeleteCard(ClaimsPrincipal user, Guid cardId)
    {
        if (!await HasUserAccessToCard(user, cardId))
            throw new NoAccessException();

        await _cardService.Delete(cardId);
    }

    private async Task<bool> HasUserAccessToCard(ClaimsPrincipal user, Guid cardId)
    {
        var currentUser = await _userManager.GetUserAsync(user);
        var userCards = await _cardService.GetByUserId(currentUser.Id);

        return userCards.Ids().Contains(cardId);
    }

    private async Task<bool> HasUserAccessToPayment(ClaimsPrincipal user, Guid paymentId)
    {
        var currentUser = await _userManager.GetUserAsync(user);
        var userCards = await _cardService.GetByUserId(currentUser.Id);
        var payment = await _paymentService.Get(paymentId);

        return userCards.Ids().Contains(payment.CardId);
    }
    
    private async Task<bool> HasUserAccessToIncome(ClaimsPrincipal user, Guid incomeId)
    {
        var currentUser = await _userManager.GetUserAsync(user);
        var userCards = await _cardService.GetByUserId(currentUser.Id);
        var income = await _incomeService.Get(incomeId);

        return userCards.Ids().Contains(income.CardId);
    }
}