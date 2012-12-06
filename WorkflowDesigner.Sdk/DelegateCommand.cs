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
using System.Windows.Input;

namespace WorkflowDesigner.Sdk
{
  // http://www.silverlightshow.net/items/Silverlight-4-How-to-Command-Control.aspx
  public class DelegateCommand : ICommand
  {
    private readonly Predicate<object> _canExecute;
    private readonly Action<object> _method;
    public event EventHandler CanExecuteChanged;

    public DelegateCommand(Action<object> method)
      : this(method, null)
    {
    }

    public DelegateCommand(Action<object> method, Predicate<object> canExecute)
    {
      _method = method;
      _canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
      return _canExecute == null || _canExecute(parameter);
    }

    public void Execute(object parameter)
    {
      _method.Invoke(parameter);
    }

    protected virtual void OnCanExecuteChanged(EventArgs e)
    {
      var canExecuteChanged = CanExecuteChanged;

      if (canExecuteChanged != null)
        canExecuteChanged(this, e);
    }

    public void RaiseCanExecuteChanged()
    {
      OnCanExecuteChanged(EventArgs.Empty);
    }
  }
}
