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

namespace Gwiz.Core.Serializer
{
    public class YamlSerializer : Contract.ISerializer
    {   
        public Graph Deserialize(Stream stream)
        {
            Graph graph = new Graph();

            using (var reader = new StreamReader(stream))
            {
                string yaml = reader.ReadToEnd();

                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(PascalCaseNamingConvention.Instance)
                    .WithTypeConverter(new NodeConverter())
                    .WithTypeConverter(new TemplateConverter())
                    .Build();

                graph = deserializer.Deserialize<Graph>(yaml);
            }

            ResolveTemplates(graph);

            return graph;
        }

        private void ResolveTemplates(Graph graph)
        {
            foreach (var node in graph.Nodes)
            {
                var nodeInternal = node as NodeInternal;

                if (nodeInternal != null)
                {
                    if (string.IsNullOrEmpty(nodeInternal.TemplateName))
                    {
                        continue;
                    }

                    if (!graph.Templates.Any(t => t.Name == nodeInternal.TemplateName))
                    {
                        throw new UnknownTemplateReference(nodeInternal.TemplateName);
                    }

                    node.Template = graph.Templates.FirstOrDefault(t => t.Name == nodeInternal.TemplateName) ?? new();
                }
            }
        }
    }
}
