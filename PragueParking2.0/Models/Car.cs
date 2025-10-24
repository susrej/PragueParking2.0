using PragueParking2._0.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking2._0.Models
{
    public class Car : Vehicle
    {
        public Car(string regNumber) : base(regNumber, "Bil") { }
        public double CalculateCostForCar(Config config)
        {
            return CalculateCost(config.FreeMinutes, config.CarRatePerHour);
        }
    }
}
