using MathNet.Spatial.Euclidean;
using SkiaSharp;
using System;
using System.Drawing;
using System.Numerics;

namespace Gwiz.UiControl.WinUi3
{
    public static class GeoConv
    {
        public static Point Add(this Point point1, Vector2D vec) => new Point((int)(point1.X + vec.X), (int)(point1.Y + vec.Y));

        public static Rectangle Add(this Rectangle rect, Rectangle otherRect)
        {
            rect.X = Math.Min(rect.X, otherRect.X);
            rect.Y = Math.Min(rect.Y, otherRect.Y);
            rect.Width = Math.Max(rect.Width, otherRect.Width);
            rect.Height = Math.Max(rect.Height, otherRect.Height);

            return rect;
        }

        public static Point2D ToMathNetPoint(this Point pos) => new Point2D(pos.X, pos.Y);
        
        public static SKRect ToSKRect(this Rectangle rect) => new SKRect(rect.X, rect.Y, rect.X + rect.Width, rect.Y + rect.Height);
        
        public static Vector2 ToVector2(this Point pos) => new Vector2(pos.X, pos.Y);
    }
}
