using System;
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
            if (diverent > 0)
            {
                ListText = $"{deliveryNumber} Pr={prior} {packageType} Time={travelTime} Real={realTravelTime} StartTime={startTime}";
            }
            else
            {
                ListText = $"{deliveryNumber} Pr={prior} {packageType} StartTime={startTime} DeliveryTime={deliveryTime}";
            }
            return ListText;
        }

        private void Compare(int dateTime1, object dateTime2, int dateTime3, object d2)
        {
            throw new NotImplementedException();
        }
    }
}
