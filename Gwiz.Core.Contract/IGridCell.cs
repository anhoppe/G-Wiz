using System;
using System.Drawing;

namespace Gwiz.Core.Contract
{
    /// <summary>
    /// Represents a cell in the grid
    /// </summary>
    public interface IGridCell
    {
        /// <summary>
        /// The editing position of the text when edit mode is enabled
        /// </summary>
        int EditTextPosition { get; set; }

        /// <summary>
        /// Enables or disables edit mode for the cell
        /// </summary>
        bool EditModeEnabled { get; set; }

        /// <summary>
        /// Defines the rectangle of the cell in absolute coordinates
        /// </summary>
        Rectangle Rectangle { get; }

        /// <summary>
        /// Content of the cell (currently only text)
        /// </summary>
        string Text { get; set; }
    }
}
