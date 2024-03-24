using ShipsForm.Logic.ShipSystem.Ships;
using ShipsForm.Logic.NodeSystem;
using ShipsForm.Timers;
using ShipsForm.Graphic;
using ShipsForm.GUI;
using System.Collections.Generic;

namespace ShipsForm.Logic
{
    internal class Manager
    {
        private List<Ship> m_ships = new List<Ship>();
        private List<GeneralNode> m_nodes = new List<GeneralNode>();
        private static List<IDrawable> drawables = new List<IDrawable>();
        private Painter m_painter;

        public static Manager? Instance { get; private set; }
        public static List<IDrawable>? Drawings { get { return Manager.drawables; } }

        private Manager(Painter p)
        {
            m_painter = p;
            TimerData.PropertyChanged += ProccedShipsLogic;
        }

        public static Manager Init(Painter p)
        {
            if (Instance == null)
            {
                Instance = new Manager(p);
            }
            return Instance;
        }
        public List<Ship> Ships
        {
            get { return m_ships; }
        }

        public List<GeneralNode> Nodes
        {
            get { return m_nodes; }
        }

        public void AssignShip(Ship newShip)
        {
            m_ships.Add(newShip);
            drawables.Add(newShip);
        }

        public void AssignNode(GeneralNode newNode)
        {
            m_nodes.Add(newNode);
            drawables.Add(newNode);
        }

        public void ProccedShipsLogic()
        {
            foreach(Ship ship in Ships)
            {
                ship.Performer.Check();
                ship.Update();
            }
            m_painter.DrawFrame();
        }
    }
}
