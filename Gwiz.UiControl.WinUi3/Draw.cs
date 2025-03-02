using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Svg;
using Microsoft.Graphics.Canvas.Text;
using System;
using System.Drawing;
using System.Numerics;
using Windows.Foundation;

namespace Gwiz.UiControl.WinUi3
{
    internal class Draw : IDraw
    {
        private static readonly Windows.UI.Color LineColor = Windows.UI.Color.FromArgb(255, 0, 0, 0);

        internal CanvasDrawingSession? DrawingSession { private get; set; }

        public void Clear()
        {
            if (DrawingSession == null)
            {
                throw new NullReferenceException("Draw.DrawingSession not set");
            }

            DrawingSession.Clear(Microsoft.UI.Colors.CornflowerBlue);
        }

        public void DrawLine(System.Drawing.Point from, System.Drawing.Point to)
        {
            if (DrawingSession == null)
            {
                throw new NullReferenceException("Draw.DrawingSession not set");
            }

            DrawingSession.DrawLine(ConvertPoint(from), ConvertPoint(to), LineColor);
        }

        public void DrawRectangle(Rectangle rect, Color color, float strokeWidth)
        {
            if (DrawingSession == null)
            {
                throw new NullReferenceException("Draw.DrawingSession not set");
            }

            DrawingSession.DrawRectangle(ConvertRect(rect), ConvertColor(color), strokeWidth);
        }

        public void DrawSvgIcon(CanvasSvgDocument? icon, Windows.Foundation.Size size, float x, float y)
        {
            if (DrawingSession == null)
            {
                throw new NullReferenceException("Draw.DrawingSession not set");
            }

            if (icon == null)
            {
                throw new NullReferenceException("Draw.DrawIcon recevied null icon document");
            }

            DrawingSession.DrawSvg(icon, size, new Vector2(x, y));
        }

        public void DrawText(string text, System.Drawing.Point position, Color color, CanvasTextFormat font)
        {
            if (DrawingSession == null)
            {
                throw new NullReferenceException("Draw.DrawingSession not set");
            }

            DrawingSession.DrawText(text, ConvertPoint(position), ConvertColor(color), font);
        }

        public void FillRectangle(Rectangle rect, Color backgroundColor)
        {
            if (DrawingSession == null)
            {
                throw new NullReferenceException("Draw.DrawingSession not set");
            }

            DrawingSession.FillRectangle(ConvertRect(rect), ConvertColor(backgroundColor));
        }


        public static Windows.UI.Color ConvertColor(System.Drawing.Color color) => Windows.UI.Color.FromArgb(color.A, color.R, color.G, color.B);

        private static Vector2 ConvertPoint(System.Drawing.Point pos) => new Vector2(pos.X, pos.Y);

        private static Rect ConvertRect(System.Drawing.Rectangle rect) => new Rect(rect.X, rect.Y, rect.Width, rect.Height);

    }
}
