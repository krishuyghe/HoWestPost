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
        public event TiktHan   dler Tick;
        public int deliveryTime = 30;

        public DateTime startTime;
        
        public int deliveryNumber = 0;
        public List<Delivery> deliveries = new List<Delivery>();
        public List<Delivery> sentPackets = new List<Delivery>();
        public Delivery activeDelivery;
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
            bool work = false;
            if ((activeDelivery == null) && (deliveries.Count() > 0))
            {
                activeDelivery = deliveries.First();
                deliveries.Remove(activeDelivery);
                startTime = DateTime.Now;
                work = true;
            }
            return work;
        }
        public double TimeLeft()
        {
            double value = 0;
            if (activeDelivery != null)
            {
                double TimeDiverence = DateTime.Now.Subtract(startTime).TotalSeconds;
                if ((10 * TimeDiverence) <= activeDelivery.realTravelTime)
                {
                    value = activeDelivery.realTravelTime - (10 * TimeDiverence);
                }
                else
                {
                    activeDelivery.deliveryTime = DateTime.Now;
                    sentPackets.Add(activeDelivery);
                    activeDelivery = null;
                    value = 0;
                }
            }
            return value;
        }
        #endregion


    }


}
