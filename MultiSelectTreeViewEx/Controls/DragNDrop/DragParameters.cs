// *********************************************************************************************************
// *
// *  Author:       Maximilian Burkhardt
// *  Disclaimer:   Copyright (c) 2023 - Kauschke Engineering Systems GmbH
// * 				        All rights reserved - Unauthorized deobfuscation and making copies of this file,
// * 				        via any medium is strictly prohibited.
// *
// *********************************************************************************************************

using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace System.Windows.Controls
{
    public class CanDragParameters
    {
        public readonly IEnumerable<MultiSelectTreeViewExItem> Items;
        public readonly Point Position;
        public readonly MouseButton Button;

        internal CanDragParameters(IEnumerable<MultiSelectTreeViewExItem> draggableItems, Point dragPosition, MouseButton mouseButton)
        {
            this.Items = draggableItems;
            this.Position = dragPosition;
            this.Button = mouseButton;
        }
    }

    public class DragParameters
    {
        public readonly IDataObject Data = new DataObject();
        public readonly IEnumerable<MultiSelectTreeViewExItem> Items;
        public readonly Point Position;
        public readonly MouseButton Button;

        public DragDropEffects AllowedEffects = DragDropEffects.All;

        internal DragParameters(IEnumerable<MultiSelectTreeViewExItem> draggableItems, Point dragPosition, MouseButton mouseButton)
        {
            this.Items = draggableItems;
            this.Position = dragPosition;
            this.Button = mouseButton;
        }
    }
}
