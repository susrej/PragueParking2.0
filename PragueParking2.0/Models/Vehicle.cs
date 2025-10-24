using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking2._0.Models
{
    public class Vehicle
    {
        public string RegNumber { get; set; }
        public string Type { get; set; }
        public DateTime CheckInTime { get; set; }

        public Vehicle() { }

        public Vehicle(string regNumber, string type)
        {
            RegNumber = regNumber;
            Type = type;
            CheckInTime = DateTime.Now;
        }

        //Beräkna parkeringsavgift
        public double CalculateCost(int freeMinutes, double ratePerHour)
        {
            TimeSpan duration = DateTime.Now - CheckInTime;
            if (duration.TotalMinutes > freeMinutes)
                return 0;

            double hours = Math.Ceiling(duration.TotalHours); //Avrundar uppåt till närmaste timme
            return hours * ratePerHour;
        }

        public override string ToString()
        {
            return $"{Type} {RegNumber} (Incheckad: {CheckInTime}";
        }
    }
}