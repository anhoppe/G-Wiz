using Gwiz.Core.Contract;
using System;
using System.Drawing;

namespace Gwiz.UiControl.WinUi3
{
    internal class GridDrawer : Drawer, IGridDrawer
    {
        internal Func<string, Size> TextSizeCalculator { get; set; } = (text) => new Size(0, 0);

        internal Icons Icons { private get; set; } = new Icons();

        public void DrawGrid(INode node)
        {
            var grid = node.Grid;

            if (grid == null)
            {
                return;
            }

            var totalRect = new Rectangle(int.MaxValue, int.MaxValue, -1, -1);

            if (node.Shape == Shape.Ellipse)
            {
                for (int x = 0; x < grid.Cols.Count; x++)
                {
                    for (int y = 0; y < grid.Rows.Count; y++)
                    {
                        var rect = grid.Cells[x, y].Rectangle;

                        totalRect = totalRect.Add(rect);
                    }
                }

                Draw.FillEllipse(totalRect, node.BackgroundColor);
                Draw.DrawEllipse(totalRect, node.LineColor);
            }

            for (int x = 0; x < grid.Cols.Count; x++)
            {
                for (int y = 0; y < grid.Rows.Count; y++)
                {
                    var cell = grid.Cells[x, y];
                    var rect = cell.Rectangle;

                    if (node.Shape == Shape.Rectangle)
                    {
                        Draw.FillRectangle(rect, node.BackgroundColor);
                        Draw.DrawRectangle(rect, node.LineColor, 1);
                    }

                    var text = cell.Text;

                    var textPos = node.Alignment.ToPosition(rect, TextSizeCalculator(cell.Text));

                    if (!string.IsNullOrEmpty(text))
                    {
                        Draw.DrawClippedText(text,
                            rect,
                            textPos,
                            node.LineColor);
                    }

                    if (cell.Editable)
                    {
                        if (cell.EditModeEnabled)
                        {
                            var textBeforeCursor = text.Substring(0, cell.EditTextPosition);

                            var size = TextSizeCalculator(textBeforeCursor);

                            Draw.DrawLine(new Point(textPos.X + size.Width, textPos.Y + 3),
                                new Point(textPos.X + size.Width, textPos.Y + size.Height - 3),
                                Style.None,
                                Color.Black,
                                2);
                        }

                        Draw.DrawSvgIcon(Icons.Edit,
                            Design.IconSize.ToWinSize(),
                            rect.X,
                            rect.Y + rect.Height / 2 - Design.IconLength / 2);
                    }
                }
            }
        }
    }
}
