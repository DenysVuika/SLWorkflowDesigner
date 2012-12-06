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

using WorkflowDesigner.Sdk;

namespace WorkflowDesigner.Activities
{
  [FunctionActivity("ODataSourceActivity")]
  public class ODataSourceActivity : FunctionActivity
  {
    public const string DataServiceUriProperty = "DataServiceUri";
    public const string DataFeedNameProperty = "DataFeedName";
    public const string DataQueryProperty = "DataQuery";
    public const string ServiceNameProperty = "ServiceName";
    public const string ColumnsProperty = "Columns";

    public string DataServiceUri
    {
      get { return (string)GetValue(DataServiceUriProperty, string.Empty); }
      set { SetValue(DataServiceUriProperty, value); }
    }

    public string DataFeedName
    {
      get { return (string)GetValue(DataFeedNameProperty, string.Empty); }
      set { SetValue(DataFeedNameProperty, value); }
    }

    public string DataQuery
    {
      get { return (string)GetValue(DataQueryProperty, string.Empty); }
      set { SetValue(DataQueryProperty, value); }
    }

    // Public name of the data feed
    public string ServiceName
    {
      get { return (string)GetValue(ServiceNameProperty, DataFeedName); }
      set { SetValue(ServiceNameProperty, value); }
    }

    public string Columns
    {
      get { return (string)GetValue(ColumnsProperty, string.Empty); }
      set { SetValue(ColumnsProperty, value); }
    }
    
    public ODataSourceActivity()
    {
      ServiceName = "OGDI: District of Columbia";
      DataServiceUri = "http://ogdi.cloudapp.net/v1/dc/";
      DataFeedName = "BankLocations";
    }
  }
}
