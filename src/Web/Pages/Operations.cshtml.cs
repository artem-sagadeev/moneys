using Identity.Entities;
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
    public Card Card { get; private set; }

    public async Task<IActionResult> OnGet()
    {
        if (!_signInManager.IsSignedIn(User))
            return Forbid();

        var user = await _signInManager.UserManager.GetUserAsync(User);
        var cardId = user.CardId;
        
        Card = await _cardService.GetById(cardId);
        
        var payments = await _paymentService.GetByCardId(cardId);
        var incomes = await _incomeService.GetByCardId(cardId);

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
            return Forbid();

        var user = await _signInManager.UserManager.GetUserAsync(User);
        var cardId = user.CardId;
        dto.CardId = cardId;
        
        await _paymentService.Create(dto);

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostIncome(CreateIncomeDto dto)
    {
        if (!_signInManager.IsSignedIn(User))
            return Forbid();

        var user = await _signInManager.UserManager.GetUserAsync(User);
        var cardId = user.CardId;
        dto.CardId = cardId;
        
        await _incomeService.Create(dto);

        return RedirectToPage();
    }
    
    public async Task<IActionResult> OnPostUpdatePayment(UpdatePaymentDto dto)
    {
        await _paymentService.Update(dto);

        return RedirectToPage();
    }
    
    public async Task<IActionResult> OnPostUpdateIncome(UpdateIncomeDto dto)
    {
        await _incomeService.Update(dto);

        return RedirectToPage();
    }
    
    public async Task<IActionResult> OnPostDeletePayment(Guid id)
    {
        await _paymentService.Delete(id);

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteIncome(Guid id)
    {
        await _incomeService.Delete(id);

        return RedirectToPage();
    }
}