using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsIotLedDriver
{
    internal class LedBase : IReadableLed
    {
        //
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
                // Fire 0 indicating that red changed.
                FireLedChangeNotificaiton(LedChangeValue.Red);
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
                // Fire 0 indicating that green changed.
                FireLedChangeNotificaiton(LedChangeValue.Green);
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
                // Fire 0 indicating that blue changed.
                FireLedChangeNotificaiton(LedChangeValue.Blue);
            }
        }

        public double Intensity
        {
            get { return m_intensity; }
            set
            {
                if (value < 0 || value > 1.0)
                {
                    throw new ArgumentOutOfRangeException("Intensity has to be between 0 and 1.");
                }
                m_intensity = value;
                // Fire all of the values indicating they all changed.
                FireLedChangeNotificaiton(LedChangeValue.All);
            }
        }

        public LedType Type { get { return m_type; } }

        //
        // Internal getters and setters
        //

        // Returns the first slot that this LED holds for the controller
        internal int FirstSlot
        {
            get
            {
                return m_firstSlot;
            }
        }

        // Returns how many slots this LED occupies on the controller.
        internal int SlotsRequired
        {
            get
            {
                return Type == LedType.RBG ? 3 : 1;
            }
        }

        //
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
        private ILedChangeListener m_controller = null;
        private int m_firstSlot = -1;
        IAnimationTickListner m_animationTickListener = null;

        public LedBase(LedType type)
        {
            m_type = type;
        }

        //
        // Internal Functions
        //

        // Called by the controller to query the current state of the LED
        public void GetLedState(out LedType type, out double red, out double green, out double blue, out double intesity)
        {
            type = m_type;
            red = m_red;
            green = m_green;
            blue = m_blue;
            intesity = m_intensity;
        }

        // Called by the controller to associate the LED with a controller.
        // The controller tells us what the first slot the LED is in is.
        public void SetNotificationCallback(int firstSlot, ILedChangeListener callback)
        {
            m_firstSlot = firstSlot;
            m_controller = callback;

            // Also register for animation if needed
            if(m_animationTickListener != null)
            {
                m_controller.ResigerForAnimationCallbacks(m_animationTickListener);
            }
        }
        
        // Called by the controller to remove the association.
        public void RemoveNotificationCallback()
        {
            m_firstSlot = -1;
            m_controller = null;
        }

        // Called by the animated LED class. This function should register with the controller 
        // for animation callbacks.
        public void ToggleResigerForAnimationTicks(bool regsiter, IAnimationTickListner listener)
        {
            if (regsiter)
            {
                m_animationTickListener = listener;
            }
            else
            {
                m_animationTickListener = null;
            }

            // Only register if the controller exists
            if (m_animationTickListener != null && m_controller != null)
            {
                m_controller.ResigerForAnimationCallbacks(listener);
            }
        }

        // Tells the controller that a value has changed.
        private void FireLedChangeNotificaiton(LedChangeValue valueChanged)
        {
            // Try to get the controller
            if (m_controller != null && m_firstSlot != -1)
            {
                m_controller.NotifiyLedChange(m_firstSlot, valueChanged);
            }
        }
    }
}
