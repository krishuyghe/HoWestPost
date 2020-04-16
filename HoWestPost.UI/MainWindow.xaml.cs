using HoWestPost.Domain;
using System;
using System.Linq;
using System.Threading;
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
            if (deliveryProcessor != null)
            {
                deliveryProcessor.Stop();
            }
        }
        #endregion
        #region DeliveryProcessor tick
        void DeliveryProcessor_Tick(object sender)
        {
            Dispatcher.Invoke(delegate
            {
                lblTime.Content = DateTime.Now.ToLongTimeString();
                Thread isThereWorkinWaitinglist = new Thread(IsThereWorkInWaitingList);
                Thread calculateAvarage = new Thread(CalculateAvarage);
                Thread timeleft = new Thread(Timeleft);
                calculateAvarage.Start();
                isThereWorkinWaitinglist.Start();
                timeleft.Start();
                if (deliveryProcessor.TimeLeft() > 0)
                {
                    progressBar.Value = 100 - ((deliveryProcessor.TimeLeft() / deliveryProcessor.activeDelivery.realTravelTime) * 100);
                }
                if (deliveryProcessor.sentPackets.Count() > ListBoxSent.Items.Count)
                {
                    ListBoxSent.Items.Add(deliveryProcessor.sentPackets.Last());
                }

            });
        }

        

        private void Timeleft()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                lblTimeLeft1.Content = deliveryProcessor.TimeLeft().ToString("#0.00");
            }));
        }

        #endregion


        #region button mini standaard maxi 
        private void ButtonMini_Click(object sender, RoutedEventArgs e)
        {
            

            deliveryProcessor.AddToWaitinglist(PackageType.Mini, int.Parse(ComboxDeliveryTime.SelectedItem.ToString()), CheckBoxPrior.IsChecked.Value);
            UpdateWaitingList();
        }
        private void CalculateAvarage()
        {

            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                lblGemiddelde.Content = deliveryProcessor.AverageTime(deliveryProcessor.sentPackets).ToString("#0.00") + "min";
                lblGemiddeldeNonPrior.Content = deliveryProcessor.AverageTime(deliveryProcessor.sentPackets.Where(_ => _.prior == false).ToList()).ToString("#0.00") + "min";
                lblGemiddelde_Pror.Content = deliveryProcessor.AverageTime(deliveryProcessor.sentPackets.Where(_ => _.prior == true).ToList()).ToString("#0.00") + "min";

            }));
        }

        private void IsThereWorkInWaitingList()
        {
            
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
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
            }));
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
