
namespace Gwiz.Core.Contract
{
    public enum Resize
    {
        None,

        // Controls for individually resizing horizontal and vertical 
        HorzVert,

        // Control for resizing horizontal and vertical at the same time
        Both,

        // Controls for all resizing actions
        HorzVertBoth
    }

    public static class ResizeExtensions
    {
        public static Resize ToResize(this string asString) => asString.ToLower() switch
        {
            "" => Resize.None,
            "horzvert" => Resize.HorzVert,
            "both" => Resize.Both,
            "horzvertboth" => Resize.HorzVertBoth,

            _ => throw new UnknownTemplateParameterValue($"Invalid resize string: {asString}")
        };
    }
}
