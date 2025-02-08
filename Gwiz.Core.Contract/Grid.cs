using System.Collections.Generic;

namespace Gwiz.Core.Contract
{
    public class Grid
    {
        private List<Col> _cols = new();
        private List<Row> _rows = new();

        public void AddRow(string space)
        {
            _rows.Add(new Row()
            {
                HorizontalSpace = space,
            });
        }

        public void AddCol(string space)
        {
            _cols.Add(new Col()
            {
                VerticalSpace = space,
            });
        }
    }

    class Row
    {
        public string HorizontalSpace { get; set; } = string.Empty;
    }

    class Col
    {
        public string VerticalSpace { get; set; } = string.Empty;
    }
}
