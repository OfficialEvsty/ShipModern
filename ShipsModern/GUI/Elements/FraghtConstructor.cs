using ShipsForm.Logic.NodeSystem;
using ShipsForm.Logic.FraghtSystem;
using ShipsForm.Logic.CargoSystem;
using ShipsForm.Exceptions;
using System.Collections.Generic;

namespace ShipsForm.GUI.Elements
{
    /// <summary>
    /// This class allow user to create his Fraght and add it in general fraght market.
    /// </summary>
    class FraghtConstructor
    {
        /*private List<GeneralNode> m_nodes;
        static private ListBox? m_nodesFrom = null;
        static private ListBox? m_nodesTo = null;
        public FraghtConstructor(List<GeneralNode> nodes)
        {
            m_nodes = nodes;
            List<int> node_ids = new List<int>();
            if (m_nodesFrom != null && m_nodesTo != null)
            {
                foreach (GeneralNode gnode in m_nodes)
                {
                    if (gnode is Node node)
                        node_ids.Add(node.Id);
                }
                node_ids.ForEach(x => m_nodesFrom.Items.Add(x));
                node_ids.ForEach(x => m_nodesTo.Items.Add(x));
            }
        }

        public Node SearchNodeById(int id)
        {
            foreach (Node node in m_nodes)
            {
                if (node.Id == id)
                    return node;
            }
            string err_mess = $"Node ID: {id} not found.";
            throw new NodeNotFoundError(err_mess);
        }

        private void IsNodesValid()
        {
            if (m_nodesFrom is null || m_nodesTo is null)
                throw new Exception("List Boxes are not exist.");
            if (m_nodesFrom.SelectedIndex == -1 || m_nodesTo.SelectedIndex == -1)
                throw new Exception("List Boxes have not selected value.");
        }

        public void CreateFraght()
        {
            IsNodesValid();
            Node n1 = SearchNodeById(Convert.ToInt16(m_nodesFrom.Text));
            Node n2 = SearchNodeById(Convert.ToInt16(m_nodesTo.Text));

            int quantity = 5;
            Dictionary<Cargo, int> reqCargo = CargoConstructor.AssignNewCargo(quantity);

            Fraght fraght = new CargoFraght(reqCargo, n1, n2);
            FraghtMarket.AddFraght(fraght);
            Console.WriteLine("Fraght was added in FraghtMarket.");
        }

        public static void InitializeLBoxes(ListBox lfrom, ListBox lto)
        {
            m_nodesFrom = lfrom;
            m_nodesTo = lto;
        }*/
    }
}
