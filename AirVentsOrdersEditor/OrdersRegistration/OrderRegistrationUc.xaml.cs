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
    /// <summary>
    /// Interaction logic for OrderRegistrationUc.xaml
    /// </summary>
    public partial class OrderRegistrationUc
    {
        public bool IsEditMode { get; set; }
        public bool IsEditMode2 { get; set; }
        public string Wizard { get; set; }

        public int OrderId { get; set; }

        public bool? Создание { get; set; }
        public string ТипКаркасаУстановки { get; set; }
        public string ТипоразмерУстановки { get; set; }
        public string ИмяПродавца { get; set; }
        public string НомерПодбора { get; set; }
        public DateTime? SelectedDate { get; set; }

        public int? @SupplyTotalStaticPressure { get; set; }
        public int? @SupplyStaticPressure { get; set; }
        public double? @SupplyAirflow { get; set; }
        public int? @ExhaustTotalStaticPressure { get; set; }
        public int? @ExhaustStaticPressure { get; set; }
        public double? @ExhaustAirflow { get; set; }
        public bool? @ServiceAccess { get; set; }
        

        public OrderRegistrationUc()
        {
            InitializeComponent();
            
            #region Менеджеры

            var managersCombo = ((IListSource) ManagersTable()).GetList();

            ManagersBox.ItemsSource = ((IListSource)ManagersTable()).GetList();
            ManagersBox.ItemsSource = managersCombo;
            ManagersBox.DisplayMemberPath = "LastName";
            ManagersBox.SelectedValuePath = "EmpID";
            ManagersBox.SelectedIndex = 0;

            #endregion

            #region Типоразмеры установок

            var airVentsStandardSize = AirVentsStandardSize();
            Типоразмер.ItemsSource = ((IListSource)airVentsStandardSize).GetList();
            Типоразмер.DisplayMemberPath = "Type";
            Типоразмер.SelectedValuePath = "SizeID";
            Типоразмер.SelectedIndex = 0;

            #endregion

            #region Заказы

            OrdersList.ItemsSource = OrdersManagersDataList(); 
            ДатаЗаказаDate.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            
            #endregion

            CancelButton.IsEnabled = false; 

            СоздатьПодбор.Visibility = Visibility.Collapsed;
            СохранитьЗаказ.Visibility = Visibility.Collapsed;

            var dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += (dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

            Проверка.Visibility = !App.IsDeveloper ? Visibility.Collapsed : Visibility.Visible;
        }

        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            Заказы.НеОбработаны = OrdersManagersDataList().Count() - Заказы.ВРаботе - Заказы.НаПрощете;

            if (OrdersList.SelectedItem != null) return;
            OrdersList.ItemsSource = OrdersManagersDataList();
            Проверка.ItemsSource = OrdersManagersDataList();

        }
        
        void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (IsEditMode)
            {
                Подборы.Visibility = Visibility.Collapsed;
                ТаблицаМенеджеров.ItemsSource = ManagersList();
                Общая.Visibility = Visibility.Collapsed;
                SelectionEditor.Visibility = Visibility.Collapsed;

                ManagersBox.SelectedIndex = 0;
            }
            else if (IsEditMode2)
            {
                SelectionEditor.Visibility = Visibility.Visible;
                РедакторМенеджеров.Visibility = Visibility.Collapsed;
                Подборы.Visibility = Visibility.Collapsed;
                Общая.Visibility = Visibility.Collapsed;

                if (НомерПодбора != null) Номерподбора.Text = НомерПодбора;
                if (ИмяПродавца != null) ManagersBox.Text = ИмяПродавца;
                if (SelectedDate != null) ДатаЗаказаDate.SelectedDate = SelectedDate;
                if (ТипоразмерУстановки != null) Типоразмер.Text = ТипоразмерУстановки;
                if (ТипКаркасаУстановки != null) ТипКаркаса.Text = ТипКаркасаУстановки;
                if (Создание != null) SelectionEditor.Header = (bool)Создание ? "Создание подбора" : "Редактирование подбора";
                if (Создание != null) СоздатьЗаказ.Content = (bool)Создание ? "Сохранить подбор" : "Сохранить изменения";

                 
                if (@SupplyTotalStaticPressure != null) ПроизводительностьПриток.Text = @SupplyTotalStaticPressure.ToString();
                if (@SupplyStaticPressure != null) ПолноеСтатическоеДавлениеВентилятораПриток.Text = @SupplyStaticPressure.ToString();
                if (@SupplyAirflow != null) СкоростьВСеченииПриток.Text = @SupplyAirflow.ToString();

                if (@ExhaustTotalStaticPressure != null) ПроизводительностьВытяжка.Text = @ExhaustTotalStaticPressure.ToString();
                if (@ExhaustStaticPressure != null) ПолноеСтатическоеДавлениеВентилятораВитяжка.Text = @ExhaustStaticPressure.ToString();
                if (@ExhaustAirflow != null) СкоростьВСеченииВитяжка.Text = @ExhaustAirflow.ToString();


                if (@ServiceAccess == null)
                {
                    Left.IsChecked = true;
                    Right.IsChecked = false;
                }
                else if ((bool)@ServiceAccess)
                {
                    Left.IsChecked = true;
                    Right.IsChecked = false;
                }
                else
                {
                    Left.IsChecked = false;
                    Right.IsChecked = true;
                }

            }
            else
            {
                РедакторМенеджеров.Visibility = Visibility.Collapsed;
                SelectionEditor.Visibility = Visibility.Collapsed;
            }

            switch (Wizard)
            {
                case "01":
                    Назад.IsEnabled = false;
                    Вперед.IsEnabled = Номерподбора.Text != "";
                    Отмена.IsEnabled = true;
                    СоздатьЗаказ.Visibility = Visibility.Collapsed;
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
                    Назад.Visibility = Visibility.Collapsed;
                    Вперед.Visibility = Visibility.Collapsed;
                    Отмена.Visibility = Visibility.Collapsed;
                    break;
            }


            

        }


        #region AirVentsStandardSize

        static DataTable AirVentsStandardSize()
        {
            var standartSizeTable = new DataTable();
            using (var con = new SqlConnection(App.ConnectionString))
            {
                try
                {
                    con.Open();
                    var sqlCommand = new SqlCommand("SELECT * FROM  AirVents.StandardSize ORDER BY Type", con);
                    var sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    sqlDataAdapter.Fill(standartSizeTable);
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
            return standartSizeTable;
        }

        #endregion
       
        #region Orders(Подборы)

        public class OrdersManagersDataClass
        {
            public string ManagerDataClass { get; set; }
            public string LastName { get; set; }
            public string FirsrtName { get; set; }
            public DateTime Date { get; set; }
            public string ProjectNumber { get; set; }
            public string Type { get; set; }
            public string Description { get; set; }
            public int OrderId { get; set; }

            public int? SupplyTotalStaticPressure { get; set; }
            public int? SupplyStaticPressure { get; set; }
            public double? SupplyAirflow { get; set; }

            public int? ExhaustTotalStaticPressure { get; set; }
            public int? ExhaustStaticPressure { get; set; }
            public double? ExhaustAirflow { get; set; }

            public bool? ServiceAccess { get; set; }            
        }

        static DataTable OrdersManagersTable()
        {
            var ordersList = new DataTable();
            using (var con = new SqlConnection(App.ConnectionString))
            {
                try
                {
                    con.Open();
                    
                    var sqlCommand = new SqlCommand( @"SELECT HumanResources.Employee.LastName +' '+ LEFT(HumanResources.Employee.FirsrtName, 1)+'.', AirVents.[Order].Date, AirVents.[Order].ProjectNumber, AirVents.StandardSize.Type, 
       AirVents.Profil.Description,  AirVents.[Order].OrderID
    ,AirVents.[Order].SupplyAirflow
    ,AirVents.[Order].SupplyStaticPressure
    ,AirVents.[Order].SupplyTotalStaticPressure
    ,AirVents.[Order].ExhaustAirflow
    ,AirVents.[Order].ExhaustStaticPressure
    ,AirVents.[Order].ExhaustTotalStaticPressure
    ,AirVents.[Order].ServiceAccess
    FROM  AirVents.DimensionType INNER JOIN
       AirVents.[Order] ON AirVents.DimensionType.DimensionTypeID = AirVents.[Order].DimensionTypeID INNER JOIN
       HumanResources.Employee ON AirVents.[Order].empid = HumanResources.Employee.EmpID INNER JOIN
       AirVents.Profil ON AirVents.DimensionType.ProfilID = AirVents.Profil.ProfilID INNER JOIN
       AirVents.StandardSize ON AirVents.DimensionType.SizeID = AirVents.StandardSize.SizeID", con);

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

        IEnumerable<OrdersManagersDataClass> OrdersManagersDataList()
        {
            var list = (from DataRow row in OrdersManagersTable().Rows
                        select new OrdersManagersDataClass
                        {
                            ManagerDataClass = row["Column1"].ToString(),
                            Date = Convert.ToDateTime(row["Date"]),
                            ProjectNumber = row["ProjectNumber"].ToString(),
                            Type = row["Type"].ToString(),
                            Description = row["Description"].ToString(),
                            OrderId = Convert.ToInt32(row["OrderID"]),

                            SupplyTotalStaticPressure =  row["SupplyTotalStaticPressure"] == DBNull.Value ? 0 : Convert.ToInt32(row["SupplyTotalStaticPressure"]),
                            SupplyStaticPressure = row["SupplyStaticPressure"] == DBNull.Value ? 0 : Convert.ToInt32(row["SupplyStaticPressure"]),
                            SupplyAirflow = row["SupplyAirflow"] == DBNull.Value ? 0 : Convert.ToDouble(row["SupplyAirflow"]),
                            ExhaustTotalStaticPressure = row["ExhaustTotalStaticPressure"] == DBNull.Value ? 0 : Convert.ToInt32(row["ExhaustTotalStaticPressure"]),
                            ExhaustStaticPressure = row["ExhaustStaticPressure"] == DBNull.Value ? 0 : Convert.ToInt32(row["ExhaustStaticPressure"]),
                            ExhaustAirflow = row["ExhaustAirflow"] == DBNull.Value ? 0 : Convert.ToDouble(row["ExhaustAirflow"]),
                            ServiceAccess = row["ExhaustAirflow"] == DBNull.Value ? null : (bool?)Convert.ToBoolean(row["ServiceAccess"])


                        }).ToList();
            return list.OrderByDescending(x => x.Date);
        }

        bool AirVents_AddOrder()
        {

            using (var con = new SqlConnection(App.ConnectionString))
            {
                try
                {
                    con.Open();
                    var sqlCommand = new SqlCommand("AirVents_AddOrder", con) { CommandType = CommandType.StoredProcedure };

                    var sqlParameter = sqlCommand.Parameters;
                    
                    sqlParameter.AddWithValue("@Manager", Convert.ToInt32(ManagersBox.SelectedValue));
                    sqlParameter.AddWithValue("@Size", Convert.ToInt32(Типоразмер.SelectedValue));
                    sqlParameter.AddWithValue("@ProjectNumber", Номерподбора.Text);
                    sqlParameter.AddWithValue("@Profile", ТипКаркаса.Text);

                    sqlParameter.AddWithValue("@Date", ДатаЗаказаDate.SelectedDate);

                    #region ПроизводительностьПриток

                    if (ПроизводительностьПриток.Text == "")
                    {
                        sqlParameter.AddWithValue("@SupplyTotalStaticPressure", DBNull.Value);
                    }
                    else
                    {
                        sqlParameter.AddWithValue("@SupplyTotalStaticPressure", Convert.ToInt32(ПроизводительностьПриток.Text));
                    }

                    #endregion

                    #region ПолноеСтатическоеДавлениеВентилятораПриток

                    if (ПолноеСтатическоеДавлениеВентилятораПриток.Text == "")
                    {
                        sqlParameter.AddWithValue("@SupplyStaticPressure", DBNull.Value);
                    }
                    else
                    {
                        sqlParameter.AddWithValue("@SupplyStaticPressure", Convert.ToInt32(ПолноеСтатическоеДавлениеВентилятораПриток.Text));
                    }

                    #endregion

                    #region СкоростьВСеченииПриток

                    if (СкоростьВСеченииПриток.Text == "")
                    {
                        sqlParameter.AddWithValue("@SupplyAirflow", DBNull.Value);
                    }
                    else
                    {
                        sqlParameter.AddWithValue("@SupplyAirflow", Convert.ToDouble(СкоростьВСеченииПриток.Text.Replace('.',',')));
                    }

                    #endregion

                    #region ПроизводительностьВытяжка

                    if (ПроизводительностьВытяжка.Text == "")
                    {
                        sqlParameter.AddWithValue("@ExhaustTotalStaticPressure", DBNull.Value);
                    }
                    else
                    {
                        sqlParameter.AddWithValue("@ExhaustTotalStaticPressure", Convert.ToInt32(ПроизводительностьВытяжка.Text));
                    }

                    #endregion

                    #region ПолноеСтатическоеДавлениеВентилятораВитяжка

                    if (ПолноеСтатическоеДавлениеВентилятораВитяжка.Text == "")
                    {
                        sqlParameter.AddWithValue("@ExhaustStaticPressure", DBNull.Value);
                    }
                    else
                    {
                        sqlParameter.AddWithValue("@ExhaustStaticPressure", Convert.ToInt32(ПолноеСтатическоеДавлениеВентилятораВитяжка.Text));
                    }

                    #endregion

                    #region СкоростьВСеченииВитяжка

                    if (СкоростьВСеченииВитяжка.Text == "")
                    {
                        sqlParameter.AddWithValue("@ExhaustAirflow", DBNull.Value);
                    }
                    else
                    {
                        sqlParameter.AddWithValue("@ExhaustAirflow", Convert.ToDouble(СкоростьВСеченииВитяжка.Text.Replace('.', ',')));
                    }

                    #endregion

                    sqlParameter.AddWithValue("@ServiceAccess", Left.IsChecked == true);

                    sqlCommand.ExecuteNonQuery();

                    #region To Delete
                    //var returnParameter = sqlCommand.Parameters.Add("@ProjectNumber", SqlDbType.VarChar);
                    //var returnParameter = sqlParameter.AddWithValue("@ProjectNumber", Номерподбора.Text);
                    //returnParameter.Direction = ParameterDirection.ReturnValue;
                    //var result = returnParameter.Value.ToString();
                    #endregion
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Введите корректные данные!\n" + exception.Message);
                    return false;
                }
                finally
                {
                    con.Close();
                }
            }
            return true;
        }

        bool AirVent_Edit_Order()
        {
            using (var con = new SqlConnection(App.ConnectionString))
            {
                try
                {
                    con.Open();
                    var sqlCommand = new SqlCommand("AirVent_Edit_Order", con) { CommandType = CommandType.StoredProcedure };

                    var sqlParameter = sqlCommand.Parameters;

                    sqlParameter.AddWithValue("@OrderID", OrderId);

                    sqlParameter.AddWithValue("@Maneger", Convert.ToInt32(ManagersBox.SelectedValue));
                    sqlParameter.AddWithValue("@Size", Convert.ToInt32(Типоразмер.SelectedValue));
                    sqlParameter.AddWithValue("@ProjectNumber", Номерподбора.Text);
                    sqlParameter.AddWithValue("@Profile", ТипКаркаса.Text);
                    sqlParameter.AddWithValue("@Date", ДатаЗаказаDate.SelectedDate);

                    #region ПроизводительностьПриток

                    if (ПроизводительностьПриток.Text == "")
                    {
                        sqlParameter.AddWithValue("@SupplyTotalStaticPressure", DBNull.Value);
                    }
                    else
                    {
                        sqlParameter.AddWithValue("@SupplyTotalStaticPressure", Convert.ToInt32(ПроизводительностьПриток.Text));
                    }

                    #endregion

                    #region ПолноеСтатическоеДавлениеВентилятораПриток

                    if (ПолноеСтатическоеДавлениеВентилятораПриток.Text == "")
                    {
                        sqlParameter.AddWithValue("@SupplyStaticPressure", DBNull.Value);
                    }
                    else
                    {
                        sqlParameter.AddWithValue("@SupplyStaticPressure", Convert.ToInt32(ПолноеСтатическоеДавлениеВентилятораПриток.Text));
                    }

                    #endregion

                    #region СкоростьВСеченииПриток

                    if (СкоростьВСеченииПриток.Text == "")
                    {
                        sqlParameter.AddWithValue("@SupplyAirflow", DBNull.Value);
                    }
                    else
                    {
                        sqlParameter.AddWithValue("@SupplyAirflow", Convert.ToDouble(СкоростьВСеченииПриток.Text.Replace('.',',')));
                    }

                    #endregion

                    #region ПроизводительностьВытяжка

                    if (ПроизводительностьВытяжка.Text == "")
                    {
                        sqlParameter.AddWithValue("@ExhaustTotalStaticPressure", DBNull.Value);
                    }
                    else
                    {
                        sqlParameter.AddWithValue("@ExhaustTotalStaticPressure", Convert.ToInt32(ПроизводительностьВытяжка.Text));
                    }

                    #endregion

                    #region ПолноеСтатическоеДавлениеВентилятораВитяжка

                    if (ПолноеСтатическоеДавлениеВентилятораВитяжка.Text == "")
                    {
                        sqlParameter.AddWithValue("@ExhaustStaticPressure", DBNull.Value);
                    }
                    else
                    {
                        sqlParameter.AddWithValue("@ExhaustStaticPressure", Convert.ToInt32(ПолноеСтатическоеДавлениеВентилятораВитяжка.Text));
                    }

                    #endregion

                    #region СкоростьВСеченииВитяжка

                    if (СкоростьВСеченииВитяжка.Text == "")
                    {
                        sqlParameter.AddWithValue("@ExhaustAirflow", DBNull.Value);
                    }
                    else
                    {
                        sqlParameter.AddWithValue("@ExhaustAirflow", Convert.ToDouble(СкоростьВСеченииВитяжка.Text.Replace('.', ',')));
                    }

                    #endregion

                    sqlParameter.AddWithValue("@ServiceAccess", Left.IsChecked == true);

                    sqlCommand.ExecuteNonQuery();

                }
                catch (Exception exception)
                {
                    MessageBox.Show("Введите корректные данные!\n" + exception.Message);
                    return false;
                }
                finally
                {
                    con.Close();
                }
            }
            return true;
        }

        bool AirVent_Delete_Order()
        {
            if (OrdersList.SelectedItem == null)
            {
                MessageBox.Show("Выберете подбор для удаления.");
                return false;
            }

            var order = (OrdersManagersDataClass)OrdersList.SelectedItem;

            if (MessageBox.Show("Подбор №" + order.ProjectNumber + " будет удален.", "", MessageBoxButton.OKCancel, MessageBoxImage.Information) != MessageBoxResult.OK) return false;

            using (var con = new SqlConnection(App.ConnectionString))
            {
                try
                {
                    con.Open();
                    var sqlCommand = new SqlCommand("AirVent_Delete_Order", con) { CommandType = CommandType.StoredProcedure };
                    sqlCommand.Parameters.Add("@OrderID", SqlDbType.Int).Value = order.OrderId;
                    sqlCommand.ExecuteNonQuery();
                }
                catch
                {
                    MessageBox.Show("Возможная причина: наличие заказа по данному подбору.", "Не удалось удалить подбор!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return false;
                }
                finally
                {
                    con.Close();
                }
            }
            return true;
        }

        #endregion

        #region Мэнеджеры

        public class Manager
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int IdManager { get; set; }
        }
        
        public List<Manager> ManagersList()
        {
            var table = ManagersTable();
            var list = (from DataRow row in table.Rows
                select new Manager
                {
                    FirstName = row["FirsrtName"].ToString(),
                    LastName = row["LastName"].ToString(),
                    IdManager = Convert.ToInt32(row["EmpID"])
                }).ToList();
            return list;
        }

        static void DeleteManager(int empId)
        {
            using (var con = new SqlConnection(App.ConnectionString))
            {
                try
                {
                    con.Open();
                    var sqlCommand =
                       new SqlCommand(@"DELETE FROM  HumanResources.Employee " +
"WHERE EmpID = " + empId, con);
                    var sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    var dataTable = new DataTable("Username");
                    sqlDataAdapter.Fill(dataTable);
                    sqlDataAdapter.Dispose();
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
                finally
                {
                    con.Close();
                }
            }
        }

        static DataTable ManagersTable()
        {
            var managersList = new DataTable();
            using (var con = new SqlConnection(App.ConnectionString))
            {
                try
                {
                    con.Open();
                    var sqlCommand = new SqlCommand("SELECT * FROM  HumanResources.Employee ORDER BY LastName", con);
                    var sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    sqlDataAdapter.Fill(managersList);
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
            return managersList;
        }

        public bool AddManager(string userName, string lastName)
        {
            using (var con = new SqlConnection(App.ConnectionString))
            {
                try
                {
                    con.Open();
                    var sqlCommand =
                        new SqlCommand("INSERT into HumanResources.Employee (LastName, FirsrtName) VALUES " + "('" + lastName + "','" + userName + "')", con);
                    var sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    var dataTable = new DataTable("Username");
                    sqlDataAdapter.Fill(dataTable);
                }
                catch (Exception)
                {
                    return false;
                }
                finally
                {
                    con.Close();
                }
            }
            return true;
        }

        void UpdateManager(string userName, string lastName, int empId)
        {
            using (var con = new SqlConnection(App.ConnectionString))
            {
                try
                {
                    con.Open();
                    var sqlCommand =
                       new SqlCommand(@"UPDATE HumanResources.Employee 
SET
LastName = '" + lastName + "'," +
"FirsrtName = '" + userName + "'" +
"WHERE EmpID = " + empId, con);
                    var sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    var dataTable = new DataTable("Username");
                    sqlDataAdapter.Fill(dataTable);
                    sqlDataAdapter.Dispose();
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
                finally
                {
                    con.Close();
                }
            }
        }
        
        #endregion

        #region Оформление

        readonly SolidColorBrush _whiteColorBrush = new SolidColorBrush(Colors.White);
        readonly SolidColorBrush _lightCyanColorBrush = new SolidColorBrush(Colors.LightCyan);

        void Типоразмер_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Типоразмер.SelectedValue == null) return;

            var profilSizeTable = new DataTable();
            using (var con = new SqlConnection(App.ConnectionString))
            {
                try
                {
                    con.Open();
                    var sqlCommand = new SqlCommand(@"SELECT AirVents.Profil.Description FROM AirVents.DimensionType INNER JOIN
                      AirVents.Profil ON AirVents.DimensionType.ProfilID = AirVents.Profil.ProfilID INNER JOIN
                      AirVents.StandardSize ON AirVents.DimensionType.SizeID = AirVents.StandardSize.SizeID
                      WHERE AirVents.DimensionType.SizeID = " + Типоразмер.SelectedValue, con);
                    var sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    sqlDataAdapter.Fill(profilSizeTable);
                    sqlDataAdapter.Dispose();
                }
                catch (Exception)
                {
                    return;
                }
                finally
                {
                    con.Close();
                }
            }

            ТипКаркаса.ItemsSource = ((IListSource)profilSizeTable).GetList();
            ТипКаркаса.DisplayMemberPath = "Description";
            ТипКаркаса.SelectedValue = "Description";
            ТипКаркаса.SelectedIndex = 0;
        }

        void OrdersList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Background = e.Row.GetIndex() % 2 == 1 ? _lightCyanColorBrush : _whiteColorBrush;

            if (OrdersList.SelectedItem == null)
            {
                РедактироватьПодбор.IsEnabled = false;
                УдалитьПодбор.IsEnabled = false;
                return;
            }
            РедактироватьПодбор.IsEnabled = true;
            УдалитьПодбор.IsEnabled = true;
        }

        void OrdersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OrdersList.SelectedItem == null)
            {
                РедактироватьПодбор.IsEnabled = false;
                УдалитьПодбор.IsEnabled = false;
                return;
            }
            РедактироватьПодбор.IsEnabled = true;
            УдалитьПодбор.IsEnabled = true;
            var item = (OrdersManagersDataClass)OrdersList.SelectedItem;
            СохранитьЗаказ.Content = "Создать заказ по подбору №" + item.ProjectNumber;
        }

        void ТаблицаМенеджеров_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var manager = (Manager)ТаблицаМенеджеров.SelectedItem;
            if (manager!=null)
            {
                Имя.Text = manager.FirstName;
                Фамилия.Text = manager.LastName;
                CancelButton.IsEnabled = true;
            }
            else
            {
                CancelButton.IsEnabled = false; 
            }
        }

        void UpdateManagersBox()
        {
            ManagersBox.ItemsSource = ((IListSource)ManagersTable()).GetList();
            ManagersBox.DisplayMemberPath = "LastName";
            ManagersBox.SelectedValuePath = "EmpID";
            ManagersBox.SelectedIndex = 0;

            OrdersList.ItemsSource = OrdersManagersDataList(); 
        }

        void Имя_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^А-Яа-яЁё]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        void Фамилия_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^А-Яа-яЁё]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        void Имя_TextChanged(object sender, TextChangedEventArgs e)
        {
            IsEnableEditing();
        }

        void Фамилия_TextChanged(object sender, TextChangedEventArgs e)
        {
            IsEnableEditing();
        }

        void IsEnableEditing()
        {
            if (Имя == null || Фамилия == null)
            {
               return;
            }
            Редактировать.IsEnabled = ManagersList().Where(x => x.FirstName == Имя.Text).Count(x => x.LastName == Фамилия.Text) == 0;
            Добавить.IsEnabled = ManagersList().Where(x => x.FirstName == Имя.Text).Count(x => x.LastName == Фамилия.Text) == 0;
        }

        void КоличествоУстановок_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }

        void numerator_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            КоличествоУстановок.Text = Convert.ToString(Numerator.Value);
        }

        void КоличествоУстановок_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (КоличествоУстановок.Text == "" || КоличествоУстановок.Text == "0")
            {
                КоличествоУстановок.Text = "1";
            }
            Numerator.Value = Convert.ToInt32(КоличествоУстановок.Text);
        }

        void Производительность_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }

        void СкоростьВСечении_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9,.]");
            e.Handled = regex.IsMatch(e.Text);
        }

        #endregion

        #region Buttons

        void РедактироватьМенеджеров_Click(object sender, RoutedEventArgs e)
        {
            var newWindow = new Window
            {
                SizeToContent = SizeToContent.WidthAndHeight,
                ResizeMode = ResizeMode.NoResize,
                Title = "Редактор менеджеров",
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = new OrderRegistrationUc
                {
                    IsEditMode = true
                }
            };
            newWindow.ShowDialog();
            UpdateManagersBox();
        }

        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var manager = (Manager)ТаблицаМенеджеров.SelectedItem;
            if (manager == null)
            {
                MessageBox.Show("Выберете строку в таблице для изменения.");
                return;
            }
            DeleteManager(manager.IdManager);
            ТаблицаМенеджеров.ItemsSource = ManagersList();
            UpdateManagersBox();
        }

        void Добавить_Click(object sender, RoutedEventArgs e)
        {
            if (Имя.Text == "" || Фамилия.Text == "")
            {
                MessageBox.Show("Введите данные!");
                return;
            }
            AddManager(Имя.Text, Фамилия.Text);
            ТаблицаМенеджеров.ItemsSource = ManagersList();

            UpdateManagersBox();
        }

        void Редактировать_Click(object sender, RoutedEventArgs e)
        {
            var manager = (Manager)ТаблицаМенеджеров.SelectedItem;
            if (manager == null)
            {
                MessageBox.Show("Выберете строку в таблице для изменения.");
                return;
            }
            UpdateManager(Имя.Text, Фамилия.Text, manager.IdManager);
            ТаблицаМенеджеров.ItemsSource = ManagersList();

            UpdateManagersBox();
        }

        void СоздатьПодбор_Click(object sender, RoutedEventArgs e)
        {
            var newWindow = new Window
            {
                SizeToContent = SizeToContent.WidthAndHeight,
                ResizeMode = ResizeMode.NoResize,
                Title = "Создать подбор",
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = new OrderRegistrationUc
                {
                    IsEditMode2 = true,
                    Создание = true
                }
            };
            newWindow.ShowDialog();
            UpdateManagersBox();
        }

        void УдалитьПодбор_Click(object sender, RoutedEventArgs e)
        {
            if (!AirVent_Delete_Order()) return;
            UpdateManagersBox();
        }

        void РедактироватьПодбор_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = (OrdersManagersDataClass)OrdersList.SelectedItem;
            if (selectedItem == null) return;

            WindowsOfApp.ОкноРедактироватьЗаказ = new Window
            {
                SizeToContent = SizeToContent.WidthAndHeight,
                ResizeMode = ResizeMode.NoResize,
                Title = "Редактирование подбора ",
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = new OrderRegistrationUc
                {
                    IsEditMode2 = true,
                    НомерПодбора = selectedItem.ProjectNumber,
                    ИмяПродавца = selectedItem.ManagerDataClass.Remove(selectedItem.ManagerDataClass.Length - 3),
                    SelectedDate = selectedItem.Date,
                    ТипоразмерУстановки = selectedItem.Type,
                    ТипКаркасаУстановки = selectedItem.Description,

                    @SupplyTotalStaticPressure = selectedItem.SupplyTotalStaticPressure,
                    @SupplyStaticPressure = selectedItem.SupplyStaticPressure,
                    @SupplyAirflow = selectedItem.SupplyAirflow,
                    @ExhaustTotalStaticPressure = selectedItem.ExhaustTotalStaticPressure,
                    @ExhaustStaticPressure = selectedItem.ExhaustStaticPressure,
                    @ExhaustAirflow = selectedItem.ExhaustAirflow,
                    @ServiceAccess = selectedItem.ServiceAccess,

                    Создание = false,
                    OrderId = selectedItem.OrderId,
                    Wizard = "01"
                }
            };
            WindowsOfApp.ОкноРедактироватьЗаказ.Show();
            WindowsOfApp.ОкноРедактироватьЗаказ.Closed += ОкноРедактироватьЗаказ_Closed;

            UpdateManagersBox();
          
        }

        void EditInWizard()
        {
            var selectedItem = (OrdersManagersDataClass)OrdersList.SelectedItem;

            if (selectedItem == null) return;
            
            WindowsOfApp.ОкноРедактировать2Заказ =
            new Window
            {
                SizeToContent = SizeToContent.WidthAndHeight,
                ResizeMode = ResizeMode.NoResize,
                Title = "Редактирование подбора",
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = new OrderRegistrationUc
                {
                    IsEditMode2 = true,
                    НомерПодбора = selectedItem.ProjectNumber,
                    ИмяПродавца = selectedItem.ManagerDataClass.Remove(selectedItem.ManagerDataClass.Length - 3),
                    SelectedDate = selectedItem.Date,
                    ТипоразмерУстановки = selectedItem.Type,
                    ТипКаркасаУстановки = selectedItem.Description,
                    Создание = false,
                    OrderId = selectedItem.OrderId,
                    Wizard = "01"
                }
            };
            WindowsOfApp.ОкноРедактировать2Заказ.Closed += ОкноРедактировать2ЗаказOnClosed;  
            
        }

        private static void ОкноРедактировать2ЗаказOnClosed(object sender, EventArgs eventArgs)
        {
            WindowsOfApp.ОкноРедактировать2Заказ = null;
        }

        private static void ОкноРедактироватьЗаказ_Closed(object sender, EventArgs e)
        {
            WindowsOfApp.ОкноРедактироватьЗаказ = null;
        }



        void СохранитьЗаказ_Click(object sender, RoutedEventArgs e)
        {

            var selectedItem = (OrdersManagersDataClass)OrdersList.SelectedItem;

            if (selectedItem == null) return;

            WindowsOfApp.ОкноСохранитьЗаказ = new Window
            {
                SizeToContent = SizeToContent.WidthAndHeight,
                ResizeMode = ResizeMode.NoResize,
                Title = "Сохранить заказ",
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = new OrderRegistrationKbUc
                {
                    OrderId = selectedItem.OrderId,
                    IsEditMode = true,
                    SavingOrder = true,
                    RequiredDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                    ShippedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                    CompletionDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                    FinishCompletionDate = null
                }
            };
            WindowsOfApp.ОкноСохранитьЗаказ.ShowDialog();

            WindowsOfApp.ОкноСохранитьЗаказ.Closed += ОкноСохранитьЗаказOnClosed;
        }

        private void ОкноСохранитьЗаказOnClosed(object sender, EventArgs eventArgs)
        {
            WindowsOfApp.ОкноСохранитьЗаказ = null;
        }

        void СоздатьЗаказ_Click(object sender, RoutedEventArgs e)
        {
            var list = OrdersList.ItemsSource as IEnumerable<OrdersManagersDataClass>;
            
            if (SelectionEditor.Header.ToString().Contains("Создание"))
            {
                if (list != null && (list.Any(x => x.ProjectNumber == Номерподбора.Text)))
                {
                    MessageBox.Show("Подбор " + Номерподбора.Text + " уже существует!");
                    return;
                }
                if (!AirVents_AddOrder()) return;
            }
            else
            {
                if (!AirVent_Edit_Order()) return;
            }

            OrdersList.ItemsSource = OrdersManagersDataList();

            if (OrdersList.Items.Count <= 0) return;
            var border = VisualTreeHelper.GetChild(OrdersList, 0) as Decorator;
            if (border == null) return;
            var scroll = border.Child as ScrollViewer;
            if (scroll != null) scroll.ScrollToBottom();
            
            var list2 = OrdersList.ItemsSource as IEnumerable<OrdersManagersDataClass>;

            OrdersList.SelectedIndex = list2.ToList().FindIndex(x => x.OrderId == list2.Max(y => y.OrderId));
            

            var parent = Window.GetWindow(this);
            if (parent != null) parent.Hide();
        }
       

        void СоздатьЗаказПолн_Click(object sender, RoutedEventArgs e)
        {
            WindowsOfApp.ОкноСоздатьЗаказ = new Window
            {
                SizeToContent = SizeToContent.WidthAndHeight,
                Name = "_новыйПодбор",
                ResizeMode = ResizeMode.NoResize,
                Title = "Создние подбора",
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = new OrderRegistrationUc
                {
                    IsEditMode2 = true,
                    Создание = true,
                    Wizard = "01"
                }
            };
            WindowsOfApp.ОкноСоздатьЗаказ.ShowDialog();
            UpdateManagersBox();
            
          
        }

        void Вперед_Click(object sender, RoutedEventArgs e)
        {
         
            var parent = Window.GetWindow(this);
            if (parent == null) return;

            switch (parent.Title)
            {
                case "Создние подбора":
                    СоздатьЗаказ_Click(null, null);
                    EditInWizard();
                    СохранитьЗаказ_Click(null, null);
                    break;

                case "Редактирование подбора":
                    СоздатьЗаказ_Click(null, null);
                    СохранитьЗаказПоказать();
                    break;

                case "Редактирование подбора ":
                    СоздатьЗаказ_Click(null, null);
                    СохранитьЗаказ_Click(null, null);
                  
                    break;

                default:
                    СоздатьЗаказ_Click(null, null);
                    СохранитьЗаказ_Click(null, null);
                    break;
            }
        }


        void СохранитьЗаказПоказать()
        {
            var parent = Window.GetWindow(this);
            if (parent != null) parent.Hide();

            if (WindowsOfApp.ОкноСохранитьЗаказ == null) return;
            
            WindowsOfApp.ОкноСохранитьЗаказ.Show();
            MessageBox.Show("sfqeqf");

        }
        
      

        void Отмена_Click(object sender, RoutedEventArgs e)
        {
            var parent = Window.GetWindow(this);
            if (parent != null) parent.Close();
        }

        #endregion

        private void Номерподбора_SelectionChanged(object sender, RoutedEventArgs e)
        {
            Вперед.IsEnabled = Номерподбора.Text != "";
        }
    }
}
