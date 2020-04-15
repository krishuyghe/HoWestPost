using System;

using System.Timers;

namespace HoWestPost.Domain
{
    public class DeliveryProcessor
    {
        #region Globale Variabelen
        private Timer timer;
        public event TiktHandler Tick;
        #endregion

        #region Constructor(s)

        public DeliveryProcessor()
        {
            timer = new Timer();
            timer.Interval = 1;   
            timer.Elapsed += Timer_Elapsed;
        }
        #endregion

        #region Klassemethoden
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Tick(this);
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }


        #endregion


    }


}
