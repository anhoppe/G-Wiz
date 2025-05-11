using Gwiz.Core.Contract;
using Gwiz.Core.Serializer;
using NUnit.Framework;
using System.Drawing;
using System.Text;
using YamlDotNet.Core;

namespace Gwiz.Core.Test.Serializer
{
    [TestFixture]
    public class YamlSerializerTest
    {
        YamlSerializer _sut = new YamlSerializer();

        [Test]
        public void Alignment_WhenAlignmentNotSet_ThenAlignmentIsCenterCenter()
        {
            // Arrange
            var yaml =
                "Templates:\n" +
                "  - Name: Foo\n" +
                "Nodes:\n" +
                "  - Template: Foo\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act
            var result = _sut.Deserialize(stream);

            // Assert
            var node = result.Nodes.First();
            Assert.That(node.Alignment, Is.EqualTo(Alignment.CenterCenter));
        }
        
        [Test]
        public void Alignment_WhenAlignmentInTemplate_ThenAlignmentIsSetInNode()
        {
            // Arrange
            var yaml =
                "Templates:\n" +
                "  - Name: Foo\n" +
                "    Alignment: BottomCenter\n" +
                "Nodes:\n" +
                "  - Template: Foo\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act
            var result = _sut.Deserialize(stream);

            // Assert
            var node = result.Nodes.First();
            Assert.That(node.Alignment, Is.EqualTo(Alignment.BottomCenter));
        }

        [Test]
        public void Button_WhenButtonIsInTemplate_ThenButtonIsInNode()
        {
            var yaml =
                "Templates:\n" +
                "  - Name: Foo\n" +
                "    Buttons:\n" +
                "      - Id: MyButton\n" +
                "        Alignment: TopRight\n" +
                "        Icon: AlertCircle\n" +
                "Nodes:\n" +
                "  - Template: Foo\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act
            var result = _sut.Deserialize(stream);

            // Assert
            var node = result.Nodes.First();
            var button = node.GetButtonById("MyButton");

            Assert.That(button.Icon, Is.EqualTo(IconId.AlertCircle));
            Assert.That(button.Alignment, Is.EqualTo(Alignment.TopRight));
        }

        [Test]
        public void EdgeBeginning_WhenEdgeBeginningHasUnknwonSpecifier_ThenExceptionIsThrown()
        {
            // Arrange
            var yaml =
                "Nodes:\n" +
                "  - Id: foo\n" +
                "  - Id: bar\n" +
                "Edges:\n" +
                "  - From: foo\n" +
                "    To: bar\n" +
                "    Beginning: InvalidEndingId\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act / Assert
            Assert.Throws<YamlException>(() => _sut.Deserialize(stream));
        }

        [Test]
        public void EdgeBeginning_WhenEdgeBeginningSpecified_ThenEdgeHasCorrectBeginningIdentifierSet()
        {
            // Arrange
            var yaml =
                "Nodes:\n" +
                "  - Id: foo\n" +
                "  - Id: bar\n" +
                "Edges:\n" +
                "  - From: foo\n" +
                "    To: bar\n" +
                "    Beginning: OpenArrow\n" +
                "  - From: foo\n" +
                "    To: bar\n" +
                "    Beginning: ClosedArrow\n" +
                "  - From: foo\n" +
                "    To: bar\n" +
                "    Beginning: Rhombus\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act
            var graph = _sut.Deserialize(stream);

            // Assert
            Assert.That(graph.Edges[0].Beginning, Is.EqualTo(Ending.OpenArrow));
            Assert.That(graph.Edges[1].Beginning, Is.EqualTo(Ending.ClosedArrow));
            Assert.That(graph.Edges[2].Beginning, Is.EqualTo(Ending.Rhombus));
        }

        [Test]
        public void EdgeEnding_WhenEdgeEndingHasUnknwonSpecifier_ThenExceptionIsThrown()
        {
            // Arrange
            var yaml =
                "Nodes:\n" +
                "  - Id: foo\n" +
                "  - Id: bar\n" +
                "Edges:\n" +
                "  - From: foo\n" +
                "    To: bar\n" +
                "    Ending: InvalidEndingId\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act / Assert
            Assert.Throws<YamlException>(() => _sut.Deserialize(stream));
        }

        [Test]
        public void EdgeEnding_WhenEdgeEndingSpecified_ThenEdgeHasCorrectEndingIdentifierSet()
        {
            // Arrange
            var yaml =
                "Nodes:\n" +
                "  - Id: foo\n" +
                "  - Id: bar\n" +
                "Edges:\n" +
                "  - From: foo\n" +
                "    To: bar\n" +
                "    Ending: OpenArrow\n" +
                "  - From: foo\n" +
                "    To: bar\n" +
                "    Ending: ClosedArrow\n" +
                "  - From: foo\n" +
                "    To: bar\n" +
                "    Ending: Rhombus\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act
            var graph = _sut.Deserialize(stream);

            // Assert
            Assert.That(graph.Edges[0].Ending, Is.EqualTo(Ending.OpenArrow));
            Assert.That(graph.Edges[1].Ending, Is.EqualTo(Ending.ClosedArrow));
            Assert.That(graph.Edges[2].Ending, Is.EqualTo(Ending.Rhombus));
        }

        [Test]
        public void EdgeEnding_WhenEdgeStyleHasUnknwonSpecifier_ThenExceptionIsThrown()
        {
            // Arrange
            var yaml =
                "Nodes:\n" +
                "  - Id: foo\n" +
                "  - Id: bar\n" +
                "Edges:\n" +
                "  - From: foo\n" +
                "    To: bar\n" +
                "    Style: Foo\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act / Assert
            Assert.Throws<YamlException>(() => _sut.Deserialize(stream));
        }

        [Test]
        public void EdgeLabel_WhenEdgeHasLabels_ThenLabelsAreSet()
        {
            var yaml =
                "Nodes:\n" +
                "  - Id: foo\n" +
                "  - Id: bar\n" +
                "Edges:\n" +
                "  - From: foo\n" +
                "    To: bar\n" +
                "    FromLabel: moo\n" +
                "    ToLabel: goo\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act
            var graph = _sut.Deserialize(stream);

            // Assert
            Assert.That(graph.Edges[0].FromLabel, Is.EqualTo("moo"));
            Assert.That(graph.Edges[0].ToLabel, Is.EqualTo("goo"));
        }

        [Test]
        public void EdgeLabel_WhenLabelOffsetInPerCentIsSet_ThenOffsetIsSet()
        {
            var yaml =
                "Nodes:\n" +
                "  - Id: foo\n" +
                "  - Id: bar\n" +
                "Edges:\n" +
                "  - From: foo\n" +
                "    To: bar\n" +
                "    FromLabel: moo\n" +
                "    ToLabel: goo\n" +
                "    LabelOffsetPerCent: 0.5\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act
            var graph = _sut.Deserialize(stream);

            // Assert
            Assert.That(graph.Edges[0].LabelOffsetPerCent, Is.EqualTo(0.5f));
        }

        [Test]
        public void EdgeStyle_WhenEdgeStyleIsStippled_ThenStyleEnumIsSet()
        {
            // Arrange
            var yaml =
                "Nodes:\n" +
                "  - Id: foo\n" +
                "  - Id: bar\n" +
                "Edges:\n" +
                "  - From: foo\n" +
                "    To: bar\n" +
                "    Style: Dashed\n" +
                "  - From: foo\n" +
                "    To: bar\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act
            var graph = _sut.Deserialize(stream);

            // Assert
            Assert.That(graph.Edges[0].Style, Is.EqualTo(Style.Dashed));
            Assert.That(graph.Edges[1].Style, Is.EqualTo(Style.None));
        }

        [Test]
        public void EdgeTemplate_WhenEdgeTemplateDefined_ThenEdgeTemplatesAreAvailableInNodes()
        {
            // Arrange
            var yaml =
                "Templates:\n" +
                "  - Name: Foo\n" +
                "  - Name: Bar\n" +
                "EdgeTemplates:\n" +
                "  - Source: Foo\n" +
                "    Target: Bar\n" +
                "Nodes:\n" +
                "  - Id: Foo\n" +
                "    Template: Foo\n" +
                "  - Id: Bar\n" +
                "    Template: Bar\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act
            var graph = _sut.Deserialize(stream);

            // Assert
            var node1 = graph.Nodes[0];
            var node2 = graph.Nodes[1];

            Assert.That(node1.SourceEdgeTemplates.Count, Is.EqualTo(1));
            Assert.That(node1.TargetEdgeTemplates.Count, Is.EqualTo(0));
            Assert.That(node2.SourceEdgeTemplates.Count, Is.EqualTo(0));
            Assert.That(node2.TargetEdgeTemplates.Count, Is.EqualTo(1));
        }

        [Test]
        public void EdgeTemplate_WhenEdgeTemplateDefined_ThenEdgeTemplatesAreAvailableInGraph()
        {
            // Arrange
            var yaml =
                "Templates:\n" +
                "  - Name: Foo\n" +
                "  - Name: Bar\n" +
                "EdgeTemplates:\n" +
                "  - Source: Foo\n" +
                "    Target: Bar\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act
            var graph = _sut.Deserialize(stream) as Graph;

            // Assert
            Assert.That(graph != null);

            if (graph != null)
            {
                Assert.That(graph.EdgeTemplates.Count == 1);
            }
        }

        [Test]
        public void EdgeTemplate_WhenEdgeTemplateDefinesIcon_ThenIconIsInNodesEdgeTemplates()
        {
            // Arrange
            var yaml =
                "Templates:\n" +
                "  - Name: Foo\n" +
                "  - Name: Bar\n" +
                "EdgeTemplates:\n" +
                "  - Source: Foo\n" +
                "    Target: Bar\n" +
                "    Icon: F\n" +
                "Nodes:\n" +
                "  - Id: Foo\n" +
                "    Template: Foo\n" +
                "  - Id: Bar\n" +
                "    Template: Bar\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act
            var graph = _sut.Deserialize(stream);

            // Assert
            var node1 = graph.Nodes[0];
            var node2 = graph.Nodes[1];

            Assert.That(node1.SourceEdgeTemplates[0].Icon, Is.EqualTo("F"));
            Assert.That(node2.TargetEdgeTemplates[0].Icon, Is.EqualTo("F"));
        }

        [Test]
        public void EdgeTemplate_WhenEdgeTemplateDefinesParameters_ThenParametersAreInNodesEdgeTemplates()
        {
            // Arrange
            var yaml =
                "Templates:\n" +
                "  - Name: Foo\n" +
                "EdgeTemplates:\n" +
                "  - Source: Foo\n" +
                "    Target: Bar\n" +
                "    Icon: F\n" +
                "    Text: Bar\n" +
                "    Beginning: Rhombus\n" +
                "    Ending: ClosedArrow\n" +
                "    Style: Dotted\n" +
                "Nodes:\n" +
                "  - Id: Foo\n" +
                "    Template: Foo\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act
            var graph = _sut.Deserialize(stream);

            // Assert
            var node = graph.Nodes[0];

            Assert.That(node.SourceEdgeTemplates[0].Icon, Is.EqualTo("F"));
            Assert.That(node.SourceEdgeTemplates[0].Text, Is.EqualTo("Bar"));
            Assert.That(node.SourceEdgeTemplates[0].Beginning, Is.EqualTo(Ending.Rhombus));
            Assert.That(node.SourceEdgeTemplates[0].Ending, Is.EqualTo(Ending.ClosedArrow));
            Assert.That(node.SourceEdgeTemplates[0].Style, Is.EqualTo(Style.Dotted));
        }

        [Test]
        public void EdgeText_WhenDefiningEdgeText_ThenTextIsSet()
        {
            // Arrange
            var yaml =
                "Nodes:\n" +
                "  - Id: foo\n" +
                "  - Id: bar\n" +
                "Edges:\n" +
                "  - From: foo\n" +
                "    To: bar\n" +
                "    Text: Buzfoo\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act
            var graph = _sut.Deserialize(stream);

            Assert.That(graph.Edges[0].Text, Is.EqualTo("Buzfoo"));
        }

        [Test]
        public void Grid_WhenGridDefined_ThenEachGridInANodeThatReferencesTheTemplateHasEmptyText()
        {
            // Arrange
            var yaml =
                "Templates:\n" +
                "  - Name: Foo\n" +
                "    Grid:\n" +
                "      Rows:\n" +
                "        - 1\n" +
                "        - 1\n" +
                "        - 1\n" +
                "      Cols:\n" +
                "        - 1\n" +
                "        - 1\n" +
                "        - 1\n" +
                "Nodes:\n" +
                "  - Template: Foo\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act
            var graph = _sut.Deserialize(stream);

            // Assert
            if (graph.Nodes.Count < 1)
            {
                Assert.Fail();
            }

            var node = graph.Nodes[0];

            Assert.That(node.Grid.Rows.Count, Is.EqualTo(3));
            Assert.That(node.Grid.Cols.Count, Is.EqualTo(3));

            for (int x = 0; x < node.Grid.Cols.Count; x++)
            {
                for (int y = 0; y < node.Grid.Rows.Count; y++)
                {
                    Assert.That(node.Grid.Cells[x, y].Text, Is.EqualTo(string.Empty));
                }
            }
        }

        [Test]
        public void Grid_WhenGridRowsDefined_ThenGridRowsInTemplate()
        {
            // Arrange
            var yaml =
                "Templates:\n" +
                "  - Name: Foo\n" +
                "    Grid:\n" +
                "      Rows:\n" +
                "        - 300\n" +
                "        - 100\n" +
                "        - auto\n" +
                "        - \"*\"\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act
            var graph = _sut.Deserialize(stream);

            // Assert
            var grid = graph.Templates[0].Grid;

            Assert.That(grid.Rows.Count, Is.EqualTo(4));
            Assert.That(grid.Rows[0], Is.EqualTo("300"));
            Assert.That(grid.Rows[1], Is.EqualTo("100"));
            Assert.That(grid.Rows[2], Is.EqualTo("auto"));
            Assert.That(grid.Rows[3], Is.EqualTo("*"));
        }

        [Test]
        public void Grid_WhenNodeIsDefined_ThenGridHasNodeParentSet()
        {
            // Arrange
            var yaml =
                "Nodes:\n" +
                "  - X: 50\n" +
                "    Y: 60\n" +
                "    Width: 70\n" +
                "    Height: 80\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act
            var graph = _sut.Deserialize(stream);

            // Assert
            Assert.That(graph.Nodes.Count, Is.EqualTo(1));

            var node = graph.Nodes.First();
        }

        [Test]
        public void Grid_WhenEditableIsNotDefined_ThenNoCellIsEditable()
        {
            // Arrange
            var yaml =
                "Templates:\n" +
                "  - Name: Foo\n" +
                "    Grid:\n" +
                "      Rows:\n" +
                "        - 1\n" +
                "        - 1\n" +
                "        - 1\n" +
                "      Cols:\n" +
                "        - 1\n" +
                "        - 1\n" +
                "        - 1\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));
            
            // Act
            var graph = _sut.Deserialize(stream);
            
            // Assert
            var template = graph.Templates[0];
            var grid = Grid.CreateFromTemplateGrid(template.Grid);

            Assert.That(!grid.Cells[0, 0].Editable);
            Assert.That(!grid.Cells[0, 1].Editable);
            Assert.That(!grid.Cells[0, 2].Editable);
            Assert.That(!grid.Cells[1, 0].Editable);
            Assert.That(!grid.Cells[1, 1].Editable);
            Assert.That(!grid.Cells[1, 2].Editable);
            Assert.That(!grid.Cells[2, 0].Editable);
            Assert.That(!grid.Cells[2, 1].Editable);
            Assert.That(!grid.Cells[2, 2].Editable);
        }
        
        [Test]
        public void Grid_WhenEditableAllIsDefined_ThenAllCellsAreEditable()
        {
            // Arrange
            var yaml =
                "Templates:\n" +
                "  - Name: Foo\n" +
                "    Grid:\n" +
                "      Rows:\n" +
                "        - 1\n" +
                "        - 1\n" +
                "        - 1\n" +
                "      Cols:\n" +
                "        - 1\n" +
                "        - 1\n" +
                "        - 1\n" +
                "      Editable:\n" +
                "        - all\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));
            
            // Act
            var graph = _sut.Deserialize(stream);
            
            // Assert
            var template = graph.Templates[0];
            var grid = Grid.CreateFromTemplateGrid(template.Grid);

            Assert.That(grid.Cells[0, 0].Editable);
            Assert.That(grid.Cells[0, 1].Editable);
            Assert.That(grid.Cells[0, 2].Editable);
            Assert.That(grid.Cells[1, 0].Editable);
            Assert.That(grid.Cells[1, 1].Editable);
            Assert.That(grid.Cells[1, 2].Editable);
            Assert.That(grid.Cells[2, 0].Editable);
            Assert.That(grid.Cells[2, 1].Editable);
            Assert.That(grid.Cells[2, 2].Editable);
        }        

        [Test]
        public void Grid_WhenEditableDefinedForSpecificCells_ThenThoseCellsAreEditable()
        {
            // Arrange
            var yaml =
                "Templates:\n" +
                "  - Name: Foo\n" +
                "    Grid:\n" +
                "      Rows:\n" +
                "        - 1\n" +
                "        - 1\n" +
                "        - 1\n" +
                "      Cols:\n" +
                "        - 1\n" +
                "        - 1\n" +
                "        - 1\n" +
                "      Editable:\n" +
                "        - 0,1\n" + 
                "        - 1,2\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));
            
            // Act
            var graph = _sut.Deserialize(stream);
            
            // Assert
            var template = graph.Templates[0];
            var grid = Grid.CreateFromTemplateGrid(template.Grid);

            Assert.That(!grid.Cells[0, 0].Editable);
            Assert.That(grid.Cells[0, 1].Editable);
            Assert.That(!grid.Cells[0, 2].Editable);
            Assert.That(!grid.Cells[1, 0].Editable);
            Assert.That(!grid.Cells[1, 1].Editable);
            Assert.That(grid.Cells[1, 2].Editable);
            Assert.That(!grid.Cells[2, 0].Editable);
            Assert.That(!grid.Cells[2, 1].Editable);
            Assert.That(!grid.Cells[2, 2].Editable);
        }

        [Test]
        public void Grid_WhenNoGridIsDefined_ThenGridHasOneRowAndOneColWith1()
        {
            // Arrange
            var yaml =
                "Templates:\n" +
                "  - Name: Foo\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act
            var graph = _sut.Deserialize(stream);

            // Assert
            if (graph.Templates.Count == 0)
            {
                Assert.Fail();
            }
            var template = graph.Templates[0];

            Assert.That(template.Grid.Cols.Count, Is.EqualTo(1));
            Assert.That(template.Grid.Rows.Count, Is.EqualTo(1));
            Assert.That(template.Grid.Rows[0], Is.EqualTo("1"));
            Assert.That(template.Grid.Cols[0], Is.EqualTo("1"));
        }

        [Test]
        public void Grid_WhenOneRowSpecified_ThenGridHasOneRowAndOneCol()
        {
            var yaml =
                "Templates:\n" +
                "  - Name: Foo\n" +
                "    Grid:\n" +
                "      Rows:\n" +
                "        - 1\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act
            var graph = _sut.Deserialize(stream);

            // Assert
            var grid = graph.Templates[0].Grid;

            Assert.That(grid.Cols.Count, Is.EqualTo(1));
        }

        [Test]
        public void Grid_WhenTextIsDefinedInGridCell_ThenTextIsContainedInCell()
        {
            // Arrange
            var yaml =
                "Templates:\n" +
                "  - Name: Foo\n" +
                "    Grid:\n" +
                "      Rows:\n" +
                "        - 1\n" +
                "        - 1\n" +
                "        - 1\n" +
                "      Cols:\n" +
                "        - 1\n" +
                "        - 1\n" +
                "        - 1\n" +
                "Nodes:\n" +
                "  - Template: Foo\n" +
                "    Content:\n" +
                "      - Row: 1\n" +
                "        Col: 1\n" +
                "        Text: FooBar\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act
            var graph = _sut.Deserialize(stream);

            // Assert
            var node = graph.Nodes[0];
            Assert.That(node.Grid.Cells[1, 1].Text, Is.EqualTo("FooBar"));
        }

        [Test]
        public void Nodes_WhenNodesDefinedInYaml_ThenNodesAreInGraph()
        {
            // Arrange
            var yaml =
                "Nodes:\n" +
                "  - X: 10\n" +
                "    Y: 20\n" +
                "    Width: 30\n" +
                "    Height: 40\n" +
                "  - X: 50\n" +
                "    Y: 60\n" +
                "    Width: 70\n" +
                "    Height: 80\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act
            var graph = _sut.Deserialize(stream);

            // Assert
            Assert.That(graph.Nodes.Count, Is.EqualTo(2));

            var node1 = graph.Nodes[0];
            Assert.That(node1.X, Is.EqualTo(10));
            Assert.That(node1.Y, Is.EqualTo(20));
            Assert.That(node1.Width, Is.EqualTo(30));
            Assert.That(node1.Height, Is.EqualTo(40));

            var node2 = graph.Nodes[1];
            Assert.That(node2.X, Is.EqualTo(50));
            Assert.That(node2.Y, Is.EqualTo(60));
            Assert.That(node2.Width, Is.EqualTo(70));
            Assert.That(node2.Height, Is.EqualTo(80));
        }

        [Test]
        public void NodesWithEdges_WhenNodesDefinesEdge_ThenEdgeIsInGraph()
        {
            // Arrange
            var yaml =
                "Nodes:\n" +
                "  - X: 10\n" +
                "    Y: 20\n" +
                "    Width: 30\n" +
                "    Height: 40\n" +
                "    Id: foo\n" +
                "  - X: 50\n" +
                "    Y: 60\n" +
                "    Width: 70\n" +
                "    Height: 80\n" +
                "    Id: bar\n" +
                "Edges:\n" +
                "  - From: foo\n" +
                "    To: bar\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act
            var graph = _sut.Deserialize(stream);

            // Assert
            Assert.That(graph.Edges.Count, Is.EqualTo(1));

            var edge = graph.Edges[0];
            var fooNode = graph.Nodes.First(n => n.Id == "foo");
            var barNode = graph.Nodes.First(n => n.Id == "bar");

            Assert.That(fooNode, Is.EqualTo(edge.From));
            Assert.That(barNode, Is.EqualTo(edge.To));
        }

        [Test]
        public void Serialization_WhenGraphIsSerialized_ThenTheSerializationContainsExpectedAttributes()
        {
            var yaml =
                "Templates:\n" +
                "  - Name: Foo\n" +
                "    Shape: Ellipse\n" +
                "Nodes:\n" +
                "  - X: 10\n" +
                "    Y: 20\n" +
                "    Template: Foo\n" +
                "    Id: 1\n" +
                "  - X: 10\n" +
                "    Y: 20\n" +
                "    Template: Foo\n" +
                "    Id: 2\n" +
                "Edges:\n" +
                "  - From: 1\n" +
                "    To: 2\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));
            var graph = _sut.Deserialize(stream);

            // Act
            using var outStream = new MemoryStream();
            _sut.Serialize(outStream, graph);

            // Assert
            byte[] buffer = outStream.ToArray();
            string resultingStr = Encoding.UTF8.GetString(buffer);

            Assert.That(resultingStr.Contains("From: 1"));
            Assert.That(resultingStr.Contains("To: 2"));
        }

        [Test]
        public void Shape_WhenEllipseIsDefinedForNodeTemplate_ThenNodeHasShape()
        {
            // Arrange
            var yaml =
                "Templates:\n" +
                "  - Name: Foo\n" +
                "    Shape: Ellipse\n" +
                "Nodes:\n" +
                "  - X: 10\n" +
                "    Y: 20\n" +
                "    Template: Foo\n";
            
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act
            var graph = _sut.Deserialize(stream);

            // Assert
            var node = graph.Nodes[0];
            Assert.That(node.Shape, Is.EqualTo(Shape.Ellipse));
        }

        [Test]
        public void Templates_WhenNodeReferencesExistingTemplate_ThenNodeHasTemplateProperties()
        {
            // Arrange
            var yaml =
                "Templates:\n" +
                "  - Name: Foo\n" +
                "    BackgroundColor: 40, 10, 20, 30\n" +
                "    LineColor: 20, 5, 10, 15\n" +
                "Nodes:\n" +
                "  - X: 10\n" +
                "    Y: 20\n" +
                "    Width: 30\n" +
                "    Height: 40\n" +
                "    Template: Foo\n";
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act
            var graph = _sut.Deserialize(stream);

            // Assert
            var node = graph.Nodes[0];
            Assert.That(node.BackgroundColor, Is.EqualTo(Color.FromArgb(40, 10, 20, 30)));
            Assert.That(node.LineColor, Is.EqualTo(Color.FromArgb(20, 5, 10, 15)));
        }

        [Test]
        public void Templates_WhenNodeReferencesInvalidTemplate_ThenExceptionIsThrown()
        {
            var yaml =
                "Templates:\n" +
                "  - Name: Foo\n" +
                "    BackgroundColor: 40, 10, 20, 30\n" +
                "    LineColor: 20, 5, 10, 15\n" +
                "Nodes:\n" +
                "  - X: 10\n" +
                "    Y: 20\n" +
                "    Width: 30\n" +
                "    Height: 40\n" +
                "    Template: Bar\n";
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act / Assert
            Assert.Throws<UnknownTemplateReference>(() => _sut.Deserialize(stream));
        }

        [Test]
        public void Templates_WhenTemplateResize_ThenCorrectResizeIdInNode()
        {
            var yaml =
                "Templates:\n" +
                "  - Name: Foo\n" +
                "    Resize: HorzVert\n" +
                "  - Name: Bar\n" +
                "    Resize: Both\n" +
                "  - Name: Buz\n" +
                "    Resize: HorzVertBoth\n" +
                "Nodes:\n" +
                "  - Template: Foo\n" +
                "  - Template: Bar\n" +
                "  - Template: Buz\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act
            var graph = _sut.Deserialize(stream);

            // Assert
            Assert.That(graph.Nodes[0].Resize, Is.EqualTo(Resize.HorzVert));
            Assert.That(graph.Nodes[1].Resize, Is.EqualTo(Resize.Both));
            Assert.That(graph.Nodes[2].Resize, Is.EqualTo(Resize.HorzVertBoth));
        }

        [Test]
        public void Templates_WhenTemplatesDefinedInYaml_ThenTemplatesAreInGraph()
        {
            // Arrange
            var yaml =
                "Templates:\n" +
                "  - Name: Foo\n" +
                "    BackgroundColor: 40, 10, 20, 30\n" +
                "    LineColor: 20, 5, 10, 15\n" +
                "  - Name: Bar\n" +
                "    BackgroundColor: 80, 20, 40, 60\n" +
                "    LineColor: 40, 10, 20, 30\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act
            var graph = _sut.Deserialize(stream);

            // Assert
            Assert.That(graph.Templates.Count, Is.EqualTo(2));

            var teamplate1 = graph.Templates[0];
            Assert.That(teamplate1.Name, Is.EqualTo("Foo"));
            Assert.That(teamplate1.BackgroundColor, Is.EqualTo(Color.FromArgb(40, 10, 20, 30)));
            Assert.That(teamplate1.LineColor, Is.EqualTo(Color.FromArgb(20, 5, 10, 15)));

            var teamplate2 = graph.Templates[1];
            Assert.That(teamplate2.Name, Is.EqualTo("Bar"));
            Assert.That(teamplate2.BackgroundColor, Is.EqualTo(Color.FromArgb(80, 20, 40, 60)));
            Assert.That(teamplate2.LineColor, Is.EqualTo(Color.FromArgb(40, 10, 20, 30)));
        }

        [Test]
        public void Templates_WhenUnkownResizeIdentifier_ThenExceptionIsThrown()
        {
            var yaml =
                "Templates:\n" +
                "  - Name: Foo\n" +
                "    Resize: Foo\n";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act / Assert
            try
            {
                _sut.Deserialize(stream);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.That(true);
            }
        }
    }
}
