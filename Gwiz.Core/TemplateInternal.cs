using Gwiz.Core.Contract;
using Gwiz.Core.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwiz.Core
{
    internal class TemplateInternal : Template
    {
        public string ResizeName { get; set; } = string.Empty;

        internal void ResolveResize()
        {
            switch (ResizeName.ToLower())
            {
                case "":
                    Resize = Resize.None;
                    break;
                case "horzvert":
                    Resize = Resize.HorzVert;
                    break;
                case "both":
                    Resize = Resize.Both;
                    break;
                case "horzvertboth":
                    Resize = Resize.HorzVertBoth;
                    break;
                default:
                    throw new UnknownTemplateParameterValue("Resize", ResizeName);
            }
        }
    }
}
