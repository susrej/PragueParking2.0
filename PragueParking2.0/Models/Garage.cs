using PragueParking2._0.DataAccess;
using PragueParking2._0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Xml.Linq;
using System.ComponentModel.Design;

namespace PragueParking2._0.Models
{
    public class Garage
    {
        public List<ParkingSpot> Spots { get; set; } = new List<ParkingSpot>();

        public Garage() { }
        public Garage(Config config)
        {
            for (int i = 1; i <= config.TotalParkingSpots; i++)
            {
                Spots.Add(new ParkingSpot
                {
                    SpotNumber = i,
                    ParkedVehicles = new List<Vehicle>()
                });
            }
        }

        //Kontrollera om platsen är ledig och parkerar fordonet
        public int ParkVehicle(Vehicle vehicle, Config config) //ändrar till int från bool då den returnerar ett värde
        {
            foreach (var spot in Spots)
            {
                if (spot.IsAvailable(vehicle, config))
                {
                    vehicle.CheckInTime = DateTime.Now;
                    spot.ParkedVehicles.Add(vehicle);
                    GarageData.Save(this);
                    return spot.SpotNumber;
                }

            }
            return -1; //ingen plats ledig

        }
        //Checka ut fordon
        public string CheckOut(string regNumber, Config config)
        {
            Vehicle foundVehicle = null;
            ParkingSpot foundSpot = null;

            foreach (var spot in Spots)
            {
                foreach (var vehicle in spot.ParkedVehicles)
                {
                    if (vehicle.RegNumber == regNumber)
                    {
                        foundVehicle = vehicle;
                        foundSpot = spot;
                        break;
                    }
                }
                if (foundVehicle != null) break;
            }

            if (foundVehicle == null)
                return "Kunde inte hitta fordon";

            foundSpot.ParkedVehicles.Remove(foundVehicle);

            TimeSpan duration = DateTime.Now - foundVehicle.CheckInTime;

            double ratePerHour = foundVehicle is Car ? config.CarRatePerHour : config.MCRatePerHour;
            double parkingFee = foundVehicle.CalculateCost(config.FreeMinutes, ratePerHour);

            GarageData.Save(this);

            string message = $"Fordon: {foundVehicle.Type} {foundVehicle.RegNumber} checkade ut.\n" +
                             $"Parkeringstid: {duration.Hours}h {duration.Minutes} min\n" +
                             $"Avgift: {parkingFee:F2} CZK";
            return message;
        }
    }

}