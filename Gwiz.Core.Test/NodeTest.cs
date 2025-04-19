using NUnit.Framework;
using System.Drawing;
using YamlDotNet.RepresentationModel;

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

        [Test]
        public void SelectedChanged_WhenSelectIsSet_ThenEventIsRaised()
        {
            // Arrange
            var node = new Node();

            bool isSelected = false;
            node.SelectedChanged += (s, e) =>
            {
                isSelected = e;
            };

            // Act
            node.Select = true;

            // Assert
            Assert.That(isSelected, "Expected that event handler (s. a.) was called which sets isSelected to true");
        }
    }
}
