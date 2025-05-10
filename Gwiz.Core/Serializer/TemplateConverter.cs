using Gwiz.Core.Contract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
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
                    case "Alignment":
                        template.Alignment = (parser.Consume<Scalar>().Value).ToAlignment();
                        break;
                    case "BackgroundColor":
                        template.BackgroundColor = ColorTranslator.FromHtml(parser.Consume<Scalar>().Value);
                        break;
                    case "Buttons":
                        template.ButtonDto = ((List<ButtonDto>)(deserializer(typeof(List<ButtonDto>)) ?? new List<ButtonDto>()));
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
                        template.Resize = (parser.Consume<Scalar>().Value).ToResize();
                        break;
                    case "Shape":
                        template.Shape = (parser.Consume<Scalar>().Value).ToShape();
                        break;
                    default:
                        // Skip unknown properties (including Template if present)
                        parser.SkipThisAndNestedEvents();
                        break;

                }

                ResolveButtons(template);
            }

            parser.Consume<MappingEnd>();

            return template;
        }

        private void ResolveButtons(Template template)
        {
            foreach(var buttonDto in template.ButtonDto)
            {
                template.Buttons.Add(new Button(buttonDto));
            }
        }

        public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
