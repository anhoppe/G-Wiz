namespace Gwiz.Core.Contract
{
    public class ContextMenuItem
    {
        public Action Callback { get; set; } = () => { };

        public string Name { get; set; } = string.Empty;

        public IList<ContextMenuItem> SubMenuItems { get; set; } = new List<ContextMenuItem>();
    }
}
