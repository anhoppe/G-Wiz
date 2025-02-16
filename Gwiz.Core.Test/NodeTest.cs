using Gwiz.Core.Contract;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwiz.Core.Test
{
    [TestFixture]
    public class NodeTest
    {
        private Node _sut;

        [SetUp]
        public void SetUp() 
        { 
            _sut = new Node(); 
        }

        [Test]
        public void RowLinePositions_When2RowsWith1To10AreDefined_ThenRowLinePositionsAreCorrectWithRespectToHeight()
        {
            // Arrange
            _sut.Template = new Template()
            {
                Grid = new Grid()
                {
                    Rows = new List<string>()
                    {
                        "1",
                        "10"
                    }
                },
            };

            _sut.Height = 100;

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
            _sut.Template = new Template()
            {
                Grid = new Grid()
                {
                    Cols = new List<string>()
                    {
                        "10",
                        "100"
                    }
                },
            };

            _sut.Width = 100;

            // Act
            var result = _sut.GetColLinePositions();

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0], Is.EqualTo(9));
        }
    }
}
