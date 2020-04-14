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
        public DateTime startTime;
        int deliveryNumber = 0;
        List<Delivery> deliveries = new List<Delivery>();
        List<Delivery> sentPackets = new List<Delivery>();
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
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += new EventHandler(dispatcherTimer_Tick);
            timer.Start();
        }
      
        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            lblTime.Content = DateTime.Now.ToLongTimeString();
            if ((activeDelivery == null) && (deliveries.Count() > 0))
            {
                    activeDelivery = deliveries.First();
                    deliveries.Remove(activeDelivery);
                    startTime = DateTime.Now;

                    lblPrior.Content = activeDelivery.prior;
                    lblTotalTravelTime.Content = activeDelivery.realTravelTime.ToString() + "min";
                    lblTimeLeft1.Content = activeDelivery.realTravelTime.ToString();
                    lblType.Content = activeDelivery.packageType;
                    lblPacketNumber.Content = activeDelivery.deliveryNumber;
                    progressBar.Value = 0;

                    ListBoxWaiting.Items.Clear();
                    foreach (Delivery d in deliveries)
                    {
                        ListBoxWaiting.Items.Add(d);
                    }
                


            }
            if (activeDelivery != null)
            {
                
                double TimeDiverence = DateTime.Now.Subtract(startTime).TotalSeconds;
                progressBar.Value = ((TimeDiverence * 10) / activeDelivery.realTravelTime) * 100;
                if ((10 * TimeDiverence) <= activeDelivery.realTravelTime)
                {
                    lblTimeLeft1.Content = activeDelivery.realTravelTime - (10 * TimeDiverence);
                }
                else
                {
                    activeDelivery.deliveryTime = DateTime.Now;
                    sentPackets.Add(activeDelivery);
                    ListBoxSent.Items.Add(activeDelivery);
                    activeDelivery = null;
                    lblTimeLeft1.Content = 0;
                }
            }
            if (sentPackets.Count() > 0)
            {
                double gemiddelde = 0;
                foreach (Delivery d in sentPackets)
                {
                    gemiddelde += d.deliveryTime.Subtract(d.startTime).TotalSeconds;
                }
                lblGemiddelde.Content = gemiddelde / sentPackets.Count();
            }
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
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
            deliveries.Add(new Delivery(packageType, deliveryTime, CheckBoxPrior.IsChecked.Value, deliveryNumber, realTravelTime));
            deliveries = deliveries.OrderBy(x => x.deliveryNumber).OrderByDescending(p => p.prior).ToList();  
            ListBoxWaiting.Items.Clear();
            foreach (Delivery d in deliveries)
            {
                ListBoxWaiting.Items.Add(d);
            }
        }

    }
}
