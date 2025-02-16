using Gwiz.Core.Contract;
using Gwiz.Core.Serializer;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Gwiz.Core.Test.Serializer
{
    [TestFixture]
    public class YamlSerializerTest
    {
        YamlSerializer _sut = new YamlSerializer();


        [Test]
        public void Deserialize_WhenNodesDefinedInYaml_ThenNodesAreInGraph()
        {
            // Arrange
            var yaml =
                "Nodes:\n" +
                "  - X: 10\n" +
                "    Y: 20\n" +
                "    Width: 30\n" +
                "    Height: 40\n" +
                "  - X: 50\n" +
                "    Y: 60\n" +
                "    Width: 70\n" +
                "    Height: 80\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act
            var graph = _sut.Deserialize(stream);

            // Assert
            Assert.That(graph.Nodes.Count, Is.EqualTo(2));

            var node1 = graph.Nodes[0];
            Assert.That(node1.X, Is.EqualTo(10));
            Assert.That(node1.Y, Is.EqualTo(20));
            Assert.That(node1.Width, Is.EqualTo(30));
            Assert.That(node1.Height, Is.EqualTo(40));

            var node2 = graph.Nodes[1];
            Assert.That(node2.X, Is.EqualTo(50));
            Assert.That(node2.Y, Is.EqualTo(60));
            Assert.That(node2.Width, Is.EqualTo(70));
            Assert.That(node2.Height, Is.EqualTo(80));
        }

        [Test]
        public void Deserialize_WhenTemplatesDefinedInYaml_ThenTemplatesAreInGraph()
        {
            // Arrange
            var yaml =
                "Templates:\n" +
                "  - Name: Foo\n" +
                "    BackgroundColor: 40, 10, 20, 30\n" +
                "    LineColor: 20, 5, 10, 15\n" +
                "  - Name: Bar\n" +
                "    BackgroundColor: 80, 20, 40, 60\n" +
                "    LineColor: 40, 10, 20, 30\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act
            var graph = _sut.Deserialize(stream);

            // Assert
            Assert.That(graph.Templates.Count, Is.EqualTo(2));

            var teamplate1 = graph.Templates[0];
            Assert.That(teamplate1.Name, Is.EqualTo("Foo"));
            Assert.That(teamplate1.BackgroundColor, Is.EqualTo(Color.FromArgb(40, 10, 20, 30)));
            Assert.That(teamplate1.LineColor, Is.EqualTo(Color.FromArgb(20, 5, 10, 15)));

            var teamplate2 = graph.Templates[1];
            Assert.That(teamplate2.Name, Is.EqualTo("Bar"));
            Assert.That(teamplate2.BackgroundColor, Is.EqualTo(Color.FromArgb(80, 20, 40, 60)));
            Assert.That(teamplate2.LineColor, Is.EqualTo(Color.FromArgb(40, 10, 20, 30)));
        }

        [Test]
        public void Deserialize_WhenNodeReferencesExistingTemplate_ThenNodeHasTemplate()
        {
            // Arrange
            var yaml =
                "Templates:\n" +
                "  - Name: Foo\n" +
                "    BackgroundColor: 40, 10, 20, 30\n" +
                "    LineColor: 20, 5, 10, 15\n" +
                "Nodes:\n" +
                "  - X: 10\n" +
                "    Y: 20\n" +
                "    Width: 30\n" +
                "    Height: 40\n" +
                "    Template: Foo\n";
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));
         
            // Act
            var graph = _sut.Deserialize(stream);
            
            // Assert
            var node = graph.Nodes[0];
            Assert.That(node.Template.Name, Is.EqualTo("Foo"));
            Assert.That(node.Template.BackgroundColor, Is.EqualTo(Color.FromArgb(40, 10, 20, 30)));
            Assert.That(node.Template.LineColor, Is.EqualTo(Color.FromArgb(20, 5, 10, 15)));
        }

        [Test]
        public void Deserialize_WhenTemplateHasInvalidTemplate_ThenExceptionIsThrown()
        {
            var yaml =
                "Templates:\n" +
                "  - Name: Foo\n" +
                "    BackgroundColor: 40, 10, 20, 30\n" +
                "    LineColor: 20, 5, 10, 15\n" +
                "Nodes:\n" +
                "  - X: 10\n" +
                "    Y: 20\n" +
                "    Width: 30\n" +
                "    Height: 40\n" +
                "    Template: Bar\n";
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act / Assert
            Assert.Throws<UnknownTemplateReference>(() => _sut.Deserialize(stream));
        }

        [Test]
        public void Deserialize_WhenTemplateResize_ThenCorrectResizeIdInNode()
        {
            var yaml =
                "Templates:\n" +
                "  - Name: Foo\n" +
                "    Resize: HorzVert\n" +
                "  - Name: Bar\n" +
                "    Resize: Both\n" +
                "  - Name: Buz\n" +
                "    Resize: HorzVertBoth\n" +
                "Nodes:\n" +
                "  - Template: Foo\n" +
                "  - Template: Bar\n" +
                "  - Template: Buz\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act
            var graph = _sut.Deserialize(stream);

            // Assert
            Assert.That(graph.Nodes[0].Template.Resize, Is.EqualTo(Resize.HorzVert));
            Assert.That(graph.Nodes[1].Template.Resize, Is.EqualTo(Resize.Both));
            Assert.That(graph.Nodes[2].Template.Resize, Is.EqualTo(Resize.HorzVertBoth));
        }

        [Test]
        public void Deserialize_WhenUnkownResizeIdentifier_ThenExceptionIsThrown()
        {
            var yaml =
                "Templates:\n" +
                "  - Name: Foo\n" +
                "    Resize: Foo\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act / Assert
            try
            {
                _sut.Deserialize(stream);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException;
                if (innerException == null)
                {
                    Assert.Fail();
                }
                else
                { 
                    Assert.That(innerException.GetType, Is.EqualTo(typeof(UnknownTemplateParameterValue)));
                }
            }
        }

        [Test]
        public void Deserialize_WhenNoGridIsDefined_ThenGridHasOneRowWithAsteriks()
        {
            // Arrange
            var yaml =
                "Templates:\n" +
                "  - Name: Foo\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act
            var graph = _sut.Deserialize(stream);

            // Assert
            if (graph.Templates.Count == 0)
            {
                Assert.Fail();
            }
            var template = graph.Templates[0];

            Assert.That(template.Grid.Cols.Count, Is.EqualTo(0));
            Assert.That(template.Grid.Rows.Count, Is.EqualTo(1));
            Assert.That(template.Grid.Rows[0], Is.EqualTo("*"));
        }

        [Test]
        public void Deserialize_WhenGridRowsDefined_ThenGridRowsInTemplate()
        {
            // Arrange
            var yaml =
                "Templates:\n" +
                "  - Name: Foo\n" +
                "    Grid:\n" +
                "      Rows:\n" +
                "        - 300\n" +
                "        - 100\n" +
                "        - auto\n" +
                "        - \"*\"\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act
            var graph = _sut.Deserialize(stream);

            // Assert
            var grid = graph.Templates[0].Grid;

            Assert.That(grid.Rows.Count, Is.EqualTo(4));
            Assert.That(grid.Rows[0], Is.EqualTo("300"));
            Assert.That(grid.Rows[1], Is.EqualTo("100"));
            Assert.That(grid.Rows[2], Is.EqualTo("auto"));
            Assert.That(grid.Rows[3], Is.EqualTo("*"));
        }
    }
}
