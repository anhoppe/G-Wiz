using Gwiz.Core.Contract;
using System;

namespace Gwiz.Core
{
    internal interface IUpdatableNode : INode
    {
        event EventHandler<IUpdatableNode> NodeChanged;
    }
}