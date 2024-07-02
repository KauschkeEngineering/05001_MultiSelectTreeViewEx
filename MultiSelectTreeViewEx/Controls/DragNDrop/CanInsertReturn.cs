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

namespace System.Windows.Controls
{
    class CanInsertReturn
    {
        public CanInsertReturn(string format, int index, bool before)
        {
            Format = format;
            Index = index;
            Before = before;
        }

        public string Format { get; set; }

        public int Index { get; set; }
        
        public bool Before { get; set; }
    }
}
