// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Defines the MultiSelectTreeViewEx type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace System.Windows.Controls
{
  #region

  using System.Collections;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.ComponentModel;
  using System.Linq;
  using System.Windows;
  using System.Windows.Automation.Peers;
  using System.Windows.Input;
  using System.Windows.Media;
  using System.Windows.Controls.Automation.Peers;
  using System;

  #endregion

  public class MultiSelectTreeViewEx : HeaderedItemsControl
  {
    #region Constants and Fields

    public event EventHandler<PreviewSelectionChangedEventArgs> PreviewSelectionChanged;

    // TODO: Provide more details. Fire once for every single change and once for all groups of changes, with different flags
    public event EventHandler SelectionChanged;

    public static readonly DependencyProperty LastSelectedItemProperty;
    public static DependencyProperty DragCommandProperty = DependencyProperty.Register("DragCommand", typeof(ICommand), typeof(MultiSelectTreeViewEx));
    public static DependencyProperty DropCommandProperty = DependencyProperty.Register("DropCommand", typeof(ICommand), typeof(MultiSelectTreeViewEx));
    public static readonly DependencyProperty InsertTemplateProperty = DependencyProperty.Register("InsertTemplate", typeof(DataTemplate), typeof(MultiSelectTreeViewEx), new PropertyMetadata(null));
    public static DependencyProperty InsertionMarkerBrushProperty = DependencyProperty.Register("InsertionMarkerBrush", typeof(Brush), typeof(MultiSelectTreeViewEx), new FrameworkPropertyMetadata(Brushes.Black, null));
    public static readonly DependencyProperty IsDragNDropEnabledProperty = DependencyProperty.Register("IsDragNDropEnabled", typeof(bool), typeof(MultiSelectTreeViewEx), new PropertyMetadata(false));
    public static DependencyProperty BackgroundSelectionRectangleProperty = DependencyProperty.Register("BackgroundSelectionRectangle", typeof(Brush), typeof(MultiSelectTreeViewExItem), new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromArgb(0x60, 0x33, 0x99, 0xFF)), null));
    public static DependencyProperty BorderBrushSelectionRectangleProperty = DependencyProperty.Register("BorderBrushSelectionRectangle", typeof(Brush), typeof(MultiSelectTreeViewExItem), new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x33, 0x99, 0xFF)), null));
    public static DependencyProperty HoverHighlightingProperty = DependencyProperty.Register("HoverHighlighting", typeof(bool), typeof(MultiSelectTreeViewEx), new FrameworkPropertyMetadata(true, null));
    public static DependencyProperty VerticalRulersProperty = DependencyProperty.Register("VerticalRulers", typeof(bool), typeof(MultiSelectTreeViewEx), new FrameworkPropertyMetadata(false, null));
    public static DependencyProperty ItemIndentProperty = DependencyProperty.Register("ItemIndent", typeof(int), typeof(MultiSelectTreeViewEx), new FrameworkPropertyMetadata(13, null));
    public static DependencyProperty IsKeyboardModeProperty = DependencyProperty.Register("IsKeyboardMode", typeof(bool), typeof(MultiSelectTreeViewEx), new FrameworkPropertyMetadata(false, null));
    public static DependencyPropertyKey LastSelectedItemPropertyKey = DependencyProperty.RegisterReadOnly("LastSelectedItem", typeof(object), typeof(MultiSelectTreeViewEx), new FrameworkPropertyMetadata(null));
    //public static DependencyProperty LastSelectedItemProperty = DependencyProperty.Register("LastSelectedItem", typeof(object), typeof(MultiSelectTreeViewEx), new FrameworkPropertyMetadata(null, OnLastSelectedItemPropertyChanged));
    public static DependencyProperty SelectedItemsProperty = DependencyProperty.Register("SelectedItems", typeof(IList), typeof(MultiSelectTreeViewEx), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemsPropertyChanged));
    public static readonly DependencyProperty IsVirtualizingProperty = DependencyProperty.Register("IsVirtualizing", typeof(bool), typeof(MultiSelectTreeViewEx), new PropertyMetadata(false));
    public static DependencyProperty AllowEditItemsProperty = DependencyProperty.Register("AllowEditItems", typeof(bool), typeof(MultiSelectTreeViewEx), new FrameworkPropertyMetadata(false, null));

    // the space where items will be realized if virtualization is enabled. This is set by virtualizingtreepanel.
    internal VerticalArea realizationSpace = new VerticalArea();
    internal SizesCache cachedSizes = new SizesCache();

    internal MultiSelectTreeViewExAutomationPeer automationPeer;

    private DragNDropController dragNDropController;

    private InputEventRouter inputEventRouter;

    private bool isInitialized;

    private ScrollViewer scroller;

    #endregion

    #region Constructors and Destructors

    static MultiSelectTreeViewEx()
    {
      LastSelectedItemProperty = LastSelectedItemPropertyKey.DependencyProperty;
      DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiSelectTreeViewEx), new FrameworkPropertyMetadata(typeof(MultiSelectTreeViewEx)));

      KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(MultiSelectTreeViewEx), new FrameworkPropertyMetadata(KeyboardNavigationMode.Contained));
      KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(MultiSelectTreeViewEx), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));

      FrameworkElementFactory vPanel = new FrameworkElementFactory(typeof(VirtualizingTreePanel));
      vPanel.SetValue(VirtualizingTreePanel.IsItemsHostProperty, true);
      ItemsPanelTemplate vPanelTemplate = new ItemsPanelTemplate();
      vPanelTemplate.VisualTree = vPanel;
      ItemsPanelProperty.OverrideMetadata(typeof(MultiSelectTreeViewEx), new FrameworkPropertyMetadata(vPanelTemplate));
    }

    public MultiSelectTreeViewEx()
    {
      SelectedItems = new ObservableCollection<object>();
      Selection = new SelectionMultiple(this);
      Selection.PreviewSelectionChanged += (s, e) => { OnPreviewSelectionChanged(e); };

      var depPropDescriptorIsDragNDropEnabled = DependencyPropertyDescriptor.FromProperty(IsDragNDropEnabledProperty, typeof(MultiSelectTreeViewEx));
      depPropDescriptorIsDragNDropEnabled.AddValueChanged(this, OnIsDragNDropEnabledChanged);
    }

    #endregion

    #region Public Events

    public event EventHandler<SelectionChangedCancelEventArgs> OnSelecting;

    #endregion

    #region Properties

    public Brush BackgroundSelectionRectangle
    {
      get
      {
        return (Brush)GetValue(BackgroundSelectionRectangleProperty);
      }

      set
      {
        SetValue(BackgroundSelectionRectangleProperty, value);
      }
    }

    public Brush BorderBrushSelectionRectangle
    {
      get
      {
        return (Brush)GetValue(BorderBrushSelectionRectangleProperty);
      }

      set
      {
        SetValue(BorderBrushSelectionRectangleProperty, value);
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

    public bool VerticalRulers
    {
      get
      {
        return (bool)GetValue(VerticalRulersProperty);
      }
      set
      {
        SetValue(VerticalRulersProperty, value);
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

    [Browsable(false)]
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

    public bool AllowEditItems
    {
      get
      {
        return (bool)GetValue(AllowEditItemsProperty);
      }
      set
      {
        SetValue(AllowEditItemsProperty, value);
      }
    }

    /// <summary>
    ///   Gets the last selected item.
    /// </summary>
    public object LastSelectedItem
    {
      get
      {
        return GetValue(LastSelectedItemProperty);
      }

      private set
      {
        //SetValue(LastSelectedItemProperty, value);
        SetValue(LastSelectedItemPropertyKey, value);
      }
    }

    private MultiSelectTreeViewExItem lastFocusedItem;
    /// <summary>
    /// Gets the last focused item.
    /// </summary>
    internal MultiSelectTreeViewExItem LastFocusedItem
    {
      get
      {
        return lastFocusedItem;
      }
      set
      {
        // Only the last focused MultiSelectTreeViewItem may have IsTabStop = true
        // so that the keyboard focus only stops a single time for the MultiSelectTreeView control.
        if (lastFocusedItem != null)
        {
          lastFocusedItem.IsTabStop = false;
        }
        lastFocusedItem = value;
        if (lastFocusedItem != null)
        {
          lastFocusedItem.IsTabStop = true;
        }
        // The MultiSelectTreeView control only has the tab stop if none of its items has it.
        IsTabStop = lastFocusedItem == null;
      }
    }

    /// <summary>
    ///   Gets or sets a list of selected items and can be bound to another list. If the source list implements <see
    ///    cref="INotifyPropertyChanged" /> the changes are automatically taken over.
    /// </summary>
    public IList SelectedItems
    {
      get
      {
        return (IList)GetValue(SelectedItemsProperty);
      }

      set
      {
        SetValue(SelectedItemsProperty, value);
      }
    }

    internal ISelectionStrategy Selection { get; private set; }

    public bool IsVirtualizing
    {
      get
      {
        return (bool)GetValue(IsVirtualizingProperty);
      }
      set
      {
        SetValue(IsVirtualizingProperty, value);
      }
    }

    public Brush InsertionMarkerBrush
    {
      get
      {
        return (Brush)GetValue(InsertionMarkerBrushProperty);
      }

      set
      {
        SetValue(InsertionMarkerBrushProperty, value);
      }
    }

    public ICommand DragCommand
    {
      get { return (ICommand)GetValue(DragCommandProperty); }
      set { SetValue(DragCommandProperty, value); }
    }

    public ICommand DropCommand
    {
      get { return (ICommand)GetValue(DropCommandProperty); }
      set { SetValue(DropCommandProperty, value); }
    }

    public DataTemplate InsertTemplate
    {
      get
      {
        return (DataTemplate)GetValue(InsertTemplateProperty);
      }

      set
      {
        SetValue(InsertTemplateProperty, value);
      }
    }

    public bool IsDragNDropEnabled
    {
      get
      {
        return (bool)GetValue(IsDragNDropEnabledProperty);
      }

      set
      {
        SetValue(IsDragNDropEnabledProperty, value);
      }
    }

    internal IsEditingManager IsEditingManager { get; set; }

    internal DragNDropController DragNDropController
    {
      get
      {
        return dragNDropController;
      }
    }

    internal ScrollViewer ScrollViewer
    {
      get
      {
        if (scroller == null)
        {
          scroller = (ScrollViewer)Template.FindName("scroller", this);
        }

        return scroller;
      }
    }

    #endregion

    #region Public Methods and Operators

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      if (!isInitialized)
      {
        Loaded += OnLoaded;
        Unloaded += OnUnLoaded;
      }
    }

    public bool ClearSelection()
    {
      if (SelectedItems.Count > 0)
      {
        // Make a copy of the list and ignore changes to the selection while raising events
        foreach (var selItem in new ArrayList(SelectedItems))
        {
          var e = new PreviewSelectionChangedEventArgs(false, selItem);
          OnPreviewSelectionChanged(e);
          if (e.CancelAny)
          {
            return false;
          }
        }

        SelectedItems.Clear();
      }
      return true;
    }

    public void FocusItem(object item, bool bringIntoView = false)
    {
      MultiSelectTreeViewExItem node = GetTreeViewItemsFor(new List<object> { item }).FirstOrDefault();
      if (node != null)
      {
        FocusHelper.Focus(node, bringIntoView);
      }
    }

    public void BringItemIntoView(object item)
    {
      MultiSelectTreeViewExItem node = GetTreeViewItemsFor(new List<object> { item }).First();
      FrameworkElement itemContent = (FrameworkElement)node.Template.FindName("headerBorder", node);
      itemContent.BringIntoView();
    }

    public bool SelectNextItem()
    {
      return Selection.SelectNextFromKey();
    }

    public bool SelectPreviousItem()
    {
      return Selection.SelectPreviousFromKey();
    }

    public bool SelectFirstItem()
    {
      return Selection.SelectFirstFromKey();
    }

    public bool SelectLastItem()
    {
      return Selection.SelectLastFromKey();
    }

    public bool SelectAllItems()
    {
      return Selection.SelectAllFromKey();
    }

    public bool SelectParentItem()
    {
      return Selection.SelectParentFromKey();
    }

    #endregion

    #region Methods

    internal bool DeselectRecursive(MultiSelectTreeViewExItem item, bool includeSelf)
    {
      List<MultiSelectTreeViewExItem> selectedChildren = new List<MultiSelectTreeViewExItem>();
      if (includeSelf)
      {
        if (item.IsSelected)
        {
          var e = new PreviewSelectionChangedEventArgs(false, item.DataContext);
          OnPreviewSelectionChanged(e);
          if (e.CancelAny)
          {
            return false;
          }
          selectedChildren.Add(item);
        }
      }
      if (!CollectDeselectRecursive(item, selectedChildren))
      {
        return false;
      }
      foreach (var child in selectedChildren)
      {
        child.IsSelected = false;
      }
      return true;
    }

    private bool CollectDeselectRecursive(MultiSelectTreeViewExItem item, List<MultiSelectTreeViewExItem> selectedChildren)
    {
      foreach (var child in item.Items)
      {
        MultiSelectTreeViewExItem tvi = item.ItemContainerGenerator.ContainerFromItem(child) as MultiSelectTreeViewExItem;
        if (tvi != null)
        {
          if (tvi.IsSelected)
          {
            var e = new PreviewSelectionChangedEventArgs(false, child);
            OnPreviewSelectionChanged(e);
            if (e.CancelAny)
            {
              return false;
            }
            selectedChildren.Add(tvi);
          }
          if (!CollectDeselectRecursive(tvi, selectedChildren))
          {
            return false;
          }
        }
      }
      return true;
    }

    internal bool ClearSelectionByRectangle()
    {
      foreach (var item in new ArrayList(SelectedItems))
      {
        var e = new PreviewSelectionChangedEventArgs(false, item);
        OnPreviewSelectionChanged(e);
        if (e.CancelAny) return false;
      }

      SelectedItems.Clear();
      return true;
    }

    public void BringIntoView(object item)
    {
      UpdateLayout();
      MultiSelectTreeViewExItem tvei = GetTreeViewItemFor(item);
      tvei.BringIntoView();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      var autoScroller = new AutoScroller();
      dragNDropController = new DragNDropController(autoScroller);
      var selectionMultiple = new SelectionMultiple(this);
      Selection = selectionMultiple;
      Selection.ApplyTemplate();
      IsEditingManager = new IsEditingManager(this);

      inputEventRouter = new InputEventRouter(this);
      inputEventRouter.Add(IsEditingManager);
      inputEventRouter.Add(autoScroller);
      inputEventRouter.Add(dragNDropController);
      //inputEventRouter.Add(selectionMultiple.BorderSelectionLogic);
      inputEventRouter.Add(selectionMultiple);
      isInitialized = true;
    }

    private void OnUnLoaded(object sender, RoutedEventArgs e)
    {
      if (inputEventRouter != null)
      {
        inputEventRouter.Dispose();
        inputEventRouter = null;
      }

      if (dragNDropController != null)
      {
        dragNDropController.Dispose();
        dragNDropController = null;
      }

      Selection = null;
    }

    internal bool CheckSelectionAllowed(object item, bool isItemAdded)
    {
      if (isItemAdded)
      {
        return CheckSelectionAllowed(new List<object> { item }, new List<object>());
      }

      return CheckSelectionAllowed(new List<object>(), new List<object> { item });
    }

    internal bool CheckSelectionAllowed(IEnumerable<object> itemsToSelect, IEnumerable<object> itemsToUnselect)
    {
      if (OnSelecting != null)
      {
        var e = new SelectionChangedCancelEventArgs(itemsToSelect, itemsToUnselect);
        foreach (var method in OnSelecting.GetInvocationList())
        {
          method.Method.Invoke(method.Target, new object[] { this, e });

          // stop iteration if one subscriber wants to cancel
          if (e.Cancel)
          {
            return false;
          }
        }

        return true;
      }

      return true;
    }

    internal IEnumerable<MultiSelectTreeViewExItem> GetChildren(MultiSelectTreeViewExItem item)
    {
      if (item == null) yield break;
      for (int i = 0; i < item.Items.Count; i++)
      {
        MultiSelectTreeViewExItem child = item.ItemContainerGenerator.ContainerFromIndex(i) as MultiSelectTreeViewExItem;
        if (child != null) yield return child;
      }
    }

    /// <summary>
    /// Send down the IsVirtualizing property if it's set on this element.
    /// </summary>
    /// <param name="element">
    /// <param name="item">
    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
      base.PrepareContainerForItemOverride(element, item);
      MultiSelectTreeViewExItem.IsVirtualizingPropagationHelper(this, element);
    }

    internal MultiSelectTreeViewExItem GetNextItem(MultiSelectTreeViewExItem item, List<MultiSelectTreeViewExItem> items)
    {
      int indexOfCurrent = items.IndexOf(item);

      for (int i = indexOfCurrent + 1; i < items.Count; i++)
      {
        if (items[i].IsVisible)
        {
          return items[i];
        }
      }

      return null;
    }

    internal MultiSelectTreeViewExItem GetPreviousItem(MultiSelectTreeViewExItem item, List<MultiSelectTreeViewExItem> items)
    {
      int indexOfCurrent = items.IndexOf(item);
      for (int i = indexOfCurrent - 1; i >= 0; i--)
      {
        if (items[i].IsVisible)
        {
          return items[i];
        }
      }

      return null;
    }

    internal MultiSelectTreeViewExItem GetFirstItem(List<MultiSelectTreeViewExItem> items)
    {
      for (int i = 0; i < items.Count; i++)
      {
        if (items[i].IsVisible)
        {
          return items[i];
        }
      }
      return null;
    }

    internal MultiSelectTreeViewExItem GetLastItem(List<MultiSelectTreeViewExItem> items)
    {
      for (int i = items.Count - 1; i >= 0; i--)
      {
        if (items[i].IsVisible)
        {
          return items[i];
        }
      }
      return null;
    }

    protected override DependencyObject GetContainerForItemOverride()
    {
      return new MultiSelectTreeViewExItem();
    }

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
      return item is MultiSelectTreeViewExItem;
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
      if (automationPeer == null)
      {
        automationPeer = new MultiSelectTreeViewExAutomationPeer(this);
      }

      return automationPeer;
    }

    private static void OnSelectedItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      MultiSelectTreeViewEx treeView = (MultiSelectTreeViewEx)d;
      if (e.OldValue != null)
      {
        INotifyCollectionChanged collection = e.OldValue as INotifyCollectionChanged;
        if (collection != null)
        {
          collection.CollectionChanged -= treeView.OnSelectedItemsChanged;
        }
      }

      if (e.NewValue != null)
      {
        INotifyCollectionChanged collection = e.NewValue as INotifyCollectionChanged;
        if (collection != null)
        {
          collection.CollectionChanged += treeView.OnSelectedItemsChanged;
        }
      }
    }

    /// <summary>
    ///     This method is invoked when the Items property changes.
    /// </summary>
    protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
    {
      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Remove:
          if (Selection != null) // happens during unload
            Selection.ClearObsoleteItems(e.OldItems);
          break;

        case NotifyCollectionChangedAction.Reset:
          // If the items list has considerably changed, the selection is probably
          // useless anyway, clear it entirely.
          SelectedItems.Clear();
          break;

        case NotifyCollectionChangedAction.Replace:
          if (e.OldItems != null)
          {
            foreach (var item in e.OldItems)
            {
              SelectedItems.Remove(item);
              // Don't preview and ask, it is already gone so it must be removed from
              // the SelectedItems list
            }
          }
          break;
        case NotifyCollectionChangedAction.Add:
        case NotifyCollectionChangedAction.Move:
          break;

        default:
          throw new NotSupportedException();
      }
      base.OnItemsChanged(e);
    }

    //internal static IEnumerable<MultiSelectTreeViewExItem> RecursiveTreeViewItemEnumerable(ItemsControl parent, bool includeInvisible)
    //{
    //  return RecursiveTreeViewItemEnumerable(parent, includeInvisible, true);
    //}

    //internal static IEnumerable<MultiSelectTreeViewExItem> RecursiveTreeViewItemEnumerable(ItemsControl parent, bool includeInvisible, bool includeDisabled)
    //{
    //  foreach (var item in parent.Items)
    //  {
    //    MultiSelectTreeViewExItem tve = (MultiSelectTreeViewExItem)parent.ItemContainerGenerator.ContainerFromItem(item);
    //    if (tve == null)
    //    {
    //      // Container was not generated, therefore it is probably not visible, so we can ignore it.
    //      continue;
    //    }
    //    if (!includeInvisible && !tve.IsVisible)
    //    {
    //      continue;
    //    }
    //    if (!includeDisabled && !tve.IsEnabled)
    //    {
    //      continue;
    //    }

    //    yield return tve;
    //    if (includeInvisible || tve.IsExpanded)
    //    {
    //      foreach (var childItem in RecursiveTreeViewItemEnumerable(tve, includeInvisible, includeDisabled))
    //      {
    //        yield return childItem;
    //      }
    //    }
    //  }
    //}

    internal IEnumerable<MultiSelectTreeViewExItem> GetNodesToSelectBetween(MultiSelectTreeViewExItem firstNode, MultiSelectTreeViewExItem lastNode)
    {
      var allNodes = TreeViewElementFinder.FindAll(this, false).ToList();
      var firstIndex = allNodes.IndexOf(firstNode);
      var lastIndex = allNodes.IndexOf(lastNode);

      if (firstIndex >= allNodes.Count)
      {
        throw new InvalidOperationException(
           "First node index " + firstIndex + "greater or equal than count " + allNodes.Count + ".");
      }

      if (lastIndex >= allNodes.Count)
      {
        throw new InvalidOperationException(
           "Last node index " + lastIndex + " greater or equal than count " + allNodes.Count + ".");
      }

      var nodesToSelect = new List<MultiSelectTreeViewExItem>();

      if (lastIndex == firstIndex)
      {
        return new List<MultiSelectTreeViewExItem> { firstNode };
      }

      if (lastIndex > firstIndex)
      {
        for (int i = firstIndex; i <= lastIndex; i++)
        {
          if (allNodes[i].IsVisible)
          {
            nodesToSelect.Add(allNodes[i]);
          }
        }
      }
      else
      {
        for (int i = firstIndex; i >= lastIndex; i--)
        {
          if (allNodes[i].IsVisible)
          {
            nodesToSelect.Add(allNodes[i]);
          }
        }
      }

      return nodesToSelect;
    }

    /// <summary>
    /// Finds the treeview item for each of the specified data items.
    /// </summary>
    /// <param name="dataItems">List of data items to search for.</param>
    /// <returns></returns>
    internal IEnumerable<MultiSelectTreeViewExItem> GetTreeViewItemsFor(IEnumerable dataItems)
    {
      if (dataItems == null)
      {
        yield break;
      }

      //foreach (var dataItem in dataItems)
      //{
      //  foreach (var treeViewItem in RecursiveTreeViewItemEnumerable(this, true))
      //  {
      //    if (treeViewItem.DataContext == dataItem)
      //    {
      //      yield return treeViewItem;
      //      break;
      //    }
      //  }
      //}
      var items = dataItems.Cast<object>().ToList();
      foreach (var treeViewExItem in TreeViewElementFinder.FindAll(this, false))
      {
        if (items.Contains(treeViewExItem.DataContext))
        {
          yield return treeViewExItem;
        }
      }
    }

    /// <summary>
    /// Gets all data items referenced in all treeview items of the entire control.
    /// </summary>
    /// <returns></returns>
    //internal IEnumerable GetAllDataItems()
    //{
    //  foreach (var treeViewItem in RecursiveTreeViewItemEnumerable(this, true))
    //  {
    //    yield return treeViewItem.DataContext;
    //  }
    //}

    private void OnSelectedItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Add:
#if DEBUG
          // Make sure we don't confuse MultiSelectTreeViewItems and their DataContexts while development
          if (e.NewItems.OfType<MultiSelectTreeViewExItem>().Any())
            throw new ArgumentException("A MultiSelectTreeViewItem instance was added to the SelectedItems collection. Only their DataContext instances must be added to this list!");
#endif

          object last = null;
          foreach (var item in GetTreeViewItemsFor(e.NewItems))
          {
            item.IsSelected = true;
            item.BringIntoView();

            last = item.DataContext;
          }

          LastSelectedItem = last;
          break;
        case NotifyCollectionChangedAction.Remove:
          foreach (var item in GetTreeViewItemsFor(e.OldItems))
          {
            item.IsSelected = false;
            if (item.DataContext == LastSelectedItem)
            {
              if (SelectedItems.Count > 0)
              {
                LastSelectedItem = SelectedItems[SelectedItems.Count - 1];
              }
              else
              {
                LastSelectedItem = null;
              }
            }
          }

          break;
        case NotifyCollectionChangedAction.Reset:
          //foreach (var item in RecursiveTreeViewItemEnumerable(this, true))
          //{
          //  if (item.IsSelected)
          //  {
          //    item.IsSelected = false;
          //  }
          //}

          //LastSelectedItem = null;
          foreach (var item in TreeViewElementFinder.FindAll(this, false))
          {
            if (item.IsSelected)
            {
              item.IsSelected = false;
            }
          }

          LastSelectedItem = null;
          break;
        default:
          throw new InvalidOperationException();
      }

      OnSelectionChanged();
    }

    //protected override void OnKeyDown(KeyEventArgs e)
    //{
    //  base.OnKeyDown(e);
    //  if (!e.Handled)
    //  {
    //    // Basically, this should not be needed anymore. It allows selecting an item with
    //    // the keyboard when the MultiSelectTreeView control has the focus. If there were already
    //    // items when the control was focused, an item has already been focused (and
    //    // subsequent key presses won't land here but at the item).
    //    Key key = e.Key;
    //    switch (key)
    //    {
    //      case Key.Up:
    //        // Select last item
    //        var lastNode = RecursiveTreeViewItemEnumerable(this, false).LastOrDefault();
    //        if (lastNode != null)
    //        {
    //          Selection.Select(lastNode);
    //          e.Handled = true;
    //        }
    //        break;
    //      case Key.Down:
    //        // Select first item
    //        var firstNode = RecursiveTreeViewItemEnumerable(this, false).FirstOrDefault();
    //        if (firstNode != null)
    //        {
    //          Selection.Select(firstNode);
    //          e.Handled = true;
    //        }
    //        break;
    //    }
    //  }
    //}

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
      base.OnPreviewKeyDown(e);
      if (!IsKeyboardMode)
      {
        IsKeyboardMode = true;
        //System.Diagnostics.Debug.WriteLine("Changing to keyboard mode from PreviewKeyDown");
      }
    }

    protected override void OnPreviewKeyUp(KeyEventArgs e)
    {
      base.OnPreviewKeyDown(e);
      if (!IsKeyboardMode)
      {
        IsKeyboardMode = true;
        //System.Diagnostics.Debug.WriteLine("Changing to keyboard mode from PreviewKeyUp");
      }
    }

    protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
    {
      base.OnPreviewMouseDown(e);
      if (IsKeyboardMode)
      {
        IsKeyboardMode = false;
        //System.Diagnostics.Debug.WriteLine("Changing to mouse mode");
      }
    }

    protected override void OnGotFocus(RoutedEventArgs e)
    {
      //System.Diagnostics.Debug.WriteLine("MultiSelectTreeView.OnGotFocus()");
      //System.Diagnostics.Debug.WriteLine(Environment.StackTrace);

      base.OnGotFocus(e);

      // If the MultiSelectTreeView control has gotten the focus, it needs to pass it to an
      // item instead. If there was an item focused before, return to that. Otherwise just
      // focus this first item in the list if any. If there are no items at all, the
      // MultiSelectTreeView control just keeps the focus.
      // In any case, the focussing must occur when the current event processing is finished,
      // i.e. be queued in the dispatcher. Otherwise the TreeView could keep its focus
      // because other focus things are still going on and interfering this final request.

      var lastFocusedItem = LastFocusedItem;
      if (lastFocusedItem != null)
      {
        Dispatcher.BeginInvoke((Action)(() => FocusHelper.Focus(lastFocusedItem)));
      }
      else
      {
        //var firstNode = RecursiveTreeViewItemEnumerable(this, false).FirstOrDefault();
        var firstNode = TreeViewElementFinder.FindAll(this, false).FirstOrDefault();
        if (firstNode != null)
        {
          Dispatcher.BeginInvoke((Action)(() => FocusHelper.Focus(firstNode)));
        }
      }
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
      base.OnMouseDown(e);

      // This happens when a mouse button was pressed in an area which is not covered by an
      // item. Then, it should be focused which in turn passes on the focus to an item.
      Focus();
    }

    protected void OnPreviewSelectionChanged(PreviewSelectionChangedEventArgs e)
    {
      var handler = PreviewSelectionChanged;
      if (handler != null)
      {
        handler(this, e);
      }
    }

    protected void OnSelectionChanged()
    {
      var handler = SelectionChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    internal MultiSelectTreeViewExItem GetTreeViewItemFor(object item)
    {
      foreach (var treeViewExItem in TreeViewElementFinder.FindAll(this, false))
      {
        if (item == treeViewExItem.DataContext)
        {
          return treeViewExItem;
        }
      }

      return null;
    }

    private void OnIsDragNDropEnabledChanged(object sender, EventArgs e)
    {
      dragNDropController.Enabled = IsDragNDropEnabled;
    }

    #endregion
  }
}
