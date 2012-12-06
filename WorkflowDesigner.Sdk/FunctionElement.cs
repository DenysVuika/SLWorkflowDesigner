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
using System.Globalization;
using System.Xml.Linq;
using System.Linq;

namespace WorkflowDesigner.Sdk
{
  public abstract class FunctionElement : ObservableObject
  {
    public static readonly object UnsetValue = new object();
    private readonly Dictionary<string, object> _properties = new Dictionary<string, object>();
    private Guid _id = Guid.NewGuid();
    
    public abstract string LocalName { get; }

    public Guid Id
    {
      get { return _id; }
      set { _id = value; }
    }

    public string TypeName { get; set; }
    
    public object this[string propertyName]
    {
      get
      {
        object result;
        return _properties.TryGetValue(propertyName, out result) ? result : UnsetValue;
      }
    }

    protected FunctionElement()
    {
      TypeName = GetType().Name;
    }

    public object GetValue(string propertyName)
    {
      object result;
      return _properties.TryGetValue(propertyName, out result) ? result : UnsetValue;
    }

    public object GetValue(string propertyName, object defaultValue)
    {
      object result;
      return _properties.TryGetValue(propertyName, out result) ? result : defaultValue;
    }

    public FunctionElement SetValue(string propertyName, object value)
    {
      if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentNullException(propertyName);

      object currentValue;
      if (_properties.TryGetValue(propertyName, out currentValue))
      {
        // Special case for removing properties - providing special UnsetValue
        if (value == UnsetValue)
        {
          _properties.Remove(propertyName);
          return this;
        }
      }
      else currentValue = UnsetValue;

      if (Equals(value, currentValue)) return this;
      _properties[propertyName] = value;
      OnPropertyChanged(propertyName);
      return this;
    }

    public bool IsPropertyDefined(string propertyName)
    {
      return _properties.ContainsKey(propertyName);
    }
    
    public virtual XElement WriteXml()
    {
      return new XElement(
        LocalName, 
        new XAttribute("Id", _id.ToString()),
        new XAttribute("Type", TypeName),
        WriteXmlProperties());
    }

    public virtual void LoadXml(XElement data)
    {
      Id = (Guid)data.Attribute("Id");
      TypeName = (string)data.Attribute("Type");

      var propertyList = data.Element("Properties");
      if (propertyList == null) return;

      ReadXmlProperties(propertyList);
    }

    protected virtual XElement WriteXmlProperties()
    {
      var element = new XElement("Properties");

      foreach (var propertyInfo in _properties.Where(pi => pi.Value != UnsetValue))
      {
        var property = new XElement("Property", new XAttribute("Name", propertyInfo.Key));
          
        if (propertyInfo.Value != null)
        {
          property.Add(
            new XAttribute("Type", propertyInfo.Value.GetType().FullName),
            new XCData(propertyInfo.Value.ToString()));
        }

        element.Add(property);
      }

      return element;
    }

    protected virtual void ReadXmlProperties(XElement properties)
    {
      var props = properties.Elements();

      foreach (var propertyData in props)
      {
        var name = (string)propertyData.Attribute("Name");
        var typeName = (string)propertyData.Attribute("Type");
        var valueString = propertyData.Value;

        var type = Type.GetType(typeName, false, true);

        object value;

        if (type == typeof(Guid))
          value = new Guid(valueString);
        else
          value = Convert.ChangeType(valueString, type, CultureInfo.InvariantCulture);

        SetValue(name, value);
      }
    }
  }
}
