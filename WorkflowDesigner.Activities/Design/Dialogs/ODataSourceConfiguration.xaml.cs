using System.Windows;

namespace WorkflowDesigner.Activities.Design.Dialogs
{
  public partial class ODataSourceConfiguration
  {
    private readonly ODataSourceActivity _activity;

    public ODataSourceConfiguration()
    {
      InitializeComponent();
    }

    public ODataSourceConfiguration(ODataSourceActivity activity)
    {
      InitializeComponent();
      _activity = activity;

      ServiceName.Text = activity.ServiceName;
      DataServiceUri.Text = activity.DataServiceUri;
      DataFeedName.Text = activity.DataFeedName;
      DataQuery.Text = activity.DataQuery;
    }

    private bool ValidateInput()
    {
      if (string.IsNullOrWhiteSpace(DataServiceUri.Text))
      {
        MessageBox.Show("Service address is missing.");
        return false;
      }

      if (string.IsNullOrWhiteSpace(DataFeedName.Text))
      {
        MessageBox.Show("Data feed name is missing.");
        return false;
      }

      return true;
    }

    private void OkButtonClick(object sender, RoutedEventArgs e)
    {
      if (!ValidateInput()) return;
      
      _activity.ServiceName = string.IsNullOrWhiteSpace(ServiceName.Text) ? DataFeedName.Text : ServiceName.Text;
      _activity.DataServiceUri = DataServiceUri.Text;
      _activity.DataFeedName = DataFeedName.Text;
      _activity.DataQuery = DataQuery.Text;
      DialogResult = true;
    }

    private void CancelButtonClick(object sender, RoutedEventArgs e)
    {
      DialogResult = false;
    }
  }
}

