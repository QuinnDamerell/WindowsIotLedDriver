using System.Collections.Generic;

namespace WindowsIotLedDriver
{
    /// <summary>
    /// An interface that is required to be implemented by all extenders of a controller. 
    /// This gives the consumer callbacks when controller events fire.
    /// </summary>
    public interface ILedControllerExtender
    {
        /// <summary>
        /// Notifies the extended controller when a new slot is being added. The controller should prepare the new slot (open the pin or what
        /// ever is needed) and then return. If the new slot can't be allocated this callback should throw.
        /// </summary>
        /// <param name="firstSlot">The first slot that is being added</param>
        /// <param name="numberOfSlots">The total number of slots to be added stating at the first slot, incremented linearly.</param>
        void NotifiySlotsAdded(int firstSlot, int numberOfSlots);

        /// <summary>
        /// Notifies the extended controller when a slot has been removed. The controller should clean up the slot at this time.
        /// </summary>
        /// <param name="firstSlot">e first slot that is being removed</param>
        /// <param name="numberOfSlots">The total number of slots to be added stating at the first slot, incremented linearly.</param>
        void NotifiySlotsRevmoed(int firstSlot, int numberOfSlots);

        /// <summary>
        /// Fired if the extended controller's update type is SingleSlot. When fired the controller should update the specified slot to the
        /// new value
        /// </summary>
        /// <param name="Slot">The slot number that changed</param>
        /// <param name="newValue">The new value of the slot</param>
        void NotifiySlotStateChanged(int Slot, double newValue);

        /// <summary>
        /// Fired if the extended controller's update type is AllSlots. When fired the controller should update all slots to the current values.
        /// </summary>
        /// <param name="newVaules">A list of each allocated slot with the current value</param>
        void NotifiySlotsStateChanged(IReadOnlyList<double> newVaules);
    }
}
