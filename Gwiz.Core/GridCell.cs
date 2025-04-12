using Gwiz.Core.Contract;
using System.Drawing;

namespace Gwiz.Core
{
    internal class GridCell : IGridCell
    {
        private bool _isEditing;

        public GridCell()
        {
            _isEditing = false;
        }

        public int EditTextPosition { get; set; }

        public bool EditModeEnabled
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                if (value)
                {
                    EditTextPosition = Text.Length;
                }
            }
        }

        public Rectangle Rectangle { get; set; }

        public string Text { get; set; } = string.Empty;
    }
}
