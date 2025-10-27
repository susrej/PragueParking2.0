using PragueParking2._0.DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking2._0.Models
{
    //    public class ParkingSpot
    //    {
    //        public int SpotNumber { get; set; }
    //        public List<Vehicle> ParkedVehicles { get; set; } = new List<Vehicle>();

    //        // Kolla om platsen är ledig för fordonet
    //        //public bool IsAvailable(Vehicle vehicle, Config config)
    //        //{
    //        //    if (ParkedVehicles == null)
    //        //        ParkedVehicles = new List<Vehicle>();

    //        //    if (vehicle is Car)
    //        //    {
    //        //        // Om det redan finns en bil, platsen upptagen
    //        //        foreach (var v in ParkedVehicles)
    //        //        {
    //        //            if (v is Car)
    //        //            {
    //        //                return false;
    //        //            }
    //        //        }
    //        //        return true;// plats ledig för bil
    //        //    }

    //        //    else if (vehicle is MC)
    //        //    {
    //        //        int mcCount = 0;
    //        //        foreach (var v in ParkedVehicles)
    //        //        {
    //        //            if (v is Car) return false; // MC kan ej stå med bil
    //        //            if (v is MC) mcCount++;
    //        //        }

    //        //        if (mcCount < 2)
    //        //        {
    //        //            return true; // plats ledig för MC
    //        //        }
    //        //        else
    //        //        {
    //        //            return false; // plats full för MC
    //        //        }
    //        //    }

    //        //    return false; // om det är någon annan typ av fordon
    //        //}

    //        public bool IsAvailable(Vehicle vehicle, Config config)
    //        {
    //            if (ParkedVehicles == null)
    //                ParkedVehicles = new List<Vehicle>(); // säkerställ att listan inte är null

    //            if (vehicle is Car)
    //            {
    //                // Om det finns någon bil på platsen är platsen upptagen
    //                for (int i = 0; i < ParkedVehicles.Count; i++)
    //                {
    //                    if (ParkedVehicles[i] is Car)
    //                        return false; // platsen upptagen
    //                }
    //                return true; // ledig för bil
    //            }
    //            else if (vehicle is MC)
    //            {
    //                int mcCount = 0;

    //                for (int i = 0; i < ParkedVehicles.Count; i++)
    //                {
    //                    if (ParkedVehicles[i] is Car)
    //                        return false; // MC kan inte stå med bil

    //                    if (ParkedVehicles[i] is MC)
    //                        mcCount++;
    //                }

    //                if (mcCount < 2)
    //                    return true; // max 2 MC per plats
    //                else
    //                    return false; // platsen full
    //            }

    //            return false; // okänd fordonstyp
    //        }
    //    }
    //}
    public class ParkingSpot
    {
        public int SpotNumber { get; set; }
        public List<Vehicle> ParkedVehicles { get; set; } = new List<Vehicle>();

        // Kontrollera om platsen är ledig för fordonet
        public bool IsAvailable(Vehicle vehicle, Config config)
        {
            if (vehicle is Car)
            {
                // Platsen är upptagen om det finns en bil
                foreach (var v in ParkedVehicles)
                {
                    if (v is Car)
                        return false;
                }
                return true; // ledig för bil
            }
            else if (vehicle is MC)
            {
                int mcCount = 0;
                foreach (var v in ParkedVehicles)
                {
                    if (v is Car)
                        return false; // MC kan ej stå med bil
                    if (v is MC)
                        mcCount++;
                }
                return mcCount < 2; // max 2 MC per plats
            }

            return false;
        }
    }
}