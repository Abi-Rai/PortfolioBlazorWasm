using PortfolioBlazorWasm.Models.UkBankPa;

namespace PortfolioBlazorWasm.Services.BankPaService;

public interface IBankPaService
{
    Task<List<BankRate>> GetBankRateRecords();
    Task<List<PersonalAllowance>> GetPersonalAllowanceRecords();
}