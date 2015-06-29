using System.Collections.Generic;

namespace WindowsIotLedDriver
{
    public interface ILedControllerExtender
    {
        // Informs the controller that a slot has been added. The implementer should throw 
        // if the slot can't be setup properly.
        void NotifiySlotsAdded(int firstSlot, int numberOfSlots);

        // Informs the controller that a slot has been removed. The implementer should throw 
        // if the slot can't be setup properly.
        void NotifiySlotsRevmoed(int firstSlot, int numberOfSlots);

        // Used if the controller update type is set to SingleSlot.
        // This callback will be fired every time one of the slots has an updated value.
        void NotifiySlotStateChanged(int Slot, double newValue);

        // Used if the controller update type is set to AllSlots.
        // This callback will be fired every time the entire slot state needs updated.
        void NotifiySlotsStateChanged(IReadOnlyList<double> newVaules);
    }
}
