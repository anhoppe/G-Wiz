using Gwiz.Core.Contract;
using Moq;
using NUnit.Framework;
using SkiaSharp;
using System.Drawing;

namespace Gwiz.UiControl.WinUi3.Test
{
    [TestFixture]
    public class GraphDrawerTest
    {
        private Mock<IDraw> _drawMock = new();

        private Mock<IGridDrawer> _drawGridMock = new();

        private GraphDrawer _sut = new GraphDrawer();

        [SetUp]
        public void SetUp()
        {
            _drawMock = new Mock<IDraw>();

            _drawGridMock = new Mock<IGridDrawer>();

            _sut = new GraphDrawer()
            {
                Draw = _drawMock.Object,
                GridDrawer = _drawGridMock.Object,
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
            _drawMock.Verify(x => x.DrawLine(It.Is<Point>(p => p == to), It.Is<Point>(p => p == expectedEndPoint1), Style.None, It.IsAny<Color>(), It.IsAny<float>()));
            _drawMock.Verify(x => x.DrawLine(It.Is<Point>(p => p == to), It.Is<Point>(p => p == expectedEndPoint2), Style.None, It.IsAny<Color>(), It.IsAny<float>()));

            // Assert correct line to arrow head
            _drawMock.Verify(x => x.DrawLine(It.Is<Point>(p => p == from), It.Is<Point>(p => p == to), Style.None, It.IsAny<Color>(), It.IsAny<float>()));
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
            _drawMock.Verify(x => x.DrawLine(It.Is<Point>(p => p == to), It.Is<Point>(p => p == expectedEndPoint1), Style.None, It.IsAny<Color>(), It.IsAny<float>()));
            _drawMock.Verify(x => x.DrawLine(It.Is<Point>(p => p == to), It.Is<Point>(p => p == expectedEndPoint2), Style.None, It.IsAny<Color>(), It.IsAny<float>()));

            // In addition, the arrow head is closed by a line
            _drawMock.Verify(x => x.DrawLine(It.Is<Point>(p => p == expectedEndPoint2), It.Is<Point>(p => p == expectedEndPoint1), Style.None, It.IsAny<Color>(), It.IsAny<float>()));

            // Assert correct line to arrow head. In this case, the line is drawn to the end of the arrow head
            var modifiedTo = new Point(9, 2);
            _drawMock.Verify(x => x.DrawLine(It.Is<Point>(p => p == from), It.Is<Point>(p => p == modifiedTo), Style.None, It.IsAny<Color>(), It.IsAny<float>()));
        }

        [Test]
        public void Edges_DrawEdgeWithRhombusBeginning_RhombusDrawnAsExpected()
        {
            // Arrange
            var edgeMock = new Mock<IEdge>();

            var from = new Point(10, 30);
            var to = new Point(10, 0);
            edgeMock.SetupGet(x => x.FromPosition).Returns(from);
            edgeMock.SetupGet(x => x.ToPosition).Returns(to);
            edgeMock.Setup(p => p.Beginning).Returns(Ending.Rhombus);

            _sut.Edges = [edgeMock.Object];

            var expectedEndPoint1 = new Point(2, 17);
            var expectedEndPoint2 = new Point(17, 17);
            var expectedModifiedFrom = new Point(10, 4);

            // Act
            _sut.DrawGraph();

            // Assert

            // Assert correct Rhombus head
            _drawMock.Verify(x => x.DrawLine(It.Is<Point>(p => p == from), It.Is<Point>(p => p == expectedEndPoint1), Style.None, It.IsAny<Color>(), It.IsAny<float>()));
            _drawMock.Verify(x => x.DrawLine(It.Is<Point>(p => p == from), It.Is<Point>(p => p == expectedEndPoint2), Style.None, It.IsAny<Color>(), It.IsAny<float>()));
            _drawMock.Verify(x => x.DrawLine(It.Is<Point>(p => p == expectedEndPoint1), It.Is<Point>(p => p == expectedModifiedFrom), Style.None, It.IsAny<Color>(), It.IsAny<float>()));
            _drawMock.Verify(x => x.DrawLine(It.Is<Point>(p => p == expectedEndPoint2), It.Is<Point>(p => p == expectedModifiedFrom), Style.None, It.IsAny<Color>(), It.IsAny<float>()));

            // Assert correct line to arrow head. In this case, the line is drawn to the end of the arrow head
            _drawMock.Verify(x => x.DrawLine(It.Is<Point>(p => p == expectedModifiedFrom), It.Is<Point>(p => p == to), Style.None, It.IsAny<Color>(), It.IsAny<float>()));
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
            _drawMock.Verify(x => x.DrawLine(It.Is<Point>(p => p == to), It.Is<Point>(p => p == expectedEndPoint1), Style.None, It.IsAny<Color>(), It.IsAny<float>()));
            _drawMock.Verify(x => x.DrawLine(It.Is<Point>(p => p == to), It.Is<Point>(p => p == expectedEndPoint2), Style.None, It.IsAny<Color>(), It.IsAny<float>()));
            _drawMock.Verify(x => x.DrawLine(It.Is<Point>(p => p == expectedEndPoint1), It.Is<Point>(p => p == expectedModifiedTo), Style.None, It.IsAny<Color>(), It.IsAny<float>()));
            _drawMock.Verify(x => x.DrawLine(It.Is<Point>(p => p == expectedEndPoint2), It.Is<Point>(p => p == expectedModifiedTo), Style.None, It.IsAny<Color>(), It.IsAny<float>()));

            // Assert correct line to arrow head. In this case, the line is drawn to the end of the arrow head
            _drawMock.Verify(x => x.DrawLine(It.Is<Point>(p => p == from), It.Is<Point>(p => p == expectedModifiedTo), Style.None, It.IsAny<Color>(), It.IsAny<float>()));
        }

        [Test]
        public void EdgeHighlight_WhenEdgeIsHighlighted_ThenHighlighterMarkIsDrawn()
        {
            // Arrange
            var edgeMock = new Mock<IEdge>();

            var from = new Point(0, 0);
            var to = new Point(0, 100);
            edgeMock.SetupGet(x => x.FromPosition).Returns(from);
            edgeMock.SetupGet(x => x.ToPosition).Returns(to);

            edgeMock.Setup(p => p.Highlight).Returns(true);

            _sut.Edges = [edgeMock.Object];

            // Act
            _sut.DrawGraph();

            // Assert
            _drawMock.Verify(m => m.DrawLine(from, to, It.IsAny<Style>(), Design.HighlightColor, Design.HighlightStrokeWidth));
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

        [Test]
        public void EdgeText_WhenEdgeTextIsDefined_ThenItIsRenderedAtTheCenter()
        {
            // Arrange
            var edgeMock = new Mock<IEdge>();
            var from = new Point(0, 25);
            var to = new Point(100, 25);

            edgeMock.SetupGet(x => x.FromPosition).Returns(from);
            edgeMock.SetupGet(x => x.ToPosition).Returns(to);

            edgeMock.Setup(p => p.Text).Returns("foo");

            _sut.Edges = [edgeMock.Object];
            _sut.TextSizeCalculator = (text) => text == "foo" ? new Size(50, 50) : new Size(0, 0);

            // Act
            _sut.DrawGraph();

            // Assert
            var expectedTextPos = new Point(25, 0);
            _drawMock.Verify(m => m.DrawText("foo", expectedTextPos, It.IsAny<Color>()));
        }

        [Test]
        public void GetSourceEdgeTemplateAtPosition_WhenMouseisOverSourceEdgeTemplates_ThenCorrectEdgeTemplateIsReturned()
        {
            // Arrange
            var nodeMock = new Mock<INode>();

            var edgeTemplate1Mock = new Mock<IEdgeTemplate>();
            edgeTemplate1Mock.Setup(p => p.Icon).Returns("F");
            
            var edgeTemplate2Mock = new Mock<IEdgeTemplate>();
            edgeTemplate2Mock.Setup(p => p.Icon).Returns("B");

            nodeMock.Setup(p => p.SourceEdgeTemplates).Returns(new List<IEdgeTemplate>() 
            {
                edgeTemplate1Mock.Object,
                edgeTemplate2Mock.Object
            });

            nodeMock.Setup(p => p.X).Returns(Design.IconSize);
            nodeMock.Setup(p => p.Y).Returns(0);
            nodeMock.Setup(p => p.Width).Returns(100);
            nodeMock.Setup(p => p.Height).Returns(100);

            _sut.Nodes.Add(nodeMock.Object);

            // Act
            var result = _sut.GetSourceEdgeTemplateAtPosition(nodeMock.Object, 3, 38);

            // Assert
            Assert.That(edgeTemplate1Mock.Object == result);
        }

        [Test]
        public void NodeConnections_WhenNodeHasSourceEdgeTemplates_ThenCorrectIconsAreDrawn()
        {
            // Arrange
            var nodeMock = new Mock<INode>();

            var edgeTemplate1Mock = new Mock<IEdgeTemplate>();
            edgeTemplate1Mock.Setup(p => p.Icon).Returns("F");

            var edgeTemplate2Mock = new Mock<IEdgeTemplate>();
            edgeTemplate2Mock.Setup(p => p.Icon).Returns("B");

            nodeMock.Setup(p => p.SourceEdgeTemplates).Returns(new List<IEdgeTemplate>()
            {
                edgeTemplate1Mock.Object,
                edgeTemplate2Mock.Object
            });

            nodeMock.Setup(p => p.X).Returns(Design.IconSize);
            nodeMock.Setup(p => p.Y).Returns(0);
            nodeMock.Setup(p => p.Width).Returns(100);
            nodeMock.Setup(p => p.Height).Returns(100);

            _sut.Nodes.Add(nodeMock.Object);

            // Act
            _sut.DrawGraph();

            // Assert
            _drawMock.Verify(m => m.DrawSvgIcon(It.IsAny<SKBitmap>(), It.IsAny<Windows.Foundation.Size>(), 0, 5), "Expected is the connection icon on top");
            _drawMock.Verify(m => m.DrawSvgIcon(It.IsAny<SKBitmap>(), It.IsAny<Windows.Foundation.Size>(), 0, 35), "Expected F icon for 1st source");
            _drawMock.Verify(m => m.DrawSvgIcon(It.IsAny<SKBitmap>(), It.IsAny<Windows.Foundation.Size>(), 0, 65), "Expected B icon for 2nd source");
        }


        [Test]
        public void NodeConnections_WhenEdgeCreationActiveSourceTemplateIsSet_ThenCorrectTargetIconIsDrawnd()
        {
            // Arrange
            var nodeMock = new Mock<INode>();

            var edgeTemplate1Mock = new Mock<IEdgeTemplate>();
            edgeTemplate1Mock.Setup(p => p.Icon).Returns("F");

            var edgeTemplate2Mock = new Mock<IEdgeTemplate>();
            edgeTemplate2Mock.Setup(p => p.Icon).Returns("B");

            nodeMock.Setup(p => p.TargetEdgeTemplates).Returns(new List<IEdgeTemplate>()
            {
                edgeTemplate1Mock.Object,
                edgeTemplate2Mock.Object
            });

            nodeMock.Setup(p => p.X).Returns(Design.IconSize);
            nodeMock.Setup(p => p.Y).Returns(0);
            nodeMock.Setup(p => p.Width).Returns(100);
            nodeMock.Setup(p => p.Height).Returns(100);

            _sut.Nodes.Add(nodeMock.Object);

            _sut.EdgeCreationActiveSourceTemplate = edgeTemplate2Mock.Object;

            // Act
            _sut.DrawGraph();

            // Assert
            _drawMock.Verify(m => m.DrawSvgIcon(It.IsAny<SKBitmap>(), It.IsAny<Windows.Foundation.Size>(), 0, 35), Times.Once, "Expected B icon is drawn as target because it is selected as source");
            _drawMock.Verify(m => m.DrawSvgIcon(It.IsAny<SKBitmap>(), It.IsAny<Windows.Foundation.Size>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never, "That the method is only called once");
        }

        [Test]
        public void NodeSelection_WhenNodeIsSelected_ThenSelectionMarkerIsDrawn()
        {
            // Arrange
            var nodeMock = new Mock<INode>();

            nodeMock.Setup(p => p.X).Returns(10);
            nodeMock.Setup(p => p.Y).Returns(10);
            nodeMock.Setup(p => p.Width).Returns(90);
            nodeMock.Setup(p => p.Height).Returns(90);
            nodeMock.Setup(p => p.Select).Returns(true);
            nodeMock.Setup(p => p.SourceEdgeTemplates).Returns(new List<IEdgeTemplate>());

            _sut.Nodes.Add(nodeMock.Object);

            // Act
            _sut.DrawGraph();

            // Assert
            var expectedRect = new Rectangle(10 - Design.SelectionMargin - Design.SelectionStrokeWidth,
                10 - Design.SelectionMargin - Design.SelectionStrokeWidth, 
                90 + 2*Design.SelectionStrokeWidth + 2*Design.SelectionMargin,
                90 + 2 * Design.SelectionStrokeWidth + 2 * Design.SelectionMargin);

            _drawMock.Verify(m => m.DrawRectangle(expectedRect, It.IsAny<Color>(), 4));
        }

        [Test]
        public void PreparePreviewLine_WhenEdgeCreationActiveIsNotEnabled_ThenPreviewLineIsNotDrawn()
        {
            // Arrange
            var nodeMock = new Mock<INode>();

            nodeMock.Setup(p => p.X).Returns(0);
            nodeMock.Setup(p => p.Y).Returns(0);
            nodeMock.Setup(p => p.Width).Returns(100);
            nodeMock.Setup(p => p.Height).Returns(100);

            // Act
            _sut.PreparePreviewLine(nodeMock.Object, 200, 300);
            _sut.DrawGraph();

            // Assert
            var expectedStart = new Point(50, 50);
            var expectedEnd = new Point(200, 300);
            _drawMock.Verify(m => m.DrawLine(expectedStart, expectedEnd, Style.Dotted, It.IsAny<Color>(), It.IsAny<float>()), Times.Never);
        }

        [Test]
        public void PreparePreviewLine_WhenPreviewLineIsPreparedAndGraphisDrawn_ThenPreviewLineIsDrawnFromNodeCenterToPosition()
        {
            // Arrange
            var nodeMock = new Mock<INode>();

            nodeMock.Setup(p => p.X).Returns(0);
            nodeMock.Setup(p => p.Y).Returns(0);
            nodeMock.Setup(p => p.Width).Returns(100);
            nodeMock.Setup(p => p.Height).Returns(100);

            // Act
            _sut.EdgeCreationActiveSourceTemplate = Mock.Of<IEdgeTemplate>();
            _sut.PreparePreviewLine(nodeMock.Object, 200, 300);
            _sut.DrawGraph();

            // Assert
            var expectedStart = new Point(50, 50);
            var expectedEnd = new Point(200, 300);
            _drawMock.Verify(m => m.DrawLine(expectedStart, expectedEnd, Style.Dotted, It.IsAny<Color>(), It.IsAny<float>()));
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
    }
}
