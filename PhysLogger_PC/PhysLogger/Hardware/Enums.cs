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
        I2C = 2,
        Instrument = 3,
        Function = 5,
        None = 4,
    }
}
