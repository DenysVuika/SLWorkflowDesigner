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
using System.Windows.Input;
using System.ComponentModel.Composition;

namespace WorkflowDesigner
{
  public partial class MainPage
  {
    private readonly DesignerViewModel _viewmodel;

    public MainPage()
    {
      InitializeComponent();

      DesignerTabs.SelectionChanged += DesignerTabsSelectionChanged;
      CompositionInitializer.SatisfyImports(this);

      _viewmodel = new DesignerViewModel();
      DataContext = _viewmodel;
    }
    
    private void DesignerTabsSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (DesignerTabs.SelectedItem == WorkflowSourceTab && WorkflowSourceTab != null)
      {
        if (designSurface.Controller != null)
          WorkflowSource.Text = designSurface.Controller.WriteXml().ToString();
      }
    }

    private void DoubleClickBehavior_OnDoubleClick(object sender, MouseButtonEventArgs e)
    {
      if (designSurface.Controller == null)
      {
        MessageBox.Show("Please create new workflow...");
        return;
      }

      var panel = sender as StackPanel;
      if (panel == null) return;
      
      var toolboxItem = panel.DataContext as ToolboxItem;
      if (toolboxItem != null)
        designSurface.Controller.DoDrop(toolboxItem, new Point(10, 10));
    }
  }
}
