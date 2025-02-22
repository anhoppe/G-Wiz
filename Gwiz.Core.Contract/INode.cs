using System.Drawing;

namespace Gwiz.Core.Contract
{
    public interface INode
    {
        Color BackgroundColor { get; }

        IGrid Grid { get; }

        int Height { get; set; }

        Color LineColor { get; }

        Resize Resize { get; }

        int Width { get; set; }

        int X { get; set; }

        int Y { get; set; }
    }
}
