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

using System.Windows.Input;
using WorkflowDesigner.Sdk;
using WorkflowDesigner.Sdk.Design;

namespace WorkflowDesigner.Activities.Design
{
  [FunctionActivityView(typeof(DataJoinActivity), "Combine data feeds")]
  public class DataJoinActivityView : FunctionActivityView
  {
    private ICommand _configureColumnsCommand;

    public ICommand ConfigureColumnsCommand
    {
      get { return _configureColumnsCommand ?? (_configureColumnsCommand = new DelegateCommand(OnConfigureColumns)); }
    }


    public DataJoinActivityView()
    {
      Connectors.Add("PIN_LEFT", new ObjectConnector("PIN_LEFT", ObjectConnectorLinkMode.IncomingSingle));
      Connectors.Add("PIN_RIGHT", new ObjectConnector("PIN_RIGHT", ObjectConnectorLinkMode.IncomingSingle));
      Connectors.Add("PIN_OUTPUT", new ObjectConnector("PIN_OUTPUT", ObjectConnectorLinkMode.OutgoingSingle));
    }

    private void OnConfigureColumns(object parameter)
    {
      var activity = Activity as DataJoinActivity;
      if (activity == null) return;

      new Dialogs.DataJoinConfigureColumns(activity).Show();
    }
  }
}
