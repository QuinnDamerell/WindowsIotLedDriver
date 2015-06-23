using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsIotLedDriver
{
    public enum LedType
    {
        SingleColor,
        RBG
    }

    public sealed class Led : IReadableLed
    {
        // Public getters and setters
        //
        public double Red
        {
            get { return m_red; }
            set
            {
                if (value < 0 || value > 1.0)
                {
                    throw new ArgumentOutOfRangeException("Red has to be between 0 and 1.");
                }
                m_red = value;
                FireLedChangeNotificaiton();
            }
        }
        public double Blue
        {
            get { return m_blue; }
            set
            {
                if (value < 0 || value > 1.0)
                {
                    throw new ArgumentOutOfRangeException("Blue has to be between 0 and 1.");
                }
                m_blue = value;
                FireLedChangeNotificaiton();
            }
        }
        public double Green
        {
            get { return m_green; }
            set
            {
                if (value < 0 || value > 1.0)
                {
                    throw new ArgumentOutOfRangeException("Green has to be between 0 and 1.");
                }
                m_green = value;
                FireLedChangeNotificaiton();
            }
        }
        public double Intensity
        {
            get { return m_intensity; }
            set
            {
                if(value < 0 || value > 1.0)
                {
                    throw new ArgumentOutOfRangeException("Intensity has to be between 0 and 1.");
                }
                m_intensity = value;
                FireLedChangeNotificaiton();
            } }
        public LedType Type { get { return m_type; } }

        // Private vars
        //

        // Used to hold the current color values, only used if the LED is RGB
        private double m_red = 0.0;
        private double m_blue = 0.0;
        private double m_green = 0.0;

        // Holds the over all intensity of the LED
        private double m_intensity = 0.0;
        private LedType m_type;        

        // Holds a weak reference to the controller that owns the LED
        private ILedChangeNotification m_controller = null;
        private int Id;

        public Led(LedType type)
        {
            m_type = type;
        }

        // Called by the controller to query the current state of the LED
        public void GetLedState(out LedType type, out double red, out double green, out double blue, out double intesity)
        {
            type = m_type;
            red = m_red;
            green = m_green;
            blue = m_blue;
            intesity = m_intensity;
        }

        // Called by the controller to associate the LED with a controller
        public void SetNotificationCallback(int LedId, ILedChangeNotification callback)
        {
            Id = LedId;
            m_controller = callback;
        }

        private void FireLedChangeNotificaiton()
        {
            // Try to get the controller
            if(m_controller != null && Id != 0)
            {
                m_controller.NotifiyLedChange(Id);
            }
        }
    }
}
