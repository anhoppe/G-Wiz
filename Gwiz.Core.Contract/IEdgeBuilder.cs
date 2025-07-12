namespace Gwiz.Core.Contract
{
    /// <summary>
    /// Builder for edge. After configuring the edge using the With.. methods
    /// the edge creation must be terminated by calling 'Build'
    /// </summary>
    public interface IEdgeBuilder
    {
        /// <summary>
        /// Terminates the edge creation by adding the configured edge to the graph
        /// </summary>
        void Build();

        /// <summary>
        /// Defines the ending icon of the edge
        /// </summary>
        IEdgeBuilder WithEnding(Ending ending);

        /// <summary>
        /// Defines the from docking position on the node
        /// </summary>
        /// <param name="direction">The direction of the edge the docking position is at</param>
        /// <param name="pos">The actual docking position from top/left on the defined edge of the node</param>
        /// <returns>Edge builder</returns>
        IEdgeBuilder WithFromDockingPosition(Direction direction, int pos);

        /// <summary>
        /// Label that is shown on the from position
        /// </summary>
        /// <param name="fromLabel">Text of the label</param>
        /// <returns></returns>
        IEdgeBuilder WithFromLabel(string fromLabel);

        IEdgeBuilder WithLabelOffsetPerCent(float labelOffsetPerCent);

        IEdgeBuilder WithStyle(Style style);

        IEdgeBuilder WithTemplate(IEdgeTemplate edgeTemplate);

        IEdgeBuilder WithText(string text);

        /// <summary>
        /// Allows to define a distance of the edge's center to the text position.
        /// The distance is always orthogonal to the edge.
        /// </summary>
        /// <param name="deltaX">Horizontal distance from the edge center to the text position</param>
        /// <param name="deltaY">Vertical distance from the edge center to the text position</param>
        /// <returns>Edge builder object</returns>
        IEdgeBuilder WithTextDistance(int deltaX, int deltaY);

        /// <summary>
        /// Defines the to docking position on the node
        /// </summary>
        /// <param name="direction">The direction of the edge </param>
        /// <param name="pos"></param>
        /// <returns></returns>
        IEdgeBuilder WithToDockingPosition(Direction direction, int pos);

        IEdgeBuilder WithToLabel(string toLabel);
    }
}
