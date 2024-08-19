// *********************************************************************************************************
// *
// *  Author:       Maximilian Burkhardt
// *  Disclaimer:   Copyright (c) 2024 - Kauschke Engineering Systems GmbH
// * 				        All rights reserved - Unauthorized deobfuscation and making copies of this file,
// * 				        via any medium is strictly prohibited.
// *
// *********************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace System.Windows.Controls
{
	public class SelectionChangedCancelEventArgs : CancelEventArgs
	{
		public IEnumerable<object> ItemsToUnSelect { get; private set; }
		public IEnumerable<object> ItemsToSelect { get; private set; }

		public SelectionChangedCancelEventArgs(IEnumerable<object> itemsToSelect, IEnumerable<object> itemsToUnSelect)
		{
			ItemsToSelect = itemsToSelect;
            ItemsToUnSelect = itemsToUnSelect;
		}
	}
}
