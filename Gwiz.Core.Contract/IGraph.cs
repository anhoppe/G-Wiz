namespace Gwiz.Core.Contract
{
    public interface IGraph
    {
        List<IEdge> Edges { get; }

        List<INode> Nodes { get; }

        List<ITemplate> Templates { get; }

        INode AddNode(string templateName);

        void AddEdge(INode from, INode to);

        void AddEdge(INode from, INode to, Ending ending, Style style);

        void AddEdge(INode from, INode to, string fromLabel, string toLabel, float labelOffsetPerCent);

        void Update();
    }
}
