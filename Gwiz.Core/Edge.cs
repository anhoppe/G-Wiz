using Gwiz.Core;
using Gwiz.Core.Contract;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using System;
using System.Drawing;

namespace Gwiz.Core
{
    internal class Edge : IEdge
    {
        private const float Eps = 0.001f;

        private const int SelectionDistanceThreshold = 3;

        private IUpdatableNode _from = new Node();

        private IUpdatableNode _to = new Node();

        public event EventHandler<bool>? SelectedChanged;

        public Ending Beginning { get; internal set; } = Ending.None;

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

        public bool Highlight { get; set; }

        public float LabelOffsetPerCent { get; set; }

        public bool Select { get; set; }

        public Style Style { get; set; } = Style.None;

        public string Text { get; set; } = string.Empty;

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

        public bool IsOver(Point position)
        {
            var fromPos = new Point2D(FromPosition.X, FromPosition.Y);
            var toPos = new Point2D(ToPosition.X, ToPosition.Y);

            var vec = fromPos - toPos;
            
            if (vec.Length < Eps)
            {
                return false;
            }

            var point = new Point2D(position.X, position.Y);
            var line = new Line2D(fromPos, toPos);

            var closesPoint = line.ClosestPointTo(point, true);

            vec = closesPoint - point;
            return vec.Length < SelectionDistanceThreshold;
        }

        internal void UpdateEdge()
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
            if (node.Shape == Shape.Ellipse)
            {
                return CalculateIntersectionWithEllipse(node, centerLine);
            }

            return CalculateIntersectionWithRectEdges(node, centerLine);
        }

        private static Point CalculateIntersectionWithEllipse(INode node, LineSegment2D centerLine)
        {
            // Define ellipse by rectangle (x, y, width, height)
            double cx = node.X + node.Width / 2;
            double cy = node.Y + node.Height / 2;
            double a = node.Width / 2;
            double b = node.Height / 2;

            // Define line segment from (x1, y1) to (x2, y2)
            var p1 = centerLine.StartPoint;
            var p2 = centerLine.EndPoint;

            // Compute quadratic coefficients
            double dx = p2.X - p1.X, dy = p2.Y - p1.Y;
            double A = (dx * dx) / (a * a) + (dy * dy) / (b * b);
            double B = 2 * ((p1.X - cx) * dx / (a * a) + (p1.Y - cy) * dy / (b * b));
            double C = ((p1.X - cx) * (p1.X - cx)) / (a * a) + ((p1.Y - cy) * (p1.Y - cy)) / (b * b) - 1;

            // Solve quadratic equation
            var roots = FindRootsQuadratic(A, B, C);
            foreach (var t in roots)
            {
                if (t >= 0 && t <= 1) // Valid intersection within segment
                {
                    double x = p1.X + t * dx;
                    double y = p1.Y + t * dy;
            
                    return new Point((int)x, (int)y);
                }
            }

            return new Point(0, 0);
        }

        private static Point CalculateIntersectionWithRectEdges(INode node, LineSegment2D centerLine)
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

        static double[] FindRootsQuadratic(double a, double b, double c)
        {
            double discriminant = b * b - 4 * a * c;
            if (discriminant < 0) return Array.Empty<double>(); // No real roots
            if (discriminant == 0) return new[] { -b / (2 * a) }; // One root
            double sqrtD = Math.Sqrt(discriminant);
            return new[] { (-b - sqrtD) / (2 * a), (-b + sqrtD) / (2 * a) }; // Two roots
        }
    }
}