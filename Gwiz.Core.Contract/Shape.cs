namespace Gwiz.Core.Contract
{
    public enum Shape
    {
        Rectangle,

        Ellipse
    }

    public static class ShapeExtensions
    {
        public static Shape ToShape(this string asString) => asString.ToLower() switch
        { 
            "rectangle" => Shape.Rectangle,
            "ellipse" => Shape.Ellipse,
            _ => throw new UnknownTemplateParameterValue($"Invalid shape string: {asString}")
        };
    }
}
