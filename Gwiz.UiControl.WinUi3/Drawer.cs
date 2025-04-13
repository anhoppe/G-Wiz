
namespace Gwiz.UiControl.WinUi3
{
    internal class Drawer : IDrawer
    {
        public int IconSize => 30;

        public IDraw Draw { get; set; } = new Draw();
    }
}
