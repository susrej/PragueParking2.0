using PragueParking2._0.DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking2._0.Models
{

    public class ParkingSpot
    {
        public int SpotNumber { get; set; }
        public int MaxSize { get; set; } = 2; // Max 2 MC per plats eller 1 bil
        public List<Vehicle> ParkedVehicles { get; set; } = new List<Vehicle>();

        // Kontrollera om platsen är ledig för fordonet
        public bool IsAvailable(Vehicle vehicle, Config config)
        {
            //Om parkeringsplatsen är ledig kan Bil eller MC parkeras
            if (ParkedVehicles.Count == 0)
                return true;

            // Om det redan finns en bil kan inget annat parkeras
            if (ParkedVehicles.Any(v => v is Car))
                return false;

            // Om vi försöker parkera en bil, tillåt endast om platsen är tom (redan hanterad ovan)
            if (vehicle is Car)
                return false;

            // Om vi försöker parkera en MC, tillåt upp till 2 MC
            if (vehicle is MC)
                return ParkedVehicles.Count < 2;

            return false;
        }
       
    }
}
