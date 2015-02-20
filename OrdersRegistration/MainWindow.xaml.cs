using OrdersRegistration.UserControls;

namespace OrdersRegistration
{
    
    public partial class MainWindow
    {
        
        public MainWindow()
        {
            InitializeComponent();

            GridUc2.Children.Add(new OrderRegistrationUc());

            GridUc.Children.Add(new OrderRegistrationKbUc());

            GridCraph.Children.Add(new ChartUc());

            Title = "Учет заказов (" + App.State + ")";

            
        }

        private void Window_Activated_1(object sender, System.EventArgs e)
        {
            GridUc.Children.Clear();
            GridUc.Children.Add(new OrderRegistrationKbUc());
        }
    }
}
