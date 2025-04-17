using SkiaSharp;
using SkiaSharp.Extended.Svg;
using System;
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

        public SKBitmap? AlphaA { get; private set; }
        
        public SKBitmap? AlphaB { get; private set; }
        
        public SKBitmap? AlphaC { get; private set; }
        
        public SKBitmap? AlphaD { get; private set; }
        
        public SKBitmap? AlphaE { get; private set; }
        
        public SKBitmap? AlphaF { get; private set; }
        
        public SKBitmap? AlphaG { get; private set; }
        
        public SKBitmap? AlphaH { get; private set; }
        
        public SKBitmap? AlphaI { get; private set; }
        
        public SKBitmap? AlphaJ { get; private set; }
        
        public SKBitmap? AlphaU { get; private set; }
        
        public SKBitmap? AlphaV { get; private set; }
        
        public SKBitmap? AlphaW { get; private set; }
        
        public SKBitmap? AlphaX { get; private set; }
        
        public SKBitmap? AlphaY { get; private set; }
        
        public SKBitmap? AlphaZ { get; private set; }

        public SKBitmap? Connection { get; private set; }

        public SKBitmap? Edit { get; private set; }
        
        public SKBitmap? ResizeBottomRight { get; private set; }

        public SKBitmap? ResizeHorz { get; private set; }

        public SKBitmap? ResizeVert { get; private set; }

        internal SKBitmap? GetAlpha(string icon)
        {
            if (icon.Length != 1)
            {
                throw new ArgumentException("Currently connection icon must be 1 letter");
            }

            switch (icon.ToUpper()[0])
            {
                case 'A':
                    return AlphaA;
                case 'B':
                    return AlphaB;
                case 'C':
                    return AlphaC;
                case 'D':
                    return AlphaD;
                case 'E':
                    return AlphaE;
                case 'F':
                    return AlphaF;
                case 'G':
                    return AlphaG;
                case 'H':
                    return AlphaH;
                case 'I':
                    return AlphaI;
                case 'J':
                    return AlphaJ;
                case 'U':
                    return AlphaU;
                case 'V':
                    return AlphaV;
                case 'W':
                    return AlphaW;
                case 'X':
                    return AlphaX;
                case 'Y':
                    return AlphaY;
                case 'Z':
                    return AlphaZ;
            }

            throw new ArgumentException($"Could not determine the alpha icon for {icon}");
        }

        private static Stream GetEmbeddedStream(string iconName)
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
            using (var stream = GetEmbeddedStream("alpha-a.png"))
            {
                AlphaA = SKBitmap.Decode(stream);
            }
            using (var stream = GetEmbeddedStream("alpha-b.png"))
            {
                AlphaB = SKBitmap.Decode(stream);
            }
            using (var stream = GetEmbeddedStream("alpha-c.png"))
            {
                AlphaC = SKBitmap.Decode(stream);
            }
            using (var stream = GetEmbeddedStream("alpha-d.png"))
            {
                AlphaD = SKBitmap.Decode(stream);
            }
            using (var stream = GetEmbeddedStream("alpha-e.png"))
            {
                AlphaE = SKBitmap.Decode(stream);
            }
            using (var stream = GetEmbeddedStream("alpha-f.png"))
            {
                AlphaF = SKBitmap.Decode(stream);
            }
            using (var stream = GetEmbeddedStream("alpha-g.png"))
            {
                AlphaG = SKBitmap.Decode(stream);
            }
            using (var stream = GetEmbeddedStream("alpha-h.png"))
            {
                AlphaH = SKBitmap.Decode(stream);
            }
            using (var stream = GetEmbeddedStream("alpha-i.png"))
            {
                AlphaI = SKBitmap.Decode(stream);
            }
            using (var stream = GetEmbeddedStream("alpha-j.png"))
            {
                AlphaJ = SKBitmap.Decode(stream);
            }
            using (var stream = GetEmbeddedStream("alpha-u.png"))
            {
                AlphaU = SKBitmap.Decode(stream);
            }
            using (var stream = GetEmbeddedStream("alpha-v.png"))
            {
                AlphaV = SKBitmap.Decode(stream);
            }
            using (var stream = GetEmbeddedStream("alpha-w.png"))
            {
                AlphaW = SKBitmap.Decode(stream);
            }
            using (var stream = GetEmbeddedStream("alpha-x.png"))
            {
                AlphaX = SKBitmap.Decode(stream);
            }
            using (var stream = GetEmbeddedStream("alpha-y.png"))
            {
                AlphaY = SKBitmap.Decode(stream);
            }
            using (var stream = GetEmbeddedStream("alpha-z.png"))
            {
                AlphaZ = SKBitmap.Decode(stream);
            }
            using (var stream = GetEmbeddedStream("transit-connection-variant.png"))
            {
                Connection = SKBitmap.Decode(stream);
            }
            using (var stream = GetEmbeddedStream("text-box-edit-outline.png"))
            {
                Edit = SKBitmap.Decode(stream);
            }
            using (var stream = GetEmbeddedStream("resize-bottom-right.png"))
            {
                ResizeBottomRight = SKBitmap.Decode(stream);
            }
            using (var stream = GetEmbeddedStream("drag-horizontal-variant.png"))
            {
                ResizeVert = SKBitmap.Decode(stream);
            }
            using (var stream = GetEmbeddedStream("drag-vertical-variant.png"))
            {
                ResizeHorz = SKBitmap.Decode(stream);
            }
        }
    }
}
