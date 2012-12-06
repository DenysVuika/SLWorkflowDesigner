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
using System.Windows.Controls;
using System.Windows.Media;
using WorkflowDesigner.Sdk;

namespace WorkflowDesigner
{
  public class DesignSurface : Canvas
  {
    private DelegateCommand _clearCommand;
    private DelegateCommand _removeSelectionCommand;
    private DelegateCommand _newFunctionCommand;
    
    private static Point _defaultSize = new Point(3000, 2000);
    public DesignSurfaceController Controller { get; private set; }

    public DelegateCommand ClearCommand
    {
      get { return _clearCommand ?? (_clearCommand = new DelegateCommand(OnClearExecute, CanClear)); }
    }

    public DelegateCommand RemoveSelectionCommand
    {
      get { return _removeSelectionCommand ?? (_removeSelectionCommand = new DelegateCommand(OnRemoveSelection, CanRemoveSelection)); }
    }

    public DelegateCommand NewFunctionCommand
    {
      get { return _newFunctionCommand ?? (_newFunctionCommand = new DelegateCommand(OnNewFunctionExecute)); }
    }

    public DesignSurface()
    {
      Controller = new DesignSurfaceController(this, new FunctionDefinition());
      Width = _defaultSize.X;
      Height = _defaultSize.Y;

      Background = new SolidColorBrush(Colors.LightGray);
    }
    
    private bool CanClear(object parameter)
    {
      return Controller != null;
    }

    private void OnClearExecute(object parameter)
    {
      Controller.Clear();
    }

    private bool CanRemoveSelection(object parameter)
    {
      return Controller != null && Controller.CanRemoveSelection();
    }

    private void OnRemoveSelection(object parameter)
    {
      Controller.RemoveSelection();
    }

    private void OnNewFunctionExecute(object parameter)
    {
      if (Controller != null) Controller.Teardown();
      Controller = new DesignSurfaceController(this, new FunctionDefinition());
      ClearCommand.RaiseCanExecuteChanged();
      ClearCommand.RaiseCanExecuteChanged();
    }
  }
}
