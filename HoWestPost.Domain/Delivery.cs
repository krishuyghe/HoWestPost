using System;
using System.Collections.Generic;
using System.Text;

namespace HoWestPost.Domain
{
    public class Delivery
    {
        private PackageType packageType;
        private int travelTime;

        public Delivery(PackageType packageType, int travelTimeToDestination)
        {
            this.packageType = packageType;
            this.travelTime = travelTimeToDestination;
        }
    }
}
