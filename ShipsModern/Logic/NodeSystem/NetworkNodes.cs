using ShipsForm.Logic.TilesSystem;
using ShipsModern.Logic.NodeSystem.OptimalNodeHost;
using System.Collections.Generic;
using System.Linq;

namespace ShipsForm.Logic.NodeSystem
{
    class NetworkNodes
    {
        private List<GeneralNode> m_nodesNetwork = new List<GeneralNode>();
        private NetworkHost m_host;
        public static NetworkNodes Network = new NetworkNodes();
        public NetworkHost Host { get { return m_host; } }
        public List<GeneralNode> Nodes { get { return m_nodesNetwork; } }
        private NetworkNodes() {  m_host = new NetworkHost(this); }


        public Node AddNode(SupportEntities.Point point, int nodeSize)
        {
            Node newNode = new Node(point, nodeSize);
            m_nodesNetwork.Add(newNode);
            return newNode;
        }

        public MarineNode AddMarine(SupportEntities.Point point)
        {
            MarineNode mn = new MarineNode(point);
            m_nodesNetwork.Add(mn);
            return mn;
        }
        public MarineNode AddMarine(Tile tile)
        {
            MarineNode mn = new MarineNode(tile);
            m_nodesNetwork.Add(mn);
            return mn;
        }
    }
}
