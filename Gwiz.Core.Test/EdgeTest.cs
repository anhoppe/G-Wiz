using Moq;
using NUnit.Framework;

namespace Gwiz.Core.Test
{
    [TestFixture]
    public class EdgeTest
    {
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
    }
}
