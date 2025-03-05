using Gwiz.Core.Contract;
using Moq;
using NUnit.Framework;
using System.Drawing;

namespace Gwiz.UiControl.WinUi3.Test
{
    [TestFixture]
    public class GraphDrawerTest
    {
        private Mock<IDraw> _drawMock = new();

        private GraphDrawer _sut = new GraphDrawer();

        [SetUp]
        public void SetUp()
        {
            _drawMock = new Mock<IDraw>();

            _sut = new GraphDrawer()
            {
                Draw = _drawMock.Object,
            };
        }

        [Test]
        public void Edges_DrawEdgeWithOpenArrowEnding_ArrowDrawnAsExpected()
        {
            // Arrange
            var edgeMock = new Mock<IEdge>();

            var from = new Point(10, 0);
            var to = new Point(10, 15);
            edgeMock.SetupGet(x => x.FromPosition).Returns(from);
            edgeMock.SetupGet(x => x.ToPosition).Returns(to);
            edgeMock.Setup(p => p.Ending).Returns(Ending.OpenArrow);

            _sut.Edges = [edgeMock.Object];


            var expectedEndPoint1 = new Point(2, 2);
            var expectedEndPoint2 = new Point(17, 2);

            // Act
            _sut.DrawGraph();

            // Assert

            // Assert correct open arrow head
            _drawMock.Verify(x => x.DrawLine(It.Is<Point>(p => p == to), It.Is<Point>(p => p == expectedEndPoint1), Style.None));
            _drawMock.Verify(x => x.DrawLine(It.Is<Point>(p => p == to), It.Is<Point>(p => p == expectedEndPoint2), Style.None));

            // Assert correct line to arrow head
            _drawMock.Verify(x => x.DrawLine(It.Is<Point>(p => p == from), It.Is<Point>(p => p == to), Style.None));
        }


        [Test]
        public void Edges_DrawEdgeWithClosedArrowEnding_ArrowDrawnAsExpected()
        {
            // Arrange
            var edgeMock = new Mock<IEdge>();

            var from = new Point(10, 0);
            var to = new Point(10, 15);
            edgeMock.SetupGet(x => x.FromPosition).Returns(from);
            edgeMock.SetupGet(x => x.ToPosition).Returns(to);
            edgeMock.Setup(p => p.Ending).Returns(Ending.ClosedArrow);

            _sut.Edges = [edgeMock.Object];


            var expectedEndPoint1 = new Point(2, 2);
            var expectedEndPoint2 = new Point(17, 2);

            // Act
            _sut.DrawGraph();

            // Assert

            // Assert correct open arrow head
            _drawMock.Verify(x => x.DrawLine(It.Is<Point>(p => p == to), It.Is<Point>(p => p == expectedEndPoint1), Style.None));
            _drawMock.Verify(x => x.DrawLine(It.Is<Point>(p => p == to), It.Is<Point>(p => p == expectedEndPoint2), Style.None));

            // In addition, the arrow head is closed by a line
            _drawMock.Verify(x => x.DrawLine(It.Is<Point>(p => p == expectedEndPoint2), It.Is<Point>(p => p == expectedEndPoint1), Style.None));


            // Assert correct line to arrow head. In this case, the line is drawn to the end of the arrow head
            var modifiedTo = new Point(9, 2);
            _drawMock.Verify(x => x.DrawLine(It.Is<Point>(p => p == from), It.Is<Point>(p => p == modifiedTo), Style.None));
        }

        [Test]
        public void Edges_DrawEdgeWithRhombusEnding_RhombusDrawnAsExpected()
        {
            // Arrange
            var edgeMock = new Mock<IEdge>();

            var from = new Point(10, 0);
            var to = new Point(10, 30);
            edgeMock.SetupGet(x => x.FromPosition).Returns(from);
            edgeMock.SetupGet(x => x.ToPosition).Returns(to);
            edgeMock.Setup(p => p.Ending).Returns(Ending.Rhombus);

            _sut.Edges = [edgeMock.Object];


            var expectedEndPoint1 = new Point(2, 17);
            var expectedEndPoint2 = new Point(17, 17);
            var expectedModifiedTo = new Point(10, 4);

            // Act
            _sut.DrawGraph();

            // Assert

            // Assert correct Rhombus head
            _drawMock.Verify(x => x.DrawLine(It.Is<Point>(p => p == to), It.Is<Point>(p => p == expectedEndPoint1), Style.None));
            _drawMock.Verify(x => x.DrawLine(It.Is<Point>(p => p == to), It.Is<Point>(p => p == expectedEndPoint2), Style.None));
            _drawMock.Verify(x => x.DrawLine(It.Is<Point>(p => p == expectedEndPoint1), It.Is<Point>(p => p == expectedModifiedTo), Style.None));
            _drawMock.Verify(x => x.DrawLine(It.Is<Point>(p => p == expectedEndPoint2), It.Is<Point>(p => p == expectedModifiedTo), Style.None));

            // Assert correct line to arrow head. In this case, the line is drawn to the end of the arrow head
            _drawMock.Verify(x => x.DrawLine(It.Is<Point>(p => p == from), It.Is<Point>(p => p == expectedModifiedTo), Style.None));
        }
    }
}
