using Gwiz.Core.Contract;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Gwiz.Core
{
    internal class Grid : IUpdatableGrid
    {
        public Grid()
        {
            FieldText = new string[1][];
            FieldText[0] = new string[1];

            FieldRects = new Rectangle[1][];
            FieldRects[0] = new Rectangle[1];
        }

        public Grid(IGrid grid)
        {
            Cols = new(grid.Cols);
            Rows = new(grid.Rows);

            FieldText = new string[Cols.Count][];
            FieldRects = new Rectangle[Cols.Count][];

            for (int x = 0; x < Cols.Count; x++)
            {
                FieldText[x] = new string[Rows.Count];
                FieldRects[x] = new Rectangle[Rows.Count];
                for (int y = 0; y < Rows.Count; y++)
                {
                    FieldText[x][y] = string.Empty;
                    FieldRects[x][y] = new Rectangle();
                }
            }
        }

        public List<string> Cols { get; set; } = new();

        public int EditButtonMargin { get; set; } = 10;

        public Rectangle[][] FieldRects { get; }

        public string[][] FieldText { get; set; }

        public List<string> Rows { get; set; } = new();

        public void UpdateFieldRects(INode parentNode)
        {
            var xRatio = ToRatioFactor(Cols);
            var yRatio = ToRatioFactor(Rows);

            var xPos = parentNode.X;

            for (int x = 0; x < Cols.Count; x++)
            {
                int width = (int)(parentNode.Width * xRatio[x]);
                var yPos = parentNode.Y;

                for (int y = 0; y < Rows.Count; y++)
                {
                    int height = (int)(parentNode.Height * yRatio[y]);
                    FieldRects[x][y] = new Rectangle(xPos, yPos, width, height);

                    yPos += height;
                }

                xPos += width;
            }
        }

        public void RegisterParentNodeChanged(IUpdatableNode parentNode)
        {
            parentNode.NodeChanged += (sender, args) =>
            {
                UpdateFieldRects(parentNode);
            };
        }

        private static List<double> ToRatioFactor(IList<string> fields)
        {
            if (fields.Count == 0)
            {
                return new();
            }

            var fieldsAsInt = fields.Select(p => int.TryParse(p, out var field) ? field : 0);
            var fieldSum = fieldsAsInt.Sum();
            var ratioFactors = new List<double>();
            foreach (var field in fieldsAsInt)
            {
                ratioFactors.Add(field / (double)fieldSum);
            }

            return ratioFactors;
        }
    }
}