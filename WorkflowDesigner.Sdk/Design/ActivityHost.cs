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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace WorkflowDesigner.Sdk.Design
{
  public class ActivityHost : ContentControl, ISelectable
  {
    public static readonly DependencyProperty CaptionProperty =
      DependencyProperty.Register("Caption", typeof(string), typeof(ActivityHost), new PropertyMetadata("Activity"));

    public static readonly DependencyProperty SelectionBrushProperty =
      DependencyProperty.Register("SelectionBrush", typeof(Brush), typeof(ActivityHost), new PropertyMetadata(new SolidColorBrush(Colors.White)));

    public static readonly DependencyProperty HeaderBorderThicknessProperty =
      DependencyProperty.Register("HeaderBorderThickness", typeof(Thickness), typeof(ActivityHost), new PropertyMetadata(new Thickness(2, 2, 2, 0)));

    private Brush _borderBrush;

    public IList<LinkHost> IncomingLinks { get; private set; }
    public IList<LinkHost> OutgoingLinks { get; private set; }

    private readonly TranslateTransform _position;

    public string Caption
    {
      get { return (string)GetValue(CaptionProperty); }
      set { SetValue(CaptionProperty, value); }
    }

    public Brush SelectionBrush
    {
      get { return (Brush)GetValue(SelectionBrushProperty); }
      set { SetValue(SelectionBrushProperty, value); }
    }

    public Thickness HeaderBorderThickness
    {
      get { return (Thickness)GetValue(HeaderBorderThicknessProperty); }
      set { SetValue(HeaderBorderThicknessProperty, value); }
    }

    public Guid Id { get; private set; }

    public FunctionActivityView View
    {
      get { return (FunctionActivityView)Content; }
    }

    public double Left
    {
      get { return _position.X; }
    }

    public double Right
    {
      get { return _position.X + ActualWidth; }
    }

    public double Top
    {
      get { return _position.Y; }
    }

    public double Bottom
    {
      get { return _position.Y + ActualHeight; }
    }
    
    public static readonly DependencyProperty IsSelectedProperty =
      DependencyProperty.Register("IsSelected", typeof(bool), typeof(ActivityHost), new PropertyMetadata(false, OnIsSelectedChanged));

    public bool IsSelected
    {
      get { return (bool)GetValue(IsSelectedProperty); }
      set { SetValue(IsSelectedProperty, value); }
    }

    private static void OnIsSelectedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      var control = (ActivityHost)sender;
      
      if ((bool)e.NewValue)
        control.DoSelect();
      else
        control.DoUnselect();
    }

    public double PositionX
    {
      get { return _position.X; }
      set
      {
        _position.X = value;
        UpdateLinkRoutes();
        FlushPosition();
      }
    }

    public double PositionY
    {
      get { return _position.Y; }
      set
      {
        _position.Y = value;
        UpdateLinkRoutes();
        FlushPosition();
      }
    }

    public ActivityHost()
    {
      DefaultStyleKey = typeof(ActivityHost);

      IncomingLinks = new List<LinkHost>();
      OutgoingLinks = new List<LinkHost>();

      _position = new TranslateTransform { X = 0, Y = 0 };
      RenderTransform = _position;
    }

    public ActivityHost(Guid id, FunctionActivityView content)
      : this()
    {
      if (content == null) throw new ArgumentNullException("content");
      Id = id;
      Content = content;

      SetBinding(CaptionProperty, new Binding
      {
        Source = content,
        Path = new PropertyPath("Caption"),
        Mode = BindingMode.OneWay,
        TargetNullValue = "[Activity]"
      });
    }

    public ActivityHost(Guid componentId, FunctionActivityView content, Point position)
      : this(componentId, content)
    {
      PositionX = position.X;
      PositionY = position.Y;
    }

    private void UpdateLinkRoutes()
    {
      foreach (var link in IncomingLinks) link.InvalidateArrange();
      foreach (var link in OutgoingLinks) link.InvalidateArrange();
    }

    private void DoSelect()
    {
      _borderBrush = BorderBrush;
      BorderBrush = SelectionBrush;
    }

    private void DoUnselect()
    {
      if (_borderBrush != null)
        BorderBrush = _borderBrush;
    }

    private void FlushPosition()
    {
      View.Activity.SetValue(DesignProperties.PositionX, _position.X);
      View.Activity.SetValue(DesignProperties.PositionY, _position.Y);
    }
  }
}
