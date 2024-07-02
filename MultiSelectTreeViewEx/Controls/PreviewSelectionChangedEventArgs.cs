// *********************************************************************************************************
// *
// *  Author:       Maximilian Burkhardt
// *  Disclaimer:   Copyright (c) 2023 - Kauschke Engineering Systems GmbH
// * 				        All rights reserved - Unauthorized deobfuscation and making copies of this file,
// * 				        via any medium is strictly prohibited.
// *
// *********************************************************************************************************

namespace System.Windows.Controls
{
    public class PreviewSelectionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets a value indicating whether the item was selected or deselected.
        /// </summary>
        public bool Selecting { get; private set; }
        /// <summary>
        /// Gets the item that is being selected or deselected.
        /// </summary>
        public object Item { get; private set; }
        /// <summary>
        /// Gets or sets a value indicating whether the selection change of this item shall be
        /// cancelled.
        /// </summary>
        public bool CancelThis { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the selection change of this item and all other
        /// affected items shall be cancelled.
        /// </summary>
        public bool CancelAll { get; set; }

        /// <summary>
        /// Gets a value indicating whether any of the Cancel flags is set.
        /// </summary>
        public bool CancelAny { get { return CancelThis || CancelAll; } }

        public PreviewSelectionChangedEventArgs(bool selecting, object item)
        {
#if DEBUG
            // Make sure we don't confuse MultiSelectTreeViewItems and their DataContexts while development
            if (item is MultiSelectTreeViewExItem)
                throw new ArgumentException("The selection preview event was passed a MultiSelectTreeViewItem instance. Only their DataContext instances must be used here!");
#endif

            Selecting = selecting;
            Item = item;
        }
    }
}
