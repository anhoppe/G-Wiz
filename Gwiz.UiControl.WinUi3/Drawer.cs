
namespace Gwiz.UiControl.WinUi3
{
    internal class Drawer : IDrawer
    {
        public IDraw Draw { get; set; } = new Draw();
    }
}
