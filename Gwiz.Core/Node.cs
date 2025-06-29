using Gwiz.Core.Contract;
using Gwiz.Core.Serializer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Gwiz.Core
{
    internal class Node : IUpdatableNode
    {
        private IUpdatableGrid _grid = new Grid();

        private int _height;

        private bool _selected;
        
        private int _width;

        private int _x;

        private int _y;

        public event EventHandler<IUpdatableNode> NodeChanged = delegate { };

        public event EventHandler<bool>? SelectedChanged;

        public Alignment Alignment { get; set; } = Alignment.CenterCenter;

        public Color BackgroundColor { get; set; } = Color.White;

        public IList<IButton> Buttons { get; set; } = new List<IButton>();

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

        public bool Highlight
        {
            get => false;
            set => throw new NotSupportedException("Cannot highlight node");
        }

        public string Id { get; set; } = string.Empty;

        public Color LineColor { get; set; } = Color.Black;

        public Resize Resize { get; set; } = Resize.None;

        public bool Select 
        { 
            get => _selected;
            set
            {
                _selected = value;
                SelectedChanged?.Invoke(this, _selected);
            }
        }

        public Shape Shape { get; set; } = Shape.Rectangle;

        public List<IEdgeTemplate> SourceEdgeTemplates { get; } = new();

        public List<IEdgeTemplate> TargetEdgeTemplates { get; } = new();

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

        internal List<ContentDto> ContentDto { get; set; } = new List<ContentDto>();

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

        public IButton GetButtonById(string id)
        {
            var button = Buttons.FirstOrDefault(b => b.Id == id);

            if (button == null)
            {
                throw new ArgumentException($"Button with id {id} not found");
            }

            return button;
        }

        public bool IsOver(Point position)
        {
            return (position.X >= X &&
                position.X <= X + Width &&
                position.Y >= Y &&
                position.Y <= Y + Height);
        }

        public void SetId(string id)
        {
            Id = id;
        }

        internal void AssignTemplate(ITemplate template)
        {
            Alignment = template.Alignment;
            BackgroundColor = template.BackgroundColor;
            LineColor = template.LineColor;
            Resize = template.Resize;
            Shape = template.Shape;
            TemplateName = template.Name;
            UpdateableGrid = Core.Grid.CreateFromTemplateGrid(template.Grid);

            foreach (var button in template.Buttons)
            {
                var newButton = new Button(button);
                Buttons.Add(newButton);
            }
        }

        internal void Update()
        {
            NodeChanged?.Invoke(this, this);
        }
    }
}
