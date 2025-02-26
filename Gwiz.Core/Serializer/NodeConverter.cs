using Gwiz.Core.Contract;
using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Gwiz.Core.Serializer
{
    internal class NodeConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type) => type == typeof(INode);

        public object ReadYaml(IParser parser, Type type, ObjectDeserializer deserializer)
        {
            var node = new Node(); // Create instance manually

            // Expect a mapping start (YAML object)
            parser.Consume<MappingStart>();

            while (parser.TryConsume<Scalar>(out var key))
            {
                switch (key.Value)
                {
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
            throw new NotImplementedException();
        }
    }
}
