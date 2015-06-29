using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsIotLedDriver
{
    // An interface used to get callbacks from the animator or in a chain of animation. 
    internal interface IAnimationTickListner
    {
        // This callback will be fired on every animation tick. The time given is the
        // time since the last animation tick.
        bool NotifiyAnimationTick(int timeElapsedMs);
    }
}
