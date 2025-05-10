using System.Drawing;

namespace Gwiz.Core.Contract
{
    public interface INode : ISelectable
    {
        Alignment Alignment { get; }

        Color BackgroundColor { get; }

        IList<IButton> Buttons { get; set; }

        IGrid Grid { get; }

        int Height { get; set; }

        string Id { get; }

        Color LineColor { get; }

        Resize Resize { get; }

        Shape Shape { get; }

        List<IEdgeTemplate> SourceEdgeTemplates { get; }

        List<IEdgeTemplate> TargetEdgeTemplates { get; }

        int Width { get; set; }

        int X { get; set; }

        int Y { get; set; }

        IButton GetButtonById(string id);
    }
}
