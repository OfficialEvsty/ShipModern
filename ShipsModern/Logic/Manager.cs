﻿using ShipsForm.Logic.ShipSystem.Ships;
using ShipsForm.Logic.NodeSystem;
using ShipsForm.Timers;
using ShipsForm.Graphic;
using ShipsForm.GUI;
using System.Collections.Generic;
using System.Linq;

namespace ShipsForm.Logic
{
    internal class Manager
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

        public void AssignShip(Ship newShip)
        {
            var ship_id = newShip.Id;
            var pathLineId = GetGuiElementID();
            m_ships.Add(newShip);
            drawables.Add(ship_id, newShip);
            pathDrawables.Add(pathLineId, newShip.PathObserver);
        }

        public void AssignNode(GeneralNode newNode)
        {
            m_nodes.Add(newNode);
            drawables.Add(newNode.Id, newNode);
        }

        public void ProccedShipsLogic()
        {
            var safeCopied = m_ships.ToList();
            foreach (Ship ship in safeCopied)
            {
                ship.Performer.Check();
                ship.Update();
            }
            m_painter.DrawFrame();
        }
    }
}
