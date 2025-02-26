using Gwiz.Core.Contract;

namespace Gwiz.Core
{
    internal interface IUpdatableGrid : IGrid
    {
        /// <summary>
        /// Registers the grid for changes in the parent node
        /// </summary>
        /// <param name="parentNode">Reference to the parent node to register on</param>
        void RegisterParentNodeChanged(IUpdatableNode parentNode);
    }
}
