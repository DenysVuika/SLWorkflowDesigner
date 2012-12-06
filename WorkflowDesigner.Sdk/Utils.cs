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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WorkflowDesigner.Sdk.Design;

namespace WorkflowDesigner.Sdk
{
  public static class Utils
  {
    public static double GetLeft(FrameworkElement element)
    {
      var designerControl = element as ActivityHost;
      return designerControl != null ? designerControl.PositionX : Canvas.GetLeft(element);
    }

    public static double GetTop(FrameworkElement element)
    {
      var designerControl = element as ActivityHost;
      return designerControl != null ? designerControl.PositionY : Canvas.GetTop(element);
    }

    public static T FindParent<T>(UIElement control) where T : UIElement
    {
      var p = VisualTreeHelper.GetParent(control) as UIElement;
      if (p != null)
      {
        if (p is T) return p as T;
        return FindParent<T>(p);
      }
      return null;
    }
  }
}
