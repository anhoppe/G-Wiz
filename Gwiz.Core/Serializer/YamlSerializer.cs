﻿using Gwiz.Core.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YamlDotNet.Core;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;

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

            return graph;
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
                    }
                }
            }
        }
    }
}
