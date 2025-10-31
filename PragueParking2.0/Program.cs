using System.Runtime.InteropServices.Marshalling;
using Spectre;
using Spectre.Console;
using PragueParking2._0.Core;

Config.SyncFromTextFile();
var config = Config.Load();
var garage = GarageData.Load(config);

UserInterface ui = new UserInterface(garage, config);
ui.Run();