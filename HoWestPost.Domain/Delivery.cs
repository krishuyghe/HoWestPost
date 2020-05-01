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
        public DateTime startFlightTime;
        public DateTime deliveryTime;
        public int deliveryNumber;
        public bool prior;
        public double realTravelTime;
        public int droneNumber;
        #region controler
        public Delivery(PackageType packageType, int travelTimeToDestination , bool prior, int deliveryNumber, double realTravelTime)
        {
            this.packageType = packageType;
            this.travelTime = travelTimeToDestination;
            this.prior = prior;
            startTime = DateTime.Now;
            this.deliveryNumber = deliveryNumber;
            this.realTravelTime = realTravelTime;
        }
        #endregion
        #region override ToString methode
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
                ListText = $"{deliveryNumber} {pr} {packageType} Reistijd={travelTime}  Start-tijd={startTime.ToLongTimeString()}";
            }
            else
            {
                 ListText = $"{deliveryNumber} {pr} {packageType} Start-tijd={startTime.ToLongTimeString()} Start vlucht={startFlightTime.ToLongTimeString()} Afgeleverd={deliveryTime.ToLongTimeString()} drone:{droneNumber} ";
            }
            return ListText;
            
        }
        
        #endregion

    }
}
