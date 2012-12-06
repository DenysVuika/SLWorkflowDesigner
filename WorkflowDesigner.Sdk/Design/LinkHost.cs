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
using System.Windows.Media;
using System.Windows.Shapes;

namespace WorkflowDesigner.Sdk.Design
{
  public class LinkHost : Path, ISelectable
  {
    private bool _isSelected;

    private readonly ObjectConnector _source;
    private readonly ObjectConnector _target;
    private readonly Orientation _orientation;
    private readonly UIElement _surface;
    private readonly FunctionReference _workflowLink;

    private PathGeometry _geometry;
    private PathFigure _figure;
    private BezierSegment _segment;

    private readonly Brush _commonBrush = new SolidColorBrush(Colors.Black);
    private readonly Brush _selectionBrush = new SolidColorBrush(Colors.Red);

    public ObjectConnector Source
    {
      get { return _source; }
    }

    public ObjectConnector Target
    {
      get { return _target; }
    }

    public FunctionReference WorkflowLink
    {
      get { return _workflowLink; }
    }

    public Guid Id
    {
      get { return _workflowLink.Id; }
    }

    public bool IsSelected
    {
      get { return _isSelected; }
      set
      {
        if (_isSelected == value) return;
        _isSelected = value;
        Stroke = (value) ? _selectionBrush : _commonBrush;
      }
    }

    public LinkHost(UIElement surface, FunctionReference workflowLink, ObjectConnector source, ObjectConnector target, Orientation orientation)
    {
      Stroke = _commonBrush;
      StrokeThickness = 2;

      _workflowLink = workflowLink;
      _surface = surface;
      _source = source;
      _target = target;
      _orientation = orientation;

      InitializeGeometry();
    }

    private void InitializeGeometry()
    {
      _geometry = new PathGeometry();
      _figure = new PathFigure();
      _segment = new BezierSegment();

      _figure.Segments.Add(_segment);
      _geometry.Figures.Add(_figure);

      Data = _geometry;
    }

    private void CreatePathData()
    {
      var sourcePos = _source.TransformToVisual(_surface).Transform(new Point(0, 0));
      var targetPos = _target.TransformToVisual(_surface).Transform(new Point(0, 0));

      //bool reverse = Utils.GetLeft(_source) < Utils.GetLeft(_target);
      
      var offsets = (_orientation == Orientation.Horizontal)
      ? new[] { _source.ActualWidth, _source.ActualHeight / 2, 0, _target.ActualHeight / 2 }
      //: new[] { _source.ActualWidth / 2, _source.ActualHeight, _target.ActualWidth / 2, 0 };
      : new[] { _source.ActualWidth / 2, _source.ActualHeight / 2, _target.ActualWidth / 2, _target.ActualHeight / 2 };

      //var values = new[]
      //{
      //  reverse ? Utils.GetLeft(_source) : Utils.GetLeft(_target),
      //  reverse ? Utils.GetTop(_source) : Utils.GetTop(_target),
      //  reverse ? Utils.GetLeft(_target) : Utils.GetLeft(_source),
      //  reverse ? Utils.GetTop(_target) : Utils.GetTop(_source)
      //};

      var values = new[]
      {
        targetPos.X, targetPos.Y,
        sourcePos.X, sourcePos.Y,
      };

      var x1 = values[0] + offsets[0];
      var y1 = values[1] + offsets[1];
      var x2 = values[2] + offsets[2];
      var y2 = values[3] + offsets[3];

      _figure.StartPoint = new Point(x1, y1);

      if (_orientation == Orientation.Horizontal)
      {
        var num = Math.Max(Math.Abs(x2 - x1) / 2.0, 20.0);
        _segment.Point1 = new Point(x1 + num, y1);
        _segment.Point2 = new Point(x2 - num, y2);
        _segment.Point3 = new Point(x2, y2);
      }
      else
      {
        var num = Math.Max(Math.Abs(y2 - y1) / 2.0, 20.0);
        _segment.Point1 = new Point(x1, y1 - num);
        _segment.Point2 = new Point(x2, y2 + num);
        _segment.Point3 = new Point(x2, y2);
      }
    }

    //protected override Size MeasureOverride(Size availableSize)
    //{
    //  return availableSize;
    //}

    protected override Size ArrangeOverride(Size finalSize)
    {
      CreatePathData();
      return finalSize;
    }

    /*
     /// <summary>
    /// Gets a value that represents the <see cref="T:System.Windows.Media.Geometry"/> of the <see cref="T:System.Windows.Shapes.Shape"/>.
    /// </summary>
    /// <value></value>
    /// <returns>The <see cref="T:System.Windows.Media.Geometry"/> of the <see cref="T:System.Windows.Shapes.Shape"/>.</returns>
    protected override Geometry DefiningGeometry
    {
      get
      {
        if (GeometrySource != Geometry.Empty)
          return GeometrySource;

        double num;
        if (Orientation == Orientation.Horizontal)
        {
          num = Math.Max((double)(Math.Abs((double)(this.EndPoint.X - this.StartPoint.X)) / 2.0), (double)20.0);
          figure.StartPoint = this.StartPoint;
          segment.Point1 = new Point(this.StartPoint.X + num, this.StartPoint.Y);
          segment.Point2 = new Point(this.EndPoint.X - num, this.EndPoint.Y);
          segment.Point3 = this.EndPoint;
        }
        else
        {
          num = Math.Max((double)(Math.Abs((double)(this.EndPoint.Y - this.StartPoint.Y)) / 2.0), (double)20.0);
          figure.StartPoint = this.StartPoint;
          segment.Point1 = new Point(this.StartPoint.X, this.StartPoint.Y - num);
          segment.Point2 = new Point(this.EndPoint.X, this.EndPoint.Y + num);
          segment.Point3 = this.EndPoint;
        }
        return geometry;
      }
    }
    */

    
  }
}
