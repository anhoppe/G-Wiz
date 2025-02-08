using Gwiz.Core.Serializer;
using NUnit.Framework;
using System;
using System.Collections.Generic;
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
        public void Deserialize_WhenNodesDefinedInYaml_ThenNodeAreInGraph()
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
    }
}
