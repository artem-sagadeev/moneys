using System.Security.Claims;
using Common.Exceptions;
using Common.Extensions;
using Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Operations.Dtos;
using Operations.Dtos.Cards;
using Operations.Dtos.IncomeRecords;
using Operations.Dtos.PaymentRecords;
using Operations.Entities;
using Operations.Interfaces;
using Operations.Logic.Cards;
using Operations.Logic.Incomes;
using Operations.Logic.Payments;

namespace ApplicationServices.Operations;

public class OperationsService : IOperationsService
{
    private readonly IPaymentRecordService _paymentRecordService;
    private readonly IIncomeRecordService _incomeRecordService;
    private readonly ICardService _cardService;
    private readonly UserManager<User> _userManager;

    public OperationsService(IPaymentRecordService paymentRecordService, 
        IIncomeRecordService incomeRecordService, ICardService cardService, UserManager<User> userManager)
    {
        _paymentRecordService = paymentRecordService;
        _incomeRecordService = incomeRecordService;
        _cardService = cardService;
        _userManager = userManager;
    }


    public async Task<List<Card>> GetAllUserCards(ClaimsPrincipal user)
    {
        var currentUser = await _userManager.GetUserAsync(user);
        var userCards = await _cardService.GetByUserId(currentUser.Id);

        return userCards;
    }

    public async Task<List<IOperationRecord>> GetAllUserOperations(ClaimsPrincipal user)
    {
        var userCards = await GetAllUserCards(user);
        var userCardIds = userCards.Ids();
        
        var payments = await _paymentRecordService.GetByCardIds(userCardIds);
        var incomes = await _incomeRecordService.GetByCardIds(userCardIds);

        var operations = payments
            .Select(payment => (IOperationRecord)payment)
            .Concat(incomes)
            .ToList();

        return operations;
    }

    public async Task<List<IOperationRecord>> GetUserOperationsByCardIds(ClaimsPrincipal user, List<Guid> cardIds)
    {
        var userCards = await GetAllUserCards(user);
        if (cardIds.Any(cardId => !userCards.Ids().Contains(cardId)))
            throw new NoAccessException();
        
        var payments = await _paymentRecordService.GetByCardIds(cardIds);
        var incomes = await _incomeRecordService.GetByCardIds(cardIds);

        var operations = payments
            .Select(payment => (IOperationRecord)payment)
            .Concat(incomes)
            .ToList();

        return operations;
    }

    public async Task CreatePayment(ClaimsPrincipal user, CreatePaymentRecordDto recordDto)
    {
        if (!await HasUserAccessToCard(user, recordDto.CardId))
            throw new NoAccessException();
        
        await _paymentRecordService.Create(recordDto);
    }
    
    public async Task CreateIncome(ClaimsPrincipal user, CreateIncomeRecordDto recordDto)
    {
        if (!await HasUserAccessToCard(user, recordDto.CardId))
            throw new NoAccessException();
        
        await _incomeRecordService.Create(recordDto);
    }

    public async Task UpdatePayment(ClaimsPrincipal user, UpdatePaymentRecordDto recordDto)
    {
        if (!await HasUserAccessToPayment(user, recordDto.Id) || !await HasUserAccessToCard(user, recordDto.CardId))
            throw new NoAccessException();
        
        await _paymentRecordService.Update(recordDto);
    }

    public async Task UpdateIncome(ClaimsPrincipal user, UpdateIncomeRecordDto recordDto)
    {
        if (!await HasUserAccessToIncome(user, recordDto.Id) || !await HasUserAccessToCard(user, recordDto.CardId))
            throw new NoAccessException();
        
        await _incomeRecordService.Update(recordDto);
    }

    public async Task DeletePayment(ClaimsPrincipal user, Guid paymentId)
    {
        if (!await HasUserAccessToPayment(user, paymentId))
            throw new NoAccessException();

        await _paymentRecordService.Delete(paymentId);
    }

    public async Task DeleteIncome(ClaimsPrincipal user, Guid incomeId)
    {
        if (!await HasUserAccessToIncome(user, incomeId))
            throw new NoAccessException();

        await _incomeRecordService.Delete(incomeId);
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
        var payment = await _paymentRecordService.Get(paymentId);

        return userCards.Ids().Contains(payment.CardId);
    }
    
    private async Task<bool> HasUserAccessToIncome(ClaimsPrincipal user, Guid incomeId)
    {
        var currentUser = await _userManager.GetUserAsync(user);
        var userCards = await _cardService.GetByUserId(currentUser.Id);
        var income = await _incomeRecordService.Get(incomeId);

        return userCards.Ids().Contains(income.CardId);
    }
}