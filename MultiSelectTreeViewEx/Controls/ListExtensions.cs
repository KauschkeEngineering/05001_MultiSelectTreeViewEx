// *********************************************************************************************************
// *
// *  Author:       Maximilian Burkhardt
// *  Disclaimer:   Copyright (c) 2023 - Kauschke Engineering Systems GmbH
// * 				        All rights reserved - Unauthorized deobfuscation and making copies of this file,
// * 				        via any medium is strictly prohibited.
// *
// *********************************************************************************************************

using System.Collections;

namespace System.Windows.Controls
{
    internal static class ListExtensions
    {
		internal static object Last(this IList list)
        {
            if (list.Count < 1)
            {
                return null;
            }
            return list[list.Count - 1];
        }

		internal static object First(this IList list)
        {
            if (list.Count < 1)
            {
                return null;
            }
            return list[0];
        }
    }
}
