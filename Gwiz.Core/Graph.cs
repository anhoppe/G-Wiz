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

        internal List<EdgeTemplate> EdgeTemplates { get; set; } = new ();

        public void AddEdge(INode from, INode to)
        {
            Edges.Add(CreateEdge(from, to, Ending.None, Style.None));
        }

        public void AddEdge(INode from, INode to, IEdgeTemplate edgeTemplate)
        {
            var edge = CreateEdge(from, to);

            edge.Beginning = edgeTemplate.Beginning;
            edge.Ending = edgeTemplate.Ending;
            edge.Style = edgeTemplate.Style;
            edge.Text = edgeTemplate.Text;

            Edges.Add(edge);
        }

        public void AddEdge(INode from, INode to, Ending ending, Style style)
        {
            Edges.Add(CreateEdge(from, to, ending, style));
        }

        public void AddEdge(INode from, INode to, string fromLabel, string toLabel, float labelOffsetPerCent)
        {
            Edges.Add(CreateEdge(from, to, fromLabel, toLabel, labelOffsetPerCent));
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

            node.UpdateableGrid = Grid.CreateFromTemplateGrid(template.Grid);

            AssignEdgeTemplates(templateName, node);

            Nodes.Add(node);

            return node;
        }

        public void Update()
        {
            foreach (var node in Nodes)
            {
                var nodeInternal = node as Node;

                if (nodeInternal != null)
                {
                    nodeInternal.Update();
                }
            }
        }

        private void AssignEdgeTemplates(string templateName, Node node)
        {
            foreach (var edgeTemplate in EdgeTemplates)
            {
                if (edgeTemplate.Source == templateName)
                {
                    node.SourceEdgeTemplates.Add(edgeTemplate);
                }
                if (edgeTemplate.Target == templateName)
                {
                    node.TargetEdgeTemplates.Add(edgeTemplate);
                }
            }
        }

        private Edge CreateEdge(INode from, INode to)
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
                FromInternal = fromInternal,
                ToInternal = toInternal,
            };
        }

        private Edge CreateEdge(INode from, INode to, Ending ending, Style style)
        {
            var edge = CreateEdge(from, to);

            edge.Ending = ending;
            edge.Style = style;

            return edge;
        }

        private IEdge CreateEdge(INode from, INode to, string fromLabel, string toLabel, float labelOffsetPerCent)
        {
            var edge = CreateEdge(from, to);

            edge.FromLabel = fromLabel;
            edge.ToLabel = toLabel;
            edge.LabelOffsetPerCent = labelOffsetPerCent;

            return edge;
        }
    }
}
