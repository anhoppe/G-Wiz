using Gwiz.Core.Contract;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System;

namespace Gwiz.Core.Serializer
{
    public class YamlSerializer : Contract.ISerializer
    {
        public IGraph Deserialize(Stream stream)
        {
            IGraph graph = new Graph();

            using (var reader = new StreamReader(stream))
            {
                string yaml = reader.ReadToEnd();

                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(PascalCaseNamingConvention.Instance)
                    .WithTypeConverter(new EdgeConverter())
                    .WithTypeConverter(new GraphConverter())
                    .WithTypeConverter(new NodeConverter())
                    .WithTypeConverter(new TemplateConverter())
                    .Build();

                graph = deserializer.Deserialize<IGraph>(yaml);
            }

            CompleteGrid(graph);
            ResolveTemplatesInNodes(graph);
            ResolveNodeReferencesInEdges(graph);
            ResolveGridContentInNodes(graph);
            UpdateEdges(graph);

            return graph;
        }

        public void Serialize(Stream stream, IGraph graph)
        {
            using (var writer = new StreamWriter(stream))
            {
                var serializer = new SerializerBuilder()
                    .WithNamingConvention(PascalCaseNamingConvention.Instance)
                    .WithTypeConverter(new EdgeConverter())
                    .WithTypeConverter(new GraphConverter())
                    .WithTypeConverter(new NodeConverter())
                    .Build();
                serializer.Serialize(writer, graph);
            }
        }

        private static void CompleteGrid(IGraph graph)
        {
            foreach (var template in graph.Templates)
            {
                if (!template.Grid.Cols.Any())
                {
                    template.Grid.Cols.Add("1");
                }

                if (!template.Grid.Rows.Any())
                {
                    template.Grid.Rows.Add("1");
                }
            }
        }

        private void UpdateEdges(IGraph graph)
        {
            foreach (var edge in graph.Edges)
            {
                var internalEdge = edge as Edge;

                if (internalEdge != null)
                {
                    internalEdge.UpdateEdge();
                }
            }
        }

        private void ResolveGridContentInNodes(IGraph graph)
        {
            foreach (var node in graph.Nodes)
            {
                var nodeInternal = node as Node;
                if (nodeInternal != null)
                {
                    foreach (var content in nodeInternal.Content)
                    {
                        node.Grid.Cells[content.Col][content.Row].Text = content.Text;
                    }
                }
            }
        }

        private static void ResolveNodeReferencesInEdges(IGraph graph)
        {
            foreach (var edge in graph.Edges)
            {
                var edgeInternal = edge as Edge;
                if (edgeInternal != null)
                {
                    var fromNode = graph.Nodes.FirstOrDefault(n => n.Id == edgeInternal.FromId) as Node;
                    var toNode = graph.Nodes.FirstOrDefault(n => n.Id == edgeInternal.ToId) as Node;

                    edgeInternal.FromInternal = fromNode ?? new Node();
                    edgeInternal.ToInternal = toNode ?? new Node();
                }
            }
        }

        private static void ResolveTemplatesInNodes(IGraph graph)
        {
            foreach (var node in graph.Nodes)
            {
                var nodeInternal = node as Node;

                if (nodeInternal != null)
                {
                    if (!string.IsNullOrEmpty(nodeInternal.TemplateName))
                    {
                        if (!graph.Templates.Any(t => t.Name == nodeInternal.TemplateName))
                        {
                            throw new UnknownTemplateReference(nodeInternal.TemplateName);
                        }

                        var template = graph.Templates.FirstOrDefault(t => t.Name == nodeInternal.TemplateName) ?? new Template();

                        nodeInternal.AssignTemplate(template);

                        nodeInternal.Update();
                    }
                }
            }
        }
    }
}
