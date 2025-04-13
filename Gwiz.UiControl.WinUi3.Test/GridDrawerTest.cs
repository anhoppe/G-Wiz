using Gwiz.Core.Contract;
using Moq;
using NUnit.Framework;
using SkiaSharp;
using System.Drawing;

namespace Gwiz.UiControl.WinUi3.Test
{
    [TestFixture]
    public class GridDrawerTest
    {
        private Mock<IDraw> _drawMock = new();

        private GridDrawer _sut = new GridDrawer();

        [SetUp]
        public void SetUp()
        {
            _drawMock = new Mock<IDraw>();

            _sut = new GridDrawer()
            {
                Draw = _drawMock.Object,
                Icons = new Icons(),
            };
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

            _sut.TextSizeCalculator = (text) => text == "foo" ? new Size(100, 100) : new Size(0, 0);

            // Act
            _sut.DrawGrid(nodeMock.Object);

            // Assert
            var expectedTextPos = new Point(expectedX, expectedY);
            _drawMock.Verify(m => m.DrawClippedText("foo", rect, expectedTextPos, It.IsAny<Color>()));
        }

        [Test]
        public void GridText_WhenCellIsNotEditable_ThenEditIconIsNotDrawn()
        {
            // Arrange
            var nodeMock = new Mock<INode>();
            var rect = new Rectangle(0, 0, 100, 100);
            var gridMock = MockGrid("foo", rect);
            nodeMock.Setup(p => p.Grid).Returns(gridMock.Object);

            // Act
            _sut.DrawGrid(nodeMock.Object);

            // Assert
            _drawMock.Verify(m => m.DrawSvgIcon(It.IsAny<SKBitmap>(), It.IsAny<Windows.Foundation.Size>(), It.IsAny<float>(), It.IsAny<float>()), Times.Never);
        }

        [Test]
        public void GridText_WhenEditingTextIsEnabled_ThenCursorIsDrawn()
        {
            var nodeMock = new Mock<INode>();
            var rect = new Rectangle(0, 0, 100, 100);
            var gridMock = MockGrid("foo", rect, 2);
            nodeMock.Setup(p => p.Grid).Returns(gridMock.Object);
            nodeMock.Setup(p => p.Alignment).Returns(Alignment.TopLeft);

            _sut.TextSizeCalculator = (text) =>
            {
                if (text == "fo")
                {
                    return new Size(50, 10);
                }

                return new Size(0, 0);
            };

            // Act
            _sut.DrawGrid(nodeMock.Object);

            // Assert
            var expecteCursorLineStartPoint = new Point(50, 3); // -3 is the offset for the cursor line
            var expecteCursorLineEndPoint = new Point(50, 10 - 3);
            _drawMock.Verify(m => m.DrawLine(expecteCursorLineStartPoint, expecteCursorLineEndPoint, Style.None));
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

            // Act
            _sut.DrawGrid(nodeMock.Object);

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

            _sut.TextSizeCalculator = (text) => text == "foo" ? new Size(100, 100) : new Size(0, 0);

            // Act
            _sut.DrawGrid(nodeMock.Object);

            // Assert
            var expectedTextPos = new Point(10, 10);
            _drawMock.Verify(m => m.DrawClippedText("foo", rect, expectedTextPos, It.IsAny<Color>()), "Expected the text to be positioned at the grid coordinates becase the text is larger then the grid cell");
        }


        private Mock<IGrid> MockGrid(string text, Rectangle rect, int editPosition = -1)
        {
            var gridMock = new Mock<IGrid>();

            gridMock.SetupGet(p => p.Cols).Returns(["1"]);
            gridMock.SetupGet(p => p.Rows).Returns(["1"]);

            // This is the expected field size for a class node (1 column with 3 rows for title, props and methods)
            var cells = new IGridCell[1][];
            cells[0] = new IGridCell[1];

            var cellMock = new Mock<IGridCell>();
            cells[0][0] = cellMock.Object;
            cellMock.Setup(p => p.Text).Returns(text);
            cellMock.Setup(p => p.Rectangle).Returns(new Rectangle(rect.X, rect.Y, rect.Width, rect.Height));

            if (editPosition != -1)
            {
                cellMock.Setup(p => p.EditModeEnabled).Returns(true);
                cellMock.Setup(p => p.EditTextPosition).Returns(editPosition);
                cellMock.Setup(p => p.Editable).Returns(true);
            }

            gridMock.SetupGet(p => p.Cells).Returns(cells);

            return gridMock;
        }

    }
}
