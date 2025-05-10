using Gwiz.Core.Contract;
using Gwiz.Core.Serializer;
using System;

namespace Gwiz.Core
{
    internal class Button : IButton
    {
        public Button() { }

        public Button(IButton button)
        {
            Alignment = button.Alignment;
            Icon = button.Icon;
            Id = button.Id;
        }

        public Button(ButtonDto buttonDto)
        {
            Alignment = buttonDto.Alignment.ToAlignment();
            Icon = buttonDto.Icon.ToIconId();
            Id = buttonDto.Id;
        }

        public event EventHandler? Clicked;

        public Alignment Alignment { get; private set; } = Alignment.CenterCenter;

        public IconId Icon { get; private set; } = IconId.None;

        public string Id { get; private set; } = string.Empty;

        public void Click()
        {
            throw new NotImplementedException();
        }
    }
}
