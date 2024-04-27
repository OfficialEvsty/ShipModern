using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Media;
using Newtonsoft.Json;
using ShipsForm.Data;
using ShipsForm.Exceptions;
using ShipsModern.SupportEntities.CustomDataStructures;

namespace ShipsForm.Logic.TilesSystem
{
    public class TileType
    {
        public int Id;
        public string? Name;
        public char? Symbol;
        public int Cost;
        public bool Passable;
        public byte[] RGB;
        public Color Color;
        public byte[] PixelRGB;
        public Color PixelColor;
        public string Category;
        public string? Description;
    }

    public class Field
    {
        private static List<TileType>? m_types;
        public List<TileType>? Types { get { return m_types; } }
        public List<string> Map { get; private set; }
        public int MapLength { get { return this.Map[0].Length; } }
        public int MapWidth { get { return this.Map.Count; } }
        public static Field? Instance;
        private Field()
        {
            LoadTilesTypeInfo();
            var data = Data.Configuration.Instance;
            if (data is null) throw new ConfigFileDoesntExistError();
            int lengthInTiles = (int)(Configuration.Instance.FieldHeight / data.TileDistance);
            int widthInTiles = (int)(Configuration.Instance.FieldWidth / data.TileDistance);
            Map = new List<string>();
            StringBuilder builder = new StringBuilder();
            Random rnd = new Random();
            for (int i = 0; i < lengthInTiles; i++)
            {
                for (int j = 0; j < widthInTiles; j++)
                    if (rnd.Next(0, 10) == 0)
                        builder.Append('e');
                    else
                        builder.Append(' ');
                Map.Add(builder.ToString());
                builder.Clear();
            }
            
            Console.WriteLine($"Map created. Length: {lengthInTiles}, Width: {widthInTiles}, Square: {lengthInTiles * widthInTiles}.");
        }

        private Field(bool b)
        {
            LoadTilesTypeInfo();
            if (Types is null)
                throw new Exception();
            Map = MapConstructor.GenMap(Types);
        }

        private void LoadTilesTypeInfo()
        {
            string filename = Path.Combine(Directory.GetCurrentDirectory(), "../../../Logic/TilesSystem/TileTypes.json"); ;
            using (StreamReader reader = new StreamReader(filename))
            {
                string json = reader.ReadToEnd();
                m_types = JsonConvert.DeserializeObject<List<TileType>>(json);
                if (m_types is null)
                    throw new Exception("TileType.json file doesn't exist in context.");
                foreach (TileType t in m_types)
                {
                    t.Color = Color.FromRgb(t.RGB[0], t.RGB[1], t.RGB[2]);
                    t.PixelColor = Color.FromRgb(t.PixelRGB[0], t.PixelRGB[1], t.PixelRGB[2]);
                }
            }
        }
        public static TileType GetTileTypeByName(string tileName)
        {
            foreach(var type in m_types)
                if (type.Name == tileName)
                    return type;
            throw new Exception("There is no tile type with this name in scope.");
        }
        public static Tile? GetTile(List<string> map, int i, int j, Tile? parent = null)
        {
            if (m_types is null)
                throw new Exception();
            bool isInsideMap = i >= 0 && j >= 0 && i < map.Count && j < map[0].Length;
            if (!isInsideMap)
                return null;
            foreach (TileType type in m_types)
            {
                if (type.Symbol == map[i][j])
                {
                    if (parent is not null)
                    {
                        float dist = MathF.Sqrt(MathF.Pow(parent.X - i, 2) + MathF.Pow(parent.Y - j, 2));
                        int additionCost = (int)(type.Cost * (dist - 1));
                        Tile newTile = new Tile() { TileCost = type.Cost + additionCost, X = i, Y = j, Passable = type.Passable, Parent = parent, Category=type.Category };
                        return newTile;
                    }
                    else
                    {
                        Tile newTile = new Tile() { TileCost = type.Cost, X = i, Y = j, Passable = type.Passable, Category = type.Category };
                        return newTile;
                    }
                }
            }
            throw new Exception();
        }
        public int GetTileCost(int x, int y)
        {
            bool isInsideMap = x >= 0 && y >= 0 && x <= Map.Count && y <= Map[0].Length;
            if (m_types is null || Map is null || !isInsideMap)
                throw new Exception();
            foreach (TileType type in m_types)
                if (type.Symbol == Map[x][y])
                    return type.Cost;
            throw new Exception();
        }
        public int GetTileId(int x, int y)
        {
            bool isInsideMap = x >= 0 && y >= 0 && x < Map.Count && y < Map[0].Length;
            if (m_types is null || Map is null || !isInsideMap)
                throw new Exception();
            foreach (TileType type in m_types)
                if (type.Symbol == Map[x][y])
                    return type.Id;
            throw new Exception();
        }
        public static void CreateField(bool b)
        {
            if (Instance != null)
                return;
            if (b == false)
                Instance = new Field();
            else
                Instance = new Field(b);
        }
        public static List<string>? PaintNavigationMap()
        {
            if (Instance == null)
                return null;
            List<string> map = Instance.Map;

            return map;
        }
        public static List<Tile> GetWalkableTiles(List<string> map, Tile currentTile, Tile targetTile, byte iceResistLevel = 1)
        {
            var possibleTiles = new List<Tile>()
            {
                GetTile(map, currentTile.X, currentTile.Y + 1, currentTile),
                GetTile(map, currentTile.X + 1, currentTile.Y, currentTile),
                GetTile(map, currentTile.X, currentTile.Y - 1, currentTile),
                GetTile(map, currentTile.X - 1, currentTile.Y, currentTile),
                GetTile(map, currentTile.X + 1, currentTile.Y + 1, currentTile),
                GetTile(map, currentTile.X + 1, currentTile.Y - 1, currentTile),
                GetTile(map, currentTile.X - 1, currentTile.Y - 1, currentTile),
                GetTile(map, currentTile.X - 1, currentTile.Y + 1, currentTile),
            };
            possibleTiles = possibleTiles.Where(tile => tile != null).ToList();
            possibleTiles.ForEach(tile => tile.Heuristic(targetTile.X, targetTile.Y));
            possibleTiles.ForEach(tile => tile.SetCost(currentTile.Cost));
            return possibleTiles
                .Where(tile => tile.X >= 0 && tile.X < map.Count)
                .Where(tile => tile.Y >= 0 && tile.Y < map.First().Length)
                .Where(tile => tile.Passable || Configuration.Instance.IceResistance[iceResistLevel].Any(x => GetTileTypeByName(x).Symbol == map[tile.X][tile.Y])).ToList();
        }

        /// <summary>
        /// Builds path of tiles from directed tiles.
        /// </summary>
        /// <param name="currentTile"></param>
        /// <param name="targetTile"></param>
        /// <param name="iceResistLevel"></param>
        /// <returns>List of tiles, if path doesn't exist returns null</returns>
        /*public static List<Tile>? BuildPath(Tile currentTile, Tile targetTile, byte iceResistLevel = 1)
        {
            Stopwatch[] timers = new Stopwatch[5] { new Stopwatch(), new Stopwatch(), new Stopwatch(), new Stopwatch(), new Stopwatch() };
            timers[0].Start();
            Dictionary<(int, int), List<Tile>> visitedTiles = new Dictionary<(int, int), List<Tile>>();
            void AddVisitedTile(Tile tile)
            {
                var coordsKey = (tile.X, tile.Y);
                if (visitedTiles.ContainsKey(coordsKey))
                    visitedTiles[coordsKey].Add(tile);
                else
                    visitedTiles[coordsKey] = new List<Tile>() { tile };
            }
            List<Tile> GetTilesByCoordsKey((int x, int y) coordsKey)
            {
                if (visitedTiles.ContainsKey(coordsKey))
                    return visitedTiles[coordsKey];
                return new List<Tile>();
            }

            Dictionary<(int, int), Tile> activeTiles = new Dictionary<(int, int), Tile>();
            LinkedList<Tile> activeSortedTiles = new LinkedList<Tile>();

            void AddActiveTile(Tile tile)
            {
                void AddNode(Tile tile)
                {
                    LinkedListNode<Tile>? node = activeSortedTiles.First;
                    if (node == null)
                    {
                        activeSortedTiles.AddFirst(tile);
                        return;
                    }
                    timers[1].Start();
                    while (node is not null)
                    {
                        //Блять тут <= просто решает, я не понимаю почему...
                        if (node.Value.CostDistance <= tile.CostDistance)
                            node = node.Next;
                        else
                        {
                            var newNode = new LinkedListNode<Tile>(tile);
                            activeSortedTiles.AddBefore(node, newNode);
                            return;
                        }
                    }
                    timers[1].Stop();
                    activeSortedTiles.AddLast(tile);
                }
                var coordsKey = (tile.X, tile.Y);
                activeTiles.Add(coordsKey, tile);
                AddNode(tile);
                //if (activeTiles.ContainsKey(coordsKey))

            }
            void RemoveActiveTile(Tile tile)
            {
                var coordsKey = (tile.X, tile.Y);
                if (activeTiles.ContainsKey(coordsKey))
                {
                    activeTiles.Remove(coordsKey);
                    activeSortedTiles.Remove(tile);
                }
            }

            currentTile.SetCost(0);
            currentTile.Heuristic(targetTile.X, targetTile.Y);
            AddActiveTile(currentTile);

            while (activeTiles.Any())
            {
                Tile checkTile = activeSortedTiles.First.Value;
                if (checkTile.X == targetTile.X && checkTile.Y == targetTile.Y)
                {
                    timers[0].Stop();
                    Console.WriteLine($"Timer [1]:{timers[0].ElapsedMilliseconds}ms, Timer [2]:{timers[1].ElapsedMilliseconds}ms, Timer [3]:{timers[2].ElapsedMilliseconds}ms, Timer(Add ActiveTiles) [3]:{timers[3].ElapsedMilliseconds}ms, Timer(Remove ActiveTiles) [4]:{timers[4].ElapsedMilliseconds}ms");
                    var returnedListTiles = new List<Tile>();
                    while (checkTile != null)
                    {
                        returnedListTiles.Add(checkTile);
                        checkTile = checkTile.Parent;
                    }
                    returnedListTiles.Reverse();
                    return returnedListTiles;
                }

                AddVisitedTile(checkTile);
                RemoveActiveTile(checkTile);
                var walkableTiles = GetWalkableTiles(Instance.Map, checkTile, targetTile, iceResistLevel);
                foreach (var walkableTile in walkableTiles)
                {
                    var tilesSection = GetTilesByCoordsKey((walkableTile.X, walkableTile.Y));
                    var isBetterTile = tilesSection.Any(tile => tile.X == walkableTile.X && tile.Y == walkableTile.Y && tile.Cost <= walkableTile.Cost);
                    if (isBetterTile)
                        continue;
                    var isActiveTile = activeTiles.ContainsKey((walkableTile.X, walkableTile.Y));
                    if (!isActiveTile)
                        AddActiveTile(walkableTile);
                }
            }
            Console.WriteLine($"Path from {currentTile.X} : {currentTile.Y} to {targetTile.X} : {targetTile.Y} not found.");
            return null;
        }*/
        public static List<Tile>? BuildPath(Tile currentTile, Tile targetTile, byte iceResistLevel = 1)
        {
            Stopwatch[] timers = new Stopwatch[5] { new Stopwatch(), new Stopwatch(), new Stopwatch(), new Stopwatch(), new Stopwatch() };
            timers[0].Start();
            Dictionary<(int, int), List<Tile>> visitedTiles = new Dictionary<(int, int), List<Tile>>();

            void AddVisitedTile(Tile tile)
            {
                var coordsKey = (tile.X, tile.Y);
                if (visitedTiles.ContainsKey(coordsKey))
                    visitedTiles[coordsKey].Add(tile);
                else
                    visitedTiles[coordsKey] = new List<Tile>() { tile };
            }
            List<Tile> GetTilesByCoordsKey((int x, int y) coordsKey)
            {
                if (visitedTiles.ContainsKey(coordsKey))
                    return visitedTiles[coordsKey];
                return new List<Tile>();
            }

            LinkedHashMap<Tile, Tile> openSet = new LinkedHashMap<Tile, Tile>();

            currentTile.SetCost(0);
            currentTile.Heuristic(targetTile.X, targetTile.Y);
            openSet.Add(currentTile, currentTile);

            while (openSet.Count > 0)
            {
                Tile checkTile = openSet.DequeueMin();
                if (checkTile.X == targetTile.X && checkTile.Y == targetTile.Y)
                {
                    timers[0].Stop();
                    Console.WriteLine($"Timer [1]:{timers[0].ElapsedMilliseconds}ms, Timer [2]:{timers[1].ElapsedMilliseconds}ms, Timer [3]:{timers[2].ElapsedMilliseconds}ms, Timer(Add ActiveTiles) [3]:{timers[3].ElapsedMilliseconds}ms, Timer(Remove ActiveTiles) [4]:{timers[4].ElapsedMilliseconds}ms");
                    var reconstructedTilesList = new List<Tile>();
                    while (checkTile != null)
                    {
                        reconstructedTilesList.Add(checkTile);
                        checkTile = checkTile.Parent;
                    }
                    reconstructedTilesList.Reverse();
                    return reconstructedTilesList;
                }

                AddVisitedTile(checkTile);
                var walkableTiles = GetWalkableTiles(Instance.Map, checkTile, targetTile, iceResistLevel);
                foreach (var walkableTile in walkableTiles)
                {
                    var tilesSection = GetTilesByCoordsKey((walkableTile.X, walkableTile.Y));
                    var isBetterTile = tilesSection.Any(tile => tile.X == walkableTile.X && tile.Y == walkableTile.Y && tile.Cost <= walkableTile.Cost);
                    if (isBetterTile)
                        continue;

                    openSet.Add(walkableTile, walkableTile);
                }
            }
            Console.WriteLine($"Path from {currentTile.X} : {currentTile.Y} to {targetTile.X} : {targetTile.Y} not found.");
            return null;
        }

        public static bool IsTilesEqual(Tile t1, Tile t2)
        {
            if (t1 == null || t2 == null)
                return false;
            return t1.X == t2.X && t1.Y == t2.Y;
        }
    }
}
