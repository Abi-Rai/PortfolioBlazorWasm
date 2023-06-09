﻿using PortfolioBlazorWasm.Models.UkBankPa;

namespace PortfolioBlazorWasm.Services.BankPa;

public interface IBankPaService
{
    Task<List<BankRate>> GetBankRateRecords();
    Task<List<PersonalAllowance>> GetPersonalAllowanceRecords();
}