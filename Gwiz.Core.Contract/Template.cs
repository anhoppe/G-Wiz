﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwiz.Core.Contract
{
    public class Template
    {
        public Color BackgroundColor { get; set; }

        public Color LineColor { get; set; }

        public string Name { get; set; } = string.Empty;

        public Resize Resize { get; set; } = Resize.None;
    }
}
