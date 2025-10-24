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
        public List<Vehicle> ParkedVehicles { get; set; } = new List<Vehicle>();

        public bool IsAvailable(Vehicle v)
        {
            if (v is Car) //om input är av typen BIL

            //kolla om det finns en bil på platsen
            {
                foreach (var vehicle in ParkedVehicles)
                {
                    if (vehicle is Car)
                    {
                        return false; //platsen upptagen
                    }
                    return true; // ledig för bild
                }
            }
            else if (v is MC)
            {
                int mcCount = 0; //räknare för att hålla koll på hur många MC som redan står parkerade på platsen

                foreach (var vehicle in ParkedVehicles)
                {
                    if (vehicle is Car)
                    {
                        return false; //platsen upptagen av en bil
                    }
                    if (vehicle is MC)
                    {
                        mcCount++; //ökar MC med 1 för att ta reda på hur många MC som står där
                    }
                    return true; // ledig för bild
                }
                if (mcCount < 2)
                    return true; //om det står mindra än 2 MC på platsen finns en ledig plats för MC
                else
                    return false;
            }
            return false;
            }
            

            }
        }
   
