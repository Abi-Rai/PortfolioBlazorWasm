using System.ComponentModel.DataAnnotations;

namespace PortfolioBlazorWasm.Models.Pathfinding;

public class GridSettings
{
    public const int RowMin = 10;
    public const int RowMax = 30;
    public const int ColMin = 20;
    public const int ColMax = 50;

    public GridSettings(int row = 25, int col = 40)
    {
        RowCount = row;
        ColumnCount = col;
    }
    [Required]
    [Range(RowMin, RowMax, ErrorMessage = "Can only set row between 10 - 30")]
    public int RowCount { get; set; }
    [Required]
    [Range(ColMin, ColMax, ErrorMessage = "Can only set column between 20 - 50")]
    public int ColumnCount { get; set; }
}
