using Gwiz.Core.Contract;
using System.Collections.Generic;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using System.Drawing;
using SkiaSharp;
using System;

namespace Gwiz.UiControl.WinUi3
{
    internal class GraphDrawer
    {
        private static readonly float ArrowHeadLen = 15;

        private static readonly float ArrowAngleDeg = 30;

        private static readonly Windows.UI.Color LineColor = Windows.UI.Color.FromArgb(255, 0, 0, 0);

        private Icons _icons = new Icons();

        public IDraw Draw { private get; set; } = new Draw();

        public List<IEdge> Edges { get; set; } = new();

        public int IconSize => 30;

        public List<INode> Nodes { get; set; } = new();

        public void DrawGraph()
        {
            Draw.Clear();

            DrawEdges();

            DrawNodes();
        }

        private void DrawEdges()
        {
            if (Edges != null)
            {
                foreach (var edge in Edges)
                {
                    var modifiedEndingPosition = DrawEdgeEnding(edge);

                    Draw.DrawLine(edge.FromPosition, modifiedEndingPosition, edge.Style);

                    if (!string.IsNullOrEmpty(edge.FromLabel))
                    {
                        (var fromPos, var toPos) = GetFromToLabelPos(edge);

                        Draw.DrawText(edge.FromLabel, fromPos, Color.Black);
                        Draw.DrawText(edge.ToLabel, toPos, Color.Black);
                    }
                }
            }
        }

        private Point DrawEdgeEnding(IEdge edge)
        {
            var modifiedEndingPosition = edge.ToPosition;

            if (edge.Ending == Ending.None)
            {
                return modifiedEndingPosition;
            }

            var directionVec = new Vector2D(edge.ToPosition.X - edge.FromPosition.X, edge.ToPosition.Y - edge.FromPosition.Y);
            directionVec = directionVec.Normalize();

            var arrowLine1 = directionVec.Rotate(Angle.FromDegrees(ArrowAngleDeg));
            arrowLine1 *= ArrowHeadLen;
            var arrowHead1 = new Point((int)(edge.ToPosition.X - arrowLine1.X), (int)(edge.ToPosition.Y - arrowLine1.Y));
            Draw.DrawLine(edge.ToPosition, arrowHead1, Style.None);

            var arrowLine2 = directionVec.Rotate(Angle.FromDegrees(-ArrowAngleDeg));
            arrowLine2 *= ArrowHeadLen;
            var arrowHead2 = new Point((int)(edge.ToPosition.X - arrowLine2.X), (int)(edge.ToPosition.Y - arrowLine2.Y));
            Draw.DrawLine(edge.ToPosition, arrowHead2, Style.None);

            if (edge.Ending == Ending.ClosedArrow)
            {
                // 'Close' the arrow
                Draw.DrawLine(arrowHead1, arrowHead2, Style.None);

                // Calculate the modified ending position (which is the middle of the arrow head closing line)
                var vec = arrowHead2.ToMathNetPoint() - arrowHead1.ToMathNetPoint();
                vec *= 0.5;
                modifiedEndingPosition = new Point((int)(arrowHead1.X + vec.X), (int)(arrowHead1.Y + vec.Y));
            }
            else if (edge.Ending == Ending.Rhombus)
            {
                var len = arrowLine1.DotProduct(directionVec);

                directionVec *= len * 2;
                var rhombusPoint = new Point((int)(edge.ToPosition.X - directionVec.X), (int)(edge.ToPosition.Y - directionVec.Y));
                Draw.DrawLine(arrowHead1, rhombusPoint, Style.None);
                Draw.DrawLine(arrowHead2, rhombusPoint, Style.None);

                modifiedEndingPosition = new Point((int)rhombusPoint.X, (int)rhombusPoint.Y);
            }

            return modifiedEndingPosition;
        }

        private void DrawNodeGrid(INode node)
        {
            var grid = node.Grid;
            for (int x = 0; x < grid.Cols.Count; x++)
            {
                for (int y = 0; y < grid.Rows.Count; y++)
                {
                    var rect = grid.FieldRects[x][y];
                    Draw.FillRectangle(rect, node.BackgroundColor);
                    Draw.DrawRectangle(rect, node.LineColor, 1);

                    var textSize = GetTextSize(grid.FieldText[x][y]);
                    var xText = (float)(rect.X + (rect.Width - textSize.Width) / 2);
                    var yText = (float)(rect.Y + (rect.Height - textSize.Height) / 2);
                    Draw.DrawText(grid.FieldText[x][y], new Point((int)xText, (int)yText), node.LineColor);
                }
            }
        }

        private void DrawNodes()
        {
            if (Nodes != null)
            {
                foreach (var node in Nodes)
                {
                    DrawNodeGrid(node);

                    // Draw the resize all icon
                    if (node.Resize == Resize.Both || node.Resize == Resize.HorzVertBoth)
                    {
                        Draw.DrawSvgIcon(_icons.ResizeBottomRight, new Windows.Foundation.Size(IconSize, IconSize), node.X + node.Width - IconSize, node.Y + node.Height - IconSize);
                    }

                    if (node.Resize == Resize.HorzVert || node.Resize == Resize.HorzVertBoth)
                    {
                        // Draw the resize horz icon
                        Draw.DrawSvgIcon(_icons.ResizeHorz, new Windows.Foundation.Size(IconSize, IconSize), node.X + node.Width - (int)(IconSize * 0.75), node.Y + node.Height / 2 - IconSize / 2);

                        // Draw the resize vert icon
                        Draw.DrawSvgIcon(_icons.ResizeVert, new Windows.Foundation.Size(IconSize, IconSize), node.X + node.Width / 2 - IconSize / 2, node.Y + node.Height - (int)(IconSize * 0.75));
                    }
                }
            }
        }

        private (Point fromPos, Point toPos) GetFromToLabelPos(IEdge edge)
        {
            Point fromPos;
            Point toPos;

            var directionVec = edge.ToPosition.ToMathNetPoint() -
                edge.FromPosition.ToMathNetPoint();

            var len = directionVec.Length;
            directionVec = directionVec.Normalize();
            directionVec *= len * edge.LabelOffsetPerCent / 100.0f;

            fromPos = edge.FromPosition.Add(directionVec);

            directionVec *= -1;
            toPos = edge.ToPosition.Add(directionVec);

            return (fromPos, toPos);
        }

        private static Size GetTextSize(string text)
        {
            var size = new Size(0, 0);

            using (var font = new SKFont
            {
                Size = 16,
                Typeface = SKTypeface.FromFamilyName("Segoe UI") // Use Segoe UI
            })
            {
                float textWidth = font.MeasureText(text);

                // Measure height using FontMetrics
                var metrics = font.Metrics;
                float textHeight = /*metrics.Descent - metrics.Ascent;*/metrics.XHeight;

                size.Width = (int)textWidth;
                size.Height = (int)textHeight;
            }

            return size;
        }
    }
}
