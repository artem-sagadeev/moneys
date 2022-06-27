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
    private readonly IPaymentService _paymentService;
    private readonly IIncomeService _incomeService;
    private readonly ICardService _cardService;
    private readonly SignInManager<User> _signInManager;

    public OperationsModel(IPaymentService paymentService, IIncomeService incomeService, ICardService cardService, SignInManager<User> signInManager)
    {
        _paymentService = paymentService;
        _incomeService = incomeService;
        _cardService = cardService;
        _signInManager = signInManager;
    }
    
    public List<IOperation> Operations { get; private set; }
    
    public List<Card> AllCards { get; private set; }
    
    public List<Card> CheckedCards { get; private set; }

    public async Task<IActionResult> OnGet()
    {
        if (!_signInManager.IsSignedIn(User))
            return Redirect("SignIn");

        var user = await _signInManager.UserManager.GetUserAsync(User);
        
        AllCards = await _cardService.GetByUserId(user.Id);
        CheckedCards = AllCards.OrderBy(card => card.Name).ToList();
        
        var payments = await _paymentService.GetByCardIds(CheckedCards.Select(card => card.Id).ToList());
        var incomes = await _incomeService.GetByCardIds(CheckedCards.Select(card => card.Id).ToList());

        Operations = payments
            .Select(payment => (IOperation)payment)
            .Concat(incomes)
            .OrderByDescending(operation => operation.DateTime)
            .ToList();

        return Page();
    }
    
    public async Task<IActionResult> OnPost(List<Guid> cardIds)
    {
        if (!_signInManager.IsSignedIn(User))
            return Redirect("SignIn");

        var user = await _signInManager.UserManager.GetUserAsync(User);
        
        AllCards = await _cardService.GetByUserId(user.Id);
        CheckedCards = (await _cardService.GetByIds(cardIds)).OrderBy(card => card.Name).ToList();

        if (CheckedCards.Any(card => card.UserId != user.Id))
            return Forbid();
        
        var payments = await _paymentService.GetByCardIds(cardIds);
        var incomes = await _incomeService.GetByCardIds(cardIds);

        Operations = payments
            .Select(payment => (IOperation)payment)
            .Concat(incomes)
            .OrderByDescending(operation => operation.DateTime)
            .ToList();

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