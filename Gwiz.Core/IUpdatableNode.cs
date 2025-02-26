using Gwiz.Core.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwiz.Core
{
    internal interface IUpdatableNode : INode
    {
        event EventHandler<IUpdatableNode> NodeChanged;
    }
}
