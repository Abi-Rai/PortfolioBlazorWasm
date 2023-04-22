using CsvHelper.Configuration;

namespace PortfolioBlazorWasm.Models.UkBankPa.ClassMaps;

public class BankRateMap : ClassMap<BankRate>
{
    public BankRateMap()
    {
        //we skip all null rows in CSVHelper's configuration property ShouldSkipRecord 
        Map(m => m.DateChanged).Name("Date Changed").TypeConverterOption.Format("dd/M/yyyy");
        Map(m => m.Rate).Name("Rate (%)");
        Map(m => m.PercentageChanged).Name("Change (%)").Convert(args =>
        decimal.Parse(args.Row.GetField("Change (%)")!.TrimEnd('%')));
    }
}