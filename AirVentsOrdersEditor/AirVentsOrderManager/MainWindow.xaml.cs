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

        void NewOrder_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("ljhlj");
        }

        void ExportToExel_Click(object sender, RoutedEventArgs e)
        {

        }

        void EditManager_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
