using NUnit.Framework;
using System.Drawing;

namespace Gwiz.Core.Test
{
    [TestFixture]
    public class NodeTest
    {
        [Test]
        public void IsOver_WhenPointIsInsideNode_ThenReturnsTrue()
        {
            // Arrange
            var node = new Node
            {
                X = 10,
                Y = 10,
                Width = 100,
                Height = 100
            };
            var point = new Point(50, 50);
         
            // Act
            var result = node.IsOver(point);
            
            // Assert
            Assert.That(result);
        }
    }
}
