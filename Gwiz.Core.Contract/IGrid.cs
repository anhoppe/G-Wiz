using System.Drawing;

namespace Gwiz.Core.Contract
{
    public interface IGrid
    {
        /// <summary>
        /// The individual cells in the grud
        /// </summary>
        IGridCell[][] Cells { get; }

        /// <summary>
        /// Defines the columns in the grid
        /// Each entry represents the available horizontal space
        /// </summary>
        List<string> Cols { get; }

        /// <summary>
        /// Defines the rows in the grid
        /// Each entry represents the available vertical space
        /// </summary>
        List<string> Rows { get; }
    }
}
