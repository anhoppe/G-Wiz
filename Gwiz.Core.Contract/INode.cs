using System.Drawing;

namespace Gwiz.Core.Contract
{
    public interface INode
    {
        Alignment Alignment { get; }

        Color BackgroundColor { get; }

        IGrid Grid { get; }

        int Height { get; set; }

        string Id { get; }

        Color LineColor { get; }

        Resize Resize { get; }

        int Width { get; set; }

        int X { get; set; }

        int Y { get; set; }
    }
}