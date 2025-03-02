using Gwiz.Core.Contract;
using System;
using System.ComponentModel;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Gwiz.Core.Serializer
{
    internal class EdgeConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type) => type == typeof(IEdge);

        public object ReadYaml(IParser parser, Type type, ObjectDeserializer deserializer)
        {
            var edge = new Edge();

            parser.Consume<MappingStart>();
            while (parser.TryConsume<Scalar>(out var key))
            {
                switch (key.Value)
                {
                    case "From":
                        edge.FromId = parser.Consume<Scalar>().Value;
                        break;
                    case "To":
                        edge.ToId = parser.Consume<Scalar>().Value;
                        break;
                    case "Ending":
                        edge.Ending = FromString(parser.Consume<Scalar>().Value);
                        break;
                    default:
                        // Skip unknown properties
                        parser.SkipThisAndNestedEvents();
                        break;
                }
            }
            parser.Consume<MappingEnd>();

            return edge;
        }

        private Ending FromString(string value)
        {
            if (value == "OpenArrow")
            {
                return Ending.OpenArrow;
            }
            if (value == "ClosedArrow")
            {
                return Ending.ClosedArrow;
            }
            if (value == "Rhombus")
            {
                return Ending.Rhombus;
            }

            throw new InvalidEnumArgumentException($"No such ending <{value}>");
        }

        public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
