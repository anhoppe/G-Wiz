using Gwiz.Core.Contract;
using System;
using System.ComponentModel;
using System.Globalization;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Gwiz.Core.Serializer
{
    internal class EdgeConverter : EdgeConverterBase, IYamlTypeConverter
    {
        public bool Accepts(Type type) => typeof(IEdge).IsAssignableFrom(type);

        public object ReadYaml(IParser parser, Type type, ObjectDeserializer deserializer)
        {
            var edge = new Edge();

            parser.Consume<MappingStart>();
            while (parser.TryConsume<Scalar>(out var key))
            {
                switch (key.Value)
                {
                    case "Beginning":
                        edge.Beginning = StringToEnding(parser.Consume<Scalar>().Value);
                        break;
                    case "Ending":
                        edge.Ending = StringToEnding(parser.Consume<Scalar>().Value);
                        break;
                    case "From":
                        edge.FromId = parser.Consume<Scalar>().Value;
                        break;
                    case "FromLabel":
                        edge.FromLabel = parser.Consume<Scalar>().Value;
                        break;
                    case "LabelOffsetPerCent":
                        edge.LabelOffsetPerCent = float.Parse(parser.Consume<Scalar>().Value, CultureInfo.InvariantCulture);
                        break;
                    case "Style":
                        edge.Style = StringToStyle(parser.Consume<Scalar>().Value);
                        break;
                    case "Text":
                        edge.Text = parser.Consume<Scalar>().Value;
                        break;
                    case "To":
                        edge.ToId = parser.Consume<Scalar>().Value;
                        break;
                    case "ToLabel":
                        edge.ToLabel = parser.Consume<Scalar>().Value;
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
            if (value is not Edge edge)
            {
                throw new ArgumentException("Expected an Edge instance.", nameof(value));
            }

            emitter.Emit(new MappingStart());

            emitter.Emit(new Scalar("Ending"));
            serializer(EndingToString(edge.Ending));

            emitter.Emit(new Scalar("From"));
            serializer(edge.FromId);

            emitter.Emit(new Scalar("FromLabel"));
            serializer(edge.FromLabel);

            emitter.Emit(new Scalar("LabelOffsetPerCent"));
            serializer(edge.LabelOffsetPerCent.ToString(CultureInfo.InvariantCulture));

            emitter.Emit(new Scalar("Style"));
            serializer(StyleToString(edge.Style));

            emitter.Emit(new Scalar("Text"));
            serializer(edge.Text);

            emitter.Emit(new Scalar("To"));
            serializer(edge.ToId);

            emitter.Emit(new Scalar("ToLabel"));
            serializer(edge.ToLabel);

            emitter.Emit(new MappingEnd());
        }

        private string EndingToString(Ending value)
        {
            return value switch
            {
                Ending.None => "None",
                Ending.OpenArrow => "OpenArrow",
                Ending.ClosedArrow => "ClosedArrow",
                Ending.Rhombus => "Rhombus",
                _ => throw new InvalidEnumArgumentException($"No such ending <{value}>"),
            };
        }

        private Style StringToStyle(string value)
        {
            if (value == "None")
            {
                return Style.None;
            }
            if (value == "Dashed")
            {
                return Style.Dashed;
            }
            if (value == "Dotted")
            {
                return Style.Dotted;
            }

            throw new InvalidEnumArgumentException($"No such style <{value}>");
        }

        private string StyleToString(Style value)
        {
            return value switch
            {
                Style.None => "None",
                Style.Dashed => "Dashed",
                Style.Dotted => "Dotted",
                _ => throw new InvalidEnumArgumentException($"No such style <{value}>"),
            };
        }
    }
}
