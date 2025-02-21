using Gwiz.Core.Contract;
using System.Collections.Generic;

namespace Gwiz.Core.Contract
{
    public class Graph : IGraph
    {
        public List<Node> Nodes { get; set; } = new();

        public List<Template> Templates { get; set; } = new ();

        public Node AddNode(string templateName)
        {
            var template = Templates.Find(t => t.Name == templateName);

            if (template == null)
            {
                throw new KeyNotFoundException($"Template {templateName} not found");
            }

            var node = new Node();

            node.Template = template;
            node.Grid = new Grid(template.Grid);
            node.Grid.ParentNode = node;

            Nodes.Add(node);

            return node;
        }
    }
}
