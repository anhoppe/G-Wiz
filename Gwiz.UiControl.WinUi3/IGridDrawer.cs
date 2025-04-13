using Gwiz.Core.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwiz.UiControl.WinUi3
{
    internal interface IGridDrawer : IDrawer
    {
        void DrawGrid(INode node);
    }
}
