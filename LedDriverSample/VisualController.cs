using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsIotLedDriver;

namespace LedDriverSample
{
    class VisualController : ILedChangeNotification
    {
        LedController m_baseController;
        IVisualLedListener m_listner;

        public VisualController()
        {
            // Create the base controller
            m_baseController = new LedController(this);
        }

        public void AssoicateLed(Led assoicateLed)
        {
            // Call through to the base
            m_baseController.AssoicateLed(assoicateLed);
        }

        public void AddVisualListener(IVisualLedListener listner)
        {
            m_listner = listner;
        }

        // Called when an LEDs value changes.
        public void NotifiyLedChange(int LedId)
        {
            if(m_listner != null)
            {
                // Get the LED state that changed.
                LedType type;
                double red, blue, green, intensity;
                m_baseController.GetLedState(LedId, out type, out red, out green, out blue, out intensity);

                // Convert the outputs to bytes
                byte redByte = (byte)(red * 255 * intensity);
                byte blueByte = (byte)(blue * 255 * intensity);
                byte greenByte = (byte)(green * 255 * intensity);

                // Pass the update along
                m_listner.UpdateVisualLed(LedId, redByte, greenByte, blueByte);
            }
        }
    }
}
