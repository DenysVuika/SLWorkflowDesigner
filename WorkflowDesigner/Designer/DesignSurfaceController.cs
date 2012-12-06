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
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;
using WorkflowDesigner.Sdk;
using WorkflowDesigner.Sdk.Design;
using System.Linq;

namespace WorkflowDesigner
{
  public class DesignSurfaceController
  {
    private readonly DesignSurface _surface;

    private bool _dragStarted;
    private Point _clickPosition;
    private Path _path;
    private LineGeometry _lineGeometry;
    private ObjectConnector _sourceConnector;

    public ISelectable SelectedItemHost { get; private set; }

    private static readonly Func<ObjectConnector, bool> CanStartConnection = connector =>
    {
      if (connector.LinkMode == ObjectConnectorLinkMode.OutgoingMultiple) return true;
      if (connector.LinkMode == ObjectConnectorLinkMode.OutgoingSingle)
        //return connector.DesignerControl.OutgoingLinks.Count == 0;
        return !connector.HasOutgoingConnections;
      return false;
    };

    private static readonly Func<ObjectConnector, bool> CanFinishConnection = connector =>
    {
      if (connector.LinkMode == ObjectConnectorLinkMode.IncomingMultiple) return true;
      if (connector.LinkMode == ObjectConnectorLinkMode.IncomingSingle)
        //return connector.DesignerControl.IncomingLinks.Count == 0;
        return !connector.HasIncomingConnections;
      return false;
    };

    [ImportMany(AllowRecomposition = true)]
    public List<ExportFactory<FunctionActivity, IFunctionActivityMetadata>> ActivityFactories { get; set; }

    [ImportMany(AllowRecomposition = true)]
    public List<ExportFactory<FunctionActivityView, IFunctionActivityViewMetadata>> ActivityViewFactories { get; set; }

    public DesignSurface Surface
    {
      get { return _surface; }
    }

    public FunctionDefinition Workflow { get; private set; }

    public DesignSurfaceController(DesignSurface surface, FunctionDefinition workflow)
    {
      CompositionInitializer.SatisfyImports(this);

      _surface = surface;
      Workflow = workflow;

      InitializeWorkflowDefinition();
      InitializeSurface();
      CreateDefaultOutputActivity();
    }

    private void CreateDefaultOutputActivity()
    {
      var factory = ActivityFactories.FirstOrDefault(f => f.Metadata.TypeName == "OutputParameterActivity");
      if (factory == null)
      {
        
      }
      //Debug.Assert(factory != null, "Default factory for Output activity not found.");
      if (factory == null) return;

      var output = factory.CreateExport().Value;
      output
        .SetValue(DesignProperties.PositionX, 10)
        .SetValue(DesignProperties.PositionY, 10);
      Workflow.AddItem(output);
    }

    public void Teardown()
    {
      _surface.MouseLeftButtonDown -= OnSurfaceMouseLeftButtonDown;
      _surface.MouseMove -= OnSurfaceMouseMove;
      _surface.MouseLeftButtonUp -= OnSurfaceMouseLeftButtonUp;

      Workflow.ActivityAdded -= OnActivityAdded;
      Workflow.ReferenceAdded -= OnLinkAdded;
      Workflow.ReferenceAdded -= OnLinkAdded;
      Workflow.ReferenceRemoved -= OnLinkRemoved;
    }

    private void InitializeSurface()
    {
      _surface.MouseLeftButtonDown += OnSurfaceMouseLeftButtonDown;
      _surface.MouseMove += OnSurfaceMouseMove;
      _surface.MouseLeftButtonUp += OnSurfaceMouseLeftButtonUp;
    }
    
    private void OnSurfaceMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      var sourceConnector = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(null), _surface).OfType<ObjectConnector>().FirstOrDefault();
      if (sourceConnector != null && CanStartConnection(sourceConnector))
      {
        _sourceConnector = sourceConnector;
        _lineGeometry = new LineGeometry();
        _lineGeometry.StartPoint = _lineGeometry.EndPoint = e.GetPosition(_surface);

        _path = new Path
        {
          Stroke = new SolidColorBrush(Colors.Black),
          StrokeThickness = 2,
          Data = _lineGeometry
        };

        _surface.Children.Add(_path);

        return;
      }

      var designerControl = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(null), _surface).OfType<ActivityHost>().FirstOrDefault();
      if (designerControl != null)
      {
        SelectItem(designerControl);

        _clickPosition = e.GetPosition(designerControl);
        _dragStarted = true;
        return;
      }

      var link = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(null), _surface).OfType<LinkHost>().FirstOrDefault();
      if (link != null)
      {
        SelectItem(link);
        return;
      }

      SelectItem(null);
    }

    private void OnSurfaceMouseMove(object sender, MouseEventArgs e)
    {
      if (_lineGeometry != null)
      {
        _lineGeometry.EndPoint = e.GetPosition(_surface);
      }

      if (_dragStarted && SelectedItemHost != null)
      {
        var designerControl = SelectedItemHost as ActivityHost;
        if (designerControl != null)
        {
          var newX = e.GetPosition(_surface).X - _clickPosition.X;
          var newY = e.GetPosition(_surface).Y - _clickPosition.Y;
          designerControl.PositionX = newX > 0 ? newX : 0;
          designerControl.PositionY = newY > 0 ? newY : 0;
        }
      }
    }
    
    private void OnSurfaceMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (_sourceConnector != null)
      {
        var targetConnector = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(null), _surface).OfType<ObjectConnector>().FirstOrDefault();
        if (targetConnector != null
          && targetConnector != _sourceConnector
          && targetConnector.DesignerControl != _sourceConnector.DesignerControl
          && CanFinishConnection(targetConnector))
        {
          var sourceId = _sourceConnector.DesignerControl.Id;
          var targetId = targetConnector.DesignerControl.Id;

          var workflowLink = new FunctionReference
          {
            SourceId = sourceId,
            SourcePin = _sourceConnector.PinName,
            TargetId = targetId,
            TargetPin = targetConnector.PinName
          };

          Workflow.AddItem(workflowLink);
        }
      }

      RemoveConnectionLine();

      if (SelectedItemHost != null)
      {
        SelectedItemHost.ReleaseMouseCapture();
        _dragStarted = false;
      }
    }

    private void InitializeWorkflowDefinition()
    {
      _surface.Children.Clear();

      foreach (var activity in Workflow.Activities)
        ProcessNewActivity(activity);

      foreach (var link in Workflow.References)
        ProcessNewLink(link);

      Workflow.ActivityAdded += OnActivityAdded;
      Workflow.ActivityRemoved += OnActivityRemoved;
      Workflow.ReferenceAdded += OnLinkAdded;
      Workflow.ReferenceRemoved += OnLinkRemoved;
    }

    private void TeardownWorkflowDefinition()
    {
      SelectItem(null);
      Workflow.ActivityAdded -= OnActivityAdded;
      Workflow.ReferenceAdded -= OnLinkAdded;
      Workflow.ReferenceAdded -= OnLinkAdded;
      Workflow.ReferenceRemoved -= OnLinkRemoved;
    }

    private void OnActivityAdded(object sender, FunctionActivityEventArgs e)
    {
      ProcessNewActivity(e.Item);
    }

    private void OnActivityRemoved(object sender, FunctionActivityEventArgs e)
    {
      ProcessDeletedActivity(e.Item);
    }

    private void OnLinkAdded(object sender, FunctionReferenceEventArgs e)
    {
      ProcessNewLink(e.Item);
    }

    private void OnLinkRemoved(object sender, FunctionReferenceEventArgs e)
    {
      ProcessDeletedLink(e.Item);
    }

    private void ProcessNewActivity(FunctionActivity activity)
    {
      if (activity == null) return;

      var viewFactory = ActivityViewFactories.FirstOrDefault(f => f.Metadata.TargetType == activity.GetType());
      if (viewFactory == null) return;

      var view = viewFactory.CreateExport().Value;
      view.Caption = viewFactory.Metadata.Caption;
      view.Activity = activity;

      var position = (activity.IsPropertyDefined(DesignProperties.PositionX) && activity.IsPropertyDefined(DesignProperties.PositionY))
        ? new Point(Convert.ToDouble(activity.GetValue(DesignProperties.PositionX)), Convert.ToDouble(activity.GetValue(DesignProperties.PositionY)))
        : new Point(0, 0);

      var host = new ActivityHost(activity.Id, view, position);
      _surface.Children.Add(host);
    }

    private void ProcessDeletedActivity(FunctionActivity activity)
    {
      if (activity == null) return;

      var host = _surface.Children.OfType<ActivityHost>().FirstOrDefault(h => h.Id == activity.Id);
      if (host == null) return;

      host.IncomingLinks.ToList().ForEach(incoming => RemoveItem(incoming.WorkflowLink));
      host.OutgoingLinks.ToList().ForEach(outgoing => RemoveItem(outgoing.WorkflowLink));
      
      _surface.Children.Remove(host);
    }

    private void ProcessNewLink(FunctionReference link)
    {
      if (link == null) return;

      var sourceHost = _surface.Children.OfType<ActivityHost>().FirstOrDefault(c => c.Id == link.SourceId);
      var targetHost = _surface.Children.OfType<ActivityHost>().FirstOrDefault(c => c.Id == link.TargetId);

      var sourceConnector = sourceHost.View.FindConnectorByName(link.SourcePin);
      var targetConnector = targetHost.View.FindConnectorByName(link.TargetPin);

      var conn = new LinkHost(_surface, link, sourceConnector, targetConnector, Orientation.Vertical);
      // TODO: outgoing links should be also kept by connector?
      sourceHost.OutgoingLinks.Add(conn);
      targetHost.OutgoingLinks.Add(conn);
      // TODO: temp hack
      sourceConnector.HasOutgoingConnections = true;
      targetConnector.HasIncomingConnections = true;

      _surface.Children.Add(conn);
    }

    private void ProcessDeletedLink(FunctionReference link)
    {
      if (link == null) return;

      var host = _surface.Children.OfType<LinkHost>().FirstOrDefault(l => l.Id == link.Id);
      if (host == null) return;

      // TODO: temp hack
      host.Source.HasOutgoingConnections = false;
      host.Target.HasIncomingConnections = false;

      // TODO: Denis: find out more structured way
      host.Source.DesignerControl.OutgoingLinks.Remove(host);
      host.Target.DesignerControl.IncomingLinks.Remove(host);

      _surface.Children.Remove(host);
    }
    
    protected virtual bool CanDrop(ToolboxItem request, Point dropPoint)
    {
      // This should be supported by actual controller implementation
      //return false;
      return true;
    }

    protected internal virtual bool DoDrop(ToolboxItem request, Point dropPoint)
    {
      var activity = Activator.CreateInstance(request.ActivityType) as FunctionActivity;
      if (activity == null) return false;

      activity
        .SetValue(DesignProperties.PositionX, dropPoint.X)
        .SetValue(DesignProperties.PositionY, dropPoint.Y);

      Workflow.AddItem(activity);
      return true;
    }

    private void SelectItem(ISelectable item)
    {
      if (SelectedItemHost == item) return;

      if (SelectedItemHost != null)
      {
        SelectedItemHost.IsSelected = false;
        SelectedItemHost.ReleaseMouseCapture();
        SelectedItemHost = null;
      }

      if (item != null)
      {
        SelectedItemHost = item;
        SelectedItemHost.IsSelected = true;
        SelectedItemHost.CaptureMouse();
      }

      _surface.RemoveSelectionCommand.RaiseCanExecuteChanged();
      //SelectionService.SelectedComponent = control;
    }

    private void RemoveConnectionLine()
    {
      if (_lineGeometry == null) return;
      _surface.Children.Remove(_path);
      _path = null;
      _lineGeometry = null;
    }

    public bool CanRemoveSelection()
    {
      if (SelectedItemHost == null) return false;

      var host = SelectedItemHost as ActivityHost;
      return host == null || host.View.Activity.IsRemovable;
    }

    public void RemoveSelection()
    {
      if (SelectedItemHost != null)
        RemoveItem(SelectedItemHost);
    }

    public void RemoveItem(ISelectable selectable)
    {
      if (selectable == null) return;
      var controlHost = selectable as ActivityHost;
      if (controlHost != null)
      {
        // TODO: Denis: there should be some rule engine
        //if (controlHost.View.Activity is OutputParameterActivity) return;
        if (!controlHost.View.Activity.IsRemovable) return;
        Workflow.RemoveItem(controlHost.View.Activity);
        return;
      }

      var link = selectable as LinkHost;
      if (link != null)
      {
        Workflow.RemoveItem(link.WorkflowLink);
      }
    }

    public void RemoveItem(FunctionReference item)
    {
      if (item == null) return;
      Workflow.RemoveItem(item);
    }

    public void RemoveItem(FunctionActivity item)
    {
      if (item == null) return;
      Workflow.RemoveItem(item);
    }

    public XElement WriteXml()
    {
      return Workflow.WriteXml();
    }

    public void LoadXml(XElement data)
    {
      TeardownWorkflowDefinition();

      var definition = new FunctionDefinition();
      definition.LoadXml(data);

      if (data != null)
      {
        definition.BeginInit();

        var activities = data.Element("Activities").Elements();
        foreach (var activity in activities.Select(ParseActivity).Where(a => a != null))
          definition.AddItem(activity);

        var links = data.Element("References").Elements();
        foreach (var link in links.Select(ParseLink).Where(l => l != null))
          definition.AddItem(link);

        definition.EndInit();
      }

      Workflow = definition;
      InitializeWorkflowDefinition();
    }

    public void Clear()
    {
      LoadXml(null);
      CreateDefaultOutputActivity();
    }

    private FunctionActivity ParseActivity(XElement data)
    {
      var typeName = (string)data.Attribute("Type");
      if (string.IsNullOrEmpty(typeName)) return null;

      var factory = ActivityFactories.FirstOrDefault(f => f.Metadata.TypeName == typeName);
      if (factory == null) return null;

      var activity = factory.CreateExport().Value;
      activity.LoadXml(data);

      return activity;
    }

    private static FunctionReference ParseLink(XElement data)
    {
      var link = new FunctionReference();
      link.LoadXml(data);
      return link;
    }
  }
}
