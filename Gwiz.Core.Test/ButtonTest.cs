using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwiz.Core.Test
{
    [TestFixture]
    public class ButtonTest
    {
        [Test]
        public void Click_WhenClickIsCalled_ThenClickedEventIsRaised()
        {
            // Arrange
            var button = new Button();
            bool eventRaised = false;
            button.Clicked += (s, e) => eventRaised = true;
         
            // Act
            button.Click();
            
            // Assert
            Assert.That(eventRaised);
        }
    }
}
