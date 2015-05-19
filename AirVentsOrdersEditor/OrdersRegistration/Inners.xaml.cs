using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;

namespace OrdersRegistration
{
    /// <summary>
    /// Interaction logic for Inners.xaml
    /// </summary>
    public partial class Inners
    {
        public int SizeAv { get; set; }
        public int OrderId { get; set; }

        public Inners()
        {
            InitializeComponent();
            DataTableToSee.Visibility = Visibility.Collapsed;
            
            //6 Нагреватель водяной
            //7 Охладитель водяной
            //8 Охладитель фреоновый
            //9 Рекуператор пластинчатый
            //11 Камера увлажнения
        }

        public class InnersListData
        {
            public string Name { get; set; }
            public int IdNomenclature { get; set; }
            
            public string Model { get; set; }
            public int Count { get; set; }
            public string Manufactoring { get; set; }
            public string Notes { get; set; }
        }

        void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            HeatExchangers.ItemsSource = ((IListSource)InnerItem(6)).GetList();
            HeatExchangers.DisplayMemberPath = "Column1";
            HeatExchangers.SelectedValuePath = "IdNomenclature";
            HeatExchangers.SelectedIndex = 0;

            ColdExchangers.ItemsSource = ((IListSource)InnerItem(7)).GetList();
            ColdExchangers.DisplayMemberPath = "Column1";
            ColdExchangers.SelectedValuePath = "IdNomenclature";
            ColdExchangers.SelectedIndex = 0;

            ColdExchangers2.ItemsSource = ((IListSource)InnerItem(8)).GetList();
            ColdExchangers2.DisplayMemberPath = "Column1";
            ColdExchangers2.SelectedValuePath = "IdNomenclature";
            ColdExchangers2.SelectedIndex = 0;

            RecuperatorBox.ItemsSource = ((IListSource)InnerItem(9)).GetList();
            RecuperatorBox.DisplayMemberPath = "Column1";
            RecuperatorBox.SelectedValuePath = "IdNomenclature";
            RecuperatorBox.SelectedIndex = 0;

            MoistureBox1.ItemsSource = ((IListSource)InnerItem(11)).GetList();
            MoistureBox1.DisplayMemberPath = "Column1";
            MoistureBox1.SelectedValuePath = "IdNomenclature";
            MoistureBox1.SelectedIndex = 0;

            DataTableToSee.ItemsSource =  InnerPartsOfOrder().AsDataView();//InnerItem().AsDataView();
            UpdateList();
        }
        
        void ChangeItem(object sender, RoutedEventArgs e)
        {
            var item = InnersList.SelectedItem as InnersListData;

            if (item != null)

            EditOrderBomItem(5, item.IdNomenclature);
        }

        void DeleteItem(object sender, RoutedEventArgs e)
        {
            var item = InnersList.SelectedItem as InnersListData;

            if (item != null) DelOrderBomItem(item.IdNomenclature);
        }

        void UpdateList()
        {
            InnersList.ItemsSource = InnerPartsList();
        }


        #region Working with SQL

        DataTable InnerItem(int id)
        {
            var standartSizeTable = new DataTable();
            using (var con = new SqlConnection(App.ConnectionString))
            {
                try
                {
                    con.Open();
                    var sqlCommand = new SqlCommand(@"SELECT n.IdNomenclature, n.Nomenclature + ' - ' + he.Model FROM Nomenclature n
  INNER JOIN AirVents.HeaterExchander he ON n.IdNomenclature = he.IdNomenclature
  WHERE n.IDNomenclatureGroup = " + id +
  " AND he.SizeID = " + SizeAv, con);
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

        public List<InnersListData> InnerPartsList()
        {
            var table = InnerPartsOfOrder();
            var list = (from DataRow row in table.Rows
                        select new InnersListData
                        {
                            Name = row["Nomenclature"].ToString(),
                            IdNomenclature = Convert.ToInt32(row["IDNomenclature"]),
                            Model  = row["Model"].ToString(),
                            Count = Convert.ToInt32(row["quantity"]),
                            //Manufactoring  = row["FirsrtName"].ToString(),
                            Notes = row["Notes"].ToString(),
                        }).ToList();
            return list;
        }
        
        DataTable InnerPartsOfOrder()
        {
            var standartSizeTable = new DataTable();
            using (var con = new SqlConnection(App.ConnectionString))
            {
                try
                {
                    con.Open();
                    var sqlCommand = new SqlCommand(@"SELECT o.ProjectNumber[Order], od.InternalNumber, ob.quantity, n.Nomenclature, n.IDNomenclature, he.Model, he.Notes FROM Nomenclature n
  INNER JOIN AirVents.HeaterExchander he ON n.IdNomenclature = he.IdNomenclature
  INNER JOIN AirVents.OrderBOM ob ON n.IdNomenclature = ob.IDnomenclature
  INNER JOIN AirVents.[Order] o ON ob.IDOrder = o.OrderID
  INNER JOIN AirVents.OrderDetails od ON o.OrderID = od.OrderID
  WHERE o.OrderID = " + OrderId, con);
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

        bool AddOrderBomItem(int quantity, int idNomenclature)
        {
            if (!WorkWithParam(1, quantity, idNomenclature))
            {
              //  return EditOrderBomItem(int quantity, int idNomenclature)
            }
            return true;
        }

        bool EditOrderBomItem(int quantity, int idNomenclature)
        {
            return WorkWithParam(2, quantity, idNomenclature);
        }

        bool DelOrderBomItem(int idNomenclature)
        {
            return WorkWithParam(3, 0, idNomenclature);
        }

        bool WorkWithParam(int param, int quantity, int idNomenclature)
        {
            using (var con = new SqlConnection(App.ConnectionString))
            {
                try
                {
                    con.Open();
                    var sqlCommand = new SqlCommand("[AirVents].[OrderBOMAddEditDel]", con) { CommandType = CommandType.StoredProcedure };

                    var sqlParameter = sqlCommand.Parameters;

                    sqlParameter.AddWithValue("@IDOrder", OrderId);
                    sqlParameter.AddWithValue("@IDnomenclature", Convert.ToInt32(idNomenclature));
                    sqlParameter.AddWithValue("@quantity", Convert.ToInt32(quantity));

                    sqlParameter.AddWithValue("@PARAM", Convert.ToInt32(param));

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
                    UpdateList();
                }
            }
            return true;
        }


        #endregion

        private void InnersList_CurrentCellChanged(object sender, EventArgs e)
        {
            var item = InnersList.SelectedItem as InnersListData;

            if (item != null)

            EditOrderBomItem(item.Count, item.IdNomenclature);
        }

        void AddHeatExchangers_Click(object sender, RoutedEventArgs e)
        {
            AddOrderBomItem(1, Convert.ToInt32(HeatExchangers.SelectedValue.ToString()));
        }

        private void AddColdExchangers_Click(object sender, RoutedEventArgs e)
        {
            AddOrderBomItem(1, Convert.ToInt32(ColdExchangers.SelectedValue.ToString()));
        }

        private void AddColdExchangers2_Click(object sender, RoutedEventArgs e)
        {
            AddOrderBomItem(1, Convert.ToInt32(ColdExchangers2.SelectedValue.ToString()));
        }

        private void Recuperator_Click(object sender, RoutedEventArgs e)
        {
            AddOrderBomItem(1, Convert.ToInt32(RecuperatorBox.SelectedValue.ToString()));
        }

        private void Moisture1_Click(object sender, RoutedEventArgs e)
        {
            AddOrderBomItem(1, Convert.ToInt32(MoistureBox1.SelectedValue.ToString()));
        }

    }
}
