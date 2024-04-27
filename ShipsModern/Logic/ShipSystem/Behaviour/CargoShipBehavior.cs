

using ShipsForm.Logic.FraghtSystem;
using ShipsForm.Logic.NodeSystem;
using ShipsForm.Logic.ShipSystem.Behaviour.ShipStates;
using ShipsForm.Logic.ShipSystem.ShipCargoCompartment;
using ShipsForm.Logic.ShipSystem.ShipEngine;
using ShipsForm.Logic.ShipSystem.ShipNavigation;
using ShipsForm.Logic.ShipSystem.Ships;
using ShipsForm.SupportEntities.PatternObserver;
using System;
using System.Linq;

namespace ShipsForm.Logic.ShipSystem.Behaviour
{
    class CargoShipBehavior : ShipBehavior
    {
        private BoardCargo m_boardCargo;
        public BoardCargo BoardCargo { get { return m_boardCargo; } }
        public Action? OnArrived;
        public SupportEntities.Point? ConvoyPosition { get; private set; }

        public CargoShipBehavior(Ship thisShip, Engine eng, Navigation nav, BoardCargo bg)
        {
            m_ship = thisShip;
            m_engine = eng;
            m_navigation = nav;
            m_boardCargo = bg;
            State = new ShipWandersState();
            EventObservable.AddEventObserver(this);
        }

        public override CargoFraght? GetFraghtInfo()
        {
            if (m_ship is CargoShip cargoShip)
                return cargoShip.Fraght;
            return null;
        }

        public override void SetStartNode(GeneralNode nd)
        {
            m_navigation.FromNode = nd;
        }

        public void SetConvoyPosition(SupportEntities.Point p)
        {
            ConvoyPosition = p;
        }

        public void LeaveConvoy()
        {
            if (State is ShipRoutingInConvoyState)
            {
                ConvoyPosition = null;
                GoNextState();
            }    
        }

        /// <summary>
        /// Due to this function cargo ship can searches suitable fraght deal.
        /// </summary>
        /// <param name="fromNode">The node of departues.</param>
        /// <returns></returns>
        public bool LookingForFraght(GeneralNode fromNode)
        {
            if (m_ship is CargoShip cargoShip)
            {
                if (cargoShip.Fraght != null || State is not LookingForFraghtState)
                    return false;
                CargoFraght selectedFraght = (CargoFraght)FraghtMarket.Fraghts.FirstOrDefault(f => f.FromNode == fromNode && f is CargoFraght);
                if (selectedFraght != null)
                {
                    cargoShip.Fraght = selectedFraght;
                    selectedFraght.Charter(m_ship);
                    GoNextState();
                    return true;
                }
            }            
            return false;
        }

        public override void Update(Fraght ev)
        {
            LookingForFraght(m_navigation.FromNode);
        }
    }
}
