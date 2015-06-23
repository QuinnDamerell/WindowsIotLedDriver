using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsIotLedDriver
{
    public sealed class LedController : ILedChangeNotification
    {
        private Dictionary<int, WeakReference<Led>> m_ledMap;
        int m_nextLedId;
        ILedChangeNotification m_extendedController;

        public LedController(ILedChangeNotification extendedController)
        {
            m_extendedController = extendedController;
            m_ledMap = new Dictionary<int, WeakReference<Led>>();
            m_nextLedId = 1;
        }

        // Binds an LED object to a controller.
        public void AssoicateLed(Led assoicateLed)
        {
            if(assoicateLed == null)
            {
                throw new ArgumentNullException("Led can't be null!");
            }

            // Lock the map and add the new LED
            int ledId = 0;
            lock (m_ledMap)
            {
                ledId = m_nextLedId++;
                m_ledMap.Add(ledId, new WeakReference<Led>(assoicateLed));
            }

            // Inform the LED of the association
            assoicateLed.SetNotificationCallback(ledId, this);
        }

        public void GetLedState(int LedId, out LedType type, out double red, out double green, out double blue, out double intesity)
        {
            // Check if the LED exists in the map            
            if(!m_ledMap.ContainsKey(LedId))
            {
                // The map doesn't contain the LED
                throw new ArgumentOutOfRangeException("The requested Led Id can't be found");
            }

            // Try to get the LED
            Led foundLed = null;
            m_ledMap[LedId].TryGetTarget(out foundLed);

            // Is now gone, throw an exception.
            if(foundLed == null)
            {
                throw new Exception("The LED no longer exists");
            }

            // Finally get the state
            foundLed.GetLedState(out type, out red, out green, out  blue, out intesity);
        }

        // Called when an LEDs value changes. We want the extened controller to handle this.
        public void NotifiyLedChange(int LedId)
        {
            if(m_extendedController != null)
            {
                m_extendedController.NotifiyLedChange(LedId);
            }
        }
    }
}
