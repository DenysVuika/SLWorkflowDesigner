using WorkflowDesigner.Sdk;

namespace WorkflowDesigner.Activities.Design.Dialogs
{
  public sealed class ColumnMapping : ObservableObject
  {
    private string _left = string.Empty;
    private string _right = string.Empty;

    public string Left
    {
      get { return _left; }
      set
      {
        if (_left == value) return;
        _left = value;
        OnPropertyChanged("Left");
      }
    }

    public string Right
    {
      get { return _right; }
      set
      {
        if (_right == value) return;
        _right = value;
        OnPropertyChanged("Right");
      }
    }

    public ColumnMapping()
    {
      
    }

    public ColumnMapping(string left, string right)
    {
      _left = left;
      _right = right;
    }
  }
}
