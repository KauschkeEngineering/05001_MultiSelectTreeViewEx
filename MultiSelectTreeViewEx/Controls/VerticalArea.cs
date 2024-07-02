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
  public class VerticalArea
  {
    public double Top { get; set; }
    public double Bottom { get; set; }

    public bool IsWithin(VerticalArea area)
    {
      return
      (area.Top >= Top && area.Top <= Bottom)
      ||
      (area.Bottom >= Top && area.Bottom <= Bottom)
      ||
      (area.Top <= Top && area.Bottom >= Bottom);
    }
  }
}
