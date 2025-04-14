using System.Drawing;

namespace Gwiz.Core.Contract
{
    public enum Ending
    {
        None,
        OpenArrow,
        ClosedArrow,
        Rhombus,
    }

    public enum Style
    {
        None,
        Dashed,
        Dotted
    }

    /// <summary>
    /// Represents an edge in a graph
    /// </summary>
    public interface IEdge
    {
        /// <summary>
        /// The marker at the source of the edge
        /// </summary>
        Ending Beginning { get; }

        /// <summary>
        /// Marker at the end of the edge
        /// </summary>
        Ending Ending { get; }

        /// <summary>
        /// The node the edge is coming from
        /// </summary>
        INode From { get; }

        /// <summary>
        /// Label that is shown on the from position
        /// </summary>
        string FromLabel { get; }

        /// <summary>
        /// The position of the edge coming from
        /// </summary>
        Point FromPosition { get; }

        /// <summary>
        /// Offset of the from / to labels in per cent of the
        /// absolute edge length. 0.5 means both labels are in the middle.
        /// </summary>
        float LabelOffsetPerCent { get; }

        /// <summary>
        /// The style of the edge indicates how it is drawn
        /// (e.g. stippled, dotted)
        /// </summary>
        Style Style { get; }

        /// <summary>
        /// Text of the edge
        /// The text is rendered at the center of the edge
        /// </summary>
        string Text { get; }

        /// <summary>
        /// The node the edge is going to
        /// </summary>
        INode To { get; }

        /// <summary>
        /// Label shown at the to position
        /// </summary>
        string ToLabel { get; }

        /// <summary>
        /// The position of the edge going to
        /// </summary>
        Point ToPosition { get; }
    }
}
