// *********************************************************************************************************
// *
// *  Author:       Maximilian Burkhardt
// *  Disclaimer:   Copyright (c) 2024 - Kauschke Engineering Systems GmbH
// * 				        All rights reserved - Unauthorized deobfuscation and making copies of this file,
// * 				        via any medium is strictly prohibited.
// *
// *********************************************************************************************************

namespace System.Windows.Controls.Automation.Peers
{
  #region

  using System;
  using System.Collections.Generic;
  using System.Reflection;
  using System.Windows;
  using System.Windows.Automation;
  using System.Windows.Automation.Peers;
  using System.Windows.Automation.Provider;
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;
  using System.Windows.Media;

  #endregion

  class MultiSelectTreeViewExItemAutomationPeer : ItemsControlAutomationPeer, IExpandCollapseProvider, ISelectionItemProvider, IScrollItemProvider, IInvokeProvider, IValueProvider, IRawElementProviderSimple
  {
    MultiSelectTreeViewExItem treeViewExItem => (MultiSelectTreeViewExItem)Owner;
    string requestedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiSelectTreeViewItemAutomationPeer"/>
    /// class. 
    /// </summary>
    /// <param name="owner">
    /// Das <see cref="T:System.Windows.Controls.MultiSelectTreeViewItem"/>, das diesem
    /// <see cref="T:System.Windows.Automation.Peers.MultiSelectTreeViewItemAutomationPeer"/>
    /// zugeordnet ist.
    /// </param>
    public MultiSelectTreeViewExItemAutomationPeer(MultiSelectTreeViewExItem owner) : base(owner)
    {
    }

    #region Overrides

    protected override Rect GetBoundingRectangleCore()
    {
      var contentPresenter = GetContentPresenter(treeViewExItem);
      if (contentPresenter != null)
      {
        Vector offset = VisualTreeHelper.GetOffset(contentPresenter);
        Point p = new Point(offset.X, offset.Y);
        p = contentPresenter.PointToScreen(p);
        return new Rect(p.X, p.Y, contentPresenter.ActualWidth, contentPresenter.ActualHeight);
      }

      return base.GetBoundingRectangleCore();
    }

    protected override Point GetClickablePointCore()
    {
      var contentPresenter = GetContentPresenter(treeViewExItem);
      if (contentPresenter != null)
      {
        Vector offset = VisualTreeHelper.GetOffset(contentPresenter);
        Point p = new Point(offset.X, offset.Y);
        p = contentPresenter.PointToScreen(p);
        return p;
      }

      return base.GetClickablePointCore();
    }

    private static ContentPresenter GetContentPresenter(MultiSelectTreeViewExItem treeViewItem)
    {
      var contentPresenter = treeViewItem.Template.FindName("PART_Header", treeViewItem) as ContentPresenter;
      return contentPresenter;
    }

    protected override List<AutomationPeer> GetChildrenCore()
    {
      //System.Diagnostics.Trace.WriteLine("MultiSelectTreeViewItemAutomationPeer.GetChildrenCore()");

      List<AutomationPeer> children = new List<AutomationPeer>();
      var button = treeViewExItem.Template.FindName("Expander", treeViewExItem) as ToggleButton;
      AddAutomationPeer(children, button);
      //System.Diagnostics.Trace.WriteLine("- Adding ToggleButton, " + (button == null ? "IS" : "is NOT") + " null, now " + children.Count + " items");

      var contentPresenter = GetContentPresenter(treeViewExItem);

      if (contentPresenter != null)
      {
        int childrenCount = VisualTreeHelper.GetChildrenCount(contentPresenter);
        for (int i = 0; i < childrenCount; i++)
        {
          var child = VisualTreeHelper.GetChild(contentPresenter, i) as UIElement;
          AddAutomationPeer(children, child);
          //System.Diagnostics.Trace.WriteLine("- Adding child UIElement, " + (child == null ? "IS" : "is NOT") + " null, now " + children.Count + " items");
        }
      }

      ItemCollection items = treeViewExItem.Items;
      for (int i = 0; i < items.Count; i++)
      {
        MultiSelectTreeViewExItem treeViewItem = treeViewExItem.ItemContainerGenerator.ContainerFromIndex(i) as MultiSelectTreeViewExItem;
        AddAutomationPeer(children, treeViewItem);
        //System.Diagnostics.Trace.WriteLine("- Adding MultiSelectTreeViewItem, " + (treeViewItem == null ? "IS" : "is NOT") + " null, now " + children.Count + " items");
      }

      if (children.Count > 0)
      {
        //System.Diagnostics.Trace.WriteLine("MultiSelectTreeViewItemAutomationPeer.GetChildrenCore(): returning " + children.Count + " children");
        //for (int i = 0; i < children.Count; i++)
        //{
        //    System.Diagnostics.Trace.WriteLine("- Item " + i + " " + (children[i] == null ? "IS" : "is NOT") + " null");
        //}
        return children;
      }

      //System.Diagnostics.Trace.WriteLine("MultiSelectTreeViewItemAutomationPeer.GetChildrenCore(): returning null");
      return null;
    }

    private static void AddAutomationPeer(List<AutomationPeer> children, UIElement child)
    {
      if (child != null)
      {
        AutomationPeer peer = FromElement(child);
        if (peer == null)
        {
          peer = CreatePeerForElement(child);
        }

        if (peer != null)
        {
          // In the array that GetChildrenCore returns, which is used by AutomationPeer.EnsureChildren,
          // no null entries are allowed or a NullReferenceException will be thrown from the guts of WPF.
          // This has reproducibly been observed null on certain systems so the null check was added.
          // This may mean that some child controls are missing for automation, but at least the
          // application doesn't crash in normal usage.
          children.Add(peer);
        }
      }
    }
    protected override string GetAutomationIdCore()
    {
      return treeViewExItem.GetValue(AutomationProperties.AutomationIdProperty) as string;
    }

    protected override string GetAcceleratorKeyCore()
    {
      return string.Empty;
    }

    protected override string GetAccessKeyCore()
    {
      return string.Empty;
    }

    protected override string GetClassNameCore()
    {
      return treeViewExItem.GetType().ToString();
    }

    protected override string GetHelpTextCore()
    {
      return string.Empty;
    }

    protected override string GetItemStatusCore()
    {
      return string.Empty;
    }

    protected override string GetItemTypeCore()
    {
      return string.Empty;
    }

    protected override AutomationPeer GetLabeledByCore()
    {
      return null;
    }

    protected override string GetNameCore()
    {
      if (treeViewExItem.DataContext == null)
      {
        return treeViewExItem.ToString();
      }

      return treeViewExItem.DataContext.ToString();
    }

    protected override AutomationOrientation GetOrientationCore()
    {
      return AutomationOrientation.Vertical;
    }

    protected override bool HasKeyboardFocusCore()
    {
      return treeViewExItem.IsKeyboardFocused;
    }

    protected override bool IsContentElementCore()
    {
      return true;
    }

    protected override bool IsControlElementCore()
    {
      return true;
    }

    protected override bool IsEnabledCore()
    {
      return treeViewExItem.IsEnabled;
    }

    protected override bool IsKeyboardFocusableCore()
    {
      return treeViewExItem.IsFocused;
    }

    protected override bool IsOffscreenCore()
    {
      if (treeViewExItem.ParentTreeViewItem != null && !treeViewExItem.ParentTreeViewItem.IsExpanded) return true;
      return false;
    }

    protected override bool IsPasswordCore()
    {
      return false;
    }

    protected override bool IsRequiredForFormCore()
    {
      return false;
    }

    protected override void SetFocusCore()
    {
    }

    public void Collapse()
    {
      if (!IsEnabled())
      {
        throw new ElementNotEnabledException();
      }

      if (!treeViewExItem.HasItems)
      {
        throw new InvalidOperationException("Cannot collapse because item has no children.");
      }

      treeViewExItem.IsExpanded = false;
    }

    public void Expand()
    {
      if (!IsEnabled())
      {
        throw new ElementNotEnabledException();
      }

      if (!treeViewExItem.HasItems)
      {
        throw new InvalidOperationException("Cannot expand because item has no children.");
      }

      treeViewExItem.IsExpanded = true;
    }

    protected override AutomationControlType GetAutomationControlTypeCore()
    {
      return AutomationControlType.TreeItem;
    }

    public override object GetPattern(PatternInterface patternInterface)
    {
      if (patternInterface == PatternInterface.ExpandCollapse)
      {
        return this;
      }

      if (patternInterface == PatternInterface.SelectionItem)
      {
        return this;
      }

      if (patternInterface == PatternInterface.ScrollItem)
      {
        return this;
      }

      if (patternInterface == PatternInterface.Value)
      {
        return this;
      }

      return base.GetPattern(patternInterface);
    }

    #endregion

    #region IExpandCollapseProvider

    public ExpandCollapseState ExpandCollapseState
    {
      get
      {
        if (!treeViewExItem.HasItems)
        {
          return ExpandCollapseState.LeafNode;
        }

        if (!treeViewExItem.IsExpanded)
        {
          return ExpandCollapseState.Collapsed;
        }

        return ExpandCollapseState.Expanded;
      }
    }

    #endregion

    #region Public Methods

    bool ISelectionItemProvider.IsSelected
    {
      get
      {
        return treeViewExItem.IsSelected;
      }
    }

    IRawElementProviderSimple ISelectionItemProvider.SelectionContainer
    {
      get
      {
        ItemsControl parentItemsControl = treeViewExItem.ParentTreeView;
        if (parentItemsControl != null)
        {
          AutomationPeer automationPeer = FromElement(parentItemsControl);
          if (automationPeer != null)
          {
            return ProviderFromPeer(automationPeer);
          }
        }

        return null;
      }
    }

    void IScrollItemProvider.ScrollIntoView()
    {
      treeViewExItem.BringIntoView();
    }

    void ISelectionItemProvider.AddToSelection()
    {
      throw new NotImplementedException();
    }

    void ISelectionItemProvider.RemoveFromSelection()
    {

      throw new NotImplementedException();
    }

    void ISelectionItemProvider.Select()
    {
      treeViewExItem.ParentTreeView.Selection.SelectFromUiAutomation(treeViewExItem);
    }

    #endregion

    #region IValueProvider Members

    public bool IsReadOnly
    {
      get
      {
        return false;
      }
    }

    public void SetValue(string value)
    {
      try
      {
        if (String.IsNullOrWhiteSpace(value)) return;

        string[] ids = value.Split(new[] { ';' });

        object obj;
        if (ids.Length > 0 && ids[0] == "Context")
        {
          obj = treeViewExItem.DataContext;
        }
        else
        {
          obj = treeViewExItem;
        }

        if (ids.Length < 2)
        {
          requestedValue = obj.ToString();
        }
        else
        {
          Type type = obj.GetType();
          PropertyInfo pi = type.GetProperty(ids[1]);
          requestedValue = pi.GetValue(obj, null).ToString();
        }
      }
      catch (Exception ex)
      {
        requestedValue = ex.ToString();
      }
    }

    public string Value
    {
      get
      {
        if (requestedValue == null)
        {
          return treeViewExItem.DataContext.ToString();
        }

        return requestedValue;
      }
    }

    #endregion

    #region IInvokeProvider members

    public void Invoke()
    {
      treeViewExItem.InvokeMouseDown();
    }

    #endregion IInvokeProvider members

    #region IRawElementProvider

    public object GetPatternProvider(int patternId)
    {
      return this;
    }

    public object GetPropertyValue(int propertyId)
    {
      return AutomationProperty.LookupById(propertyId);
    }

    protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
    {
      return new MultiSelectTreeViewItemDataAutomationPeer(item, this);
    }

    public IRawElementProviderSimple HostRawElementProvider
    {
      get { return this; }
    }

    public ProviderOptions ProviderOptions
    {
      get { return ProviderOptions.OverrideProvider; }
    }

    #endregion
  }
}
