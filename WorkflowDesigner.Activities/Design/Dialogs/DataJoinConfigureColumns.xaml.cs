using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace WorkflowDesigner.Activities.Design.Dialogs
{
  public partial class DataJoinConfigureColumns
  {
    private readonly DataJoinActivity _activity;
    private readonly ObservableCollection<ColumnMapping> _mappings = new ObservableCollection<ColumnMapping>();

    public DataJoinConfigureColumns()
    {
      InitializeComponent();
    }

    public DataJoinConfigureColumns(DataJoinActivity activity)
    {
      InitializeComponent();
      _activity = activity;

      foreach (var mapping in ParseColumnMappings(activity))
        _mappings.Add(mapping);

      mappingsGrid.ItemsSource = _mappings;
    }

    private void OkButtonClick(object sender, RoutedEventArgs e)
    {
      _activity.Columns = SaveColumnMappings(_mappings);
      DialogResult = true;
    }

    private void CancelButtonClick(object sender, RoutedEventArgs e)
    {
      DialogResult = false;
    }

    private static string SaveColumnMappings(IEnumerable<ColumnMapping> mappings)
    {
      var result = mappings
        .Where(m => !string.IsNullOrWhiteSpace(m.Left) && !string.IsNullOrWhiteSpace(m.Right))
        .Select(m => m.Left + "=" + m.Right);

      return string.Join(";", result);
    }

    private static IEnumerable<ColumnMapping> ParseColumnMappings(DataJoinActivity activity)
    {
      if (activity == null) yield break;
      if (string.IsNullOrWhiteSpace(activity.Columns)) yield break;

      var mappings = activity.Columns.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

      foreach (var pair in mappings
        .Select(mapping => mapping.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries))
        .Where(pair => pair.Length == 2))
        yield return new ColumnMapping(pair[0].Trim(), pair[1].Trim());
    }

    private void DoAddNewRow(object sender, RoutedEventArgs e)
    {
      var mapping = new ColumnMapping();
      _mappings.Add(mapping);

      mappingsGrid.SelectedItem = mapping;
      mappingsGrid.CurrentColumn = mappingsGrid.Columns[0];
      mappingsGrid.UpdateLayout();

      // Denis: using both (non)dispatchered ways does not help
      // there's some sort of issue with scrollbar as it does not scroll properly
      // user needs to move scrollbar to the bottom in order it to work properly next times

      //mappingsGrid.Dispatcher.BeginInvoke(() =>
      //{
      mappingsGrid.Focus();
      mappingsGrid.ScrollIntoView(mapping, mappingsGrid.Columns[0]);
      mappingsGrid.BeginEdit();
      //});
    }

    private void DoRemoveRow(object sender, RoutedEventArgs e)
    {
      var mapping = mappingsGrid.SelectedItem as ColumnMapping;
      if (mapping == null) return;
      _mappings.Remove(mapping);
    }
  }
}

