
namespace Gwiz.Core.Contract
{
    public interface INodeBuilder
    {
        INode Build();

        /// <summary>
        /// When set, the node will automatically adjust its width based on the text width.
        /// </summary>
        /// <returns></returns>
        INodeBuilder WithAutoWidth();

        INodeBuilder WithHeight(int height);

        INodeBuilder WithId(string id);

        INodeBuilder WithPos(int x, int y);

        INodeBuilder WithSize(int width, int height);

        INodeBuilder WithText(int row, int col, string text);

        INodeBuilder WithWidth(int width);

        INodeBuilder WithX(int x);
        
        INodeBuilder WithY(int y);
    }
}
