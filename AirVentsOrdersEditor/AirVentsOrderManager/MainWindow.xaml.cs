using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AirVentsOrderManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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
            var editManagerWindow = new Window {};
            editManagerWindow.ShowDialog();
        }
    }
}
