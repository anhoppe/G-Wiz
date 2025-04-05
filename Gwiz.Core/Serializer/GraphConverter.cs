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
        public bool Accepts(Type type) => typeof(IGraph).IsAssignableFrom(type);

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
            if (value is not IGraph graph)
            {
                throw new ArgumentException("Expected an IGraph instance.", nameof(value));
            }

            emitter.Emit(new MappingStart());

            if (graph.Nodes.Any())
            {
                emitter.Emit(new Scalar("Nodes"));
                serializer(graph.Nodes);
            }

            if (graph.Edges.Any())
            {
                emitter.Emit(new Scalar("Edges"));
                serializer(graph.Edges);
            }

            emitter.Emit(new MappingEnd());
        }
    }
}
