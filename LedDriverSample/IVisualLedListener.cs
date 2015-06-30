using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedDriverSample
{
    // A simple interface used by the Visual Controller to tell the UI when there are updates.
    interface IVisualLedListener
    {
        void UpdateVisualLed(int LedNumber, byte red, byte green, byte blue);

        void UpdateVisualLed(int singleSlot, byte value);
    }
}
