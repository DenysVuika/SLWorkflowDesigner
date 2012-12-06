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

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WorkflowDesigner.Sdk.Design
{
  public abstract class FunctionActivityView : Control
  {
    private readonly Dictionary<string, ObjectConnector> _connectors = new Dictionary<string, ObjectConnector>();

    public IDictionary<string, ObjectConnector> Connectors
    {
      get { return _connectors; }
    }

    public static readonly DependencyProperty ActivityProperty =
      DependencyProperty.Register("Activity", typeof(FunctionActivity), typeof(FunctionActivityView), new PropertyMetadata(null, OnActivityChanged));

    private static void OnActivityChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      var view = (FunctionActivityView)sender;
      view.DataContext = e.NewValue;
    }

    public static readonly DependencyProperty CaptionProperty =
      DependencyProperty.Register("Caption", typeof(string), typeof(FunctionActivityView), new PropertyMetadata(null));

    public FunctionActivity Activity
    {
      get { return (FunctionActivity)GetValue(ActivityProperty); }
      set { SetValue(ActivityProperty, value); }
    }

    public string Caption
    {
      get { return (string)GetValue(CaptionProperty); }
      set { SetValue(CaptionProperty, value); }
    }
    
    protected FunctionActivityView()
    {
      DefaultStyleKey = GetType();
    }
    
    public ObjectConnector FindConnectorByName(string name)
    {
      ObjectConnector connector;
      return _connectors.TryGetValue(name, out connector) ? connector : null;
    }
  }
}
