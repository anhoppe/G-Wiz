using System.Drawing;

namespace awiz.Graph.Core
{

    public class Node
    {
        public int Height { get; set; }

        public int Width { get; set; }

        public int X { get; set; }
        
        public int Y { get; set; }

        public Node(/*NodeTemplate nodeTemplate*/)
        {
            Width = 100;
            Height = 100;
            //_nodeTemplate = nodeTemplate;
        }
    }
}
