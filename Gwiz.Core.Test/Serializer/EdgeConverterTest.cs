using Gwiz.Core.Contract;
using Gwiz.Core.Serializer;
using NUnit.Framework;
using YamlDotNet.Serialization;

namespace Gwiz.Core.Test.Serializer
{
    [TestFixture]
    public class EdgeConverterTest
    {
        [Test]
        public void Serialize_WhenInstantiatedEdgeIsSerialized_ThenOnlyExpectedInformationIsInYaml()
        {
            // Arrange
            var edge = new Edge()
            {
                FromId = "TestNode1",
                FromDocking = Direction.Top,
                FromDockingPosition = 7,
                ToDocking = Direction.Bottom,
                ToDockingPosition = 13,
                ToId = "TestNode2",
                FromLabel = "FromLabel",
                ToLabel = "ToLabel",
                Text = "Text",
                LabelOffsetPerCent = 0.5f,
                Style = Style.Dashed,
            };

            var serializer = new SerializerBuilder()
                .WithTypeConverter(new EdgeConverter())
                .Build();

            // Act
            var yaml = serializer.Serialize(edge);

            // Assert
            Assert.That(yaml.Contains("From: TestNode1"));
            Assert.That(yaml.Contains("FromDocking: Top"));
            Assert.That(yaml.Contains("FromDockingPos: 7"));
            Assert.That(yaml.Contains("To: TestNode2"));
            Assert.That(yaml.Contains("ToDocking: Bottom"));
            Assert.That(yaml.Contains("ToDockingPos: 13"));
            Assert.That(yaml.Contains("FromLabel: FromLabel"));
            Assert.That(yaml.Contains("ToLabel: ToLabel"));
            Assert.That(yaml.Contains("Text: Text"));
            Assert.That(yaml.Contains("LabelOffsetPerCent: 0.5"));
            Assert.That(yaml.Contains("Style: Dashed"));
        }
    }
}
