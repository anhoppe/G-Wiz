using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
