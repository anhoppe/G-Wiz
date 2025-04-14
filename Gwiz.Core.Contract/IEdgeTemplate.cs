namespace Gwiz.Core.Contract
{
    public interface IEdgeTemplate
    {
        Ending Beginning { get; }

        Ending Ending { get; }

        /// <summary>
        /// Icon that shall be used to visualize the source / target
        /// connection possibility
        /// </summary>
        string Icon { get; }

        Style Style { get; }

        string Text { get; }
    }
}
