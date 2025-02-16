using System.Collections.Generic;

namespace Gwiz.Core.Contract
{
    public class Grid
    {
        public List<string> Cols { get; set; } = new();

        public List<string> Rows { get; set; } = new();
    }
}
