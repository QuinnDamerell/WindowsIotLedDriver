using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsIotLedDriver
{
    public sealed class PwmLedController
    {
        // Holds a reference to the base controller class
        PwmLedControllerBase m_base;

        public PwmLedController()
        {
            m_base = new PwmLedControllerBase();
        }

        public void AssoicateLed(int startingPosition, Led assoicateLed)
        {
            m_base.AssoicateLed(startingPosition, assoicateLed);
        }

        public void DissociateLed(Led dissociateLed)
        {
            m_base.DissociateLed(dissociateLed);
        }

        public void ToggleAnimation(bool enableAnmation, bool alwaysPaint)
        {
            m_base.ToggleAnimation(enableAnmation, alwaysPaint);
        }

        public void ToggleAnimation(bool enableAnmation, bool alwaysPaint, int animationRateMilliseconds)
        {
            m_base.ToggleAnimation(enableAnmation, alwaysPaint, animationRateMilliseconds);
        }
    }
}
