using System.Collections.Generic;
using System.Linq;

namespace ShipsForm.Logic.NodeSystem
{
    class NetworkNodes
    {
        private List<GeneralNode> m_nodesNetwork = new List<GeneralNode>();
        public static NetworkNodes Network = new NetworkNodes();
        public List<GeneralNode> Nodes { get { return m_nodesNetwork; } }
        private NetworkNodes() { }



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

        public MarineNode GetNearMarineNode(GeneralNode togn, GeneralNode? fromgn = null)
        {
            return (fromgn is null) ? (MarineNode)Enumerable.First(m_nodesNetwork.OrderBy(x => x.GetCoords.GetDistance(togn.GetCoords)).ToList(), x => x is MarineNode) : (MarineNode)Enumerable.First(m_nodesNetwork.OrderBy(x => x.GetCoords.GetDistance(togn.GetCoords)).ToList(), x => x is MarineNode && x != fromgn);

        }
    }
}
