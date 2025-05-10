namespace Gwiz.Core.Contract
{
    /// <summary>
    /// Represents a clickable button in a graph
    /// </summary>
    public interface IButton
    {
        Alignment Alignment { get; }
     
        IconId Icon { get; }

        string Id { get; }

        /// <summary>
        /// Emits a click event on the button
        /// </summary>
        void Click();

        /// <summary>
        /// Event is raised when the button was clicked
        /// </summary>
        event EventHandler Clicked;
    }
}
