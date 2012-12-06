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
using System.Windows.Controls;

namespace WorkflowDesigner.Sdk.Design
{
  public class ObjectConnector : Control
  {
    public static readonly DependencyProperty LinkModeProperty =
      DependencyProperty.Register("LinkMode", typeof(ObjectConnectorLinkMode), typeof(ObjectConnector), new PropertyMetadata(ObjectConnectorLinkMode.None));

    public ObjectConnectorLinkMode LinkMode
    {
      get { return (ObjectConnectorLinkMode)GetValue(LinkModeProperty); }
      set { SetValue(LinkModeProperty, value); }
    }

    [Obsolete("Temporary hack")]
    public bool HasIncomingConnections { get; set; }

    [Obsolete("Temporary hack")]
    public bool HasOutgoingConnections { get; set; }

    public string PinName { get; set; }

    public ActivityHost DesignerControl
    {
      get { return Utils.FindParent<ActivityHost>(this); }
    }

    public ObjectConnector()
    {
      DefaultStyleKey = typeof(ObjectConnector);
    }

    public ObjectConnector(string pinName, ObjectConnectorLinkMode linkMode)
      : this()
    {
      PinName = pinName;
      LinkMode = linkMode;
    }
  }
}
