using DocumentFormat.OpenXml.Spreadsheet;
using HoWestPost.Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace HoWestPost.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int counter = 30;
        int deliveryTime = 30;
        bool prior = false;
        Conversie convers;
        int timePast = 0;
        private DeliveryProcessor deliveryProcessor;

        int deliveryNumber = 0;
     //   ArrayList deliveries = new ArrayList();
        List<Delivery> deliveries = new List<Delivery>();
        IEnumerable<Delivery> deliv;
        Delivery activeDelivery;
        

        public MainWindow()
        {
            InitializeComponent();
           
            while (counter < 95)
            {
                ComboxDeliveryTime.Items.Add(counter);
                counter += 5;
            }
            ComboxDeliveryTime.SelectedIndex = 0;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += timer_Tick;
            timer.Start();

        }
        //// https://www.wpf-tutorial.com/misc/dispatchertimer/
        void timer_Tick(object sender, EventArgs e)
        {
            
            lblTime.Content = DateTime.Now.ToLongTimeString();
            if ((activeDelivery == null) && (deliv != null))
            {
                try
                {
                    activeDelivery = deliv.First();
                    timePast = 0;
                    deliveries.Remove(activeDelivery);
                    deliv = deliveries.OrderBy(x => x.deliveryNumber).OrderByDescending(a => a.prior);

                    lblPrior.Content = activeDelivery.prior;
                    lblTotalTravelTime.Content = activeDelivery.realTravelTime.ToString() + "min";
                    lblTimeLeft1.Content = activeDelivery.realTravelTime.ToString();
                    lblType.Content = activeDelivery.packageType;

                    ListBoxWaiting.Items.Clear();
                    foreach (Delivery d in deliv)
                    {
                        ListBoxWaiting.Items.Add(d);
                    }
                }
                catch
                {

                }
                
                
            }
            if (activeDelivery != null)
            {
                timePast += 1;
                if (timePast <= activeDelivery.realTravelTime)
                {
                    lblTimeLeft1.Content = activeDelivery.realTravelTime - timePast;
                }
                else
                {
                    activeDelivery = null;
                }
                
            }
            
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
            //MessageBox.Show("window loaded");
            // Create a timer and set a two second interval.
            
        }
        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
             MessageBox.Show(e.SignalTime.ToString());
          //  lblTime.Content = e.SignalTime.ToString();
        }




        private void ComboxDeliveryTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
         
            deliveryTime = Int32.Parse(ComboxDeliveryTime.SelectedItem.ToString());
            
        }

        private void ButtonMini_Click(object sender, RoutedEventArgs e)
        {
            
            AddToWaitinglist(PackageType.Mini, double.Parse(ComboxDeliveryTime.SelectedItem.ToString()));
        }
        private void ButtonStandaard_Click(object sender, RoutedEventArgs e)
        {
            AddToWaitinglist(PackageType.Standard, double.Parse(ComboxDeliveryTime.SelectedItem.ToString()));
        }
        private void buttonMaxi_Click(object sender, RoutedEventArgs e)
        {
            AddToWaitinglist(PackageType.Maxi, double.Parse(ComboxDeliveryTime.SelectedItem.ToString()));
        }
        

        private void CheckBoxPrior_Checked(object sender, RoutedEventArgs e)
        {

        }
        void AddToWaitinglist(PackageType packageType, double traveltime)
        {
            double realTravelTime = 0;
            if (packageType == PackageType.Mini ) { 
                Conversie ec = new Conversie(CalcTime.Mini);
                realTravelTime = ec(traveltime);
            }
            else if (packageType == PackageType.Standard)
            {
                Conversie ec = new Conversie(CalcTime.Standaard);
                realTravelTime = ec(traveltime);
            }
            else if (packageType == PackageType.Maxi)
            {
                Conversie ec = new Conversie(CalcTime.Maxi);
                realTravelTime = ec(traveltime);
            }


            deliveryNumber++;
            var delivery = new Delivery(packageType, deliveryTime, CheckBoxPrior.IsChecked.Value, deliveryNumber, realTravelTime);
            //  deliveries.Append(delivery);
            deliveries.Add(delivery);
            deliv = deliveries.OrderBy(x => x.deliveryNumber).OrderByDescending(a => a.prior);

            ListBoxWaiting.Items.Clear();
            foreach (Delivery d in deliv)
            {
                ListBoxWaiting.Items.Add(d);
            }
        }

    }
}
