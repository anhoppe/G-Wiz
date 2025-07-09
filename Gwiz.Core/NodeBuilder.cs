using Gwiz.Core.Contract;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Gwiz.Core
{
    internal class NodeBuilder : INodeBuilder
    {
        private bool _hasAutoWidth = false;

        private Node _node;

        private IList<INode> _nodes;

        private Func<string, (int, int)> _textSizeCalculator;

        public NodeBuilder(IList<INode> nodes, ITemplate template, Func<string, (int, int)> textSizeCalculator)
        {
            _textSizeCalculator = textSizeCalculator;

            _node = new Node()
            {
                Id = Guid.NewGuid().ToString(),
            };

            _node.AssignTemplate(template);
            _node.UpdateableGrid = Grid.CreateFromTemplateGrid(template.Grid);

            _nodes = nodes;
        }

        internal Node Node { get => _node; }

        public INode Build()
        {
            if (_hasAutoWidth)
            {
                // Calculate the width based on the text size of the first cell
                var firstCell = _node.Grid.Cells[0, 0];
                var (textWidth, textHeight) = _textSizeCalculator(firstCell.Text);
                _node.Width = textWidth + 20; // Adding some padding
            }

            _nodes.Add(_node);

            return _node;
        }

        public INodeBuilder WithAutoWidth()
        {
            _hasAutoWidth = true;
            return this;
        }

        public INodeBuilder WithHeight(int height)
        {
            _node.Height = height;
            return this;
        }

        public INodeBuilder WithId(string id)
        {
            _node.SetId(id);
            return this;
        }

        public INodeBuilder WithPos(int x, int y)
        {
            _node.X = x;
            _node.Y = y;
            return this;
        }

        public INodeBuilder WithSize(int width, int height)
        {
            _node.Width = width;
            _node.Height = height;
            return this;
        }

        public INodeBuilder WithText(int row, int col, string text)
        {
            _node.Grid.Cells[row, col].Text = text;
            return this;
        }

        public INodeBuilder WithWidth(int width)
        {
            _node.Width = width;
            return this;
        }

        public INodeBuilder WithX(int x)
        {
            _node.X = x;
            return this;
        }

        public INodeBuilder WithY(int y)
        {
            _node.Y = y;
            return this;
        }
    }
}
