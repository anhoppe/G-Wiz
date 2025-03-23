using Gwiz.Core.Contract;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using System.Drawing;

namespace Gwiz.Core
{
    internal class Edge : IEdge
    {
        private IUpdatableNode _from = new Node();
        
        private IUpdatableNode _to = new Node();

        public Ending Ending { get; internal set; } = Ending.None;

        public INode From => FromInternal;

        public INode To => ToInternal;

        internal string FromId { get; set; } = string.Empty;

        internal IUpdatableNode FromInternal
        {
            get => _from;

            set
            {
                _from = value;
                _from.NodeChanged += (sender, args) =>
                {
                    UpdateEdge();
                };
            }
        }

        public string FromLabel { get; set; } = string.Empty;

        public Point FromPosition { get; set; }

        public float LabelOffsetPerCent { get; set; }

        public Style Style { get; internal set; } = Style.None;

        internal string ToId { get; set; } = string.Empty;

        internal IUpdatableNode ToInternal 
        { 
            get => _to;

            set
            {
                _to = value;
                _to.NodeChanged += (sender, args) =>
                {
                    UpdateEdge();
                };
            }
        }

        public string ToLabel { get; set; } = string.Empty;

        public Point ToPosition { get; set; }

        private void UpdateEdge()
        {
            var centerFromNode = new Point2D(From.X + From.Width / 2, From.Y + From.Height / 2);
            var centerToNode = new Point2D(To.X + To.Width / 2, To.Y + To.Height / 2);

            if (centerFromNode != centerToNode)
            {
                var centerLine = new LineSegment2D(centerFromNode, centerToNode);

                FromPosition = CalculateIntersection(From, centerLine);
                ToPosition = CalculateIntersection(To, centerLine);
            }
        }

        private static Point CalculateIntersection(INode node, LineSegment2D centerLine)
        {
            var topLeft = new Point2D(node.X, node.Y);
            var topRight = new Point2D(node.X + node.Width, node.Y);
            var bottomLeft = new Point2D(node.X, node.Y + node.Height);
            var bottomRight = new Point2D(node.X + node.Width, node.Y + node.Height);

            Point2D intersection = new Point2D();

            var angleTolerance = Angle.FromDegrees(0.0001);

            if (centerLine.TryIntersect(new LineSegment2D(topLeft, topRight), out intersection, angleTolerance))
            {
                return new Point((int)intersection.X, (int)intersection.Y);
            }
            
            if (centerLine.TryIntersect(new LineSegment2D(topRight, bottomRight), out intersection, angleTolerance))
            {
                return new Point((int)intersection.X, (int)intersection.Y);
            }
            
            if (centerLine.TryIntersect(new LineSegment2D(bottomRight, bottomLeft), out intersection, angleTolerance))
            {
                return new Point((int)intersection.X, (int)intersection.Y);
            }
            
            if (centerLine.TryIntersect(new LineSegment2D(bottomLeft, topLeft), out intersection, angleTolerance))
            {
                return new Point((int)intersection.X, (int)intersection.Y);
            }

            // Fallback is the center of the node (can happen on overlapping nodes)
            return new Point(node.X + node.Width / 2, node.Y + node.Height / 2);
        }
    }
}
