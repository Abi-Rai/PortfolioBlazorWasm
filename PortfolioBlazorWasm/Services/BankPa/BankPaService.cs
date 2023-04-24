using PortfolioBlazorWasm.Models.UkBankPa;
using PortfolioBlazorWasm.Models.UkBankPa.ClassMaps;
using PortfolioBlazorWasm.Services.CsvHelper;

namespace PortfolioBlazorWasm.Services.BankPa;

public class BankPaService : IBankPaService
{
    public const string PersonalAllowanceFilePath = @"sample-data/Uk-PA-1975-2023.csv";
    public const string BankInterestRateFilePath = @"sample-data/UK-Bank-Rate-1975-2023.csv";
    private readonly ICsvHelperService _csvHelperService;

    public BankPaService(ICsvHelperService csvHelperService)
    {
        _csvHelperService = csvHelperService;
    }
    public async Task<List<BankRate>> GetBankRateRecords()
    {
        List<BankRate> records = await _csvHelperService.GetDataFromCsv<BankRate, BankRateMap>(BankInterestRateFilePath);
        return records;
    }
    public async Task<List<PersonalAllowance>> GetPersonalAllowanceRecords()
    {
        List<PersonalAllowance> records = await _csvHelperService.GetDataFromCsv<PersonalAllowance, PersonalAllowanceMap>(PersonalAllowanceFilePath);
        return records;
    }
}
