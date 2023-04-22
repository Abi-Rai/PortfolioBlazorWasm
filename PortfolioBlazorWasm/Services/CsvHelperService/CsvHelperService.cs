using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace PortfolioBlazorWasm.Services.CsvHelperService;

public class CsvHelperService : ICsvHelperService
{
    private readonly HttpClient _httpClient;

    public CsvHelperService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<List<Model>> GetDataFromCsv<Model, ModelMap>(string filePath) where ModelMap : ClassMap
    {
        using var stream = await _httpClient.GetStreamAsync(filePath);
        using var reader = new StreamReader(stream);
        CsvConfiguration configuration = new(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
            ShouldSkipRecord = x => x.Row.Parser.Record?.All(field => string.IsNullOrWhiteSpace(field)) ?? false
        };
        using var csv = new CsvReader(reader, configuration);
        csv.Context.RegisterClassMap<ModelMap>();
        List<Model> records = new();
        await foreach (var record in csv.GetRecordsAsync<Model>())
        {
            records.Add(record);
        }
        return records;
    }
}