using Gwiz.Core.Contract;
using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Gwiz.Core.Serializer
{
    internal class EdgeTemplateConverter : EdgeConverterBase, IYamlTypeConverter
    {
        public bool Accepts(Type type) => typeof(IEdgeTemplate).IsAssignableFrom(type);

        public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
        {
            var edgeTemplate = new EdgeTemplate();

            parser.Consume<MappingStart>();

            while (parser.TryConsume<Scalar>(out var key))
            {
                switch (key.Value)
                {
                    case "Beginning":
                        edgeTemplate.Beginning = StringToEnding(parser.Consume<Scalar>().Value);
                        break;
                    case "Ending":
                        edgeTemplate.Ending = StringToEnding(parser.Consume<Scalar>().Value);
                        break;
                    case "Icon":
                        edgeTemplate.Icon = parser.Consume<Scalar>().Value;
                        break;
                    case "Source":
                        edgeTemplate.Source = parser.Consume<Scalar>().Value;
                        break;
                    case "Target":
                        edgeTemplate.Target = parser.Consume<Scalar>().Value;
                        break;
                    case "Text":
                        edgeTemplate.Text = parser.Consume<Scalar>().Value;
                        break;
                    default:
                        // Skip unknown properties
                        parser.SkipThisAndNestedEvents();
                        break;
                }
            }
            parser.Consume<MappingEnd>();

            return edgeTemplate;
        }

        public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
