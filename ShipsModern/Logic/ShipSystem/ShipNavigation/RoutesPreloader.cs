

using Newtonsoft.Json;
using ShipsForm.Exceptions;
using ShipsForm.Logic.NodeSystem;
using ShipsForm.Logic.ShipSystem.ShipNavigation;
using ShipsForm.Logic.TilesSystem;
using ShipsForm.SupportEntities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Documents;
using static ShipsModern.Logic.ShipSystem.ShipNavigation.RoutesPreloader.OpenRoutes;

namespace ShipsModern.Logic.ShipSystem.ShipNavigation
{    

    static class RoutesPreloader
    {
        [Serializable]
        public class OpenRoutes
        {
            [Serializable]
            public class SerializableRoute
            {
                public List<Tile> Tiles;
                public byte IceResistance;

                public SerializableRoute(List<Tile> tiles, byte iceResistance)
                {
                    Tiles = GetTiles(tiles);
                    IceResistance = iceResistance;
                }

                private List<Tile> GetTiles(List<Tile> routeTiles)
                {
                    List<Tile> tiles = new List<Tile>();
                    foreach(var tile in routeTiles)
                    {
                        var nonParentTile = tile.GetSerializableTile();
                        tiles.Add(nonParentTile);
                    }
                    return tiles;
                }

                public Route? Restore()
                {
                    
                    GeneralNode? from = NetworkNodes.Network.GetGeneralNodeByTile(new Point(Tiles[0].X, Tiles[0].Y));
                    GeneralNode? to = NetworkNodes.Network.GetGeneralNodeByTile(new Point(Tiles[Tiles.Count-1].X, Tiles[Tiles.Count - 1].Y));
                    if (from == null || to == null)
                        return null;
                    return new Route(Tiles, from, to, IceResistance);
                }
                public static SerializableRoute Compress(Route routeToCompress)
                {
                    if (routeToCompress is null)
                        throw new Exception("Route nullreference");
                    return new SerializableRoute(routeToCompress.Tiles, routeToCompress.IceLevel);
                }
            }
            public string LastModifiedTime;
            public SerializableRoute[] SerializableRoutes;

            public OpenRoutes(string time, SerializableRoute[] sroutes) { LastModifiedTime = time; SerializableRoutes = sroutes;}

            public void AddOpenRoutesToNavigation()
            {
                var restoredRoutes = new List<Route>();
                var restoredSRoutes = new List<SerializableRoute>();
                foreach (var route in SerializableRoutes)
                {
                    var restored = route.Restore();
                    if (restored != null)
                    {
                        restoredSRoutes.Add(route);
                        restoredRoutes.Add(restored);
                    }                        
                }
                Navigation.AddOpenRoutes(restoredRoutes.ToArray());
                Save(restoredSRoutes.ToArray());
            }

            public List<SerializableRoute>? GetSerializableRoutes() { if (SerializableRoutes is null) return null; else return SerializableRoutes.ToList(); }

            public void Clear()
            {
                SerializableRoutes = new SerializableRoute[0];
            }
        }

        private static string s_openRoutesFileName = Directory.GetCurrentDirectory() + "..\\..\\..\\..\\..\\Maps\\openRoutes.json";
        public static void Load(DateTime dateTimeFileChanged)
        {
            using (StreamReader sr = new StreamReader(s_openRoutesFileName))
            {
                string json = sr.ReadToEnd();
                sr.Close();
                if (json is null)
                    throw new JsonFileEmptyError($"File settings is Empty. You should fill \\bin\\..\\..\\openRoutes.json");

                OpenRoutes? model = JsonConvert.DeserializeObject<OpenRoutes>(json);
                if (model is null)
                    throw new Exception("openRoutes.json is empty.");
                if (DateTime.Parse(model.LastModifiedTime) <= dateTimeFileChanged.AddSeconds(10))
                    model.AddOpenRoutesToNavigation();
            }
        }
        
        public static void Save(Route newRoute)
        {
            using (StreamReader sr = new StreamReader(s_openRoutesFileName))
            {
                string json = sr.ReadToEnd();
                if (json is null)
                    throw new JsonFileEmptyError($"File settings is Empty. You should fill \\Map\\..\\..\\openRoutes.json");
                OpenRoutes? model = JsonConvert.DeserializeObject<OpenRoutes>(json);
                sr.Close();
                if (model is null)
                    throw new Exception("openRoutes.json is empty.");

                List<SerializableRoute> routes = model.GetSerializableRoutes();
                if (routes is null)
                    routes = new List<SerializableRoute>();
                routes.Add(SerializableRoute.Compress(newRoute));

                using (StreamWriter sw = new StreamWriter(s_openRoutesFileName))
                {
                    OpenRoutes or = new OpenRoutes(DateTime.Now.ToString(), routes.ToArray());
                    string writeableJson = JsonConvert.SerializeObject(or, Formatting.Indented);
                    sw.Write(writeableJson);
                }
            }
            
        }
        public static void Save(SerializableRoute[] sroutes)
        {            
            using (StreamWriter sw = new StreamWriter(s_openRoutesFileName))
            {
                OpenRoutes or = new OpenRoutes(DateTime.Now.ToString(), sroutes);
                string writeableJson = JsonConvert.SerializeObject(or, Formatting.Indented);
                sw.Write(writeableJson);
            }
            
        }
    }
}
