using System.Windows;

namespace WorkflowDesigner.Activities.Design.Dialogs
{
  public partial class SqlSourceConfigureConnection
  {
    private readonly SqlSourceActivity _activity;

    public SqlSourceConfigureConnection()
    {
      InitializeComponent();
    }

    public SqlSourceConfigureConnection(SqlSourceActivity activity)
    {
      InitializeComponent();
      _activity = activity;
      ConnectionString.Text = _activity.ConnectionString;
    }

    private void OKButton_Click(object sender, RoutedEventArgs e)
    {
      _activity.ConnectionString = ConnectionString.Text;
      DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = false;
    }
  }
}

