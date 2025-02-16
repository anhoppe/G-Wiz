using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwiz.Core.Serializer
{
    internal class UnknownTemplateParameterValue : Exception
    {
        public UnknownTemplateParameterValue(string parameter, string value)
            : base($"The value {value} is not valid for the template parameter {parameter}")
        { }
    }
}
