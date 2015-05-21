
using System.Windows;


namespace AirVentsOrderManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            
            MainGrid.Children.Clear();
            MainGrid.Children.Add(new MainUC());
        }

        private void NewOrder_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ExportToExel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditManager_Click(object sender, RoutedEventArgs e)
        {
            var editManagerWindow = new Window();
            editManagerWindow.ShowDialog();
        }
    }
}
