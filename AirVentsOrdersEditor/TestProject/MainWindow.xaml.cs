using System.Windows;
using OrdersRegistration;

namespace TestProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           //var pdmFilesFolders = 

            new PdmFilesFolders
            {
                OrderName = "AV05 788 6B.sldasm",
                VaultName = "Tets_debag", //"Vents-PDM",
            }.CreateOrder();

            //try
            //{
              
            //}
            //catch (Exception exception)
            //{
            //    MessageBox.Show(exception.StackTrace);
            //}
               
        }
    }
}
