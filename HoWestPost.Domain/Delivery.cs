﻿using System;
using System.Collections.Generic;
using System.Text;

namespace HoWestPost.Domain
{
    public class Delivery
    {
        public PackageType packageType;
        public int travelTime;
        public DateTime startTime;
        public DateTime deliveryTime;
        public int deliveryNumber;
        public bool prior;
        public double realTravelTime;
        public Delivery(PackageType packageType, int travelTimeToDestination , bool prior, int deliveryNumber, double realTravelTime)
        {
            this.packageType = packageType;
            this.travelTime = travelTimeToDestination;
            this.prior = prior;
            startTime = DateTime.Now;
            this.deliveryNumber = deliveryNumber;
            this.realTravelTime = realTravelTime;
        }
        public override string ToString()
        {
            int diverent = DateTime.Compare(startTime, deliveryTime);
            string ListText = "leeg";
            string pr = "Non-prior";
            if (prior == true)
            {
                pr = "Prior";
            }
            if (diverent > 0)
            {
                ListText = $"{deliveryNumber} {pr} {packageType} Reistijd={travelTime} Starttijd={startTime.ToLongTimeString()}";
            }
            else
            {
                double tijdsduur = deliveryTime.Subtract(startTime).TotalSeconds * 10;
                ListText = $"{deliveryNumber} {pr} {packageType} Starttijd={startTime.ToLongTimeString()} Afgeleverd={deliveryTime.ToLongTimeString()} Tijdsduur:{tijdsduur.ToString("#0")}min";
            }
            return ListText;
        }

        
    }
}
