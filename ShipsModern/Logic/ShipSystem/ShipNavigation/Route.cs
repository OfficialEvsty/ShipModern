using ShipsForm.Logic.NodeSystem;
using ShipsForm.Logic.TilesSystem;
using System.Collections.Generic;
using System.Linq;

namespace ShipsModern.Logic.ShipSystem.ShipNavigation
{
    class Route
    {
        private List<Tile> m_tiles;
        private GeneralNode m_from;
        private GeneralNode m_to;
        private byte b_iceResistanceLevel;

        public List<Tile> Tiles { get { return m_tiles; } }

        public Route(GeneralNode from, GeneralNode to, List<string> map, byte iceResistanceLevel)
        {
            m_from = from;
            m_to = to;
            this.b_iceResistanceLevel = iceResistanceLevel;

            Tile fromNode = Tile.GetTileCoords(from.GetCoords);
            Tile toNode = Tile.GetTileCoords(to.GetCoords);

            if (fromNode == null || toNode == null)
                throw new System.Exception();
            if (map == null)
                throw new System.Exception();
            var newRoute = Field.BuildPath(map, fromNode, toNode, b_iceResistanceLevel);
            if (newRoute == null)
                throw new System.Exception();
            m_tiles = newRoute;
        }

        private Route(GeneralNode from, GeneralNode to, byte iceResistanceLevel)
        {
            m_from = from;
            m_to = to;
            b_iceResistanceLevel = iceResistanceLevel;
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
            var reversedRoute = new Route(routeToReverse.m_to, routeToReverse.m_from, routeToReverse.b_iceResistanceLevel);
            reversedRoute.m_tiles = reversedTiles;
            return reversedRoute;
        }
    }
}
