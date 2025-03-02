using System.Drawing;

namespace Gwiz.Core.Contract
{
    public enum Ending
    {
        None,
        OpenArrow,
        ClosedArrow,
        Rhombus
    }

    /// <summary>
    /// Represents an edge in a graph
    /// </summary>
    public interface IEdge
    {
        /// <summary>
        /// Ending symbol of the edge
        /// </summary>
        Ending Ending { get; }

        /// <summary>
        /// The node the edge is coming from
        /// </summary>
        INode From { get; }

        /// <summary>
        /// The position of the edge coming from
        /// </summary>
        Point FromPosition { get; }

        /// <summary>
        /// The node the edge is going to
        /// </summary>
        INode To { get; }

        /// <summary>
        /// The position of the edge going to
        /// </summary>
        Point ToPosition { get; }
    }
}
