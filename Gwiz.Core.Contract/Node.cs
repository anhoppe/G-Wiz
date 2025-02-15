using Gwiz.Core.Contract;
using System.Drawing;

namespace Gwiz.Core.Contract
{
    public class Node
    {
        public int Height { get; set; }

        public Template Template { get; set; } = new();

        public int Width { get; set; }

        public int X { get; set; }

        public int Y { get; set; }
    }
}
