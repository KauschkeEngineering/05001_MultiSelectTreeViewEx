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
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary>
    /// Base for a class, which wants to be provided with different events.
    /// </summary>
    internal class InputSubscriberBase
    {
        protected internal bool IsLeftButtonDown { get; set; }

        internal virtual void Initialized()
        {
        }

        internal virtual void OnScrollChanged(ScrollChangedEventArgs e)
        {
        }

        internal virtual void OnMouseDown(MouseButtonEventArgs e)
        {
        }

        internal virtual void OnMouseUp(MouseButtonEventArgs e)
        {
        }

        internal virtual void OnMouseMove(MouseEventArgs e)
        {
        }

        internal Point GetMousePosition()
        {
            Point currentPoint = Mouse.GetPosition(TreeView);
            return currentPoint;
        }

        internal Point GetMousePositionRelativeToContent()
        {
            Point currentPoint = Mouse.GetPosition(TreeView);

            currentPoint = new Point(currentPoint.X + TreeView.ScrollViewer.ContentHorizontalOffset, currentPoint.Y + TreeView.ScrollViewer.ContentVerticalOffset);
            return currentPoint;
        }

        protected bool IsMouseOverAdorner(Point positionRelativeToTree)
        {
            HitTestResult hitTestResult = VisualTreeHelper.HitTest(TreeView, positionRelativeToTree);
            if (hitTestResult == null || hitTestResult.VisualHit == null) return false;

            Adorner item = null;
            DependencyObject currentObject = hitTestResult.VisualHit;

            while (item == null && currentObject != null)
            {
                item = currentObject as Adorner;
                if (item != null) return true;

                currentObject = VisualTreeHelper.GetParent(currentObject);
            }

            return false;
        }

        protected MultiSelectTreeViewExItem GetTreeViewItemUnderMouse(Point positionRelativeToTree)
        {
            HitTestResult hitTestResult = VisualTreeHelper.HitTest(TreeView, positionRelativeToTree);
            if (hitTestResult == null || hitTestResult.VisualHit == null) return null;

            FrameworkElement child = hitTestResult.VisualHit as FrameworkElement;
            
            do
            {
                if (child is MultiSelectTreeViewExItem) return GetVisible(child as MultiSelectTreeViewExItem);
                if (child.DataContext is InsertContent) return  GetVisible((child.DataContext as InsertContent).Item);
                if (child is MultiSelectTreeViewEx) return null;
                child = VisualTreeHelper.GetParent(child) as FrameworkElement;
            } while (child != null);

            return null;
        }

        private MultiSelectTreeViewExItem GetVisible(MultiSelectTreeViewExItem treeViewExItem)
        {
            if (treeViewExItem.IsVisible) return treeViewExItem;
            return null;
        }

        internal protected MultiSelectTreeViewEx TreeView { get; internal set; }
    }
}
