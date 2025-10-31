using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PragueParking2._0.Core
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
                    MaxSize = 2,
                    ParkedVehicles = new List<Vehicle>()
                });
            }
        }

        //Kontrollera om platsen är ledig och parkerar fordonet
        public int ParkVehicle(Vehicle vehicle, Config config)
        {
            //kontrollera om regnummer redan finns
            foreach (var spot in Spots)
            {
                foreach (var parkedVehicle in spot.ParkedVehicles)
                {
                    if (parkedVehicle.RegNumber.ToUpper() == vehicle.RegNumber.ToUpper())
                    {
                        return -2;

                    }
                }
            }

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
        
        //Flytta fordon
        public bool MoveVehicle(string regNumber, int targetSpotNumber, Config config, out string message)
        {
            Vehicle vehicleToMove = null;
            ParkingSpot currentSpot = null;

            // 1. Hitta fordonet
            foreach (var spot in Spots)
            {
                vehicleToMove = spot.ParkedVehicles.FirstOrDefault(v => v.RegNumber == regNumber);
                if (vehicleToMove != null)
                {
                    currentSpot = spot;
                    break;
                }
            }

            if (vehicleToMove == null)
            {
                message = $"Fordon med regnummer {regNumber.ToUpper()} hittades inte.";
                return false;
            }

            // 2. Hitta målet
            var targetSpot = Spots.FirstOrDefault(s => s.SpotNumber == targetSpotNumber);
            if (targetSpot == null)
            {
                message = $"Plats {targetSpotNumber} finns inte.";
                return false;
            }

            // 3. Kontrollera om platsen är tillgänglig
            if (!targetSpot.IsAvailable(vehicleToMove, config))
            {
                message = $"Plats {targetSpotNumber} är inte ledig för detta fordon.";
                return false;
            }

            // 4. Flytta fordonet
            currentSpot.ParkedVehicles.Remove(vehicleToMove);
            targetSpot.ParkedVehicles.Add(vehicleToMove);

            GarageData.Save(this);

            message = $"Fordon {regNumber.ToUpper()} flyttad till plats {targetSpotNumber}.";
            return true;
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
                    if (vehicle.RegNumber.ToUpper() == regNumber.ToUpper())
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

            string message = $"{foundVehicle.Type}: \t\t[bold yellow]{foundVehicle.RegNumber.ToUpper()}[/] checkades ut.\n" +
                             $"Parkeringstid: \t[bold yellow]{duration.Hours}h {duration.Minutes}[/] min\n" +
                             $"Avgift: \t[bold yellow]{parkingFee:F2}[/] CZK\n";
            return message;
        }
    }

}