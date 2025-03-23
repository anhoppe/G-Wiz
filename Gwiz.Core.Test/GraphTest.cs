using Gwiz.Core.Contract;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwiz.Core.Test
{
    [TestFixture]
    public class GraphTest
    {
        [Test]
        public void AddNode_WhenAddingSingleNode_ThenNodeInGraph()
        {
            // Arrange
            var sut = new Graph();

            sut.Templates.Add(new Template()
            {
                Name = "foo"
            });

            // Act
            var node = sut.AddNode("foo");

            // Assert
            Assert.That(sut.Nodes.Count, Is.EqualTo(1));
        }

        [Test]
        public void AddNode_WhenAddingNodeWithTemplate_ThenTemplateIsAssignedToNode()
        {
            var sut = new Graph();
            sut.Templates.Add(new Template()
            {
                Name = "foo",
                BackgroundColor = Color.Red,
                LineColor = Color.Blue,
                Resize = Resize.HorzVertBoth,
            });

            // Act
            var node = sut.AddNode("foo");

            // Assert
            Assert.That(node.BackgroundColor, Is.EqualTo(Color.Red));
            Assert.That(node.LineColor, Is.EqualTo(Color.Blue));
            Assert.That(node.Resize, Is.EqualTo(Resize.HorzVertBoth));
        }

        [Test]
        public void AddNode_WhenAddingNodeWithTemplate_ThenNodeHasTGridFromTemplate()
        {
            // Arrange
            var sut = new Graph();
            sut.Templates.Add(new Template()
            {
                Name = "foo",

                Grid = new Grid()
                {
                    Cols = new List<string>() { "1", "2" },
                    Rows = new List<string>() { "1", "2" }
                }
            });
         
            // Act
            var node = sut.AddNode("foo");
            
            // Assert
            Assert.That(node.Grid.Cols.Count, Is.EqualTo(2));
            Assert.That(node.Grid.Rows.Count, Is.EqualTo(2));
        }

        [Test]
        public void AddEdge_WhenAddingEdgeWithNodes_ThenEdgeHasFromAndToSet()
        {
            // Arrange
            var node1Mock = new Mock<IUpdatableNode>();
            var node2Mock = new Mock<IUpdatableNode>();

            var sut = new Graph()
            {
                Nodes = [node1Mock.Object, node2Mock.Object],
            };

            // Act
            sut.AddEdge(node1Mock.Object, node2Mock.Object);

            // Assert
            Assert.That(sut.Edges.Count, Is.EqualTo(1));

            var edge = sut.Edges[0];
            Assert.That(edge.From, Is.EqualTo(node1Mock.Object));
            Assert.That(edge.To, Is.EqualTo(node2Mock.Object));
        }

        [Test]
        public void AddEdge_WhenAddingEdgeWithFromToLabelsAndOffset_ThenEdgeHasFromAndToLabelsSet()
        {
            // Arrange
            var node1Mock = new Mock<IUpdatableNode>();
            var node2Mock = new Mock<IUpdatableNode>();

            var sut = new Graph()
            {
                Nodes = [node1Mock.Object, node2Mock.Object],
            };

            // Act
            sut.AddEdge(node1Mock.Object, node2Mock.Object, "foo", "bar", 7);

            // Assert
            Assert.That(sut.Edges.Count, Is.EqualTo(1));

            var edge = sut.Edges[0];
            Assert.That(edge.FromLabel, Is.EqualTo("foo"));
            Assert.That(edge.ToLabel, Is.EqualTo("bar"));
            Assert.That(edge.LabelOffsetPerCent, Is.EqualTo(7));
        }
    }
}
