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
using System.Windows.Input;

namespace System.Windows.Controls
{
    class IsEditingManager:InputSubscriberBase
    {
        MultiSelectTreeViewEx treeview;
        MultiSelectTreeViewExItem editedItem;

        public IsEditingManager(MultiSelectTreeViewEx treeview)
        {
            this.treeview = treeview;
        }

        internal override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            StopEditing();
        }

        public void StartEditing(MultiSelectTreeViewExItem item)
        {
            StopEditing();
            editedItem = item;
            editedItem.IsEditing = true;
        }

        internal void StopEditing()
        {
            if (editedItem == null) return;

            Keyboard.Focus(editedItem);
            editedItem.IsEditing = false;
            FocusHelper.Focus(editedItem);
            editedItem = null;
        }
    }
}
