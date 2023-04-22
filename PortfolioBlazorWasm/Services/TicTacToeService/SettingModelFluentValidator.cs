using FluentValidation;
using PortfolioBlazorWasm.Models.TicTacToe.Settings;

namespace PortfolioBlazorWasm.Services.TicTacToeService;

public class SettingModelFluentValidator : AbstractValidator<ChangeSettings>
{
    public SettingModelFluentValidator()
    {
        RuleFor(cs => cs.PlayerOneSettings.Name)
                    .NotNull()
                    .Length(1, 10)
                    .Must((settings, name) => !settings.PlayerTwoSettings.Name.ToLowerInvariant().Equals(name.ToLowerInvariant()))
                    .WithMessage("Cannot be same name as other player's name");

        RuleFor(cs => cs.PlayerOneSettings.Marker)
            .NotEmpty()
            .Length(1)
            .Must((settings, marker) => !settings.PlayerTwoSettings.Marker.ToLowerInvariant().Equals(marker.ToLowerInvariant()))
            .WithMessage("Cannot set same marker as other player's marker");



        RuleFor(cs => cs.PlayerTwoSettings.Name)
            .NotNull()
            .Length(1, 10)
            .Must((settings, name) => !settings.PlayerOneSettings.Name.ToLowerInvariant().Equals(name.ToLowerInvariant()))
            .WithMessage("Cannot be same name as other player's name");

        RuleFor(cs => cs.PlayerTwoSettings.Marker)
            .NotEmpty()
            .Length(1)
            .Must((settings, marker) => !settings.PlayerOneSettings.Marker.ToLowerInvariant().Equals(marker.ToLowerInvariant()))
            .WithMessage("Cannot set same marker as other player's marker");


        RuleFor(cs => cs.BoardSettings.StartingPlayer)
            .IsInEnum();

        RuleFor(cs => cs.BoardSettings.RowSizes)
            .IsInEnum();

        RuleFor(cs => cs.PlayerOneSettings.ColorCell)
            .Must((settings, mudColor) => !settings.PlayerTwoSettings.ColorCell.Equals(mudColor))
            .WithMessage("Cannot set same color as other player's color");

        RuleFor(cs => cs.PlayerTwoSettings.ColorCell)
            .Must((settings, mudColor) => !settings.PlayerOneSettings.ColorCell.Equals(mudColor))
            .WithMessage("Cannot set same color as other player's color");
    }
}
