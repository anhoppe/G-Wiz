using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwiz.Core.Contract
{
    public interface ITemplate
    {
        Color BackgroundColor { get; }
     
        IGrid Grid { get; }

        Color LineColor { get; }

        string Name { get; }

        Resize Resize { get; }
    }
}
