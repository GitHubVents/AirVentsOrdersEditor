using System.Collections.Generic;
using System.Windows;

namespace OrdersRegistration.UserControls
{
    /// <summary>
    /// Interaction logic for Assemblage.xaml
    /// </summary>
    public partial class Assemblage
    {

        public Assemblage()
        {
            InitializeComponent();
        }

        public bool? IsEdit { get; set; }

        private void Grid_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (IsEdit == null)
            {
                РедакторЗаказа.Visibility = Visibility.Collapsed;
                Комплектующие.Visibility = Visibility.Collapsed;
            }
            else if ((bool)IsEdit)
            {
                РедакторЗаказа.Visibility = Visibility.Visible;
                Комплектующие.Visibility = Visibility.Collapsed;
            }
            else if (!(bool)IsEdit)
            {
                РедакторЗаказа.Visibility = Visibility.Collapsed;
                Комплектующие.Visibility = Visibility.Visible;
            }
        }

        public class AssemblageParts
        {
            public string Name { get; set; }
            public string Type { get; set; }
        }


        private List<AssemblageParts> AssemblyPartsList()
        {
            return null;
        }

        private void ДобавитьКомплект_Click(object sender, RoutedEventArgs e)
        {
            var newWindow = new Window
            {
                SizeToContent = SizeToContent.WidthAndHeight,
                ResizeMode = ResizeMode.NoResize,
                Title = "Добавление комплектующих",
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = new Assemblage
                {
                    IsEdit = true,
                }
            };
            newWindow.ShowDialog();
        }

        private void РедактироватьКомплект_Click(object sender, RoutedEventArgs e)
        {

        }

        private void УдалитьКомплект_Click(object sender, RoutedEventArgs e)
        {

        }

        private void СохранитьЗапчасть_Click(object sender, RoutedEventArgs e)
        {

        }

        private void РедактироватьЗапчасть_Click(object sender, RoutedEventArgs e)
        {

        }

       

        
    }
}
