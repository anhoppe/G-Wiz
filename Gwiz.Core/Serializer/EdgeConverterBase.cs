using Gwiz.Core.Contract;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwiz.Core.Serializer
{
    internal class EdgeConverterBase
    {
        protected Ending StringToEnding(string value)
        {
            if (value == "None")
            {
                return Ending.None;
            }
            if (value == "OpenArrow")
            {
                return Ending.OpenArrow;
            }
            if (value == "ClosedArrow")
            {
                return Ending.ClosedArrow;
            }
            if (value == "Rhombus")
            {
                return Ending.Rhombus;
            }

            throw new InvalidEnumArgumentException($"No such ending <{value}>");
        }

    }
}
