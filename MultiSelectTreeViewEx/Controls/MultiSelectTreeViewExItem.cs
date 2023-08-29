// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Defines the MultiSelectTreeViewExItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace System.Windows.Controls
{
    #region

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Controls.Automation.Peers;

    #endregion

    /// <summary>
    /// An item of the MultiSelectTreeViewEx.
    /// </summary>
    public partial class MultiSelectTreeViewExItem : HeaderedItemsControl
    {

        #region Constants and Fields

        internal double itemTopInTreeSystem; // for virtualization purposes
        internal int hierachyLevel;// for virtualization purposes

        #endregion

        #region Dependency properties

        public static DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(MultiSelectTreeViewExItem), new FrameworkPropertyMetadata(true));
        public static DependencyProperty IsEditableProperty = DependencyProperty.Register("IsEditable", typeof(bool), typeof(MultiSelectTreeViewExItem), new FrameworkPropertyMetadata(true, null));
        public static new DependencyProperty IsVisibleProperty = DependencyProperty.Register("IsVisible", typeof(bool), typeof(MultiSelectTreeViewExItem), new FrameworkPropertyMetadata(true));
        public static DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(MultiSelectTreeViewExItem), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedChanged)));
        public static DependencyProperty IsEditingProperty = DependencyProperty.Register("IsEditing", typeof(bool), typeof(MultiSelectTreeViewExItem), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsEditingChanged)));
        public static DependencyProperty ContentTemplateEditProperty = DependencyProperty.Register("ContentTemplateEdit", typeof(DataTemplate), typeof(MultiSelectTreeViewExItem));
        public static DependencyProperty DisplayNameProperty = DependencyProperty.Register("DisplayName", typeof(string), typeof(MultiSelectTreeViewExItem));
        public static DependencyProperty HoverHighlightingProperty = DependencyProperty.Register("HoverHighlighting", typeof(bool), typeof(MultiSelectTreeViewExItem), new FrameworkPropertyMetadata(false));
        public static DependencyProperty ItemIndentProperty = DependencyProperty.Register("ItemIndent", typeof(int), typeof(MultiSelectTreeViewExItem), new FrameworkPropertyMetadata(13));
        public static DependencyProperty IsKeyboardModeProperty = DependencyProperty.Register("IsKeyboardMode", typeof(bool), typeof(MultiSelectTreeViewExItem), new FrameworkPropertyMetadata(false));
        public static DependencyProperty RemarksProperty = DependencyProperty.Register("Remarks", typeof(string), typeof(MultiSelectTreeViewExItem));
        public static DependencyProperty RemarksTemplateProperty = DependencyProperty.Register("RemarksTemplate", typeof(DataTemplate), typeof(MultiSelectTreeViewExItem));
        public static DependencyProperty IsCurrentDropTargetProperty = DependencyProperty.Register("IsCurrentDropTarget", typeof(bool), typeof(MultiSelectTreeViewExItem), new FrameworkPropertyMetadata(false, null));
        public static DependencyProperty TemplateEditProperty = DependencyProperty.Register("TemplateEdit", typeof(DataTemplate), typeof(MultiSelectTreeViewExItem), new FrameworkPropertyMetadata(null, null));
        public static DependencyProperty TemplateSelectorEditProperty = DependencyProperty.Register("TemplateSelectorEdit", typeof(DataTemplateSelector), typeof(MultiSelectTreeViewExItem), new FrameworkPropertyMetadata(null, null));
        public static readonly DependencyProperty IndentationProperty = DependencyProperty.Register("Indentation", typeof(double), typeof(MultiSelectTreeViewExItem), new PropertyMetadata(10.0));

        #endregion

        // Using a DependencyProperty as the backing store for Offset.  This enables animation, styling, binding, etc...
        #region Constructors and Destructors

        static MultiSelectTreeViewExItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiSelectTreeViewExItem), new FrameworkPropertyMetadata(typeof(MultiSelectTreeViewExItem)));

            FrameworkElementFactory vPanel = new FrameworkElementFactory(typeof(VirtualizingTreePanel));
            vPanel.SetValue(VirtualizingTreePanel.IsItemsHostProperty, true);
            ItemsPanelTemplate vPanelTemplate = new ItemsPanelTemplate();
            vPanelTemplate.VisualTree = vPanel;
            ItemsPanelProperty.OverrideMetadata(typeof(MultiSelectTreeViewExItem), new FrameworkPropertyMetadata(vPanelTemplate));

            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(MultiSelectTreeViewExItem), new FrameworkPropertyMetadata(KeyboardNavigationMode.Continue));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(MultiSelectTreeViewExItem), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));
            IsTabStopProperty.OverrideMetadata(typeof(MultiSelectTreeViewExItem), new FrameworkPropertyMetadata(false));
        }

        public MultiSelectTreeViewExItem()
        {
        }

        #endregion

        #region Properties

        public DataTemplate ContentTemplateEdit
        {
            get 
            { 
                return (DataTemplate)GetValue(ContentTemplateEditProperty); 
            }
            set 
            { 
                SetValue(ContentTemplateEditProperty, value); 
            }
        }

        public bool IsExpanded
        {
            get
            {
                return (bool)GetValue(IsExpandedProperty);
            }

            set
            {
                SetValue(IsExpandedProperty, value);
            }
        }

        public bool IsEditable
        {
            get
            {
                return (bool)GetValue(IsEditableProperty);
            }

            set
            {
                SetValue(IsEditableProperty, value);
            }
        }

        public new bool IsVisible
        {
            get 
            { 
                return (bool)GetValue(IsVisibleProperty); 
            }
            set 
            { 
                SetValue(IsVisibleProperty, value); 
            }
        }

        public bool IsEditing
        {
            get { return (bool)GetValue(IsEditingProperty); }
            set { SetValue(IsEditingProperty, value); }
        }

        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }

            set
            {
                // Debug.WriteLine("IsSelected of " + DataContext + " is " + value + " from " + ParentItemsControl.GetHashCode());
                SetValue(IsSelectedProperty, value);
            }
        }

        public string DisplayName
        {
            get 
            { 
                return (string)GetValue(DisplayNameProperty);
            }
            set 
            { 
                SetValue(DisplayNameProperty, value);
            }
        }

        public bool HoverHighlighting
        {
            get 
            { 
                return (bool)GetValue(HoverHighlightingProperty);
            }
            set 
            { 
                SetValue(HoverHighlightingProperty, value); 
            }
        }

        public int ItemIndent
        {
            get 
            { 
                return (int)GetValue(ItemIndentProperty); 
            }
            set 
            { 
                SetValue(ItemIndentProperty, value); 
            }
        }

        public bool IsKeyboardMode
        {
            get 
            { 
                return (bool)GetValue(IsKeyboardModeProperty); 
            }
            set 
            {
                SetValue(IsKeyboardModeProperty, value); 
            }
        }

        public string Remarks
        {
            get 
            { 
                return (string)GetValue(RemarksProperty); 
            }
            set 
            { 
                SetValue(RemarksProperty, value); 
            }
        }

        public DataTemplate RemarksTemplate
        {
            get 
            { 
                return (DataTemplate)GetValue(RemarksTemplateProperty);
            }
            set 
            { 
                SetValue(RemarksTemplateProperty, value);
            }
        }

        public bool IsCurrentDropTarget
        {
            get
            {
                return (bool)GetValue(IsCurrentDropTargetProperty);
            }

            set
            {
                SetValue(IsCurrentDropTargetProperty, value);
            }
        }

        public DataTemplate TemplateEdit
        {
            get
            {
                return (DataTemplate)GetValue(TemplateEditProperty);
            }

            set
            {
                SetValue(TemplateEditProperty, value);
            }
        }

        public DataTemplateSelector TemplateSelectorEdit
        {
            get 
            { 
                return (DataTemplateSelector)GetValue(TemplateSelectorEditProperty);
            }
            set 
            {
                SetValue(TemplateSelectorEditProperty, value);
            }
        }

        public double Indentation
        {
            get
            {
                return (double)GetValue(IndentationProperty);
            }
            set
            {
                SetValue(IndentationProperty, value);
            }
        }

        public new FrameworkElement ContextMenu
        {
            get
            {
                return (FrameworkElement)GetValue(ContextMenuProperty);
            }
            set
            {
                SetValue(ContextMenuProperty, value);
            }
        }

        public MultiSelectTreeViewExItem ParentTreeViewItem 
        { 
            get 
            { 
                return ItemsControl.ItemsControlFromItemContainer(this) as MultiSelectTreeViewExItem; 
            } 
        }

        //public MultiSelectTreeViewEx ParentTreeView { get; set; }

        [DependsOn("Indentation")]
        public double Offset
        {
            get
            {
                if (ParentTreeViewItem == null) return 0;
                return ParentTreeViewItem.Offset + Indentation;
            }
        }

        private bool IsExpandableOnInput
        {
            get
            {
                return IsEnabled;
            }
        }

        #endregion

        #region Non-public properties

        private MultiSelectTreeViewEx lastParentTreeView;

        internal MultiSelectTreeViewEx ParentTreeView
        {
            get
            {
                for (ItemsControl itemsControl = ParentItemsControl;
                    itemsControl != null;
                    itemsControl = ItemsControlFromItemContainer(itemsControl))
                {
                    MultiSelectTreeViewEx treeView = itemsControl as MultiSelectTreeViewEx;
                    if (treeView != null)
                    {
                        return lastParentTreeView = treeView;
                    }
                }
                return null;
            }
        }

        private static bool IsControlKeyDown
        {
            get
            {
                return (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
            }
        }

        private static bool IsShiftKeyDown
        {
            get
            {
                return (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
            }
        }

        private bool CanExpand
        {
            get
            {
                return HasItems;
            }
        }

        private bool CanExpandOnInput
        {
            get
            {
                return CanExpand && IsEnabled;
            }
        }

        private ItemsControl ParentItemsControl
        {
            get
            {
                return ItemsControlFromItemContainer(this);
            }
        }


        #endregion Non-public properties

        #region Public Methods and Operators

        public override string ToString()
        {
            if (DataContext != null)
            {
                return string.Format("{0} ({1})", DataContext, base.ToString());
            }

            return base.ToString();
        }

        protected static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // The item has been selected through its IsSelected property. Update the SelectedItems
            // list accordingly (this is the authoritative collection). No PreviewSelectionChanged
            // event is fired - the item is already selected.
            MultiSelectTreeViewExItem item = d as MultiSelectTreeViewExItem;
            if (item != null)
            {
                if ((bool)e.NewValue)
                {
                    if (!item.ParentTreeView.SelectedItems.Contains(item.DataContext))
                    {
                        item.ParentTreeView.SelectedItems.Add(item.DataContext);
                    }
                }
                else
                {
                    item.ParentTreeView.SelectedItems.Remove(item.DataContext);
                }
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ParentTreeView == null) return;

            //System.Diagnostics.Debug.WriteLine("P(" + ParentTreeView.Name + "): " + e.Property + " " + e.NewValue);
            if (e.Property.Name == "IsEditing")
            {
                if ((bool)e.NewValue == false)
                {
                    StopEditing();
                }
            }

            if (e.Property.Name == "IsExpanded")
            {
                // Bring newly expanded child nodes into view if they'd be outside of the current view
                if ((bool)e.NewValue == true)
                {
                    if (VisualChildrenCount > 0)
                    {
                        ((FrameworkElement)GetVisualChild(VisualChildrenCount - 1)).BringIntoView();
                    }
                }
                // Deselect children of collapsed item
                // (If one resists, don't collapse)
                if ((bool)e.NewValue == false)
                {
                    if (!ParentTreeView.DeselectRecursive(this, false))
                    {
                        IsExpanded = true;
                    }
                }
            }

            if (e.Property.Name == "IsVisible")
            {
                // Deselect invisible item and its children
                // (If one resists, don't hide)
                if ((bool)e.NewValue == false)
                {
                    if (!ParentTreeView.DeselectRecursive(this, true))
                    {
                        IsVisible = true;
                    }
                }
            }

            //if (ParentTreeView != null && ParentTreeView.Selection != null && e.Property.Name == "IsSelected")
            //{
            //    if (ParentTreeView.SelectedItems.Contains(DataContext) != IsSelected)
            //    {
            //        ParentTreeView.Selection.SelectFromProperty(this, IsSelected);
            //    }
            //}

            base.OnPropertyChanged(e);
        }

        /// <summary>
        ///     Create or identify the element used to display the given item.
        /// </summary>
        /// <returns>The container.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MultiSelectTreeViewExItem();
        }

        /// <summary>
        ///     Returns true if the item is or should be its own container.
        /// </summary>
        /// <param name="item">The item to test.
        /// <returns>true if its type matches the container type.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is MultiSelectTreeViewExItem;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new MultiSelectTreeViewExItemAutomationPeer(this);
            //return new TreeViewExItemAutomationPeer(this, ParentTreeView.automationPeer);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            if (ParentTreeView != null && ParentTreeView.SelectedItems.Contains(DataContext))
            {
                IsSelected = true;
            }
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            FrameworkElement itemContent = (FrameworkElement)this.Template.FindName("headerBorder", this);
            if (!itemContent.IsMouseOver)
            {
                // A (probably disabled) child item was really clicked, do nothing here
                return;
            }

            if (IsKeyboardFocused && e.ChangedButton == MouseButton.Left) IsExpanded = !IsExpanded;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!e.Handled)
            {
                Key key = e.Key;
                switch (key)
                {
                    case Key.Left:
                        if (IsExpanded)
                        {
                            IsExpanded = false;
                        }
                        else
                        {
                            ParentTreeView.Selection.SelectParentFromKey();
                        }
                        e.Handled = true;
                        break;
                    case Key.Right:
                        if (CanExpand)
                        {
                            if (!IsExpanded)
                            {
                                IsExpanded = true;
                            }
                            else
                            {
                                ParentTreeView.Selection.SelectNextFromKey();
                            }
                        }
                        e.Handled = true;
                        break;
                    case Key.Up:
                        ParentTreeView.Selection.SelectPreviousFromKey();
                        e.Handled = true;
                        break;
                    case Key.Down:
                        ParentTreeView.Selection.SelectNextFromKey();
                        e.Handled = true;
                        break;
                    case Key.Home:
                        ParentTreeView.Selection.SelectFirstFromKey();
                        e.Handled = true;
                        break;
                    case Key.End:
                        ParentTreeView.Selection.SelectLastFromKey();
                        e.Handled = true;
                        break;
                    case Key.PageUp:
                        ParentTreeView.Selection.SelectPageUpFromKey();
                        e.Handled = true;
                        break;
                    case Key.PageDown:
                        ParentTreeView.Selection.SelectPageDownFromKey();
                        e.Handled = true;
                        break;
                    case Key.A:
                        if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                        {
                            ParentTreeView.Selection.SelectAllFromKey();
                            e.Handled = true;
                        }
                        break;
                    case Key.Add:
                        if (CanExpandOnInput && !IsExpanded)
                        {
                            IsExpanded = true;
                        }
                        e.Handled = true;
                        break;
                    case Key.Subtract:
                        if (CanExpandOnInput && IsExpanded)
                        {
                            IsExpanded = false;
                        }

                        e.Handled = true;
                        break;
                    case Key.F2:
                        if (ParentTreeView.AllowEditItems && ContentTemplateEdit != null && IsFocused && IsEditable)
                        {
                            IsEditing = true;
                            e.Handled = true;
                        }
                        break;
                    case Key.Escape:
                        StopEditing();
                        e.Handled = true;
                        break;
                    case Key.Return:
                        FocusHelper.Focus(this, true);
                        IsEditing = false;
                        e.Handled = true;
                        break;
                    case Key.Space:
                        ParentTreeView.Selection.SelectCurrentBySpace();
                        e.Handled = true;
                        break;

                }
            }
        }

        private void StopEditing()
        {
            FocusHelper.Focus(this, true);
            IsEditing = false;
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            // Do not call the base method because it would bring all of its children into view on
            // selecting which is not the desired behaviour.
            //base.OnGotFocus(e);
            ParentTreeView.LastFocusedItem = this;
            //System.Diagnostics.Debug.WriteLine("MultiSelectTreeViewItem.OnGotFocus(), DisplayName = " + DisplayName);
            //System.Diagnostics.Debug.WriteLine(Environment.StackTrace);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            IsEditing = false;
            //System.Diagnostics.Debug.WriteLine("MultiSelectTreeViewItem.OnLostFocus(), DisplayName = " + DisplayName);
            //System.Diagnostics.Debug.WriteLine(Environment.StackTrace);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("MultiSelectTreeViewItem.OnMouseDown(Item = " + this.DisplayName + ", Button = " + e.ChangedButton + ")");
            base.OnMouseDown(e);

            FrameworkElement itemContent = (FrameworkElement)this.Template.FindName("headerBorder", this);
            if (!itemContent.IsMouseOver)
            {
                // A (probably disabled) child item was really clicked, do nothing here
                return;
            }

            if (e.ChangedButton == MouseButton.Left)
            {
                ParentTreeView.Selection.Select(this);
                e.Handled = true;
            }
            if (e.ChangedButton == MouseButton.Right)
            {
                if (!IsSelected)
                {
                    ParentTreeView.Selection.Select(this);
                }
                e.Handled = true;
            }
        }

        /// <summary>
        ///     This method is invoked when the Items property changes.
        /// </summary>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            MultiSelectTreeViewEx parentTV;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    // Remove all items from the SelectedItems list that have been removed from the
                    // Items list
                    parentTV = ParentTreeView;
                    if (parentTV == null)
                        parentTV = lastParentTreeView;
                    if (parentTV != null)
                    {
                        foreach (var item in e.OldItems)
                        {
                            parentTV.SelectedItems.Remove(item);
                            var multiselection = parentTV.Selection as SelectionMultiple;
                            if (multiselection != null)
                            {
                                multiselection.InvalidateLastShiftRoot(item);
                            }
                            // Don't preview and ask, it is already gone so it must be removed from
                            // the SelectedItems list
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    // Remove all items from the SelectedItems list that are no longer in the Items
                    // list
                    parentTV = ParentTreeView;
                    if (parentTV == null)
                        parentTV = lastParentTreeView;
                    if (parentTV != null)
                    {
                        var selection = new object[parentTV.SelectedItems.Count];
                        parentTV.SelectedItems.CopyTo(selection, 0);
            HashSet<object> dataItems = new HashSet<object>(TreeViewElementFinder.FindAll(parentTV, false).Cast<object>());

            //HashSet<object> dataItems = new HashSet<object>(parentTV.GetAllDataItems().Cast<object>());
                        foreach (var item in selection)
                        {
                            if (!dataItems.Contains(item))
                            {
                                parentTV.SelectedItems.Remove(item);
                                // Don't preview and ask, it is already gone so it must be removed
                                // from the SelectedItems list
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Move:
                    break;

                default:
                    throw new NotSupportedException();
            }

            base.OnItemsChanged(e);
        }

        /// <summary>
        /// Send down the IsVirtualizing property if it's set on this element.
        /// </summary>
        /// <param name="element">
        /// <param name="item">
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
        }

        // Synchronizes the value of the child's IsVirtualizing property with that of the parent's
        internal static void IsVirtualizingPropagationHelper(DependencyObject parent, DependencyObject element)
        {
            SynchronizeValue(VirtualizingStackPanel.IsVirtualizingProperty, parent, element);
            SynchronizeValue(VirtualizingStackPanel.VirtualizationModeProperty, parent, element);
            MultiSelectTreeViewExItem tveItem = element as MultiSelectTreeViewExItem;
        }

        private static void SynchronizeValue(DependencyProperty dp, DependencyObject parent, DependencyObject child)
        {
            object value = parent.GetValue(dp);
            child.SetValue(dp, value);
        }

        private static void OnIsEditingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MultiSelectTreeViewEx || d is MultiSelectTreeViewExItem) return;

            throw new InvalidOperationException("IsEditing can only be set internally");
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (ParentTreeView != null && ParentTreeView.SelectedItems != null && ParentTreeView.SelectedItems.Contains(DataContext))
            {
                IsSelected = true;
            }
        }

        private bool StartEditing()
        {
            if ((TemplateEdit != null || TemplateSelectorEdit != null) && IsFocused && IsEditable)
            {
                ParentTreeView.IsEditingManager.StartEditing(this);
                return true;
            }

            return false;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (!e.Handled)
            {
                Key key = e.Key;
                switch (key)
                {
                    case Key.Left:
                    case Key.Right:
                    case Key.Up:
                    case Key.Down:
                    case Key.Add:
                    case Key.Subtract:
                    case Key.Space:
                        IEnumerable<MultiSelectTreeViewExItem> items = TreeViewElementFinder.FindAll(ParentTreeView, false);
                        MultiSelectTreeViewExItem focusedItem = items.FirstOrDefault(x => x.IsFocused);

                        if (focusedItem != null)
                        {
                            focusedItem.BringIntoView(new Rect(1, 1, 1, 1));
                        }

                        break;
                }
            }
        }

        #endregion

        #region Internal methods

        internal void InvokeMouseDown()
        {
            var e = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Right);
            e.RoutedEvent = Mouse.MouseDownEvent;
            this.OnMouseDown(e);
        }

        #endregion Internal methods
    }
}
