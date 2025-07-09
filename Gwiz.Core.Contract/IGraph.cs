namespace Gwiz.Core.Contract
{
    /// <summary>
    /// Represents the entire graph with instances and tempates
    /// </summary>
    public interface IGraph
    {
        public event EventHandler<IList<ContextMenuItem>>? ContextMenuShown;

        public event EventHandler<INode>? NodeRemoved;

        List<IEdge> Edges { get; }

        List<INode> Nodes { get; }

        List<ITemplate> Templates { get; }

        IEdgeBuilder AddEdge(INode from, INode to);

        INodeBuilder AddNode(string templateName);

        void Remove(IEdge edge);
        
        void Remove(INode node);

        void SetTextSizeCalculator(Func<string, (int, int)> textSizeCalculator);

        void ShowContextMenu(IList<ContextMenuItem> contextMenuItems);

        void Update();
    }
}
