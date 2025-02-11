﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;


namespace HoWestPost.Domain
{
    public class DeliveryProcessor
    {
        #region Globale Variabelen
        private System.Timers.Timer timer;
        public event TiktHandler Tick;
        public int deliveryTime = 30;
        public DateTime startTime;
        public int deliveryNumber = 0;
        public List<Delivery> deliveries = new List<Delivery>();
        public List<Delivery> sentPackets = new List<Delivery>();

        // array gemaakt nu nog maar 1 drone in de toekomst misschien meerdere 
        public Delivery[] activeDelivery = new Delivery[1];
      

        
        #endregion

        #region Constructor

        public DeliveryProcessor()
        {
            timer = new System.Timers.Timer();
            timer.Interval = 100;   
            timer.Elapsed += Timer_Elapsed;
        }
        #endregion

        #region Methodes
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Tick?.Invoke(this);
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }
        // Maakt de leveringen aan met de juiste gegevens
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
        // berekent de gemiddelde tijd van de verzonden berichten deze funtie wordt meerdere keren gebruikt voor verschillende filters
        public double AverageTime(List<Delivery> sent)
        {
            double totaltime = 0;
            if (sent.Count() > 0)
            {                
                foreach (Delivery d in sent)
                {
                    totaltime += d.deliveryTime.Subtract(d.startTime).TotalSeconds;
                }
                return totaltime / sent.Count();
            }
            else
            {
                return totaltime;
            }
           
        }

        // is er werk in de lijst de wachtlijst? indien ja laat de drone(s) vliegen
        public  bool IsThereWorkInWaitingList ()
        {
            //voor de eerste drone voorlopig de enige
            
            bool work = false;
            for (int i = 0; i < activeDelivery.Count(); i++)
            {
                if ((activeDelivery[i] == null) && (deliveries.Count() > 0))
                {
                    activeDelivery[i] = deliveries.First();
                    activeDelivery[i].startFlightTime = DateTime.Now;
                    activeDelivery[i].droneNumber = i + 1;
                    deliveries.Remove(activeDelivery[i]);
                    Thread fly = new Thread(Fly);
                    
                    
                    fly.Start(activeDelivery[i]);
                    work = true;
                }
            }

            
            return work;
        }
        // pakket word verzonden tijd gaan teroverheen en als het verzonden in voeg het toe aan verzonden pakketten en verwijder het uit aktieve vezendingslijst
        private void Fly(object obj)
        {
            var delivery = (Delivery)obj;
            var realTraveltime = delivery.realTravelTime * 100;
            Thread.Sleep((int)realTraveltime);
            delivery.deliveryTime = DateTime.Now;
            sentPackets.Add(delivery);
           
            for (int i = 0; i < activeDelivery.Count(); i++)
            {
                if (activeDelivery[i] != null) {
                    if (activeDelivery[i].deliveryNumber == delivery.deliveryNumber) activeDelivery[i] = null;
                }
            }
            
            
        }
        // functie om tijdelijk te stoppen als er geen werk is dit bespaart veel processerkracht. 
        public bool CanIStop ()
        {
            bool answer = false;
            if (deliveries.Count == 0)
            {
                if (activeDelivery.Where(c => c != null).ToArray().Count() == 0) answer = true;                  
            }
            return answer; 
        }




        // geeft de tijden terug van hoelang de drone nog moet vliegen voordat dit terug komt.
        public double[] TimeLeft ()
        {
            
            double[] value = new double [activeDelivery.Length];

            for (int i = 0  ; i < activeDelivery.Count(); i++)
            {

                if (activeDelivery[i] != null)
                {
                    double TimeDiverence = DateTime.Now.Subtract(activeDelivery[i].startFlightTime).TotalSeconds;
                    if ((10 * TimeDiverence) <= activeDelivery[i].realTravelTime)
                    {
                        value[i] = activeDelivery[i].realTravelTime - (10 * TimeDiverence);
                    }
                    else
                    {
                       
                        value[i] = 0;
                    }
                }
           

            }
            
            return value;
        }
        #endregion


    }


}
