using Gwiz.Core.Contract;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Gwiz.Core
{
    public class Graph : IGraph
    {
        public List<INode> Nodes { get; set; } = new();

        public List<ITemplate> Templates { get; set; } = new ();

        public INode AddNode(string templateName)
        {
            var template = Templates.Find(t => t.Name == templateName);

            if (template == null)
            {
                throw new KeyNotFoundException($"Template {templateName} not found");
            }

            var node = new Node();
            node.AssignTemplate(template);

            node.UpdateableGrid = new Grid(template.Grid);
            Nodes.Add(node);

            return node;
        }
    }
}
