using Gwiz.Core.Contract;
using Gwiz.Core.Serializer;
using Moq;
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

        [Test]
        public void Serialize_WhenNodeHasGridWithText_ThenTextIsSavedAsContent()
        {
            // Arrange
            var gridMock = new Mock<IUpdatableGrid>();

            IGridCell[][] gridCells = new GridCell[2][];
            gridCells[0] = new GridCell[2];
            gridCells[1] = new GridCell[2];

            gridCells[0][0] = new GridCell(false)
            {
                Text = "foo",
            };
            gridCells[0][1] = new GridCell(false)
            {
                Text = "bar",
            };
            gridCells[1][0] = new GridCell(false)
            {
                Text = "buz",
            };
            gridCells[1][1] = new GridCell(false)
            {
                Text = "qux",
            };

            gridMock.Setup(gridMock => gridMock.Cells).Returns(gridCells);
            gridMock.Setup(p => p.Rows).Returns(new List<string> {"1", "1"});
            gridMock.Setup(p => p.Cols).Returns(new List<string> {"1", "1"});
            var node = new Node()
            {
                Id = "TestNode",
                UpdateableGrid = gridMock.Object,
                TemplateName = "foo",
            };

            var serializer = new SerializerBuilder()
                .WithTypeConverter(new NodeConverter())
                .Build();

            // Act
            var yaml = serializer.Serialize(node);

            // Assert
            Assert.That(yaml.Contains("Content"));
            Assert.That(yaml.Contains("Row: 0"));
            Assert.That(yaml.Contains("Row: 1"));
            Assert.That(yaml.Contains("Col: 0"));
            Assert.That(yaml.Contains("Col: 1"));
            Assert.That(yaml.Contains("Text: foo"));
            Assert.That(yaml.Contains("Text: bar"));
            Assert.That(yaml.Contains("Text: buz"));
            Assert.That(yaml.Contains("Text: qux"));
        }

    }
}
