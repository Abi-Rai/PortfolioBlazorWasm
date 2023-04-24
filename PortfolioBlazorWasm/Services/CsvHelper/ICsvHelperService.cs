using CsvHelper.Configuration;

namespace PortfolioBlazorWasm.Services.CsvHelper;

public interface ICsvHelperService
{
    Task<List<Model>> GetDataFromCsv<Model, ModelMap>(string filePath) where ModelMap : ClassMap;
}