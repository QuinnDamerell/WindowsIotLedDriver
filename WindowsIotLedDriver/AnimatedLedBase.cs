using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsIotLedDriver
{
    class AnimatedLedBase : IAnimationTickListner
    {
        // 
        // Private Vars
        //
        Led m_led;

        //
        // Constructor 
        //
        public AnimatedLedBase(LedType type)
        {
            m_led = new Led(type);
            ToggleResigerForAnimationTicks(true);
        }

        public AnimatedLedBase(Led led)
        {
            m_led = led;
            ToggleResigerForAnimationTicks(true);
        }

        // 
        // Public functions
        //
        public Led GetLed()
        {
            return m_led;
        }

        public bool NotifiyAnimationTick(int timeElapsedMs)
        {
            if(m_led.Intensity < .95)
            {
                m_led.Intensity += .05;
            }
            else
            {
                m_led.Intensity = 0;
                m_led.Red = 1.0;
            }

            return true;
        }

        //
        // Private functions
        //
        private void ToggleResigerForAnimationTicks(bool register)
        {
            m_led.GetBase().ToggleResigerForAnimationTicks(register, this);
        }
    }
}
