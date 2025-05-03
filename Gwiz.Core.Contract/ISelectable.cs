using System.Drawing;

namespace Gwiz.Core.Contract
{
    public interface ISelectable
    {
        bool Highlight { get; set; }

        bool Select { get; set; }

        bool IsOver(Point position);

        /// <summary>
        /// Event is raised when a node is selected
        /// </summary>

        event EventHandler<bool>? SelectedChanged;
    }
}
