using ShipsForm.Logic.ShipSystem.ShipCargoCompartment;
using ShipsForm.Logic.ShipSystem.ShipNavigation;
using ShipsForm.Logic.ShipSystem.ShipEngine;
using ShipsForm.Logic.ShipSystem.Behaviour;
using ShipsForm.Logic.TilesSystem;
using ShipsForm.Logic.NodeSystem;
using ShipsForm.Logic.FraghtSystem;
using ShipsForm.Logic.ShipSystem.Behaviour.ShipStates;
using ShipsForm.Logic.ShipSystem.IceBreakerSystem;
using ShipsForm.Exceptions;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System;
using ShipsModern.SupportEntities;
using ShipsForm.Graphic;
using ShipsForm.SupportEntities.PatternObserver;

namespace ShipsForm.Logic.ShipSystem.Ships
{
    class CargoShip : Ship
    {
        private CargoShipBehavior m_behavior;
        private BoardCargo m_boardCargo;
        private CargoFraght? m_fraght;
        private Shell m_shell;
        private bool b_inConvoy { get { return m_behavior.State is ShipRoutingInConvoyState; } }

        public CargoFraght? Fraght { get { return m_fraght; } set {  m_fraght = value; } }
        public Shell Shell { get { return m_shell; } }
        public CargoShipBehavior Behavior { get { return m_behavior; } }
        public CargoShip(Node nodeToSpawnShip)
        {
            Id = Manager.GetGuiElementID();
            var data = Data.Configuration.Instance;
            if (data is null)
                throw new ConfigFileDoesntExistError();
            m_navigation = new Navigation();
            m_boardCargo = new BoardCargo();
            m_engine = new Engine();
            m_shell = new Shell(data.ShipIceResistLevel);
            m_behavior = new CargoShipBehavior(this, m_engine, m_navigation, m_boardCargo);          
            nodeToSpawnShip.ShipTryEnterInNode(this);
            EventObservable.NotifyObservers((IDrawable)this);
        }

        public override void Update()
        {
            if (m_navigation != null && m_engine != null)
            {
                m_navigation.ObserveMoving(m_engine.Running());
            }

            if (m_navigation.CurrentTile is null)
                return;
        }

        public override ImageSource GetSkin(int size)
        {
            var modelName = "CargoShip";
            return ((IDrawable)this).DownloadImage(modelName, size);
        }

        public override SupportEntities.Point? GetCurrentPoint()
        {
            if (m_navigation.CurrentTile is null)
                return null;
            Tile curTile = m_navigation.CurrentTile;
            SupportEntities.Point curPoint = new SupportEntities.Point(curTile.X, curTile.Y);
            return curPoint;
        }

        public override double GetRotation()
        {
            return m_navigation.CurrentRotation;
        }

        public override int GetSize()
        {
            var data = Data.Configuration.Instance;
            if (data is null)
                throw new Exception();
            return b_inConvoy ? data.ConvoyShipImageSize : data.ShipImageSize;
        }
    }
}
