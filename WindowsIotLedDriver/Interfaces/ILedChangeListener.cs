using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsIotLedDriver
{
    // Indicates what value was changed.
    public enum LedChangeValue
    {
        All,
        Red,
        Green,
        Blue
    }

    // An interface for the LED to talk to the controller
    internal interface ILedChangeListener
    {
        // Fired when a LED value has changed
        void NotifiyLedChange(int baseSlot, LedChangeValue changedValue);

        // Used for the LED to register for animation callbacks.
        void ResigerForAnimationCallbacks(IAnimationTickListner listener);
    }
}
