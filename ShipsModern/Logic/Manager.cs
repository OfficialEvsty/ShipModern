using ShipsForm.Logic.ShipSystem.Ships;
using ShipsForm.Logic.NodeSystem;
using ShipsForm.Timers;
using ShipsForm.Graphic;
using ShipsForm.GUI;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using ShipsForm.SupportEntities.PatternObserver.Observers;
using ShipsForm.SupportEntities.PatternObserver;
using System;

namespace ShipsForm.Logic
{
    class Manager : IEventObserver<IDrawable>
    {
        private static int i_guiCounter = 0;
        private List<Ship> m_ships = new List<Ship>();
        private List<GeneralNode> m_nodes = new List<GeneralNode>();
        private static Dictionary<int, IDrawable> drawables = new Dictionary<int, IDrawable>();
        private static Dictionary<int, IPathDrawable> pathDrawables = new Dictionary<int, IPathDrawable>();
        private Painter m_painter;

        public static Manager? Instance { get; private set; }
        public static Dictionary<int, IDrawable>? Drawings { get { return Manager.drawables; } }
        public static Dictionary<int, IPathDrawable>? Paths { get { return Manager.pathDrawables; } }

        private Manager(Painter p)
        {
            m_painter = p;
            TimerData.PropertyChanged += ProccedShipsLogic;
            EventObservable.AddEventObserver(this);
        }

        public static Manager Init(Painter p)
        {
            if (Instance == null)
            {
                Instance = new Manager(p);
            }
            return Instance;
        }
        
        /// <summary>
        /// Returns unic identifier (ID) for GUI element.
        /// </summary>
        /// <returns>Identifier</returns>
        public static int GetGuiElementID()
        {
            return ++i_guiCounter;
        }

        public List<Ship> Ships
        {
            get { return m_ships; }
        }

        public List<GeneralNode> Nodes
        {
            get { return m_nodes; }
        }

        private void AssignShip(Ship newShip)
        {
            var ship_id = newShip.Id;
            var pathLineId = GetGuiElementID();
            m_ships.Add(newShip);
            drawables.Add(ship_id, newShip);
            pathDrawables.Add(pathLineId, newShip.PathObserver);
        }

        private void AssignNode(GeneralNode newNode)
        {
            m_nodes.Add(newNode);
            drawables.Add(newNode.Id, newNode);
            MarineNode[] marineNodes = m_nodes.Where(x => x is MarineNode).OfType<MarineNode>().ToArray();
            if (marineNodes.Length > 0 && marineNodes.Length % 2 == 0)
            {
                var mn = marineNodes[new Random().Next(0, marineNodes.Length - 1)];
                IceBreaker newIb = new IceBreaker(mn);
            }                
        }

        public void ProccedShipsLogic(object sender, PropertyChangedEventArgs args)
        {
            var safeCopied = m_ships.ToList();
            foreach (Ship ship in safeCopied)
            {
                ship.Performer.Check();
                ship.Update();
            }
            m_painter.DrawFrame();
        }

        public void Update(IDrawable ev)
        {
            if (ev is Ship)
                AssignShip((Ship)ev);
            else if (ev is GeneralNode)
                AssignNode((GeneralNode)ev);
        }
    }
}
