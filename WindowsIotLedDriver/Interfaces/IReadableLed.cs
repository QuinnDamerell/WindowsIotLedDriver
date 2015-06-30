using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsIotLedDriver
{
    // An interface used for the controller to read LEDs
    internal interface IReadableLed
    {
        // Gets the current state of the LED
        void GetLedState(out LedType type, out double red, out double green, out double blue, out double intesity);

        // Used to register the controller for callbacks from the LED
        void SetNotificationCallback(int LedId, ILedChangeListener callback);
    }
}
