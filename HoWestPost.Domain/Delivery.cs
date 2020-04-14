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
            string ListText = $"{deliveryNumber} Pr={prior} {packageType} Time={travelTime} Real={realTravelTime} StartTime={startTime} DeliveryTime={deliveryTime}";
            return ListText;
        }

    }
}
