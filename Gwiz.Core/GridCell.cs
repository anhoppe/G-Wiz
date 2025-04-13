using Gwiz.Core.Contract;
using System.Drawing;

namespace Gwiz.Core
{
    internal class GridCell : IGridCell
    {
        private bool _isEditing;

        public GridCell(bool editable)
        {
            _isEditing = false;
            Editable = editable;
        }

        public bool Editable { get; }

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
