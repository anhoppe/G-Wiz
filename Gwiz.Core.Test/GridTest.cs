using Gwiz.Core.Contract;
using NUnit.Framework;
using System.Drawing;

namespace Gwiz.Core.Test
{
    [TestFixture]
    public class GridTest
    {
        private Node _parentNode = new Node();
        private Grid _sut = new Grid();

        [SetUp]
        public void SetUp()
        {
            _sut = new Grid()
            {
                ParentNode = _parentNode,
            };
        }

        [Test]
        public void RowLinePositions_When2RowsWith1To10AreDefined_ThenRowLinePositionsAreCorrectWithRespectToHeight()
        {
            // Arrange
            _sut.Rows = new List<string>()
            {
                "1",
                "10"
            };

            _parentNode.Height = 100;

            // Act
            var result = _sut.GetRowLinePositions();

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0], Is.EqualTo(9));
        }

        [Test]
        public void ColLinePositions_When2ColsWith1To10AreDefined_ThenColLinePositionsAreCorrectWithRespectToHeight()
        {
            // Arrange
            _sut.Cols = new List<string>()
            {
                "10",
                "100"
            };

            _parentNode.Width = 100;

            // Act
            var result = _sut.GetColLinePositions();

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0], Is.EqualTo(9));
        }


        [Test]
        public void GetFieldTextAndEditButtonPosition_WhenOneFieldWithDefinedSizeAndSimulatedTextSize_ThenExectedTopLeftTextPositionsAreReturned()
        {
            // Arrange
            _parentNode.Width = 200;
            _parentNode.Height = 200;

            _parentNode.X = 100;
            _parentNode.Y = 200;

            _sut.Cols = new List<string> { "1", "1" };
            _sut.Rows = new List<string> { "1", "1" };

            _sut.TextSizeFactory = (string text) => { 
                if (text == "foo") 
                {
                    return new Size(50, 50);
                }            

                return new Size(0, 0);
            };

            _sut.FieldText = new string[2][];
            _sut.FieldText[0] = new[] { "foo", "foo" };
            _sut.FieldText[1] = new[] { "foo", "foo" };

            _sut.EditButtonMargin = 5;
            _sut.IconSize = 20;
            // Act
            var positionTopLeft = _sut.GetFieldTextAndEditButtonPosition(0, 0);
            var positionTopRight = _sut.GetFieldTextAndEditButtonPosition(1, 0);
            var positionBottomLeft = _sut.GetFieldTextAndEditButtonPosition(0, 1);
            var positionBottomRight = _sut.GetFieldTextAndEditButtonPosition(1, 1);

            // Assert
            Assert.That(positionTopLeft.Item1.X, Is.EqualTo(125));
            Assert.That(positionTopLeft.Item1.Y, Is.EqualTo(225));
            Assert.That(positionTopLeft.Item2.X, Is.EqualTo(155));
            Assert.That(positionTopLeft.Item2.Y, Is.EqualTo(210));

            Assert.That(positionTopRight.Item1.X, Is.EqualTo(225));
            Assert.That(positionTopRight.Item1.Y, Is.EqualTo(225));
            Assert.That(positionTopRight.Item2.X, Is.EqualTo(255));
            Assert.That(positionTopRight.Item2.Y, Is.EqualTo(210));

            Assert.That(positionBottomLeft.Item1.X, Is.EqualTo(125));
            Assert.That(positionBottomLeft.Item1.Y, Is.EqualTo(325));
            Assert.That(positionBottomLeft.Item2.X, Is.EqualTo(155));
            Assert.That(positionBottomLeft.Item2.Y, Is.EqualTo(310));

            Assert.That(positionBottomRight.Item1.X, Is.EqualTo(225));
            Assert.That(positionBottomRight.Item1.Y, Is.EqualTo(325));
            Assert.That(positionBottomRight.Item2.X, Is.EqualTo(255));
            Assert.That(positionBottomRight.Item2.Y, Is.EqualTo(310));
        }

        [Test]
        public void GetFieldTextPosition_WhenNonExistingFieldIsRequested_ThenInvalidArgumentExceptionIsThrown()
        {
            // Arrange 
            _sut.Cols = new List<string> { "1" };
            _sut.Rows = new List<string> { "1" };

            // Act
            Assert.Throws<ArgumentException>(() => _sut.GetFieldTextAndEditButtonPosition(1, 0));
        }
    }
}
