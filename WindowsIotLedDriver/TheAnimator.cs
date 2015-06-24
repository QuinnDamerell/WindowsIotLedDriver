using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsIotLedDriver
{
    class TheAnimator
    {
        //
        // Private Vars 
        //

        IAnimationTickListner m_tickListner;
        int m_animationRateMilliseconds;
        bool m_continueAnimating;
        Task m_task;
        DateTime m_lastTickTime;

        //
        // Constructor 
        //

        public TheAnimator(IAnimationTickListner listener, int animationRateMilliseconds = 100)
        {
            m_tickListner = listener;
            m_animationRateMilliseconds = animationRateMilliseconds;
            m_continueAnimating = true;
            StartThread();
        }

        public void Stop()
        {
            m_continueAnimating = false;
        }

        private void StartThread()
        {
            m_lastTickTime = DateTime.Now;
            m_task = new Task(Animate);
            m_task.Start();
        }

        private void Animate()
        {
            while(m_continueAnimating)
            {
                // Calculate the time difference
                TimeSpan diff = DateTime.Now - m_lastTickTime;                
                int timeElapsedMs = (int)diff.TotalMilliseconds;

                // Get the current time
                m_lastTickTime = DateTime.Now;

                // Call tick on the listener
                try
                {
                    m_tickListner.NotifiyAnimationTick(timeElapsedMs);
                }
                catch(Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Exception caught while notifying animation! What: " + e.Message);
                }

                // Calculate how long we should sleep
                int sleeptime = m_animationRateMilliseconds - (int)(DateTime.Now - m_lastTickTime).TotalMilliseconds;

                if(sleeptime > 0 && m_continueAnimating)
                {
                    System.Threading.Tasks.Task.Delay(sleeptime).Wait();
                }
            }
        }
    }
}
