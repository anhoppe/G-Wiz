using System.Drawing;

namespace Gwiz.Core.Contract
{
    public interface IGrid
    {
        /// <summary>
        /// Defines the columns in the grid
        /// Each entry represents the available horizontal space
        /// </summary>
        List<string> Cols { get; }

        /// <summary>
        /// Represents the position and size of the fields defined in the grid
        /// </summary>
        Rectangle[][] FieldRects { get; }

        /// <summary>
        /// Text in the defined grid fields
        /// </summary>
        string[][] FieldText { get; }

        /// <summary>
        /// Defines the rows in the grid
        /// Each entry represents the available vertical space
        /// </summary>
        List<string> Rows { get; }
    }
}