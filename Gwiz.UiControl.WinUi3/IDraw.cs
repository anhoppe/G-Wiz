using Microsoft.Graphics.Canvas.Svg;
using Microsoft.Graphics.Canvas.Text;
using System.Drawing;

namespace Gwiz.UiControl.WinUi3
{
    /// <summary>
    /// Interface for drawing on a canvas
    /// </summary>
    internal interface IDraw
    {
        void Clear();

        void DrawLine(Point from, Point to);

        void DrawRectangle(Rectangle rect, Color color, float strokeWidth);

        void DrawSvgIcon(CanvasSvgDocument? icon, Windows.Foundation.Size size, float x, float y);

        void DrawText(string text, Point position, Color color, CanvasTextFormat font);

        void FillRectangle(Rectangle rect, Color backgroundColor);
    }
}
