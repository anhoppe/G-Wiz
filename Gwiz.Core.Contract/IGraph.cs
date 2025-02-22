using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwiz.Core.Contract
{
    public interface IGraph
    {
        List<INode> Nodes { get; }

        List<ITemplate> Templates { get; }

        INode AddNode(string templateName);
    }
}
