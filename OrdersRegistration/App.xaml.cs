using System;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using OrdersRegistration.Properties;

namespace OrdersRegistration
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public static bool IsDeveloper = false;

        static App()
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
        }

        public static string State
        {
            get
            {
                return String.Format(" База данных: {0}",
                    ConnectionString.Split(';')[1].Split('=')[1]);
            }
        }

        public static string ConnectionString
        {
            get
            {
                return IsDeveloper ? Settings.Default.TestSqlCon : Settings.Default.WorkSqlCon;
            }
        }
    }

    public static class WindowsOfApp
    {
        public static Window ОкноСоздатьЗаказ { get; set; }
        public static Window ОкноРедактироватьЗаказ { get; set; }
        public static Window ОкноРедактировать2Заказ { get; set; }
        public static Window ОкноСохранитьЗаказ { get; set; }
    }

    public static class Заказы
    {
        public static int ВРаботе { get; set; }
        public static int НеОбработаны { get; set; }
        public static int НаПрощете { get; set; }
    }

   


     
}
