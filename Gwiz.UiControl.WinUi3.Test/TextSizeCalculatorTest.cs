using NUnit.Framework;

namespace Gwiz.UiControl.WinUi3.Test
{
    [TestFixture]
    public class TextSizeCalculatorTest
    {
        [Test]
        public void GetTextSize_WhenMultipleLinesOfSameText_ThenTextWidthIsEqual()
        {
            // Arrange
            var text1 = "HelloWorld\n";
            var text2 = "HelloWorld\nHelloWorld\nHelloWorld\nHelloWorld\nHelloWorld\n";

            // Act
            var size1 = TextSizeCalculator.GetTextSize(text1);
            var size2 = TextSizeCalculator.GetTextSize(text2);

            // Assert
            Assert.That(size1.Width, Is.EqualTo(size2.Width), "Expected same width since text2 has line breaks");
        }
    }
}
