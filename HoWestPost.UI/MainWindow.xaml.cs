using HoWestPost.Domain;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace HoWestPost.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Global variable
        
        DeliveryProcessor deliveryProcessor;
        #endregion
        #region MainWindow
        public MainWindow()
        {
            InitializeComponent();
            FillComboxDeliveryTime();
        }
        #endregion
        #region window open/closed
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            deliveryProcessor = new DeliveryProcessor();
            // Toevoegen van event => Link naar methode voor afhandeling
            deliveryProcessor.Tick += DeliveryProcessor_Tick;
            deliveryProcessor.Start();
        }
         private void MainWindow_Closed(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Zorgt ervoor dat de timer niet blijft lopen als process (thread)
            if (deliveryProcessor != null)
            {
                deliveryProcessor.Stop();
            }
        }
        #endregion
        #region DeliveryProcessor tick
        private void DeliveryProcessor_Tick(object sender)
        {
            Dispatcher.Invoke(delegate
            {
                lblTime.Content = DateTime.Now.ToLongTimeString();
            
                lblGemiddelde.Content = deliveryProcessor.AverageTime(deliveryProcessor.sentPackets);
                lblGemiddeldeNonPrior.Content = deliveryProcessor.AverageTime(deliveryProcessor.sentPackets.Where(_ => _.prior == false).ToList());
                lblGemiddelde_Pror.Content = deliveryProcessor.AverageTime(deliveryProcessor.sentPackets.Where(_ => _.prior == true).ToList());

                if (deliveryProcessor.IsThereWorkInWaitingList() == true)
                {
                    lblPrior.Content = deliveryProcessor.activeDelivery.prior;
                    lblTotalTravelTime.Content = deliveryProcessor.activeDelivery.realTravelTime.ToString() + "min";
                    lblTimeLeft1.Content = deliveryProcessor.activeDelivery.realTravelTime.ToString();
                    lblType.Content = deliveryProcessor.activeDelivery.packageType;
                    lblPacketNumber.Content = deliveryProcessor.activeDelivery.deliveryNumber;
                    progressBar.Value = 0;
                    ListBoxWaiting.Items.Clear();
                    foreach (Delivery d in deliveryProcessor.deliveries)
                    {
                        ListBoxWaiting.Items.Add(d);
                    }
                }


                lblTimeLeft1.Content = deliveryProcessor.TimeLeft();
                if (deliveryProcessor.TimeLeft() > 0)
                {
                    progressBar.Value = 100 - ((deliveryProcessor.TimeLeft()/deliveryProcessor.activeDelivery.realTravelTime  ) * 100);
                }
                if (deliveryProcessor.sentPackets.Count() > 0)
                {
                    ListBoxSent.Items.Clear();
                    foreach (Delivery d in deliveryProcessor.sentPackets)
                    {
                        ListBoxWaiting.Items.Add(d);
                    }
                }



            });
        }
        
        #endregion

      
        #region button mini standaard maxi 
        private void ButtonMini_Click(object sender, RoutedEventArgs e)
        {

            deliveryProcessor.AddToWaitinglist(PackageType.Mini, int.Parse(ComboxDeliveryTime.SelectedItem.ToString()), CheckBoxPrior.IsChecked.Value);
            UpdateWaitingList();
        }
        private void ButtonStandaard_Click(object sender, RoutedEventArgs e)
        {
            deliveryProcessor.AddToWaitinglist(PackageType.Standard, int.Parse(ComboxDeliveryTime.SelectedItem.ToString()), CheckBoxPrior.IsChecked.Value);
            UpdateWaitingList();
        }
        private void buttonMaxi_Click(object sender, RoutedEventArgs e)
        {
            deliveryProcessor.AddToWaitinglist(PackageType.Maxi, int.Parse(ComboxDeliveryTime.SelectedItem.ToString()), CheckBoxPrior.IsChecked.Value);
            UpdateWaitingList();
        }
        #endregion
        
        #region UpdateWaitinglist
        
        private void UpdateWaitingList ()
        {
            ListBoxWaiting.Items.Clear();
            foreach (Delivery d in deliveryProcessor.deliveries)
            {
                ListBoxWaiting.Items.Add(d);
            }
        }
        #endregion
        private void FillComboxDeliveryTime ()
        {
            int counter = 30;
            while (counter < 95)
            {
                ComboxDeliveryTime.Items.Add(counter);
                counter += 5;
            }
            ComboxDeliveryTime.SelectedIndex = 0;
        }

        private void ComboxDeliveryTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
        private void CheckBoxPrior_Checked(object sender, RoutedEventArgs e)
        {
        }
    }
}
