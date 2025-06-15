
namespace Gwiz.Core.Contract
{
    public enum IconId
    {
        None,
        AlertCircle,
        ArrowLeft,
        ArrowRight,
    }

    public static class IconIdExtensions
    {
        public static IconId ToIconId(this string iconName) => iconName.ToLower() switch
        {
            "none" => IconId.None,
            "alertcircle" => IconId.AlertCircle,
            "arrowleft" => IconId.ArrowLeft,
            "arrowright" => IconId.ArrowRight,
            _ => throw new ArgumentOutOfRangeException(nameof(iconName), iconName, null)
        };
    }
}
