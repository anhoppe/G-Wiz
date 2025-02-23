using Gwiz.Core.Contract;

namespace Gwiz.Core
{
    internal interface IUpdateableGrid : IGrid
    {
        /// <summary>
        /// Updates the field rectangles of the grid according 
        /// to the size and position of the parent node
        /// </summary>
        /// <param name="parentNode">Reference to the parent node</param>
        void UpdateFieldRects(INode parentNode);
    }
}
