using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsIotLedDriver
{
    public enum LedChangeValue
    {
        All,
        Red,
        Green,
        Blue
    }

    internal interface ILedChangeListener
    {
        void NotifiyLedChange(int baseSlot, LedChangeValue changedValue);

        void ResigerForAnimationCallbacks(IAnimationTickListner listener);
    }
}
