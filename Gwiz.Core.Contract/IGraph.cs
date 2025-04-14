namespace Gwiz.Core.Contract
{
    /// <summary>
    /// Represents the entire graph with instances and tempates
    /// </summary>
    public interface IGraph
    {
        List<IEdge> Edges { get; }

        List<INode> Nodes { get; }

        List<ITemplate> Templates { get; }

        INode AddNode(string templateName);
        
        void AddEdge(INode from, INode to);

        void AddEdge(INode from, INode to, IEdgeTemplate edgeTemplate);

        void AddEdge(INode from, INode to, Ending ending, Style style);

        void AddEdge(INode from, INode to, string fromLabel, string toLabel, float labelOffsetPerCent);

        void Update();
    }
}
