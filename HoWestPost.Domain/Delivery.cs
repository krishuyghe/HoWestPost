using System;
using System.Collections.Generic;
using System.Text;

namespace HoWestPost.Domain
{
    public class Delivery
    {
        private PackageType packageType;
        private int travelTime;
        private bool prior;
        private DateTime startTime;
        private int deliveryNumber; 

        public Delivery(PackageType packageType, int travelTimeToDestination , bool prior, int deliveryNumber)
        {
            this.packageType = packageType;
            this.travelTime = travelTimeToDestination;
            this.prior = prior;
            startTime = DateTime.Now;
            this.deliveryNumber = deliveryNumber;
        }
        public override string ToString()
        {
            string ListText = $"nr={deliveryNumber} Prior={prior} Type ={packageType} Time={travelTime}";
            return ListText;
        }
    }
}
