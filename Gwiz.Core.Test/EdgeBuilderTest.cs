using Gwiz.Core.Contract;
using Moq;
using NUnit.Framework;

namespace Gwiz.Core.Test
{
    [TestFixture]
    public class EdgeBuilderTest
    {
        [Test]
        public void Build_WhenAddingEdgeWithNodes_ThenEdgeHasFromAndToSetAndUpdateEdgeIsCalled()
        {
            // Arrange
            var node1Mock = new Mock<IUpdatableNode>();
            var node2Mock = new Mock<IUpdatableNode>();

            node1Mock.Setup(p => p.Id).Returns("node1");
            node2Mock.Setup(p => p.Id).Returns("node2");

            node1Mock.Setup(p => p.X).Returns(100);
            node1Mock.Setup(p => p.Width).Returns(10);
            node1Mock.Setup(p => p.Height).Returns(10);

            node2Mock.Setup(p => p.X).Returns(0);
            node2Mock.Setup(p => p.Width).Returns(10);
            node2Mock.Setup(p => p.Height).Returns(10);

            var edges = new List<IEdge>();

            var sut = new EdgeBuilder(node1Mock.Object, node2Mock.Object, edges);

            // Act
            sut.Build();

            // Assert
            Assert.That(edges.Count, Is.EqualTo(1));

            var edge = edges[0] as Edge;
            if (edge == null)
            {
                throw new NullReferenceException();
            }
            Assert.That(edge.From, Is.EqualTo(node1Mock.Object));
            Assert.That(edge.To, Is.EqualTo(node2Mock.Object));
            Assert.That(edge.FromId, Is.EqualTo("node1"));
            Assert.That(edge.ToId, Is.EqualTo("node2"));

            Assert.That(edge.FromPosition.X, Is.EqualTo(100), "Expected the FromPosition to be set by the Edge.UpdateEdge Method");
        }

        [Test]
        public void WithFromLabel_WhenAddingEdgeWithFromToLabelsAndOffset_ThenEdgeHasFromAndToLabelsSet()
        {
            // Arrange
            var node1Mock = new Mock<IUpdatableNode>();
            var node2Mock = new Mock<IUpdatableNode>();
            var edges = new List<IEdge>();

            var sut = new EdgeBuilder(node1Mock.Object, node2Mock.Object, edges);

            // Act
            sut.WithFromLabel("foo").
                WithToLabel("bar").
                WithLabelOffsetPerCent(7).
                Build();

            // Assert
            Assert.That(edges.Count, Is.EqualTo(1));

            var edge = edges[0];
            Assert.That(edge.FromLabel, Is.EqualTo("foo"));
            Assert.That(edge.ToLabel, Is.EqualTo("bar"));
            Assert.That(edge.LabelOffsetPerCent, Is.EqualTo(7));
        }

        [Test]
        public void WithTemplate_WhenAddingEdgeUsingTemplate_ThenTheEdgeHasParametersFromTemplate()
        {
            // Arrange
            var node1Mock = new Mock<IUpdatableNode>();
            var node2Mock = new Mock<IUpdatableNode>();
            var edges = new List<IEdge>();

            var sut = new EdgeBuilder(node1Mock.Object, node2Mock.Object, edges);

            var edgeTemplateMock = new Mock<IEdgeTemplate>();
            edgeTemplateMock.Setup(p => p.Beginning).Returns(Ending.ClosedArrow);
            edgeTemplateMock.Setup(p => p.Ending).Returns(Ending.Rhombus);
            edgeTemplateMock.Setup(p => p.Style).Returns(Style.Dashed);
            edgeTemplateMock.Setup(p => p.Text).Returns("buz");

            // Act
            sut.WithTemplate(edgeTemplateMock.Object).
                Build();

            // Assert
            Assert.That(edges.Count, Is.EqualTo(1));

            var edge = edges[0];
            Assert.That(edge.Beginning, Is.EqualTo(Ending.ClosedArrow));
            Assert.That(edge.Ending, Is.EqualTo(Ending.Rhombus));
            Assert.That(edge.Style, Is.EqualTo(Style.Dashed));
            Assert.That(edge.Text, Is.EqualTo("buz"));
        }
    }
}
