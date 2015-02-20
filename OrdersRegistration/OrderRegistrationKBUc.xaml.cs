using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace OrdersRegistration
{
    public partial class OrderRegistrationKbUc
    {
        public int OrderId { get; set; }
        public bool IsEditMode { get; set; }
        public bool SavingOrder { get; set; }
        public bool EditOrder { get; set; }

        public string OrderName { get; set; }
        public string ConstructorName { get; set; }

        public DateTime RequiredDate { get; set; }
        public DateTime ShippedDate { get; set; }
        public DateTime CompletionDate { get; set; }
        //public DateTime? FinishCompletionDate { get; set; }
        public string FinishCompletionDate { get; set; }

        public int OrderDetailId { get; set; }

        private void Grid_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (IsEditMode)
            {
                ТаблицаЗаказов.Visibility = Visibility.Collapsed;
            }
            else
            {
                РедакторЗаказа.Visibility = Visibility.Collapsed;
            }
            if (SavingOrder)
            {
                РежимРедактированияЗаказа.Header = "Сохранение заказа";
                Редактировать.Visibility = Visibility.Collapsed;
            }
            if (EditOrder)
            {
                РежимРедактированияЗаказа.Header = "Редактирование заказа";
                СоздатьЗаказ1.Visibility = Visibility.Collapsed;
                ВнутреннийНомерЗаказа.Visibility = Visibility.Collapsed;
                НомерЗаказа.Text = "rfg";
            }
            ЗаказовТаблица.ItemsSource = OrdersConstructorDataList();

            #region Авто

            ДатаПоступленияЗаказа.SelectedDate = RequiredDate.Date;
            ДатаОтгрузкиЗаказа.SelectedDate = ShippedDate.Date;
            ПланируемаяСдачаКд.SelectedDate = CompletionDate.Date;

            ФактическаяСдачаКд.SelectedDate = null;
            
            Конструктор.Text = ConstructorName;

            #endregion

             switch (Wizard)
            {
                case "00":
                    Назад.Visibility = Visibility.Collapsed;
                    Вперед.Visibility = Visibility.Collapsed;
                    Отмена.Visibility = Visibility.Collapsed;
                    СоздатьЗаказ1.Visibility = Visibility.Collapsed;
                    Редактировать.Visibility = Visibility.Visible;
                    ФактическаяСдачаКд.SelectedDate = Convert.ToDateTime(FinishCompletionDate);
                    break;
                case "01":
                    Назад.IsEnabled = false;
                    Вперед.IsEnabled = false;
                    Отмена.IsEnabled = true;
                    break;
                case "10":
                    Назад.IsEnabled = true;
                    Вперед.IsEnabled = false;
                    Отмена.IsEnabled = true;
                    break;
                case "11":
                    Назад.IsEnabled = true;
                    Вперед.IsEnabled = true;
                    Отмена.IsEnabled = true;
                    break;
                default:
                    Назад.Visibility = Visibility.Visible;
                    Вперед.Visibility = Visibility.Visible;
                    Отмена.Visibility = Visibility.Visible;
                    СоздатьЗаказ1.Visibility = Visibility.Collapsed;
                    Редактировать.Visibility = Visibility.Collapsed;

                    Вперед.IsEnabled = false;
                    break;
            }

        }

        public string Wizard { get; set; }


        private void EditOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (ЗаказовТаблица.SelectedItem == null)
            {
                MessageBox.Show("Выберете заказ для редактирования.", "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            var selectedItem = (OrdersConstructorDataClass)ЗаказовТаблица.SelectedItem;
            var constrName = selectedItem.Expr2.Substring(0, selectedItem.Expr2.IndexOf(' '));
            //MessageBox.Show(constrName);

            var newWindow = new Window
            {
                SizeToContent = SizeToContent.WidthAndHeight,
                ResizeMode = ResizeMode.NoResize,
                Title = String.Format("Подбор №{0}, {1} ({2})", selectedItem.ProjectNumber, selectedItem.Type, selectedItem.Description),
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = new OrderRegistrationKbUc
                {
                    IsEditMode = true,
                    EditOrder = true,

                    OrderId = selectedItem.OrderDetailId,
                    RequiredDate = selectedItem.RequiredDate,
                    ShippedDate = selectedItem.ShippedDate,
                    CompletionDate = selectedItem.CompletionDate,
                    FinishCompletionDate = selectedItem.FinishCompletionDate,
                    ConstructorName = constrName,
                    Wizard = "00"
                }
            };
            newWindow.ShowDialog();
        }

        public OrderRegistrationKbUc()
        {
            InitializeComponent();

            ЗаказовТаблица.ColumnHeaderHeight = 25;


            #region Конструктора

            Конструктор.ItemsSource = ((IListSource)ConstructorsList()).GetList();
            Конструктор.DisplayMemberPath = "LastName";
            Конструктор.SelectedValuePath = "ConstructorID";
            Конструктор.SelectedIndex = 0;

            #endregion
            
        }

        #region Data

        static DataTable ConstructorsList()
        {
            var constructorsList = new DataTable();
            using (var con = new SqlConnection(App.ConnectionString))
            {
                try
                {
                    con.Open();
                    var sqlCommand = new SqlCommand("SELECT * FROM  HumanResources.Constructor ORDER BY LastName", con);
                    var sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    sqlDataAdapter.Fill(constructorsList);
                    sqlDataAdapter.Dispose();
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Ошибка выгрузки данных из базы");
                }
                finally
                {
                    con.Close();
                }
            }
            return constructorsList;
        }

        #region DataTable From SQL to List<Class>

        static DataTable OrdersConstructorTable()
        {
            var ordersList = new DataTable();
            using (var con = new SqlConnection(App.ConnectionString))
            { 
                try
                {
                    con.Open();
                    var sqlCommand = new SqlCommand(@"SELECT HumanResources.Employee.LastName +' '+ LEFT (HumanResources.Employee.FirsrtName, 1)+'.', AirVents.[Order].ProjectNumber, AirVents.[Order].Date, AirVents.StandardSize.Type +' '+ CAST(AirVents.OrderDetails.InternalNumber as NVARCHAR),
                      AirVents.Profil.Description, AirVents.OrderDetails.RequiredDate, AirVents.OrderDetails.ShippedDate, 
                      AirVents.OrderDetails.CompletionDate, AirVents.OrderDetails.FinishCompletionDate, HumanResources.Constructor.LastName +' '+ LEFT (HumanResources.Constructor.FirstName, 1)+'.', AirVents.OrderDetails.OrderDetailID
                      FROM AirVents.[Order] INNER JOIN
                      AirVents.DimensionType ON AirVents.[Order].DimensionTypeID = AirVents.DimensionType.DimensionTypeID INNER JOIN
                      HumanResources.Employee ON AirVents.[Order].empid = HumanResources.Employee.EmpID INNER JOIN
                      AirVents.OrderDetails ON AirVents.[Order].OrderID = AirVents.OrderDetails.OrderID INNER JOIN
                      HumanResources.Constructor ON AirVents.OrderDetails.ConstructorID = HumanResources.Constructor.ConstructorID AND 
                      AirVents.OrderDetails.ConstructorID = HumanResources.Constructor.ConstructorID INNER JOIN
                      AirVents.Profil ON AirVents.DimensionType.ProfilID = AirVents.Profil.ProfilID AND AirVents.DimensionType.ProfilID = AirVents.Profil.ProfilID INNER JOIN
                      AirVents.StandardSize ON AirVents.DimensionType.SizeID = AirVents.StandardSize.SizeID AND AirVents.DimensionType.SizeID = AirVents.StandardSize.SizeID", con);
                    var sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    sqlDataAdapter.Fill(ordersList);
                    sqlDataAdapter.Dispose();
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Ошибка выгрузки данных из базы");
                }
                finally
                {
                    con.Close();
                }
            }
            return ordersList;
        }

        public class OrdersConstructorDataClass
        {
            public string Expr1 { get; set; }
            public string Expr2 { get; set; }
            public string ProjectNumber { get; set; }
            public DateTime Date { get; set; }
            public string Type { get; set; }
            public string Description { get; set; }
            public string InternalNumber { get; set; }
            public string Order { get; set; }
            public DateTime RequiredDate { get; set; }
            public DateTime ShippedDate { get; set; }
            public DateTime CompletionDate { get; set; }
            //public DateTime? FinishCompletionDate { get; set; }
            public string FinishCompletionDate { get; set; }
            public int OrderDetailId { get; set; }
        }
        
        private IEnumerable<OrdersConstructorDataClass> OrdersConstructorDataList()
        {
            var list = (from DataRow row in OrdersConstructorTable().Rows
                   select new OrdersConstructorDataClass
                   {
                       Expr1 = row["Column1"].ToString(),
                       Expr2 = row["Column3"].ToString(),
                       Order = row["Column2"].ToString(),
                       OrderDetailId = Convert.ToInt32(row["OrderDetailID"]),
                       ProjectNumber = row["ProjectNumber"].ToString(),
                       Date = Convert.ToDateTime(row["Date"]),
                      // Type = row["Type"].ToString(),
                       Description = row["Description"].ToString(),
                      // InternalNumber = row["InternalNumber"].ToString(),
                       RequiredDate = Convert.ToDateTime(row["RequiredDate"]),
                       ShippedDate = Convert.ToDateTime(row["ShippedDate"]),
                       CompletionDate = Convert.ToDateTime(row["CompletionDate"]),
                       FinishCompletionDate = row["FinishCompletionDate"] !=   DBNull.Value ?  Convert.ToDateTime(row["FinishCompletionDate"]).ToShortDateString(): "",
                   }).ToList();
            return list.OrderByDescending(x => x.Date);
        }
        
        #region После обновления DataGrid спуститься вниз

        private void СоздатьЗаказ_Click(object sender, RoutedEventArgs e)
        {
            if (!AirVents_AddOrderDetail()) return;
            ЗаказовТаблица.ItemsSource = OrdersConstructorDataList();
            //if (ЗаказовТаблица.Items.Count > 0)
            //{
            //    var border = VisualTreeHelper.GetChild(ЗаказовТаблица, 0) as Decorator;
            //    if (border != null)
            //    {
            //        var scroll = border.Child as ScrollViewer;
            //        if (scroll != null) scroll.ScrollToEnd();
            //    }
            //}
            var parent = Window.GetWindow(this);
            if (parent != null) parent.Close();
        }

        #endregion


        #endregion

       
        #endregion


        bool AirVents_AddOrderDetail()
        {
            if (НомерЗаказа.Text == "")
            {
                MessageBox.Show("Введите внутренний номер!", "", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return false;
            }

            foreach (var ordersConstructorDataClass in OrdersConstructorDataList())
                    {
                        if (ordersConstructorDataClass.InternalNumber == НомерЗаказа.Text)
                        {
                            if ( MessageBox.Show("Внести изменения в заказ?","Данный заказ уже в базе", MessageBoxButton.YesNoCancel, MessageBoxImage.Question) != MessageBoxResult.Yes)
                            {
                                return false;
                            }
                        }
                    }


            using (var con = new SqlConnection(App.ConnectionString))
            {
                try
                {
                    con.Open();
                    var sqlCommand = new SqlCommand("AirVent_Add_OrderDetails", con) { CommandType = CommandType.StoredProcedure };

                    sqlCommand.Parameters.Add("@InternalNumber", SqlDbType.Char).Value = НомерЗаказа.Text;

                    sqlCommand.Parameters.Add("@RequiredDate", SqlDbType.Date).Value = ДатаПоступленияЗаказа.SelectedDate;
                    sqlCommand.Parameters.Add("@ShippedDate", SqlDbType.Date).Value = ДатаОтгрузкиЗаказа.SelectedDate;
                    sqlCommand.Parameters.Add("@CompletionDate", SqlDbType.Date).Value = ПланируемаяСдачаКд.SelectedDate;

                    if (ФактическаяСдачаКд.SelectedDate != null)
                    {
                        sqlCommand.Parameters.AddWithValue("@FinishCompletionDate", ФактическаяСдачаКд.SelectedDate);    
                    }
                    else
                    {
                        sqlCommand.Parameters.AddWithValue("@FinishCompletionDate", DBNull.Value);
                    }

                    sqlCommand.Parameters.Add("@OrderID", SqlDbType.Int).Value = OrderId;
                    sqlCommand.Parameters.Add("@ConstructorID", SqlDbType.Int).Value = Convert.ToInt32(Конструктор.SelectedValue);
                    sqlCommand.ExecuteNonQuery();
                }
                catch
                {
                  //  MessageBox.Show(exception.Message);
                    return false;
                }
                finally
                {
                    con.Close();
                }
            }
            return true;
        }

        bool AirVent_Delete_OrderDetails()
        {
            if (ЗаказовТаблица.SelectedItem == null)
            {
                MessageBox.Show("Выберете заказ для удаления.");
                return false;
            }

            var order = (OrdersConstructorDataClass)ЗаказовТаблица.SelectedItem;

            if (MessageBox.Show("Заказ " + order.Order + " будет удален.", "", MessageBoxButton.OKCancel, MessageBoxImage.Information) != MessageBoxResult.OK) return false;


            using (var con = new SqlConnection(App.ConnectionString))
            {
                try
                {
                    con.Open();
                    var sqlCommand = new SqlCommand("AirVent_Delete_OrderDetails", con) { CommandType = CommandType.StoredProcedure };
                    sqlCommand.Parameters.Add("@OrderDeteilID", SqlDbType.Int).Value = order.OrderDetailId;
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                    MessageBox.Show("Не удалось удалить заказ!");
                    return false;
                }
                finally
                {
                    con.Close();
                }
            }
            return true;
        }

        bool AirVent_Edit_OrderDetails()
        {
            using (var con = new SqlConnection(App.ConnectionString))
            {
                try
                {
                    con.Open();
                    var sqlCommand = new SqlCommand("AirVent_Edit_OrderDetails", con) { CommandType = CommandType.StoredProcedure };

                    sqlCommand.Parameters.Add("@OrderDetailID", SqlDbType.Int).Value = OrderId;
                    sqlCommand.Parameters.Add("@RequiredDate", SqlDbType.Date).Value = ДатаПоступленияЗаказа.SelectedDate;
                    sqlCommand.Parameters.Add("@ShippedDate", SqlDbType.Date).Value = ДатаОтгрузкиЗаказа.SelectedDate;
                    sqlCommand.Parameters.Add("@CompletionDate", SqlDbType.Date).Value = ПланируемаяСдачаКд.SelectedDate;

                    if (ФактическаяСдачаКд.SelectedDate != null)
                    {
                        sqlCommand.Parameters.AddWithValue("@FinishCompletionDate", ФактическаяСдачаКд.SelectedDate);
                    }
                    else
                    {
                        sqlCommand.Parameters.AddWithValue("@FinishCompletionDate", DBNull.Value);
                    }
                    sqlCommand.Parameters.Add("@ConstructorID", SqlDbType.Int).Value = Convert.ToInt32(Конструктор.SelectedValue);

                    sqlCommand.ExecuteNonQuery();
                }
                catch
                {
                  //  MessageBox.Show(exception.Message);
                    return false;
                }
                finally
                {
                    con.Close();
                }
            }
            return true;
        }

        readonly SolidColorBrush _whiteColorBrush = new SolidColorBrush(Colors.White);
        readonly SolidColorBrush _lightCyanColorBrush = new SolidColorBrush(Colors.LightCyan);
        readonly SolidColorBrush _lemonChiffonColorBrush = new SolidColorBrush(Colors.LemonChiffon);
        readonly SolidColorBrush _lawnGreenColorBrush = new SolidColorBrush(Colors.LightGreen);

        void ЗаказовТаблица_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Background = e.Row.GetIndex() % 2 == 1 ? _lightCyanColorBrush : _whiteColorBrush;

            var item = (OrdersConstructorDataClass)e.Row.DataContext;

            e.Row.Background = item.FinishCompletionDate == "" ? _lemonChiffonColorBrush : _lawnGreenColorBrush;
        }

        void Редактировать_Click(object sender, RoutedEventArgs e)
        {
            if (AirVent_Edit_OrderDetails())
            {
                ЗаказовТаблица.ItemsSource = OrdersConstructorDataList();
            }
            else
            {
                MessageBox.Show("Не удалось изменить заказ!");
            }
            var parent = Window.GetWindow(this);
            if (parent != null) parent.Close();
        }

        void Удалить_Click(object sender, RoutedEventArgs e)
        {
            if (AirVent_Delete_OrderDetails())
            {
                ЗаказовТаблица.ItemsSource = OrdersConstructorDataList();
            }
        }

        void НомерЗаказа_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        void Назад_Click(object sender, RoutedEventArgs e)
        {
            var parent = Window.GetWindow(this);
            if (parent != null) parent.Hide();


            if (WindowsOfApp.ОкноРедактироватьЗаказ != null)
            {
                WindowsOfApp.ОкноРедактироватьЗаказ.Show();
                return;
            }

            if (WindowsOfApp.ОкноРедактировать2Заказ != null)
            {
                WindowsOfApp.ОкноРедактировать2Заказ.Show();
            }

            

            //var редоктироватьПодбор = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.Title == "Редактирование подбора");
            //if (редоктироватьПодбор != null) редоктироватьПодбор.Show();

            //var редоктироватьПодбор2 = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.Title == "Редактирование подбора ");
            //if (редоктироватьПодбор2 != null) редоктироватьПодбор2.Show();
        }

        private void Отмена_Click(object sender, RoutedEventArgs e)
        {
            var parent = Window.GetWindow(this);
            if (parent != null) parent.Close();
        }

        private void Вперед_Click(object sender, RoutedEventArgs e)
        {
            //var редоктироватьПодбор = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.Title == "Редактирование подбора");
            //if (редоктироватьПодбор != null) //редоктироватьПодбор.Close();
            //редоктироватьПодбор.ShowDialog();

            if (!IsEditMode)
            {
                Редактировать_Click(null,null);
            }
            СоздатьЗаказ_Click(null, null);   
        }

        private void НомерЗаказа_SelectionChanged(object sender, RoutedEventArgs e)
        {
            IsAhead();
        }

        void IsAhead()
        {
            if (НомерЗаказа.Text == "" || Конструктор.Text == null)
            {
                Вперед.IsEnabled = false;
            }
            else if (НомерЗаказа.Text != "" || Конструктор.Text != null)
            {
                Вперед.IsEnabled = true;
            } 
        }

        private void Конструктор_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IsAhead();
        }

        private void Конструктор_LayoutUpdated(object sender, EventArgs e)
        {
            IsAhead();
        }

        private void ЗаказовТаблица_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsEditMode) return;

            Заказы.НаПрощете = OrdersConstructorDataList().Count(x => x.FinishCompletionDate == "");
            Заказы.ВРаботе = OrdersConstructorDataList().Count(x => x.FinishCompletionDate != "");
        }
        
        

    }
}
