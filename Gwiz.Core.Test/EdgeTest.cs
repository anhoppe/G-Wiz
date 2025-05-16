using Gwiz.Core.Contract;
using Moq;
using NUnit.Framework;
using System.Drawing;

namespace Gwiz.Core.Test
{
    [TestFixture]
    public class EdgeTest
    {
        [Test]
        public void DockingPosition_WhenDockingPositionIsSet_ThenFromToPositionsCorrect()
        {
            // Arrange
            var node1Mock = new Mock<IUpdatableNode>();
            var node2Mock = new Mock<IUpdatableNode>();

            node1Mock.Setup(p => p.X).Returns(0);
            node1Mock.Setup(p => p.Y).Returns(0);
            node1Mock.Setup(p => p.Width).Returns(100);
            node1Mock.Setup(p => p.Height).Returns(100);

            node2Mock.Setup(p => p.X).Returns(200);
            node2Mock.Setup(p => p.Y).Returns(0);
            node2Mock.Setup(p => p.Width).Returns(100);
            node2Mock.Setup(p => p.Height).Returns(100);

            var edges = new List<IEdge>();
            var sut = new EdgeBuilder(node1Mock.Object, node2Mock.Object, edges);
            sut.WithFromDockingPosition(Direction.Right, 25).
                WithToDockingPosition(Direction.Left, 75).
                Build();

            // Act
            // Assert
            var edge = edges[0];
            Assert.That(edge.FromPosition, Is.EqualTo(new Point(100, 25)));
            Assert.That(edge.ToPosition, Is.EqualTo(new Point(200, 75)));
        }

        [Test]
        public void IsOver_WhenPointOverLine_ThenIsOverReturnsTrue()
        {
            // Arrange
            Edge edge = new Edge()
            {
                FromPosition = new Point(0, 0),
                ToPosition = new Point(0, 100),
            };

            // Act
            bool isOver = edge.IsOver(new Point(2, 50));

            // Assert
            Assert.That(isOver);
        }

        [Test]
        public void IsOver_WhenPointTooFarAwayFromLine_ThenIsOverReturnsFalse()
        {
            // Arrange
            Edge edge = new Edge()
            {
                FromPosition = new Point(0, 0),
                ToPosition = new Point(0, 100),
            };

            // Act
            bool isOver = edge.IsOver(new Point(10, 50));

            // Assert
            Assert.That(!isOver);
        }

        [Test]
        public void IsOver_WhenStartAndEndIdentical_ThenIsOverReturnsFalse()
        {
            // Arrange
            Edge edge = new Edge()
            {
                FromPosition = new Point(0, 0),
                ToPosition = new Point(0, 0),
            };

            // Act
            bool isOver = edge.IsOver(new Point(0, 0));

            // Assert
            Assert.That(!isOver);
        }

        [Test]
        public void UpdateEdge_WhenToNodeChanged_ThenEdgeUpdated()
        {
            // Arrange
            var sut = new Edge();

            sut.FromInternal = new Node()
            {
                X = 10,
                Y = 10,
                Width = 100,
                Height = 100,
            };

            sut.ToInternal = new Node()
            {
                X = 0,
                Y = 10,
                Width = 100,
                Height = 100
            };

            // Act

            // By changing the node position the edge should be updated
            sut.ToInternal.X = 120;

            // Assert
            Assert.That(sut.FromPosition.X, Is.EqualTo(110), "Expected coordinate on the right edge of the left node (intersection point of line from (center of left to center of right) with left edge");
            Assert.That(sut.FromPosition.Y, Is.EqualTo(60));
            Assert.That(sut.ToPosition.X, Is.EqualTo(120));
            Assert.That(sut.ToPosition.Y, Is.EqualTo(60));
        }

        [Test]
        public void UpdateEdge_WhenToNodeIsEllipse_ThenIntersectionPointCorrect()
        {
            var sut = new Edge();

            sut.FromInternal = new Node()
            {
                X = 0,
                Y = 0,
                Width = 100,
                Height = 100,
            };

            sut.ToInternal = new Node()
            {
                X = 0,
                Y = 100,
                Width = 100,
                Height = 100,
                Shape = Shape.Ellipse,
            };

            // Act

            // By changing the node position the edge should be updated
            sut.ToInternal.X = 100;

            // Assert

            // This is the delta
            // -----
            // |   |
            // |   |
            // -----   ..
            //       .    .
            //      .      .  Center of the circle=150, 150, radius=50
            //       .    .
            //         ..
            double expectedDelta = Math.Sin(Math.PI / 4f) * 50;

            Assert.That(sut.ToPosition.X, Is.EqualTo(150 - expectedDelta).Within(0.75));
            Assert.That(sut.ToPosition.Y, Is.EqualTo(150 - expectedDelta).Within(0.75));
        }
    }
}
