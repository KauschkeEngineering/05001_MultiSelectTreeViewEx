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
  #region

  using System.Threading;
  using System.Windows;
  using System.Windows.Threading;
  using System.Diagnostics;

  #endregion

  /// <summary>
  /// Helper methods to focus.
  /// </summary>
  public static class FocusHelper
  {
    #region Public Methods

    public static void Focus(EditTextBox element)
    {
      //System.Diagnostics.Debug.WriteLine("Focus textbox with helper:" + element.Text);
      FocusCore(element);
      //element.BringIntoView();
    }

    public static void Focus(MultiSelectTreeViewExItem element, bool bringIntoView = false)
    {
      //System.Diagnostics.Debug.WriteLine("FocusHelper focusing " + (bringIntoView ? "[into view] " : "") + element.DataContext);
      FocusCore(element);

      //if (bringIntoView)
      //{
      //  FrameworkElement itemContent = (FrameworkElement)element.Template.FindName("headerBorder", element);
      //  if (itemContent != null)   // May not be rendered yet...
      //  {
      //    itemContent.BringIntoView();
      //  }
      //}
    }

    public static void Focus(MultiSelectTreeViewEx element)
    {
      //System.Diagnostics.Debug.WriteLine("Focus Tree with helper");
      FocusCore(element);
      //element.BringIntoView();
    }

    private static void FocusCore(FrameworkElement element)
    {
      //System.Diagnostics.Debug.WriteLine("Focusing element " + element.ToString());
      //System.Diagnostics.Debug.WriteLine(Environment.StackTrace);
      if (!element.Focus())
      {
        //System.Diagnostics.Debug.WriteLine("- Element could not be focused, invoking in dispatcher thread");
        element.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() => element.Focus()));
      }

#if DEBUG
      // no good idea, seems to block sometimes
      int i = 0;
      while (i < 5)
      {
        if (element.IsFocused)
        {
          //if (i > 0)
          //    System.Diagnostics.Debug.WriteLine("- Element is focused now in round " + i + ", leaving");
          return;
        }
        Thread.Sleep(20);
        i++;
      }
      //if (i >= 5)
      //{
      //    System.Diagnostics.Debug.WriteLine("- Element is not focused after 500 ms, giving up");
      //}
#endif
    }

    #endregion
  }
}
