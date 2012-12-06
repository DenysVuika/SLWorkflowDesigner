/*
The MIT License (MIT)
Copyright (c) 2012 Denys Vuika

Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
and associated documentation files (the "Software"), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace WorkflowDesigner
{
  public class DoubleClickBehavior : Behavior<FrameworkElement>
  {
    #region Constants

    /// <summary>
    /// The threshold (in miliseconds) between clicks to be considered a double-click.  Windows default is 500; I'm a fast clicker.
    /// </summary>
    private const int ClickThresholdInMiliseconds = 300;

    #endregion

    #region Properties [private]

    /// <summary>
    /// Holds the timestamp of the last click.
    /// </summary>
    private DateTime? LastClick { get; set; }

    /// <summary>
    /// Holds a reference to the instance of the last source object to generate a click.
    /// </summary>
    private object LastSource { get; set; }

    #endregion

    #region Events

    /// <summary>
    /// The event to be raised upon double-click.
    /// </summary>
    public event EventHandler<MouseButtonEventArgs> DoubleClick;

    #endregion

    #region Behavior Members [overridden]

    /// <summary>
    /// This is triggered when the behavior is attached to a FrameworkElement.  An event handler is attached to the
    /// FrameworkElement's MouseLeftButtonUp event.
    /// </summary>
    protected override void OnAttached()
    {
      base.OnAttached();
      AssociatedObject.MouseLeftButtonUp += AssociatedObjectMouseLeftButtonUp;
    }

    /// <summary>
    /// This is triggered when the behavior is detached from a FrameworkElement.  The event handler attached to the MouseLeftButtonUp
    /// event is removed.
    /// </summary>
    protected override void OnDetaching()
    {
      base.OnDetaching();
      AssociatedObject.MouseLeftButtonUp -= AssociatedObjectMouseLeftButtonUp;
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Occurs when the MouseLeftButtonDown event is triggered on the object associated to this behavior (this.AssociatedObject).
    /// </summary>
    /// <param name="sender">The object which is firing the MouseLeftButtonUp.  Note that this is not always the actual source of the event
    /// since events are bubbled; this is why we access e.OriginalSource</param>
    /// <param name="e">The MouseButtonEventArgs associated with the MouseLeftButtonDown event.</param>
    void AssociatedObjectMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (LastSource == null || !Equals(LastSource, e.OriginalSource))
      {
        LastSource = e.OriginalSource;
        LastClick = DateTime.Now;
      }
      else if ((DateTime.Now - LastClick.Value).Milliseconds <= ClickThresholdInMiliseconds)
      {
        LastClick = null;
        LastSource = null;
        if (DoubleClick != null)
          DoubleClick(sender, e);
      }
      else
      {
        LastClick = null;
        LastSource = null;
      }
    }

    #endregion
  }
}
