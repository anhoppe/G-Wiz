using Gwiz.Core.Contract;
using SkiaSharp;
using System;
using System.Drawing;

namespace Gwiz.UiControl.WinUi3
{
    internal class Draw : IDraw
    {
        private static readonly SKColor LineColor = SKColors.Black;

        internal SKCanvas? DrawingSession { private get; set; }

        public void Clear()
        {
            if (DrawingSession == null)
            {
                throw new NullReferenceException("Draw.DrawingSession not set");
            }

            DrawingSession.Clear(SKColors.CornflowerBlue);
        }

        public void DrawLine(Point from, Point to, Style style)
        {
            if (DrawingSession == null)
            {
                throw new NullReferenceException("Draw.DrawingSession not set");
            }

            using (var paint = new SKPaint
            {
                Color = LineColor,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2
            })
            {
                switch (style)
                {
                    case Style.Dashed:
                        paint.PathEffect = SKPathEffect.CreateDash(new float[] { 10, 10 }, 0);
                        break;
                    case Style.Dotted:
                        paint.PathEffect = SKPathEffect.CreateDash(new float[] { 2, 2 }, 0);
                        break;
                }
                DrawingSession.DrawLine(from.ToVector2(), to.ToVector2(), paint);
            }
        }

        public void DrawRectangle(Rectangle rect, Color color, float strokeWidth)
        {
            if (DrawingSession == null)
            {
                throw new NullReferenceException("Draw.DrawingSession not set");
            }

            using (var paint = new SKPaint { Color = ConvertColor(color), Style = SKPaintStyle.Stroke, StrokeWidth = 2 })
            {
                DrawingSession.DrawRect(rect.ToSKRect(), paint);
            }
        }

        public void DrawSvgIcon(SKBitmap? icon, Windows.Foundation.Size size, float x, float y)
        {
            if (DrawingSession == null)
            {
                throw new NullReferenceException("Draw.DrawingSession not set");
            }

            if (icon == null)
            {
                throw new NullReferenceException("Draw.DrawIcon recevied null icon document");
            }

            DrawingSession.DrawBitmap(icon, new SKPoint(x, y));
        }

        public void DrawClippedText(string text, Rectangle rect, Point position, Color color)
        {
            if (DrawingSession == null)
            {
                throw new NullReferenceException("Draw.DrawingSession not set");
            }

            DrawingSession.Save();
            DrawingSession.ClipRect(rect.ToSKRect()); // Restrict drawing to the box

            DrawText(text, position, color);

            DrawingSession.Restore();
        }

        public void DrawText(string text, Point position, Color color)
        {
            if (DrawingSession == null)
            {
                throw new NullReferenceException("Draw.DrawingSession not set");
            }

            using (var paint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
            })
            {
                using (var font = new SKFont
                {
                    Size = 16,
                    Typeface = SKTypeface.FromFamilyName("Segoe UI") // Use Segoe UI
                })
                {
                    float textWidth = font.MeasureText(text);

                    // Measure height using FontMetrics
                    var metrics = font.Metrics;
                    float textHeight = metrics.Descent - metrics.Ascent;
                    const float lineFeed = 0.12f;

                    string[] lines = text.Split('\n');
                    int count = 0;

                    foreach (var line in lines)
                    {
                        DrawingSession.DrawText(line, position.X, position.Y + count++ * (textHeight + lineFeed), font, paint);
                    }
                }
            }
        }

        public void FillRectangle(Rectangle rect, Color backgroundColor)
        {
            if (DrawingSession == null)
            {
                throw new NullReferenceException("Draw.DrawingSession not set");
            }

            if (DrawingSession == null)
            {
                throw new NullReferenceException("Draw.DrawingSession not set");
            }

            using (var paint = new SKPaint { Color = ConvertColor(backgroundColor), Style = SKPaintStyle.Fill, StrokeWidth = 1 })
            {
                DrawingSession.DrawRect(rect.ToSKRect(), paint);
            }
        }

        private static SKColor ConvertColor(Color color) => new SKColor(color.R, color.G, color.B, color.A);
    }
}