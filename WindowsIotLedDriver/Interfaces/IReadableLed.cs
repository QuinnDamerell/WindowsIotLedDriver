using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsIotLedDriver
{
    public interface IReadableLed
    {
        void GetLedState(out LedType type, out double red, out double green, out double blue, out double intesity);

        void SetNotificationCallback(int LedId, ILedChangeNotification callback);
    }
}
