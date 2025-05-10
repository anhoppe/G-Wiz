using System.Drawing;

namespace Gwiz.Core.Contract
{
    public interface ITemplate
    {
        Alignment Alignment { get; }

        Color BackgroundColor { get; }

        IList<IButton> Buttons { get; }

        IGrid Grid { get; }

        Color LineColor { get; }

        string Name { get; }

        Resize Resize { get; }

        Shape Shape { get; }
    }
}