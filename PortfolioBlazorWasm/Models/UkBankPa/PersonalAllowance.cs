﻿namespace PortfolioBlazorWasm.Models.UkBankPa;

public class PersonalAllowance
{
    public DateOnly To { get; set; }
    public decimal AllowanceAmountGBP { get; set; }
    public decimal PercentageChanged { get; set; }
}
