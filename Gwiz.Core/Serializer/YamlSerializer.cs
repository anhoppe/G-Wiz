using Gwiz.Core.Contract;
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
using YamlDotNet.Serialization.BufferedDeserialization;
using YamlDotNet.Serialization.NodeDeserializers;

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
                    .WithTypeConverter(new GraphConverter())
                    .WithTypeConverter(new NodeConverter())
                    .WithTypeConverter(new TemplateConverter())
                    .Build();

                graph = deserializer.Deserialize<IGraph>(yaml);
            }

            CompleteGrid(graph);
            ResolveTemplatesInNodes(graph);

            return graph;
        }

        private void CompleteGrid(IGraph graph)
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

        private void ResolveTemplatesInNodes(IGraph graph)
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
