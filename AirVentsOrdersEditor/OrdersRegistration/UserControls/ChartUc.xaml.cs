using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.DataVisualization.Charting;

namespace OrdersRegistration.UserControls
{
    /// <summary>
    /// Interaction logic for ChartUc.xaml
    /// </summary>
    public partial class ChartUc
    {
        public ChartUc()
        {
            InitializeComponent();

            var dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += (dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

        }


        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            var isChengedValues = false;

            if (Заказы.ВРаботе != ВРаботе)
            {
                isChengedValues = true;
                ВРаботе = Заказы.ВРаботе;
            }
            if (Заказы.НаПрощете != НаПрощете)
            {
                isChengedValues = true;
                НаПрощете = Заказы.НаПрощете;
            }
            if (Заказы.НеОбработаны != НеОбработаны)
            {
                isChengedValues = true;
                НеОбработаны = Заказы.НеОбработаны;
            }
            if (isChengedValues)
            {
                LoadPieChartData(ВРаботе, НаПрощете, НеОбработаны);
            }
        }




        private int всегоЗаказов;

        public int ВРаботе;
        public int НеОбработаны;
        public int НаПрощете;





        private void LoadPieChartData(int вРаботе, int наПрощете, int неОбработаны)
        {
            McChart.Title = "Отчет по заказам";

            

            ((PieSeries)McChart.Series[0]).ItemsSource =
               new[]
                {
                    new KeyValuePair<string, int>("В работе", вРаботе),
                    new KeyValuePair<string, int>("На прощете", наПрощете),
                    new KeyValuePair<string, int>("Не обработаны", неОбработаны)
                };

            


            //    ((PieSeries)McChart.Series[0]).ItemsSource =
        //        new KeyValuePair<string, int>[]{
        //new KeyValuePair<string,int>("Project Manager", 12),
        //new KeyValuePair<string,int>("CEO", 25),
        //new KeyValuePair<string,int>("Software Engg.", 5),
        //new KeyValuePair<string,int>("Team Leader", 6),
        //new KeyValuePair<string,int>("Project Leader", 10),
        //new KeyValuePair<string,int>("Developer", 4) };
            
        }

        private void PieSeries_Loaded_1(object sender, RoutedEventArgs e)
        {
            LoadPieChartData(ВРаботе, НеОбработаны, НаПрощете);
        }
    }
}
