using ShipsForm.Exceptions;
using ShipsForm.Logic.NodeSystem;
using ShipsForm.Logic.TilesSystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShipsModern.Logic.ShipSystem.ShipNavigation
{
    [Serializable]
    class Route
    {
        private List<Tile> m_tiles;
        private GeneralNode m_from;
        private GeneralNode m_to;
        private byte b_iceResistanceLevel;

        public GeneralNode From { get {  return m_from; } }
        public GeneralNode To { get { return m_to; } }

        public byte IceLevel { get { return b_iceResistanceLevel; } }

        public List<Tile> Tiles { get { return m_tiles; } }

        public Route(GeneralNode from, GeneralNode to, byte iceResistanceLevel)
        {
            m_from = from;
            m_to = to;
            this.b_iceResistanceLevel = iceResistanceLevel;

            Tile fromNode = Tile.GetTileCoords(from.GetCoords);
            Tile toNode = Tile.GetTileCoords(to.GetCoords);
            m_tiles = BuildRoute(fromNode, toNode, iceResistanceLevel);
        }

        private Route(GeneralNode from, GeneralNode to, byte iceResistanceLevel, int mode = 0)
        {
            m_from = from;
            m_to = to;
            b_iceResistanceLevel = iceResistanceLevel;
        }

        public Route(List<Tile> tilesList, GeneralNode from, GeneralNode to, byte iceResistanceLevel)
        {
            m_tiles = tilesList;
            m_from = from;
            m_to = to;
            b_iceResistanceLevel = iceResistanceLevel;
        }

        public static Route? GetRouteFromList(GeneralNode from, GeneralNode to, List<Route> availableRoutes)
        {
            Tile tileFrom = from.TileCoords;
            Tile tileTo = to.TileCoords;
            if (tileFrom == null || tileTo == null)
                return null;

            foreach (var route in availableRoutes)
            {

                if (route.Tiles.First().X == tileFrom.X && route.Tiles.First().Y == tileFrom.Y)
                {
                    if (route.Tiles.Last().X == tileTo.X && route.Tiles.Last().Y == tileTo.Y)
                        return route;
                }
            }
            return null;
        }

        private static List<Tile> BuildRoute(Tile from, Tile to, byte iceResistanceLevel)
        {
            if (from == null || to == null)
                throw new System.Exception();
            var newRoute = Field.BuildPath(from, to, iceResistanceLevel);
            if (newRoute == null)
                throw new System.Exception();
            return newRoute;
        }
        public bool IsIceZone()
        {
            var category = "ice";
            foreach (var tile in Tiles)
            {
                if (tile.Category == category)
                    return true;
            }
            return false;
        }
        private (Tile fst, Tile snd) GetIceBonds()
        {
            var category = "ice";
            int fst_index = 0, snd_index = Tiles.Count-1;
            (Tile fst, Tile snd) bonds;
            Tile? fst = null, snd = null;
            for (int i = fst_index; i < snd_index; i++)
            {
                if (Tiles[i].Category == category)
                {
                    var shifted = Math.Max(fst_index, i - 1);
                    fst = Tiles[shifted];
                    break;
                }
            }
            for (int j = snd_index; j > fst_index; j--)
            {
                if (Tiles[j].Category == category)
                {
                    var shifted = Math.Min(snd_index, j + 1);
                    snd = Tiles[shifted];
                    break;
                }
            }
            if (fst == null || snd == null)
                throw new System.Exception("There're not tiles with ice category.");
            bonds = (fst, snd);
            return bonds;
        }

        public static Route GetMainRoute(GeneralNode from, Node to, List<Route> allRoutes)
        {
            var data = ShipsForm.Data.Configuration.Instance;
            if (data is null)
                throw new ConfigFileDoesntExistError();

            Route? mainRoute = GetRouteFromList(from, to, allRoutes);
            if (mainRoute != null)
                return mainRoute;
            byte maxIceLevelResistance = data.IceResistance.Keys.Max();
            mainRoute = new Route(from, to, maxIceLevelResistance);
            return mainRoute;
        }

        public static (GeneralNode gn1, GeneralNode gn2) GetReachedNodes(Route mainRoute, byte iceResistanceLevel)
        {
            
            if (!mainRoute.IsIceZone())
            {
                mainRoute.b_iceResistanceLevel = iceResistanceLevel;
                return (mainRoute.m_from, mainRoute.m_to);
            }
            (Tile fst, Tile snd) MNTilesPair = mainRoute.GetIceBonds();
            var fst_mn = NetworkNodes.Network.Host.GetOrCreateMarineNode(MNTilesPair.fst);
            var snd_mn = NetworkNodes.Network.Host.GetOrCreateMarineNode(MNTilesPair.snd);            
            return (fst_mn, snd_mn);
        }

        public Route[] SplitMainRoute((GeneralNode mn1, GeneralNode mn2) splitNodes, byte iceResistance)
        {
            var data = ShipsForm.Data.Configuration.Instance;
            if (data is null)
                throw new ConfigFileDoesntExistError();
            byte maxIceLevelResistance = data.IceResistance.Keys.Max();

            var fstSplitTile = splitNodes.Item1.TileCoords;
            var sndSplitTile = splitNodes.Item2.TileCoords;
            Queue<(GeneralNode Node, Tile Tile)> splittedTiles = 
                new Queue<(GeneralNode, Tile)>
                (new (GeneralNode, Tile)[] { (m_from, Tiles[0]), 
                    (splitNodes.Item1, fstSplitTile), 
                    (splitNodes.Item2, sndSplitTile), 
                    (m_to, Tiles[Tiles.Count - 1]) });

            var start = splittedTiles.Dequeue();
            var end = splittedTiles.Dequeue();
            List<Tile> tilesRoute = new List<Tile>();
            List<Route> splittedRoute = new List<Route>();            
            for (int i = 0; i < Tiles.Count; i++)
            {
                tilesRoute.Add(Tiles[i]);
                if (Tiles[i].Equals(end.Tile)) 
                {
                    splittedRoute.Add(
                        new Route(tilesList: tilesRoute.ToList(), 
                        start.Node, end.Node, 
                        (start.Node==splitNodes.mn1 && end.Node == splitNodes.mn2) ? maxIceLevelResistance : iceResistance));
                    tilesRoute.Clear();
                    if(splittedTiles.Count > 0)
                    {
                        start = end;
                        tilesRoute.Add(start.Tile);
                        end = splittedTiles.Dequeue();
                    }
                }
            }
            return splittedRoute.ToArray();
        }
        /// <summary>
        /// Checks route existing with up-cost condition.
        /// </summary>
        /// <param name="cost"></param>
        /// <returns></returns>
        public static bool IsRouteValidUpperCost(Tile destiny, MarineNode mn, byte iceLevel, int cost)
        {
            var tiles = BuildRoute(destiny, mn.TileCoords, iceLevel);
            if (tiles == null)
                return false;
            if (tiles[tiles.Count - 1].Cost <= cost)
                return true;
            return false;
        }

        public static bool IsRouteValid(GeneralNode gnFrom, GeneralNode gnTo, byte iceLevel = 1)
        {
            var tiles = BuildRoute(gnFrom.TileCoords, gnTo.TileCoords, iceLevel);
            if (tiles == null)
                return false;
            return true;
        }

        public static bool IsRoutesEqual(Route roate1, Route roate2)
        {
            if (roate1.m_tiles.Count != roate2.m_tiles.Count)
                return false;
            for (int i = 0; i < roate1.m_tiles.Count; i++)
            {
                if (roate1.m_tiles[i].X != roate2.m_tiles[i].X && roate1.m_tiles[i].Y != roate2.m_tiles[i].Y)
                    break;
                if (i == roate1.m_tiles.Count - 1)
                    return true;
            }

            for (int i = roate1.m_tiles.Count - 1; i >= 0; i--)
            {
                if (roate1.m_tiles[i].X != roate2.m_tiles[i].X && roate1.m_tiles[i].Y != roate2.m_tiles[i].Y)
                    break;
                if (i == 1)
                    return true;
            }
            return false;
        }
        public static Route GetReversedRoute(Route routeToReverse)
        {
            var reversedTiles = routeToReverse.Tiles.ToList();
            reversedTiles.Reverse();
            var reversedRoute = new Route(routeToReverse.m_to, routeToReverse.m_from, routeToReverse.b_iceResistanceLevel, mode:0);
            reversedRoute.m_tiles = reversedTiles;
            return reversedRoute;
        }
    }
}
