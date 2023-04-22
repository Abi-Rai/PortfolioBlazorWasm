namespace PortfolioBlazorWasm.Models.UkBankPa;

public class BankRate
{
    public DateOnly DateChanged { get; set; }
    public double Rate { get; set; }
    public decimal PercentageChanged { get; set; }
}