using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsIotLedDriver
{
    public sealed class AnimatedLed
    {
        public LedType Type
        {
            get { return m_base.GetLed().Type; }
        }

        //
        // Private Vars
        //
        AnimatedLedBase m_base;

        public AnimatedLed(LedType type)
        {
            m_base = new AnimatedLedBase(type);
        }

        public AnimatedLed(Led led, bool boolThatMeansNothingForWinRt)
        {
            m_base = new AnimatedLedBase(led);
        }

        public Led GetLed()
        {
            return m_base.GetLed();
        }

        internal AnimatedLedBase GetBase()
        {
            return m_base;
        }
    }
}
