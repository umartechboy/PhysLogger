using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysLogger.Hardware
{
    public enum ChannelType:byte
    {
        AnalogInDifferential = 0,
        AnalogInRSE = 1,
        Instrument = 2,
        Function = 3,
        None = 4,
    }
}
