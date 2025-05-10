using System.Diagnostics;
using System.Drawing;

namespace Gwiz.Core.Contract
{
    public enum Alignment
    {
        TopLeft,

        TopCenter,

        TopRight,

        CenterLeft,

        CenterCenter,

        CenterRight,

        BottomLeft,

        BottomCenter,

        BottomRight,
    }

    public static class AlignmentExtension
    {
        public static Alignment ToAlignment(this string asString) => asString.ToLower() switch
        {
            "topleft" => Alignment.TopLeft,
            "topcenter" => Alignment.TopCenter,
            "topright" => Alignment.TopRight,
            "centerleft" => Alignment.CenterLeft,
            "centercenter" => Alignment.CenterCenter,
            "centerright" => Alignment.CenterRight,
            "bottomleft" => Alignment.BottomLeft,
            "bottomcenter" => Alignment.BottomCenter,
            "bottomright" => Alignment.BottomRight,

            _ => throw new ArgumentException($"Invalid alignment string: {asString}")
        };

        public static Point ToPosition(this Alignment alignment, Rectangle targetRect, Size contentSize) => alignment switch
        {
            Alignment.TopLeft => new Point(targetRect.Left, targetRect.Top),
            Alignment.TopCenter => new Point(targetRect.X + (targetRect.Width - contentSize.Width) / 2, targetRect.Top),
            Alignment.TopRight => new Point(targetRect.Right - contentSize.Width, targetRect.Top),
            Alignment.CenterLeft => new Point(targetRect.Left, targetRect.Y + (targetRect.Height - contentSize.Height) / 2),
            Alignment.CenterCenter => new Point(targetRect.X + (targetRect.Width - contentSize.Width) / 2, targetRect.Y + (targetRect.Height - contentSize.Height) / 2),
            Alignment.CenterRight => new Point(targetRect.Right - contentSize.Width, targetRect.Y + (targetRect.Height - contentSize.Height) / 2),
            Alignment.BottomLeft => new Point(0, targetRect.Bottom - contentSize.Height),
            Alignment.BottomCenter => new Point(targetRect.X + (targetRect.Width - contentSize.Width) / 2, targetRect.Bottom - contentSize.Height),
            Alignment.BottomRight => new Point(targetRect.Right - contentSize.Width, targetRect.Bottom - contentSize.Height),
            _ => throw new UnknownTemplateParameterValue($"Alignment {alignment} cannot be converted to position"),
        };
    }
}
