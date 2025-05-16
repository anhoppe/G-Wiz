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
        public void AddEdge_WhenAddingEdge_ThenEdgeBuilderIsReturned()
        {
            // Arrange
            var node1Mock = new Mock<IUpdatableNode>();
            var node2Mock = new Mock<IUpdatableNode>();

            var edgeBuilderMock = new Mock<IEdgeBuilder>();

            bool edgeBuilderCreated = false;
            var sut = new Graph()
            {
                EdgeBuilderFactory = (IUpdatableNode from, IUpdatableNode to, IList<IEdge> edges) => 
                {
                    if (from == node1Mock.Object && to == node2Mock.Object)
                    {
                        edgeBuilderCreated = true;
                    }
                    return edgeBuilderMock.Object;
                },
                Nodes = [node1Mock.Object, node2Mock.Object]
            };

            // Act
            var result = sut.AddEdge(node1Mock.Object, node2Mock.Object);
            result.Build();

            // Assert
            Assert.That(edgeBuilderCreated);
            Assert.That(result, Is.EqualTo(edgeBuilderMock.Object));
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
        public void AddNode_WhenAddingNode_ThenTheNodeHasAnId()
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
            Assert.That(!string.IsNullOrEmpty(node.Id));
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
            var node = sut.AddNode("foo") as Node;

            // Assert
            if (node == null)
            {
                throw new NullReferenceException();
            }
            Assert.That(node.BackgroundColor, Is.EqualTo(Color.Red));
            Assert.That(node.LineColor, Is.EqualTo(Color.Blue));
            Assert.That(node.Resize, Is.EqualTo(Resize.HorzVertBoth));
            Assert.That(node.TemplateName, Is.EqualTo("foo"));
        }

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
        public void Remove_WhenEdgeIsRemoved_ThenEdgeRemovedFromGraph()
        {
            // Arrange
            var edge1 = new Mock<IEdge>();
            var edge2 = new Mock<IEdge>();

            var sut = new Graph()
            {
                Edges = [edge1.Object, edge2.Object],
            };

            // Act
            sut.Remove(edge1.Object);

            // Assert
            Assert.That(sut.Edges.Count, Is.EqualTo(1));
            Assert.That(sut.Edges[0], Is.EqualTo(edge2.Object));
        }

        [Test]
        public void Remove_WhenNodeIsRemoved_ThenNodeAndConnectedEdgesAreRemoved()
        {
            // Arrange
            var node = new Mock<INode>();

            var edge1 = new Mock<IEdge>();
            var edge2 = new Mock<IEdge>();

            edge1.Setup(p => p.From).Returns(node.Object);
            edge2.Setup(p => p.To).Returns(node.Object);

            var sut = new Graph()
            {
                Edges = [edge1.Object, edge2.Object],
                Nodes = [node.Object],
            };

            // Act
            sut.Remove(node.Object);

            // Assert
            Assert.That(!sut.Nodes.Any());
            Assert.That(!sut.Edges.Any());
        }

        [Test]
        public void Remove_WhenNodeIsRemoved_ThenNodeRemovedEventIsRaised()
        {
            // Arrange
            var nodeMock = new Mock<INode>();

            var sut = new Graph()
            {
                Nodes = [nodeMock.Object],
            };

            bool nodeRemoved = false;
            sut.NodeRemoved += (sender, node) =>
            {
                if (node == nodeMock.Object)
                {
                    nodeRemoved = true;
                }
            };

            // Act
            sut.Remove(nodeMock.Object);

            // Assert
            Assert.That(nodeRemoved);
        }
    }
}
