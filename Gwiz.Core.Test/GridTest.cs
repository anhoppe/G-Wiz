using Gwiz.Core.Contract;
using Moq;
using Newtonsoft.Json.Bson;
using NUnit.Framework;
using System.Drawing;
using System.Xml.Serialization;

namespace Gwiz.Core.Test
{
    [TestFixture]
    public class GridTest
    {
        private Node _parentNode = new Node();

        [Test]
        public void RegisterParentNodeChanged_WhenNodeChanged_ThenUpdateFieldRectsCalled()
        {
            // Arrange
            var nodeMock = new Mock<IUpdatableNode>();

            nodeMock.Setup(p => p.Width).Returns(100);
            nodeMock.Setup(p => p.Height).Returns(100);
            nodeMock.Setup(p => p.X).Returns(100);
            nodeMock.Setup(p => p.Y).Returns(200);

            var sut = new Grid()
            {
                Cols = ["1"],
                Rows = ["1"],
            };
            sut.RegisterParentNodeChanged(nodeMock.Object);

            // Act
            nodeMock.Raise(p => p.NodeChanged += null, EventArgs.Empty, nodeMock.Object);

            // Assert
            Assert.That(sut.FieldRects[0][0].X, Is.EqualTo(100));
            Assert.That(sut.FieldRects[0][0].Y, Is.EqualTo(200));
            Assert.That(sut.FieldRects[0][0].Width, Is.EqualTo(100));
            Assert.That(sut.FieldRects[0][0].Height, Is.EqualTo(100));
        }

        [Test]
        public void UpdateFieldRects_WhenOneFieldConfigured_ThenRectUsesEntireSpaceOfNode()
        {
            // Arrange
            var gridMock = new Mock<IGrid>();

            gridMock.Setup(p => p.Cols).Returns(new List<string> { "1" });
            gridMock.Setup(p => p.Rows).Returns(new List<string> { "1" }); 
         
            _parentNode.Width = 100;
            _parentNode.Height = 100;
            _parentNode.X = 100;
            _parentNode.Y = 200;

            var sut = new Grid(gridMock.Object);

            // Act
            sut.UpdateFieldRects(_parentNode);

            // Assert
            Assert.That(sut.FieldRects[0][0].X, Is.EqualTo(100));
            Assert.That(sut.FieldRects[0][0].Y, Is.EqualTo(200));
            Assert.That(sut.FieldRects[0][0].Width, Is.EqualTo(100));
            Assert.That(sut.FieldRects[0][0].Height, Is.EqualTo(100));
        }

        [Test]
        public void UpdateFieldRects_WhenTwoFieldsWithDefinedRatio_ThenFieldsAreOfExpectedSizeAndPosition()
        {
            // Arrange
            var gridMock = new Mock<IGrid>();

            gridMock.Setup(p => p.Cols).Returns(new List<string> { "3", "7" });
            gridMock.Setup(p => p.Rows).Returns(new List<string> { "4", "6" });

            _parentNode.Width = 100;
            _parentNode.Height = 100;
            _parentNode.X = 20;
            _parentNode.Y = 10;

            var sut = new Grid(gridMock.Object);

            // Act
            sut.UpdateFieldRects(_parentNode);

            // Assert
            Assert.That(sut.FieldRects[0][0].X, Is.EqualTo(20));
            Assert.That(sut.FieldRects[0][0].Y, Is.EqualTo(10));
            Assert.That(sut.FieldRects[0][0].Width, Is.EqualTo(30));
            Assert.That(sut.FieldRects[0][0].Height, Is.EqualTo(40));

            Assert.That(sut.FieldRects[1][0].X, Is.EqualTo(50));
            Assert.That(sut.FieldRects[1][0].Y, Is.EqualTo(10));
            Assert.That(sut.FieldRects[1][0].Width, Is.EqualTo(70));
            Assert.That(sut.FieldRects[1][0].Height, Is.EqualTo(40));

            Assert.That(sut.FieldRects[0][1].X, Is.EqualTo(20));
            Assert.That(sut.FieldRects[0][1].Y, Is.EqualTo(50));
            Assert.That(sut.FieldRects[0][1].Width, Is.EqualTo(30));
            Assert.That(sut.FieldRects[0][1].Height, Is.EqualTo(60));

            Assert.That(sut.FieldRects[1][1].X, Is.EqualTo(50));
            Assert.That(sut.FieldRects[1][1].Y, Is.EqualTo(50));
            Assert.That(sut.FieldRects[1][1].Width, Is.EqualTo(70));
            Assert.That(sut.FieldRects[1][1].Height, Is.EqualTo(60));
        }
    }
}
