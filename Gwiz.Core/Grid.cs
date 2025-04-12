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
            GridCellInternal = new GridCell[1][];
            Cells[0] = new GridCell[1];
            Cells[0][0] = new GridCell();
        }

        public IGridCell[][] Cells
        {
            get
            {
                return GridCellInternal;
            }
        }

        public List<string> Cols { get; set; } = new();

        public int EditButtonMargin { get; set; } = 10;

        public List<string> Rows { get; set; } = new();

        internal GridCell[][] GridCellInternal { get; set; }

        public static Grid CreateFromTemplateGrid(IGrid grid)
        {
            var createdGrid = new Grid();

            createdGrid.Cols = new(grid.Cols);
            createdGrid.Rows = new(grid.Rows);

            createdGrid.GridCellInternal = new GridCell[grid.Cols.Count][];
            for (int x = 0; x < grid.Cols.Count; x++)
            {
                createdGrid.Cells[x] = new GridCell[grid.Rows.Count];
                for (int y = 0; y < grid.Rows.Count; y++)
                {
                    createdGrid.Cells[x][y] = new GridCell();
                }
            }

            return createdGrid;
        }

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
                    GridCellInternal[x][y].Rectangle = new Rectangle(xPos, yPos, width, height);

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