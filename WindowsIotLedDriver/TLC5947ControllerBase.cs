using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsIotLedDriver
{
    class TLC5947ControllerBase : ILedControllerExtender
    {
        //
        // Public Vars
        //

        public double MasterIntensity
        {
            get
            {
                return m_controller.MasterIntensity;
            }
            set
            {
                m_controller.MasterIntensity = value;
            }
        }

        //
        // Private Vars
        // 
        LedController m_controller;

        //
        // Constructor
        //

        public TLC5947ControllerBase()
        {
            // Create the controller
            m_controller = new LedController(this, ControlerUpdateType.AllSlots); // #todo set this when known.
        }

        public void NotifiySlotsAdded(int firstSlot, int numberOfSlots)
        {
            // For this controller we don't need to do anything.
        }

        public void NotifiySlotsRevmoed(int firstSlot, int numberOfSlots)
        {
            // For this controller we don't need to do anything.
        }

        public void AssoicateLed(int startingPosition, Led assoicateLed)
        {
            m_controller.AssociateLed(startingPosition, assoicateLed);
        }

        public void DissociateLed(Led dissociateLed)
        {
            m_controller.DissociateLed(dissociateLed);
        }

        public void ToggleAnimation(bool enableAnmation)
        {
            m_controller.ToggleAnimation(enableAnmation, false); // #todo set alwayspaint when we know the correct value
        }

        public void ToggleAnimation(bool enableAnmation, int animationRateMilliseconds)
        {
            m_controller.ToggleAnimation(enableAnmation, false, animationRateMilliseconds);
        }

        public void NotifiySlotStateChanged(int Slot, double newValue)
        {
            // We are using AllSlot update mode, so this won't be called
        }

        public void NotifiySlotsStateChanged(IReadOnlyList<double> newVaules)
        {
        }
    }
}
