using Gwiz.Core.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Gwiz.Core.Serializer
{
    internal class GraphConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type) => type == typeof(IGraph);

        public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
        {
            var graph = new Graph();

            parser.Consume<MappingStart>();

            while (parser.TryConsume<Scalar>(out var key))
            {
                switch (key.Value)
                {
                    case "Edges":
                        var edges = (List<IEdge>)(rootDeserializer(typeof(List<IEdge>)) ?? new List<IEdge>());
                        graph.Edges = edges;
                        break;
                    case "Nodes":
                        var nodes = (List<INode>)(rootDeserializer(typeof(List<INode>)) ?? new List<INode>());
                        graph.Nodes = nodes;
                        break;
                    case "Templates":
                        var templates = (List<Template>)(rootDeserializer(typeof(List<Template>)) ?? new List<Template>());
                        graph.Templates = templates.Cast<ITemplate>().ToList();
                        break;
                }
            }

            parser.Consume<MappingEnd>();

            return graph;
        }

        public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
