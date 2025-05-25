using System.ComponentModel;
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

    public static class EndingExtensions
    {
        public static string FromEnding(this Ending value) => value switch
        {
            Ending.None => "None",
            Ending.OpenArrow => "OpenArrow",
            Ending.ClosedArrow => "ClosedArrow",
            Ending.Rhombus => "Rhombus",
            _ => throw new InvalidEnumArgumentException($"No such ending <{value}>"),
        };

        public static Ending ToEnding(this string asString) => asString.ToLower() switch
        {
            "none" => Ending.None,
            "openarrow" => Ending.OpenArrow,
            "closedarrow" => Ending.ClosedArrow,
            "rhombus" => Ending.Rhombus,
            _ => throw new ArgumentException($"Not such ending {asString}"),
        };
    }
    
    public enum Style
    {
        None,
        Dashed,
        Dotted
    }

    public static class StyleExtensions
    {
        public static string FromStyle(this Style value) => value switch
        {
            Style.None => "None",
            Style.Dashed => "Dashed",
            Style.Dotted => "Dotted",
            _ => throw new InvalidEnumArgumentException($"No such style <{value}>"),
        };

        public static Style ToStyle(this string asString) => asString.ToLower() switch
        {
            "none" => Style.None,
            "dashed" => Style.Dashed,
            "dotted" => Style.Dotted,
            _ => throw new ArgumentException($"No such style: {asString}"),
        };
    }

    /// <summary>
    /// Represents an edge in a graph
    /// </summary>
    public interface IEdge : ISelectable
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
