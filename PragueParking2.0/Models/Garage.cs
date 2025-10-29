using PragueParking2._0.DataAccess;
using PragueParking2._0.Models;
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
                    MaxSize = 2,
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

        public bool MoveVehicle(string regNumber, int targetSpotNumber, Config config)
        {
            Vehicle vehicleToMove = null;
            ParkingSpot currentSpot = null;

            foreach (var spot in Spots)
            {
                foreach (var vehicle in spot.ParkedVehicles)
                {
                    if (vehicle.RegNumber == regNumber)
                    {
                        vehicleToMove = vehicle;
                        currentSpot = spot;
                        break;
                    }
                }
                if (vehicleToMove != null)
                    break;
            }
            if (vehicleToMove == null)
            {
                Console.WriteLine($"Fordon med regnummer {regNumber} hittades inte.");
                return false;
            } // 2. Hitta målet

            ParkingSpot targetSpot = null;
            foreach (var spot in Spots)
            {
                if (spot.SpotNumber == targetSpotNumber)
                {
                    targetSpot = spot;
                    break;
                }
            }
            if (targetSpot == null)
            {
                Console.WriteLine($"Plats {targetSpotNumber} finns inte.");
                return false;
            }

            // 3. Kontrollera om platsen är tillgänglig
            if (!targetSpot.IsAvailable(vehicleToMove, config))
            {
                Console.WriteLine($"Plats {targetSpotNumber} är inte ledig för detta fordon.");
                return false;
            }

            // 4. Flytta fordonet
            currentSpot.ParkedVehicles.Remove(vehicleToMove);
            targetSpot.ParkedVehicles.Add(vehicleToMove);

            Console.WriteLine($"Fordon {regNumber} flyttat till plats {targetSpotNumber}.");
            GarageData.Save(this);
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