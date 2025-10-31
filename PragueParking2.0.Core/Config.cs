using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace PragueParking2._0.Core
{
    public class Config
    {
        //Textfil
        public static void SyncFromTextFile()
        {
            string textFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../config.txt");

            // Skapa filen om den inte finns
            if (!File.Exists(textFile))
            {
                using (var writer = new StreamWriter(textFile))
                {
                    writer.WriteLine("# Prague Parking konfiguration");
                    writer.WriteLine("# Ändra värdena nedan och spara filen");
                    writer.WriteLine("FreeMinutes = 10");
                    writer.WriteLine("CarRatePerHour = 20");
                    writer.WriteLine("MCRatePerHour = 10");
                    writer.WriteLine("TotalParkingSpots = 100");
                }
            }

            var config = Load(); // läs in nuvarande JSON

            // Läs textfilen och uppdatera värden
            foreach (var line in File.ReadAllLines(textFile))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;

                var parts = line.Split('=');
                if (parts.Length != 2) continue;

                string key = parts[0].Trim();
                string value = parts[1].Trim();

                switch (key)
                {
                    case "FreeMinutes":
                        config.FreeMinutes = int.Parse(value);
                        break;
                    case "CarRatePerHour":
                        config.CarRatePerHour = double.Parse(value);
                        break;
                    case "MCRatePerHour":
                        config.MCRatePerHour = double.Parse(value);
                        break;
                    case "TotalParkingSpots":
                        config.TotalParkingSpots = int.Parse(value);
                        break;
                }
            }

            // Spara tillbaka till JSON så det alltid är i synk
            config.Save();
        }
        public int FreeMinutes { get; set; } = 10;
        public double CarRatePerHour { get; set; } = 20;
        public double MCRatePerHour { get; set; } = 10;
        public int TotalParkingSpots { get; set; } = 100;

        // ändrar så att det alltid går att hitta filen oavsett varifrån den startas
        private static readonly string ConfigFile = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "../../../config.json"); 

        //Läser in konfiguration från fil, skapar en ny om fil saknas
        public static Config Load()
        {

            //kontrollerar om configfilen finns
            if (File.Exists(ConfigFile))
            {
                var json = File.ReadAllText(ConfigFile);
                return JsonSerializer.Deserialize<Config>(json) ?? new Config();
            }
            else //annars skapas en ny configfil
            {
                var config = new Config();
                config.Save();
                return config;
            }
        }

        //sparar konfigurationen till fil
        public void Save()
        {
            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigFile, json);
        }

    }
}
