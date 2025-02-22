using Gwiz.Core.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
