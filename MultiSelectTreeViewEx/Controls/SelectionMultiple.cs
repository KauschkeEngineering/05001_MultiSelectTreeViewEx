namespace System.Windows.Controls
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;
  using System.Windows;
  using System.Windows.Input;
  using System.Windows.Media;

  /// <summary>
  /// Logic for the multiple selection
  /// </summary>
  internal class SelectionMultiple : InputSubscriberBase, ISelectionStrategy
  {
    #region Private fields and constructor

    public event EventHandler<PreviewSelectionChangedEventArgs> PreviewSelectionChanged;

    private readonly MultiSelectTreeViewEx treeView;
    private BorderSelectionLogic borderSelectionLogic;
    private object lastShiftRoot;

    public SelectionMultiple(MultiSelectTreeViewEx treeViewEx)
    {
      this.treeView = treeViewEx;
    }
    #endregion

    #region Properties

    public bool LastCancelAll { get; private set; }

    internal static bool IsControlKeyDown
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

    #endregion

    #region Private helper methods

    private MultiSelectTreeViewExItem GetFocusedItem()
    {
      foreach (var item in TreeViewElementFinder.FindAll(treeView, false))
      {
        if (item.IsFocused) return item;
      }

      return null;
    }
    #endregion

    #region Private modify selection methods

    private void ToggleItem(MultiSelectTreeViewExItem item)
    {
      if (treeView.SelectedItems.Contains(item.DataContext))
      {
        ModifySelection(null, item.DataContext);
      }
      else
      {
        ModifySelection(item.DataContext, null);
      }
    }

    private bool ModifySelection(object itemToSelect, List<object> itemsToUnselect)
    {
      List<object> itemsToSelect = new List<object>();
      itemsToSelect.Add(itemToSelect);

      if (itemsToUnselect == null) itemsToUnselect = new List<object>();

      return ModifySelection(itemsToSelect, itemsToUnselect);
    }

    private bool ModifySelection(List<object> itemsToSelect, object itemToUnselect)
    {
      if (itemsToSelect == null) itemsToSelect = new List<object>();

      List<object> itemsToUnselect = new List<object>();
      itemsToUnselect.Add(itemToUnselect);

      return ModifySelection(itemsToSelect, itemsToUnselect);
    }

    private bool ModifySelection(List<object> itemsToSelect, List<object> itemsToUnselect)
    {
      //clean up any duplicate or unnecessery input
      OptimizeModifySelection(itemsToSelect, itemsToUnselect);

      //check if there's anything to do.
      if (itemsToSelect.Count == 0 && itemsToUnselect.Count == 0)
      {
        return false;
      }

      // notify listeners what is about to change.
      // Let them cancel and/or handle the selection list themself
      bool allowed = treeView.CheckSelectionAllowed(itemsToSelect, itemsToUnselect);
      if (!allowed) return false;

      // Unselect and then select items
      foreach (object itemToUnSelect in itemsToUnselect)
      {
        treeView.SelectedItems.Remove(itemToUnSelect);
      }

      foreach (object itemToSelect in itemsToSelect)
      {
        treeView.SelectedItems.Add(itemToSelect);
      }

      object lastSelectedItem = itemsToSelect.LastOrDefault();

      if (itemsToUnselect.Contains(lastShiftRoot)) lastShiftRoot = null;
      if (!(TreeView.SelectedItems.Contains(lastShiftRoot) && IsShiftKeyDown)) lastShiftRoot = lastSelectedItem;

      return true;
    }

    private void OptimizeModifySelection(List<object> itemsToSelect, List<object> itemsToUnselect)
    {
      // check for items in both lists and remove them in unselect list
      List<object> biggerList;
      List<object> smallerList;
      if (itemsToSelect.Count > itemsToUnselect.Count)
      {
        biggerList = itemsToSelect;
        smallerList = itemsToUnselect;
      }
      else
      {
        smallerList = itemsToUnselect;
        biggerList = itemsToSelect;
      }

      List<object> temporaryList = new List<object>();
      foreach (object item in biggerList)
      {
        if (smallerList.Contains(item))
        {
          temporaryList.Add(item);
        }
      }

      foreach (var item in temporaryList)
      {
        itemsToUnselect.Remove(item);
      }

      // check for itemsToSelect allready in treeViewEx.SelectedItems
      temporaryList.Clear();
      foreach (object item in itemsToSelect)
      {
        if (treeView.SelectedItems.Contains(item))
        {
          temporaryList.Add(item);
        }
      }

      foreach (var item in temporaryList)
      {
        itemsToSelect.Remove(item);
      }

      // check for itemsToUnSelect not in treeViewEx.SelectedItems
      temporaryList.Clear();
      foreach (object item in itemsToUnselect)
      {
        if (!treeView.SelectedItems.Contains(item))
        {
          temporaryList.Add(item);
        }
      }

      foreach (var item in temporaryList)
      {
        itemsToUnselect.Remove(item);
      }
    }

    private void SelectSingleItem(MultiSelectTreeViewExItem item)
    {

      // selection with SHIFT is not working in virtualized mode. Thats because the Items are not visible.
      // Therefor the children cannot be found/selected.
      if (IsShiftKeyDown && treeView.SelectedItems.Count > 0 && !treeView.IsVirtualizing)
      {
        SelectWithShift(item);
      }
      else if (IsControlKeyDown)
      {
        ToggleItem(item);
      }
      else
      {
        List<object> itemsToUnSelect = treeView.SelectedItems.Cast<object>().ToList();
        if (itemsToUnSelect.Contains(item.DataContext))
        {
          itemsToUnSelect.Remove(item.DataContext);
        }
        ModifySelection(item.DataContext, itemsToUnSelect);
      }

    }

    private bool SelectFromKey(MultiSelectTreeViewExItem item)
    {
      SelectSingleItem(item);

      FocusHelper.Focus(item);
      return true;
      //if (item == null)
      //{
      //    return false;
      //}

      //// If Ctrl is pressed just focus it, so it can be selected by Space. Otherwise select it.
      //if (IsControlKeyDown)
      //{
      //    FocusHelper.Focus(item, true);
      //    return true;
      //}
      //else
      //{
      //    return SelectCore(item);
      //}
    }

    #endregion

    #region Overrides InputSubscriberBase

    private MultiSelectTreeViewExItem selectItem = null;
    private Point selectPosition;
    private MouseButton selectButton;

    internal override void OnMouseDown(MouseButtonEventArgs e)
    {
      base.OnMouseDown(e);

      MultiSelectTreeViewExItem item = GetTreeViewItemUnderMouse(e.GetPosition(treeView));
      if (item == null) return;
      if (e.ChangedButton != MouseButton.Left && !(e.ChangedButton == MouseButton.Right && item.ContextMenu != null)) return;
      if (item.IsEditing) return;

      // ToggleItem or SelectWithShift
      if (IsControlKeyDown || (IsShiftKeyDown))
      {
        SelectSingleItem(item);
        FocusHelper.Focus(item);
        return;
      }

      // begin click
      if (selectItem == null)
      {
        selectItem = item;
        selectPosition = e.GetPosition(TreeView);
        selectButton = e.ChangedButton;
      }
    }

    internal override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);

      if (selectItem != null)
      {
        // detect drag
        var dragDiff = selectPosition - e.GetPosition(TreeView);
        if ((Math.Abs(dragDiff.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(dragDiff.Y) > SystemParameters.MinimumVerticalDragDistance))
        {
          // abort click
          selectItem = null;
        }
      }
    }

    internal override void OnMouseUp(MouseButtonEventArgs e)
    {
      base.OnMouseUp(e);

      // click
      if (selectItem != null && e.ChangedButton == selectButton)
      {
        // select item
        SelectSingleItem(selectItem);
        FocusHelper.Focus(selectItem);

        // end click
        selectItem = null;
      }
    }

    private void SelectWithShift(MultiSelectTreeViewExItem item)
    {
      object firstSelectedItem;
      if (lastShiftRoot != null)
      {
        firstSelectedItem = lastShiftRoot;
      }
      else
      {
        // Get the first item in the SelectedItems that is also bound to the Tree.
        firstSelectedItem = treeView.SelectedItems.Cast<object>().FirstOrDefault((x) => { return treeView.GetTreeViewItemFor(x) != null; });
      }

      if (firstSelectedItem != null)
      {
        MultiSelectTreeViewExItem shiftRootItem = treeView.GetTreeViewItemsFor(new List<object> { firstSelectedItem }).First();

        List<object> itemsToSelect = treeView.GetNodesToSelectBetween(shiftRootItem, item).Select(x => x.DataContext).ToList();
        List<object> itemsToUnSelect = treeView.SelectedItems.Cast<object>().ToList();

        ModifySelection(itemsToSelect, itemsToUnSelect);
      }
      else
      {   // Fall-back to sigle selection
        List<object> itemsToUnSelect = treeView.SelectedItems.Cast<object>().ToList();
        if (itemsToUnSelect.Contains(item.DataContext))
          itemsToUnSelect.Remove(item.DataContext);
        ModifySelection(item.DataContext, itemsToUnSelect);
      }
    }
    #endregion

    #region Methods called by BorderSelection

    public void InvalidateLastShiftRoot(object item)
    {
      if (lastShiftRoot == item)
      {
        lastShiftRoot = null;
      }
    }

    public void ApplyTemplate()
    {
      borderSelectionLogic = new BorderSelectionLogic(
         treeView,
         treeView.Template.FindName("selectionBorder", treeView) as Border,
         treeView.Template.FindName("scroller", treeView) as ScrollViewer,
         treeView.Template.FindName("content", treeView) as ItemsPresenter,
         TreeViewElementFinder.FindAll(treeView, false));
      //MultiSelectTreeViewEx.RecursiveTreeViewItemEnumerable(treeView, false, false));
    }

    public bool Select(MultiSelectTreeViewExItem item)
    {
      if (IsControlKeyDown)
      {
        if (treeView.SelectedItems.Contains(item.DataContext))
        {
          return Deselect(item, true);
        }
        else
        {
          var e = new PreviewSelectionChangedEventArgs(true, item.DataContext);
          OnPreviewSelectionChanged(e);
          if (e.CancelAny)
          {
            FocusHelper.Focus(item, true);
            return false;
          }

          return SelectCore(item);
        }
      }
      else
      {
        if (treeView.SelectedItems.Count == 1 &&
            treeView.SelectedItems[0] == item.DataContext)
        {
          // Requested to select the single already-selected item. Don't change the selection.
          FocusHelper.Focus(item, true);
          lastShiftRoot = item.DataContext;
          return true;
        }
        else
        {
          return SelectCore(item);
        }
      }
    }

    internal bool SelectByRectangle(MultiSelectTreeViewExItem item)
    {
      var e = new PreviewSelectionChangedEventArgs(true, item.DataContext);
      OnPreviewSelectionChanged(e);
      if (e.CancelAny)
      {
        lastShiftRoot = item.DataContext;
        return false;
      }

      if (!treeView.SelectedItems.Contains(item.DataContext))
      {
        treeView.SelectedItems.Add(item.DataContext);
      }
      lastShiftRoot = item.DataContext;
      return true;
    }

    internal bool DeselectByRectangle(MultiSelectTreeViewExItem item)
    {
      var e = new PreviewSelectionChangedEventArgs(false, item.DataContext);
      OnPreviewSelectionChanged(e);
      if (e.CancelAny)
      {
        lastShiftRoot = item.DataContext;
        return false;
      }

      treeView.SelectedItems.Remove(item.DataContext);
      if (item.DataContext == lastShiftRoot)
      {
        lastShiftRoot = null;
      }
      return true;
    }

    internal void UnSelectByRectangle(MultiSelectTreeViewExItem item)
    {
      if (!treeView.CheckSelectionAllowed(item.DataContext, false)) return;

      treeView.SelectedItems.Remove(item.DataContext);
      if (item.DataContext == lastShiftRoot)
      {
        lastShiftRoot = null;
      }
    }

    internal void SelectByRectangle(List<object> itemsToSelect, List<object> itemsToUnselect)
    {
      if (itemsToSelect == null) itemsToSelect = new List<object>();
      if (itemsToUnselect == null) itemsToUnselect = new List<object>();

      ModifySelection(itemsToSelect, itemsToUnselect);
    }

    #endregion

    #region ISelectionStrategy Members

    public void SelectFromUiAutomation(MultiSelectTreeViewExItem item)
    {
      SelectSingleItem(item);

      FocusHelper.Focus(item);
    }

    public void SelectFromProperty(MultiSelectTreeViewExItem item, bool isSelected)
    {
      // we do not check if selection is allowed, because selecting on that way is no user action.
      // Hopefully the programmer knows what he does...
      if (isSelected)
      {
        treeView.SelectedItems.Add(item.DataContext);
        lastShiftRoot = item.DataContext;
        FocusHelper.Focus(item);
      }
      else
      {
        treeView.SelectedItems.Remove(item.DataContext);
      }
    }

    public void SelectFirst()
    {
      MultiSelectTreeViewExItem item = TreeViewElementFinder.FindFirst(treeView, true);
      if (item != null)
      {
        SelectSingleItem(item);
      }

      FocusHelper.Focus(item);
    }

    public void SelectLast()
    {
      MultiSelectTreeViewExItem item = TreeViewElementFinder.FindLast(treeView, true);
      if (item != null)
      {
        SelectSingleItem(item);
      }

      FocusHelper.Focus(item);
    }

    public bool SelectFirstFromKey()
    {
      //List<MultiSelectTreeViewExItem> items = MultiSelectTreeViewEx.RecursiveTreeViewItemEnumerable(treeView, false, false).ToList();
      //List<MultiSelectTreeViewExItem> items = TreeViewElementFinder.FindAll(treeView, false).ToList();
      //MultiSelectTreeViewExItem item = treeView.GetFirstItem(items);
      //      return SelectFromKey(item);
      MultiSelectTreeViewExItem item = TreeViewElementFinder.FindFirst(treeView, true);
      if (item != null)
      {
        SelectSingleItem(item);
      }

      FocusHelper.Focus(item);
      return true;
    }

    public bool SelectLastFromKey()
    {
      //List<MultiSelectTreeViewExItem> items = MultiSelectTreeViewEx.RecursiveTreeViewItemEnumerable(treeView, false, false).ToList();
      //List<MultiSelectTreeViewExItem> items = TreeViewElementFinder.FindAll(treeView, false).ToList();
      //MultiSelectTreeViewExItem item = treeView.GetLastItem(items);
      //      return SelectFromKey(item);
      MultiSelectTreeViewExItem item = TreeViewElementFinder.FindLast(treeView, true);
      if (item != null)
      {
        SelectSingleItem(item);
      }

      FocusHelper.Focus(item);
      return true;
    }

    public void SelectNextFromKeyEx()
    {
      MultiSelectTreeViewExItem item = GetFocusedItem();
      item = TreeViewElementFinder.FindNext(item, true);
      if (item == null) return;

      // if ctrl is pressed just focus it, so it can be selected by space. Otherwise select it.
      if (!IsControlKeyDown)
      {
        SelectSingleItem(item);
      }

      FocusHelper.Focus(item);
    }

    public bool SelectNextFromKey()
    {
      //List<MultiSelectTreeViewExItem> items = TreeViewElementFinder.FindAll(treeView, false).ToList();
      ////List<MultiSelectTreeViewExItem> items = MultiSelectTreeViewEx.RecursiveTreeViewItemEnumerable(treeView, false, false).ToList();
      //      MultiSelectTreeViewExItem item = treeView.GetNextItem(GetFocusedItem(), items);
      //      return SelectFromKey(item);
      //  }

      //  public void SelectPreviousFromKeyEx()
      //  {
      //      List<MultiSelectTreeViewExItem> items = TreeViewElementFinder.FindAll(treeView, true).ToList();
      //      MultiSelectTreeViewExItem item = GetFocusedItem();
      //      item = treeView.GetPreviousItem(item, items);
      //      if (item == null) return;

      //      // if ctrl is pressed just focus it, so it can be selected by space. Otherwise select it.
      //      if (!IsControlKeyDown)
      //      {
      //          SelectSingleItem(item);
      //      }

      //      FocusHelper.Focus(item);
      MultiSelectTreeViewExItem item = GetFocusedItem();
      item = TreeViewElementFinder.FindNext(item, true);
      if (item == null) return false;

      // if ctrl is pressed just focus it, so it can be selected by space. Otherwise select it.
      if (!IsControlKeyDown)
      {
        SelectSingleItem(item);
      }

      FocusHelper.Focus(item);
      return true;
    }

    public bool SelectPreviousFromKey()
    {
      ////List<MultiSelectTreeViewExItem> items = MultiSelectTreeViewEx.RecursiveTreeViewItemEnumerable(treeView, false, false).ToList();
      //List<MultiSelectTreeViewExItem> items = TreeViewElementFinder.FindAll(treeView, true).ToList();
      //MultiSelectTreeViewExItem item = treeView.GetPreviousItem(GetFocusedItem(), items);
      //      return SelectFromKey(item);
      List<MultiSelectTreeViewExItem> items = TreeViewElementFinder.FindAll(treeView, true).ToList();
      MultiSelectTreeViewExItem item = GetFocusedItem();
      item = treeView.GetPreviousItem(item, items);
      if (item == null) return false;

      // if ctrl is pressed just focus it, so it can be selected by space. Otherwise select it.
      if (!IsControlKeyDown)
      {
        SelectSingleItem(item);
      }

      FocusHelper.Focus(item);
      return true;
    }

    private bool SelectPageUpDown(bool down)
    {
      //List<MultiSelectTreeViewExItem> items = MultiSelectTreeViewEx.RecursiveTreeViewItemEnumerable(treeView, false, false).ToList();
      List<MultiSelectTreeViewExItem> items = TreeViewElementFinder.FindAll(treeView, true).ToList();
      MultiSelectTreeViewExItem item = GetFocusedItem();
      if (item == null)
      {
        return down ? SelectLastFromKey() : SelectFirstFromKey();
      }

      double targetY = item.TransformToAncestor(treeView).Transform(new Point()).Y;
      FrameworkElement itemContent = (FrameworkElement)item.Template.FindName("headerBorder", item);
      double offset = treeView.ActualHeight - 2 * itemContent.ActualHeight;
      if (!down) offset = -offset;
      targetY += offset;
      while (true)
      {
        var newItem = down ? treeView.GetNextItem(item, items) : treeView.GetPreviousItem(item, items);
        if (newItem == null) break;
        item = newItem;
        double itemY = item.TransformToAncestor(treeView).Transform(new Point()).Y;
        if (down && itemY > targetY ||
            !down && itemY < targetY)
        {
          break;
        }
      }
      return SelectFromKey(item);
    }

    public bool SelectPageUpFromKey()
    {
      return SelectPageUpDown(false);
    }

    public bool SelectPageDownFromKey()
    {
      return SelectPageUpDown(true);
    }

    public bool SelectAllFromKey()
    {
      List<MultiSelectTreeViewExItem> items = TreeViewElementFinder.FindAll(treeView, true).ToList();
      //var items = MultiSelectTreeViewEx.RecursiveTreeViewItemEnumerable(treeView, false, false).ToList();
      // Add new selected items
      foreach (var item in items.Where(i => !treeView.SelectedItems.Contains(i.DataContext)))
      {
        var e = new PreviewSelectionChangedEventArgs(true, item.DataContext);
        OnPreviewSelectionChanged(e);
        if (e.CancelAll)
        {
          return false;
        }
        if (!e.CancelThis)
        {
          treeView.SelectedItems.Add(item.DataContext);
        }
      }
      return true;
    }

    public bool SelectCore(MultiSelectTreeViewExItem item)
    {
      if (IsControlKeyDown)
      {
        if (!treeView.SelectedItems.Contains(item.DataContext))
        {
          treeView.SelectedItems.Add(item.DataContext);
        }
        lastShiftRoot = item.DataContext;
      }
      else if (IsShiftKeyDown && treeView.SelectedItems.Count > 0)
      {
        object firstSelectedItem = lastShiftRoot ?? treeView.SelectedItems.First();
        MultiSelectTreeViewExItem shiftRootItem = treeView.GetTreeViewItemsFor(new List<object> { firstSelectedItem }).First();

        var newSelection = treeView.GetNodesToSelectBetween(shiftRootItem, item).Select(n => n.DataContext).ToList();
        // Make a copy of the list because we're modifying it while enumerating it
        var selectedItems = new object[treeView.SelectedItems.Count];
        treeView.SelectedItems.CopyTo(selectedItems, 0);
        // Remove all items no longer selected
        foreach (var selItem in selectedItems.Where(i => !newSelection.Contains(i)))
        {
          var e = new PreviewSelectionChangedEventArgs(false, selItem);
          OnPreviewSelectionChanged(e);
          if (e.CancelAll)
          {
            FocusHelper.Focus(item);
            return false;
          }
          if (!e.CancelThis)
          {
            treeView.SelectedItems.Remove(selItem);
          }
        }
        // Add new selected items
        foreach (var newItem in newSelection.Where(i => !selectedItems.Contains(i)))
        {
          var e = new PreviewSelectionChangedEventArgs(true, newItem);
          OnPreviewSelectionChanged(e);
          if (e.CancelAll)
          {
            FocusHelper.Focus(item, true);
            return false;
          }
          if (!e.CancelThis)
          {
            treeView.SelectedItems.Add(newItem);
          }
        }
      }
      else
      {
        if (treeView.SelectedItems.Count > 0)
        {
          foreach (var selItem in new ArrayList(treeView.SelectedItems))
          {
            var e2 = new PreviewSelectionChangedEventArgs(false, selItem);
            OnPreviewSelectionChanged(e2);
            if (e2.CancelAll)
            {
              FocusHelper.Focus(item);
              lastShiftRoot = item.DataContext;
              return false;
            }
            if (!e2.CancelThis)
            {
              treeView.SelectedItems.Remove(selItem);
            }
          }
        }

        var e = new PreviewSelectionChangedEventArgs(true, item.DataContext);
        OnPreviewSelectionChanged(e);
        if (e.CancelAny)
        {
          FocusHelper.Focus(item, true);
          lastShiftRoot = item.DataContext;
          return false;
        }

        treeView.SelectedItems.Add(item.DataContext);
        lastShiftRoot = item.DataContext;
      }

      FocusHelper.Focus(item, true);
      return true;
    }

    public bool SelectCurrentBySpace()
    {
      // Another item was focused by Ctrl+Arrow key
      var item = GetFocusedItem();
      if (treeView.SelectedItems.Contains(item.DataContext))
      {
        // With Ctrl key, toggle this item selection (deselect now).
        // Without Ctrl key, always select it (is already selected).
        if (IsControlKeyDown)
        {
          if (!Deselect(item, true)) return false;
          item.IsSelected = false;
        }
      }
      else
      {
        var e = new PreviewSelectionChangedEventArgs(true, item.DataContext);
        OnPreviewSelectionChanged(e);
        if (e.CancelAny)
        {
          FocusHelper.Focus(item, true);
          return false;
        }

        item.IsSelected = true;
        if (!treeView.SelectedItems.Contains(item.DataContext))
        {
          treeView.SelectedItems.Add(item.DataContext);
        }
      }
      FocusHelper.Focus(item, true);
      return true;
    }

    public void ClearObsoleteItems(IEnumerable items)
    {
      foreach (var itemToUnSelect in items)
      {
        if (itemToUnSelect == lastShiftRoot)
          lastShiftRoot = null;
        if (!treeView.SelectedItems.IsReadOnly && treeView.SelectedItems.Contains(itemToUnSelect))
          treeView.SelectedItems.Remove(itemToUnSelect);
      }
    }

    public bool SelectParentFromKey()
    {
      DependencyObject parent = GetFocusedItem();
      while (parent != null)
      {
        parent = VisualTreeHelper.GetParent(parent);
        if (parent is MultiSelectTreeViewExItem) break;
        if (parent is VirtualizingTreePanel)
        {
          parent = GetFocusedItem();
          break;
        }
      }
      return SelectFromKey(parent as MultiSelectTreeViewExItem);
    }

    public bool Deselect(MultiSelectTreeViewExItem item, bool bringIntoView = false)
    {
      var e = new PreviewSelectionChangedEventArgs(false, item.DataContext);
      OnPreviewSelectionChanged(e);
      if (e.CancelAny) return false;

      treeView.SelectedItems.Remove(item.DataContext);
      if (item.DataContext == lastShiftRoot)
      {
        lastShiftRoot = null;
      }
      FocusHelper.Focus(item, bringIntoView);
      return true;
    }

    #endregion

    public void Dispose()
    {
      if (borderSelectionLogic != null)
      {
        borderSelectionLogic.Dispose();
        borderSelectionLogic = null;
      }

      GC.SuppressFinalize(this);
    }

    protected void OnPreviewSelectionChanged(PreviewSelectionChangedEventArgs e)
    {
      var handler = PreviewSelectionChanged;
      if (handler != null)
      {
        handler(this, e);
        LastCancelAll = e.CancelAll;
      }
    }
  }
}
