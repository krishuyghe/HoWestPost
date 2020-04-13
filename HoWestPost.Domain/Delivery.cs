using System;
using System.Collections.Generic;
using System.Text;

namespace HoWestPost.Domain
{
    public class Delivery
    {
        private PackageType packageType;
        private int travelTime;
        private DateTime startTime;
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
            string ListText = $"{deliveryNumber} Pr={prior} {packageType} Time={travelTime} Real={realTravelTime}";
            return ListText;
        }
    }
}
