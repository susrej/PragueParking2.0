using PragueParking2._0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace PragueParking2._0.DataAccess
{
    public class Config
    {
        public int FreeMinutes { get; set; } = 10;
        public double CarRatePerHour { get; set; } = 20;
        public double MCRatePerHour { get; set; } = 10;

        public int TotalParkingSpots { get; set; } = 100;

        private const string ConfigFile = "config.json";

        public Config() { }

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
