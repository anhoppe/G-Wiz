using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwiz.Core.Serializer
{
    /// <summary>
    /// Represents the content of a grid cell. 
    /// Used to deserialize the content information from the yaml file
    /// </summary>
    public class Content
    {
        public int Col { get; set; } 

        public int Row { get; set; }

        public string Text { get; set; } = string.Empty;
    }
}
