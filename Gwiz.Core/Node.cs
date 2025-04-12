using Gwiz.Core.Contract;
using Gwiz.Core.Serializer;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Gwiz.Core
{
    internal class Node : IUpdatableNode
    {
        private int _height;

        private IUpdatableGrid _grid = new Grid();

        private int _width;

        private int _x;

        private int _y;

        public Alignment Alignment { get; set; } = Alignment.CenterCenter;

        public Color BackgroundColor { get; set; } = Color.White;

        public IGrid Grid => UpdateableGrid;

        public int Height
        {
            get => _height;
            set
            {
                _height = value;
                NodeChanged?.Invoke(this, this);
            }
        }

        public string Id { get; set; } = string.Empty;

        public Color LineColor { get; set; } = Color.Black;

        public Resize Resize { get; set; } = Resize.None;

        public Shape Shape { get; set; } = Shape.Rectangle;

        public int Width
        {
            get => _width;
            set
            {
                _width = value;
                NodeChanged?.Invoke(this, this);
            }
        }

        public int X
        {
            get => _x;
            set
            {
                _x = value;
                NodeChanged?.Invoke(this, this);
            }
        }

        public int Y
        {
            get => _y;
            set
            {
                _y = value;
                NodeChanged?.Invoke(this, this);
            }
        }

        public event EventHandler<IUpdatableNode> NodeChanged = delegate { };

        internal List<Content> Content { get; set; } = new List<Content>();

        internal string TemplateName { get; set; } = string.Empty;

        internal IUpdatableGrid UpdateableGrid
        {
            get => _grid;
            set
            {
                _grid = value;
                _grid.RegisterParentNodeChanged(this);
            }
        }

        internal void AssignTemplate(ITemplate template)
        {
            Alignment = template.Alignment;
            BackgroundColor = template.BackgroundColor;
            LineColor = template.LineColor;
            Resize = template.Resize;
            Shape = template.Shape;
            UpdateableGrid = Core.Grid.CreateFromTemplateGrid(template.Grid);
        }

        internal void Update()
        {
            NodeChanged?.Invoke(this, this);
        }
    }
}
