using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsIotLedDriver;

namespace LedDriverSample
{
    // This is a reference example of implementing a controller. A controller must extend the base controller, which drives the controller
    // The purpose of an extended controller is to handle incoming data and do the correct thing for what it is controlling. For example a software PWM
    // controller would take slot value updates and update the PWM for that slot to the new value. a SPI controller might take entire slot updates and
    // push new data to the SPI device when any one value updates.

    // The propose of this controller is to push slot updates to the UI so they can "update" the LEDs on the screen. 
    class VisualLedController : ILedControllerExtender
    {
        // Hold a reference to the base controller
        LedController m_baseController;

        // Used only for the visual controller, this gives us a reference to the UI.
        IVisualLedListener m_listner;

        public VisualLedController()
        {
            // Create the base controller, specifiy how we want upates; if want everything everytime, or if we want signle slot updates.
            m_baseController = new LedController(this, ControlerUpdateType.AllSlots);

            // Enable animation on the base controller. This must be enabled for led animation.
            m_baseController.ToggleAnimation(true, false);
        }

        // Called by the consumer when a new LED is associated to the controller
        public void AssociateLed(int startingSlot, Led assoicateLed)
        {
            // Call through to the base
            m_baseController.AssociateLed(startingSlot, assoicateLed);
        }

        // Used only by the visual controller, this is how we get a ref to the UI.
        public void AddVisualListener(IVisualLedListener listner)
        {
            m_listner = listner;
        }


        // Called by the base controller when a new slot has been added. The controller should setup whatever is needed, if it fails
        // it should throw.
        public void NotifiySlotsAdded(int firstSlot, int numberOfSlots)
        {
            // We don't care for this controller
        }

        // Called by the base controller when a slot has been removed.
        public void NotifiySlotsRevmoed(int firstSlot, int numberOfSlots)
        {
            // We don't care for this controller
        }

        // Called by the base controller when a single slot has changed value. This will only be called if the controller asked for 
        // single slot updates
        public void NotifiySlotStateChanged(int slot, double newValue)
        {
            if (m_listner != null)
            {
                // Send a update to only one slot
                m_listner.UpdateVisualLed(slot, (byte)(newValue * 255));
            }
        }

        // Called by the base controller when a value has changed, but this will give us all of the slot values. This will be called when 
        // the controller asked for all slot updates.
        public void NotifiySlotsStateChanged(IReadOnlyList<double> newVaules)
        {
            if (m_listner != null)
            {
                // Send a update for each LED
                m_listner.UpdateVisualLed(1, (byte)(newVaules[0] * 255), (byte)(newVaules[1] * 255), (byte)(newVaules[2] * 255));
                m_listner.UpdateVisualLed(2, (byte)(newVaules[3] * 255), (byte)(newVaules[4] * 255), (byte)(newVaules[5] * 255));
                m_listner.UpdateVisualLed(3, (byte)(newVaules[6] * 255), (byte)(newVaules[7] * 255), (byte)(newVaules[8] * 255));
                m_listner.UpdateVisualLed(4, (byte)(newVaules[9] * 255), (byte)(newVaules[10] * 255), (byte)(newVaules[11] * 255));
                m_listner.UpdateVisualLed(5, (byte)(newVaules[12] * 255), (byte)(newVaules[13] * 255), (byte)(newVaules[14] * 255));
            }
        }
    }
}
