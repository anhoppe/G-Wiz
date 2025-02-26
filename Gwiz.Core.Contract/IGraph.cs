namespace Gwiz.Core.Contract
{
    public interface IGraph
    {
        List<IEdge> Edges { get; }

        List<INode> Nodes { get; }

        List<ITemplate> Templates { get; }

        INode AddNode(string templateName);
    }
}
