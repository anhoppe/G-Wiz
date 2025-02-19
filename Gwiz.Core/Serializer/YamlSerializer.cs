﻿using Gwiz.Core.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.Drawing;

namespace Gwiz.Core.Serializer
{
    public class YamlSerializer : Contract.ISerializer
    {
        Func<string, Size> _textSizeFactory;

        public YamlSerializer(Func<string, Size> textSizeFactory) 
        {
            _textSizeFactory = textSizeFactory;
        }

        public Graph Deserialize(Stream stream)
        {
            Graph graph = new Graph();

            ;
            using (var reader = File.OpenRead("c:/test.txt"))
            {
                string yaml = reader.ReadToEnd();

                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(PascalCaseNamingConvention.Instance)
                    .WithTypeConverter(new NodeConverter())
                    .WithTypeConverter(new TemplateConverter())
                    .Build();

                graph = deserializer.Deserialize<Graph>(yaml);
            }

            CompleteGrid(graph);
            ResolveTemplatesInNodes(graph);

            return graph;
        }

        private void CompleteGrid(Graph graph)
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

        private void ResolveTemplatesInNodes(Graph graph)
        {
            foreach (var node in graph.Nodes)
            {
                var nodeInternal = node as NodeInternal;

                if (nodeInternal != null)
                {
                    if (!string.IsNullOrEmpty(nodeInternal.TemplateName))
                    {
                        if (!graph.Templates.Any(t => t.Name == nodeInternal.TemplateName))
                        {
                            throw new UnknownTemplateReference(nodeInternal.TemplateName);
                        }

                        var template = graph.Templates.FirstOrDefault(t => t.Name == nodeInternal.TemplateName) ?? new();
                        node.Template = template;

                        node.Grid = new Grid(template.Grid)
                        {
                            TextSizeFactory = _textSizeFactory,
                            ParentNode = node,
                        };
                    }

                    node.Grid.ParentNode = node;
                }
            }
        }
    }
}
