using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysLogger.Hardware
{
    public class PlotLabel
    {
        public string Name { get; set; }
        public MeasurementUnit Unit { get; set; }
        public override string ToString()
        {
            return Name + " (" + Unit.ToString() + ")";
        }
    }
}
