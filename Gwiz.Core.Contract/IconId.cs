
namespace Gwiz.Core.Contract
{
    public enum IconId
    {
        None,
        AlertCircle,
        ArrowRight,
    }

    public static class IconIdExtensions
    {
        public static IconId ToIconId(this string iconName) => iconName.ToLower() switch
        {
            "none" => IconId.None,
            "alertcircle" => IconId.AlertCircle,
            "arrowright" => IconId.ArrowRight,
            _ => throw new ArgumentOutOfRangeException(nameof(iconName), iconName, null)
        };
    }
}
