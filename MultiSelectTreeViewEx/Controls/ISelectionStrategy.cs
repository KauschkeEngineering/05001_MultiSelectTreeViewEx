// *********************************************************************************************************
// *
// *  Author:       Maximilian Burkhardt
// *  Disclaimer:   Copyright (c) 2024 - Kauschke Engineering Systems GmbH
// * 				        All rights reserved - Unauthorized deobfuscation and making copies of this file,
// * 				        via any medium is strictly prohibited.
// *
// *********************************************************************************************************

using System.Collections;
namespace System.Windows.Controls
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    internal interface ISelectionStrategy : IDisposable
    {
        event EventHandler<PreviewSelectionChangedEventArgs> PreviewSelectionChanged;

        void ApplyTemplate();
        bool SelectCore(MultiSelectTreeViewExItem owner);
        bool Deselect(MultiSelectTreeViewExItem item, bool bringIntoView = false);
        bool SelectPreviousFromKey();
        //void SelectPreviousFromKeyEx();
        bool SelectNextFromKey();
        void SelectNextFromKeyEx();
        void SelectFromUiAutomation(MultiSelectTreeViewExItem item);
        bool SelectCurrentBySpace();
        void SelectFromProperty(MultiSelectTreeViewExItem item, bool isSelected);
        void SelectFirst();
        void SelectLast();
        bool SelectFirstFromKey();
        bool SelectLastFromKey();
        bool SelectPageUpFromKey();
        bool SelectPageDownFromKey();
        bool SelectAllFromKey();
        bool SelectParentFromKey();
        void ClearObsoleteItems(IEnumerable items);
        bool Select(MultiSelectTreeViewExItem treeViewItem);
    }
}
