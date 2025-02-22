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

        public Color BackgroundColor { get; set; }

        public IGrid Grid { get; set; } = new Grid();

        public Color LineColor { get; set; }

        public string Name { get; set; } = string.Empty;

        public Resize Resize { get; set; } = Resize.None;

        internal string ResizeName { get; set; } = string.Empty;
        
        internal void ResolveResize()
        {
            switch (ResizeName.ToLower())
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
                    throw new UnknownTemplateParameterValue("Resize", ResizeName);
            }
        }
    }
}
