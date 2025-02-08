
using System.Collections.Generic;

class Row
{
    public string HorizontalSpace { get; set; } = string.Empty;
}

class Col
{
    public string VerticalSpace { get; set; } = string.Empty;
}

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

class EdgeTemplate
{
    //public Node Source { get; set; }
    //public Node Target { get; set; }

    //Bitmap SourceSymbol { get; set; }
    //Bitmap TargetSymbol { get; set; }
}

class Test
{
    //var grid = new Grid();

    //grid.AddRow();
    //grid.AddCol();

    //grid.SetCell(0, 0);
}
