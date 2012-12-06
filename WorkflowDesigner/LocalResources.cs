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
using System.Windows.Markup;

namespace WorkflowDesigner
{
  internal static class LocalResources
  {
    public static class DataTemplates
    {
      private static DataTemplate _applicationDragTemplate;
      private static DataTemplate _applicationDragForbiddenTemplate;

      public static DataTemplate ApplicationDragTemplate
      {
        get
        {
          if (_applicationDragTemplate == null)
            _applicationDragTemplate = XamlReader.Load(@"<DataTemplate xmlns=""http://schemas.microsoft.com/client/2007""><Image Source=""{Binding Icon}"" Stretch=""None"" VerticalAlignment=""Top"" /></DataTemplate>") as DataTemplate;
          return _applicationDragTemplate;
        }
      }

      public static DataTemplate ApplicationDragForbiddenTemplate
      {
        get
        {
          if (_applicationDragForbiddenTemplate == null)
            _applicationDragForbiddenTemplate = XamlReader.Load(string.Format(@"<DataTemplate xmlns=""http://schemas.microsoft.com/client/2007""><Image Source=""{0}"" Stretch=""None"" VerticalAlignment=""Top""/></DataTemplate>", Diagramming.Smaller.Forbidden)) as DataTemplate;
          return _applicationDragForbiddenTemplate;
        }
      }
    }

    internal static class Diagramming
    {
      private const string ComponentPart = "/WorkflowDesigner;component/Resources/32/";
      public const string Wrench = ComponentPart + "wrench.png";

      internal static class Smaller
      {
        private const string ComponentPart = "/WorkflowDesigner;component/Resources/16/";
        public const string Forbidden = ComponentPart + "forbidden.png";
      }
    }
  }
}
