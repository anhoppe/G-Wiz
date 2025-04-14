using Gwiz.Core.Contract;

namespace Gwiz.Core
{
    internal class EdgeTemplate : IEdgeTemplate
    {
        public Ending Beginning { get; set; } = Ending.None;
        
        public Ending Ending { get; set; } = Ending.None;

        public string Icon { get; set; } = string.Empty;

        public string Source { get; set; } = string.Empty;

        public string Target { get; set; } = string.Empty;
        
        public Style Style { get; set; } = Style.None;

        public string Text { get; set; } = string.Empty;

    }
}
