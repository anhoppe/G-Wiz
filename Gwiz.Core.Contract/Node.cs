using Gwiz.Core.Contract;
using System.Drawing;

namespace Gwiz.Core.Contract
{
    public class Node
    {
        public int Height { get; set; }

        public Template Template { get; set; } = new();

        public int Width { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public List<int> GetColLinePositions() => ToFieldLinePositions(Template.Grid.Cols, Width);

        public List<int> GetRowLinePositions() => ToFieldLinePositions(Template.Grid.Rows, Height);

        private List<int> ToFieldLinePositions(IList<string> fields, int space)
        {
            if (fields.Count == 0)
            {
                return new();
            }
            var fieldsAsInt = fields.Select(p => int.TryParse(p, out var field) ? field : 0);
            var fieldSum = fieldsAsInt.Sum();
            return fieldsAsInt.SkipLast(1).Select(field =>
            {
                double perCent = 100.0 / fieldSum * field;
                return (int)(space / 100.0 * perCent);
            }).ToList();
        }
    }
}


// G/100 = P/p
// p = 100 / G * P
// P = G / 100 * p