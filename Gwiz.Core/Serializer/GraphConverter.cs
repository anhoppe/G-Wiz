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

            List<EdgeTemplate> edgeTemplates = new();

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
                    case "EdgeTemplates":
                        edgeTemplates = (List<EdgeTemplate>)(rootDeserializer(typeof(List<EdgeTemplate>)) ?? new List<EdgeTemplate>());
                        break;
                }
            }

            parser.Consume<MappingEnd>();

            ResolveSourceTargetEdgeTemplatesInNodes(graph, edgeTemplates);

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

        private static void ResolveSourceTargetEdgeTemplatesInNodes(Graph graph, List<EdgeTemplate> edgeTemplates)
        {
            foreach (var node in graph.Nodes)
            {
                var internalNode = node as Node;

                if (internalNode != null)
                {
                    foreach (var edgeTemplate in edgeTemplates)
                    {
                        if (internalNode.TemplateName == edgeTemplate.Source)
                        {
                            internalNode.SourceEdgeTemplates.Add(edgeTemplate);
                        }
                        if (internalNode.TemplateName == edgeTemplate.Target)
                        {
                            internalNode.TargetEdgeTemplates.Add(edgeTemplate);
                        }
                    }
                }
            }
        }
    }
}
