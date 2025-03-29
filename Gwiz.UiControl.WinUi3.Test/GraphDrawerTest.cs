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

        [Test]
        public void EdgeLabels_WhenEdgeHasFromToLabelsDefined_ThenTheLabelsAreDrawn()
        {
            // Arrange
            var edgeMock = new Mock<IEdge>();

            var from = new Point(10, 0);
            var to = new Point(10, 30);
            edgeMock.SetupGet(x => x.FromPosition).Returns(from);
            edgeMock.SetupGet(x => x.ToPosition).Returns(to);

            edgeMock.SetupGet(x => x.FromLabel).Returns("foo");
            edgeMock.SetupGet(x => x.ToLabel).Returns("bar");

            _sut.Edges = [edgeMock.Object];

            // Act
            _sut.DrawGraph();

            // Assert
            _drawMock.Verify(m => m.DrawText("foo", It.Is<Point>(p => p == from), It.IsAny<Color>()));
            _drawMock.Verify(m => m.DrawText("bar", It.Is<Point>(p => p == to), It.IsAny<Color>()));
        }

        [Test]
        public void EdgeLabels_WhenEdgeLabelIsDefinedWithOffset_ThenLabelAtCorrectPosition()
        {
            // Arrange
            var edgeMock = new Mock<IEdge>();
            var from = new Point(0, 0);
            var to = new Point(100, 0);

            edgeMock.SetupGet(x => x.FromPosition).Returns(from);
            edgeMock.SetupGet(x => x.ToPosition).Returns(to);

            edgeMock.SetupGet(x => x.FromLabel).Returns("foo");
            edgeMock.SetupGet(x => x.ToLabel).Returns("bar");

            edgeMock.SetupGet(x => x.LabelOffsetPerCent).Returns(5);

            _sut.Edges = [edgeMock.Object];

            var fromLabelExpectedPos = new Point(5, 0);
            var toLabelExpectedPos = new Point(95, 0);

            // Act
            _sut.DrawGraph();

            // Assert
            _drawMock.Verify(m => m.DrawText("foo", It.Is<Point>(p => p == fromLabelExpectedPos), It.IsAny<Color>()));
            _drawMock.Verify(m => m.DrawText("bar", It.Is<Point>(p => p == toLabelExpectedPos), It.IsAny<Color>()));
        }

        [TestCase(Alignment.TopLeft, 0, 0)]
        [TestCase(Alignment.TopCenter, 50, 0)]
        [TestCase(Alignment.TopRight, 100, 0)]
        [TestCase(Alignment.CenterLeft, 0, 50)]
        [TestCase(Alignment.CenterCenter, 50, 50)]
        [TestCase(Alignment.CenterRight, 100, 50)]
        [TestCase(Alignment.BottomLeft, 0, 100)]
        [TestCase(Alignment.BottomCenter, 50, 100)]
        [TestCase(Alignment.BottomRight, 100, 100)]

        public void GridText_WhenAlignemntSet_ThenTextPlacedAccordingly(Alignment alignment, int expectedX, int expectedY)
        {
            var nodeMock = new Mock<INode>();
            var rect = new Rectangle(0, 0, 200, 200);
            var gridMock = MockGrid("foo", rect);

            nodeMock.Setup(p => p.Grid).Returns(gridMock.Object);
            nodeMock.Setup(p => p.Alignment).Returns(alignment);

            _sut.Nodes = [nodeMock.Object];

            _sut.TextSizeCalculator = (text) => text == "foo" ? new Size(100, 100) : new Size(0, 0);

            // Act
            _sut.DrawGraph();

            // Assert
            var expectedTextPos = new Point(expectedX, expectedY);
            _drawMock.Verify(m => m.DrawClippedText("foo", rect, expectedTextPos, It.IsAny<Color>()));
        }

        [Test]
        public void TextSizeCalculator_WhenMultipleLinesOfSameText_ThenTextWidthIsEqual()
        {
            // Arrange
            var text1 = "HelloWorld\n";
            var text2 = "HelloWorld\nHelloWorld\nHelloWorld\nHelloWorld\nHelloWorld\n";

            // Act
            var size1 = _sut.TextSizeCalculator.Invoke(text1);
            var size2 = _sut.TextSizeCalculator.Invoke(text2);

            // Assert
            Assert.That(size1.Width, Is.EqualTo(size2.Width));
        }

        [Test]
        public void Shape_WhenEllispeShapeIsDefined_ThenNodeIsEllipse()
        {
            // Arrange
            var nodeMock = new Mock<INode>();

            nodeMock.Setup(p => p.Shape).Returns(Shape.Ellipse);
            nodeMock.Setup(p => p.X).Returns(13);
            nodeMock.Setup(p => p.Y).Returns(23);
            nodeMock.Setup(p => p.Width).Returns(100);
            nodeMock.Setup(p => p.Height).Returns(100);

            var gridMock = MockGrid("barfoo", new Rectangle(13, 23, 100, 100));
            nodeMock.Setup(p => p.Grid).Returns(gridMock.Object);

            _sut.Nodes = [nodeMock.Object];

            // Act
            _sut.DrawGraph();

            // Assert
            var expectedRect = new Rectangle(13, 23, 100, 100);
            _drawMock.Verify(m => m.DrawEllipse(expectedRect, It.IsAny<Color>()));
            _drawMock.Verify(m => m.FillEllipse(expectedRect, It.IsAny<Color>()));
            _drawMock.Verify(m => m.DrawClippedText("barfoo", It.IsAny<Rectangle>(), It.IsAny<Point>(), It.IsAny<Color>()));
        }

        [Test]
        public void TextSizeCalculator_WhenTextIsLargerThanTheGridCell_ThenTextStartsAtTopLeftCornerOfCell()
        {
            // Arrange
            var nodeMock = new Mock<INode>();
            var rect = new Rectangle(10, 10, 50, 50);
            var gridMock = MockGrid("foo", rect);

            nodeMock.Setup(p => p.Grid).Returns(gridMock.Object);
            _sut.Nodes = [nodeMock.Object];

            _sut.TextSizeCalculator = (text) => text == "foo" ? new Size(100, 100) : new Size(0, 0);

            // Act
            _sut.DrawGraph();

            // Assert
            var expectedTextPos = new Point(10, 10);
            _drawMock.Verify(m => m.DrawClippedText("foo", rect, expectedTextPos, It.IsAny<Color>()), "Expected the text to be positioned at the grid coordinates becase the text is larger then the grid cell");
        }

        private Mock<IGrid> MockGrid(string text, Rectangle rect)
        {
            var gridMock = new Mock<IGrid>();

            gridMock.SetupGet(p => p.Cols).Returns(["1"]);
            gridMock.SetupGet(p => p.Rows).Returns(["1"]);

            // This is the expected field size for a class node (1 column with 3 rows for title, props and methods)
            var fieldText = new string[1][];
            fieldText[0] = new string[1];
            fieldText[0][0] = text;
            gridMock.Setup(p => p.FieldText).Returns(fieldText);

            var fieldRects = new Rectangle[1][];
            fieldRects[0] = new Rectangle[1];
            fieldRects[0][0] = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
            gridMock.Setup(p => p.FieldRects).Returns(fieldRects);

            return gridMock;
        }
    }
}