using ApplicationServices.Operations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Operations.Dtos;
using Operations.Dtos.IncomeRecords;
using Operations.Dtos.PaymentRecords;
using Operations.Entities;
using Operations.Interfaces;

namespace Web.Pages.Operations.Single;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IOperationsService _operationsService;

    public IndexModel(IOperationsService operationsService)
    {
        _operationsService = operationsService;
    }

    public List<Card> AllCards { get; private set; }
    
    public List<Guid> CheckedCardIds { get; private set; }
    
    public Dictionary<Guid, string> CardNames { get; set; }
    
    public int Sum { get; set; }
    
    public List<IOperationRecord> Operations { get; private set; }

    public async Task<IActionResult> OnGet()
    {
        var userCards = await _operationsService.GetAllUserCards(User);
        var operations = await _operationsService.GetAllUserOperations(User);

        AllCards = userCards?.OrderBy(card => card.Name).ToList();
        CheckedCardIds = AllCards?.Select(card => card.Id).ToList();
        CardNames = AllCards?.ToDictionary(card => card.Id, card => card.Name);
        Sum = AllCards is not null ? 
            AllCards.Where(card => CheckedCardIds.Contains(card.Id)).Select(card => card.Balance).Sum() : 0;
        Operations = operations?.OrderByDescending(operation => operation.DateTime).ToList();

        return Page();
    }
    
    public async Task<IActionResult> OnPost(List<Guid> cardIds)
    {
        var userCards = await _operationsService.GetAllUserCards(User);
        var operations = await _operationsService.GetUserOperationsByCardIds(User, cardIds);

        AllCards = userCards?.OrderBy(card => card.Name).ToList();
        CheckedCardIds = cardIds;
        CardNames = AllCards?.ToDictionary(card => card.Id, card => card.Name);
        Sum = AllCards is not null ? 
            AllCards.Where(card => CheckedCardIds.Contains(card.Id)).Select(card => card.Balance).Sum() : 0;
        Operations = operations?.OrderByDescending(operation => operation.DateTime).ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostPayment(CreatePaymentRecordDto recordDto)
    {
        await _operationsService.CreatePayment(User, recordDto);

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostIncome(CreateIncomeRecordDto recordDto)
    {
        await _operationsService.CreateIncome(User, recordDto);

        return RedirectToPage();
    }
    
    public async Task<IActionResult> OnPostUpdatePayment(UpdatePaymentRecordDto recordDto)
    {
        await _operationsService.UpdatePayment(User, recordDto);

        return RedirectToPage();
    }
    
    public async Task<IActionResult> OnPostUpdateIncome(UpdateIncomeRecordDto recordDto)
    {
        await _operationsService.UpdateIncome(User, recordDto);

        return RedirectToPage();
    }
    
    public async Task<IActionResult> OnPostDeletePayment(Guid id)
    {
        await _operationsService.DeletePayment(User, id);

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteIncome(Guid id)
    {
        await _operationsService.DeleteIncome(User, id);

        return RedirectToPage();
    }
}