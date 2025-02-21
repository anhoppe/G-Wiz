using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwiz.Core.Contract
{
    public interface IGraph
    {
        List<Node> Nodes { get; }

        Node AddNode(string templateName);
    }
}
