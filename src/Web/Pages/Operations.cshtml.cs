using Common.DTOs.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

    public OperationsModel(IPaymentService paymentService, IIncomeService incomeService, ICardService cardService)
    {
        _paymentService = paymentService;
        _incomeService = incomeService;
        _cardService = cardService;
    }
    
    public List<IOperation> Operations { get; private set; }
    public Card Card { get; private set; }
    public Guid CardId => Guid.Parse("0fcf5a3a-d616-43fd-8a27-844689b7b801");

    public async Task OnGet()
    {
        Card = await _cardService.GetById(CardId);
        
        var payments = await _paymentService.GetByCardId(CardId);
        var incomes = await _incomeService.GetByCardId(CardId);

        Operations = payments
            .Select(payment => (IOperation)payment)
            .Concat(incomes)
            .OrderByDescending(operation => operation.DateTime)
            .ToList();
    }

    public async Task<IActionResult> OnPostPayment(CreatePaymentDto dto)
    {
        await _paymentService.Create(dto);

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostIncome(CreateIncomeDto dto)
    {
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