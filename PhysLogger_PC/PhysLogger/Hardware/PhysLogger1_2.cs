using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO.Ports;
using FivePointNine.Windows.IO;
using PhysLogger.LogControls;

namespace PhysLogger.Hardware
{
    public class PhysLogger1_2HW : PhysLogger1_1HW
    {
        protected override float MaxInputVoltage { get; set; } = 10;
        public PhysLogger1_2HW()
        {
            Signature = PhysLoggerHWSignature.PhysLogger1_2;
        }
    }
}
