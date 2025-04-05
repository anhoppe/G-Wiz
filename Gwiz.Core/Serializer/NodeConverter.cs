﻿using Gwiz.Core.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Gwiz.Core.Serializer
{
    internal class NodeConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type) => typeof(INode).IsAssignableFrom(type);

        public object ReadYaml(IParser parser, Type type, ObjectDeserializer deserializer)
        {
            var node = new Node(); // Create instance manually

            // Expect a mapping start (YAML object)
            parser.Consume<MappingStart>();

            while (parser.TryConsume<Scalar>(out var key))
            {
                switch (key.Value)
                {
                    case "Content":
                        node.Content = ((List<Content>)(deserializer(typeof(List<Content>)) ?? new List<Content>()));
                        break;
                    case "Height":
                        node.Height = int.Parse(parser.Consume<Scalar>().Value);
                        break;
                    case "Id":
                        node.Id = parser.Consume<Scalar>().Value;
                        break;
                    case "Template":
                        node.TemplateName = parser.Consume<Scalar>().Value;
                        break;
                    case "Width":
                        node.Width = int.Parse(parser.Consume<Scalar>().Value);
                        break;
                    case "X":
                        node.X = int.Parse(parser.Consume<Scalar>().Value);
                        break;
                    case "Y":
                        node.Y = int.Parse(parser.Consume<Scalar>().Value);
                        break;
                    default:
                        // Skip unknown properties (including Template if present)
                        parser.SkipThisAndNestedEvents();
                        break;
                }
            }

            parser.Consume<MappingEnd>();

            return node;
        }

        public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
        {
            if(value is not Node node)
            {
                throw new ArgumentException("Expected a Node instance.", nameof(value));
            }

            emitter.Emit(new MappingStart());

            if (node.Content.Any())
            {
                emitter.Emit(new Scalar("Content"));
                serializer(node.Content);
            }

            emitter.Emit(new Scalar("Height"));
            serializer(node.Height);

            emitter.Emit(new Scalar("Id"));
            serializer(node.Id);

            emitter.Emit(new Scalar("Template"));
            serializer(node.TemplateName);

            emitter.Emit(new Scalar("Width"));
            serializer(node.Width);

            emitter.Emit(new Scalar("X"));
            serializer(node.X);

            emitter.Emit(new Scalar("Y"));
            serializer(node.Y);
            
            emitter.Emit(new MappingEnd());
        }
    }
}
