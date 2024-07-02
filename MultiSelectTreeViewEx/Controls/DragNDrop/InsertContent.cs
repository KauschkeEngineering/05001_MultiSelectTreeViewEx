// *********************************************************************************************************
// *
// *  Author:       Maximilian Burkhardt
// *  Disclaimer:   Copyright (c) 2023 - Kauschke Engineering Systems GmbH
// * 				        All rights reserved - Unauthorized deobfuscation and making copies of this file,
// * 				        via any medium is strictly prohibited.
// *
// *********************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace System.Windows.Controls
{
    class InsertContent
    {
        public bool Before { get; set; }

        public Point Position { get; set; }

        public Brush InsertionMarkerBrush { get; set; }

        public MultiSelectTreeViewExItem Item { get; set; }
    }
}
