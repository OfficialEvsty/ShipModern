using System;
using ShipsForm.Exceptions;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Newtonsoft.Json;

namespace ShipsForm.Data
{
    public class Configuration
    {
        public float TileDistance;
        public HashSet<string> TileCategories;

        public int FieldWidth;
        public int FieldHeight;
        public int TimeTickMS;
        public int MultiplyTimer;
        public int CellScale;
        public int TileWidth;

        public bool IBWaitingMode;
        public int IBWaitsFraghtInSeconds;

        public float DistanceBetweenIcebreakerAndShips;
        public float DistanceBetweenShips;

        public float MaxTilesToPlaceMarineNode;
        

        //SVG filename's uri
        public Dictionary<string, string> SVG_Uris;

        //Image size fields
        public int IcebreakerImageSize;
        public int ShipImageSize;
        public int DefaultImageSize;
        public int ConvoyShipImageSize;
        public int NodeImageSize;
        public int MarineNodeImageSize;

        public float CaravanSpeed;

        public byte ShipIceResistLevel;
        public byte IBIceResistLevel;
        public Dictionary<byte, string[]> IceResistance;

        static public Configuration? Instance;
        private Configuration() { }

        static public void Init()
        {
            string filename = "Configuration.json";
            using (StreamReader reader = new StreamReader(Path.Combine(Directory.GetCurrentDirectory() + @"../../../../", filename)))
            {
                string json = reader.ReadToEnd();
                if (json is null)
                    throw new JsonFileEmptyError($"File settings is Empty. You should fill \\bin\\..\\..\\Configuration.json");

                Configuration? model = JsonConvert.DeserializeObject<Configuration>(json);
                Instance = model;
                Console.WriteLine($"Model's settings successfully read.");
            }
        }
    }
}
