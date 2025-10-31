using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking2._0.Core
{
    public class MC : Vehicle
    {
        public MC(string regNumber) : base(regNumber, "MC") { Size = 1; }
    }
}
