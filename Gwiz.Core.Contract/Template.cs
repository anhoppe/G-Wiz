using System.Drawing;

namespace Gwiz.Core.Contract
{
    public class Template
    {
        public Template() 
        {
            Grid.Rows.Add("1");
            Grid.Cols.Add("1");
        }

        public Color BackgroundColor { get; set; }

        public Grid Grid { get; set; } = new();

        public Color LineColor { get; set; }

        public string Name { get; set; } = string.Empty;

        public Resize Resize { get; set; } = Resize.None;
    }
}
