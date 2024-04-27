using ShipsForm.Exceptions;
using ShipsForm.Logic.NodeSystem;
using ShipsForm.Logic.TilesSystem;
using ShipsForm.SupportEntities;
using ShipsModern.Logic.ShipSystem.ShipNavigation;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Windows.Documents;

namespace ShipsModern.Logic.NodeSystem.OptimalNodeHost
{
    /// <summary>
    /// This class supports nodes to be place on optimal routes on map.
    /// </summary>
    class NetworkHost
    {
        private NetworkNodes m_network;

        public NetworkHost(NetworkNodes nn) { m_network = nn; }
        
        
        /// <summary>
        /// Find near MarineNode or create new MarineNode on isshue's route with ice zones.
        /// </summary>
        /// <returns></returns>
        public MarineNode GetOrCreateMarineNode(Tile supposedMNTile)
        {
            var data = ShipsForm.Data.Configuration.Instance;
            if (data is null)
                throw new ConfigFileDoesntExistError();
            Point supposedMNPoint = new Point(supposedMNTile.X * data.TileDistance, supposedMNTile.Y * data.TileDistance);
            var maxDist = data.MaxTilesToPlaceMarineNode * data.TileDistance;
            var nearMarineNodes = GetNearMarineNodes(supposedMNPoint, maxDist);
            var existedMN = GetCheckedRouteToMarineNodes(nearMarineNodes, supposedMNTile, (int)(maxDist / data.TileDistance)).FirstOrDefault();
            if (existedMN != null)
                return existedMN;
            return m_network.AddMarine(supposedMNTile);
        }

        private MarineNode[] GetNearMarineNodes(Point destiny, float maxDistance)
        {
            var config = ShipsForm.Data.Configuration.Instance;
            if (config is null)
                throw new ConfigFileDoesntExistError();
            MarineNode[] mnodes = m_network.Nodes.Where(x => Math.GetDistance(new Point(x.GetCoords.X * config.TileDistance, x.GetCoords.Y * config.TileDistance), destiny) <= maxDistance && x is MarineNode).OrderBy(node => node.GetRelatedCoords.GetDistance(destiny)).OfType<MarineNode>().ToArray();
            return mnodes;
        }
        private MarineNode[] GetCheckedRouteToMarineNodes(MarineNode[] mnodes, Tile destinyTile, int maxCost)
        {
            byte iceResistanceLevel = 1;
            return mnodes.Where(node => Route.IsRouteValid(destinyTile, node, iceResistanceLevel, maxCost)).ToArray();
        }
                
        public MarineNode GetNearMarineNode(GeneralNode togn, GeneralNode? fromgn = null)
        {
            return (fromgn is null) ? (MarineNode)Enumerable.First(m_network.Nodes.OrderBy(x => x.GetCoords.GetDistance(togn.GetCoords)).ToList(), x => x is MarineNode) : (MarineNode)Enumerable.First(m_network.Nodes.OrderBy(x => x.GetCoords.GetDistance(togn.GetCoords)).ToList(), x => x is MarineNode && x != fromgn);
        }
    }
}
