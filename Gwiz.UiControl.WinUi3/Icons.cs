using SkiaSharp;
using SkiaSharp.Extended.Svg;
using System.IO;
using System.Reflection;

namespace Gwiz.UiControl.WinUi3
{
    internal class Icons
    {
        public Icons()
        {
            LoadSvgAsync();
        }

        public SKBitmap? Edit { get; private set; }
        
        public SKBitmap? ResizeBottomRight { get; private set; }

        public SKBitmap? ResizeHorz { get; private set; }

        public SKBitmap? ResizeVert { get; private set; }

        private static Stream GetEmbeddedSvgStream(string iconName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = "Gwiz.UiControl.WinUi3.icons." + iconName; // Adjust the namespace

            Stream? stream = assembly.GetManifestResourceStream(resourceName);

            if (stream == null)
            {
                throw new FileNotFoundException($"Resource {resourceName} not found in assembly {assembly.FullName}");
            }

            return stream;
        }

        private void LoadSvgAsync()
        {

            using (var stream = GetEmbeddedSvgStream("resize-bottom-right.png"))
            {
                ResizeBottomRight = SKBitmap.Decode(stream);
            }
            using (var stream = GetEmbeddedSvgStream("drag-horizontal-variant.png"))
            {
                ResizeVert = SKBitmap.Decode(stream);
            }
            using (var stream = GetEmbeddedSvgStream("drag-vertical-variant.png"))
            {
                ResizeHorz = SKBitmap.Decode(stream);
            }
        }
    }
}
