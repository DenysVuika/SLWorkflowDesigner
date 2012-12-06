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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Linq;
using System.Linq;

namespace WorkflowDesigner.Sdk
{
  public class FunctionDefinition : FunctionElement, ISupportInitialize
  {
    private bool _isInitializing;

    public override string LocalName
    {
      get { return "Function"; }
    }

    private readonly ObservableCollection<FunctionActivity> _activities = new ObservableCollection<FunctionActivity>();
    private readonly ObservableCollection<FunctionReference> _references = new ObservableCollection<FunctionReference>();

    public ReadOnlyObservableCollection<FunctionActivity> Activities
    {
      get { return new ReadOnlyObservableCollection<FunctionActivity>(_activities); }
    }

    public ReadOnlyObservableCollection<FunctionReference> References
    {
      get { return new ReadOnlyObservableCollection<FunctionReference>(_references); }
    }

    public event EventHandler<FunctionActivityEventArgs> ActivityAdded;
    public event EventHandler<FunctionActivityEventArgs> ActivityRemoved;
    public event EventHandler<FunctionReferenceEventArgs> ReferenceAdded;
    public event EventHandler<FunctionReferenceEventArgs> ReferenceRemoved;
    
    public void AddItem(FunctionActivity item)
    {
      if (item == null || _activities.Contains(item)) return;

      _activities.Add(item);
      OnActivityAdded(item);
    }

    public void AddItem(FunctionReference item)
    {
      if (item == null) return;
      if (_references.Contains(item)) return;

      _references.Add(item);
      OnLinkAdded(item);
    }

    public void RemoveItem(FunctionActivity item)
    {
      if (item == null || !_activities.Contains(item)) return;
      
      _activities.Remove(item);
      OnActivityRemoved(item);
    }

    public void RemoveItem(FunctionReference item)
    {
      if (item == null || !_references.Contains(item)) return;

      _references.Remove(item);
      OnLinkRemoved(item);
    }

    public override void LoadXml(XElement data)
    {
      if (data != null)
      {
        Id = (Guid)data.Attribute("Id");
        TypeName = (string)data.Attribute("Type");
      }
      else
      {
        Id = Guid.NewGuid();
      }
    }

    public override XElement WriteXml()
    {
      var element = new XElement(LocalName,
        new XAttribute("Id", Id.ToString()),
        new XAttribute("Type", TypeName),
        WriteActivitiesXml("Activities"),
        WriteLinksXml("References"));

      return element;
    }

    private XElement WriteActivitiesXml(string localName)
    {
      return new XElement(localName, _activities.Select(a => a.WriteXml()));
    }

    private XElement WriteLinksXml(string localName)
    {
      return new XElement(localName, _references.Select(l => l.WriteXml()));
    }

    private void OnActivityAdded(FunctionActivity element)
    {
      if (_isInitializing) return;

      var handler = ActivityAdded;
      if (handler != null) handler(this, new FunctionActivityEventArgs(element));
    }

    private void OnActivityRemoved(FunctionActivity element)
    {
      if (_isInitializing) return;

      var handler = ActivityRemoved;
      if (handler != null) handler(this, new FunctionActivityEventArgs(element));
    }

    private void OnLinkAdded(FunctionReference element)
    {
      if (_isInitializing) return;

      var handler = ReferenceAdded;
      if (handler != null) handler(this, new FunctionReferenceEventArgs(element));
    }

    private void OnLinkRemoved(FunctionReference element)
    {
      if (_isInitializing) return;

      var handler = ReferenceRemoved;
      if (handler != null) handler(this, new FunctionReferenceEventArgs(element));
    }

    public void BeginInit()
    {
      _isInitializing = true;
    }

    public void EndInit()
    {
      _isInitializing = false;
    }
  }
}
