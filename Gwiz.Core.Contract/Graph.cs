using Gwiz.Core.Contract;
using System.Collections.Generic;

namespace Gwiz.Core.Contract
{
    public class Graph
    {
        public List<Node> Nodes { get; set; } = new();

        public List<Template> Templates { get; set; } = new ();
    }
}
