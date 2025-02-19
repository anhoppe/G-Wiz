using System.Collections.Generic;
using System.Drawing;

namespace Gwiz.Core.Contract
{
    public class Grid
    {
        public Grid()
        {
            FieldText = new string[1][];

            FieldText[0] = new string[1];

            TextSizeFactory = text => throw new InvalidOperationException("Cannot use TextSizeFactory without injecting method from host");
        }

        public Grid(Grid grid)
        {
            Cols = new(grid.Cols);

            FieldText = new string[grid.Cols.Count][];
            for (int x = 0; x < grid.Cols.Count; x++)
            {
                FieldText[x] = new string[grid.Rows.Count];
                for (int y = 0; y < grid.Rows.Count; y++)
                {
                    FieldText[x][y] = "Hello, world...";
                }
            }

            ParentNode = grid.ParentNode;

            Rows = new(grid.Rows);

            TextSizeFactory = grid.TextSizeFactory;
        }

        /// <summary>
        /// Defines the columns in the grid
        /// Each entry represents the available horizontal space
        /// </summary>
        public List<string> Cols { get; set; } = new();

        public int EditButtonMargin { get; set; } = 10;

        public int IconSize { get; set; } = 30;

        public Node? ParentNode { get; set; }

        /// <summary>
        /// Defines the rows in the grid
        /// Each entry represents the available vertical space
        /// </summary>
        public List<string> Rows { get; set; } = new();

        public List<int> GetColLinePositions() 
        {
            if (ParentNode == null)
            {
                throw new NullReferenceException("ParentNode of Grid is not set");
            }

            return ToFieldLinePositions(Cols, ParentNode.Width);
        }

        public (Point, Point) GetFieldTextAndEditButtonPosition(int col, int row)
        {
            if (ParentNode == null)
            {
                throw new InvalidOperationException("Cannot determine text position for Grid field when parent Node not set");
            }

            if (col < 0 || col >= Cols.Count ||
                row < 0 || row >= Rows.Count)
            {
                throw new ArgumentException($"Tried to access field {col}:{row}, but field size is only {Cols.Count}:{Rows.Count}");
            }

            string text = FieldText[col][row];

            var size = TextSizeFactory.Invoke(text);

           
            int xText = 0;
            int xEdit = 0;
            if (Cols.Count == 1)
            {
                xText = ParentNode.X + ParentNode.Width / 2 - size.Width / 2;
                xEdit = ParentNode.X + ParentNode.Width / 2 + size.Width / 2 + EditButtonMargin;
            }
            else 
            {
                var colLinePositions = GetColLinePositions();
                if (col == 0)
                {
                    xText = ParentNode.X + colLinePositions[0] / 2 - size.Width / 2;
                    xText = ParentNode.X + colLinePositions[0] / 2 + size.Width / 2 + EditButtonMargin;
                }
                else if (col == Cols.Count - 1)
                {
                    xText = ParentNode.X + colLinePositions[col - 1] + (ParentNode.Width - colLinePositions[col - 1]) / 2 - size.Width / 2;
                    xEdit = ParentNode.X + colLinePositions[col - 1] + (ParentNode.Width - colLinePositions[col - 1]) / 2 + size.Width / 2 + EditButtonMargin;
                }
                else
                {
                    xText = ParentNode.X + colLinePositions[col - 1] + (colLinePositions[col] - colLinePositions[col - 1]) / 2 - size.Width / 2;
                    xText = ParentNode.X + colLinePositions[col - 1] + (colLinePositions[col] - colLinePositions[col - 1]) / 2 - size.Width / 2 + EditButtonMargin;
                }
            }

            int yText = 0;
            int yEdit = 0;
            if (Rows.Count == 1)
            {
                yText = ParentNode.Y + ParentNode.Height / 2 - size.Height / 2;
                yEdit = ParentNode.Y + ParentNode.Height / 2 - IconSize / 2;
            }
            else
            {
                var rowLinePositions = GetRowLinePositions();
                if (row == 0)
                {
                    yText = ParentNode.Y + rowLinePositions[0] / 2 - size.Height / 2;
                    yEdit = ParentNode.Y + rowLinePositions[0] / 2 - IconSize / 2;
                }
                else if (row == Rows.Count - 1)
                {
                    yText = ParentNode.Y + rowLinePositions[row - 1] + (ParentNode.Height - rowLinePositions[row - 1]) / 2 - size.Height / 2;
                    yEdit = ParentNode.Y + rowLinePositions[row - 1] + (ParentNode.Height - rowLinePositions[row - 1]) / 2 - IconSize / 2;
                }
                else
                {
                    yText = ParentNode.Y + rowLinePositions[row - 1] + (rowLinePositions[row] - rowLinePositions[row - 1]) / 2 - size.Height / 2;
                    yEdit = ParentNode.Y + rowLinePositions[row - 1] + (rowLinePositions[row] - rowLinePositions[row - 1]) / 2 - IconSize / 2;
                }
            }

            return (new Point(xText, yText), new Point(xEdit, yEdit));
        }

        /// <summary>
        /// Row line positions relative to the y position
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public List<int> GetRowLinePositions()
        {
            if (ParentNode == null)
            {
                throw new NullReferenceException("ParentNode of Grid is not set");
            }

            return ToFieldLinePositions(Rows, ParentNode.Height);
        }

        /// <summary>
        /// Text in the defined grid fields
        /// </summary>
        public string[][] FieldText { get; set; }

        /// <summary>
        /// Factory to create a text size used for text positioning
        /// </summary>
        public Func<string, Size> TextSizeFactory { private get; set; }

        private List<int> ToFieldLinePositions(IList<string> fields, int space)
        {
            if (fields.Count == 0)
            {
                return new();
            }

            var fieldsAsInt = fields.Select(p => int.TryParse(p, out var field) ? field : 0);
            var fieldSum = fieldsAsInt.Sum();

            int linePosition = 0;
    
            var linePositions = new List<int>();

            foreach (var field in fieldsAsInt.SkipLast(1))
            {
                double perCent = 100.0 / fieldSum * field;
                linePosition += (int)(space / 100.0 * perCent);
                linePositions.Add(linePosition);
            }

            return linePositions;
        }
    }
}
