using DocumentFormat.OpenXml.Spreadsheet;
using HoWestPost.Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        int deliveryNumber = 0;
     //   ArrayList deliveries = new ArrayList();
        List<Delivery> deliveries = new List<Delivery>();
        IEnumerable<Delivery> deliv;

        public MainWindow()
        {
            InitializeComponent();
           
            while (counter < 95)
            {
                ComboxDeliveryTime.Items.Add(counter);
                counter += 5;
            }
            ComboxDeliveryTime.SelectedIndex = 0;
           
        }
      

        private void ComboxDeliveryTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
         
            deliveryTime = Int32.Parse(ComboxDeliveryTime.SelectedItem.ToString());
            
        }

        private void ButtonMini_Click(object sender, RoutedEventArgs e)
        {
            
            AddToWaitinglist(PackageType.Mini);
        }
        private void ButtonStandaard_Click(object sender, RoutedEventArgs e)
        {
            AddToWaitinglist(PackageType.Standard);
        }
        private void buttonMaxi_Click(object sender, RoutedEventArgs e)
        {
            AddToWaitinglist(PackageType.Maxi);
        }
        

        private void CheckBoxPrior_Checked(object sender, RoutedEventArgs e)
        {

        }
        void AddToWaitinglist(PackageType packageType)
        {
            double realTravelTime = 0;
            if (packageType == PackageType.Mini ) { 
                Conversie ec = new Conversie(CalcTime.Mini);
                realTravelTime = ec(double.Parse(ComboxDeliveryTime.SelectedItem.ToString()));
            }
            if (packageType == PackageType.Standard)
            {
                Conversie ec = new Conversie(CalcTime.Standaard);
                realTravelTime = ec(double.Parse(ComboxDeliveryTime.SelectedItem.ToString()));
            }
            if (packageType == PackageType.Maxi)
            {
                Conversie ec = new Conversie(CalcTime.Maxi);
                realTravelTime = ec(double.Parse(ComboxDeliveryTime.SelectedItem.ToString()));
            }


            deliveryNumber++;
            var delivery = new Delivery(packageType, deliveryTime, CheckBoxPrior.IsChecked.Value, deliveryNumber, realTravelTime);
            //  deliveries.Append(delivery);
            deliveries.Add(delivery);
            deliv = deliveries.OrderBy(x => x.deliveryNumber);
            deliv = deliv.OrderByDescending(a => a.prior);

            
            ListBoxWaiting.Items.Clear();
            foreach (Delivery d in deliv)
            {
                ListBoxWaiting.Items.Add(d);
            }
        }

    }
}
