using Gwiz.Core.Contract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gwiz.Core
{
    public class Graph : IGraph
    {
        public List<IEdge> Edges { get; set; } = new();

        public List<INode> Nodes { get; set; } = new();

        public List<ITemplate> Templates { get; set; } = new ();

        public void AddEdge(INode from, INode to)
        {
            Edges.Add(CreateEdge(from, to, Ending.None, Style.None));
        }

        public void AddEdge(INode from, INode to, Ending ending, Style style)
        {
            Edges.Add(CreateEdge(from, to, ending, style));
        }

        public INode AddNode(string templateName)
        {
            var template = Templates.Find(t => t.Name == templateName);

            if (template == null)
            {
                throw new KeyNotFoundException($"Template {templateName} not found");
            }

            var node = new Node();
            node.AssignTemplate(template);

            node.UpdateableGrid = new Grid(template.Grid);
            Nodes.Add(node);

            return node;
        }

        private Edge CreateEdge(INode from, INode to, Ending ending, Style style)
        {
            var fromInternal = from as IUpdatableNode;
            var toInternal = to as IUpdatableNode;

            if (fromInternal == null || toInternal == null)
            {
                throw new ArgumentException("Nodes passed to Graph.AddEdge have invalid type");
            }

            if (Nodes.All(n => n != fromInternal) || Nodes.All(n => n != toInternal))
            {
                throw new ArgumentException("Nodes passed to Graph.AddEdge are not part of the graph");
            }

            return new Edge()
            {
                Ending = ending,
                FromInternal = fromInternal,
                Style = style,
                ToInternal = toInternal,
            };
        }
    }
}
