using Gwiz.Core.Contract;
using System;
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
                    default:
                        // Skip unknown properties
                        parser.SkipThisAndNestedEvents();
                        break;
                }
            }
            parser.Consume<MappingEnd>();

            return edge;
        }
        
        public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
