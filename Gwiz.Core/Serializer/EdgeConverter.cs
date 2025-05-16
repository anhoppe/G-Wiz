using Gwiz.Core.Contract;
using System;
using System.Globalization;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Gwiz.Core.Serializer
{
    internal class EdgeConverter : IYamlTypeConverter
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
                        edge.Beginning = (parser.Consume<Scalar>().Value).ToEnding();
                        break;
                    case "Ending":
                        edge.Ending = (parser.Consume<Scalar>().Value).ToEnding();
                        break;
                    case "From":
                        edge.FromId = parser.Consume<Scalar>().Value;
                        break;
                    case "FromDocking":
                        edge.FromDocking = (parser.Consume<Scalar>().Value).ToDirection();
                        break;
                    case "FromDockingPos":
                        edge.FromDockingPosition = int.Parse(parser.Consume<Scalar>().Value);
                        break;
                    case "FromLabel":
                        edge.FromLabel = parser.Consume<Scalar>().Value;
                        break;
                    case "LabelOffsetPerCent":
                        edge.LabelOffsetPerCent = float.Parse(parser.Consume<Scalar>().Value, CultureInfo.InvariantCulture);
                        break;
                    case "Style":
                        edge.Style = (parser.Consume<Scalar>().Value).ToStyle();
                        break;
                    case "Text":
                        edge.Text = parser.Consume<Scalar>().Value;
                        break;
                    case "To":
                        edge.ToId = parser.Consume<Scalar>().Value;
                        break;
                    case "ToDocking":
                        edge.ToDocking = (parser.Consume<Scalar>().Value).ToDirection();
                        break;
                    case "ToDockingPos":
                        edge.ToDockingPosition = int.Parse(parser.Consume<Scalar>().Value);
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
            serializer(edge.Ending.FromEnding());

            emitter.Emit(new Scalar("From"));
            serializer(edge.FromId);

            emitter.Emit(new Scalar("FromDocking"));
            serializer(edge.FromDocking.FromDirection());

            emitter.Emit(new Scalar("FromDockingPos"));
            serializer(edge.FromDockingPosition.ToString(CultureInfo.InvariantCulture));

            emitter.Emit(new Scalar("FromLabel"));
            serializer(edge.FromLabel);

            emitter.Emit(new Scalar("LabelOffsetPerCent"));
            serializer(edge.LabelOffsetPerCent.ToString(CultureInfo.InvariantCulture));

            emitter.Emit(new Scalar("Style"));
            serializer(edge.Style.FromStyle());

            emitter.Emit(new Scalar("Text"));
            serializer(edge.Text);

            emitter.Emit(new Scalar("To"));
            serializer(edge.ToId);

            emitter.Emit(new Scalar("ToDocking"));
            serializer(edge.ToDocking.FromDirection());

            emitter.Emit(new Scalar("ToDockingPos"));
            serializer(edge.ToDockingPosition.ToString(CultureInfo.InvariantCulture));

            emitter.Emit(new Scalar("ToLabel"));
            serializer(edge.ToLabel);

            emitter.Emit(new MappingEnd());
        }
    }
}
