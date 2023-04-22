using PortfolioBlazorWasm.Models.UkBankPa;
using PortfolioBlazorWasm.Models.UkBankPa.ClassMaps;
using PortfolioBlazorWasm.Services.CsvHelperService;

namespace PortfolioBlazorWasm.Services.BankPaService;

public class BankPaService : IBankPaService
{
    private readonly ICsvHelperService _csvHelperService;

    public BankPaService(ICsvHelperService csvHelperService)
    {
        _csvHelperService = csvHelperService;
    }
    public async Task<List<BankRate>> GetBankRateRecords()
    {
        List<BankRate> records = await _csvHelperService.GetDataFromCsv<BankRate, BankRateMap>(@"sample-data/UK-Bank-Rate-1975-2023.csv");
        return records;
    }
    public async Task<List<PersonalAllowance>> GetPersonalAllowanceRecords()
    {
        List<PersonalAllowance> records = await _csvHelperService.GetDataFromCsv<PersonalAllowance, PersonalAllowanceMap>(@"sample-data/Uk-PA-1974-2023.csv");
        return records;
    }
}
