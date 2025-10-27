using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PragueParking2._0.Models;

namespace PragueParking2._0.DataAccess
{
    public class GarageData

    {
        private const string GarageFile = "garageData.json";

        public static Garage Load(Config config)
        {
            //kontrollera om garagefilen finns
            if (File.Exists(GarageFile))
            {
                var json = File.ReadAllText(GarageFile);
                var garage = JsonSerializer.Deserialize<Garage>(json);

                //om filen finns men Spots är null eller tom
                if (garage == null || garage.Spots == null || garage.Spots.Count == 0)
                {
                    garage = new Garage(config);
                    Save(garage);
                }
                return garage;
            }
            else //annars skapas en ny garagefil
            {
                var garage = new Garage();
                Save(garage);
                return garage;
            }
        }

        //sparar garagedata till fil
        public static void Save(Garage garage)
        {
            var json = JsonSerializer.Serialize(garage, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(GarageFile, json);
        }
    }
}
