using SkiaSharp;
using System;
using System.Drawing;

namespace Gwiz.UiControl.WinUi3
{
    public static class TextSizeCalculator
    {
        public static Size GetTextSize(string text)
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

        }
    }
}
