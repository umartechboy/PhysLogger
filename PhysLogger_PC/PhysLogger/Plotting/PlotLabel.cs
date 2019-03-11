using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhysLogger.Maths;

namespace PhysLogger.LogControls
{
    public class PlotLabel
    {

        public PlotLabel(string title, string units)
        {
            Name = title;
            Unit = units;   
        }

        public string Name { get; set; }
        public string Unit { get; set; }
        public override string ToString()
        {
            if (Unit.Length > 0)
                return Name + " (" + Unit.ToString() + ")";
            else return Name;
        }
    }
}
