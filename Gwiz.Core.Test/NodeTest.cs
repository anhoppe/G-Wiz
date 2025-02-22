using Moq;
using NUnit.Framework;

namespace Gwiz.Core.Test
{
    [TestFixture]
    public class NodeTest
    {
        [Test]
        public void SizeAndPosition_WhenSizeAndPosAreModified_ThenUpdateFieldRectsIsCalledForEachChange()
        {
            // Arrange
            var sut = new Node();
            var grid = new Mock<IUpdateableGrid>();

            sut.UpdateableGrid = grid.Object;
            
            // Act
            sut.Width = 200;
            sut.Height = 200;
            sut.X = 1;
            sut.Y = 1;
            
            // Assert
            grid.Verify(p => p.UpdateFieldRects(sut), Times.Exactly(5), "It was expected that the method is called 4 times since each pos/size update is supposed to update the field rectangels and once when the property is set which is needed to have the field rects correct after initialization");
        }
    }
}
