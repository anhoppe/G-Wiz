using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Svg;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Gwiz.UiControl.WinUi3
{
    internal class Icons
    {
        public Icons()
        {
            LoadSvgAsync();
        }

        public CanvasSvgDocument? Edit { get; private set; }
        
        public CanvasSvgDocument? ResizeBottomRight { get; private set; }

        public CanvasSvgDocument? ResizeHorz { get; private set; }

        public CanvasSvgDocument? ResizeVert { get; private set; }

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

        private async void LoadSvgAsync()
        {
            CanvasDevice device = CanvasDevice.GetSharedDevice();
            using (var stream = GetEmbeddedSvgStream("file-edit-outline.svg"))
            {
                Edit = await CanvasSvgDocument.LoadAsync(device, stream.AsRandomAccessStream());
            }
            using (var stream = GetEmbeddedSvgStream("resize-bottom-right.svg"))
            {
                ResizeBottomRight = await CanvasSvgDocument.LoadAsync(device, stream.AsRandomAccessStream());
            }
            using (var stream = GetEmbeddedSvgStream("drag-horizontal-variant.svg"))
            {
                ResizeVert = await CanvasSvgDocument.LoadAsync(device, stream.AsRandomAccessStream());
            }
            using (var stream = GetEmbeddedSvgStream("drag-vertical-variant.svg"))
            {
                ResizeHorz = await CanvasSvgDocument.LoadAsync(device, stream.AsRandomAccessStream());
            }
        }
    }
}
