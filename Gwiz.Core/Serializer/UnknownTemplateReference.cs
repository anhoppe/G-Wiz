using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwiz.Core.Serializer
{
    internal class UnknownTemplateReference : Exception
    {
        public UnknownTemplateReference(string templateName) :
            base($"Node references unknown template {templateName}")
        {}
    }
}
