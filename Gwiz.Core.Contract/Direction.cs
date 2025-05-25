
namespace Gwiz.Core.Contract
{
    public enum Direction
    {
        None,
        Left,
        Right,
        Top,
        Bottom
    }

    public static class DirectionExtensions
    {
        public static string FromDirection(this Direction value) => value switch
        {
            Direction.None => "None",
            Direction.Left => "Left",
            Direction.Right => "Right",
            Direction.Top => "Top",
            Direction.Bottom => "Bottom",

            _ => throw new ArgumentException($"No such direction <{value}>"),
        };

        public static Direction ToDirection(this string asString) => asString.ToLower() switch
        {
            "none" => Direction.None,
            "left" => Direction.Left,
            "right" => Direction.Right,
            "top" => Direction.Top,
            "bottom" => Direction.Bottom,

            _ => throw new ArgumentException($"Invalid direction string: {asString}")
        };
    }
}
