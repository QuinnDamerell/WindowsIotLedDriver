using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsIotLedDriver
{
    public interface IAnimationTickListner
    {
        bool NotifiyAnimationTick(int timeElapsedMs);
    }
}
