using Gwiz.Core.Contract;
using Gwiz.Core.Serializer;
using System.Drawing;

namespace Gwiz.Core
{
    internal class Template : ITemplate
    {
        public Template() 
        {
            Grid.Rows.Add("1");
            Grid.Cols.Add("1");
        }

        public Alignment Alignment { get; set; } = Alignment.CenterCenter;

        public Color BackgroundColor { get; set; }

        public IGrid Grid { get; set; } = new Grid();

        public Color LineColor { get; set; }

        public string Name { get; set; } = string.Empty;

        public Resize Resize { get; set; } = Resize.None;

        internal string AlignmentStr { get; set; } = string.Empty;

        internal string ResizeStr { get; set; } = string.Empty;

        internal void ResolveEnums()
        {
            ResolveAlignment();
            ResolveResize();
        }

        private void ResolveAlignment()
        {
            switch (AlignmentStr.ToLower())
            {
                case "topleft":
                    Alignment = Alignment.TopLeft;
                    break;
                case "topcenter":
                    Alignment = Alignment.TopCenter;
                    break;
                case "topright":
                    Alignment = Alignment.TopRight;
                    break;
                case "centerleft":
                    Alignment = Alignment.CenterLeft;
                    break;
                case "centercenter":
                    Alignment = Alignment.CenterCenter;
                    break;
                case "centerright":
                    Alignment = Alignment.CenterRight;
                    break;
                case "bottomleft":
                    Alignment = Alignment.BottomLeft;
                    break;
                case "bottomcenter":
                    Alignment = Alignment.BottomCenter;
                    break;
                case "bottomright":
                    Alignment = Alignment.BottomRight;
                    break;
            }
        }

        private void ResolveResize()
        {
            switch (ResizeStr.ToLower())
            {
                case "":
                    Resize = Resize.None;
                    break;
                case "horzvert":
                    Resize = Resize.HorzVert;
                    break;
                case "both":
                    Resize = Resize.Both;
                    break;
                case "horzvertboth":
                    Resize = Resize.HorzVertBoth;
                    break;
                default:
                    throw new UnknownTemplateParameterValue("Resize", ResizeStr);
            }
        }
    }
}
