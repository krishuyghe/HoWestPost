using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace HoWestPost.Domain
{
    public class DeliveryProcessor
    {
        #region Globale Variabelen
        private Timer timer;
        public event TiktHandler Tick;
        public int deliveryTime = 30;

        public DateTime startTime;
        
        public int deliveryNumber = 0;
        public List<Delivery> deliveries = new List<Delivery>();
        public List<Delivery> sentPackets = new List<Delivery>();

        // array gemaakt nu nog maar 1 drone in de toekomst misschien meerdere
        public Delivery[] activeDelivery = { null };
        

        //  public Delivery activeDelivery;
        #endregion

        #region Constructor

        public DeliveryProcessor()
        {
            timer = new Timer();
            timer.Interval = 50;   
            timer.Elapsed += Timer_Elapsed;
        }
        #endregion

        #region Methodes
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Tick != null) Tick(this);
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }
        public void AddToWaitinglist(PackageType packageType, int traveltime, bool prior)
        {
            double realTravelTime = 0;
            if (packageType == PackageType.Mini)
            {
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
            deliveries.Add(new Delivery(packageType, traveltime, prior, deliveryNumber, realTravelTime));
            deliveries = deliveries.OrderBy(x => x.deliveryNumber).OrderByDescending(p => p.prior).ToList();
        }
        public double AverageTime(List<Delivery> sent)
        {
            double gemiddelde = 0;
            if (sent.Count() > 0)
            {                
                foreach (Delivery d in sent)
                {
                    gemiddelde += d.deliveryTime.Subtract(d.startTime).TotalSeconds;
                }
                return gemiddelde / sent.Count() * 10;
            }
            else
            {
                return gemiddelde;
            }
           
        }

    
        public  bool IsThereWorkInWaitingList ()
        {
            //voor de eerste drone voorlopig de enige
            
            bool work = false;
            if ((activeDelivery[0] == null) && (deliveries.Count() > 0))
            {
                activeDelivery[0] = deliveries.First();
                activeDelivery[0].startFlightTime = DateTime.Now;
                deliveries.Remove(activeDelivery[0]);
               // startTime = DateTime.Now;
                work = true;
            }
            return work;
        }
        
        public double TimeLeft()
        {
            // voor de eerste drone voorlopig de enige
            double value = 0;
          
            if (activeDelivery[0] != null)
            {
                double TimeDiverence = DateTime.Now.Subtract(activeDelivery[0].startFlightTime).TotalSeconds;
                if ((10 * TimeDiverence) <= activeDelivery[0].realTravelTime)
                {
                    value = activeDelivery[0].realTravelTime - (10 * TimeDiverence);
                }
                else
                {
                    activeDelivery[0].deliveryTime = DateTime.Now;
                    sentPackets.Add(activeDelivery[0]);
                    activeDelivery[0] = null;
                    value = 0;
                }
            }
            return value;
        }
        #endregion


    }


}
