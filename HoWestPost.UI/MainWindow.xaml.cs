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
            ClearGui();
            FillComboxDeliveryTime();
            
        }
        #endregion
        #region window open/closed

        //wanneer het window geladen wordt
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            deliveryProcessor = new DeliveryProcessor();
            // Toevoegen van event => Link naar methode voor afhandeling
            deliveryProcessor.Tick += DeliveryProcessor_Tick;
            
            deliveryProcessor.Start();
           
        }
        // wanneer het scherm gesloten wordt
         private void MainWindow_Closed(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (deliveryProcessor != null)
            {
                deliveryProcessor.Stop();
            }
        }
        #endregion
        #region DeliveryProcessor tick
        // bij iedere tick overloopt hij deze functies 
        void DeliveryProcessor_Tick(object sender)
        {
            Dispatcher.Invoke(delegate
            {
                lblTime.Content = DateTime.Now.ToLongTimeString();
                Thread isThereWorkinWaitinglist = new Thread(IsThereWorkInWaitingList);
                Thread calculateAvarage = new Thread(CalculateAvarage);
                Thread printTimeleft = new Thread(PrintTimeleft);
                Thread progresBarLeftTime = new Thread(ProgresBarLeftTime);
                Thread addListboxSent = new Thread(AddListboxSent);
                
                calculateAvarage.Start();
                isThereWorkinWaitinglist.Start();
                printTimeleft.Start();
                progresBarLeftTime.Start();
                addListboxSent.Start();

                if (deliveryProcessor.CanIStop() == true) deliveryProcessor.Stop();
            });
        }
        #endregion
        #region Thread voids
        // beheerd de addlistbox
        private void AddListboxSent()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                lblPakketten.Content = deliveryProcessor.sentPackets.Count().ToString();
                if (deliveryProcessor.sentPackets.Count() > ListBoxSent.Items.Count)
                {
                    ListBoxSent.Items.Clear();
                    foreach (Delivery s in deliveryProcessor.sentPackets)
                    {
                        ListBoxSent.Items.Add(s);
                    }
                }

            }));
        }
        // beheerd de progressbar 
        private void ProgresBarLeftTime()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                
                if (deliveryProcessor.TimeLeft()[0] > 0)
                {
                   
                    progressBar.Value = 100 - (deliveryProcessor.TimeLeft()[0] / deliveryProcessor.activeDelivery[0].realTravelTime * 100) ;
                    
                }
                
            }));
            
        }
        // drukt de tijd af dat de drones nog onderweg zijn
        private void PrintTimeleft()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                lblTimeLeft1.Content = deliveryProcessor.TimeLeft()[0].ToString("#0.00");
                drone.Children.Clear();
                for (int i = 0; i < deliveryProcessor.activeDelivery.Length; i++)
                {
                    
                    System.Windows.Controls.Label label = new Label();
                    label.Content = "Drone" + (i+1) + " Tijd Rest:" + deliveryProcessor.TimeLeft()[i].ToString("#0.00") ;
                    label.Name = "dynamiclabel" + i.ToString();
                    drone.Children.Add(label);
                }

            }));
        }
        // berekend de verschillende gemiddelden
        private void CalculateAvarage()
        {

            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                lblGemiddelde.Content = deliveryProcessor.AverageTime(deliveryProcessor.sentPackets).ToString("#0.00");
                lblGemiddeldeNonPrior.Content = deliveryProcessor.AverageTime(deliveryProcessor.sentPackets.Where(_ => _.prior == false).ToList()).ToString("#0.00");
                lblGemiddelde_Pror.Content = deliveryProcessor.AverageTime(deliveryProcessor.sentPackets.Where(_ => _.prior == true).ToList()).ToString("#0.00");
                lblGemiddeldeMaxi.Content = deliveryProcessor.AverageTime(deliveryProcessor.sentPackets.Where(_ => _.packageType == PackageType.Maxi).ToList()).ToString("0.00");
                lblGemiddeldeMini.Content = deliveryProcessor.AverageTime(deliveryProcessor.sentPackets.Where(_ => _.packageType == PackageType.Mini).ToList()).ToString("0.00");
                lblGemiddeldeStandaard.Content = deliveryProcessor.AverageTime(deliveryProcessor.sentPackets.Where(_ => _.packageType == PackageType.Standard).ToList()).ToString("0.00");
            }));
        }
        // kijk na of er nog werk in in wachtlijst 
        private void IsThereWorkInWaitingList()
        {

            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (deliveryProcessor.IsThereWorkInWaitingList() == true)
                {
                    //voor de eerste drone voorlopig de enige 
                    lblPrior.Content = deliveryProcessor.activeDelivery[0].prior;
                    lblTotalTravelTime.Content = deliveryProcessor.activeDelivery[0].realTravelTime.ToString() + "min";
                    lblTimeLeft1.Content = deliveryProcessor.activeDelivery[0].realTravelTime.ToString();
                    lblType.Content = deliveryProcessor.activeDelivery[0].packageType;
                    lblPacketNumber.Content = deliveryProcessor.activeDelivery[0].deliveryNumber;
                    progressBar.Value = 0;
                    ListBoxWaiting.Items.Clear();
                    foreach (Delivery d in deliveryProcessor.deliveries)
                    {
                        ListBoxWaiting.Items.Add(d);
                    }
                  
                }
                if (deliveryProcessor.activeDelivery[0] == null )
                {
                    lblPrior.Content = "";
                    lblTotalTravelTime.Content = "";
                    lblType.Content = "";
                    lblPacketNumber.Content = "";
                    progressBar.Value = 0;
                }


            }));
        }
#endregion


                #region button mini standaard maxi 
        
        private void ButtonMini_Click(object sender, RoutedEventArgs e)
        {
            deliveryProcessor.AddToWaitinglist(PackageType.Mini, int.Parse(ComboxDeliveryTime.SelectedItem.ToString()), CheckBoxPrior.IsChecked.Value);
            UpdateWaitingList();
            deliveryProcessor.Start();
        }
       
       

        private void ButtonStandaard_Click(object sender, RoutedEventArgs e)
        {
            deliveryProcessor.AddToWaitinglist(PackageType.Standard, int.Parse(ComboxDeliveryTime.SelectedItem.ToString()), CheckBoxPrior.IsChecked.Value);
            UpdateWaitingList();
            
            deliveryProcessor.Start();
        }
        private void buttonMaxi_Click(object sender, RoutedEventArgs e)
        {
            deliveryProcessor.AddToWaitinglist(PackageType.Maxi, int.Parse(ComboxDeliveryTime.SelectedItem.ToString()), CheckBoxPrior.IsChecked.Value);
            UpdateWaitingList();
            deliveryProcessor.Start();
        }
                #endregion
                #region clear gui and fill combobox 
        private void ClearGui()
        {
            lblPrior.Content = "";
            lblTotalTravelTime.Content = "";
            lblTimeLeft1.Content = "";
            lblType.Content = "";
            lblPacketNumber.Content = "";
            progressBar.Value = 0;
            ListBoxWaiting.Items.Clear();
            ListBoxSent.Items.Clear();
        }
        // vult de combobox met de mogelijke tijdduur tussen 30 - 90
        private void FillComboxDeliveryTime()
        {
            int counter = 30;
            while (counter < 95)
            {
                ComboxDeliveryTime.Items.Add(counter);
                counter += 5;
            }
            ComboxDeliveryTime.SelectedIndex = 0;
        }
                #endregion
                #region UpdateWaitinglist
        // update de wachtlijst in de gui
        private void UpdateWaitingList ()
        {
            ListBoxWaiting.Items.Clear();
            foreach (Delivery d in deliveryProcessor.deliveries)
            {
                ListBoxWaiting.Items.Add(d);
            }
        }
                #endregion
        // stop het proces bv bij slecht weer of einde werkdag
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            deliveryProcessor.Stop();
           
        }
        // herstarten van het proces
        private void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            
            deliveryProcessor.Start();
        }
        private void ComboxDeliveryTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
        private void CheckBoxPrior_Checked(object sender, RoutedEventArgs e)
        {
        }
    }
}
