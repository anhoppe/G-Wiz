﻿using Gwiz.Core.Contract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gwiz.Core
{
    public class Graph : IGraph
    {
        private Func<string, (int, int)> _textSizeCalculator = (text) => throw new NotImplementedException("Text size calculator is not set in graph.");

        public event EventHandler<IList<ContextMenuItem>>? ContextMenuShown;

        public event EventHandler<INode>? NodeRemoved;

        public List<IEdge> Edges { get; set; } = new();

        public List<INode> Nodes { get; set; } = new();

        public List<ITemplate> Templates { get; set; } = new ();

        internal Func<IUpdatableNode, IUpdatableNode, IList<IEdge>, IEdgeBuilder> EdgeBuilderFactory { private get; set; } = (IUpdatableNode from, IUpdatableNode to, IList<IEdge> edges) => new EdgeBuilder(from, to, edges);

        internal List<EdgeTemplate> EdgeTemplates { get; set; } = new();

        public IEdgeBuilder AddEdge(INode from, INode to)
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

            return EdgeBuilderFactory(fromInternal, toInternal, Edges);
        }

        public INodeBuilder AddNode(string templateName)
        {
            var template = Templates.Find(t => t.Name == templateName);

            if (template == null)
            {
                throw new KeyNotFoundException($"Template {templateName} not found");
            }

            var nodeBuilder = new NodeBuilder(Nodes, template, _textSizeCalculator);

            AssignEdgeTemplates(templateName, nodeBuilder.Node);

            return nodeBuilder;
        }

        public void Remove(IEdge edge)
        {
            Edges.Remove(edge);
        }

        public void Remove(INode node)
        {
            foreach (var edge in Edges.ToArray())
            {
                if (edge.From == node || edge.To == node)
                {
                    Edges.Remove(edge);
                }
            }

            Nodes.Remove(node);

            NodeRemoved?.Invoke(this, node);
        }

        public void SetTextSizeCalculator(Func<string, (int, int)> textSizeCalculator) => _textSizeCalculator = textSizeCalculator;

        public void ShowContextMenu(IList<ContextMenuItem> contextMenuItems)
        {
            ContextMenuShown?.Invoke(this, contextMenuItems);
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
    }
}
