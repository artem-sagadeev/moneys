using ApplicationServices;
using Identity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Operations.Dtos;
using Operations.Entities;
using Operations.Interfaces;
using Operations.Logic.Cards;
using Operations.Logic.Incomes;
using Operations.Logic.Payments;

namespace Web.Pages;

[Authorize]
public class OperationsModel : PageModel
{
    private readonly IOperationsService _operationsService;
    private readonly IPaymentService _paymentService;
    private readonly IIncomeService _incomeService;
    private readonly ICardService _cardService;
    private readonly SignInManager<User> _signInManager;

    public OperationsModel(IPaymentService paymentService, IIncomeService incomeService, ICardService cardService, SignInManager<User> signInManager, IOperationsService operationsService)
    {
        _paymentService = paymentService;
        _incomeService = incomeService;
        _cardService = cardService;
        _signInManager = signInManager;
        _operationsService = operationsService;
    }

    public List<Card> AllCards { get; private set; }
    
    public List<Guid> CheckedCardIds { get; private set; }
    
    public Dictionary<Guid, string> CardNames { get; set; }
    
    public int Sum { get; set; }
    
    public List<IOperation> Operations { get; private set; }

    public async Task<IActionResult> OnGet()
    {
        var userCards = await _operationsService.GetAllUserCards(User);
        var operations = await _operationsService.GetAllUserOperations(User);

        AllCards = userCards.OrderBy(card => card.Name).ToList();
        CheckedCardIds = AllCards.Select(card => card.Id).ToList();
        CardNames = AllCards.ToDictionary(card => card.Id, card => card.Name);
        Sum = AllCards.Where(card => CheckedCardIds.Contains(card.Id)).Select(card => card.Balance).Sum();
        Operations = operations.OrderByDescending(operation => operation.DateTime).ToList();

        return Page();
    }
    
    public async Task<IActionResult> OnPost(List<Guid> cardIds)
    {
        var userCards = await _operationsService.GetAllUserCards(User);
        var operations = await _operationsService.GetUserOperationsByCardIds(User, cardIds);

        AllCards = userCards.OrderBy(card => card.Name).ToList();
        CheckedCardIds = cardIds;
        CardNames = AllCards.ToDictionary(card => card.Id, card => card.Name);
        Sum = AllCards.Where(card => CheckedCardIds.Contains(card.Id)).Select(card => card.Balance).Sum();
        Operations = operations.OrderByDescending(operation => operation.DateTime).ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostPayment(CreatePaymentDto dto)
    {
        if (!_signInManager.IsSignedIn(User))
            return Redirect("SignIn");

        var user = await _signInManager.UserManager.GetUserAsync(User);
        var cardIds = (await _cardService.GetByUserId(user.Id)).Select(card => card.Id);
        if (!cardIds.Contains(dto.CardId))
            return Forbid();
        
        await _paymentService.Create(dto);

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostIncome(CreateIncomeDto dto)
    {
        if (!_signInManager.IsSignedIn(User))
            return Redirect("SignIn");
        
        var user = await _signInManager.UserManager.GetUserAsync(User);
        var cardIds = (await _cardService.GetByUserId(user.Id)).Select(card => card.Id);
        if (!cardIds.Contains(dto.CardId))
            return Forbid();

        await _incomeService.Create(dto);

        return RedirectToPage();
    }
    
    public async Task<IActionResult> OnPostUpdatePayment(UpdatePaymentDto dto)
    {
        await _paymentService.Update(dto);
        
        //TODO: add checks

        return RedirectToPage();
    }
    
    public async Task<IActionResult> OnPostUpdateIncome(UpdateIncomeDto dto)
    {
        await _incomeService.Update(dto);
        
        //TODO: add checks

        return RedirectToPage();
    }
    
    public async Task<IActionResult> OnPostDeletePayment(Guid id)
    {
        await _paymentService.Delete(id);
        
        //TODO: add checks

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteIncome(Guid id)
    {
        await _incomeService.Delete(id);
        
        //TODO: add checks

        return RedirectToPage();
    }
}