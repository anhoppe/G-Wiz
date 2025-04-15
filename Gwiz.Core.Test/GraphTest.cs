using Gwiz.Core.Contract;
using Moq;
using NUnit.Framework;
using System.Drawing;

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
        public void AddNode_WhenAddingNode_ThenSourceAndTargetTemplatesAreAssigned()
        {
            var sut = new Graph();
            sut.Templates.Add(new Template()
            {
                Name = "foo",
            });
            sut.Templates.Add(new Template()
            {
                Name = "bar",
            });

            var edgeTemplate = new EdgeTemplate()
            {
                Source = "foo",
                Target = "bar"
            };

            sut.EdgeTemplates.Add(edgeTemplate);

            // Act
            var fooNode = sut.AddNode("foo");
            var barNode = sut.AddNode("bar");

            Assert.That(fooNode.SourceEdgeTemplates.Contains(edgeTemplate));
            Assert.That(fooNode.TargetEdgeTemplates.Count == 0);
            Assert.That(barNode.SourceEdgeTemplates.Count == 0);
            Assert.That(barNode.TargetEdgeTemplates.Contains(edgeTemplate));
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
        public void AddNode_WhenEdgeTemplateIsDefined_ThenNodeReferencesEdgeTemplate()
        {
            // Arrange
            var edgeTeamplates = new List<EdgeTemplate>()
            {
            };

            var sut = new Graph();

            sut.Templates.Add(new Template()
            {
                Name = "foo",

                Grid = new Grid()
                {
                    Cols = new List<string>() { "1" },
                    Rows = new List<string>() { "1" }
                }
            });

            sut.Templates.Add(new Template()
            {
                Name = "bar",

                Grid = new Grid()
                {
                    Cols = new List<string>() { "1" },
                    Rows = new List<string>() { "1" }
                }
            });

            sut.EdgeTemplates.Add(new EdgeTemplate()
            {
                Source = "foo",
                Target = "bar",
            });

            // Act
            var node1 = sut.AddNode("foo");
            var node2 = sut.AddNode("bar");

            // Assert
            Assert.That(node1.SourceEdgeTemplates.Count, Is.EqualTo(1));
            Assert.That(node1.TargetEdgeTemplates.Count, Is.EqualTo(0));
            Assert.That(node2.SourceEdgeTemplates.Count, Is.EqualTo(0));
            Assert.That(node2.TargetEdgeTemplates.Count, Is.EqualTo(1));
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

        [Test]
        public void AddEdge_WhenAddingEdgeUsingTemplate_ThenTheEdgeHasParametersFromTemplate()
        {
            // Arrange
            var node1Mock = new Mock<IUpdatableNode>();
            var node2Mock = new Mock<IUpdatableNode>();

            var sut = new Graph()
            {
                Nodes = [node1Mock.Object, node2Mock.Object],
            };

            var edgeTemplateMock = new Mock<IEdgeTemplate>();
            edgeTemplateMock.Setup(p => p.Beginning).Returns(Ending.ClosedArrow);
            edgeTemplateMock.Setup(p => p.Ending).Returns(Ending.Rhombus);
            edgeTemplateMock.Setup(p => p.Style).Returns(Style.Dashed);
            edgeTemplateMock.Setup(p => p.Text).Returns("buz");

            // Act
            sut.AddEdge(node1Mock.Object, node2Mock.Object, edgeTemplateMock.Object);

            // Assert
            Assert.That(sut.Edges.Count, Is.EqualTo(1));

            var edge = sut.Edges[0];
            Assert.That(edge.Beginning, Is.EqualTo(Ending.ClosedArrow));
            Assert.That(edge.Ending, Is.EqualTo(Ending.Rhombus));
            Assert.That(edge.Style, Is.EqualTo(Style.Dashed));
            Assert.That(edge.Text, Is.EqualTo("buz"));

        }
    }
}
