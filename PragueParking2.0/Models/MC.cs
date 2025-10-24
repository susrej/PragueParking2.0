using PragueParking2._0.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking2._0.Models
{
    public class MC : Vehicle
    {
        public MC(string regNumber) : base(regNumber, "MC") { }

        //beräkna parkeringsavgift för MC
        public double CalculateCostForMC(Config config)
        {
            return CalculateCost(config.FreeMinutes, config.MCRatePerHour);
        }
    }
}
