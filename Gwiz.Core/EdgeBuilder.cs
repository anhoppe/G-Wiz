using Gwiz.Core.Contract;
using System;
using System.Collections.Generic;

namespace Gwiz.Core
{
    internal class EdgeBuilder : IEdgeBuilder
    {
        private Edge _edge;

        private IList<IEdge> _edges;

        public EdgeBuilder(IUpdatableNode from, IUpdatableNode to, IList<IEdge> edges)
        {
            _edges = edges;

            _edge = new Edge
            {
                FromId = from.Id,
                FromInternal = from,
                ToId = to.Id,
                ToInternal = to,
            };
        }

        public void Build()
        {
            _edge.UpdateEdge();
            _edges.Add(_edge);
        }

        public IEdgeBuilder WithEnding(Ending ending)
        {
            _edge.Ending = ending;
            return this;
        }

        public IEdgeBuilder WithFromDockingPosition(Direction direction, int pos)
        {
            _edge.FromDocking = direction;
            _edge.FromDockingPosition = pos; 
            return this;
        }

        public IEdgeBuilder WithFromLabel(string fromLabel)
        {
            _edge.FromLabel = fromLabel;
            return this;
        }

        public IEdgeBuilder WithLabelOffsetPerCent(float labelOffsetPerCent)
        {
            _edge.LabelOffsetPerCent = labelOffsetPerCent;
            return this;
        }

        public IEdgeBuilder WithStyle(Style style)
        {
            _edge.Style = style;
            return this;
        }

        public IEdgeBuilder WithTemplate(IEdgeTemplate edgeTemplate)
        {
            _edge.Beginning = edgeTemplate.Beginning;
            _edge.Ending = edgeTemplate.Ending;
            _edge.Style = edgeTemplate.Style;
            _edge.Text = edgeTemplate.Text;

            return this;
        }

        public IEdgeBuilder WithText(string text)
        {
            _edge.Text = text;
            return this;
        }

        public IEdgeBuilder WithToDockingPosition(Direction direction, int pos)
        {
            _edge.ToDocking = direction;
            _edge.ToDockingPosition = pos;
            return this;
        }

        public IEdgeBuilder WithToLabel(string toLabel)
        {
            _edge.ToLabel = toLabel;
            return this;
        }
    }
}
