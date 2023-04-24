using CsvHelper.Configuration;
using System.Globalization;

namespace PortfolioBlazorWasm.Models.UkBankPa.ClassMaps;

public class PersonalAllowanceMap : ClassMap<PersonalAllowance>
{
    public PersonalAllowanceMap()
    {
        //we skip all null rows in CSVHelper's configuration property ShouldSkipRecord 
        Map(m => m.ToDate).Name("To").TypeConverterOption.Format("dd/M/yyyy");
        Map(m => m.AllowanceAmountGBP).Name("Personal Allowance")
            .Convert(args => decimal.Parse(args.Row.GetField("Personal Allowance")!, NumberStyles.Currency, new CultureInfo("en-GB")));
        Map(m => m.PercentageChanged).Name("Change (%)").Convert(args => decimal.Parse(args.Row.GetField("Change (%)")!.TrimEnd('%')));
    }
}