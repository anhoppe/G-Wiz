using Gwiz.Core.Contract;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwiz.Core.Test
{
    [TestFixture]
    public class GraphTest
    {
        [Test]
        public void AddNode_WhenAddingSingleNode_ThenNodeInGraph()
        {
            // Arrange
            var sut = new Graph();

            sut.Templates.Add(new Template()
            {
                Name = "foo"
            });

            // Act
            var node = sut.AddNode("foo");

            // Assert
            Assert.That(sut.Nodes.Count, Is.EqualTo(1));
        }

        [Test]
        public void AddNode_WhenAddingNodeWithTemplate_ThenTemplateIsAssignedToNode()
        {
            var sut = new Graph();
            sut.Templates.Add(new Template()
            {
                Name = "foo",
                BackgroundColor = Color.Red,
                LineColor = Color.Blue,
                Resize = Resize.HorzVertBoth,
            });

            // Act
            var node = sut.AddNode("foo");

            // Assert
            Assert.That(node.BackgroundColor, Is.EqualTo(Color.Red));
            Assert.That(node.LineColor, Is.EqualTo(Color.Blue));
            Assert.That(node.Resize, Is.EqualTo(Resize.HorzVertBoth));
        }

        [Test]
        public void AddNode_WhenAddingNodeWithTemplate_ThenNodeHasTGridFromTemplate()
        {
            // Arrange
            var sut = new Graph();
            sut.Templates.Add(new Template()
            {
                Name = "foo",

                Grid = new Grid()
                {
                    Cols = new List<string>() { "1", "2" },
                    Rows = new List<string>() { "1", "2" }
                }
            });
         
            // Act
            var node = sut.AddNode("foo");
            
            // Assert
            Assert.That(node.Grid.Cols.Count, Is.EqualTo(2));
            Assert.That(node.Grid.Rows.Count, Is.EqualTo(2));
        }
    }
}
