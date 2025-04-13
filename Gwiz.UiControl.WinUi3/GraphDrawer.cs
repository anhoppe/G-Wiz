using Gwiz.Core.Contract;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using WinRT;

namespace Gwiz.UiControl.WinUi3
{
    internal class GraphDrawer : Drawer
    {
        private static readonly float ArrowHeadLen = 15;

        private static readonly float ArrowAngleDeg = 30;

        private static readonly Windows.UI.Color LineColor = Windows.UI.Color.FromArgb(255, 0, 0, 0);

        private Icons _icons = new Icons();

        public GraphDrawer()
        {
            GridDrawer = new GridDrawer()
            {
                Draw = Draw,
                Icons = _icons,
                TextSizeCalculator = TextSizeCalculator,
            };
        }

        public List<IEdge> Edges { get; set; } = new();

        public List<INode> Nodes { get; set; } = new();

        public Func<string, Size> TextSizeCalculator { get; set; } = (text) =>
        {
            var size = new Size(0, 0);

            using (var font = new SKFont
            {
                Size = 16,
                Typeface = SKTypeface.FromFamilyName("Segoe UI") // Use Segoe UI
            })
            {
                var metrics = font.Metrics;
                float lineHeight = metrics.Descent - metrics.Ascent; // Correct height per line
                float baselineAdjustment = lineHeight / 2; // Fix baseline offset

                string[] lines = text.Split('\n'); // Handle multi-line text

                float maxWidth = 0;
                foreach (string line in lines)
                {
                    float lineWidth = font.MeasureText(line);
                    maxWidth = Math.Max(maxWidth, lineWidth);
                }

                size.Width = (int)Math.Ceiling(maxWidth);
                size.Height = (int)Math.Ceiling(lineHeight * (lines.Length - 1) - baselineAdjustment); // Adjust for baseline
            }

            return size;
        };

        internal IGridDrawer GridDrawer { get; set; }

        public void DrawGraph()
        {
            Draw.Clear();

            DrawEdges();

            DrawNodes();
        }

        public void SetDraw(IDraw draw)
        {
            Draw = draw;
            GridDrawer.Draw = draw;
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

                    if (!string.IsNullOrEmpty(edge.Text))
                    {
                        var size = TextSizeCalculator(edge.Text);
                        var center = GeoConv.Center(edge.FromPosition, edge.ToPosition);

                        Draw.DrawText(edge.Text, new Point(center.X - size.Width / 2, center.Y - size.Height / 2), Color.Black);
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


        private void DrawSizingIcons(INode node)
        {
            // Draw the resize all icon
            if (node.Resize == Resize.Both || node.Resize == Resize.HorzVertBoth)
            {
                Draw.DrawSvgIcon(_icons.ResizeBottomRight, new Windows.Foundation.Size(IconSize, IconSize), node.X + node.Width - IconSize, node.Y + node.Height - IconSize);
            }

            if (node.Resize == Resize.HorzVert || node.Resize == Resize.HorzVertBoth)
            {
                // Draw the resize horz icon
                Draw.DrawSvgIcon(_icons.ResizeHorz,
                    new Windows.Foundation.Size(IconSize, IconSize),
                    node.X + node.Width - (int)(IconSize * 0.75),
                    node.Y + node.Height / 2 - IconSize / 2);

                // Draw the resize vert icon
                Draw.DrawSvgIcon(_icons.ResizeVert,
                    new Windows.Foundation.Size(IconSize, IconSize),
                    node.X + node.Width / 2 - IconSize / 2,
                    node.Y + node.Height - (int)(IconSize * 0.75));
            }
        }

        private void DrawNodes()
        {
            if (Nodes != null)
            {
                foreach (var node in Nodes)
                {
                    GridDrawer.DrawGrid(node);
                    DrawSizingIcons(node);
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
    }
}