using Gwiz.Core.Contract;
using Gwiz.Core.Serializer;
using NUnit.Framework;
using System.Drawing;
using YamlDotNet.Serialization;

namespace Gwiz.Core.Test.Serializer
{
    [TestFixture]
    public class NodeConverterTest
    {
        [Test]
        public void Serialize_WhenInstantiatedNodeIsSerialized_ThenOnlyExpectedInformationIsInYaml()
        {
            // Arrange
            var node = new Node()
            {
                Id = "TestNode",
                X = 10,
                Y = 20,
                Width = 100,
                Height = 200,
                LineColor = Color.Red,
                Resize = Resize.Both,
                Shape = Shape.Ellipse,
                TemplateName = "foo",
            };

            var serializer = new SerializerBuilder()
                .WithTypeConverter(new NodeConverter())
                .Build();

            // Act
            var yaml = serializer.Serialize(node);

            // Assert
            Assert.That(yaml.Contains("Id: TestNode"));
            Assert.That(yaml.Contains("X: 10"));
            Assert.That(yaml.Contains("Y: 20"));
            Assert.That(yaml.Contains("Width: 100"));
            Assert.That(yaml.Contains("Height: 200"));
            Assert.That(yaml.Contains("Template: foo"));

            Assert.That(!yaml.Contains("LineColor"));
            Assert.That(!yaml.Contains("Resize"));

        }

    }
}
