using Gwiz.Core.Contract;
using System.Drawing;

namespace Gwiz.Core
{
    internal class Node : INode
    {
        private int _height;

        private IUpdateableGrid _grid = new Grid();

        private int _width;

        private int _x;

        private int _y;

        public Color BackgroundColor { get; set; } = Color.White;

        public IGrid Grid => UpdateableGrid;

        public int Height 
        { 
            get => _height;
            set
            {
                _height = value;
                UpdateableGrid.UpdateFieldRects(this);
            }
        }

        public Color LineColor { get; set; } = Color.Black;

        public Resize Resize { get; set; } = Resize.None;

        public int Width
        {
            get => _width;
            set
            {
                _width = value;
                UpdateableGrid.UpdateFieldRects(this);
            }
        }

        public int X 
        { 
            get => _x;
            set 
            {
                _x = value;
                UpdateableGrid.UpdateFieldRects(this);
            } 
        }

        public int Y
        {
            get => _y;
            set
            {
                _y = value;
                UpdateableGrid.UpdateFieldRects(this);
            }
        }

        internal string TemplateName { get; set; } = string.Empty;

        internal IUpdateableGrid UpdateableGrid 
        { 
            get => _grid;
            set 
            {
                _grid = value;
                _grid.UpdateFieldRects(this);
            } 
        }

        internal void AssignTemplate(ITemplate template)
        {
            UpdateableGrid = new Grid(template.Grid);
            BackgroundColor = template.BackgroundColor;
            LineColor = template.LineColor;
            Resize = template.Resize;
        }
    }
}
