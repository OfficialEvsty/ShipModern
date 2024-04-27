using ShipsForm.Logic.ShipSystem.Ships;
using ShipsForm.Logic.FraghtSystem;
using ShipsForm.Logic.CargoSystem;
using ShipsForm.Logic.NodeSystem;
using ShipsForm.Timers;
using ShipsForm.Logic;
using ShipsForm.GUI;
using System.Collections.Generic;

namespace ShipsForm.Launching
{
    class Launcher
    {
        private Manager m_manager;
        public Manager Manager {  get { return m_manager; } }

        public List<GeneralNode> Nodes 
        { 
            get { return m_manager.Nodes; } 
        }

        public List<Ship> Ships
        {
            get { return m_manager.Ships; }
        }

        public Launcher(Painter painter)
        {
            m_manager = Manager.Init(painter);

            Node node1 = NetworkNodes.Network.AddNode(new SupportEntities.Point(0.7f, 0.7f), 5);
            Node node2 = NetworkNodes.Network.AddNode(new SupportEntities.Point(0.2f, 0.1f), 8);
            //Node node3 = NetworkNodes.Network.AddNode(new SupportEntities.Point(0.2f, 0.1f), 2);
            //MarineNode marineNode1 = NetworkNodes.Network.AddMarine(new SupportEntities.Point(0.1f, 0.1f));
            //MarineNode marineNode2 = NetworkNodes.Network.AddMarine(new SupportEntities.Point(0.1f, 0.12f));
            Dictionary<Cargo, int> requiredCargo = new Dictionary<Cargo, int>();
            requiredCargo.Add(new CargoContainer(node2), 5);
            Dictionary<Cargo, int> requiredCargo1 = new Dictionary<Cargo, int>();
            requiredCargo1.Add(new CargoContainer(node1), 5);
            Dictionary<Cargo, int> requiredCargo2 = new Dictionary<Cargo, int>();
            //requiredCargo2.Add(new CargoContainer(node3), 8);
            //Ship thirdShip = new CargoShip(node3);
            Ship myGreatShip = new CargoShip(node2);
            Ship MYSHIP = new CargoShip(node1);

            //Ship icebreaker = new IceBreaker(marineNode1);

            //m_manager.AssignShip(thirdShip);
            //m_manager.AssignNode(node3);
            //m_manager.AssignNode(marineNode1);
            //m_manager.AssignNode(marineNode2);

            //m_manager.AssignShip(icebreaker);
            CargoFraght fraght = new CargoFraght(requiredCargo, node2, node1);
            CargoFraght fraght2 = new CargoFraght(requiredCargo1, node1, node2);
            //CargoFraght fraght3 = new CargoFraght(requiredCargo2, node3, node2);
        }
    }
}
