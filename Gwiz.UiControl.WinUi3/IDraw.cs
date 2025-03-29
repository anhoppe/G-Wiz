using Gwiz.Core.Contract;
using SkiaSharp;
using System.Drawing;

namespace Gwiz.UiControl.WinUi3
{
    /// <summary>
    /// Interface for drawing on a canvas
    /// </summary>
    internal interface IDraw
    {
        void Clear();

        void DrawEllipse(Rectangle rect, Color color);
        
        void DrawLine(Point from, Point to, Style style);

        void DrawRectangle(Rectangle rect, Color color, float strokeWidth);

        void DrawSvgIcon(SKBitmap? icon, Windows.Foundation.Size size, float x, float y);

        void DrawClippedText(string text, Rectangle rect, Point position, Color color);

        void DrawText(string text, Point position, Color color);

        void FillEllipse(Rectangle rect, Color backgroundColor);
        
        void FillRectangle(Rectangle rect, Color backgroundColor);
    }
}