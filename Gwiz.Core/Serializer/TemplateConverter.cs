using Gwiz.Core.Contract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Gwiz.Core.Serializer
{
    internal class TemplateConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type) => type == typeof(Template);

        public object ReadYaml(IParser parser, Type type, ObjectDeserializer deserializer)
        {
            var template = new Template();
            
            parser.Consume<MappingStart>();

            while (parser.TryConsume<Scalar>(out var key))
            {
                switch (key.Value)
                {
                    case "BackgroundColor":
                        template.BackgroundColor = ColorTranslator.FromHtml(parser.Consume<Scalar>().Value);
                        break;
                    case "Grid":
                        template.Grid = (Grid)(deserializer(typeof(Grid)) ?? new Grid());
                        break;
                    case "LineColor":
                        template.LineColor = ColorTranslator.FromHtml(parser.Consume<Scalar>().Value);
                        break;
                    case "Name":
                        template.Name = parser.Consume<Scalar>().Value;
                        break;
                    case "Resize":
                        template.ResizeName = parser.Consume<Scalar>().Value;
                        break;
                    default:
                        // Skip unknown properties (including Template if present)
                        parser.SkipThisAndNestedEvents();
                        break;
                }
            }
            
            parser.Consume<MappingEnd>();

            template.ResolveResize();

            return template;
        }

        public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
