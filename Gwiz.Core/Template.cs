using Gwiz.Core.Contract;
using Gwiz.Core.Serializer;
using System.Collections.Generic;
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

        public IList<IButton> Buttons { get; set; } = new List<IButton>();

        public IGrid Grid { get; set; } = new Grid();

        public Color LineColor { get; set; }

        public string Name { get; set; } = string.Empty;

        public Resize Resize { get; set; } = Resize.None;

        public Shape Shape { get; set; } = Shape.Rectangle;

        internal IList<ButtonDto> ButtonDto { get; set; } = new List<ButtonDto>();
    }
}
