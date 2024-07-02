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
using System.Windows.Controls;

namespace System.Windows.Controls
{
    internal static class TreeViewElementFinder
    {
        internal static MultiSelectTreeViewExItem FindNext(MultiSelectTreeViewExItem treeViewItem, bool visibleOnly)
        {
            // find first child
            if (treeViewItem.IsExpanded || !visibleOnly)
            {
                MultiSelectTreeViewExItem item = GetFirstVirtualizedItem(treeViewItem);
                if (item != null)
                {
                    if (item.IsEnabled)
                    {
                        if (visibleOnly && !item.IsVisible)
                            return FindNext(item, visibleOnly);
                        else
                            return item;
                    }
                    else
                    {
                        return FindNext(item, visibleOnly);
                    }
                }
            }

            // find next sibling
            MultiSelectTreeViewExItem sibling = FindNextSiblingRecursive(treeViewItem) as MultiSelectTreeViewExItem;
            if (sibling == null)
                return null;
            if (!visibleOnly || sibling.IsVisible)
                return sibling;
            return null;
        }

        private static MultiSelectTreeViewExItem GetFirstVirtualizedItem(MultiSelectTreeViewExItem treeViewItem)
        {
            for (int i = 0; i < treeViewItem.Items.Count; i++)
            {
                MultiSelectTreeViewExItem item = treeViewItem.ItemContainerGenerator.ContainerFromIndex(i) as MultiSelectTreeViewExItem;
                if (item != null) return item;

            }

            return null;
        }

        internal static ItemsControl FindNextSibling(ItemsControl itemsControl)
        {
            ItemsControl parentIc = ItemsControl.ItemsControlFromItemContainer(itemsControl);
            if (parentIc == null) return null;
            int index = parentIc.ItemContainerGenerator.IndexFromContainer(itemsControl);
            return parentIc.ItemContainerGenerator.ContainerFromIndex(index + 1) as ItemsControl; // returns null if index to large or nothing found
        }

        internal static ItemsControl FindNextSiblingRecursive(ItemsControl itemsControl)
        {
            ItemsControl parentIc = ItemsControl.ItemsControlFromItemContainer(itemsControl);
            if (parentIc == null) return null;
            int index = parentIc.ItemContainerGenerator.IndexFromContainer(itemsControl);
            if (index < parentIc.Items.Count - 1)
            {
                return parentIc.ItemContainerGenerator.ContainerFromIndex(index + 1) as ItemsControl; // returns null if index to large or nothing found
            }

            return FindNextSiblingRecursive(parentIc);
        }

        /// <summary>
        /// Returns the first item. If tree is virtualized, it is the first realized item.
        /// </summary>
        /// <param name="treeView">The tree.</param>
        /// <returns>Returns a MultiSelectTreeViewExItem.</returns>
        internal static MultiSelectTreeViewExItem FindFirst(MultiSelectTreeViewEx treeView, bool visibleOnly)
        {
            for (int i = 0; i < treeView.Items.Count; i++)
            {
                var item = treeView.ItemContainerGenerator.ContainerFromIndex(i) as MultiSelectTreeViewExItem;
                if (item == null) continue;
                if (!visibleOnly || item.IsVisible) return item;
            }

            return null;
        }

        /// <summary>
        /// Returns the last item. If tree is virtualized, it is the last realized item.
        /// </summary>
        /// <param name="treeView">The tree.</param>
        /// <returns>Returns a MultiSelectTreeViewExItem.</returns>
        internal static MultiSelectTreeViewExItem FindLast(MultiSelectTreeViewEx treeView, bool visibleOnly)
        {
            for (int i = treeView.Items.Count - 1; i >= 0; i--)
            {
                var item = treeView.ItemContainerGenerator.ContainerFromIndex(i) as MultiSelectTreeViewExItem;
                if (item == null) continue;
                if (!visibleOnly || item.IsVisible) return item;
            }

            return null;
        }

        /// <summary>
        /// Returns all items in tree recursively. If virtualization is enabled, only realized items are returned.
        /// </summary>
        /// <param name="treeView">The tree.</param>
        /// <param name="visibleOnly">True if only visible items should be returned.</param>
        /// <returns>Returns an enumerable of items.</returns>
        internal static IEnumerable<MultiSelectTreeViewExItem> FindAll(MultiSelectTreeViewEx treeView, bool visibleOnly)
        {
            MultiSelectTreeViewExItem currentItem = FindFirst(treeView, visibleOnly);
            while (currentItem != null)
            {
                if (!visibleOnly || currentItem.IsVisible) yield return currentItem;
                currentItem = FindNext(currentItem, visibleOnly);
            }
        }
    }
}
