﻿using Gwiz.Core.Contract;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using WinRT;

namespace Gwiz.UiControl.WinUi3
{
    internal class GraphDrawer : Drawer
    {
        private static readonly float ArrowHeadLen = 15;

        private static readonly float ArrowAngleDeg = 30;

        private static readonly Windows.UI.Color LineColor = Windows.UI.Color.FromArgb(255, 0, 0, 0);

        private Point _previewLineFrom = new Point(0, 0);

        private Point _previewLineTo = new Point(0, 0);

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

        public IEdgeTemplate? EdgeCreationActiveSourceTemplate { get; set; }

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

            if (EdgeCreationActiveSourceTemplate != null)
            {
                Draw.DrawLine(_previewLineFrom, _previewLineTo, Style.Dotted, Color.Black, 2);
            }

            DrawEdges();

            DrawNodes();
        }

        public IEdgeTemplate? GetSourceEdgeTemplateAtPosition(INode node, int xPos, int yPos)
        {
            // cannot hit a source template if there are no sources
            if (!node.SourceEdgeTemplates.Any())
            {
                return null;
            }

            // If the mouse x position is not in range we can also hit no source
            if (xPos < node.X - Design.IconSize || xPos > node.X)
            {
                return null;
            }

            // Now we have to check if the mouse y is between start end of an icon
            int iconCount = node.SourceEdgeTemplates.Count + 1; // +1 for the connection icon on top
            int count = 1; // start at 1 to not check the connection icon

            foreach (var edgeTemplate in node.SourceEdgeTemplates)
            {
                int startY = node.Y + node.Height / 2 - (iconCount * Design.IconSize) / 2 + Design.IconSize * count++;

                if (startY < yPos && startY + Design.IconSize > yPos) 
                {
                    return edgeTemplate;
                }
            }

            return null;
        }

        public void PreparePreviewLine(INode node, int xTo, int yTo)
        {
            _previewLineFrom.X = node.X + node.Width / 2;
            _previewLineFrom.Y = node.Y + node.Height / 2;
            _previewLineTo.X = xTo;
            _previewLineTo.Y = yTo;
        }

        public void SetDraw(IDraw draw)
        {
            Draw = draw;
            GridDrawer.Draw = draw;
        }

        private void DrawEdgeCreationIcons(INode node)
        {
            if (EdgeCreationActiveSourceTemplate == null)
            {
                var iconCount = node.SourceEdgeTemplates.Count;

                if (iconCount > 0)
                {
                    // Icon count is increased by one because the additional connection icon is drawn when source icons are available
                    iconCount++;

                    int startY = node.Y + node.Height / 2 - (iconCount * Design.IconSize) / 2;

                    // First icon is always the connection icon to demonstrate that the alpha buttons can be used to select a connection type
                    Draw.DrawSvgIcon(_icons.Connection, new Windows.Foundation.Size(Design.IconSize, Design.IconSize), node.X - Design.IconSize, startY);
                    startY += Design.IconSize;

                    foreach (var edgeTemplate in node.SourceEdgeTemplates)
                    {
                        Draw.DrawSvgIcon(_icons.GetAlpha(edgeTemplate.Icon), new Windows.Foundation.Size(Design.IconSize, Design.IconSize), node.X - Design.IconSize, startY);
                        startY += Design.IconSize;
                    }
                }
            }
            else if (node.TargetEdgeTemplates.Contains(EdgeCreationActiveSourceTemplate))
            {
                int startY = node.Y + node.Height / 2 - Design.IconSize / 2;
                Draw.DrawSvgIcon(_icons.GetAlpha(EdgeCreationActiveSourceTemplate.Icon), new Windows.Foundation.Size(Design.IconSize, Design.IconSize), node.X - Design.IconSize, startY);
            }
        }

        private void DrawEdges()
        {
            if (Edges != null)
            {
                foreach (var edge in Edges)
                {
                    var color = edge.Highlight ? Design.HighlightColor : edge.Select ? Design.SelectionColor : Design.DefaultLineColor;
                    int strokeWidth = edge.Highlight ? Design.HighlightStrokeWidth : edge.Select ? Design.SelectionStrokeWidth : Design.DefaulEdgeStrokeWidth;
                    
                    var modifiedStartPosition = DrawEdgeModifier(edge, true, color, strokeWidth);
                    var modifiedEndingPosition = DrawEdgeModifier(edge, false, color, strokeWidth);

                    Draw.DrawLine(modifiedStartPosition, modifiedEndingPosition, edge.Style, color, strokeWidth);

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

        private Point DrawEdgeModifier(IEdge edge, bool isSourceModifier, Color color, int strokeWidth)
        {
            var modifiedEdgePosition = isSourceModifier ? edge.FromPosition : edge.ToPosition;

            if (isSourceModifier && edge.Beginning == Ending.None ||
                !isSourceModifier && edge.Ending == Ending.None)
            {
                return modifiedEdgePosition;
            }

            var marker = isSourceModifier ? edge.Beginning : edge.Ending;
            var toPos = isSourceModifier ? edge.FromPosition : edge.ToPosition;
            var fromPos = isSourceModifier ? edge.ToPosition : edge.FromPosition;

            var directionVec = new Vector2D(toPos.X - fromPos.X, toPos.Y - fromPos.Y);
            directionVec = directionVec.Normalize();

            var arrowLine1 = directionVec.Rotate(Angle.FromDegrees(ArrowAngleDeg));
            arrowLine1 *= ArrowHeadLen;
            var arrowHead1 = new Point((int)(toPos.X - arrowLine1.X), (int)(toPos.Y - arrowLine1.Y));
            Draw.DrawLine(toPos, arrowHead1, Style.None, color, strokeWidth);

            var arrowLine2 = directionVec.Rotate(Angle.FromDegrees(-ArrowAngleDeg));
            arrowLine2 *= ArrowHeadLen;
            var arrowHead2 = new Point((int)(toPos.X - arrowLine2.X), (int)(toPos.Y - arrowLine2.Y));
            Draw.DrawLine(toPos, arrowHead2, Style.None, color, strokeWidth);

            if (marker == Ending.ClosedArrow)
            {
                // 'Close' the arrow
                Draw.DrawLine(arrowHead1, arrowHead2, Style.None, color, strokeWidth);

                // Calculate the modified ending position (which is the middle of the arrow head closing line)
                var vec = arrowHead2.ToMathNetPoint() - arrowHead1.ToMathNetPoint();
                vec *= 0.5;
                modifiedEdgePosition = new Point((int)(arrowHead1.X + vec.X), (int)(arrowHead1.Y + vec.Y));
            }
            else if (marker == Ending.Rhombus)
            {
                var len = arrowLine1.DotProduct(directionVec);

                directionVec *= len * 2;
                var rhombusPoint = new Point((int)(toPos.X - directionVec.X), (int)(toPos.Y - directionVec.Y));
                Draw.DrawLine(arrowHead1, rhombusPoint, Style.None, color, strokeWidth);
                Draw.DrawLine(arrowHead2, rhombusPoint, Style.None, color, strokeWidth);

                modifiedEdgePosition = new Point((int)rhombusPoint.X, (int)rhombusPoint.Y);
            }

            return modifiedEdgePosition;
        }

        private void DrawSizingIcons(INode node)
        {
            // Draw the resize all icon
            if (node.Resize == Resize.Both || node.Resize == Resize.HorzVertBoth)
            {
                Draw.DrawSvgIcon(_icons.ResizeBottomRight, new Windows.Foundation.Size(Design.IconSize, Design.IconSize), node.X + node.Width - Design.IconSize, node.Y + node.Height - Design.IconSize);
            }

            if (node.Resize == Resize.HorzVert || node.Resize == Resize.HorzVertBoth)
            {
                // Draw the resize horz icon
                Draw.DrawSvgIcon(_icons.ResizeHorz,
                    new Windows.Foundation.Size(Design.IconSize, Design.IconSize),
                    node.X + node.Width - (int)(Design.IconSize * 0.75),
                    node.Y + node.Height / 2 - Design.IconSize / 2);

                // Draw the resize vert icon
                Draw.DrawSvgIcon(_icons.ResizeVert,
                    new Windows.Foundation.Size(Design.IconSize, Design.IconSize),
                    node.X + node.Width / 2 - Design.IconSize / 2,
                    node.Y + node.Height - (int)(Design.IconSize * 0.75));
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
                    DrawEdgeCreationIcons(node);
                    DrawNodeSlection(node);
                }
            }
        }

        private void DrawNodeSlection(INode node)
        {
            if (node.Select)
            {
                var rect = new Rectangle(node.X - Design.SelectionMargin - Design.SelectionStrokeWidth,
                    node.Y - Design.SelectionMargin - Design.SelectionStrokeWidth,
                    node.Width + Design.SelectionMargin * 2 + Design.SelectionStrokeWidth * 2,
                    node.Height + Design.SelectionMargin * 2 + Design.SelectionStrokeWidth * 2);

                Draw.DrawRectangle(rect, Design.SelectionColor, Design.SelectionStrokeWidth);
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