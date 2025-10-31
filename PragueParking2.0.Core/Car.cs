using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking2._0.Core

{
    public class Car : Vehicle
    {
        public Car(string regNumber) : base(regNumber, "Bil") { Size = 2; }
    }
}
