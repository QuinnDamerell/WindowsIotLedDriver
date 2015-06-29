using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsIotLedDriver;

namespace LedDriverSample
{
    class VisualController : ILedControllerExtender
    {
        LedController m_baseController;
        IVisualLedListener m_listner;

        public VisualController()
        {
            // Create the base controller
            m_baseController = new LedController(this, ControlerUpdateType.AllSlots);
            m_baseController.ToggleAnimation(true, true);
        }

        public void AssoicateLed(int startingSlot, Led assoicateLed)
        {
            // Call through to the base
            m_baseController.AssoicateLed(startingSlot, assoicateLed);
        }

        public void AddVisualListener(IVisualLedListener listner)
        {
            m_listner = listner;
        }

        public void NotifiySlotsAdded(int firstSlot, int numberOfSlots)
        {
            // We don't care
        }

        public void NotifiySlotsRevmoed(int firstSlot, int numberOfSlots)
        {
            // We don't care
        }

        public void NotifiySlotStateChanged(int slot, double newValue)
        {
            if (m_listner != null)
            {
                // Send a update to only one slot
                m_listner.UpdateVisualLed(slot, (byte)(newValue * 255));
            }
        }

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
